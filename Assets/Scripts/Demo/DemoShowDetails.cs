using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoShowDetails : MonoBehaviour
{
    public GameObject details;
    public GameObject card;

    public void ShowDetails()
    {
        bool myBool = !details.activeSelf;
        details.SetActive(myBool);
        card.SetActive(!myBool);
    }
}
