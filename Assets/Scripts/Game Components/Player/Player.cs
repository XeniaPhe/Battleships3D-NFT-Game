using UnityEngine;
using BattleShips.GameComponents.Ships;

namespace BattleShips.GameComponents.Player
{
    internal abstract class Player : MonoBehaviour
    {
        protected abstract void Awake();
        protected virtual void Start() { }
        internal virtual void Initialize() { }
        internal abstract AttackResult CheckTile(Attack attack);
        internal abstract Ship GetShip(ShipType shipType);
        internal abstract Destroyer GetDestroyer();
        internal abstract Cruiser GetCruiser();
        internal abstract Submarine GetSubmarine();
        internal abstract Battleship GetBattleship();
        internal abstract Carrier GetCarrier();
    }
}