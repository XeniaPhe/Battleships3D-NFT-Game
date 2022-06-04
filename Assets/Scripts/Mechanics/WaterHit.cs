using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHit : MonoBehaviour
{
    public Transform hitTransform;

    public void HitWater(Vector3 pos)
    {
        GameObject shot = Instantiate(hitTransform.gameObject, pos, hitTransform.rotation);
        shot.SetActive(true);
    }
}
