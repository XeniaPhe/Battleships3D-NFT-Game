using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHit : MonoBehaviour
{
    public Transform hits;

    public Transform fires;

    public int slot = 1;

    public bool hit;

    private void Update()
    {
        if (hit)
        {
            GameObject shot = Instantiate(hits.GetChild(slot - 1).gameObject, hits.GetChild(slot - 1).position, hits.GetChild(slot - 1).rotation);
            shot.SetActive(true);
            fires.GetChild(slot - 1).gameObject.SetActive(true);
            hit = false;
        }
    }
}
