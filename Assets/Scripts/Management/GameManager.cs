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
        [SerializeField] float winLoseScreenDisplayDuration;

        #endregion

        #region Cached Fields

        IPlayer computer;
        IPlayer player;
        GameUIManager uiManager;
        ShipPlacementTool shipPlacementTool;
        Timer timer;
        MoveReporter moveReporter;
        GamePhase phase;
        Turn turn = Turn.Player;
        bool canExecuteAction;
        UnityAction actionToExecute;
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
                    shipPlacementTool.HighlightShipPlacement((DefenseTile)enteredTile);
                else
                    shipPlacementTool.HighlightShipPlacement(null);
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
                    shipPlacementTool.PlaceShip();
                }
                else
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
            moveReporter = MoveReporter.Instance;
            uiManager.Initialize();
            shipPlacementTool = ShipPlacementTool.Instance;
            StartShipPlacementPhase();
        }

        private void Update()
        {
            if (phase == GamePhase.ShipPlacement && Input.GetKeyDown(KeyCode.R))
                shipPlacementTool.Rotate();
            if (actionToExecute != null && canExecuteAction)
            {
                actionToExecute.Invoke();
                actionToExecute = null;
            }
        }

        private void StartShipPlacementPhase()
        {
            uiManager.DisableReadyButton();
            phase = GamePhase.ShipPlacement;
            moveReporter.PublishReport("Please place your ships and press Ready",Color.white,shipPlacementTime);
            timer.StartCountdown(shipPlacementTime, player.PlaceShipsRandom);
        }

        public void StartAIShipPlacement()
        {
            turn = Turn.AI;
            uiManager.TurnOffMenu(UIParts.ReadyButton);
            computer.PlaceShipsRandom();
            moveReporter.PublishReport("Enemy is placing his ships",Color.white,aiShipPlacementTime);
            StartCoroutine(WaitFor(() => { SwitchTurn(keepTurn); },aiShipPlacementTime));
        }

        private void PlayerAttack(Attack attack)
        {
            var attackResult = computer.CheckTile(attack);

            if(attackResult == AttackResult.AllDestroyed)
            {
                StartCoroutine(WaitFor(() => { DisplayWinLoseScreen(Turn.Player); }, timeTillWinLoseScreen));
                return;
            }

            keepTurn = true;

            if (attackResult == AttackResult.Miss)
            {
                hit = null;
                keepTurn = false;
                moveReporter.PublishReport("Miss!", Color.red, aiTurnTime);
            }
            else if (attackResult == AttackResult.Hit)
            {
                hit = attack.coordinates;
                moveReporter.PublishReport("Hit!", Color.green, aiTurnTime);
            }
            else
            {
                hit = null;
                moveReporter.PublishReport($"Enemy Ship Sunk!", Color.green, aiTurnTime); //Details later
            }

            turn = Turn.Player;
            SwitchTurn(keepTurn);
        }

        private void EnemyAttack(Attack attack)
        {
            var attackResult = player.CheckTile(attack);

            if (attackResult == AttackResult.AllDestroyed)
            {
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
                };
            }
            else if (attackResult == AttackResult.Miss)
            {
                hit = null;
                keepTurn = false;
                action += () =>
                {
                    moveReporter.PublishReport("Enemy missed!", Color.green, aiTurnTime);
                };
            }
            else
            {
                hit = null;
                action += () =>
                {
                    moveReporter.PublishReport($"Enemy sank your ship", Color.red, aiTurnTime);
                };
            }

            turn = Turn.AI;
            action += () => { SwitchTurn(keepTurn); };
            StartCoroutine(WaitFor(action, aiTurnTime));
        }

        internal void SwitchTurn(bool keepTurn = false)
        {
            
            if ((turn == Turn.AI && !keepTurn) || (turn == Turn.Player && keepTurn))
            {
                if (phase == GamePhase.ShipPlacement)
                {
                    moveReporter.PublishReport("Enemy placed his ships,player's turn to attack", Color.white, turnTime);
                    phase = GamePhase.Bombarding;
                }
                else
                {
                    moveReporter.PublishReport("Player's turn", Color.white, turnTime);
                }
                timer.StartCountdown(turnTime, () =>
                {
                    var attack = player.PlayRandom();
                    PlayerAttack(attack);
                });
            }
            else
            {
                moveReporter.PublishReport("Enemy's turn", Color.white, aiTurnTime);
                var attack = computer.PlayRandom(hit);
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
            actionToExecute = action;
            canExecuteAction = false;
            yield return new WaitForSeconds(seconds);
            canExecuteAction = true;
        }

        private void DisplayWinLoseScreen(Turn winner)
        {

        }
    }
}