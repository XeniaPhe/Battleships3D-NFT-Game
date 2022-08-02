using System.Linq;
using UnityEngine;
using BattleShips.GameComponents.Ships;
using BattleShips.GameComponents.Tiles;

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

        Player player;
        DefenseTile[] defenseTiles;
        AttackTile[] attackTiles;
        WaterHit waterHitter;
        ExplosionHit explosionHitter;

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
                waterHitter = GetComponent<WaterHit>();
                explosionHitter = GetComponent<ExplosionHit>();
            }
        }

        private void Start()
        {
            player = Player.Instance;
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

            for (int i = 0; i < ship.Length/2; i++)
                middleTile = GetTile(middleTile.GetTileCoordinatesAt(dir), TileType.Attack);

            var shipInstance = ship.InstantiateShip(middleTile.transform.position, dir);
            shipInstance.GetComponent<WaveSimulator>().InitializeRandom(shipInstance.transform.position, shipInstance.transform.rotation.eulerAngles);
            shipInstance.GetComponent<ShipHit>().HitEntirely();
        }

        internal Tile GetTile(Vector2Int coord, TileType type, bool zeroBased = false) => GetTile(coord.x, coord.y, type, zeroBased);

        internal Tile GetTile(Coordinate coord, TileType type) => coord is not null ? GetTile(coord.GetCoordinateVector(), type, false) : null;

        internal void UpdateShipUI(Coordinate coord)
        {
            GetTile(coord, TileType.Defense).tileData.ship.UpdateUI();
        }
        internal void HitShip(Coordinate coord, TileType type)
        {
            string name = GetTile(coord, type).tileData.ship.name;
            int index = GetTile(coord, type).tileData.shipIndex + 1;
            switch (name)
            {
                case "Carrier(Clone)":
                    FindObjectsOfType<ShipHit>().Where(s => s.name == "Carrier").FirstOrDefault().HitShip(index);
                    break;
                case "Battleship(Clone)":
                    FindObjectsOfType<ShipHit>().Where(s => s.name == "Battleship").FirstOrDefault().HitShip(index);
                    break;
                case "Cruiser(Clone)":
                    FindObjectsOfType<ShipHit>().Where(s => s.name == "Cruiser").FirstOrDefault().HitShip(index);
                    break;
                case "Submarine(Clone)":
                    FindObjectsOfType<ShipHit>().Where(s => s.name == "Submarine").FirstOrDefault().HitShip(index);
                    break;
                case "Destroyer(Clone)":
                    FindObjectsOfType<ShipHit>().Where(s => s.name == "Destroyer").FirstOrDefault().HitShip(index);
                    break;
            }
        }

        internal void CreateExplosion(Coordinate coord, TileType type)
        {
            Vector3 pos = GetTile(coord, type).transform.position;
            explosionHitter.HitExplosion(pos);
        }

        internal void CreateWaterHit(Coordinate coord, TileType type)
        {
            Vector3 pos = GetTile(coord, type).transform.position;
            waterHitter.HitWater(pos);
        }
    }
}