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
            SendGetPointsRequest(gameObject.name, "OnGetPointsRequestSuccess", "OnRequestFailed");
        }

        private void OnGetPointsRequestSuccess()
        {
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }

        private void OnRequestFailed()
        {

        }
    }
}