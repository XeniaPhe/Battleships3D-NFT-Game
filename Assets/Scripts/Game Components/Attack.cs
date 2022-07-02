using BattleShips.GameComponents.Tiles;

namespace BattleShips.GameComponents
{
    internal struct Attack
    {
        internal Coordinate coordinates;
        internal int attackPower;

        internal Attack(Coordinate coordinates,int attackPower)
        {
            this.attackPower = attackPower;
            this.coordinates = coordinates;
        }
    }
}