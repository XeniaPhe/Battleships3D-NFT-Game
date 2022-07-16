using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoSetUpgradeMode : MonoBehaviour
{
    public void OpenMountWeaponScene()
    {
        SceneManager.LoadScene("Dockyard Mount Weapon");
    }

    public void OpenCombineCardsScene()
    {
        SceneManager.LoadScene("Dockyard Combine Cards");
    }
}
