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

        #region Serialized Fields

        [SerializeField] TMP_Text reportText;

        #endregion

        #region Cached Fields

        IEnumerator report;
        IEnumerator secondReport;

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

        internal void PublishReport(string announcement, Color color, float seconds)
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            reportText.text = announcement;
            reportText.color = color;
            report = WaitFor(CancelReport, seconds);
            StartCoroutine(report);
        }
        
        internal void CancelReport()
        {
            reportText.text = "";
            gameObject.SetActive(false);
            report = null;
            secondReport = null;
            StopAllCoroutines();
        }

        IEnumerator WaitFor(UnityAction action,float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke();
        }

    }
}