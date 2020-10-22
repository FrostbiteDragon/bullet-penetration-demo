using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectileAsset
{
    public class ProjectileController : MonoBehaviour
    {
        public GameObject prefab;

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
        private float _ricochetAngle;
        public float RicochetAngle
        {
            get => _ricochetAngle;
        }

        [SerializeField] 
        [HideInInspector]
        private FlightTrajectory _flightTrajectory;
        public FlightTrajectory FlightTrajectory
        {
            get => _flightTrajectory;
            protected set => _flightTrajectory = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _gravityMultiplier;
        public float GravityMultiplier
        {
            get => _gravityMultiplier;
        }

        [SerializeField]
        [HideInInspector]
        private float _speed;
        public float Speed
        {
            get => _speed;
            protected set => _speed = value;
        }

        protected virtual void OnPenetrationFailed(float angle, Vector3 point) { }
        protected virtual void OnPenetrationEnter(Vector3 entryPoint, Vector3 entryDirection) { }
        protected virtual void OnPenetrationExit(Vector3 exitPoint, Vector3 exitDirection) { }
        protected virtual void OnRicochet(Vector3 entryDirection, Vector3 exitDirection) { }


        private Vector3 startPosition;
        private float startTime;
        protected void Awake()
        {
            startPosition = transform.position;
            startTime = Time.time;
        }

        protected void FixedUpdate()
        {
            var nextPosition = Projectile.CalculateTrajectory(Time.time - startTime + Time.fixedDeltaTime, startPosition, transform.forward, GravityMultiplier, Speed);
            var nextNextPosition = Projectile.CalculateTrajectory(Time.time - startTime + Time.fixedDeltaTime * 2, startPosition, transform.forward, GravityMultiplier, Speed);
            if (ProjectileCSharp.CheckCollision(transform.position, nextPosition))
                if (PenetrationEnabled)
                {
                    var results = Projectile.GetPenetrations(new Vector3[] { transform.position, nextPosition, nextNextPosition });
                    Instantiate(prefab, transform.position, Quaternion.identity);
                    Instantiate(prefab, nextPosition, Quaternion.identity);
                    Instantiate(prefab, nextNextPosition, Quaternion.identity);
                    foreach (var result in results)
                    {
                        //if penetration is successful
                        if (result.thickness <= Penetration)
                        {
                            OnPenetrationEnter(result.entryHit.point, Vector3.zero);
                            if (result.exitHit != null)
                                OnPenetrationExit(result.exitHit.Value.point, Vector3.zero);
                        }
                        //if richochet
                        //else if ()
                        else
                            Destroy(gameObject);
                    }
                }

            transform.position = nextPosition;
        }
    }
}