using UnityEngine;
using BattleShips.GameComponents;
using UnityEngine.UI;
using TMPro;

namespace BattleShips.GUI
{
    [RequireComponent(typeof(Button))]
    internal abstract class ShipWrapper : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] protected Image shipImage;
        [SerializeField] protected TMP_Text typeText;
        [SerializeField] protected ShipType constraint;

        #endregion

        #region Nonserialized Public Fields/Properties

        protected Ship ship;
        
        internal virtual Ship Ship 
        { 
            get => ship; 

            set
            {
                if (value is null)
                    Debug.LogError("Ship can't be null!");
                else if (constraint == value.Type)
                {
                    ship = value;
                    shipImage.gameObject.SetActive(true);
                    shipImage.sprite = ship.Image;
                    typeText.text = constraint.ToString();
                }
                else
                    Debug.LogWarning("Non-matching ship type!");
            }
        }
        internal ShipType Constraint => constraint;

        #endregion

        protected virtual void Awake()
        {
            typeText.text = "";
            shipImage.gameObject.SetActive(false);
        }
    }
}