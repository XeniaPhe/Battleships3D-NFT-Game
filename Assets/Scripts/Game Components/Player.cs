using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BattleShips.Management;
using BattleShips.GameComponents.Tiles;

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
        [SerializeField] bool debugMode;

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

                    foreach (var type in allTypes)
                        deck.Assign(shipsOwned.Find(s => s.Type == type));
                }
            }
        }
        private void Start()
        {
            manager = GameManager.Instance;
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

        public void Attack()
        {

        }
    }
}