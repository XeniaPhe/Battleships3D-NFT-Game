using UnityEngine;

namespace BattleShips.GameComponents
{
    [CreateAssetMenu(fileName = "Carrier",menuName = "Ships/Carrier")]
    internal class Carrier : Ship
    {
        #region Nonserialized Public Fields/Properties

        internal override int Length => 5;
        internal override ShipType Type => ShipType.Carrier;

        #endregion

        protected override void Awake() => armourParts = new int[5] { armour, armour, armour, armour ,armour};
    }
}