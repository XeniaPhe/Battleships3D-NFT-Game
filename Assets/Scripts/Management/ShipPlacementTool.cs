using System.Collections.Generic;
using UnityEngine;
using BattleShips.GameComponents;
using BattleShips.GameComponents.Tiles;
using BattleShips.Utils;

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
        Transform shipInstance;
        bool isSelectionSuccessful;

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
            }
        }

        private void Start()
        {
            board = GameBoard.Instance;
        }

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

            if(shipInstance) Destroy(shipInstance.gameObject);

            foreach (var tile in tilesToPlaceTo)
                tile.RemoveTemporaryPaint();

            if (!(selectedShip is not null && startTile is not null && startTile.IsTileInNormalState()))
                return;
            
            tilesToPlaceTo = new List<DefenseTile>() { startTile };
            DefenseTile temp;
            isSelectionSuccessful = true;

            void Traverse(DefenseTile tile,Directions direction,int distance)
            {
                for (int i = 0; i < distance; i++)
                {
                    Coordinate coord = tile.GetTileCoordinatesAt(direction);
                    temp = board.GetTile(coord, TileType.Defense) as DefenseTile;

                    if (temp is not null && temp.IsTileInNormalState())
                    {
                        tilesToPlaceTo.Add(temp);
                        tile = temp;
                    }
                    else
                    {
                        isSelectionSuccessful = false;
                        break;
                    }
                }
            }

            tilesToPlaceTo.Add(startTile);
            int firstDir = selectedShip.Length / 2;
            Traverse(startTile, currentDirection, firstDir);
            Directions oppositeDir = Helper.GetOppositeDirection(currentDirection);
            int secondDir = selectedShip.Length - firstDir - 1;
            Traverse(startTile, oppositeDir,secondDir);

            Vector3 rotation = selectedShip.NormalRotation;
            rotation.y = ((int)currentDirection-1) * 90;
            Vector3 pos = enteredTile.transform.position;
            Vector3 correction = selectedShip.Correction * board.TileSize;

            pos += currentDirection switch
            {
                Directions.Left => -correction,
                Directions.Right => correction,
                Directions.Up => new Vector3(-correction.z,0,-correction.x),
                Directions.Down => new Vector3(correction.z,0,correction.x),
                _ => Vector3.zero
            };

            shipInstance = Instantiate<Transform>(selectedShip.Model, pos, Quaternion.Euler(rotation), null);
            shipInstance.localScale = selectedShip.PreferedScale;
            shipInstance.GetComponent<MeshRenderer>().material.color = isSelectionSuccessful ? Color.white : unsuccessfulColor;

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