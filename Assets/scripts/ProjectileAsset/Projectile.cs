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
                    return position + direction * speed * Time.deltaTime;

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

        public static PenetrationResult CheckPenetration(Vector3 start, Vector3 end, float penetration)
        {
            var forward = Physics.RaycastAll(
                origin: start,
                direction: end - start,
                maxDistance: Vector3.Distance(start, end));

            var backward =
                Physics.RaycastAll(
                origin: end,
                direction: start - end,
                maxDistance: Vector3.Distance(start, end));

            var thickness = Vector3.Distance(forward.First().point, backward.Last().point) * 100;

            return new PenetrationResult(
                passed: penetration >= thickness,
                thickness: thickness,
                collider: forward.First().collider,
                entryPoint: forward.First().point,
                exitPoint: backward.Last().point);
        }
    }
}
