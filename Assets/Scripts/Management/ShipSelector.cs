using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BattleShips.GameComponents;
using BattleShips.GameComponents.Tiles;
using BattleShips.GameComponents.Ships;
using BattleShips.Utils;
using BattleShips.GUI;

namespace BattleShips.Management
{
    internal class ShipSelector : MonoBehaviour
    {
        #region Singleton

        static ShipSelector instance;
        internal static ShipSelector Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] Color successfulColor;
        [SerializeField] Color unsuccessfulColor;

        #endregion

        #region Cached Fields

        GameBoard board;
        internal Ship selectedShip;
        Directions currentDirection = Directions.Up;
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
            selectedShip = ship;
            if(shipInstance)
                HighlightShipPlacement(enteredTile);
        }

        internal void FireFromSelectedShip()
        {
            if (selectedShip is null) Debug.Log("Not selected ship!");

            var ship = FindObjectsOfType<ShipFire>().Where(f => f.name == selectedShip.Type.ToString()).FirstOrDefault();

            ship.FireFromShip(0);
        }

        #region Ship Placement

        internal void Rotate()
        {
            currentDirection = (Directions)(((int)currentDirection + 1) % 4);
            HighlightShipPlacement(enteredTile);
        }

        internal void HighlightShipPlacement(DefenseTile startTile)
        {
            if(shipInstance) Destroy(shipInstance.gameObject);

            foreach (var tile in tilesToPlaceTo)
                tile.RemoveTemporaryPaint();

            if (!(selectedShip is not null && startTile is not null && startTile.IsTileInNormalState()))
                return;

            enteredTile = startTile;
            tilesToPlaceTo = new List<DefenseTile>();
            isSelectionSuccessful = true;
            int firstDir = selectedShip.Length / 2;
            Coordinate tileCoord = enteredTile.tileData.Coordinates;
            Coordinate tempCoords;

            for (int i = 0; i < firstDir; i++)
            {
                if ((tempCoords = tileCoord.GetCoordinatesAt(currentDirection)) != null)
                    tileCoord = tempCoords;
                else
                    isSelectionSuccessful = false;
            }

            DefenseTile temp;
            var oppositeDirection = Helper.GetOppositeDirection(currentDirection);

            Traverse(firstDir,oppositeDirection);
            tilesToPlaceTo.Add(enteredTile);
            tileCoord = enteredTile.GetTileCoordinatesAt(oppositeDirection);
            int secondDir = selectedShip.Length - firstDir - 1;
            Traverse(secondDir,oppositeDirection);

            Vector3 rotation = selectedShip.NormalRotation;
            Vector3 pos = enteredTile.transform.position;

            if(selectedShip.Length %2 == 0)
            {
                switch (currentDirection)
                {
                    case Directions.Right:
                        pos += (Vector3.back / 2);
                        break;
                    case Directions.Up:
                        pos += (Vector3.right / 2);
                        break;
                    case Directions.Left:
                        pos += (Vector3.forward / 2);
                        break;
                    case Directions.Down:
                        pos += (Vector3.left / 2);
                        break;
                }
            }

            pos.y = 1.2f;
            rotation.y = (int)(currentDirection) * 90;
            
            shipInstance = Instantiate<Transform>(selectedShip.Model.transform, pos, Quaternion.Euler(rotation), null);
            shipInstance.localScale = selectedShip.PreferedScale;
            foreach (var child in shipInstance.transform)
            {
                foreach (var item in (Transform)child)
                {
                    ((Transform)item).localScale *= (selectedShip.PreferedScale.y / 25);
                }
            }
            shipInstance.name = selectedShip.Type.ToString();

            /*foreach(var mat in shipInstance.GetComponent<MeshRenderer>().materials)
                mat.color = Color.white;*/

            foreach (var tile in tilesToPlaceTo)
                tile.PaintTemporarily(isSelectionSuccessful ? successfulColor : unsuccessfulColor);


            void Traverse(int distance,Directions direction)
            {
                for (int i = 0; i < distance; i++)
                {
                    temp = board.GetTile(tileCoord, TileType.Defense) as DefenseTile;

                    if (temp != null && temp.IsTileInNormalState())
                    {
                        tilesToPlaceTo.Add(temp);
                        tileCoord = tileCoord.GetCoordinatesAt(direction);
                    }
                    else
                    {
                        isSelectionSuccessful = false;
                        break;
                    }
                }
            }
        }

        internal void PlaceShip()
        {
            if (selectedShip is null) return;
            if (shipInstance is null) return;
            if (isSelectionSuccessful is false) return;

            if (selectedShip.Type == ShipType.Submarine)
                shipInstance.position = new Vector3(shipInstance.position.x, 0f, shipInstance.position.z);
            else
                shipInstance.position = new Vector3(shipInstance.position.x, 0.4f, shipInstance.position.z);

            selectedShip.OnShipPlaced();
            shipInstance = null;
            selectedShip = null;

            DefenseTile tile;
            TileData start = tilesToPlaceTo[0].tileData;
            for (int i = 0; i < tilesToPlaceTo.Count; i++)
            {
                tile = tilesToPlaceTo[i];
                tile.RemoveTemporaryPaint();
                tile.PlaceShip(selectedShip,start,i,Helper.GetOppositeDirection(currentDirection));
            }

            tilesToPlaceTo.Clear();

            
        }

        #endregion
    }
}