using BattleShips.GameComponents.Ships;

namespace BattleShips.GameComponents
{
    internal sealed class Deck
    {
        #region Singleton

//        private Deck()
//        {

//        }

//#nullable enable

//        static volatile Deck? instance;

//#nullable disable

//        static readonly object locker = new object();
//        internal static Deck Instance
//        {
//            get
//            {
//                lock (locker)
//                    if (instance is null)
//                        instance = new Deck();
//                return instance;
//            }
//        }

        #endregion

        #region Cached Fields

        ShipBundle bundle;

        #endregion

        #region Nonserialized Public Fields/Properties

        internal ShipBundle Bundle
        {
            get => bundle;
            set
            {
                if(value is not null)
                    bundle = value;
            }
        }

        internal Destroyer Destroyer => bundle.Destroyer;

        internal Cruiser Cruiser => bundle.Cruiser;

        internal Submarine Submarine => bundle.Submarine;

        internal Battleship Battleship => bundle.Battleship;

        internal Carrier Carrier => bundle.Carrier;

        #endregion

        internal Deck()
        {
            bundle = new ShipBundle();
        }

        internal void Assign(Ship ship)
        {
            bundle.Assign(ship);
        }

        internal bool IsDeckFull() => bundle.IsFull;
    }
}