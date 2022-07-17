using UnityEngine;
using UnityEngine.Events;
using BattleShips.GUI;

namespace BattleShips.GameComponents.Ships
{
    internal abstract class Ship : ScriptableObject
    {
        #region CachedFields

        protected int[] armourParts;

        #endregion

        #region Serialized Fields

        [SerializeField] internal string baseUID;
        [SerializeField] protected GameObject model;
        [SerializeField] internal Sprite cardSprite;
        [SerializeField] protected Vector3 preferedScale;
        [SerializeField] protected Vector3 normalRotation;
        [SerializeField] protected float preferredHeight;
        [SerializeField] protected int armour;
        [SerializeField] protected Weapon weapon1;
        [SerializeField] protected Weapon weapon2;

        #endregion

        #region Public Fields/Properties

        internal GameShipWrapper wrapper;
        internal Vector3 PreferedScale => preferedScale;
        internal Vector3 NormalRotation => normalRotation;
        internal GameObject Model => model;

        internal ShipHit shipHit;
        internal Sprite CardSprite => cardSprite;
        internal int Armour => armour;
        internal float PreferredHeigth => preferredHeight;
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

        internal void UpdateUI()
        {
            float sum = 0;

            foreach (var part in armourParts)
                sum+=part;
            wrapper.durabilityIndicator.UpdateIndicators(sum/(armour*Length));
        }

        internal static int GetLength(ShipType type) => type switch
        {
            ShipType.Battleship => 4,
            ShipType.Destroyer => 2,
            ShipType.Submarine => 3,
            ShipType.Carrier => 5,
            ShipType.Cruiser => 3,
            _ => 0,
        };
    }
}