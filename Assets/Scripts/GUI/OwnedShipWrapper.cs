using UnityEngine;
using UnityEngine.UI;
using BattleShips.GameComponents;
using BattleShips.Management.UI;
using UnityEngine.EventSystems;

namespace BattleShips.GUI
{
    [System.Obsolete("Be careful using this class since it has unhandled errors", false)]
    internal class OwnedShipWrapper : ShipWrapper,ISelectHandler,IDeselectHandler
    {
        #region Serialized Fields

        [SerializeField] Button equipButton;
        [SerializeField] Button unequipButton;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            equipButton.onClick.AddListener(Equip);
            unequipButton.onClick.AddListener(Unequip);
            unequipButton.interactable = false;
            equipButton.interactable = true;
            OnDeselect(null);
        }

        internal void SetConstraint(ShipType constraint) => this.constraint = constraint;

        public void OnSelect(BaseEventData eventData)
        {
            equipButton.gameObject.SetActive(true);
            unequipButton.gameObject.SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            equipButton.gameObject.SetActive(false);
            unequipButton.gameObject.SetActive(false);
        }

        internal void Equip()
        {
            equipButton.interactable = false;
            unequipButton.interactable = true;
            DockyardManager.Instance.Equip(this);
        }

        internal void Unequip()
        {
            unequipButton.interactable = false;
            equipButton.interactable = true;
            DockyardManager.Instance.Unequip(this);
        }
    }
}