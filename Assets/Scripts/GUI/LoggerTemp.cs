using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace BattleShips.GUI
{
    internal class LoggerTemp : MonoBehaviour
    {
        #region Singleton

        static LoggerTemp instance;
        internal static LoggerTemp Instance => instance;
        #endregion

        #region Serialized Fields

        [SerializeField] TMP_Text reportText;

        #endregion

        #region Cached Fields

        internal string reportTextString;
        internal Color reportColor;

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                reportText.text = "";
                gameObject.SetActive(false);
            }
        }

        internal void PublishReport(string announcement,Color color)
        {
            reportTextString = announcement;
            reportColor = color;
            reportText.text = announcement;
            reportText.color = color;
        }

        internal void Wipe()
        {
            reportText.text = string.Empty;
        }
    }
}