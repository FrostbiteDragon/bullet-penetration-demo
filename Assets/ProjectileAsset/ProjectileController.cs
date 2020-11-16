using System;
using UnityEngine;

namespace ProjectileAsset
{
    public abstract class ProjectileController : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private LayerMask _layerMask;
        public LayerMask LayerMask
        {
            get => _layerMask;
            protected set => _layerMask = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _penetrationEnabled;
        public bool PenetrationEnabled
        {
            get => _penetrationEnabled;
            protected set => _penetrationEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _penetration = 50;
        public float Penetration
        {
            get => PenetrationEnabled ? _penetration : 0;
            protected set => _penetration = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _ricochetEnabled;
        public bool RicochetEnabled
        {
            get => _ricochetEnabled;
            protected set => _ricochetEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _ricochetAngle = 5;
        public float RicochetAngle
        {
            get => RicochetEnabled ? _ricochetAngle : 0;
            protected set => _ricochetAngle = value; 
        }

        [SerializeField]
        [HideInInspector]
        private float _gravityMultiplier = 1;
        public float GravityMultiplier
        {
            get => _gravityMultiplier;
        }

        [SerializeField]
        [HideInInspector]
        private float _speed = 300;
        public float Speed
        {
            get => _speed;
            protected set => _speed = value;
        }

        public Vector3 Velocity { get; private set; }

        protected virtual void OnPenetrationEnter(RaycastHit entry, Vector3 velocity, float thickness) { }
        protected virtual void OnPenetrationExit(RaycastHit exit, Vector3 velocity) { }
        protected virtual void OnPenetrationFailed(RaycastHit hit, Vector3 velocity) { }
        protected virtual void OnRicochet(float inAngle, Vector3 entryDirection, Vector3 exitDirection, RaycastHit hit) { }

        private ProjectileResult result;
        private Vector3 targetPosition;

        private void Awake()
        {
            targetPosition = transform.position;
        }

        protected virtual void FixedUpdate()
        {
            result = ProjectileNew.CalculateTrajectory(
                targetPosition,
                result?.velocity.normalized ?? transform.forward,
                result?.velocity.magnitude ?? Speed,
                Penetration,
                GravityMultiplier,
                RicochetAngle,
                LayerMask);

            foreach (var hit in result.results)
            {
                switch (hit)
                {
                    case HitResult.Ricochet ricochet:
                        OnRicochet(ricochet.angle, ricochet.inVelocity, ricochet.outVelocity, ricochet.hit);
                        break;
                    case HitResult.Penetration penetration:
                        OnPenetrationEnter(penetration.entry, penetration.velocity, penetration.thickness);
                        OnPenetrationExit(penetration.exit, penetration.velocity);
                        break;
                    case HitResult.FailedPenetration failedPen:
                        OnPenetrationFailed(failedPen.hit, failedPen.velocity);
                        break;
                }
            }
            targetPosition = result.position;
            Velocity = result.velocity;
        }
    }
}
