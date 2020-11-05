using UnityEngine;
using ProjectileAsset;
using UnityEngine.UI;

public class TestImplementation : ProjectileController
{
    public GameObject bulletmarkPrefab;
    private Color color;

    protected override void Awake()
    {
        base.Awake();
        color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }

    protected override void OnPenetrationEnter(RaycastHit entry, Vector3 dirrection)
    {
        var mark = Instantiate(bulletmarkPrefab, entry.point + entry.normal * 0.01f, Quaternion.LookRotation(entry.normal), entry.transform);
        mark.GetComponent<SpriteRenderer>().color = color;
    }

    protected override void OnPenetrationExit(RaycastHit exit, Vector3 dirrection)
    {
        Instantiate(bulletmarkPrefab, exit.point + exit.normal * 0.01f, Quaternion.LookRotation(exit.normal), exit.transform).GetComponent<SpriteRenderer>().color = color;
    }

    protected override void OnPenetrationFailed(RaycastHit hit, Vector3 dirrection)
    {
        Destroy(gameObject);
    }
}
