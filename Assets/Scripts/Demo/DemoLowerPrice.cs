using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoLowerPrice : MonoBehaviour
{
    public static int price = 100;

    public TMP_Text text;

    private void Start()
    {
        text.text = price.ToString();
    }

    public void UpdatePrice(int decreaseAmount)
    {
        if (price < decreaseAmount) return;
        price -= decreaseAmount;
        text.text = price.ToString();
    }
}
