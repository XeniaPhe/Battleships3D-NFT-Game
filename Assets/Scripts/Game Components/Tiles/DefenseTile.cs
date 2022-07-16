using System;
using System.Linq;
using UnityEngine.EventSystems;
using BattleShips.GameComponents.Ships;
using BattleShips.Utils;

namespace BattleShips.GameComponents.Tiles
{
    internal class DefenseTile : Tile
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (isTemporarilyPainted) return;
            board.EnteredTile = this;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
        }

        internal void PlaceShip(Ship ship,TileData startTile,int index,Direction direction)
        {
            tileData.tileState = TileState.HasShip;
            tileData.ship = ship;
            tileData.startTile = startTile;
            tileData.shipIndex = index;
            tileData.shipDirection = direction;

            Direction[] orthogonal = Helper.GetOrthogonalDirections(direction);
            Action tryAction = null;

            foreach (var dir in orthogonal)
            {
                tryAction = () => 
                { 
                    board.GetTile(GetTileCoordinatesAt(dir), TileType.Defense).tileData.tileState = TileState.BlockedByAnotherShip; 
                };

                Helper.Try(tryAction);
            }

            tryAction = () => 
            { 
                board.GetTile(GetTileCoordinatesAt(direction), TileType.Defense).tileData.tileState = TileState.BlockedByAnotherShip; 
            };

            Helper.Try(tryAction);

            Direction oppositeDir = Helper.GetOppositeDirection(direction);
            TileData tile;

            tryAction = () =>
            {
                if ((tile = board.GetTile(GetTileCoordinatesAt(oppositeDir), TileType.Defense).tileData).tileState != TileState.HasShip)
                    tile.tileState = TileState.BlockedByAnotherShip;
            };

            Helper.Try(tryAction);
        }
    }
}