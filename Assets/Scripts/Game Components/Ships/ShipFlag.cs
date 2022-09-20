using System;

namespace BattleShips.GameComponents.Ships
{
    internal struct ShipFlag
    {
        byte flag;
        byte shipCount;

        internal int AvailableShipCount => shipCount;

        internal int AliveShipCount
        {
            get
            {
                int count = 0;

                for (byte i = 0; i < 5; i++)
                {
                    if (CheckShip(i))
                        count++;
                }

                return count;
            }
        }

        internal ShipFlag(ShipBundle bundle)
        {
            flag = 0b00000000;
            shipCount = 0;

            if (bundle.ContainsDestroyer)
            {
                flag += 0b00000001;
                shipCount++;
            }
            if (bundle.ContainsSubmarine)
            {
                flag += 0b00000010;
                shipCount++;
            }
            if (bundle.ContainsCruiser)
            {
                flag += 0b00000100;
                shipCount++;
            }
            if (bundle.ContainsBattleship)
            {
                flag += 0b00001000;
                shipCount++;
            }
            if (bundle.ContainsCarrier)
            {
                flag += 0b00010000;
                shipCount++;
            }
        }

        internal ShipFlag(bool destroyer, bool submarine, bool cruiser, bool battleship, bool carrier)
        {
            flag = 0b00000000;
            shipCount = 0;

            if (destroyer)
            {
                flag += 0b00000001;
                shipCount++;
            }
            if (submarine)
            {
                flag += 0b00000010;
                shipCount++;
            }
            if (cruiser)
            {
                flag += 0b00000100;
                shipCount++;
            }
            if (battleship)
            {
                flag += 0b00001000;
                shipCount++;
            }
            if (carrier)
            {
                flag += 0b00010000;
                shipCount++;
            }
        }

        /// <summary>
        /// Checks if a ship is destroyed or not
        /// </summary>
        /// <param name="ship">Destroyer = 0, Submarine=1, Cruiser=2, Battleship=3, Carrier=4</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Value of argument ship should be between 0 and 4 (inclusive)</exception>
        private bool CheckShip(byte ship)
        {
            if (ship > 4)
                throw new ArgumentOutOfRangeException("ship", ship, "Ship should be between 0 and 4!");

            return (flag & (byte)(1 << ship)) > 0;
        }

        /// <summary>
        /// Sets a ship as destroyed
        /// </summary>
        /// <param name="ship">Destroyer = 0, Submarine=1, Cruiser=2, Battleship=3, Carrier=4</param>
        /// <exception cref="ArgumentOutOfRangeException">Value of argument ship should be between 0 and 4 (inclusive)</exception>
        private void SetDestroyed(byte ship)
        {
            if (ship > 4)
                throw new ArgumentOutOfRangeException("ship", ship, "Ship should be between 0 and 4!");

            flag &= (byte)(~(1 << ship));
        }

        internal bool AreAllDestroyed() => flag == 0;
        internal bool IsDestroyerDestroyed() => !CheckShip(0);
        internal bool IsSubmarineDestroyed() => !CheckShip(1);
        internal bool IsCruiserDestroyed() => !CheckShip(2);
        internal bool IsBattleshipDestroyed() => !CheckShip(3);
        internal bool IsCarrierDestroyed() => !CheckShip(4);
        internal void SetShipDestroyed(ShipType type)
        {
            switch (type)
            {
                case ShipType.Destroyer:
                    SetDestroyerDestroyed();
                    break;
                case ShipType.Cruiser:
                    SetCruiserDestroyed();
                    break;
                case ShipType.Submarine:
                    SetSubmarineDestroyed();
                    break;
                case ShipType.Battleship:
                    SetBattleshipDestroyed();
                    break;
                case ShipType.Carrier:
                    SetCarrierDestroyed();
                    break;
                default:
                    throw new ArgumentException("Undefined ship type!");
            }
        }
        internal void SetAllDestroyed() => flag = 0;
        internal void SetDestroyerDestroyed() => SetDestroyed(0);
        internal void SetSubmarineDestroyed() => SetDestroyed(1);
        internal void SetCruiserDestroyed() => SetDestroyed(2);
        internal void SetBattleshipDestroyed() => SetDestroyed(3);
        internal void SetCarrierDestroyed() => SetDestroyed(4);
    }
}