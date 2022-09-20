using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BattleShips.Management;
using BattleShips.Management.UI;
using BattleShips.GameComponents.Tiles;
using BattleShips.GameComponents.Ships;

namespace BattleShips.GameComponents.Player
{
    internal class HumanPlayer : Player
    {
        #region Singleton

        static HumanPlayer instance;
        internal static HumanPlayer Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] List<Ship> shipsOwned;     //Will be given with welcome pack

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
        ShipFlag shipFlag;

        #endregion

        protected override void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        internal override void Initialize()
        {
            board.ReinstantiateTiles();
            Instantiate(deck);
        }

        internal ShipFlag Instantiate(Deck deck)
        {
            Ship ship;

            if (deck is not null && deck != this.deck)
            {
                this.deck = deck;

                for (int i = 0; i < 5; i++)
                {
                    ship = deck.Bundle[i];

                    if (ship is null)
                        continue;

                    ship = Instantiate<Ship>(ship, null);

                    this.deck.Assign(ship);
                }
            }
            else
            {
                this.deck = new Deck();
                var allTypes = Enum.GetValues(typeof(ShipType)).Cast<ShipType>().ToList();

                foreach (var type in allTypes)
                {
                    ship = Instantiate<Ship>(shipsOwned.Find(s => s.Type == type), null);
                    for (int i = 0; i < ship.Length; i++)
                        ship[i] = ship.Armour;

                    this.deck.Assign(ship);
                }
            }

            shipFlag = new ShipFlag(this.deck.Bundle);
            return shipFlag;
        }

        protected override void Start()
        {
            manager = GameManager.Instance;
            board = GameBoard.Instance;
        }

        internal bool IsDeckFull => deck.IsDeckFull();
        internal void AssignShip(Ship ship) => deck.Assign(ship);
        internal override Destroyer GetDestroyer() => deck.Destroyer;
        internal override Cruiser GetCruiser() => deck.Cruiser;
        internal override Submarine GetSubmarine() => deck.Submarine;
        internal override Battleship GetBattleship() => deck.Battleship;
        internal override Carrier GetCarrier() => deck.Carrier;
        internal override AttackResult CheckTile(Attack attack)
        {
            var tile = board.GetTile(attack.coordinates, TileType.Defense);

            TileData tileData;
            Ship ship;

            try
            {
                tileData = tile.tileData;
                ship = tileData.ship;
            }
            catch (Exception)
            {
                Debug.Log(attack.coordinates);

                throw;
            }


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

                shipFlag.SetShipDestroyed(ship.Type);
                Coordinate coords = tileData.startTile.Coordinates;
                var direction = tileData.shipDirection;

                for (int i = 0; i < ship.Length; i++)
                {
                    board.GetTile(coords, TileType.Defense).tileData.tileState = TileState.HasSunkenShip;
                    coords = coords.GetCoordinatesAt(direction);
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
        internal override Ship GetShip(ShipType shipType) => shipType switch
        {
            ShipType.Destroyer => GetDestroyer(),
            ShipType.Cruiser => GetCruiser(),
            ShipType.Submarine => GetSubmarine(),
            ShipType.Battleship => GetBattleship(),
            ShipType.Carrier => GetCarrier(),
            _ => throw new NotImplementedException()
        };
    }
}