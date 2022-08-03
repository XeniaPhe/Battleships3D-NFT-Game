using UnityEngine;
using BattleShips.GameComponents.Ships;

namespace BattleShips.GameComponents.Player
{
    internal abstract class Player : MonoBehaviour
    {
        protected virtual void Awake() { Instantiate(); }
        protected abstract void Start();
        internal virtual void Instantiate() { }
        internal abstract AttackResult CheckTile(Attack attack);
        internal abstract Ship GetShip(ShipType shipType);
        internal abstract Destroyer GetDestroyer();
        internal abstract Cruiser GetCruiser();
        internal abstract Submarine GetSubmarine();
        internal abstract Battleship GetBattleship();
        internal abstract Carrier GetCarrier();
    }
}