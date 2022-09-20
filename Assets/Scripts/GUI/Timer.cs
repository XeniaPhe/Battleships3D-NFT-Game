using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace BattleShips.GUI
{
    internal class Timer : MonoBehaviour
    {
        #region Singleton

        static Timer instance;
        internal static Timer Instance => instance;

        #endregion

        #region Serialized Fields

        [SerializeField] TMP_Text countText;

        #endregion

        #region Cached Fields

        float currentCountdown;
        WaitForSeconds waitASec;

        #endregion

        #region Nonserialized Public Fields

        uint remainingTime;
        internal uint RemainingTime => remainingTime;

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                waitASec = new WaitForSeconds(1);
                gameObject.SetActive(false);
            }
        }

        internal void StartCountdown(uint seconds, UnityAction timeOutAction)
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Countdown(seconds, timeOutAction));
        }

        internal void CancelCountdown()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            remainingTime = 0;
        }

        IEnumerator Countdown(uint seconds, UnityAction timeOutAction)
        {
            remainingTime = seconds;
            for (int i = (int)seconds; i > 0; i--)
            {
                countText.text = i.ToString();
                yield return waitASec;
                --remainingTime;
            }

            countText.text = "0";
            remainingTime = 0;
            gameObject.SetActive(false);
            timeOutAction?.Invoke();
        }
    }
}