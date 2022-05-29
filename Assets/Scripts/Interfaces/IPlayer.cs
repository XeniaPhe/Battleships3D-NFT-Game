using UnityEngine;
using BattleShips.GameComponents.Tiles;
using BattleShips.Management;

namespace BattleShips.GameComponents
{
    internal interface IPlayer
    {
        internal AttackResult CheckTile(Attack attack);
        internal Attack PlayRandom(Coordinate hit = null);
        internal void PlaceShipsRandom();
    }
}