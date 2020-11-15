namespace ProjectileAsset

open UnityEngine
open System

module ProjectileNew =

    let CalculateTrajectory position direction speed (penetration : single) gravityMultiplier ricochetAngle layerMask = 
        let GetPosition (position : Vector3) (direction : Vector3) time = 
            let angle = 
                let positiveAngle = Vector3.Angle(Vector3(direction.x, 0.0f, direction.z), direction)
                match direction.y >= 0.0f with
                | true -> positiveAngle
                | false -> -positiveAngle 
        
            Vector3
                ( position.x + direction.x * time * speed,
                  speed * time * Mathf.Sin(angle * Mathf.Deg2Rad) - (4.9f * gravityMultiplier) * Mathf.Pow(time, 2.0f) + position.y,
                  position.z + direction.z * time * speed )

        let rec GetResults (startPoint : Vector3) (endPoint : Vector3) distanceLeft projectileResult =
            let mutable contact = RaycastHit()
            let hitObject = Physics.Linecast(startPoint, endPoint, &contact, layerMask)

            let result =
                match hitObject with
                | false -> NoContact
                | true ->
                    let inDirection = (endPoint - startPoint).normalized
                    let angle = -(90.0f - Vector3.Angle(contact.normal, inDirection))

                    if angle <= ricochetAngle then
                        let outDirection = Vector3.Reflect(inDirection, contact.normal).normalized
                        Ricochet(outDirection, inDirection, angle, contact)
                    else
                        let hits =  
                            let backCastStartPoint = Vector3(contact.point.x + inDirection.x * 10f, contact.point.y + inDirection.y * 10f, contact.point.z + inDirection.z * 10f)
                            Physics.RaycastAll(
                                backCastStartPoint,
                                contact.point - backCastStartPoint,
                                Vector3.Distance(contact.point, backCastStartPoint),
                                layerMask)
                            |> Array.sortBy(fun x -> x.distance)
                        
                        if hits.Length = 0 then
                            FailedPenetration(inDirection, contact, Single.PositiveInfinity)
                        else
                            let exitHit = Array.last(hits)
                            let thickness = Vector3.Distance(contact.point, exitHit.point) * 1000f

                            if thickness < penetration
                                then Penetration(inDirection, contact, exitHit, thickness)
                                else FailedPenetration(inDirection, contact, Single.PositiveInfinity)

            match result with
            | Ricochet (outDirection, _, _, hit) -> 
                let distanceLeft = distanceLeft - hit.distance
                let newEndPoint = Vector3(hit.point.x + outDirection.x * distanceLeft, hit.point.y + outDirection.y * distanceLeft, hit.point.z + outDirection.z * distanceLeft)
                let newProjectileResult = 
                    { position = hit.point
                      volocity = outDirection * speed
                      results = projectileResult.results |> Array.append [|result|] }
                GetResults hit.point newEndPoint distanceLeft newProjectileResult

            | Penetration (direction, entry, exit, _) -> //{ projectileResult with results = projectileResult.results |> Array.append [|result|] }
                let newDistanceLeft = distanceLeft - Vector3.Distance(startPoint, exit.point)
                GetResults 
                    exit.point
                    (Vector3(exit.point.x + direction.x * newDistanceLeft, exit.point.y + direction.y * newDistanceLeft, exit.point.z + direction.z * newDistanceLeft) )
                    newDistanceLeft 
                    { position = entry.point
                      volocity = direction * speed
                      results = projectileResult.results |> Array.append [|result|] }

            | FailedPenetration (_,hit,_) ->
                { projectileResult with 
                    position = hit.point
                    results = projectileResult.results |> Array.append [|result|] }
            | NoContact -> 
                { projectileResult with 
                    position = endPoint 
                    volocity = (endPoint - startPoint) * speed}

        let startPoint = GetPosition position direction 0f
        let endPoint = GetPosition position direction Time.fixedDeltaTime
        GetResults startPoint endPoint (Vector3.Distance(startPoint, endPoint)) { position = startPoint; volocity = direction * speed; results = [||]; }
