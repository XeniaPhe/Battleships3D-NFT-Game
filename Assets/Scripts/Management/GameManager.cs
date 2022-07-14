using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using BattleShips.GameComponents;
using BattleShips.GameComponents.AI;
using BattleShips.GameComponents.Tiles;
using BattleShips.GameComponents.Ships;
using BattleShips.GUI;
using BattleShips.Management.UI;
using UnityEngine.SceneManagement;

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

        AI computer;
        IPlayer player;
        GameUIManager uiManager;
        ShipSelector shipSelector;
        GameBoard board;
        Timer timer;
        MoveLogger moveLogger;
        GamePhase phase;
        Turn turn = Turn.Player;
        AttackResult attackResult;
        Attack attack;

        int shipsPlaced = 0;
        Coordinate hit = null;
        ShipType? shipSunk = null;
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
                    if (shipSelector.selectedShip is null)
                    {
                        moveLogger.PublishReport("You should select a ship!", Color.red,2);
                        return;
                    }
                    else if(clickedTile.peg && clickedTile.peg.isWhitePeg)
                    {
                        moveLogger.PublishReport("There's nothing in that tile!", Color.red, 2);
                        return;
                    }
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
            moveLogger = MoveLogger.Instance;
            board = GameBoard.Instance;
            uiManager.Initialize();
            shipSelector = ShipSelector.Instance;
            StartShipPlacementPhase();
        }

        private void Update()
        {
            if (phase == GamePhase.ShipPlacement && Input.GetKeyDown(KeyCode.R))
                shipSelector.Rotate();
        }

        private void StartShipPlacementPhase()
        {
            uiManager.DisableReadyButton();
            phase = GamePhase.ShipPlacement;
            moveLogger.PublishReport("Place your ships and press ready",Color.white,shipPlacementTime);
            timer.StartCountdown(turnTime, () =>
            {
                moveLogger.PublishReport("Timeout!", Color.red, 3f);
                StartCoroutine(WaitFor(() =>
                {
                    DisplayWinLoseScreen(Turn.AI);
                }, 3f));
            });
        }

        internal AttackResult CheckAttack(Attack attack)
        {
            this.attack = attack;

            if(turn == Turn.Player)
                attackResult = computer.AsPlayer().CheckTile(attack);
            else
                attackResult = player.CheckTile(attack);

            return attackResult;
        }

        public void StartAIShipPlacement()
        {
            turn = Turn.AI;
            uiManager.TurnOffMenu(UIParts.ReadyButton);uiManager.SetWrapperButtonsInteractable();
            timer.CancelCountdown();
            moveLogger.PublishReport("Enemy is placing his ships",Color.white,aiShipPlacementTime);
            StartCoroutine(WaitFor(() => { SwitchTurn(keepTurn); },aiShipPlacementTime));
            phase = GamePhase.Bombarding;
        }

        internal void SwitchTurn(bool keepTurn = false)
        {
            if ((turn == Turn.AI && !keepTurn) || (turn == Turn.Player && keepTurn))
            {
                if (phase == GamePhase.ShipPlacement)
                {
                    moveLogger.PublishReport("Your turn", Color.white, turnTime);
                }

                StartCoroutine(WaitFor(() =>
                {
                    moveLogger.PublishReport("Your turn", Color.white, turnTime - intermediateTextDuration);
                },intermediateTextDuration));

                timer.StartCountdown(turnTime, () =>
                {
                    moveLogger.PublishReport("Timeout!", Color.red, 3f);
                    StartCoroutine(WaitFor(() =>
                    {
                        DisplayWinLoseScreen(Turn.AI);
                    }, 3f));
                });

                turn = Turn.Player;
            }
            else
            {
                timer.CancelCountdown();
                StartCoroutine(WaitFor(() =>
                {
                    moveLogger.PublishReport("Enemy's turn", Color.white, aiTurnTime - intermediateTextDuration);
                }, intermediateTextDuration));

                turn = Turn.AI;
                computer.Play();
                EnemyAttack();
            }
        }

        private void PlayerAttack(Attack attack)
        {
            shipSelector.FireFromSelectedShip();
            var attackResult = computer.AsPlayer().CheckTile(attack);

            if(attackResult == AttackResult.AllDestroyed)
            {
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                board.CreateExplosion(attack.coordinates, TileType.Attack);
                moveLogger.PublishReport("You won!",Color.green,timeTillWinLoseScreen);
                StartCoroutine(WaitFor(() => { DisplayWinLoseScreen(Turn.Player); }, timeTillWinLoseScreen));
                return;
            }

            keepTurn = true;

            if (attackResult == AttackResult.Miss)
            {
                keepTurn = false;
                moveLogger.PublishReport("Miss!", Color.red, aiTurnTime);
                board.PlacePeg(TileType.Attack,attack.coordinates,false);
                board.CreateWaterHit(attack.coordinates, TileType.Attack);
            }
            else if (attackResult == AttackResult.Hit)
            {
                moveLogger.PublishReport("Hit!", Color.green, aiTurnTime);
                board.PlacePeg(TileType.Attack, attack.coordinates,true);
                board.CreateExplosion(attack.coordinates, TileType.Attack);
            }
            else
            {
                moveLogger.PublishReport($"You sank enemy's {GetShipType(attackResult).ToString()}!", Color.green, aiTurnTime); //Details later
                board.PlacePeg(TileType.Attack, attack.coordinates,true);
                board.CreateExplosion(attack.coordinates, TileType.Attack);
                board.RevealShip(null);
            }

            SwitchTurn(keepTurn);
        }
        bool enemyHit = false;
        private void EnemyAttack()
        {
            if (attackResult == AttackResult.AllDestroyed)
            {
                board.HitShip(attack.coordinates, TileType.Defense);
                board.UpdateShipUI(attack.coordinates);
                //board.PlacePeg(TileType.Defense, attack.coordinates, true);
                moveLogger.PublishReport("Enemy won!", Color.red, timeTillWinLoseScreen);
                StartCoroutine(WaitFor(() => { DisplayWinLoseScreen(Turn.AI); }, timeTillWinLoseScreen));
                return;
            }
            UnityAction action = () => { };
            keepTurn = true;

            if (attackResult == AttackResult.Hit)
            {
                hit = attack.coordinates;
                shipSunk = null;
                action += () =>
                {
                    moveLogger.PublishReport("Enemy hits!", Color.red, aiTurnTime);
                    board.HitShip(attack.coordinates, TileType.Defense);
                    board.UpdateShipUI(attack.coordinates);
                    //board.PlacePeg(TileType.Defense, attack.coordinates, true);
                };
            }
            else if (attackResult == AttackResult.Miss)
            {
                hit = null;
                shipSunk = null;
                keepTurn = false;
                action += () =>
                {
                    board.CreateWaterHit(attack.coordinates, TileType.Defense);
                    moveLogger.PublishReport("Enemy misses!", Color.green, aiTurnTime);
                    board.PlacePeg(TileType.Defense, attack.coordinates, false);
                };
            }
            else
            {
                hit = attack.coordinates;
                shipSunk = GetShipType(attackResult);
                action += () =>
                {
                    board.UpdateShipUI(attack.coordinates);
                    board.HitShip(attack.coordinates, TileType.Defense);
                    //board.PlacePeg(TileType.Defense, attack.coordinates, true);
                    moveLogger.PublishReport($"Enemy sank your {shipSunk.ToString()}!", Color.red, aiTurnTime);
                    board.SunkShip();
                };
            }

            action += () => { SwitchTurn(keepTurn); };
            StartCoroutine(WaitFor(action, aiTurnTime));
        }

        internal void OnShipPlaced()
        {
            ++shipsPlaced;
            //This is being called twice

            if (shipsPlaced == 5)
            {
                uiManager.EnableReadyButton();
                moveLogger.PublishReport("Press ready whenever you are!", Color.white, timer.RemainingTime);
            }
        }

        ShipType GetShipType(AttackResult attackResult) => attackResult switch
        {
            AttackResult.DestroyerDestroyed => ShipType.Destroyer,
            AttackResult.SubmarineDestroyed => ShipType.Submarine,
            AttackResult.CruiserDestroyed => ShipType.Cruiser,
            AttackResult.BattleshipDestroyed => ShipType.Battleship,
            AttackResult.CarrierDestroyed => ShipType.Carrier,
            _ => throw new ArgumentOutOfRangeException("Attack result ?? "),
        };

        IEnumerator WaitFor(UnityAction action,float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        private void DisplayWinLoseScreen(Turn winner)
        {
            if (winner == Turn.Player)
                SceneManager.LoadScene("Victory");
            else
                SceneManager.LoadScene("Lose");
        }
    }
}