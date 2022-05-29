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

        //Just add a queue if things get messy
        internal void PublishReport(string announcement,Color color,float seconds,float overrideOldReportAfter = 0)
        {
            if(overrideOldReportAfter > 0 && overrideOldReportAfter < seconds && report != null)
            {
                report = WaitFor(() =>
                {
                    PublishReport(announcement, color, seconds - overrideOldReportAfter);
                }, overrideOldReportAfter + 0.05f);
            }
            else
            {
                StopAllCoroutines();
                gameObject.SetActive(true);
                reportText.text = announcement;
                reportText.color = color;
                report = WaitFor(CancelReport,seconds);
            }

            StartCoroutine(report);
        }
        
        internal void CancelReport()
        {
            reportText.text = "";
            gameObject.SetActive(false);
            report = null;
            StopAllCoroutines();
        }

        IEnumerator WaitFor(UnityAction action,float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke();
        }
    }
}