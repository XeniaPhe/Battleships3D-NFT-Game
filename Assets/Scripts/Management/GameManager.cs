using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using BattleShips.GameComponents;
using BattleShips.GameComponents.Player;
using BattleShips.GameComponents.Player.AI;
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
        [SerializeField] float roundEndWaitTime;
        [SerializeField] float intermediateTextDuration;

        #endregion

        #region Cached Fields

        AIPlayer computer;
        HumanPlayer player;

        GameUIManager uiManager;
        ShipSelector shipSelector;
        GameBoard board;
        Timer timer;
        MoveLogger moveLogger;
        GamePhase phase;
        PlayerType turn = PlayerType.Human;
        AttackResult attackResult;
        Attack attack;

        int shipsPlaced = 0;
        Coordinate hit = null;
        ShipType? shipSunk = null;
        bool keepTurn = false;
        int computerWins = 0;
        int playerWins = 0;
        int currentLevel = 0;

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
                else if (phase == GamePhase.Bombarding && turn == PlayerType.Human && clickedTile?.GetType() == typeof(AttackTile))
                {
                    if (shipSelector.selectedShip is null)
                    {
                        moveLogger.PublishReport("You should select a ship!", Color.red, 2);
                        return;
                    }
                    else if (clickedTile.peg && clickedTile.peg.isWhitePeg)
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
            computer = AIPlayer.Instance;
            player = HumanPlayer.Instance;
            uiManager = GameUIManager.Instance;
            timer = Timer.Instance;
            moveLogger = MoveLogger.Instance;
            board = GameBoard.Instance;
            uiManager.Initialize();
            shipSelector = ShipSelector.Instance;
            StartGame(0);
        }

        private void Update()
        {
            if (phase == GamePhase.ShipPlacement && Input.GetKeyDown(KeyCode.R))
                shipSelector.Rotate();
        }

        //Will be called from main menu when player selects AI level
        internal void StartGame(int level)
        {
            currentLevel = level;
            computer.Instantiate(currentLevel);
            StartShipPlacementPhase();
        }

        private void StartShipPlacementPhase()
        {
            uiManager.DisableReadyButton();
            phase = GamePhase.ShipPlacement;
            moveLogger.PublishReport("Place your ships and press ready", Color.white, shipPlacementTime);
            timer.StartCountdown(turnTime, () =>
            {
                moveLogger.PublishReport("Timeout!", Color.red, 3f);
                StartCoroutine(WaitFor(() =>
                {
                    DisplayWinLoseScreen(PlayerType.AI);
                }, 3f));
            });
        }

        internal AttackResult CheckAttack(Attack attack)
        {
            this.attack = attack;

            if (turn == PlayerType.Human)
                attackResult = computer.CheckTile(attack);
            else
                attackResult = player.CheckTile(attack);

            return attackResult;
        }

        public void StartAIShipPlacement()
        {
            turn = PlayerType.AI;
            uiManager.TurnOffMenu(UIParts.ReadyButton);
            uiManager.SetWrapperButtonsInteractable();
            timer.CancelCountdown();
            moveLogger.PublishReport("Enemy is placing his ships", Color.white, aiShipPlacementTime);
            StartCoroutine(WaitFor(() => { SwitchTurn(keepTurn); }, aiShipPlacementTime));
            phase = GamePhase.Bombarding;
        }

        internal void SwitchTurn(bool keepTurn = false)
        {
            if ((turn == PlayerType.AI && !keepTurn) || (turn == PlayerType.Human && keepTurn))
            {
                if (phase == GamePhase.ShipPlacement)
                {
                    moveLogger.PublishReport("Your turn", Color.white, turnTime);
                }

                StartCoroutine(WaitFor(() =>
                {
                    moveLogger.PublishReport("Your turn", Color.white, turnTime - intermediateTextDuration);
                }, intermediateTextDuration));

                timer.StartCountdown(turnTime, () =>
                {
                    moveLogger.PublishReport("Timeout!", Color.red, 3f);
                    StartCoroutine(WaitFor(() =>
                    {
                        DisplayWinLoseScreen(PlayerType.AI);
                    }, 3f));
                });

                turn = PlayerType.Human;
            }
            else
            {
                timer.CancelCountdown();
                StartCoroutine(WaitFor(() =>
                {
                    moveLogger.PublishReport("Enemy's turn", Color.white, aiTurnTime - intermediateTextDuration);
                }, intermediateTextDuration));

                turn = PlayerType.AI;
                computer.MakeMove();
                EnemyAttack();
            }
        }

        private void PlayerAttack(Attack attack)
        {
            shipSelector.FireFromSelectedShip();
            var attackResult = computer.CheckTile(attack);

            if (attackResult == AttackResult.AllDestroyed)
            {
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                board.CreateExplosion(attack.coordinates, TileType.Attack);

                //moveLogger.PublishReport("You won!", Color.green, roundEndWaitTime);
                //StartCoroutine(WaitFor(() => { DisplayWinLoseScreen(PlayerType.Human); }, roundEndWaitTime));
                //return;

                FinishRound(PlayerType.Human);
                return;
            }

            keepTurn = true;

            if (attackResult == AttackResult.Miss)
            {
                keepTurn = false;
                moveLogger.PublishReport("Miss!", Color.red, aiTurnTime);
                board.PlacePeg(TileType.Attack, attack.coordinates, false);
                board.CreateWaterHit(attack.coordinates, TileType.Attack);
            }
            else if (attackResult == AttackResult.Hit)
            {
                moveLogger.PublishReport("Hit!", Color.green, aiTurnTime);
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                board.CreateExplosion(attack.coordinates, TileType.Attack);
            }
            else
            {
                moveLogger.PublishReport($"You sank enemy's {Ship.GetShipType(attackResult).ToString()}!", Color.green, aiTurnTime); //Details later
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                board.CreateExplosion(attack.coordinates, TileType.Attack);
                board.RevealShip(attack.coordinates, computer.GetShip(Ship.GetShipType(attackResult)));
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


                //moveLogger.PublishReport("Enemy won!", Color.red, roundEndWaitTime);
                //StartCoroutine(WaitFor(() => { DisplayWinLoseScreen(PlayerType.AI); }, roundEndWaitTime));
                //return;
                FinishRound(PlayerType.AI);
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
                shipSunk = Ship.GetShipType(attackResult);

                action += () =>
                {
                    board.UpdateShipUI(attack.coordinates);
                    board.HitShip(attack.coordinates, TileType.Defense);
                    moveLogger.PublishReport($"Enemy sank your {shipSunk.ToString()}!", Color.red, aiTurnTime);
                };
            }

            action += () => { SwitchTurn(keepTurn); };
            StartCoroutine(WaitFor(action, aiTurnTime));
        }

        internal void OnShipPlaced()
        {
            ++shipsPlaced;

            if (shipsPlaced == 5)
            {
                uiManager.EnableReadyButton();
                moveLogger.PublishReport("Press ready whenever you are!", Color.white, timer.RemainingTime);
            }
        }

        IEnumerator WaitFor(UnityAction action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        private void FinishRound(PlayerType winner)
        {
            if (winner == PlayerType.Human)
                playerWins++;
            else
                computerWins++;

            moveLogger.PublishReport(winner == PlayerType.Human ? "Round won!" : "Round lost!", winner == PlayerType.Human ? Color.green : Color.red, roundEndWaitTime);

            StartCoroutine(WaitFor(() =>
                {
                    if (computerWins == 2 || playerWins == 2)
                        DisplayWinLoseScreen(winner);
                    else
                        StartNewRound();
                }, roundEndWaitTime));
        }

        private void StartNewRound()
        {
            FindObjectsOfType<GameObject>().Where(g => g.tag.Equals("Disposable")).ToList().ForEach(d => Destroy(d));
            FindObjectsOfType<Peg>().ToList().ForEach(p => Destroy(p.gameObject));

            uiManager.TurnOnMenu(UIParts.ReadyButton);
            computer.Instantiate();
            player.Instantiate();
            StartShipPlacementPhase();
        }

        private void DisplayWinLoseScreen(PlayerType winner)
        {
            if (winner == PlayerType.Human)
                SceneManager.LoadScene("Victory");
            else
                SceneManager.LoadScene("Lose");
        }
    }
}