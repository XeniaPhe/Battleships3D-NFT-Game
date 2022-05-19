using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace BattleShips.Management.UI
{
    internal class PopupManager : MonoBehaviour
    {
        #region Singleton

        static PopupManager instance;
        internal static PopupManager Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text questionText;
        [SerializeField] Button firstOptionButton;
        [SerializeField] Button secondOptionButton;
        [SerializeField] Button cancelButton;
        [SerializeField] Button closeButton;

        #endregion

        #region Cached Fields

        TMP_Text firstOptionText;
        TMP_Text secondOptionText;
        TMP_Text cancelOptionText;
        Vector3 secondOptionOriginalPos;
        Vector3 cancelOriginalPos;

        #endregion

        private void Awake()
        {
            if(instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                firstOptionText = firstOptionButton.GetComponentInChildren<TMP_Text>();
                secondOptionText = secondOptionButton.GetComponentInChildren<TMP_Text>();
                cancelOptionText = cancelButton.GetComponentInChildren<TMP_Text>();

                secondOptionOriginalPos = secondOptionButton.transform.localPosition;
                cancelOriginalPos = cancelButton.transform.localPosition;

                ClosePopup();
            }
        }

        //For creating 2 to 4 buttons and 2 to 3 different callbacks
        internal void LoadPopup(string title,string question,UnityAction firstOption,string firstOptionText,
            UnityAction secondOption,string secondOptionText,bool isCancelActive = true,bool isSecondOptionAndCancelSwitched=false,
            UnityAction cancelAction = null,string cancelOptionText = "CANCEL", bool isCloseActive = true,bool isCloseEquivalentToCancel = true)
        {
            gameObject.SetActive(true);
            firstOptionButton.gameObject.SetActive(true);
            secondOptionButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(isCancelActive);
            closeButton.gameObject.SetActive(isCloseActive);

            titleText.text = title;
            questionText.text = question;

            this.firstOptionText.text = firstOptionText;
            firstOptionButton.onClick.AddListener(firstOption);

            this.secondOptionText.text=secondOptionText;
            secondOptionButton.onClick.AddListener(secondOption);

            if(isCancelActive)
            {
                this.cancelOptionText.text = cancelOptionText;
                cancelButton.onClick.AddListener(cancelAction);

                if(isSecondOptionAndCancelSwitched)
                {
                    cancelButton.transform.localPosition = secondOptionOriginalPos;
                    secondOptionButton.transform.localPosition = cancelOriginalPos;
                }
            }

            if (isCloseEquivalentToCancel)
                closeButton.onClick.AddListener(cancelAction);
        }

        //For creating 1 to 3 buttons and 1 to 2 different callbacks
        internal void LoadPopup(string title,string question,UnityAction firstOption,string firstOptionText,
            bool isCancelActive = true, bool isSecondOptionAndCancelSwitched = false,UnityAction cancelAction = null,
            string cancelOptionText = "CANCEL", bool isCloseActive = true, bool isCloseEquivalentToCancel = true)
        {
            gameObject.SetActive(true);
            firstOptionButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(isCancelActive);
            closeButton.gameObject.SetActive(isCloseActive);

            titleText.text = title;
            questionText.text = question;

            this.firstOptionText.text = firstOptionText;
            firstOptionButton.onClick.AddListener(firstOption);

            if (isCancelActive)
            {
                this.cancelOptionText.text = cancelOptionText;
                cancelButton.onClick.AddListener(cancelAction);

                if (isSecondOptionAndCancelSwitched)
                    cancelButton.transform.localPosition = secondOptionOriginalPos;
            }

            if (isCloseEquivalentToCancel)
                closeButton.onClick.AddListener(cancelAction);
        }

        //For creating 1 to 2 buttons and 1 callback (Callback button is one of either middle or rightmost buttons)
        internal void LoadPopup(string title, string question, bool isSecondOptionAndCancelSwitched = false, 
            UnityAction cancelAction = null,string cancelOptionText = "CANCEL",bool isCloseActive = true, 
            bool isCloseEquivalentToCancel = true)
        {
            gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(isCloseActive);

            titleText.text = title;
            questionText.text = question;

            
            this.cancelOptionText.text = cancelOptionText;
            cancelButton.onClick.AddListener(cancelAction);

            if (isSecondOptionAndCancelSwitched)
                cancelButton.transform.localPosition = secondOptionOriginalPos;

            if (isCloseEquivalentToCancel)
                closeButton.onClick.AddListener(cancelAction);
        }

        //For creating 1 to 2 buttons and 1 callback (Callback button is the leftmost button)
        internal void LoadPopup(string title,string question,UnityAction firstOption, string firstOptionText, bool isCloseActive = true)
        {
            gameObject.SetActive(true);
            firstOptionButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(isCloseActive);

            titleText.text = title;
            questionText.text = question;

            this.firstOptionText.text = firstOptionText;
            firstOptionButton.onClick.AddListener(firstOption);
        }

        private void ResetEverything()
        {
            firstOptionButton.onClick.RemoveAllListeners();
            secondOptionButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();

            firstOptionButton.onClick.AddListener(ClosePopup);
            secondOptionButton.onClick.AddListener(ClosePopup);
            cancelButton.onClick.AddListener(ClosePopup);
            closeButton.onClick.AddListener(ClosePopup);

            firstOptionButton.gameObject.SetActive(false);
            secondOptionButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);

            secondOptionButton.transform.localPosition = secondOptionOriginalPos;
            cancelButton.transform.localPosition = cancelOriginalPos;
        }

        private void ClosePopup()
        {
            ResetEverything();
            gameObject.SetActive(false);
        }
    }
}