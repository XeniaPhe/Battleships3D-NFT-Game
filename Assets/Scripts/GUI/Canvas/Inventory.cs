using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleShips.GUI.Canvas
{
    public class Inventory : MonoBehaviour
    {
        public void MainMenuReturnButtonClick()
        {
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }
    }
}