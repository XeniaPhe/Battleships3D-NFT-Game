using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoOpenDockyard : MonoBehaviour
{
    public void OpenScene()
    {
        SceneManager.LoadScene("Select Dockyard Mode");
    }
}
