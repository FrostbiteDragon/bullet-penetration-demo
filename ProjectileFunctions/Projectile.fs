namespace ProjectileAsset

open UnityEngine
open System

module Projectile =
    let CalculateTrajectory time (startPosition : Vector3) (startDirection : Vector3) gravityMultiplier speed = 
        let angle = 
            let positiveAngle = Vector3.Angle(Vector3(startDirection.x, 0.0f, startDirection.z), startDirection)
    
            match startDirection.y >= 0.0f with
            | true -> positiveAngle
            | false -> -positiveAngle 
    
        Vector3
            ( startPosition.x + startDirection.x * time * speed,
              speed * time * Mathf.Sin(angle * Mathf.Deg2Rad) - (4.9f * gravityMultiplier) * Mathf.Pow(time, 2.0f) + startPosition.y,
              startPosition.z + startDirection.z * time * speed )

    let GetPenetrations (points : Vector3[]) = 
        seq {
            for i in 0 .. points.Length - 2 do
                let forward =
                    Physics.RaycastAll(
                        points.[i],
                        points.[i + 1] - points.[i],
                        Vector3.Distance(points.[i], points.[i + 1]))
                    |> Array.sortBy(fun x -> x.distance)
    
                let backwards = 
                    Physics.RaycastAll(
                        points.[i + 1],
                        points.[i] - points.[i + 1],
                        Vector3.Distance(points.[i], points.[i + 1]))
                    |> Array.sortBy(fun x -> x.distance)
                    
                let extractPenetrationResults (entry : RaycastHit) (exit : Option<RaycastHit>) =
                    let thickness =
                        match exit with
                        |Some exit -> Vector3.Distance(entry.point, exit.point) * 1000.0f
                        |None -> Single.PositiveInfinity

                    { thickness = thickness
                      entryHit = entry
                      exitHit = exit |> Option.toNullable }
    
                yield forward
                |> Array.mapi (fun i x -> extractPenetrationResults x (backwards |> Array.tryItem (backwards.Length - i - 1)))
        }
        |> Seq.concat
        |> Array.ofSeq


