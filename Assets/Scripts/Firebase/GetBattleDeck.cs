using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleShips.Firebase
{
    public class GetBattleDeck : MonoBehaviour
    {
        [DllImport("__Internal")] public static extern void SendGetBattleDeckRequest(string objectName, string callback, string fallback);

        public void Start()
        {
            SendGetBattleDeckRequest(gameObject.name, "OnRequestSuccess", "OnRequestFailed");
        }

        private void OnRequestSuccess()
        {
            SceneManager.LoadScene("Select Admiral", LoadSceneMode.Single);
        }

        private void OnRequestFailed()
        {

        }
    }
}