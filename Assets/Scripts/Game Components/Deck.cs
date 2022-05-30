using BattleShips.GameComponents.Ships;

namespace BattleShips.GameComponents
{
    internal sealed class Deck
    {
        #region Singleton

        private Deck()
        {

        }

#nullable enable

        static volatile Deck? instance;

#nullable disable

        static readonly object locker = new object();
        internal static Deck Instance
        {
            get
            {
                lock (locker)
                    if (instance is null)
                        instance = new Deck();
                return instance;
            }
        }

        #endregion

        #region Cached Fields

        ShipBundle bundle;

        #endregion

        #region Nonserialized Public Fields/Properties

        internal Destroyer Destroyer => bundle.destroyer;

        internal Cruiser Cruiser => bundle.cruiser;

        internal Submarine Submarine => bundle.submarine;

        internal Battleship Battleship => bundle.battleship;

        internal Carrier Carrier => bundle.carrier;

        #endregion


        internal void Assign(Ship ship)
        {
            if (ship is null)
                return;

            switch (ship.Type)
            {
                case ShipType.Destroyer:
                    bundle.destroyer = (Destroyer)ship;
                    break;
                case ShipType.Cruiser:
                    bundle.cruiser = (Cruiser)ship;
                    break;
                case ShipType.Submarine:
                    bundle.submarine = (Submarine)ship;
                    break;
                case ShipType.Battleship:
                    bundle.battleship = (Battleship)ship;
                    break;
                case ShipType.Carrier:
                    bundle.carrier = (Carrier)ship;
                    break;
            }
        }

        internal bool IsDeckFull() => bundle.IsBundleFull();
    }
}