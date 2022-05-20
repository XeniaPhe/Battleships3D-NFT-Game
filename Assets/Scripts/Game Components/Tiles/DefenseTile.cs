using System.Collections;
using UnityEngine.EventSystems;

namespace BattleShips.GameComponents.Tiles
{
    internal class DefenseTile : Tile
    {
        #region Nonserialized Public Fields/Properties

        internal static TileMaskMode maskMode = TileMaskMode.PermanentMask;

        #endregion

        #region Cached Fields

        IEnumerator currentCoroutine;

        #endregion


        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (isTemporarilyPainted) return;
            self.color = normalColor;

            //if (maskMode == TileMaskMode.TemporaryMask)
            //{
            //    currentCoroutine = TurnOffMask();
            //    StartCoroutine(currentCoroutine);
            //}

            board.EnteredTile = this;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            board.ClickedTile = this;
        }

        private IEnumerator TurnOffMask()
        {
            yield return maskWait;
            self.color = disabledColor;
            currentCoroutine = null;
            yield return null;
        }

        internal void PlaceShip(Ship ship)
        {
            tileData.tileState = TileState.HasShip;
            self.color = disabledColor;
            tileData.ship = ship;
        }
    }
}