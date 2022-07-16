using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoWeaponSelect : MonoBehaviour
{
    public void UpgradeWeapon()
    {
        DemoShipUpgradeWeapon.objectSelf.gameObject.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
    }
}
