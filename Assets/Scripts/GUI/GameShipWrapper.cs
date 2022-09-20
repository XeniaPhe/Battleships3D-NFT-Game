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
        #region Serialized Fields

        [SerializeField] GameShipWrapperButton up;
        [SerializeField] GameShipWrapperButton down;
        [SerializeField] bool isSelectedByDefault;

        #endregion

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
            button.up = up;
            button.down = down;
            if (isSelectedByDefault && GameShipWrapperButton.currentlySelected == null)
                button.OnSelect(null);
        }

        internal void Initialise(Ship ship)
        {
            this.ship = ship;
            ship.wrapper = this;
            shipImage.sprite = ship.CardSprite;
            ship.ShipPlaced += OnShipPlaced;
            ship.ShipPlaced += GameManager.Instance.OnShipPlaced;
        }

        internal void Reset()
        {
            if (!button.selectable)
                GameShipWrapperButton.nonSelectableCount--;
            button.interactable = true;
            button.selectable = true;
        }

        private void OnClick() => ShipSelector.Instance.SelectShip(Ship);

        private void OnShipPlaced()
        {
            button.OnDeselect(null);
            button.interactable = false;
            button.selectable = false;
            GameShipWrapperButton.nonSelectableCount++;
        }
    }
}