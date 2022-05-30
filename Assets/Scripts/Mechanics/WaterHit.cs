using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHit : MonoBehaviour
{
    public Transform hitTransform;

    public void HitWater(Transform locationTransform)
    {
        GameObject shot = Instantiate(hitTransform.gameObject, locationTransform.position, hitTransform.rotation);
        shot.SetActive(true);
    }
}
