using System;
using UnityEngine;

namespace BattleShips.GameComponents
{
    [Serializable]
    public struct ShipBundle
    {
        [SerializeField] internal Destroyer destroyer;
        [SerializeField] internal Cruiser cruiser;
        [SerializeField] internal Submarine submarine;
        [SerializeField] internal Battleship battleship;
        [SerializeField] internal Carrier carrier;

        internal ShipBundle(Destroyer destroyer = null, Cruiser cruiser = null, Submarine submarine = null, Battleship battleship = null, Carrier carrier = null)
        {
            this.destroyer = destroyer;
            this.cruiser = cruiser;
            this.submarine = submarine;
            this.battleship = battleship;
            this.carrier = carrier;
        }

        internal Ship this[int index]
        {
            get => index == 0 ? destroyer : index == 1 ? cruiser : index == 2 ? submarine : index == 3 ? battleship : index == 4 ? carrier : null;
            set
            {
                switch (index)
                {
                    case 0: destroyer = (Destroyer)value; break;
                    case 1: cruiser = (Cruiser)value; break;
                    case 2: submarine = (Submarine)value; break;
                    case 3: battleship = (Battleship)value; break;
                    case 4: carrier = (Carrier)value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        internal bool IsBundleFull() => destroyer is not null && cruiser is not null && submarine is not null && battleship is not null && carrier is not null;
    }
}