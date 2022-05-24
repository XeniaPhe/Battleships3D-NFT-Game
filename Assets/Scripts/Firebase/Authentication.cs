using System.Runtime.InteropServices;
using UnityEngine;

namespace BattleShips.Firebase
{
    public class Authentication : MonoBehaviour
    {
        public GameObject nextButtonGameObject;

        [DllImport("__Internal")] public static extern void SendAuthenticateRequest(string objectName, string callback, string fallback);

        public void Start()
        {
            SendAuthenticateRequest(gameObject.name, "OnRequestSuccess", "OnRequestFailed");
        }

        private void OnRequestSuccess(string data)
        {
            nextButtonGameObject.SetActive(true);
        }

        private void OnRequestFailed(string error)
        {
            
        }
    }
}