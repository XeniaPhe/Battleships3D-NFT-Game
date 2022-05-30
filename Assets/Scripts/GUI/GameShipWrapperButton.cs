using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleShips.GUI
{
    public class GameShipWrapperButton : Button
    {
        #region Cached Fields

        static GameShipWrapperButton currentlySelected;

        #endregion

        public override void OnSelect(BaseEventData eventData)
        {
            if (currentlySelected)
                currentlySelected.OnDeselect(null);

            currentlySelected = this;
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if(eventData is null)
                base.OnDeselect(eventData);
        }
    }
}