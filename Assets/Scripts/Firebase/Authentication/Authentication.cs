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
            SendAuthenticateRequest(gameObject.name, "OnAuthenticateRequestSuccess", "OnRequestFailed");
        }

        private void OnAuthenticateRequestSuccess()
        {
            gameObject.GetComponent<GetPoints>().Get();
        }

        private void OnRequestFailed()
        {
            
        }
    }
}