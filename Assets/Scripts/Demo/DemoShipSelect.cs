using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoShipSelect : MonoBehaviour
{
    public void UpgradeShip()
    {
        DemoShipUpgradeShip.objectSelf.gameObject.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
    }
}
