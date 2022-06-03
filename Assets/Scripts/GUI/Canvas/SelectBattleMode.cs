using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleShips.GUI.Canvas
{
    public class SelectBattleMode
    {
        public void FreeToPlayButtonClick()
        {
            SceneManager.LoadScene("Loading Deck", LoadSceneMode.Single);
        }
    }
}