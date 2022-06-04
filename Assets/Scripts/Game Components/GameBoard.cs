using UnityEngine;
using BattleShips.Management;
using BattleShips.GameComponents.Tiles;
using System.Collections.Generic;

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

        internal Tile GetTile(int x,int y,TileType type,bool zeroBased = false)
        {
            if (!Coordinate.IsValidCoordinate(x, y, zeroBased)) return null;
            int index = zeroBased ? x * 10 + y : x * 10 + y - 11;
            return type == TileType.Defense ? defenseTiles[index] : attackTiles[index];
        }
        
        internal void PlacePeg(TileType tileType,Coordinate coordinate,bool red)
        {
            Peg peg = red ? redPeg : whitePeg;
            var tile = GetTile(coordinate, tileType);
            var pos = tile.transform.position;
            pos.y -= 0.5f;
            peg = Instantiate<Peg>(peg, pos, transform.rotation, transform);
            peg.InitializeRandom(pos);
            tile.peg = peg;
        }

        internal void RevealShip(TileData shipStart)
        {
           
        }

        internal void SunkShip()
        {

        }

        internal Tile GetTile(Vector2Int coord,TileType type,bool zeroBased = false) => GetTile(coord.x,coord.y,type,zeroBased);

        internal Tile GetTile(Coordinate coord,TileType type) => coord is not null ? GetTile(coord.GetCoordinateVector(), type, false) : null;

        internal void UpdateShipUI(Coordinate coord)
        {
            GetTile(coord, TileType.Defense).tileData.ship.UpdateUI();
        }
        internal void HitShip(Coordinate coord,TileType type)
        {
            var tile = GetTile(coord, type).tileData;
            tile.ship.Model.GetComponent<ShipHit>().HitShip(tile.shipIndex + 1);
        }

        internal void CreateExplosion(Coordinate coord,TileType type)
        {
            explosionHitter.HitExplosion((GetTile(coord, type).transform));
        }

        internal void CreateWaterHit(Coordinate coord, TileType type)
        {
            waterHitter.HitWater((GetTile(coord, type).transform));
        }
    }
}