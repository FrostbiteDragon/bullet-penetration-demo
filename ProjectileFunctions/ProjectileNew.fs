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
                    let inDirection = (startPoint - endPoint).normalized
                    let angle = 90.0f - Vector3.Angle(contact.normal, inDirection)

                    if angle <= ricochetAngle then
                        let outDirection = -Vector3.Reflect(inDirection, contact.normal).normalized
                        Ricochet(outDirection, inDirection, angle, contact)
                    else 
                        let thickness = penetration

                        match thickness >= penetration with
                        | true -> Penetration(true)
                        | false -> FailedPenetration(true)

            match result with
            | Ricochet (outDirection, _, _, hit) -> 
                let projectileResult = 
                    { position = hit.point
                      startInfo = { position = hit.point; direction = outDirection; time = Time.time }
                      results = projectileResult.results |> Array.append [|result|] }
                let endPoint = Vector3(hit.point.x + outDirection.x * distanceLeft, hit.point.y + outDirection.y * distanceLeft, hit.point.z + outDirection.z * distanceLeft)
                GetResults hit.point endPoint (distanceLeft - hit.distance) projectileResult
            //| Penetration -> GetResults startPoint endPoint distanceLeft projectileResults
            | NoContact -> projectileResult
            | _ -> { projectileResult with results = projectileResult.results |> Array.append [|result|] }

        let startPoint = GetPosition startInfo (Time.time - startInfo.time)
        let endPoint = GetPosition startInfo (Time.time - startInfo.time + Time.fixedDeltaTime)
        GetResults startPoint endPoint (Vector3.Distance(startPoint, endPoint)) {position = startPoint; startInfo = startInfo; results = [||]}
