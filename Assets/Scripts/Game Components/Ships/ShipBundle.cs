using System;
using UnityEngine;

namespace BattleShips.GameComponents.Ships
{
    [Serializable]
    public class ShipBundle : ICloneable
    {
        [SerializeField] bool containsDestroyer;
        [SerializeField] bool containsCruiser;
        [SerializeField] bool containsSubmarine;
        [SerializeField] bool containsBattleship;
        [SerializeField] bool containsCarrier;

        [SerializeField] Destroyer destroyer;
        [SerializeField] Cruiser cruiser;
        [SerializeField] Submarine submarine;
        [SerializeField] Battleship battleship;
        [SerializeField] Carrier carrier;

        internal ShipBundle() : this(true, true, true, true, true) { }

        internal ShipBundle(bool containsDestroyer, bool containsCruiser, bool containsSubmarine, bool containsBattleship, bool containsCarrier)
        {
            this.containsDestroyer = containsDestroyer;
            this.containsCruiser = containsCruiser;
            this.containsSubmarine = containsSubmarine;
            this.containsBattleship = containsBattleship;
            this.containsCarrier = containsCarrier;
        }

        internal bool ContainsDestroyer => containsDestroyer;
        internal bool ContainsCruiser => containsCruiser;
        internal bool ContainsSubmarine => containsSubmarine;
        internal bool ContainsBattleship => containsBattleship;
        internal bool ContainsCarrier => containsCarrier;

        internal Destroyer Destroyer
        {
            get => destroyer;
            set
            {
                if (containsDestroyer)
                    destroyer = value;
            }
        }

        internal Cruiser Cruiser
        {
            get => cruiser;
            set
            {
                if (containsCruiser)
                    cruiser = value;
            }
        }

        internal Submarine Submarine
        {
            get => submarine;
            set
            {
                if(containsSubmarine)
                    submarine = value;
            }
        }

        internal Battleship Battleship
        {
            get => battleship;
            set
            {
                if (containsBattleship)
                    battleship = value;
            }
        }

        internal Carrier Carrier
        {
            get => carrier;
            set
            {
                if (containsCarrier)
                    carrier = value;
            }
        }

        internal bool IsFull
        {
            get
            {
                int count = 0;

                if (destroyer is not null)
                    count++;
                if (cruiser is not null)
                    count++;
                if (submarine is not null)
                    count++;
                if (battleship is not null)
                    count++;
                if (carrier is not null)
                    count++;

                return count == FullCount;
            }
        }

        internal int FullCount
        {
            get
            {
                int count = 0;

                if (containsBattleship)
                    count++;
                if (containsCarrier)
                    count++;
                if (containsCruiser)
                    count++;
                if (containsDestroyer)
                    count++;
                if (containsSubmarine)
                    count++;

                return count;
            }
        }

        internal Ship this[int index]
        {
            get => index switch
            {
                0 => destroyer,
                1 => cruiser,
                2 => submarine,
                3 => battleship,
                4 => carrier,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
            set
            {
                switch (index)
                {
                    case 0: Destroyer = (Destroyer)value; break;
                    case 1: Cruiser = (Cruiser)value; break;
                    case 2: Submarine = (Submarine)value; break;
                    case 3: Battleship = (Battleship)value; break;
                    case 4: Carrier = (Carrier)value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        internal void Assign(Ship ship)
        {
            if (ship is null)
                return;

            switch (ship.Type)
            {
                case ShipType.Destroyer:
                    Destroyer = (Destroyer)ship;
                    break;
                case ShipType.Cruiser:
                    Cruiser = (Cruiser)ship;
                    break;
                case ShipType.Submarine:
                    Submarine = (Submarine)ship;
                    break;
                case ShipType.Battleship:
                    Battleship = (Battleship)ship;
                    break;
                case ShipType.Carrier:
                    Carrier = (Carrier)ship;
                    break;
            }
        }

        public object Clone()
        {
            ShipBundle clone = new ShipBundle(containsDestroyer, containsCruiser, containsSubmarine, containsBattleship, containsCarrier);

            clone.destroyer = destroyer;
            clone.submarine = submarine;
            clone.cruiser = cruiser;
            clone.battleship = battleship;
            clone.carrier = carrier;

            return clone;
        }
    }
}