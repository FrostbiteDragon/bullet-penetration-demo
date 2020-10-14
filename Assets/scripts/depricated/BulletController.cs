using UnityEngine;
using System;

public class BulletController : MonoBehaviour
{
    public float ActivationDistance;
    public float volocity;
    public float penetration;

    [SerializeField] private GameObject bulletMarkPrefab;
    [SerializeField] private GameObject sparksPrefab;
    
    private float startTime;
    private float InitialAngle;
    private Vector3 startPosition;
    private Vector3 moveDirection;

    void Start()
    {

        SetDirection(transform.position, transform.forward, volocity);

        nextPosition = PositionAt(0);
    }

    Action OnNextStep;
    Vector3 curPosition;
    Vector3 nextPosition;
    void FixedUpdate()
    {
        float nextStepTime = Time.time + Time.fixedDeltaTime - startTime;

        OnNextStep?.Invoke();
        OnNextStep = null;

        curPosition = nextPosition;
        nextPosition = PositionAt(nextStepTime);

        CheckContact(curPosition, nextPosition);

        void CheckContact(Vector3 start, Vector3 end)
        {
            Debug.DrawLine(start, end, Color.yellow, 6);
            if (Physics.Linecast(start, end, out RaycastHit hit))
            {
                Transform hitObject = hit.collider.transform;

                Vector3 localDirection = hitObject.InverseTransformDirection(curPosition - nextPosition);
                Vector3 localNormal = hitObject.InverseTransformDirection(hit.normal);
                float hitAngleH = Vector3.Angle(new Vector3(localDirection.x, localNormal.y, localDirection.z), localNormal);
                float hitAngleV = Vector3.Angle(new Vector3(localNormal.x, localDirection.y, localDirection.z), localNormal);

                //hit armour
                if (hit.collider.tag == "Armour")
                {
                    //make the hitAngle able to be negative 
                    hitAngleH = localDirection.x <= 0 ? hitAngleH : -hitAngleH;
                    hitAngleV = localDirection.y <= 0 ? hitAngleV : -hitAngleV;

                    float armourThickness = hit.collider.GetComponent<Armour>().thickness;
                    float relitiveArmourThickness = armourThickness / Mathf.Cos(hitAngleH * Mathf.Deg2Rad) / Mathf.Cos(hitAngleV * Mathf.Deg2Rad);
                    Debug.Log($"{armourThickness}mm => {relitiveArmourThickness}mm ({hitAngleH}/ {hitAngleV})");

                    //if ricochet
                    if (Mathf.Abs(hitAngleH) >= 70 || Mathf.Abs(hitAngleV) >= 70)
                    {
                        OnNextStep = () =>
                        {
                            //ricochet
                            transform.position = hit.point;
                            SetDirection(hit.point, Vector3.Reflect(moveDirection, hit.normal), 0.5f);
                        };
                    }
                    //if bullet penetrates
                    else if (relitiveArmourThickness <= penetration)
                    {
                        //spawn bullet mark
                        GameObject bulletMark = Instantiate(
                           bulletMarkPrefab,
                           hit.point + hit.normal * 0.01f,
                           Quaternion.LookRotation(hit.normal));

                        bulletMark.transform.parent = hitObject;
                        Destroy(bulletMark, 6);

                        Vector3 localEntryPoint = hitObject.InverseTransformDirection(hit.point);
                        Vector3 exitPoint = hitObject.TransformDirection(
                            new Vector3(
                                localEntryPoint.x + Mathf.Tan(hitAngleH * Mathf.Deg2Rad) * (armourThickness / 1000 / 2),
                                localEntryPoint.y + Mathf.Tan(hitAngleV * Mathf.Deg2Rad) * (armourThickness / 1000 / 2),
                                localEntryPoint.z - armourThickness / 1000 / 2));

                        CheckContact(exitPoint, nextPosition);
                        DebugPenetration();

                        void DebugPenetration()
                        {
                            Debug.DrawLine(hit.point, exitPoint, Color.cyan, 100);
                            Debug.DrawLine(curPosition, nextPosition, Color.yellow, 100);
                            Debug.DrawLine(exitPoint, nextPosition, Color.red, 100);
                            Debug.DrawRay(hit.point + hit.normal * 0.25f, -hit.normal, Color.magenta, 100);
                        }
                    }
                    else
                        DestroyBullet();
                }
                else if (hit.collider.tag == "Player")
                {
                    //Deal Damage
                    Destroy(gameObject);
                }
                else
                    DestroyBullet();

                void DestroyBullet()
                {
                    ParticleSystem sparks = Instantiate(
                          sparksPrefab,
                          hit.point,
                          Quaternion.LookRotation(Vector3.Reflect(moveDirection, hit.normal))).GetComponent<ParticleSystem>();

                    var main = sparks.main;
                    main.startSpeed = 8 * (1 - Mathf.Abs(hitAngleH) / 90);

                    var shape = sparks.shape;
                    shape.angle = Mathf.Abs(hitAngleH);

                    sparks.Play();
                    Destroy(sparks.gameObject, 3);

                    //Destroy Bullet
                    Destroy(gameObject);
                }
            }
            else
                OnNextStep = () => transform.position = curPosition;
        }
    }

    void SetDirection(Vector3 startPosition, Vector3 direction, float speed)
    {
        startTime = Time.time;
        this.startPosition = startPosition;
        moveDirection = direction * speed;
        volocity = speed;
        InitialAngle = Vector3.Angle(new Vector3(moveDirection.x, 0, moveDirection.z), moveDirection);
        InitialAngle = moveDirection.y >= 0 ? InitialAngle : -InitialAngle;

        nextPosition = PositionAt(0);
    }

    Vector3 PositionAt(float time)
    {
        return new Vector3(
            startPosition.x + moveDirection.x * time,
            volocity * time * Mathf.Sin(InitialAngle * Mathf.Deg2Rad) - 4.9f * Mathf.Pow(time, 2) + startPosition.y,
            startPosition.z + moveDirection.z * time);
    }
}