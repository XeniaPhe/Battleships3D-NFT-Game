using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleShips.GUI
{
    public class GameShipWrapperButton : Button
    {
        #region Cached Fields

        internal static GameShipWrapperButton currentlySelected;
        internal GameShipWrapperButton up;
        internal GameShipWrapperButton down;
        internal bool selectable = true;
        internal static int nonSelectableCount = 0;

        #endregion

        internal void SelectUpper() => up.OnSelect(null);
        internal void SelectLower() => down.OnSelect(null);
        public override void OnSelect(BaseEventData eventData)
        {
            if (nonSelectableCount == 5)
                return;

            if (!selectable)
            {
                currentlySelected = this;
                if (currentlySelected == up)
                    SelectLower();
                else if(currentlySelected == down)
                    SelectUpper();

                return;
            }

            currentlySelected?.OnDeselect(null);
            currentlySelected = this;
            if (eventData is null)
                onClick?.Invoke();
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if(eventData is null) base.OnDeselect(eventData);
        }
    }
}