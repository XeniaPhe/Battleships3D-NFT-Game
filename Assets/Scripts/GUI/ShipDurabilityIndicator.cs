using UnityEngine;
using UnityEngine.UI;

namespace BattleShips.GUI.Ships
{
    internal class ShipDurabilityIndicator : MonoBehaviour
    {
        [SerializeField] Image[] goodFills;
        [SerializeField] Image[] badFills;
        [SerializeField] Color goodColor;
        [SerializeField] Color badColor;

        private void Awake()
        {
            foreach (var fill in goodFills)
            {
                fill.type = Image.Type.Filled;
                fill.fillMethod = Image.FillMethod.Horizontal;
                fill.fillOrigin = (int)Image.OriginHorizontal.Left;
                fill.color  = goodColor;
                fill.fillAmount = 1;
            }
            foreach (var fill in badFills)
            {
                fill.type= Image.Type.Filled;
                fill.fillMethod= Image.FillMethod.Horizontal;
                fill.fillOrigin = (int)Image.OriginHorizontal.Right;
                fill.color = badColor;
                fill.fillAmount = 0f;
            }
        }

        internal void UpdateIndicators(int armour,int maxArmour,int partIndex)
        {
            float good = (float)armour / maxArmour;
            goodFills[partIndex].fillAmount = good;
            badFills[partIndex].fillAmount = 1 - good;
        }
    }
}