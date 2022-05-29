using System;

namespace BattleShips.GameComponents
{
    internal class ShipFlag
    {
        byte shipFlag = 0b00011111;

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

            return (shipFlag & (byte)(1 << ship)) > 0;
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

            shipFlag &= (byte)(~(1 << ship));
        }

        internal bool AreAllDestroyed() => shipFlag == 0;
        internal bool IsDestroyerDestroyed() => CheckShip(0);
        internal bool IsSubmarineDestroyed() => CheckShip(1);
        internal bool IsCruiserDestroyed() => CheckShip(2);
        internal bool IsBattleshipDestroyed() => CheckShip(3);
        internal bool IsCarrierDestroyed() => CheckShip(4);
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
        internal void SetDestroyerDestroyed() => SetDestroyed(0);
        internal void SetSubmarineDestroyed() => SetDestroyed(1);
        internal void SetCruiserDestroyed() => SetDestroyed(2);
        internal void SetBattleshipDestroyed() => SetDestroyed(3);
        internal void SetCarrierDestroyed() => SetDestroyed(4);
    }
}