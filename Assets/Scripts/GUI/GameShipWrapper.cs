using UnityEngine;
using UnityEngine.EventSystems;
using BattleShips.Management;
using BattleShips.GameComponents;

namespace BattleShips.GUI
{
    [RequireComponent(typeof(GameShipWrapperButton))]
    internal class GameShipWrapper : ShipWrapper
    {
        #region Public Fields/Properties

        new internal ShipType Constraint { get => constraint; }

        #endregion

        #region Cached Fields

        GameShipWrapperButton button;

        #endregion

        protected override void Awake()
        {
            button = GetComponent<GameShipWrapperButton>();
            button.onClick.AddListener(OnClick);
        }

        internal void Initialise(Ship ship)
        {
            this.ship = ship;
            shipImage.texture = ship.Texture;
            ship.ShipPlaced += OnShipPlaced;
        }

        private void OnClick() => ShipPlacementTool.Instance.SelectShip(Ship);

        private void OnShipPlaced()
        {
            BaseEventData data = new BaseEventData(FindObjectOfType<EventSystem>());
            data.selectedObject = null;
            button.OnDeselect(data);
            button.interactable = false;
            button.SetSprite();
        }
    }
}