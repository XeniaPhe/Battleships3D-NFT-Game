using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleShips.GUI
{
    public class GameShipWrapperButton : Button
    {
        #region Serialized Fields

        public GameShipWrapperButton up;
        public GameShipWrapperButton down;
        public bool isSelectedByDefault;

        #endregion

        #region Cached Fields

        internal static GameShipWrapperButton currentlySelected;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            if (isSelectedByDefault && currentlySelected==null)
                currentlySelected = this;
        }

        protected override void Start()
        {
            base.Start();
            if (currentlySelected == this) OnSelect(null);
        }

        internal void SelectUpper() => up.OnSelect(null);
        internal void SelectLower() => down.OnSelect(null);

        public override void OnSelect(BaseEventData eventData)
        {
            currentlySelected?.OnDeselect(null);
            currentlySelected = this;
            onClick?.Invoke();
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if(eventData is null) base.OnDeselect(eventData);
        }
    }
}