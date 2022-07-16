using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoMountTheWeapon : MonoBehaviour
{
    public GameObject ship;
    public GameObject weapon1;
    public GameObject weapon2;
    
    public void MountWeapon()
    {
        ship.GetComponent<Image>().sprite = weapon2.GetComponent<Image>().sprite;
        weapon1.GetComponent<Image>().sprite = weapon2.GetComponent<Image>().sprite;
    }
}
