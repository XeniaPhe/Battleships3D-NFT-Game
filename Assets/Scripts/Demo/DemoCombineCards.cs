using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoCombineCards : MonoBehaviour
{
    public GameObject defaultSlot;

    public GameObject slot1;
    public GameObject slot2;

    public void CombineCards()
    {
        slot1.GetComponent<Image>().sprite = defaultSlot.GetComponent<Image>().sprite;
        slot2.GetComponent<Image>().sprite = defaultSlot.GetComponent<Image>().sprite;
    }
}
