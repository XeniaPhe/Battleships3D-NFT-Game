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
        [SerializeField] float turnOffTime = 1f;

        #endregion

        #region Cached Fields

        float currentCountdown;
        WaitForSeconds waitASec;
        WaitForSeconds waitForTurnOff;

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
                waitForTurnOff = new WaitForSeconds(turnOffTime);
                gameObject.SetActive(false);
            }
        }

        internal void StartCountdown(uint seconds,UnityAction timeOutAction)
        {
            StopAllCoroutines();
            gameObject.SetActive(true);
            StartCoroutine(Countdown(seconds, timeOutAction));
        }

        internal void CancelCountdown(uint seconds = 0)
        {
            StopAllCoroutines();
            StartCoroutine(TurnOff(seconds));
        }

        IEnumerator Countdown(uint seconds,UnityAction timeOutAction)
        {
            remainingTime = seconds;
            for (int i = (int)seconds; i > 0; i--)
            {
                countText.text = i.ToString();
                yield return waitASec;
                --remainingTime;
            }

            countText.text = "0";
            StartCoroutine(TurnOff());
            remainingTime = 0;
            timeOutAction?.Invoke();
        }

        IEnumerator TurnOff(float seconds = 0)
        {
            if(seconds == 0)
                yield return waitForTurnOff;
            else
                yield return new WaitForSeconds(seconds);

            gameObject.SetActive(false);
        }
    }
}