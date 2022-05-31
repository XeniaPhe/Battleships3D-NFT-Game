using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BattleShips.GameComponents;
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
            shipWrappers = FindObjectsOfType<GameShipWrapper>().ToList();
            if (shipWrappers.Count != 5)
                Debug.LogWarning("There's something wrong");
        }

        internal void Initialize()
        {
            Start();
            TurnOnMenu(UIParts.Main);

            Player player = Player.Instance;

            shipWrappers.Find(s => s.Constraint == ShipType.Destroyer).Initialise(player.GetDestroyer());
            shipWrappers.Find(s => s.Constraint == ShipType.Cruiser).Initialise(player.GetCruiser());
            shipWrappers.Find(s => s.Constraint == ShipType.Submarine).Initialise(player.GetSubmarine());
            shipWrappers.Find(s => s.Constraint == ShipType.Battleship).Initialise(player.GetBattleship());
            shipWrappers.Find(s => s.Constraint == ShipType.Carrier).Initialise(player.GetCarrier());

            TurnOnMenu(UIParts.Ships);
            TurnOnMenu(UIParts.MoveReporter);
           // TurnOffMenu(UIParts.DurabilityIndicator);
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
                case UIParts.MoveReporter:
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
                case UIParts.MoveReporter:
                    moveReporter.gameObject.SetActive(false);
                    break;
                case UIParts.DurabilityIndicator:
                    shipWrappers.ForEach(w => w.durabilityIndicator.gameObject.SetActive(false));
                    break;
            }
        }

        internal void ToggleMenu(UIParts menu)
        {
            switch (menu)
            {
                case UIParts.Ships:
                    shipsMenu.gameObject.SetActive(!shipsMenu.gameObject.activeSelf);
                    break;
                case UIParts.ReadyButton:
                    readyButton.gameObject.SetActive(!readyButton.gameObject.activeSelf);
                    break;
                case UIParts.MoveReporter:
                    moveReporter.gameObject.SetActive(!moveReporter.gameObject.activeSelf);
                    break;
                case UIParts.DurabilityIndicator:
                    shipWrappers.ForEach(w => w.durabilityIndicator.gameObject.SetActive(!w.gameObject.activeSelf));
                    break;
            }
        }

        internal void EnableReadyButton() => readyButton.interactable = true;
        internal void DisableReadyButton() => readyButton.interactable = false;
    }
}