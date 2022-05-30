using UnityEngine;

namespace BattleShips.GameComponents.Ships
{
    [CreateAssetMenu(fileName = "Cruiser",menuName = "Ships/Cruiser")] 
    internal class Cruiser : Ship
    {
        #region Nonserialized Public Fields/Properties

        internal override int Length => 3;
        internal override ShipType Type => ShipType.Cruiser;

        #endregion

        protected override void Awake() => armourParts = new int[3] { armour, armour, armour};
    }
}