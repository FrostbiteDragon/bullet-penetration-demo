using UnityEngine;
using ProjectileAsset;

public class TestImplementation : ProjectileController
{
    public GameObject bulletmarkPrefab;
    public Color color;

    private void Start()
    {
        color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }

    protected override void OnPenetrationEnter(Vector3 entryPoint, Vector3 entryDirection)
    {
        Instantiate(bulletmarkPrefab, entryPoint, Quaternion.identity).GetComponent<SpriteRenderer>().color = color;
    }

    protected override void OnPenetrationExit(Vector3 exitPoint, Vector3 exitDirection)
    {
        Instantiate(bulletmarkPrefab, exitPoint, Quaternion.identity).GetComponent<SpriteRenderer>().color = color;
    }
}
