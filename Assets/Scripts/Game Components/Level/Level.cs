using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using BattleShips.GameComponents.Player;
using BattleShips.GameComponents.Player.AI;
using BattleShips.Management;
using BattleShips.Management.UI;
using BattleShips.GUI;
using BattleShips.GameComponents.Tiles;
using BattleShips.GameComponents.Ships;
using UnityEngine.SceneManagement;
using BattleShips.VFX;

namespace BattleShips.GameComponents.Levels
{
    [CreateAssetMenu(fileName ="level",menuName ="Level/Against AI")]
    internal class Level : ScriptableObject
    {
        [SerializeField] protected AIData AiData;
        [SerializeField] protected float roundStartTextTime;
        [SerializeField] protected uint shipPlacementTime;
        [SerializeField] protected uint turnTime;
        [SerializeField] protected float AiShipPlacementTime;
        [SerializeField] protected float AiTurnTime;
        [SerializeField] protected float roundEndWaitTime;
        [SerializeField] protected float intermediateTextTime;

        protected AIPlayer Ai;
        protected HumanPlayer player;
        protected GameManager manager;
        protected GameUIManager UiManager;
        protected MoveLogger logger;
        protected Timer timer;
        protected ShipSelector shipSelector;
        protected GameBoard board;

        protected GamePhase phase;
        protected PlayerType turn;
        protected AttackResult attackResult;
        protected Attack attack;

        protected int shipsPlaced = 0;
        protected Coordinate hit = null;
        protected ShipType? shipSunk = null;
        protected bool keepTurn = false;
        protected int computerWins = 0;
        protected int playerWins = 0;

        internal GamePhase Phase => phase;

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
                        logger.Log("You should select a ship!", Color.red, 2);
                        return;
                    }
                    else if (clickedTile.peg && clickedTile.peg.isWhitePeg)
                    {
                        logger.Log("There's nothing in that tile!", Color.red, 2);
                        return;
                    }
                    var attack = new Attack(clickedTile.tileData.Coordinates, 80);
                    PlayerAttack(attack);
                }
            }
        }

        internal virtual void StartLevel()
        {
            InstantiateDependencies();
            shipsPlaced = 0;
            ShipPlacementPhase();
        }

        protected virtual void InstantiateDependencies()
        {
            GetInstances();

            var playerShipFlag = player.Instantiate(null);
            Ai.Instantiate(this, AiData, playerShipFlag);

            player.Initialize();
            UiManager.Initialize();
        }

        protected void GetInstances()
        {
            Ai = AIPlayer.Instance;
            player = HumanPlayer.Instance;
            manager = GameManager.Instance;
            UiManager = GameUIManager.Instance;
            logger = MoveLogger.Instance;
            timer = Timer.Instance;
            shipSelector = ShipSelector.Instance;
            board = GameBoard.Instance;
        }

        protected virtual void ShipPlacementPhase()
        {
            UiManager.DisableReadyButton();
            phase = GamePhase.ShipPlacement;

            UnityAction shipPlacement = () => { };
            UnityAction readyButton = () => timer.CancelCountdown();
            UnityAction startGame = () => UiManager.ResetWrapperButtons();
            startGame += () => phase = GamePhase.Bombarding;
            startGame += () => SwitchTurn(true);

            if (Random.Range(0, 1) == 0)
            {
                turn = PlayerType.Human;

                readyButton += () => AIShipPlacement(startGame);
                UiManager.SetReadyButtonListener(readyButton);

                logger.Log("You are starting", Color.white, roundStartTextTime);

                shipPlacement += PlayerShipPlacement;
            }
            else
            {
                turn = PlayerType.AI;

                readyButton += startGame;
                UiManager.SetReadyButtonListener(readyButton);

                logger.Log("Enemy is starting", Color.white, roundStartTextTime);

                shipPlacement += () => AIShipPlacement(PlayerShipPlacement);
            }

            Invoke(roundStartTextTime, shipPlacement);
        }

        protected virtual void PlayerShipPlacement()
        {
            logger.Log("Place your ships and press ready", Color.white, shipPlacementTime);
            timer.StartCountdown(shipPlacementTime, () => PlayerLose("Timeout!"));
        }

        protected virtual void AIShipPlacement(UnityAction afterEnemyShipPlacement)
        {
            logger.Log("Enemy is placing his ships", Color.white, AiShipPlacementTime);
            Invoke(AiShipPlacementTime, afterEnemyShipPlacement);
        }

        internal virtual AttackResult CheckAttack(Attack attack)
        {
            this.attack = attack;

            if (turn == PlayerType.Human)
                attackResult = Ai.CheckTile(attack);
            else
                attackResult = player.CheckTile(attack);

            return attackResult;
        }

        protected virtual void SwitchTurn(bool keepTurn = false)
        {
            if ((turn == PlayerType.AI && !keepTurn) || (turn == PlayerType.Human && keepTurn))
                PlayerTurn();
            else
                AITurn();
        }

        protected virtual void PlayerTurn()
        {
            Invoke(intermediateTextTime, () => logger.Log("Your turn", Color.white, turnTime - intermediateTextTime));
            timer.StartCountdown(turnTime, () => PlayerLose("Timeout!"));
            turn = PlayerType.Human;
        }

        protected virtual void AITurn()
        {
            timer.CancelCountdown();
            Invoke(intermediateTextTime, () => logger.Log("Enemy's turn", Color.white, AiTurnTime - intermediateTextTime));
            turn = PlayerType.AI;

            Ai.MakeMove();
            EnemyAttack();
        }

        protected virtual void PlayerAttack(Attack attack)
        {
            shipSelector.FireFromSelectedShip();
            var attackResult = Ai.CheckTile(attack);

            if (attackResult == AttackResult.AllDestroyed)
            {
                logger.Log("You sank all of enemy's ships!", Color.green, intermediateTextTime);
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                board.CreateExplosion(attack.coordinates, TileType.Attack, ExplosionType.ShipExplosion);

                FinishRound(PlayerType.Human);
                return;
            }

            keepTurn = true;

            if (attackResult == AttackResult.Miss)
            {
                keepTurn = false;

                logger.Log("Miss!", Color.red, intermediateTextTime);
                board.PlacePeg(TileType.Attack, attack.coordinates, false);
                board.CreateExplosion(attack.coordinates, TileType.Attack, ExplosionType.WaterExplosion);
            }
            else if (attackResult == AttackResult.Hit)
            {
                logger.Log("Hit!", Color.green, intermediateTextTime);
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                board.CreateExplosion(attack.coordinates, TileType.Attack, ExplosionType.ShipExplosion);
            }
            else
            {
                logger.Log($"You sank enemy's {Ship.GetShipType(attackResult).ToString()}!", Color.green, intermediateTextTime);
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                board.CreateExplosion(attack.coordinates, TileType.Attack, ExplosionType.ShipExplosion);
                board.RevealShip(attack.coordinates, Ai.GetShip(Ship.GetShipType(attackResult)));
            }

            SwitchTurn(keepTurn);
        }

        protected virtual void EnemyAttack()
        {
            UnityAction actionToInvoke = () => { };

            if (attackResult == AttackResult.AllDestroyed)
            {
                actionToInvoke += () => logger.Log("Enemy sank all of your ships!", Color.red, intermediateTextTime);
                actionToInvoke += () => actionToInvoke += () => board.CreateExplosion(attack.coordinates, TileType.Defense, ExplosionType.ShipExplosion);
                actionToInvoke += () => board.HitShip(attack.coordinates, TileType.Defense);
                actionToInvoke += () => board.UpdateShipUI(attack.coordinates);
                actionToInvoke += () => FinishRound(PlayerType.AI);

                Invoke(AiTurnTime, actionToInvoke);
                return;
            }

            keepTurn = true;

            if (attackResult == AttackResult.Miss)
            {
                hit = null;
                shipSunk = null;
                keepTurn = false;

                actionToInvoke += () => logger.Log("Enemy misses!", Color.green, intermediateTextTime);
                actionToInvoke += () => board.CreateExplosion(attack.coordinates, TileType.Defense, ExplosionType.WaterExplosion);
                actionToInvoke += () => board.PlacePeg(TileType.Defense, attack.coordinates, false);
            }
            else if (attackResult == AttackResult.Hit)
            {
                hit = attack.coordinates;
                shipSunk = null;

                actionToInvoke += () => logger.Log("Enemy hits!", Color.red, intermediateTextTime);
                actionToInvoke += () => board.CreateExplosion(attack.coordinates, TileType.Defense, ExplosionType.ShipExplosion);
                actionToInvoke += () => board.HitShip(attack.coordinates, TileType.Defense);
                actionToInvoke += () => board.UpdateShipUI(attack.coordinates);
            }
            else
            {
                hit = attack.coordinates;
                shipSunk = Ship.GetShipType(attackResult);

                actionToInvoke += () => logger.Log($"Enemy sank your {shipSunk.ToString()}!", Color.red, intermediateTextTime);
                actionToInvoke += () => board.CreateExplosion(attack.coordinates, TileType.Defense, ExplosionType.ShipExplosion);
                actionToInvoke += () => board.HitShip(attack.coordinates, TileType.Defense);
                actionToInvoke += () => board.UpdateShipUI(attack.coordinates);
            }

            actionToInvoke += () => SwitchTurn(keepTurn);
            Invoke(AiTurnTime, actionToInvoke);
        }

        protected virtual void FinishRound(PlayerType winner)
        {
            if (winner == PlayerType.Human)
            {
                playerWins++;
                PlayerWin(playerWins == 2 ? "WIN!" : "Round won!", playerWins == 2);
            }
            else
            {
                computerWins++;
                PlayerLose(computerWins == 2 ? "LOSS!" : "Round lost!", computerWins == 2);
            }
        }

        protected virtual void StartNewRound()
        {
            FindObjectsOfType<Transform>().Where(s => s.tag.Equals("Ship Instance")).ToList().ForEach(t => Destroy(t.gameObject));

            Ai.Initialize();
            player.Initialize();
            UiManager.TurnOnMenu(UIParts.ReadyButton);
            UiManager.ResetWrapperButtons();
            shipsPlaced = 0;
            ShipPlacementPhase();
        }

        protected virtual void PlayerLose(string text, bool endGame = true)
        {
            logger.Log(text, Color.red, roundEndWaitTime);
            if (endGame)
                Invoke(roundEndWaitTime, () => DisplayWinLoseScreen(PlayerType.AI));
            else
                Invoke(roundEndWaitTime, StartNewRound);
        }

        protected virtual void PlayerWin(string text, bool endGame = true)
        {
            logger.Log(text, Color.green, roundEndWaitTime);
            if (endGame)
                Invoke(roundEndWaitTime, () => DisplayWinLoseScreen(PlayerType.Human));
            else
                Invoke(roundEndWaitTime, StartNewRound);
        }

        protected void Invoke(float seconds, UnityAction method)
        {
            if (method is null)
                return;

            manager.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                yield return new WaitForSeconds(seconds);
                method.Invoke();
            }
        }

        protected virtual void DisplayWinLoseScreen(PlayerType winner)
        {
            if (winner == PlayerType.Human)
                SceneManager.LoadScene("Victory");
            else
                SceneManager.LoadScene("Lose");
        }

        internal virtual void OnShipPlaced()
        {
            shipsPlaced++;

            if (shipsPlaced == 5)
            {
                UiManager.EnableReadyButton();
                logger.Log("Press ready whenever you are!", Color.white, timer.RemainingTime);
            }
        }

        internal void PauseGame()
        {
            Time.timeScale = 0;
        }

        internal void ContinueGame()
        {
            Time.timeScale = 1f;
        }
    }
}