using UnityEngine;

namespace BattleShips.GameComponents.Ships
{
    [CreateAssetMenu(fileName = "Battleship", menuName = "Ships/Battleship")]
    internal class Battleship : Ship
    {
        #region Nonserialized Public Fields/Properties

        internal override int Length => 4;
        internal override ShipType Type => ShipType.Battleship;

        #endregion

        protected override void Awake() => armourParts = new int[4] { armour, armour, armour, armour };
    }
}