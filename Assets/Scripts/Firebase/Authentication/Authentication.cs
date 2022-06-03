using System.Runtime.InteropServices;
using UnityEngine;
using BattleShips.Firebase.Get;
using UnityEngine.SceneManagement;

namespace BattleShips.Firebase.Auth
{
    public class Authentication : MonoBehaviour
    {
        [DllImport("__Internal")] public static extern void SendAuthenticateRequest(string objectName, string callback, string fallback);

        public void Start() => SendAuthenticateRequest(gameObject.name, "OnRequestSuccess", "OnRequestFailed");

        private void OnRequestSuccess() => SceneManager.LoadScene("Main Menu");

        private void OnRequestFailed()
        {
            
        }
    }
}