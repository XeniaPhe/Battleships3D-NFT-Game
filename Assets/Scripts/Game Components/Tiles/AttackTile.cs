using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleShips.GameComponents.Tiles
{
    internal class AttackTile : Tile
    {
        #region Serialized Fields

        [SerializeField] Color missPegColor;
        [SerializeField] Color hitPegColor;
        [SerializeField] Color completeShipColor;

        #endregion

        public override void OnPointerClick(PointerEventData eventData)
        {

        }
    }
}