using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectileAsset
{
    public class PenetrationResult
    {
        /// <summary>
        /// The thickness of the object penetrated
        /// </summary>
        public float Thickness { get; }
        /// <summary>
        /// The Collider that was penetrated
        /// </summary>
        public Collider Collider { get; }
        /// <summary>
        /// The point in world space where the projectile entered the object
        /// </summary>
        public Vector3 EntryPoint { get; }
        /// <summary>
        /// The point in world space where the projectile exited the object
        /// </summary>
        public Vector3 ExitPoint { get; }

        public PenetrationResult(float thickness, Collider collider, Vector3 entryPoint, Vector3 exitPoint)
        {
            Thickness = thickness;
            Collider = collider;
            EntryPoint = entryPoint;
            ExitPoint = exitPoint;
        }
    }
}
