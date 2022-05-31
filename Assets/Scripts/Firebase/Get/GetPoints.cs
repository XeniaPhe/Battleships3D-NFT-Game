using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;
using BattleShips.Firebase.Json;

namespace BattleShips.Firebase.Get
{
    public class GetPoints : MonoBehaviour
    {
        public TMP_Text xwsText, xpText;

        Points points;

        [DllImport("__Internal")] public static extern void SendGetBattleDeckRequest(string objectName, string callback, string fallback);

        public void Start()
        {
            SendGetBattleDeckRequest(gameObject.name, "OnRequestSuccess", "OnRequestFailed");
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