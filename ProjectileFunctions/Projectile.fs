namespace ProjectileAsset

open UnityEngine

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


