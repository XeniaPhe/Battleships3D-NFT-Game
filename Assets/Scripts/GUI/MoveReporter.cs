using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace BattleShips.GUI
{
    internal class MoveReporter : MonoBehaviour
    {
        #region Singleton

        static MoveReporter instance;
        internal static MoveReporter Instance => instance;
        #endregion

        #region Serialized Fields

        [SerializeField] TMP_Text reportText;

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

        internal void PublishReport(string announcement,Color color,float seconds)
        {
            StopAllCoroutines();
            gameObject.SetActive(true);
            reportText.text = announcement;
            StartCoroutine(CancelAfter(seconds));
        }

        internal void CancelReport()
        {
            reportText.text = "";
            gameObject.SetActive(false);
            StopAllCoroutines();
        }

        IEnumerator CancelAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            CancelReport();
        }
    }
}