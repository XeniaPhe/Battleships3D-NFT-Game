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

namespace BattleShips.GameComponents.Levels.StoryMode
{
    [CreateAssetMenu(fileName ="story level",menuName ="Level/Story Level")]
    internal class StoryModeLevel : Level
    {
        [SerializeField] Deck playerDeck;

        [SerializeField] RoundDialogs firstRoundDialogs;
        [SerializeField] RoundDialogs secondRoundDialogs;
        [SerializeField] RoundDialogs thirdRoundDialogs;

        [SerializeField] Dialog b_AI_VictoryScreen;
        [SerializeField] Dialog b_P_VictoryScreen;

        StoryModeEvents playerEvents;
        StoryModeEvents AiEvents;

        private RoundDialogs RoundDialogs
        {
            get => (playerWins + computerWins) switch
            {
                0 => firstRoundDialogs,
                1 => secondRoundDialogs,
                _ => thirdRoundDialogs
            };
        }

        protected override void InstantiateDependencies()
        {
            GetInstances();
            var playerShipFlag = player.Instantiate(playerDeck);
            Ai.Instantiate(this, AiData, playerShipFlag);
            player.Initialize();
            UiManager.Initialize();
        }

        protected override void ShipPlacementPhase()
        {
            playerEvents = new StoryModeEvents();
            AiEvents = new StoryModeEvents();
            base.ShipPlacementPhase();
        }

        protected override void PlayerShipPlacement()
        {
            base.PlayerShipPlacement();
            TriggerDialog(RoundDialogs.playerTriggeredDialogs.b_ShipPlacement);
        }

        protected override void AIShipPlacement(UnityAction afterEnemyShipPlacement)
        {
            base.AIShipPlacement(afterEnemyShipPlacement);
            TriggerDialog(RoundDialogs.AiTriggeredDialogs.b_ShipPlacement);
        }

        protected override void PlayerTurn()
        {
            base.PlayerTurn();

            if(playerEvents.FirstMove)
                TriggerDialog(RoundDialogs.playerTriggeredDialogs.b_FirstMove);
        }

        protected override void AITurn()
        {
            base.AITurn();

            if(AiEvents.FirstMove)
                TriggerDialog(RoundDialogs.AiTriggeredDialogs.b_FirstMove);
        }

        protected override void PlayerAttack(Attack attack)
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

                if(playerEvents.FirstMiss)
                    TriggerDialog(RoundDialogs.playerTriggeredDialogs.a_FirstMiss);
            }
            else if (attackResult == AttackResult.Hit)
            {
                logger.Log("Hit!", Color.green, intermediateTextTime);
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                board.CreateExplosion(attack.coordinates, TileType.Attack, ExplosionType.ShipExplosion);

                if(playerEvents.FirstHit)
                    TriggerDialog(RoundDialogs.playerTriggeredDialogs.a_FirstHit);
            }
            else
            {
                ShipType type;

                logger.Log($"You sank enemy's {(type = Ship.GetShipType(attackResult)).ToString()}!", Color.green, intermediateTextTime);
                board.PlacePeg(TileType.Attack, attack.coordinates, true);
                board.CreateExplosion(attack.coordinates, TileType.Attack, ExplosionType.ShipExplosion);
                board.RevealShip(attack.coordinates, Ai.GetShip(Ship.GetShipType(attackResult)));

                if (playerEvents.DestroyFirstShip)
                    TriggerDialog(RoundDialogs.playerTriggeredDialogs.a_DestroyFirstShip);

                switch (type)
                {
                    case ShipType.Destroyer:
                        TriggerDialog(RoundDialogs.playerTriggeredDialogs.a_DestroyDestroyer);
                        break;
                    case ShipType.Cruiser:
                        TriggerDialog(RoundDialogs.playerTriggeredDialogs.a_DestroyCruiser);
                        break;
                    case ShipType.Submarine:
                        TriggerDialog(RoundDialogs.playerTriggeredDialogs.a_DestroySubmarine);
                        break;
                    case ShipType.Battleship:
                        TriggerDialog(RoundDialogs.playerTriggeredDialogs.a_DestroyBattleship);
                        break;
                    case ShipType.Carrier:
                        TriggerDialog(RoundDialogs.playerTriggeredDialogs.a_DestroyCarrier);
                        break;
                }
            }

            if (playerEvents.FirstMove)
                TriggerDialog(RoundDialogs.playerTriggeredDialogs.a_FirstMove);

            SwitchTurn(keepTurn);
        }

        protected override void EnemyAttack()
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

                if(AiEvents.FirstMiss)
                    actionToInvoke += () => TriggerDialog(RoundDialogs.AiTriggeredDialogs.a_FirstMiss);
            }
            else if (attackResult == AttackResult.Hit)
            {
                hit = attack.coordinates;
                shipSunk = null;

                actionToInvoke += () => logger.Log("Enemy hits!", Color.red, intermediateTextTime);
                actionToInvoke += () => board.CreateExplosion(attack.coordinates, TileType.Defense, ExplosionType.ShipExplosion);
                actionToInvoke += () => board.HitShip(attack.coordinates, TileType.Defense);
                actionToInvoke += () => board.UpdateShipUI(attack.coordinates);

                if (AiEvents.FirstHit)
                    actionToInvoke += () => TriggerDialog(RoundDialogs.AiTriggeredDialogs.a_FirstHit);
            }

            else
            {
                hit = attack.coordinates;
                shipSunk = Ship.GetShipType(attackResult);

                actionToInvoke += () => logger.Log($"Enemy sank your {shipSunk.ToString()}!", Color.red, intermediateTextTime);
                actionToInvoke += () => board.CreateExplosion(attack.coordinates, TileType.Defense, ExplosionType.ShipExplosion);
                actionToInvoke += () => board.HitShip(attack.coordinates, TileType.Defense);
                actionToInvoke += () => board.UpdateShipUI(attack.coordinates);

                if (AiEvents.DestroyFirstShip)
                    actionToInvoke += () => TriggerDialog(RoundDialogs.AiTriggeredDialogs.a_DestroyFirstShip);

                switch (shipSunk)
                {
                    case ShipType.Destroyer:
                        actionToInvoke += () => TriggerDialog(RoundDialogs.AiTriggeredDialogs.a_DestroyDestroyer);
                        break;
                    case ShipType.Cruiser:
                        actionToInvoke += () => TriggerDialog(RoundDialogs.AiTriggeredDialogs.a_DestroyCruiser);
                        break;
                    case ShipType.Submarine:
                        actionToInvoke += () => TriggerDialog(RoundDialogs.AiTriggeredDialogs.a_DestroySubmarine);
                        break;
                    case ShipType.Battleship:
                        actionToInvoke += () => TriggerDialog(RoundDialogs.AiTriggeredDialogs.a_DestroyBattleship);
                        break;
                    case ShipType.Carrier:
                        actionToInvoke += () => TriggerDialog(RoundDialogs.AiTriggeredDialogs.a_DestroyCarrier);
                        break;
                }
            }

            if(AiEvents.FirstMove)
                actionToInvoke += () => TriggerDialog(RoundDialogs.AiTriggeredDialogs.a_FirstMove);

            actionToInvoke += () => SwitchTurn(keepTurn);
            Invoke(AiTurnTime, actionToInvoke);
        }

        protected override void PlayerLose(string text, bool endGame = true)
        {
            logger.Log(text, Color.red, roundEndWaitTime);

            TriggerDialog(RoundDialogs.b_RoundEnd);
            TriggerDialog(RoundDialogs.AiTriggeredDialogs.b_Win);

            if (endGame)
            {
                TriggerDialog(b_AI_VictoryScreen);
                Invoke(roundEndWaitTime, () => DisplayWinLoseScreen(PlayerType.AI));
            }
            else
            {
                Invoke(roundEndWaitTime, StartNewRound);
            }
        }

        protected override void PlayerWin(string text, bool endGame = true)
        {
            logger.Log(text, Color.green, roundEndWaitTime);

            TriggerDialog(RoundDialogs.b_RoundEnd);
            TriggerDialog(RoundDialogs.playerTriggeredDialogs.b_Win);

            if (endGame)
            {
                TriggerDialog(b_P_VictoryScreen);
                Invoke(roundEndWaitTime, () => DisplayWinLoseScreen(PlayerType.Human));
            }
            else
            {
                Invoke(roundEndWaitTime, StartNewRound);
            }
        }

        private void TriggerDialog(Dialog dialog)
        {
            if (dialog is null)
                return;

            PauseGame();
            dialog.Trigger();
        }
    }
}