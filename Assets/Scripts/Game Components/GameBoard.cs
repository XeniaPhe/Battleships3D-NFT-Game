using System.Linq;
using UnityEngine;
using BattleShips.GameComponents.Ships;
using BattleShips.GameComponents.Tiles;
using BattleShips.GameComponents.Player;
using BattleShips.VFX;

namespace BattleShips.GameComponents
{
    internal class GameBoard : MonoBehaviour
    {
        #region Singleton

        static GameBoard instance;
        internal static GameBoard Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] Peg redPeg;
        [SerializeField] Peg whitePeg;

        #endregion

        #region Cached Fields

        HumanPlayer player;
        DefenseTile[] defenseTiles;
        AttackTile[] attackTiles;
        ExplosionCreator explosionCreator;

        #endregion

        #region Nonserialized Public Fields/Properties

        Tile enteredTile;
        internal Tile EnteredTile
        {
            get => enteredTile;
            set
            {
                enteredTile = value;
                player.EnteredTile = value;
            }
        }

        Tile clickedTile;

        internal Tile ClickedTile
        {
            get => clickedTile;
            set
            {
                clickedTile = value;
                player.ClickedTile = value;
            }
        }

        float tileSize;

        internal float TileSize { get => tileSize; }

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                defenseTiles = GetComponentsInChildren<DefenseTile>();
                attackTiles = GetComponentsInChildren<AttackTile>();
                tileSize = GetComponentInChildren<Tile>().transform.localScale.x;
                explosionCreator = GetComponent<ExplosionCreator>();
            }
        }

        private void Start()
        {
            player = HumanPlayer.Instance;
        }

        internal void ReinstantiateTiles()
        {
            foreach (var tile in defenseTiles)
            {
                Reset(tile.tileData);
            }

            foreach (var tile in attackTiles)
            {
                Reset(tile.tileData);
            }

            void Reset(TileData tileData)
            {
                tileData.startTile = null;
                tileData.ship = null;
                tileData.tileState = TileState.Normal;
            }
        }

        internal Tile GetTile(int x, int y, TileType type, bool zeroBased = false)
        {
            if (!Coordinate.IsValidCoordinate(x, y, zeroBased)) return null;
            int index = zeroBased ? x * 10 + y : x * 10 + y - 11;
            return type == TileType.Defense ? defenseTiles[index] : attackTiles[index];
        }

        internal void PlacePeg(TileType tileType, Coordinate coordinate, bool red)
        {
            var tile = GetTile(coordinate, tileType);

            if (tile.peg is not null)
                return;

            var pos = tile.transform.position;
            pos.y -= 0.5f;
            Peg peg = red ? redPeg : whitePeg;
            peg = Instantiate<Peg>(peg, pos, transform.rotation, transform);
            peg.InitializeRandom(pos);
            tile.peg = peg;
        }

        internal void RevealShip(Coordinate shipStart,Ship ship)
        {
            var startTile = GetTile(shipStart, TileType.Attack);

            Direction dir = 0;
            Tile temp;

            for (int i = 0; i < 4; i++)
                if ((temp = GetTile(startTile.GetTileCoordinatesAt(dir = (Direction)i), TileType.Attack)) is not null && temp.peg is not null && !temp.peg.isWhitePeg)
                    break;

            temp = startTile;

            for (int i = 0; i < ship.Length; i++)
            {
                Destroy(temp.peg.gameObject);
                temp = GetTile(temp.GetTileCoordinatesAt(dir), TileType.Attack);
            }

            Tile middleTile = startTile;

            int middle = (ship.Length % 2 == 0) ? ship.Length / 2 - 1 : ship.Length / 2;

            for (int i = 0; i < middle; i++)
                middleTile = GetTile(middleTile.GetTileCoordinatesAt(dir), TileType.Attack);

            var shipInstance = ship.InstantiateShip(middleTile.transform.position, dir,PlayerType.AI);
            shipInstance.GetComponent<WaveSimulator>().InitializeRandom(shipInstance.transform.position, shipInstance.transform.rotation.eulerAngles);
            shipInstance.GetComponent<ShipExploder>().ExplodeEntirely();
        }

        internal Tile GetTile(Vector2Int coord, TileType type, bool zeroBased = false) => GetTile(coord.x, coord.y, type, zeroBased);

        internal Tile GetTile(Coordinate coord, TileType type) => coord is not null ? GetTile(coord.GetCoordinateVector(), type, false) : null;

        internal void UpdateShipUI(Coordinate coord)
        {
            GetTile(coord, TileType.Defense).tileData.ship.UpdateUI();
        }
        internal void HitShip(Coordinate coord, TileType type)
        {
            ShipType shipType = GetTile(coord, type).tileData.ship.Type;
            int index = GetTile(coord, type).tileData.shipIndex;
            FindObjectsOfType<ShipExploder>().Where(s => s.tag.Equals(shipType.ToString())).FirstOrDefault().ExplodeShip(index);
        }

        internal void CreateExplosion(Coordinate coord, TileType type,ExplosionType explosionType)
        {
            Vector3 pos = GetTile(coord, type).transform.position;
            explosionCreator.Explode(pos,explosionType);
        }
    }
}