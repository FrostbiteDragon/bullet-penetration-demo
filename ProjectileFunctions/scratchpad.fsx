#r """C:\Users\Cameron\.nuget\packages\unity3d.unityengine\2018.3.5.1\lib\UnityEngine.dll"""

open UnityEngine
open System

type PenetrationResults =
    { thickness : float32
      collider : Collider 
      entryPoint : Vector3
      exitpoint : Vector3 }

let CalculateTrajectory time (startPosition : Vector3) (startDirection : Vector3) gravityMultiplier speed = 
    let angle = 
        let positiveAngle = Vector3.Angle(Vector3(startDirection.x, 0f, startDirection.z), startDirection)

        match startDirection.y >= 0f with
        | true -> positiveAngle
        | false -> -positiveAngle 

    Vector3
        ( startPosition.x + startDirection.x * time * speed,
          speed * time * Mathf.Sin(angle * Mathf.Deg2Rad) - (4.9f * gravityMultiplier) * Mathf.Pow(time, 2f) + startPosition.y,
          startPosition.z + startDirection.z * time * speed )
(*

    public static Vector3 CalculateTrajectory(float time, Vector3 startPosition, Vector3 direction, float gravityMultiplier, float speed)
    {
        float angle = Vector3.Angle(new Vector3(direction.x, 0, direction.z), direction);
        angle = direction.y >= 0 ? angle : -angle;

        return new Vector3(
            x: startPosition.x + direction.x * time * speed,
            y: speed * time * Mathf.Sin(angle * Mathf.Deg2Rad) - (4.9f * gravityMultiplier) * Mathf.Pow(time, 2) + startPosition.y,
            z: startPosition.z + direction.z * time * speed 
            );
    }
    
*)

let GetPenetrations (points : Vector3[]) = 
    seq {
        for i in 0 .. points.Length - 1 do
            let forward =
                Physics.RaycastAll(
                    points.[0],
                    points.[1] - points.[0],
                    Vector3.Distance(points.[0], points.[1]))
                |> Array.sortBy(fun x -> x.distance)

            let backwards = 
                Physics.RaycastAll(
                    points.[0],
                    points.[0] - points.[1],
                    Vector3.Distance(points.[0], points.[1]))
                |> Array.sortBy(fun x -> x.distance)
                
            let extractPenetrationResuslts (entry : RaycastHit) (exit : RaycastHit) =
                let thickness = Single.PositiveInfinity
                { thickness = thickness
                  collider = entry.collider
                  entryPoint = entry.point
                  exitpoint = exit.point }
                    

            yield forward
            |> Array.mapi (fun x i -> extractPenetrationResuslts x)
    }
    |> Seq.append Seq.empty
    |> Array.ofSeq

    

(*
public static PenetrationResult[] GetPenetrations(Vector3[] points)
{
    var results = new List<PenetrationResult>();
    for (int i = 0; i < points.Length - 1; i++)
    {
        var forward =
            Physics.RaycastAll(
            origin: points[0],
            direction: points[1] - points[0],
            maxDistance: Vector3.Distance(points[0], points[1])).OrderBy(x => x.distance);

        var backward =
            Physics.RaycastAll(
            origin: points[1],
            direction: points[0] - points[1],
            maxDistance: Vector3.Distance(points[0], points[1])).OrderBy(x => x.distance).ToArray();

        results.AddRange(forward.Select((entry, j) =>
        {
            var exit = backward.ToList().ElementAtOrDefault(backward.Length - 1 - j);
            var thickness = float.PositiveInfinity;
            if (exit.point != null)
                thickness = Vector3.Distance(entry.point, exit.point) * 1000;

            return new PenetrationResult(
                thickness: thickness,
                collider: entry.collider,
                entryPoint: entry.point,
                exitPoint: exit.point
            );
        }));
    }
    return results.ToArray();
}
*)