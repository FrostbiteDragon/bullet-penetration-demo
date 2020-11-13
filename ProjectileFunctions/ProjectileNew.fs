namespace ProjectileAsset

open UnityEngine
open System

module ProjectileNew =

    let CalculateTrajectory (startInfo : ProjectileStart ) speed (penetration : single) gravityMultiplier ricochetAngle layerMask = 
        let GetPosition startInfo time = 
            let angle = 
                let positiveAngle = Vector3.Angle(Vector3(startInfo.direction.x, 0.0f, startInfo.direction.z), startInfo.direction)
                match startInfo.direction.y >= 0.0f with
                | true -> positiveAngle
                | false -> -positiveAngle 
        
            Vector3
                ( startInfo.position.x + startInfo.direction.x * time * speed,
                  speed * time * Mathf.Sin(angle * Mathf.Deg2Rad) - (4.9f * gravityMultiplier) * Mathf.Pow(time, 2.0f) + startInfo.position.y,
                  startInfo.position.z + startInfo.direction.z * time * speed )

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
                            let backCastStartPoint = Vector3(contact.point.x + inDirection.x, contact.point.y + inDirection.y, contact.point.z + inDirection.z) * 10f
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
                            let thickness = Vector3.Distance(contact.point, exitHit.point)

                            if thickness < penetration
                                then Penetration(inDirection, contact, exitHit, thickness)
                                else FailedPenetration(inDirection, contact, Single.PositiveInfinity)

            match result with
            | Ricochet (outDirection, _, _, hit) -> 
                let distanceLeft = distanceLeft - hit.distance
                let newEndPoint = Vector3(hit.point.x + outDirection.x * distanceLeft, hit.point.y + outDirection.y * distanceLeft, hit.point.z + outDirection.z * distanceLeft)
                let newProjectileResult = 
                    { projectileResult with
                        position = hit.point
                        startInfo = { position = hit.point; direction = outDirection; }
                        results = projectileResult.results |> Array.append [|result|] }
                GetResults hit.point newEndPoint distanceLeft newProjectileResult

            | Penetration (direction, entry, exit, _) -> //{ projectileResult with results = projectileResult.results |> Array.append [|result|] }
                let newDistanceLeft = distanceLeft - Vector3.Distance(startPoint, exit.point)
                GetResults 
                    exit.point
                    (Vector3(exit.point.x + direction.x, exit.point.y + direction.y, exit.point.z + direction.z) * newDistanceLeft)
                    newDistanceLeft 
                    { projectileResult with position =  entry.point; results = projectileResult.results |> Array.append [|result|] }

            | FailedPenetration _ ->
                { projectileResult with results = projectileResult.results |> Array.append [|result|] }
            | NoContact -> 
                { projectileResult with 
                    position = endPoint
                    startInfo = { position = endPoint; direction = -(startPoint - endPoint).normalized }}

            //| _ -> { projectileResult with results = projectileResult.results |> Array.append [|result|] }

        let startPoint = GetPosition startInfo (0f)
        let endPoint = GetPosition startInfo (Time.fixedDeltaTime)
        GetResults startPoint endPoint (Vector3.Distance(startPoint, endPoint)) { position = startPoint; startInfo = startInfo; results = [||]; }
