using System.Runtime.InteropServices;
using UnityEngine;
using BattleShips.Firebase.Get;

namespace BattleShips.Firebase.Auth
{
    public class Authentication : MonoBehaviour
    {
        [DllImport("__Internal")] public static extern void SendAuthenticateRequest(string objectName, string callback, string fallback);

        public void Start()
        {
            SendAuthenticateRequest(gameObject.name, "OnRequestSuccess", "OnRequestFailed");
        }

        private void OnRequestSuccess()
        {
            gameObject.GetComponent<GetPoints>().Get();
        }

        private void OnRequestFailed()
        {
            
        }
    }
}