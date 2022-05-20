using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleShips.GUI
{
    public class GameShipWrapperButton : Button
    {
        #region Serialized Fields

        [SerializeField] public Sprite selectedSprite;
        [SerializeField] public Sprite placedSprite;

        #endregion

        #region Cached Fields

        Image imageComponent;
        static GameShipWrapperButton currentlySelected;
        Sprite normalSprite;

        #endregion


        protected override void Awake()
        {
            base.Awake();
            imageComponent = GetComponent<Image>();
            normalSprite = imageComponent.sprite;
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (currentlySelected)
                currentlySelected.OnDeselect(null);

            imageComponent.sprite = selectedSprite;
            currentlySelected = this;
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if(eventData is null)
            {
                base.OnDeselect(eventData);
                if(imageComponent.sprite !=placedSprite)
                    imageComponent.sprite = normalSprite;
            }
        }
        public void SetSprite()
        {
            imageComponent.sprite = placedSprite;
        }
    }
}