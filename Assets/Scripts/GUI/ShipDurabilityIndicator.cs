using UnityEngine;
using UnityEngine.UI;

namespace BattleShips.GUI.Ships
{
    internal class ShipDurabilityIndicator : MonoBehaviour
    {
        [SerializeField] Slider fill;

        private void Awake()
        {
            fill.value = 1;
        }

        internal void UpdateIndicators(float ratio)
        {
            fill.value = ratio;
        }
    }
}