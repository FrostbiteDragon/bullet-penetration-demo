namespace ProjectileAsset

open UnityEngine
open System

type PenetrationResults =
    { thickness : single
      dirrection : Vector3
      entryHit : RaycastHit
      exitHit : Nullable<RaycastHit> }