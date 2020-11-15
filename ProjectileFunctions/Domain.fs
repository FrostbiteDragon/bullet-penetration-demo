namespace ProjectileAsset

open UnityEngine
open System

type PenetrationResults =
    { thickness : single
      dirrection : Vector3
      entryHit : RaycastHit
      exitHit : Nullable<RaycastHit> }

type Peramaters =
    { penetration : single
      speed : single 
      gravityMultiplier : single
      ricochetAngle : single 
      layerMask : int }
      

type ProjectileStart =
    { position : Vector3
      direction : Vector3 }

type HitResult =
| Ricochet of inVelocity : Vector3 * outVelocity : Vector3 * angle : single * hit : RaycastHit
| Penetration of velocity : Vector3 * entry : RaycastHit * exit : RaycastHit * thickness : single
| FailedPenetration of velocity : Vector3 * hit : RaycastHit * thickness : single
| NoContact

type ProjectileResult =
    { position : Vector3
      volocity : Vector3
      results : HitResult array }