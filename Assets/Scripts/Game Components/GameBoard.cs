using UnityEngine;
using BattleShips.Management;
using BattleShips.GameComponents.Tiles;

namespace BattleShips.GameComponents
{
    internal class GameBoard : MonoBehaviour
    {
        #region Singleton

        static GameBoard instance;
        internal static GameBoard Instance { get => instance; }

        #endregion

        #region Cached Fields

        Player player;
        DefenseTile[] defenseTiles;
        AttackTile[] attackTiles;

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
        
        internal Tile GetTile(Vector2Int coord,TileType type,bool zeroBased = false) => GetTile(coord.x,coord.y,type,zeroBased);

        internal Tile GetTile(Coordinate coord,TileType type) => coord is not null ? GetTile(coord.GetCoordinateVector(), type, false) : null;
    }
}