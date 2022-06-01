using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleShips.Firebase.Get
{
    public class GetBattleDeck : MonoBehaviour
    {
        [DllImport("__Internal")] public static extern void SendGetBattleDeckRequest(string objectName, string callback, string fallback);

        public void Start()
        {
            SendGetBattleDeckRequest(gameObject.name, "OnGetBattleDeckRequestSuccess", "OnRequestFailed");
        }

        private void OnGetBattleDeckRequestSuccess()
        {
            SceneManager.LoadScene("3D", LoadSceneMode.Single);
        }

        private void OnRequestFailed()
        {

        }
    }
}