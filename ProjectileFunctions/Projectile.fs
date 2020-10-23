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
        let validate (forward : RaycastHit[]) (backwards : RaycastHit[]) =
            match forward.Length = backwards.Length with
            |true -> backwards
            |false ->
                let nextBackwards =
                    Physics.RaycastAll(
                        points.[2],
                        points.[1] - points.[2],
                        Vector3.Distance(points.[1], points.[2]))
                    |> Array.sortBy(fun x -> x.distance)
                    
                match nextBackwards.Length > 0 && forward.[forward.Length - 1].collider = nextBackwards.[nextBackwards.Length - 1].collider with
                |true -> backwards
                            |> List.ofArray
                            |> List.append [nextBackwards.[nextBackwards.Length - 1]]
                            |> Array.ofList
                | false -> backwards

        let forward =
            Physics.RaycastAll(
                points.[0],
                points.[1] - points.[0],
                Vector3.Distance(points.[0], points.[1]))
            |> Array.sortBy(fun x -> x.distance)
    
        let backwards = 
            Physics.RaycastAll(
                points.[1],
                points.[0] - points.[1],
                Vector3.Distance(points.[0], points.[1]))
            |> Array.sortBy(fun x -> x.distance)
            |> validate forward

        let extractPenetrationResults (entry : RaycastHit) (exit : Option<RaycastHit>) =
            let thickness =
                match exit with
                |Some exit -> Vector3.Distance(entry.point, exit.point) * 1000.0f
                |None -> Single.PositiveInfinity

            { thickness = thickness
              entryHit = entry
              exitHit = exit |> Option.toNullable }
    
        forward
        |> Array.mapi (fun i x -> extractPenetrationResults x (backwards |> Array.tryItem (backwards.Length - i - 1)))


