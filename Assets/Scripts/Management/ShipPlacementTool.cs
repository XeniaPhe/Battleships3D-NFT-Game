using System.Collections.Generic;
using UnityEngine;
using BattleShips.GameComponents;
using BattleShips.GameComponents.Tiles;

namespace BattleShips.Management
{
    internal class ShipPlacementTool : MonoBehaviour
    {
        #region Singleton

        static ShipPlacementTool instance;
        internal static ShipPlacementTool Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] Color successfulColor;
        [SerializeField] Color unsuccessfulColor;

        #endregion

        #region Cached Fields

        GameBoard board;
        Ship selectedShip;
        Directions currentDirection = Directions.Right;
        DefenseTile enteredTile;
        List<DefenseTile> tilesToPlaceTo = new List<DefenseTile>();
        bool isSelectionSuccessful;

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
                instance = this;
        }

        private void Start() => board = GameBoard.Instance;

        internal void SelectShip(Ship ship)
        {
            if (selectedShip is null && ship is not null)
                DefenseTile.maskMode = TileMaskMode.TemporaryMask;
            selectedShip = ship;
            selectedShip.ShipPlaced += GameManager.Instance.OnShipPlaced;
        }

        internal void Rotate()
        {
            currentDirection = (Directions)(((int)currentDirection + 1) % 4);
            HighlightShipPlacement(enteredTile);
        }

        internal void HighlightShipPlacement(DefenseTile startTile)
        {
            enteredTile = startTile;

            foreach (var tile in tilesToPlaceTo)
                tile.RemoveTemporaryPaint();

            if (!(selectedShip is not null && startTile is not null && startTile.IsTileInNormalState()))
                return;
            
            tilesToPlaceTo = new List<DefenseTile>() { startTile };
            DefenseTile temp;
            isSelectionSuccessful = true;
            for (int i = 0; i < selectedShip.Length - 1; i++)
            {
                Coordinate coord = startTile.GetTileCoordinatesAt(currentDirection);
                temp = board.GetTile(coord,TileType.Defense) as DefenseTile;

                if (temp is not null && temp.IsTileInNormalState())
                {
                    tilesToPlaceTo.Add(temp);
                    startTile = temp;
                }
                else
                {
                    isSelectionSuccessful = false;
                    break;
                }
            }

            foreach (var tile in tilesToPlaceTo)
                tile.PaintTemporarily(isSelectionSuccessful ? successfulColor : unsuccessfulColor);
        }

        internal void PlaceShip(DefenseTile tile)
        {
            if (selectedShip is null || !isSelectionSuccessful) 
                return;

            tilesToPlaceTo.ForEach(t =>
            {
                t.RemoveTemporaryPaint();
                t.PlaceShip(selectedShip);
            });

            tilesToPlaceTo.Clear();
            selectedShip.OnShipPlaced();
            selectedShip = null;
            DefenseTile.maskMode = TileMaskMode.PermanentMask;
        }
    }
}