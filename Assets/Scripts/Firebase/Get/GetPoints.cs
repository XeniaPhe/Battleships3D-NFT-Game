using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleShips.Firebase.Get
{
    public class GetPoints : MonoBehaviour
    {
        [DllImport("__Internal")] public static extern void SendGetPointsRequest(string objectName, string callback, string fallback);

        public void Get()
        {
            SendGetPointsRequest(gameObject.name, "OnRequestSuccess", "OnRequestFailed");
        }

        private void OnRequestSuccess(string jsonPoints)
        {
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }

        private void OnRequestFailed()
        {

        }
    }
}