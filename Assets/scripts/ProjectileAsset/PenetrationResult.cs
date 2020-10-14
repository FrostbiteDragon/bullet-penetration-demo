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
        /// Returns true if the projectile sucssesfully penetrated the object
        /// </summary>
        public bool Passed { get; }
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

        public PenetrationResult(bool passed, float thickness, Collider collider, Vector3 entryPoint, Vector3 exitPoint)
        {
            Passed = passed;
            Thickness = thickness;
            Collider = collider;
            EntryPoint = entryPoint;
            ExitPoint = exitPoint;
        }
    }
}
