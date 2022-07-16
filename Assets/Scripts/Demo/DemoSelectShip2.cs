using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoSelectShip2 : MonoBehaviour
{
    public GameObject slots;

    public void SelectShip()
    {
        if (!slots.GetComponent<DemoShipSlots>().isSlot1Set){
            slots.GetComponent<DemoShipSlots>().slot1.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
            slots.GetComponent<DemoShipSlots>().isSlot1Set = true;
        }
        else
        {
            slots.GetComponent<DemoShipSlots>().slot2.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
            slots.GetComponent<DemoShipSlots>().isSlot2Set = true;
        }
    }
}
