namespace ProjectileAsset

open UnityEngine
open System

type PenetrationResults =
    { thickness : single
      entryHit : RaycastHit
      exitHit : Nullable<RaycastHit> }