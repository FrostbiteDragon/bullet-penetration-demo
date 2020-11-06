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
      direction : Vector3 
      time : single }

type HitResult =
| Ricochet of outDirection : Vector3 * inDirection : Vector3 * angle : single * hit : RaycastHit
| Penetration of bool
| FailedPenetration of bool
| NoContact

type ProjectileResult =
    { position : Vector3
      startInfo : ProjectileStart
      results : HitResult array }