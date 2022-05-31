using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using BattleShips.GameComponents;
using BattleShips.GameComponents.AI;
using BattleShips.GameComponents.Tiles;
using BattleShips.GUI;
using BattleShips.Management.UI;

namespace BattleShips.Management
{
    internal class GameManager : MonoBehaviour
    {
        #region Singleton

        static GameManager instance;
        public static GameManager Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] uint shipPlacementTime;
        [SerializeField] uint turnTime;
        [SerializeField] float aiShipPlacementTime;
        [SerializeField] float aiTurnTime;
        [SerializeField] float timeTillWinLoseScreen;
        [SerializeField] float intermediateTextDuration;

        #endregion

        #region Cached Fields

        IPlayer computer;
        IPlayer player;
        GameUIManager uiManager;
        ShipSelector shipSelector;
        GameBoard board;
        Timer timer;
        MoveLogger moveReporter;
        GamePhase phase;
        Turn turn = Turn.Player;

        int shipsPlaced = 0;
        Coordinate hit;
        bool keepTurn = false;

        #endregion

        #region Nonserialized Public Fields/Properties

        Tile enteredTile;
        internal Tile EnteredTile
        {
            get => enteredTile;
            set
            {
                enteredTile = value;
                if (phase == GamePhase.ShipPlacement && enteredTile?.GetType() == typeof(DefenseTile))
                    shipSelector.HighlightShipPlacement((DefenseTile)enteredTile);
                else
                    shipSelector.HighlightShipPlacement(null);
            }
        }

        Tile clickedTile;
        internal Tile ClickedTile
        {
            get => clickedTile;
            set
            {
                clickedTile = value;
                if (phase == GamePhase.ShipPlacement && clickedTile?.GetType() == typeof(DefenseTile))
                {
                    shipSelector.PlaceShip();
                }
                else if(phase == GamePhase.Bombarding && turn == Turn.Player && clickedTile?.GetType() == typeof(AttackTile))
                {
                    var attack = new Attack(clickedTile.tileData.Coordinates, 80);
                    PlayerAttack(attack);
                }
            }
        }

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
                instance = this;
        }

        private void Start()
        {
            computer = AI.Instance;
            player = Player.Instance;
            uiManager = GameUIManager.Instance;
            timer = Timer.Instance;
            moveReporter = MoveLogger.Instance;
            board = GameBoard.Instance;
            uiManager.Initialize();
            shipSelector = ShipSelector.Instance;
            StartShipPlacementPhase();
        }

        private void Update()
        {
            if (phase == GamePhase.ShipPlacement && Input.GetKeyDown(KeyCode.R))
                shipSelector.Rotate();
            //if (GetActionToExecute() != null && GetCanExecute())
            //{
            //    Debug.Log("five");
            //    GetActionToExecute().Invoke();
            //    SetActionToExecute("142",null);
            //}
        }

        private void StartShipPlacementPhase()
        {
            uiManager.DisableReadyButton();
            phase = GamePhase.ShipPlacement;
            moveReporter.PublishReport("Place your ships and press ready",Color.white,shipPlacementTime);
            timer.StartCountdown(shipPlacementTime, player.PlaceShipsRandom);
        }

        public void StartAIShipPlacement()
        {
            turn = Turn.AI;
            timer.CancelCountdown(1);
            uiManager.TurnOffMenu(UIParts.ReadyButton);
            computer.PlaceShipsRandom();
            moveReporter.PublishReport("Enemy is placing his ships",Color.white,aiShipPlacementTime);
            StartCoroutine(WaitFor(() => { SwitchTurn(keepTurn); },aiShipPlacementTime));
        }

        private void PlayerAttack(Attack attack)
        {
            shipSelector.FireFromSelectedShip();
            var attackResult = computer.CheckTile(attack);

            if(attackResult == AttackResult.AllDestroyed)
            {
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                StartCoroutine(WaitFor(() => { DisplayWinLoseScreen(Turn.Player); }, timeTillWinLoseScreen));
                return;
            }

            keepTurn = true;

            if (attackResult == AttackResult.Miss)
            {
                //Place white peg at the tile
                hit = null;
                keepTurn = false;
                moveReporter.PublishReport("Miss!", Color.red, aiTurnTime);
                board.PlacePeg(TileType.Attack,attack.coordinates,false);
            }
            else if (attackResult == AttackResult.Hit)
            {
                //Place red peg at the tile (attack)
                hit = attack.coordinates;
                moveReporter.PublishReport("Hit!", Color.green, aiTurnTime);
                board.PlacePeg(TileType.Attack, attack.coordinates,true);
            }
            else
            {
                //Reveal enemy ship
                hit = null;
                moveReporter.PublishReport($"Enemy Ship Sunk!", Color.green, aiTurnTime); //Details later
                board.PlacePeg(TileType.Attack, attack.coordinates,true);
                board.RevealShip(null);
            }

            SwitchTurn(keepTurn);
        }
        bool enemyHit = false;
        private void EnemyAttack(Attack attack)
        {
            var attackResult = player.CheckTile(attack);

            if (attackResult == AttackResult.AllDestroyed)
            {
                board.GetTile(attack.coordinates, TileType.Defense).tileData.ship.Model.GetComponent<ShipHit>().
                    HitShip(board.GetTile(attack.coordinates, TileType.Defense).tileData.shipIndex + 1);
                board.GetTile(attack.coordinates, TileType.Defense).tileData.ship.UpdateUI();
                board.PlacePeg(TileType.Defense, attack.coordinates, true);
                StartCoroutine(WaitFor(() => { DisplayWinLoseScreen(Turn.AI); }, timeTillWinLoseScreen));
                return;
            }
            UnityAction action = () => { };
            keepTurn = true;

            if (attackResult == AttackResult.Hit)
            {
                hit = attack.coordinates;
                action += () =>
                {
                    moveReporter.PublishReport("Enemy hit a ship!", Color.red, aiTurnTime);
                    board.GetTile(attack.coordinates, TileType.Defense).tileData.ship.Model.GetComponent<ShipHit>().
                    HitShip(board.GetTile(attack.coordinates,TileType.Defense).tileData.shipIndex+1);
                    board.PlacePeg(TileType.Defense, attack.coordinates, true);
                    board.GetTile(attack.coordinates, TileType.Defense).tileData.ship.UpdateUI();
                };
            }
            else if (attackResult == AttackResult.Miss)
            {
                hit = null;
                keepTurn = false;
                action += () =>
                {
                    moveReporter.PublishReport("Enemy missed!", Color.green, aiTurnTime);
                    board.PlacePeg(TileType.Defense, attack.coordinates, false);
                };
            }
            else
            {
                hit = null;
                action += () =>
                {
                    board.GetTile(attack.coordinates, TileType.Defense).tileData.ship.UpdateUI();
                    board.GetTile(attack.coordinates, TileType.Defense).tileData.ship.Model.GetComponent<ShipHit>().
                    HitShip(board.GetTile(attack.coordinates, TileType.Defense).tileData.shipIndex + 1);
                    moveReporter.PublishReport($"Enemy sank your ship", Color.red, aiTurnTime);
                    board.PlacePeg(TileType.Defense, attack.coordinates, true);
                    board.SunkShip();
                };
            }

            action += () => { SwitchTurn(keepTurn); };
            StartCoroutine(WaitFor(action, aiTurnTime));
        }

        internal void SwitchTurn(bool keepTurn = false)
        {
            if ((turn == Turn.AI && !keepTurn) || (turn == Turn.Player && keepTurn))
            {
                if (phase == GamePhase.ShipPlacement)
                {
                    moveReporter.PublishReport("Enemy placed his ships", Color.white, turnTime);
                    phase = GamePhase.Bombarding;
                }

                moveReporter.PublishReport("Player's turn", Color.white, turnTime,intermediateTextDuration);
                timer.StartCountdown(turnTime, () =>
                {
                    var attack = player.PlayRandom();
                    PlayerAttack(attack);
                });
                turn = Turn.Player;
            }
            else
            {
                timer.CancelCountdown(1);
                moveReporter.PublishReport("Enemy's turn", Color.white, aiTurnTime,intermediateTextDuration);
                var attack = computer.PlayRandom(hit);
                turn = Turn.AI;
                EnemyAttack(attack);
            }
        }

        internal void OnShipPlaced()
        {
            ++shipsPlaced;

            if (shipsPlaced == 5)
            {
                uiManager.EnableReadyButton();
                moveReporter.PublishReport("Press ready whenever you are", Color.white, timer.RemainingTime);
            }
        }

        IEnumerator WaitFor(UnityAction action,float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        private void DisplayWinLoseScreen(Turn winner)
        {

        }
    }
}