using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoBuyPopupButton : MonoBehaviour
{
    public DemoLowerPrice demoLowerPrice;
    public void BuyItem()
    {
        demoLowerPrice.UpdatePrice(20);
        gameObject.SetActive(false);
    }

    public void ClosePopupPanel()
    {
        gameObject.SetActive(false);
    }
}
