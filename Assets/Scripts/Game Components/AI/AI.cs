using BattleShips.GameComponents.Ships;
using BattleShips.GameComponents.Tiles;
using BattleShips.Management;
using BattleShips.Utils;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleShips.GameComponents.AI
{
    internal class AI : MonoBehaviour, IPlayer
    {
        #region Singleton

        static AI instance;
        internal static AI Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] AILevel[] levels;

        #endregion

        #region Cached Fields

        GameManager manager;
        AILevel currentLevel;

        readonly int[] lengths = { 2, 3, 4, 5 };
        readonly int[] lengthsRepeated = { 2, 3, 3, 4, 5 };
        TileData[,] tiles = new TileData[10, 10];
        TileData[,] enemyTiles = new TileData[10, 10];

        ShipBundle deck;
        ShipFlag ships = new ShipFlag();
        ShipFlag enemyShips = new ShipFlag();

        int[,] heatMap = new int[10, 10];
        double[,] distortionMatrix = new double[10, 10];
        List<Tuple<double, Coordinate>> probabilityList = new List<Tuple<double, Coordinate>>();
        Parity parity;


        AIMode mode = AIMode.Seek;
        List<Tuple<Direction, int, double>> validDirections = new List<Tuple<Direction, int, double>>();
        Direction? searchDirection = null;
        int[] marker = new int[3]; //marker[0] = negative dir,marker[1] = first tile,marker[2] = positive dir 
        Coordinate lastAttack;
        Coordinate firstTile;
        bool isPositiveDirection = true;
        bool shipNewlyDestroyed = false;
        bool newlySwappedDirection = false;
        bool terminationDirection = true;
        Queue<Coordinate> terminationQueue = new Queue<Coordinate>();
        List<Coordinate> hitPath = new List<Coordinate>();

        #endregion

        #region Instantiation

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                InstantiateAI(0);
            }
        }
        private void Start()
        {
            manager = GameManager.Instance;
        }
        internal void InstantiateAI(int level)
        {
            currentLevel = levels[level];
            parity = currentLevel.Parity ? Parity.Even : Parity.Off;

            InstantiateTiles();
            InstantiateDeck();
            RecalculateHeatMap();
            CalculateDistortionMatrix();
            RecalculateProbabilityList();
            PlaceShips();
        }
        private void InstantiateTiles()
        {
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
        }
        private void InstantiateDeck()
        {
            deck = new ShipBundle();
            var bundle = currentLevel.ShipBundle;

            deck.battleship = Instantiate<Battleship>(bundle.battleship, null);
            deck.destroyer = Instantiate<Destroyer>(bundle.destroyer, null);
            deck.cruiser = Instantiate<Cruiser>(bundle.cruiser, null);
            deck.carrier = Instantiate<Carrier>(bundle.carrier, null);
            deck.submarine = Instantiate<Submarine>(bundle.submarine, null);

            SetAllParts(deck.submarine);
            SetAllParts(deck.carrier);
            SetAllParts(deck.destroyer);
            SetAllParts(deck.cruiser);
            SetAllParts(deck.battleship);

            void SetAllParts(Ship ship)
            {
                for (int i = 0; i < ship.Length; i++)
                    ship[i] = ship.Armour;
            }
        }
        private void PlaceShips()
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

                while (j != lengthsRepeated[i])
                {

                    otherDimension = UnityEngine.Random.Range(0, 10);
                    startIndex = UnityEngine.Random.Range(0, 11 - lengthsRepeated[i]);

                    placing.Clear();

                    for (j = 0; j < lengthsRepeated[i]; j++)
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
                    placing[j].shipDirection = horizontal ? Direction.Right : Direction.Down;
                }
            }
        }

        #endregion

        #region Utilities
        private void PrintTileInformation()
        {
            StringBuilder line = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (tiles[i, j].ship == null)
                        line.Append("[0]");
                    else if (tiles[i, j].ship == deck.submarine)
                        line.Append("[S]");
                    else if (tiles[i, j].ship == deck.battleship)
                        line.Append("[B]");
                    else if (tiles[i, j].ship == deck.cruiser)
                        line.Append("[C]");
                    else if (tiles[i, j].ship == deck.carrier)
                        line.Append("[A]");
                    else
                        line.Append("[D]");
                }

                Debug.Log(line.ToString());
                line.Clear();
            }
        }
        private void PrintMatrix(bool matrix)
        {
            StringBuilder line = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                    line.Append(String.Format("[{0:f2}]", matrix ? heatMap[i, j] : distortionMatrix[i, j]));

                Debug.Log(line.ToString());
                line.Clear();
            }
        }
        private void PrintProbabilityList()
        {
            int counter = 0;
            Coordinate coords = null;
            Vector2Int coordVector = Vector2Int.zero;

            foreach (var p in probabilityList)
            {
                if ((coords = p.Item2).Equals(new Coordinate(counter / 10, counter % 10, true)))
                {

                    ++counter;
                }
                else
                {
                    coordVector = coords.GetCoordinateVector(true);

                }
            }
        }

        #endregion

        #region IPlayer

        internal IPlayer AsPlayer() => this as IPlayer;
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

        #endregion

        #region Calculations

        private void CalculateDistortionMatrix()
        {
            //Centrality
            double sqrt2Over2 = Math.Sqrt(2.0) / 2.0;
            double minDistance = sqrt2Over2;
            double diffDistance = 8 * sqrt2Over2;
            double minCentrality = 1.0;
            double diffCentrality = currentLevel.MaxCentrality - minCentrality;
            double distance = 0;

            //Random Distortion
            double minDistortion = currentLevel.MinDistortion;
            double maxDistortion = currentLevel.MaxDistortion;

            //Edge and Corner Distortion
            double edgeDistortion = currentLevel.EdgeMultiplier;
            double cornerDistortion = currentLevel.CornerMultiplier;

            distortionMatrix = new double[10, 10];

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    distance = sqrt2Over2 + Math.Sqrt(Math.Pow(x - 4.5, 2) + Math.Pow(y - 4.5, 2));
                    distortionMatrix[x, y] = minCentrality - diffCentrality * ((distance - minDistance) / diffDistance);

                    distortionMatrix[x, y] *= Helper.Random(minDistortion, maxDistortion);

                    if (x == 0 || y == 0 || x == 9 || y == 9)  //Edge
                    {
                        distortionMatrix[x, y] *= edgeDistortion;

                        if ((x == 0 && y == 0) || (x == 0 && y == 9) || (x == 9 && y == 0) || (x == 9 && y == 9))    //Corner
                            distortionMatrix[x, y] *= cornerDistortion;
                    }

                    if (distortionMatrix[x, y] < 0)
                        distortionMatrix[x, y] = 0;
                }
            }
        }
        private void RecalculateHeatMap()
        {
            int count;

            if (lastAttack is not null && GetTileAt(enemyTiles, lastAttack).tileState == TileState.Miss)
            {
                heatMap[lastAttack.X - 1, lastAttack.Y - 1] = 0;
                Coordinate left, right, up, down;

                foreach (var l in lengths)
                {
                    if ((count = GetIterationCount(l)) == 0)
                        continue;

                    Vector2Int index = Vector2Int.zero;

                    left = lastAttack.Left;
                    right = lastAttack.Right;
                    down = lastAttack.Down;
                    up = lastAttack.Up;

                    for (int i = 0; i < l - 1; i++)
                    {
                        if (left is not null)
                        {
                            index = left.GetCoordinateVector(true);
                            DecreaseFromHeatmap();
                            left = left.Left;
                        }
                        if (right is not null)
                        {
                            index = right.GetCoordinateVector(true);
                            DecreaseFromHeatmap();
                            right = right.Right;
                        }
                        if (down is not null)
                        {
                            index = down.GetCoordinateVector(true);
                            DecreaseFromHeatmap();
                            down = down.Down;
                        }
                        if (up is not null)
                        {
                            index = up.GetCoordinateVector(true);
                            DecreaseFromHeatmap();
                            up = up.Up;
                        }

                        void DecreaseFromHeatmap()
                        {
                            if (heatMap[index.x, index.y] > count)
                                heatMap[index.x, index.y] -= count;
                            else
                                heatMap[index.x, index.y] = 0;
                        }
                    }
                }
                return;
            }
            else
            {
                heatMap = new int[10, 10];

                foreach (var l in lengths)
                {
                    if ((count = GetIterationCount(l)) == 0)
                        continue;

                    //Stride-1 pattern
                    for (int x = 0; x < 10; x++)
                    {
                        for (int y = 0; y < 11 - l; y++)
                        {
                            int j;
                            for (j = y; j < y + l; j++)
                                if (tiles[x, j].tileState != TileState.Normal)
                                    break;
                            if (j == y + l)
                                for (j = y; j < y + l; j++)
                                    heatMap[x, j] += count;
                        }
                    }

                    //Transformed to stride-1 pattern with some extra space tradeoffs
                    for (int x = 0; x < 11 - l; x++)
                    {
                        bool[] unavailables = new bool[10];

                        for (int i = x; i < x + l; i++)
                            for (int y = 0; y < 10; y++)
                                if (enemyTiles[i, y].tileState != TileState.Normal)
                                    unavailables[y] = true;
                        for (int i = x; i < x + l; i++)
                            for (int y = 0; y < 10; y++)
                                if (!unavailables[y])
                                    heatMap[i, y] += count;
                    }
                }
            }
        }
        private void RecalculateProbabilityList()
        {
            double oddSum = 0, evenSum = 0;
            int oddCount = 0, evenCount = 0, tempInt;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
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
                }
            }

            if (parity != Parity.Off)
            {
                if (evenSum / evenCount >= oddSum / oddCount)
                    this.parity = Parity.Even;
                else
                    this.parity = Parity.Odd;
            }

            double p = 0, totalP = 0;

            probabilityList = new List<Tuple<double, Coordinate>>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    p = (heatMap[i, j] * distortionMatrix[i, j]);

                    if (p == 0)
                        continue;

                    if ((this.parity == Parity.Odd && (i % 2) != (j % 2)) || (this.parity == Parity.Even && (i % 2) == (j % 2)))
                        p *= currentLevel.OffParityChanceMultiplier;

                    totalP += p;

                    probabilityList.Add(new(totalP, new Coordinate(i, j, true)));
                }
            }
        }

        #endregion

        private TileData GetTileAt(TileData[,] tiles, Coordinate coords) => tiles[coords.X - 1, coords.Y - 1];
        private int GetIterationCount(int l)
        {
            if (l < 2 || l > 5)
                return 0;

            if (!shipNewlyDestroyed && ((l == 2 && ships.IsDestroyerDestroyed()) || (l == 4 && ships.IsBattleshipDestroyed()) || (l == 5 && ships.IsCarrierDestroyed())))
                return 0;

            if (l == 3)
            {
                if (ships.IsSubmarineDestroyed() && ships.IsCruiserDestroyed() && !shipNewlyDestroyed)
                    return 0;
                if (!ships.IsSubmarineDestroyed() && !ships.IsCruiserDestroyed())
                    return 2;
            }

            return 1;
        }
        internal void MakeMove()
        {
            TileData tileToAttack = null;

            if (mode == AIMode.Seek)
            {
                tileToAttack = ChooseTileFromList();
            }
            if (mode == AIMode.TrackDown)
            {
                int direction = 0;
                bool found = true;

                double max = 0;
                double prob = 0;
                List<double> probs = validDirections.Select(d => d.Item3).ToList();

                do
                {
                    prob = Helper.Random(0, max);

                    foreach (var p in probs)
                        if (prob > p)
                            ++direction;

                    found = true;
                    tileToAttack = GetTileAt(enemyTiles, firstTile.GetCoordinatesAt((Direction)direction));

                    found &= (validDirections[direction].Item2 == 0);

                } while (!found);

                var dir = validDirections[direction];

                validDirections[direction] = new(dir.Item1, dir.Item2 + 1, dir.Item3);
            }
            if (mode == AIMode.Mark)
            {
                var coords = (newlySwappedDirection ? firstTile : lastAttack).GetCoordinatesAt(searchDirection.Value);

                if (coords == null || (coords != null && GetTileAt(enemyTiles, coords).tileState != TileState.Normal))
                {
                    var oppositeDirection = Helper.GetOppositeDirection(searchDirection.Value);
                    Tuple<Direction, int, double> dir;

                    if (isPositiveDirection && marker[0] == 0 &&
                        (dir = validDirections.Where(d => d.Item1 == oppositeDirection).FirstOrDefault()) != null && dir.Item2 == 0)
                    {
                        searchDirection = oppositeDirection;
                        newlySwappedDirection = true;
                        isPositiveDirection = false;
                        MakeMove();
                        return;
                    }
                    else
                    {
                        InstantiateTerminateMode();
                        EliminateSurroundingBlocks();
                    }
                }
                else
                {
                    tileToAttack = GetTileAt(enemyTiles, coords);
                }
            }
            if (mode == AIMode.Terminate)
            {
                var coords = terminationQueue.Dequeue();
                tileToAttack = GetTileAt(enemyTiles, coords);

                if (terminationQueue.Count == 0)
                    InstantiateTerminateMode();
            }

            lastAttack = tileToAttack.Coordinates;
            Attack attack = new Attack(lastAttack, 80);
            var result = manager.CheckAttack(attack);

            if (result == AttackResult.Miss)
            {
                GetTileAt(enemyTiles, lastAttack).tileState = TileState.Miss;

                if (mode == AIMode.Mark)
                {
                    if (isPositiveDirection)
                    {
                        searchDirection = Helper.GetOppositeDirection(searchDirection.Value);
                        newlySwappedDirection = true;
                        isPositiveDirection = false;
                    }
                    else
                    {
                        InstantiateTerminateMode();
                        EliminateSurroundingBlocks();
                    }
                }
            }
            else if (result == AttackResult.Hit)
            {
                GetTileAt(enemyTiles, lastAttack).tileState = TileState.HasHitShip;

                if (mode != AIMode.Terminate)
                    hitPath.Add(lastAttack);

                if (mode == AIMode.Seek)    //Instantiate TrackDown mode
                {
                    InstantiateTrackDownMode();
                }
                else if (mode == AIMode.TrackDown)  //Instantiate Mark mode
                {
                    InstantiateMarkMode();
                }
                else if (mode == AIMode.Mark)
                {
                    int tilesMarked = 1;

                    if (isPositiveDirection)
                        tilesMarked = ++marker[2];
                    else
                        tilesMarked = marker[2] + ++marker[0];

                    int maxShipLength = 0;
                    foreach (var len in lengths)
                        if (GetIterationCount(len) > 0)
                            maxShipLength = len;

                    if (tilesMarked == maxShipLength)
                    {
                        InstantiateTerminateMode();
                        EliminateSurroundingBlocks();
                    }
                }
            }
            else
            {
                enemyShips.SetShipDestroyed((ShipType)result);

                int shipLength = Ship.GetLength((ShipType)result);

                hitPath.ForEach(c => GetTileAt(enemyTiles, c).tileState = TileState.HasSunkenShip);

                mode = AIMode.Seek;
            }

            RecalculateHeatMap();
            RecalculateProbabilityList();
        }
        private TileData ChooseTileFromList()
        {
            int low = 0;
            int high = probabilityList.Count - 1;
            int mid = 0;
            double prob;

            System.Random rand = new System.Random();
            double randomVal = rand.NextDouble() * probabilityList[probabilityList.Count - 1].Item1;

            while (high > low)
            {
                mid = (low + high) / 2;
                prob = probabilityList[mid].Item1;

                if (prob < randomVal) low = mid + 1;
                else if (prob > randomVal) high = mid - 1;
                else break;
            }

            return GetTileAt(enemyTiles, probabilityList[mid].Item2);
        }
        private void InstantiateTrackDownMode()
        {
            firstTile = lastAttack;
            searchDirection = null;
            mode = AIMode.TrackDown;
            validDirections = new List<Tuple<Direction, int, double>>();
            var possibleDirections = firstTile.GetNeighborDirections();
            Coordinate coords = null;
            double prob = 0;
            int temp = 0;

            foreach (var dir in possibleDirections)
            {
                if (GetTileAt(enemyTiles, coords = firstTile.GetCoordinatesAt(dir)).tileState == TileState.Normal)
                {
                    prob = (from p in probabilityList
                            select ((temp = probabilityList.IndexOf(p)) == 0 ? p.Item1 :
                           p.Item1 - probabilityList[temp - 1].Item1)).First();

                    validDirections.Add(new(dir, 0, prob));
                }
            }

            validDirections = (from d in validDirections
                               orderby d.Item3 ascending
                               select d).ToList();
        }
        private void InstantiateMarkMode()
        {
            searchDirection = Coordinate.GetDirection(lastAttack, firstTile);
            shipNewlyDestroyed = false;
            newlySwappedDirection = false;
            isPositiveDirection = true;
            marker = new int[3] { 0, 1, 1 };
            mode = AIMode.Mark;
        }
        private void EliminateSurroundingBlocks()
        {
            if (terminationQueue is null || !terminationQueue.Any())
                return;

            Coordinate[] firstTwo = terminationQueue.Take(2).ToArray();
            Direction dir = Coordinate.GetDirection(firstTwo[1], firstTwo[0]).Value!;
            Direction[] orthoDirs = Helper.GetOrthogonalDirections(dir);
            Direction oppositeDir = Helper.GetOppositeDirection(dir);

            Coordinate first = firstTwo[0];
            Coordinate last = terminationQueue.TakeLast(1).ToArray()[0];

            Coordinate end = first.GetCoordinatesAt(oppositeDir);

            SetTileBlocked(end);

            end = last.GetCoordinatesAt(dir);

            SetTileBlocked(end);

            Coordinate second = null;

            foreach (var coords in terminationQueue)
            {
                first = coords.GetCoordinatesAt(orthoDirs[0]);
                second = coords.GetCoordinatesAt(orthoDirs[1]);

                SetTileBlocked(first);
                SetTileBlocked(second);
            }

            void SetTileBlocked(Coordinate coords)
            {
                if (coords is not null)
                    GetTileAt(enemyTiles, coords).tileState = TileState.BlockedByAnotherShip;
            }
        }
        private void InstantiateTerminateMode()
        {
            terminationDirection = UnityEngine.Random.Range(0, 2) == 0 ? true : false;

            int len = marker[0];
            Coordinate tile = firstTile;
            var negativeDirection = (!isPositiveDirection) ? searchDirection.Value : Helper.GetOppositeDirection(searchDirection.Value);

            for (int i = 0; i < len; i++)
                tile = tile.GetCoordinatesAt(negativeDirection);

            var firstDirection = Helper.GetOppositeDirection(negativeDirection);
            len = marker[0] + marker[1] + marker[2];

            var terminationList = new List<Coordinate>();

            if (marker[1] == 0)
                tile = tile.GetCoordinatesAt(firstDirection);

            for (int i = 0; i < len; i++)
            {
                terminationList.Add(tile);
                tile = tile.GetCoordinatesAt(firstDirection);
            }

            if (!terminationDirection)
                terminationList.Reverse();

            terminationQueue = new Queue<Coordinate>();
            mode = AIMode.Terminate;

            foreach (var coord in terminationList)
                terminationQueue.Enqueue(coord);
        }
    }
}