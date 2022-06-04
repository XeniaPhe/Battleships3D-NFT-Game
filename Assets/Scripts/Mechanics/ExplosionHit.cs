using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHit : MonoBehaviour
{
    public Transform hitTransform;

    public void HitExplosion(Vector3 pos)
    {
        GameObject shot = Instantiate(hitTransform.gameObject, pos, hitTransform.rotation);
        shot.SetActive(true);
    }
}
