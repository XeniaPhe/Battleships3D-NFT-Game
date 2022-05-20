using UnityEngine;
using BattleShips.GameComponents;
using BattleShips.GameComponents.Tiles;
using BattleShips.Management.UI;

namespace BattleShips.Management
{
    internal class GameManager : MonoBehaviour
    {
        #region Singleton

        static GameManager instance;
        public static GameManager Instance { get => instance; }

        #endregion

        #region Cached Fields
        GameUIManager uiManager;
        ShipPlacementTool shipPlacementTool;
        GamePhase phase;
        Turn turn = Turn.Player;
        int shipsPlaced;

        #endregion

        #region Nonserialized Public Fields/Properties

        Tile enteredTile;
        internal Tile EnteredTile
        {
            get => enteredTile;
            set
            {
                enteredTile = value;
                if (phase == GamePhase.ShipPlacement && enteredTile?.GetType() == typeof(DefenseTile))
                    shipPlacementTool.HighlightShipPlacement((DefenseTile)enteredTile);
                else
                    shipPlacementTool.HighlightShipPlacement(null);
            }
        }

        Tile clickedTile;
        internal Tile ClickedTile
        {
            get => clickedTile;
            set
            {
                clickedTile = value;
                if (phase == GamePhase.ShipPlacement && clickedTile?.GetType() == typeof(DefenseTile))
                    shipPlacementTool.PlaceShip((DefenseTile)clickedTile);
            }
        }

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
            uiManager = GameUIManager.Instance;
            uiManager.Initialize();
            shipPlacementTool = ShipPlacementTool.Instance;
            StartShipPlacementPhase();
        }

        private void Update()
        {
            if (phase == GamePhase.ShipPlacement && Input.GetKeyDown(KeyCode.R))
                shipPlacementTool.Rotate();
        }

        private void StartShipPlacementPhase()
        {
            uiManager.TurnOnMenu(UIParts.ShipPlacement);
            phase = GamePhase.ShipPlacement;
        }

        private void StartAIShipPlacement()
        {
            uiManager.TurnOffMenu(UIParts.ShipPlacement);
        }

        internal void OnShipPlaced()
        {
            
        }
    }
}