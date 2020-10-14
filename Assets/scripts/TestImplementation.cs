using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ProjectileAsset;

public class TestImplementation : ProjectileController
{
    public GameObject bulletmarkPrefab;

    protected override void OnPenetrationEnter(Vector3 entryPoint, Vector3 entryDirection)
    {
        Instantiate(bulletmarkPrefab, entryPoint, Quaternion.identity);
    }
}
