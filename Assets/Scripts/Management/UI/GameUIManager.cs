using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using BattleShips.GameComponents.Player;
using BattleShips.GameComponents.Ships;
using BattleShips.GUI;

namespace BattleShips.Management.UI
{
    internal class GameUIManager : MonoBehaviour
    {
        #region Singleton

        static GameUIManager instance;
        public static GameUIManager Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] RectTransform shipsMenu;
        [SerializeField] Button readyButton;
        [SerializeField] TMP_Text moveReporter;

        #endregion

        #region Cached Fields

        List<GameShipWrapper> shipWrappers;

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
                instance = this;
        }

        private void Start()
        {
            if(shipWrappers == null)
                shipWrappers = FindObjectsOfType<GameShipWrapper>().ToList();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                GameShipWrapperButton.currentlySelected.SelectLower();
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                GameShipWrapperButton.currentlySelected.SelectUpper();
        }

        internal void Initialize()
        {
            Start();

            TurnOnMenu(UIParts.Ships);
            TurnOnMenu(UIParts.DurabilityIndicator);
            TurnOnMenu(UIParts.MoveLogger);
            TurnOnMenu(UIParts.ReadyButton);

            HumanPlayer player = HumanPlayer.Instance;

            shipWrappers.Find(s => s.Constraint == ShipType.Destroyer).Initialise(player.GetDestroyer());
            shipWrappers.Find(s => s.Constraint == ShipType.Cruiser).Initialise(player.GetCruiser());
            shipWrappers.Find(s => s.Constraint == ShipType.Submarine).Initialise(player.GetSubmarine());
            shipWrappers.Find(s => s.Constraint == ShipType.Battleship).Initialise(player.GetBattleship());
            shipWrappers.Find(s => s.Constraint == ShipType.Carrier).Initialise(player.GetCarrier());

            ResetWrapperButtons();
        }

        internal void TurnOnMenu(UIParts menu)
        {
            switch (menu)
            {
                case UIParts.Ships:
                    shipsMenu.gameObject.SetActive(true);
                    break;
                case UIParts.ReadyButton:
                    readyButton.gameObject.SetActive(true);
                    break;
                case UIParts.MoveLogger:
                    moveReporter.gameObject.SetActive(true);
                    break;
                case UIParts.DurabilityIndicator:
                    shipWrappers.ForEach(w => w.durabilityIndicator.gameObject.SetActive(true));
                    break;
            }
        }

        internal void TurnOffMenu(UIParts menu)
        {
            switch(menu)
            {
                case UIParts.Ships:
                    shipsMenu.gameObject.SetActive(false);
                    break;
                case UIParts.ReadyButton:
                    readyButton.gameObject.SetActive(false);
                    break;
                case UIParts.MoveLogger:
                    moveReporter.gameObject.SetActive(false);
                    break;
                case UIParts.DurabilityIndicator:
                    shipWrappers.ForEach(w => w.durabilityIndicator.gameObject.SetActive(false));
                    break;
            }
        }

        internal void SetReadyButtonListener(UnityAction callback)
        {
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(callback);
            readyButton.onClick.AddListener(() => TurnOffMenu(UIParts.ReadyButton));
        }

        internal void ResetWrapperButtons()
        {
            shipWrappers.ForEach(w => w.Reset());
        }
        internal void EnableReadyButton() => readyButton.interactable = true;
        internal void DisableReadyButton() => readyButton.interactable = false;
    }
}