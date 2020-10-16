using ICSharpCode.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectileAsset
{
    public enum FlightTrajectory { Linear, Perabolic }

    public static class Projectile
    {
        public static Vector3 CalculateTrajectory(float time, Vector3 startPosition, Vector3 position, Vector3 direction, FlightTrajectory trajectory, float speed)
        {
            switch (trajectory)
            {
                case FlightTrajectory.Linear:
                    return startPosition + direction * time * speed;

                case FlightTrajectory.Perabolic:
                    float angle = Vector3.Angle(new Vector3(direction.x, 0, direction.z), direction);
                    angle = direction.y >= 0 ? angle : -angle;

                    return new Vector3(
                            x: startPosition.x + direction.x * time * speed,
                            y: speed * time * Mathf.Sin(angle * Mathf.Deg2Rad) - 4.9f * Mathf.Pow(time, 2) + startPosition.y,
                            z: startPosition.z + direction.z * time * speed
                        );

                default: return position;
            }
        }

        public static bool CheckCollision(Vector3 position, Vector3 nextPosition)
        {
            return Physics.Linecast(position, nextPosition);
        }

        public static PenetrationResult[] GetPenetrations(Vector3[] points)
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
            
            return forward.Select((entry, i) =>
            {
                var exit = backward[backward.Length - 1 - i];
                return new PenetrationResult(
                    thickness: Vector3.Distance(entry.point, exit.point) * 1000,
                    collider: entry.collider,
                    entryPoint: entry.point,
                    exitPoint: exit.point
                );
            }).ToArray();
        }
    }
}
