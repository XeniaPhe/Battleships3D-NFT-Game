using UnityEngine;
using UnityEngine.EventSystems;
using BattleShips.Management;
using BattleShips.GameComponents.Ships;
using BattleShips.GUI.Ships;

namespace BattleShips.GUI
{
    [RequireComponent(typeof(GameShipWrapperButton))]
    internal class GameShipWrapper : ShipWrapper
    {
        #region Public Fields/Properties

        new internal ShipType Constraint { get => constraint; }

        internal ShipDurabilityIndicator durabilityIndicator;

        #endregion

        #region Cached Fields

        GameShipWrapperButton button;

        #endregion

        protected override void Awake()
        {
            durabilityIndicator = GetComponent<ShipDurabilityIndicator>();
            button = GetComponent<GameShipWrapperButton>();
            button.onClick.AddListener(OnClick);
        }

        internal void Initialise(Ship ship)
        {
            this.ship = ship;
            ship.wrapper = this;
            shipImage.sprite = ship.CardSprite;
            ship.ShipPlaced += OnShipPlaced;
        }

        private void OnClick() => ShipSelector.Instance.SelectShip(Ship);

        private void OnShipPlaced()
        {
            BaseEventData data = new BaseEventData(FindObjectOfType<EventSystem>());
            data.selectedObject = null;
            button.OnDeselect(data);
        }
    }
}