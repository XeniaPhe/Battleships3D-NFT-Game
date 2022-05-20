using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using BattleShips.GameComponents;
using BattleShips.Management.UI;

namespace BattleShips.GUI
{
    internal class DeckShipWrapper : ShipWrapper,ISelectHandler,IDeselectHandler
    {
        #region Serialized Fields

        [SerializeField] Button unequipButton;

        #endregion

        #region Nonserialized Public Fields/Properties

        internal override Ship Ship
        {
            get => base.Ship;

            set
            {
                if(value is null || constraint == value.Type)
                {
                    ship = value;

                    if(ship)
                    {
                        shipImage.gameObject.SetActive(true);
                        shipImage.texture = ship.Texture;
                    }
                    else
                    {
                        shipImage.texture = null;
                        shipImage.gameObject.SetActive(false);
                    }
                }
                else
                    Debug.LogError("Non-matching ship type!");
            }
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            typeText.text = constraint.ToString();
            OnDeselect(null);
            unequipButton.onClick.AddListener(Unequip);
        }

        private void Start()
        {
            switch (constraint)
            {
                case ShipType.Destroyer:
                    Ship = Player.Instance.GetDestroyer();
                    break;
                case ShipType.Cruiser:
                    Ship = Player.Instance.GetCruiser();
                    break;
                case ShipType.Submarine:
                    Ship = Player.Instance.GetSubmarine();
                    break;
                case ShipType.Battleship:
                    Ship = Player.Instance.GetBattleship();
                    break;
                case ShipType.Carrier:
                    Ship = Player.Instance.GetCarrier();
                    break;
                default:
                    return;
            }

            UnityAction markAsEquipped = () => DockyardManager.Instance.Equip(this);
            StartCoroutine(InvokeWhenManagerIsReady(markAsEquipped));
        }

        private void Unequip()
        {
            OnDeselect(null);
            DockyardManager.Instance.Unequip(this);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if(ship)
                unequipButton.gameObject.SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            unequipButton.gameObject.SetActive(false);
        }

        IEnumerator InvokeWhenManagerIsReady(UnityAction action)
        {
            DockyardManager manager = DockyardManager.Instance;

            yield return new WaitUntil(() => manager.IsReady);
            action?.Invoke();
            yield return null;
        }
    }
}