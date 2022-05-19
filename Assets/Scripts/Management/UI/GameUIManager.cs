using UnityEngine;
using BattleShips.GameComponents;
using BattleShips.GUI;
using System.Collections.Generic;
using System.Linq;

namespace BattleShips.Management.UI
{
    internal class GameUIManager : MonoBehaviour
    {
        #region Singleton

        static GameUIManager instance;
        public static GameUIManager Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] RectTransform main;
        [SerializeField] RectTransform shipPlacementMenu;

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
            TurnOnMenu(UIParts.Main);

            Player player = Player.Instance;

            shipWrappers.Find(s => s.Constraint == ShipType.Destroyer).Initialise(player.GetDestroyer());
            shipWrappers.Find(s => s.Constraint == ShipType.Cruiser).Initialise(player.GetCruiser());
            shipWrappers.Find(s => s.Constraint == ShipType.Submarine).Initialise(player.GetSubmarine());
            shipWrappers.Find(s => s.Constraint == ShipType.Battleship).Initialise(player.GetBattleship());
            shipWrappers.Find(s => s.Constraint == ShipType.Carrier).Initialise(player.GetCarrier());

            TurnOffMenu(UIParts.ShipPlacement);
        }

        internal void TurnOnMenu(UIParts menu)
        {
            switch (menu)
            {
                case UIParts.Main:
                    main.gameObject.SetActive(true);
                    break;
                case UIParts.ShipPlacement:
                    shipPlacementMenu.gameObject.SetActive(true);
                    break;
            }
        }

        internal void TurnOffMenu(UIParts menu)
        {
            switch(menu)
            {
                case UIParts.Main:
                    main.gameObject.SetActive(false);
                    break;
                case UIParts.ShipPlacement:
                    shipPlacementMenu.gameObject.SetActive(false);
                    break;
            }
        }

        internal void ToggleMenu(UIParts menu)
        {
            switch (menu)
            {
                case UIParts.Main:
                    main.gameObject.SetActive(!main.gameObject.activeSelf);
                    break;
                case UIParts.ShipPlacement:
                    shipPlacementMenu.gameObject.SetActive(!shipPlacementMenu.gameObject.activeSelf);
                    break;
            }
        }
    }
}