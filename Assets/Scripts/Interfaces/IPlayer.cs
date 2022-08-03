using UnityEngine;
using BattleShips.GameComponents.Tiles;
using BattleShips.GameComponents.Ships;

namespace BattleShips.GameComponents
{
    internal interface IPlayer
    {
        public AttackResult CheckTile(Attack attack);
        public void MakeMove();
        public Ship GetShip(ShipType shipType);
    }
}