namespace ProjectileAsset

open UnityEngine
open System

module ProjectileNew =

    let CalculateTrajectory (startInfo : ProjectileStart ) speed (penetration : single) gravityMultiplier ricochetAngle layerMask casts = 
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
            let color = Color(single <| Random.Range(0f, 1f), single <| Random.Range(0f, 1f), single <| Random.Range(0f, 1f), 1f)

            let mutable contact = RaycastHit()
            //DEBUG
            let hitObject = Physics.Linecast(startPoint, endPoint, &contact, layerMask)
            let projectileResult = { projectileResult with 
                                        casts = projectileResult.casts |> Array.append [|(startPoint, endPoint)|] }
            //DEBUG

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
                let distanceLeft = distanceLeft - hit.distance
                let newEndPoint = Vector3(hit.point.x + outDirection.x * distanceLeft, hit.point.y + outDirection.y * distanceLeft, hit.point.z + outDirection.z * distanceLeft)
                let newProjectileResult = 
                    { projectileResult with
                        position = hit.point
                        startInfo = { position = hit.point; direction = outDirection; time = Time.fixedTime }
                        results = projectileResult.results |> Array.append [|result|] }
                GetResults hit.point newEndPoint distanceLeft newProjectileResult

            //| Penetration -> GetResults startPoint endPoint distanceLeft projectileResults
            | NoContact -> 
                { projectileResult with 
                    position = endPoint
                    startInfo = { position = endPoint; direction = -(startPoint - endPoint).normalized; time = Time.fixedTime }}

            //| _ -> { projectileResult with results = projectileResult.results |> Array.append [|result|] }

        let startPoint = GetPosition startInfo (0f)
        let endPoint = GetPosition startInfo (Time.fixedDeltaTime)
        GetResults startPoint endPoint (Vector3.Distance(startPoint, endPoint)) {position = startPoint; startInfo = startInfo; results = [||]; casts = casts}
