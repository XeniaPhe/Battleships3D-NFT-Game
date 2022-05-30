using UnityEngine;

namespace BattleShips.GameComponents.Ships
{
    [CreateAssetMenu(fileName = "Submarine",menuName = "Ships/Submarine")]
    internal class Submarine : Ship
    {
        #region Nonserialized Public Fields/Properties

        internal override int Length => 3;
        internal override ShipType Type => ShipType.Submarine;

        #endregion

        protected override void Awake() => armourParts = new int[3] { armour, armour, armour};
    }
}