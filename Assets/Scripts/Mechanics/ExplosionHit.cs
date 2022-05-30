using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHit : MonoBehaviour
{
    public Transform hitTransform;

    public void HitExplosion(Transform locationTransform)
    {
        GameObject shot = Instantiate(hitTransform.gameObject, locationTransform.position, hitTransform.rotation);
        shot.SetActive(true);
    }
}
