using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleShips.GUI.Canvas
{
    public class MainMenu : MonoBehaviour
    {
        public void InventoryButtonClick()
        {
            SceneManager.LoadScene("Inventory", LoadSceneMode.Single);
        }

        public void PlayButtonClick()
        {
            SceneManager.LoadScene("Select Battle Mode", LoadSceneMode.Single);
        }

        public void ShopButtonClick()
        {

        }

        public void DockyardButtonClick()
        {

        }
    }
}