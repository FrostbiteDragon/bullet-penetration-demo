using System;
using UnityEngine;

namespace ProjectileAsset
{
    public class ProjectileController : MonoBehaviour
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

        protected virtual void OnPenetrationFailed(RaycastHit hit, Vector3 dirrection) { }
        protected virtual void OnPenetrationEnter(RaycastHit entry, Vector3 dirrection) { }
        protected virtual void OnPenetrationExit(RaycastHit exit, Vector3 dirrection) { }
        protected virtual void OnRicochet(Vector3 entryDirection, Vector3 exitDirection, RaycastHit hit) { }


        private ProjectileStart projectileStart;
        private ProjectileResult result;
        protected virtual void Awake()
        {
            //projectileStart = new ProjectileStart(transform.position, transform.forward, Time.fixedTime);
            Instantiate(prefab, transform.position, Quaternion.identity);
            SendMessage("OnPenetration");
            //result = ProjectileNew.CalculateTrajectory(projectileStart, Speed, Penetration, GravityMultiplier, RicochetAngle, LayerMask, new Tuple<Vector3, Vector3>[0]);

            _ricochetAngle = 90f;
        }

        protected void FixedUpdate()
        {
            result = ProjectileNew.CalculateTrajectory(
                result?.startInfo ?? new ProjectileStart(transform.position, transform.forward, Time.fixedTime),
                Speed,
                Penetration,
                GravityMultiplier,
                RicochetAngle,
                LayerMask,
                result?.casts ?? new Tuple<Vector3, Vector3>[0]);

            foreach (var hit in result.results)
            {
                switch (hit)
                {
                    case HitResult.Ricochet _:
                        Debug.Log("Ricohet!");
                        break;
                    case HitResult.Penetration _:
                        break;
                }
            }
            //DEBUG
            foreach (var cast in result.casts)
            {
                var (start, end) = cast;
                Debug.DrawLine(start, end);
            }

            Instantiate(prefab, result.position, Quaternion.identity);

            //DEBUG

            transform.position = result.position;
        }
    }
}
