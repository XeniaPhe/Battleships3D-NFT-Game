using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHit : MonoBehaviour
{
    public Transform hitTransform;

    public bool hit;

    private void Update()
    {
        if (hit)
        {
            GameObject shot = Instantiate(hitTransform.gameObject, hitTransform.position, hitTransform.rotation);
            shot.SetActive(true);
            hit = false;
        }
    }
}
