using UnityEngine;
using BattleShips.GameComponents.Tiles;
using BattleShips.GameComponents.Ships;

namespace BattleShips.GameComponents
{
    internal interface IPlayer
    {
        internal AttackResult CheckTile(Attack attack);
    }
}