using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace BattleShips.GUI
{
    internal class MoveLogger : MonoBehaviour
    {
        #region Singleton

        static MoveLogger instance;
        internal static MoveLogger Instance => instance;
        #endregion


        [SerializeField] TMP_Text logText;

        IEnumerator log;

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                logText.text = "";
                gameObject.SetActive(false);
            }
        }

        internal void Log(string log, Color color, float seconds)
        {
            gameObject.SetActive(true);
            CancelInvoke();
            logText.text = log;
            logText.color = color;
            Invoke(nameof(CancelLog),seconds);
        }

        internal void CancelLog()
        {
            logText.text = string.Empty;
            gameObject.SetActive(false);
            CancelInvoke();
        }
    }
}