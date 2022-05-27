using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFire : MonoBehaviour
{
    public Transform shots;

    public int slot = 1;

    public bool fire;

    private void Update()
    {
        if(fire)
        {
            GameObject shot = Instantiate(shots.GetChild(slot - 1).gameObject, shots.GetChild(slot - 1).position, shots.GetChild(slot - 1).rotation);
            shot.SetActive(true);
            fire = false;
        }
    }
}
