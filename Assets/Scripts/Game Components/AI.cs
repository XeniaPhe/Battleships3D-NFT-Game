using System;
using System.Collections.Generic;
using UnityEngine;
using BattleShips.Utils;
using BattleShips.GameComponents.Tiles;
using BattleShips.GameComponents.Ships;

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

        readonly int[] lengths = { 2, 3, 3, 4, 5 };
        TileData[,] tiles = new TileData[10, 10];
        TileData[,] enemyTiles = new TileData[10, 10];
        ShipBundle deck;
        ShipFlag ships = new ShipFlag();
        ShipFlag enemyShips = new ShipFlag();
        AIMode mode = AIMode.Search;
        Dictionary<Directions, int> directionsGone = new Dictionary<Directions, int>();
        Directions? foundShipDirection = null;
        TileData tileHit = null;
        Coordinate lastAttack;

        List<TileData> moveLog = new List<TileData>();
        int[,] heatMap = new int[10, 10];
        double[,] probabilityMap = new double[10, 10];
        Parity parity;

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
                {
                    for (int j = 1; j < 11; j++)
                    {
                        tile = new TileData(i, j);
                        tiles[i - 1, j - 1] = tile;
                        tile = new TileData(i, j);
                        enemyTiles[i - 1, j - 1] = tile;
                    }
                }

                var bundle = shipBundles[currentLevel-1];

                deck = new ShipBundle();
                deck.battleship = Instantiate<Battleship>(bundle.battleship, null);
                deck.destroyer = Instantiate<Destroyer>(bundle.destroyer, null);
                deck.cruiser = Instantiate<Cruiser>(bundle.cruiser, null);
                deck.carrier = Instantiate<Carrier>(bundle.carrier, null);
                deck.submarine = Instantiate<Submarine>(bundle.submarine, null);

                SetAll(deck.submarine);
                SetAll(deck.carrier);
                SetAll(deck.destroyer);
                SetAll(deck.cruiser);
                SetAll(deck.battleship);

                void SetAll(Ship ship)
                {
                    for (int i = 0; i < ship.Length; i++)
                    {
                        ship[i] = 80;
                    }
                }
            }
        }
        private TileData GetTileAt(TileData[,] tiles,Coordinate coords) => tiles[coords.X - 1, coords.Y - 1];
        private bool CheckTileAvailability(TileData[,] tiles, Coordinate coords) => GetTileAt(tiles, coords).tileState == TileState.Normal;

        #region On Development

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
            if (coords.GetCoordinateVector(true) == Vector2Int.zero)
                return pn;
            Coordinate prev = coords.Previous;
            return pn - GetProbabilityAt(prev);
        }

        private void RecalculateHeatMap()
        {
            int CheckShips(int l)
            {
                if ((l == 2 && !ships.IsDestroyerDestroyed()) || (l == 4 && !ships.IsBattleshipDestroyed()) || (l == 5 && !ships.IsCarrierDestroyed()))
                    return 0;

                if (l == 3)
                {
                    if (!ships.IsSubmarineDestroyed() && !ships.IsCruiserDestroyed())
                        return 0;
                    if (ships.IsSubmarineDestroyed() && ships.IsCruiserDestroyed())
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
                        index = left.GetCoordinateVector(true);
                        if (heatMap[index.x, index.y] != 0)
                            heatMap[index.x, index.y] -= count;
                        left = left.Left;
                    }
                    if(right is not null)
                    {
                        index = right.GetCoordinateVector(true);
                        if (heatMap[index.x, index.y] != 0)
                            heatMap[index.x, index.y] -= count;
                        right = right.Right;
                    }
                    if (down is not null)
                    {
                        index = down.GetCoordinateVector(true);
                        if (heatMap[index.x, index.y] != 0)
                            heatMap[index.x, index.y] -= count;
                        down = down.Down;
                    }
                    if (up is not null)
                    {
                        index = up.GetCoordinateVector(true);
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
                                if (enemyTiles[x, j].tileState != TileState.Normal)
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
                                if (enemyTiles[j, y].tileState != TileState.Normal)
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
                    TileData tileToAttack = GetTileAt(tiles,coords);
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

        #endregion

        #region Temporary

        private void PrintTileInformation()
        {
            string board = "";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (tiles[i, j].ship == null)
                        board += "[0]";
                    else if (tiles[i, j].ship == deck.submarine)
                        board += "[S]";
                    else if (tiles[i, j].ship == deck.battleship)
                        board += "[B]";
                    else if (tiles[i, j].ship == deck.cruiser)
                        board += "[C]";
                    else if (tiles[i, j].ship == deck.carrier)
                        board += "[A]";
                    else
                        board += "[D]";
                }

                Debug.Log(board);
                board = "";
            }
        }

        void IPlayer.PlaceShipsRandom()
        {
            bool horizontal;
            TileData tile;
            int startIndex;
            int otherDimension;
            int j = 0;
            List<TileData> placing = new List<TileData>();

            for (int i = 0; i < 5; i++)
            {
                j = 0;

                horizontal = (UnityEngine.Random.Range(0, 2) & 1) == 0;

                while (j != lengths[i])
                {

                    otherDimension = UnityEngine.Random.Range(0, 10);
                    startIndex = UnityEngine.Random.Range(0, 11 - lengths[i]);

                    placing.Clear();

                    for (j = 0; j < lengths[i]; j++)
                    {
                        tile = horizontal ? tiles[otherDimension, startIndex + j] : tiles[startIndex + j, otherDimension];
                        if (tile.ship || tile.tileState != TileState.Normal)
                            break;
                        else
                            placing.Add(tile);
                    }
                }

                for (j = 0; j < placing.Count; j++)
                {
                    placing[j].ship = deck[i];
                    placing[j].tileState = TileState.HasShip;
                    placing[j].shipIndex = j;
                    placing[j].startTile = placing[0];
                    placing[j].shipDirection = horizontal ? Directions.Right : Directions.Down;
                }
            }

            PrintTileInformation();
        }

        AttackResult IPlayer.CheckTile(Attack attack)
        {
            Vector2Int vectorCoords = attack.coordinates.GetCoordinateVector(true);
            var tileData = tiles[vectorCoords.x, vectorCoords.y];
            var ship = tileData.ship;

            if (ship is null)
            {
                tileData.tileState = TileState.Miss;
                return AttackResult.Miss;
            }
            else if (ship[tileData.shipIndex] > 0)
            {
                if ((ship[tileData.shipIndex] -= attack.attackPower) <= 0)
                    tileData.tileState = TileState.HasDestroyedShipPart;
                else
                    tileData.tileState = TileState.HasHitShip;

                for (int i = 0; i < ship.Length; i++)
                    if (ship[i] > 0)
                        return AttackResult.Hit;

                ships.SetShipDestroyed(ship.Type);
                Coordinate coords = tileData.startTile.Coordinates;
                var direction = tileData.shipDirection;

                for (int i = 0; i < ship.Length; i++)
                {
                    try
                    {
                        vectorCoords = coords.GetCoordinateVector(true);
                        tiles[vectorCoords.x, vectorCoords.y].tileState = TileState.HasSunkenShip;
                        coords = coords.GetCoordinatesAt(direction);
                    }
                    catch (Exception)
                    {
                        Debug.Log("Coords : " + coords?.ToString());
                        Debug.Log("Start Coords : " + tileData.startTile.Coordinates.ToString());
                        Debug.Log("Direction : " + direction);
                    }
                }

                if (ships.AreAllDestroyed())
                    return AttackResult.AllDestroyed;
                else return ship.Type switch
                {
                    ShipType.Destroyer => AttackResult.DestroyerDestroyed,
                    ShipType.Submarine => AttackResult.SubmarineDestroyed,
                    ShipType.Cruiser => AttackResult.CruiserDestroyed,
                    ShipType.Battleship => AttackResult.BattleshipDestroyed,
                    ShipType.Carrier => AttackResult.CarrierDestroyed,
                    _ => throw new Exception("Undefined Ship type!")
                };
            }

            return AttackResult.Miss;
        }

        Attack IPlayer.PlayRandom(Coordinate hit = null,ShipType? sunkenShip = null)
        {
            if (mode == AIMode.Search && hit != null && sunkenShip == null)
            {
                mode = AIMode.Hunt;
                GetTileAt(enemyTiles, lastAttack).tileState = TileState.HasHitShip;
                tileHit = GetTileAt(enemyTiles, hit);
                directionsGone = new Dictionary<Directions, int>();
                var possibleDirections = hit.GetNeighborDirections();
                foreach (var dir in possibleDirections)
                    directionsGone.Add(dir, 0);
            }

            if (mode == AIMode.Hunt)
            {
                if (hit != null &&  tileHit != GetTileAt(enemyTiles,hit) && !foundShipDirection.HasValue)
                {
                    foundShipDirection = Coordinate.GetDirection(tileHit.Coordinates,hit);
                    directionsGone[foundShipDirection.Value] = 1;
                    mode = AIMode.Destroy;
                }
                else if(hit == null)
                {
                    GetTileAt(enemyTiles, lastAttack).tileState = TileState.Miss;
                }

                if (mode != AIMode.Destroy)
                {
                    int direction;
                    bool found = true;
                    do
                    {
                        direction = UnityEngine.Random.Range(0, 4);
                        found = true;

                        lastAttack = tileHit.Coordinates.GetCoordinatesAt((Directions)direction);
                        if (!directionsGone.ContainsKey((Directions)direction))
                        {
                            found = false;
                            continue;
                        }
                        found &= (directionsGone[(Directions)direction] == 0);
                        found &= CheckTileAvailability(enemyTiles, lastAttack);

                    } while (!found);

                    directionsGone[(Directions)direction]++;

                    return new Attack(lastAttack, 80);
                }
            }

            if (mode == AIMode.Destroy)
            {
                if (sunkenShip.HasValue)
                {
                    var cleaner = GetTileAt(enemyTiles, tileHit.Coordinates);
                    if(directionsGone.ContainsKey(foundShipDirection.Value))
                    {
                        int way = directionsGone[foundShipDirection.Value];

                        for (int i = 0; i < way; i++)
                        {
                            cleaner.tileState = TileState.HasSunkenShip;
                            cleaner = GetTileAt(enemyTiles, cleaner.Coordinates.GetCoordinatesAt(foundShipDirection.Value));
                        }
                    }

                    foundShipDirection = Helper.GetOppositeDirection(foundShipDirection.Value);

                    if (directionsGone.ContainsKey(foundShipDirection.Value))
                    {
                        int way = directionsGone[foundShipDirection.Value];

                        for (int i = 0; i < way; i++)
                        {
                            cleaner.tileState = TileState.HasSunkenShip;
                            cleaner = GetTileAt(enemyTiles, cleaner.Coordinates.GetCoordinatesAt(foundShipDirection.Value));
                        }
                    }

                    enemyShips.SetShipDestroyed(sunkenShip.Value);
                    mode = AIMode.Search;
                    tileHit = null;
                    foundShipDirection = null;
                }
                else if (hit == null)
                {
                    GetTileAt(enemyTiles, lastAttack).tileState = TileState.Miss;
                    foundShipDirection = Helper.GetOppositeDirection(foundShipDirection.Value);
                }
                else
                {
                    GetTileAt(enemyTiles, lastAttack).tileState = TileState.HasHitShip;
                }

                if (mode != AIMode.Search)
                {
                    int distanceGone = 0;
                    if(directionsGone.ContainsKey(foundShipDirection.Value))
                        distanceGone = directionsGone[foundShipDirection.Value] + 1;
                    else
                    {
                        mode = AIMode.Search;
                        tileHit = null;
                        foundShipDirection = null;
                        directionsGone = null;
                        return ((IPlayer)this).PlayRandom();
                    }

                    Coordinate coords = tileHit.Coordinates;

                    for (int i = 0; i < distanceGone; i++)
                    {
                        if (coords == null) 
                            break;
                        coords = coords.GetCoordinatesAt(foundShipDirection.Value);
                    }

                    if (coords == null || (coords != null && !CheckTileAvailability(enemyTiles, coords)))
                    {
                        foundShipDirection = Helper.GetOppositeDirection(foundShipDirection.Value);
                        coords = tileHit.Coordinates.GetCoordinatesAt(foundShipDirection.Value);
                    }

                    directionsGone[foundShipDirection.Value]++;
                    lastAttack = coords;

                    if (coords == null)
                        Debug.Log("wtfff");

                    return new Attack(lastAttack, 80);
                }
            }

            if (lastAttack != null && GetTileAt(enemyTiles, lastAttack).tileState != TileState.HasSunkenShip)
                GetTileAt(enemyTiles, lastAttack).tileState = TileState.Miss;

            do
            {
                lastAttack = new Coordinate(UnityEngine.Random.Range(1, 11), UnityEngine.Random.Range(1, 11));
            } while (!CheckTileAvailability(enemyTiles, lastAttack));

            return new Attack(lastAttack, 80);
        }

        #endregion
    }
}