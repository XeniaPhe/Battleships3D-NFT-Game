using System;
using UnityEngine;
using UnityEngine.Events;
using BattleShips.GUI;
using BattleShips.GameComponents.Tiles;
using BattleShips.VFX;

namespace BattleShips.GameComponents.Ships
{
    internal abstract class Ship : ScriptableObject
    {
        #region CachedFields

        protected int[] armourParts;

        #endregion

        #region Serialized Fields

        [SerializeField] internal string shipName;
        [SerializeField] internal string baseUID;
        [SerializeField] protected GameObject model;
        [SerializeField] internal Sprite cardSprite;
        [SerializeField] protected Vector3 preferedScale;
        [SerializeField] protected Vector3 normalRotation;
        [SerializeField] protected float preferedHeight;
        [SerializeField] protected int armour;
        [SerializeField] protected Weapon weapon1;
        [SerializeField] protected Weapon weapon2;

        #endregion

        #region Public Fields/Properties

        internal GameShipWrapper wrapper;
        internal Vector3 PreferedScale => preferedScale;
        internal Vector3 NormalRotation => normalRotation;
        internal GameObject Model => model;

        internal ShipExploder shipExploder;
        internal Sprite CardSprite => cardSprite;
        internal int Armour => armour;
        internal float PreferedHeigth => preferedHeight;
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

        internal Transform InstantiateShip(Vector3 position,Direction direction,PlayerType owner)
        {
            Vector3 rotation = normalRotation;

            if (Length % 2 == 0)
            {
                switch (direction)
                {
                    case Direction.Right:
                        position += (Vector3.back);
                        break;
                    case Direction.Up:
                        position += (Vector3.right);
                        break;
                    case Direction.Left:
                        position += (Vector3.forward);
                        break;
                    case Direction.Down:
                        position += (Vector3.left);
                        break;
                }
            }

            position.y = preferedHeight;
            rotation.y = (int)(direction) * 90;

            var shipInstance = Instantiate<Transform>(model.transform, position, Quaternion.Euler(rotation), null);
            shipInstance.localScale = preferedScale;

            shipInstance.name = Type.ToString();
            shipInstance.tag = "Ship Instance";
            return shipInstance;
        }
        internal static ShipType GetShipType(AttackResult attackResult) => attackResult switch
        {
            AttackResult.DestroyerDestroyed => ShipType.Destroyer,
            AttackResult.SubmarineDestroyed => ShipType.Submarine,
            AttackResult.CruiserDestroyed => ShipType.Cruiser,
            AttackResult.BattleshipDestroyed => ShipType.Battleship,
            AttackResult.CarrierDestroyed => ShipType.Carrier,
            _ => throw new ArgumentException("Attack result ?? "),
        };
    }
}