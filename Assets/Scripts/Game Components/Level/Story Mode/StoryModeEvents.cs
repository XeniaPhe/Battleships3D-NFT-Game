namespace BattleShips.GameComponents.Levels.StoryMode
{
    internal class StoryModeEvents
    {
        private bool firstMove;
        private int firstMoveCallCounter;

        private bool firstMiss;
        private bool firstHit;

        private bool destroyFirstShip;

        private bool oneShipRemaining;

        #region Properties

        internal bool FirstMove
        {
            get
            {
                bool val = firstMove;
                firstMoveCallCounter++;

                if (val && firstMoveCallCounter == 2)
                    firstMove = false;

                return val;
            }
        }

        internal bool FirstMiss
        {
            get
            {
                bool val = firstMiss;

                if (val)
                    firstMiss = false;

                return val;
            }
        }

        internal bool FirstHit
        {
            get
            {
                bool val = firstHit;

                if (val)
                    firstHit = false;

                return val;
            }
        }

        internal bool DestroyFirstShip
        {
            get
            {
                bool val = destroyFirstShip;

                if (val)
                    destroyFirstShip = false;

                return val;
            }
        }

        internal bool OneShipRemaining
        {
            get
            {
                bool val = oneShipRemaining;

                if(val)
                    oneShipRemaining = false;

                return val;
            }
        }

        #endregion

        internal StoryModeEvents()
        {
            firstMove = true;
            firstMiss = true;
            firstHit = true;
            oneShipRemaining = true;

            firstMoveCallCounter = 0;
        }
    }
}