using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoBuyButton : MonoBehaviour
{

    public GameObject PopupPanel;

    public void OnpenPopup()
    {
        PopupPanel.SetActive(true);
    }
}
