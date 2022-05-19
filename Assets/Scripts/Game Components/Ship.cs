using UnityEngine;
using UnityEngine.Events;
using BattleShips.GameComponents.Tiles;

namespace BattleShips.GameComponents
{
    internal abstract class Ship : ScriptableObject
    {
        #region CachedFields

        protected int[] armourParts;

        #endregion

        #region Serialized Fields

        [SerializeField] protected Sprite image;
        [SerializeField] protected int armour;
        [SerializeField] protected Weapon weapon1;
        [SerializeField] protected Weapon weapon2;

        #endregion

        #region Public Fields/Properties

        internal Sprite Image => image;
        internal int Armour => armour;
        internal Weapon Weapon1 => weapon1;
        internal Weapon Weapon2 => weapon2;

        #endregion

        #region Nonserialized Public Fields/Properties/Events

        internal event UnityAction ShipPlaced;
        internal abstract int Length { get; }
        internal int this[int index]
        {
            get => armourParts[index];
            set => armourParts[index] = value;
        }

        internal abstract ShipType Type { get; }

        #endregion

        protected abstract void Awake();
        internal void OnShipPlaced() => ShipPlaced?.Invoke();
    }
}