using System;
using System.Collections.Generic;
using UnityEngine;
using BattleShips.Utils;
using BattleShips.GameComponents.Tiles;

namespace BattleShips.GameComponents.AI
{
    internal class AI : MonoBehaviour,IPlayer
    {
        #region Singleton

        static AI instance;
        internal static AI Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] ShipBundle[] shipBundles;
        [SerializeField] int currentLevel = 1;

        #endregion

        #region Cached Fields

        readonly int[] lengths = { 2, 3, 4, 5 };

        TileData[,] tiles = new TileData[10, 10];
        List<TileData> moveLog = new List<TileData>();
        int[,] heatMap = new int[10, 10];
        double[,] probabilityMap = new double[10, 10];
        TileData tileFound;
        AIMode mode = AIMode.Hunt;
        Parity parity;

        #endregion

        #region Ship Bitmask

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

            return (shipFlag & (byte)(1 << ship)) == 1;
        }

        /// <summary>
        /// Sets a ship as destroyed
        /// </summary>
        /// <param name="ship">Destroyer = 0, Submarine=1, Cruiser=2, Battleship=3, Carrier=4</param>
        /// <exception cref="ArgumentOutOfRangeException">Value of argument ship should be between 0 and 4 (inclusive)</exception>
        private void SetShipDestroyed(byte ship)
        {
            if (ship > 4)
                throw new ArgumentOutOfRangeException("ship", ship, "Ship should be between 0 and 4!");

            shipFlag &= (byte)(~(1 << ship));
        }

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;

                TileData tile;

                for (int i = 1; i < 11; i++)
                    for (int j = 1; j < 11; j++)
                    {
                        tile = new TileData(i, j);
                        tiles[i - 1, j - 1] = tile;
                    }
            }
        }

        private TileData GetTileAt(Coordinate coords) => tiles[coords.X-1,coords.Y-1];
        private double GetProbabilityAt(int n) => probabilityMap[n / 10, n % 10];
        private double GetProbabilityAt(Coordinate coords) => probabilityMap[coords.X - 1, coords.Y - 1];
        private double GetRealProbabilityAt(int n)
        {
            double pn = probabilityMap[n / 10, n % 10];
            if(n == 0)
                return pn;
            --n;
            return pn - probabilityMap[n / 10, n % 10];
        }
        private double GetRealProbabilityAt(Coordinate coords)
        {
            double pn = GetProbabilityAt(coords);
            if (coords.GetCoordinates(true) == Vector2Int.zero)
                return pn;
            Coordinate prev = coords.Previous;
            return pn - GetProbabilityAt(prev);
        }
        private void MakeDecision()
        {

        }

        private async void RecalculateHeatMap()
        {
            int CheckShips(int l)
            {
                if ((l == 2 && !CheckShip(0)) || (l == 4 && !CheckShip(3)) || (l == 5 && !CheckShip(4)))
                    return 0;

                if (l == 3)
                {
                    if (!CheckShip(1) && !CheckShip(2))
                        return 0;
                    if (CheckShip(1) && CheckShip(2))
                        return 2;
                }

                return 1;
            }

            int count;
            if (moveLog.Count > 0 && moveLog[moveLog.Count-1].tileState == TileState.Miss)
            {
                var coord = moveLog[moveLog.Count - 1].Coordinates;
                heatMap[coord.X - 1, coord.Y - 1] = 0;
                Coordinate left = coord.Left,  right = coord.Right, down = coord.Down,up  = coord.Up;

                foreach (var l in lengths)
                {
                    if ((count = CheckShips(l)) == 0)
                        continue;

                    Vector2Int index;
                    if(left is not null)
                    {
                        index = left.GetCoordinates(true);
                        if (heatMap[index.x, index.y] != 0)
                            heatMap[index.x, index.y] -= count;
                        left = left.Left;
                    }
                    if(right is not null)
                    {
                        index = right.GetCoordinates(true);
                        if (heatMap[index.x, index.y] != 0)
                            heatMap[index.x, index.y] -= count;
                        right = right.Right;
                    }
                    if (down is not null)
                    {
                        index = down.GetCoordinates(true);
                        if (heatMap[index.x, index.y] != 0)
                            heatMap[index.x, index.y] -= count;
                        down = down.Down;
                    }
                    if (up is not null)
                    {
                        index = up.GetCoordinates(true);
                        if (heatMap[index.x, index.y] != 0)
                            heatMap[index.x, index.y] -= count;
                        up = up.Up;
                    }
                }
                return;
            }
            else
            {
                heatMap = new int[10, 10];

                foreach (var l in lengths)
                {
                    if ((count = CheckShips(l)) == 0)
                        continue;

                    //Works faster with rather filled grids

                    //for (int x = 0; x < 10; x++)
                    //    for (int y = 0; y < 11 - l; y++)
                    //    {
                    //        int j;
                    //        for (j = y; j < y + l; j++)
                    //            if (tiles[x, j].tileState != TileState.Normal)
                    //                break;
                    //        if (j == y + l)
                    //            for (j = y; j < y + l; j++)
                    //                heatMap[x, j] += count;
                    //    }

                    //Works faster with rather empty grids

                    for (int x = 0; x < 10; x++)
                        for (int y = 0; y < 11 - l; y++)
                            for (int j = y; j < y + l; j++)
                            {
                                if (tiles[x, j].tileState != TileState.Normal)
                                {
                                    for (int k = j - 1; k >= y; --k)
                                        heatMap[x, k] -= count;
                                    break;
                                }
                                heatMap[x, j] += count;
                            }

                    for (int x = 0; x < 11 - l; x++)
                        for (int y = 0; y < 10; y++)
                            for (int j = x; j < x + l; j++)
                            {
                                if (tiles[j, y].tileState != TileState.Normal)
                                {
                                    for (int k = j - 1; k >= x; --k)
                                        heatMap[k, y] -= count;
                                    break;
                                }
                                heatMap[j, y] += count;
                            }
                }
            }
            RecalculateProbabilityMap();
        }

        private void RecalculateProbabilityMap(bool parity = true)
        {
            double oddSum = 0, evenSum = 0;
            int oddCount = 0, evenCount = 0, tempInt;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if ((i % 2) == (j % 2))   //Odd parity(I named it odd coz it includes the first tile)
                    {
                        oddSum += (tempInt = heatMap[i, j]);
                        if (tempInt != 0) ++oddCount;
                    }
                    else    //Even parity
                    {
                        evenSum += (tempInt = heatMap[i, j]);
                        if (tempInt != 0) ++evenCount;
                    }

            
            double sum = oddSum + evenSum;

            if (parity)
                if (evenSum / evenCount >= oddSum / oddCount) this.parity = Parity.Even;
                else this.parity = Parity.Odd;
            else this.parity = Parity.Off;

            double p, d, ad, d2, c = Math.PI * Math.E / 7;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    p = heatMap[i, j];
                    if (p == 0)
                    {
                        probabilityMap[i, j] = 0;
                        continue;
                    }
                    else if(this.parity == Parity.Odd)
                    {
                        if ((i % 2) != (j % 2))
                        {
                            probabilityMap[i, j] = 0;
                            continue;
                        }
                    }
                    else if(this.parity == Parity.Even)
                    {
                        if ((i % 2) == (j % 2))
                        {
                            probabilityMap[i, j] = 0;
                            continue;
                        }
                    }

                    d = (p - 14);
                    ad = Math.Abs(d);
                    d2 = Math.Pow(Math.Log10(ad), 12) + Helper.Root2n(ad, 2) + Helper.Root2n(ad, 4) + Helper.Root2n(ad, 6);
                    probabilityMap[i, j] = p + Math.Sign(d) * d2 * c;
                }

            

            double prob = 0;
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    prob += (probabilityMap[i, j] * 100 / sum);
                    probabilityMap[i, j] = prob;
                }
        }

        private Coordinate SearchProbability(double val)
        {
            int low = 0;
            int high = 99;
            int mid = 0;
            double prob;

            while (high>low)
            {
                mid = (low + high) / 2;
                prob = GetProbabilityAt(mid);

                if (prob < val) low = mid + 1;
                else if (prob > val) high = mid - 1;
                else break;
            }

            double lowVal = GetProbabilityAt(low);
            double highVal = GetProbabilityAt(high);
            double midVal = GetProbabilityAt(mid);

            if (val > lowVal && val <= midVal)
                return new Coordinate(mid / 10, mid % 10, true);
            else if (val > midVal && val <= highVal)
                return new Coordinate(high / 10, high % 10, true);
            else if (low == 0)
                return new Coordinate(1, 1);
            else
                throw new Exception("You made an index error dumbass");
        }
        private void AttackTile()
        {
            switch (mode)
            {
                case AIMode.Hunt:
                    double rand = Helper.Random100();
                    var coords = SearchProbability(rand);
                    TileData tileToAttack = GetTileAt(coords);
                    break;
                case AIMode.Destroy:
                    if (parity != Parity.Off)
                        RecalculateProbabilityMap(false);

                    List<double> destroyProbabilityMap = new List<double>();

                        //Make a tile list for neighbors and remove from the list as they're attacked
                    break;

                default:
                    throw new Exception("AIMode should be Hunt or Destroy");
            }

        }

        public void Attack()
        {
            
        }

        #region Temporary
        internal void PlaceShipsRandom()
        {
            bool horizontal;
            TileData tile;
            int startIndex;
            int otherDimension;
            int j = 0;

            for (int i = 0; i < 5; i++)
            {
                j = 0;
                horizontal = (UnityEngine.Random.Range(0, 2) & 1) == 0;

                while (j != lengths[i])
                {
                    otherDimension = UnityEngine.Random.Range(0, 10);
                    startIndex = UnityEngine.Random.Range(0, 11 - lengths[i]);

                    for (j = 0; j < lengths[i]; j++)
                    {
                        tile = horizontal ? tiles[startIndex + j, otherDimension] : tiles[otherDimension, startIndex + j];

                        if (tile.ship || tile.tileState != TileState.Normal)
                            break;

                        tile.ship = shipBundles[currentLevel - 1][i];
                        tile.tileState = TileState.HasShip;
                    }
                }
            }
        }

        private void PrintTileInformation()
        {
            string board = "";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                    board += tiles[i, j].tileState == TileState.Normal ? "[0]" : "[+]";
                Debug.Log(board);
                board = "";
            }
        }

 

        #endregion
    }
}