using System;
using UnityEngine;

namespace ProjectileAsset
{
    public abstract class ProjectileController : MonoBehaviour
    {
        //DEBUG
        public GameObject prefab;

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
        private float _penetration;
        public float Penetration
        {
            get => _penetration;
            protected set => _penetration = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _ricochetAngle = 5;
        public float RicochetAngle
        {
            get => _ricochetAngle;
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

        protected virtual void OnPenetrationEnter(RaycastHit entry, Vector3 velocity, float thickness) { }
        protected virtual void OnPenetrationExit(RaycastHit exit, Vector3 velocity) { }
        protected virtual void OnPenetrationFailed(RaycastHit hit, Vector3 velocity) { }
        protected virtual void OnRicochet(float inAngle, Vector3 entryDirection, Vector3 exitDirection, RaycastHit hit) { }


        private ProjectileResult result;
        protected virtual void Awake()
        {
            //projectileStart = new ProjectileStart(transform.position, transform.forward, Time.fixedTime);
            //Instantiate(prefab, transform.position, Quaternion.identity);
            //result = ProjectileNew.CalculateTrajectory(projectileStart, Speed, Penetration, GravityMultiplier, RicochetAngle, LayerMask, new Tuple<Vector3, Vector3>[0]);

            _ricochetAngle = 5f;
        }

        protected void FixedUpdate()
        {
            result = ProjectileNew.CalculateTrajectory(
                transform.position,
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
            transform.position = result.position;
        }
    }
}
