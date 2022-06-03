using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BattleShips.Management;
using BattleShips.GameComponents.Tiles;
using BattleShips.GameComponents.Ships;

namespace BattleShips.GameComponents
{
    internal class Player : MonoBehaviour, IPlayer
    {
        #region Singleton

        static Player instance;
        internal static Player Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] List<Ship> shipsOwned;     //Will be given with welcome pack
        [SerializeField] bool debugMode = true;

        #endregion

        #region Nonserizalized Public Fields/Properties

        internal List<Ship> ShipsOwned { get => shipsOwned; }

        Tile enteredTile;
        internal Tile EnteredTile
        {
            get => enteredTile;
            set
            {
                enteredTile = value;
                manager.EnteredTile = value;
            }
        }

        Tile clickedTile;

        internal Tile ClickedTile
        {
            get => clickedTile;
            set
            {
                clickedTile = value;
                manager.ClickedTile = value;
            }
        }

        #endregion

        #region Cached Fields

        Deck deck;
        GameManager manager;
        GameBoard board;
        ShipFlag shipFlag = new ShipFlag();

        #endregion

        private void Awake()
        {
            if(instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                deck = Deck.Instance;

                if(debugMode)
                {
                    var allTypes = Enum.GetValues(typeof(ShipType)).Cast<ShipType>().ToList();
                    Ship ship;

                    foreach (var type in allTypes)
                    {
                        ship = Instantiate<Ship>(shipsOwned.Find(s => s.Type == type),null);
                        for (int i = 0; i < ship.Length; i++)
                            ship[i] = ship.Armour;
                        deck.Assign(ship);
                    }
                }
            }
        }
        private void Start()
        {
            manager = GameManager.Instance;
            board = GameBoard.Instance;
        }

        internal bool IsDeckFull => deck.IsDeckFull();
        internal void AssignShip(Ship ship) => deck.Assign(ship);
        internal Destroyer GetDestroyer() => deck.Destroyer;
        internal Cruiser GetCruiser() => deck.Cruiser;
        internal Submarine GetSubmarine() => deck.Submarine;
        internal Battleship GetBattleship() => deck.Battleship;
        internal Carrier GetCarrier() => deck.Carrier;
        internal Ship GetShip(ShipType type) => type switch
        {
            ShipType.Destroyer => GetDestroyer(),
            ShipType.Cruiser => GetCruiser(),
            ShipType.Submarine => GetSubmarine(),
            ShipType.Battleship => GetBattleship(),
            ShipType.Carrier => GetCarrier(),
            _ => throw new NotImplementedException()
        };

        void IPlayer.PlaceShipsRandom()
        {

        }

        AttackResult IPlayer.CheckTile(Attack attack)
        {
            var tile = board.GetTile(attack.coordinates, TileType.Defense);

            var tileData = tile.tileData;
            var ship = tileData.ship;

            if (ship is null)
            {
                tileData.tileState = TileState.Miss;
                return AttackResult.Miss;
            }
            else if(ship[tileData.shipIndex] > 0)
            {
                if ((ship[tileData.shipIndex] -= attack.attackPower) <= 0)
                    tileData.tileState = TileState.HasDestroyedShipPart;
                else
                    tileData.tileState = TileState.HasHitShip;

                for (int i = 0; i < ship.Length; i++)
                    if (ship[i] > 0)
                        return AttackResult.Hit;

                shipFlag.SetShipDestroyed(ship.Type);
                Coordinate coords = tileData.startTile.Coordinates;
                var direction = Coordinate.GetDirection(coords, tileData.Coordinates);

                if (!direction.HasValue) Debug.LogError("There's something wrong!");

                for (int i = 0; i < ship.Length; i++)
                {
                    board.GetTile(coords, TileType.Defense).tileData.tileState = TileState.HasSunkenShip;
                    coords = coords.GetCoordinatesAt(direction.Value);
                }

                if (shipFlag.AreAllDestroyed())
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

        Attack IPlayer.PlayRandom(Coordinate hit = null,ShipType? sunkenShip = null) => new Attack(new Coordinate(UnityEngine.Random.Range(1, 11),UnityEngine.Random.Range(1, 11)), 80);
    }
}