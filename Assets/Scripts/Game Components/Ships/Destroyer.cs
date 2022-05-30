using UnityEngine;

namespace BattleShips.GameComponents.Ships
{
    [CreateAssetMenu(fileName = "Destroyer",menuName = "Ships/Destroyer")]
    internal class Destroyer : Ship
    {
        #region Nonserialized Public Fields/Properties

        internal override int Length => 2;
        internal override ShipType Type => ShipType.Destroyer;

        #endregion

        protected override void Awake() => armourParts = new int[2] { armour, armour};
    }
}