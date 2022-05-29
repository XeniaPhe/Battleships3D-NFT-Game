using System.Collections;
using UnityEngine.EventSystems;

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

        internal void PlaceShip(Ship ship,TileData startTile,int index)
        {
            tileData.tileState = TileState.HasShip;
            tileData.ship = ship;
            tileData.startTile = startTile;
            tileData.shipIndex = index;
        }
    }
}