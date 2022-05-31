using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BattleShips.Firebase.Json;
using System.Runtime.InteropServices;

namespace BattleShips.Firebase.Load
{
    public class LoadPoints : MonoBehaviour
    {
        public TMP_Text xwsText, xpText;

        Points points;

        [DllImport("__Internal")] public static extern void SendLoadPointsRequest(string objectName, string callback, string fallback);

        public void Start()
        {
            SendLoadPointsRequest(gameObject.name, "OnRequestSuccess", "OnRequestFailed");
        }

        private void OnRequestSuccess(string jsonPoints)
        {
            points = JsonUtility.FromJson<Points>(jsonPoints);
            xwsText.text = points.xws.ToString();
            xpText.text = points.xp.ToString();
        }

        private void OnRequestFailed()
        {

        }
    }
}
