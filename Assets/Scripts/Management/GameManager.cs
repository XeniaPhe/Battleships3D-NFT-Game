using UnityEngine;
using System.Collections.Generic;
using BattleShips.GameComponents.Levels;
using BattleShips.GameComponents.Levels.StoryMode;
using BattleShips.GameComponents.Tiles;

namespace BattleShips.Management
{
    internal class GameManager : MonoBehaviour
    {
        #region Singleton

        static GameManager instance;
        public static GameManager Instance { get => instance; }

        #endregion

        [SerializeField] List<StoryModeLevel> storyLevels;
        [SerializeField] List<Level> levels;

        ShipSelector shipSelector;

        Level level;

        private static int levelSelected = -1;
        private static GameMode gameMode;


        Tile enteredTile;
        internal Tile EnteredTile
        {
            get => enteredTile;
            set
            {
                enteredTile = value;
                level.EnteredTile = value;
            }
        }

        Tile clickedTile;
        internal Tile ClickedTile
        {
            get => clickedTile;
            set
            {
                clickedTile = value;
                level.ClickedTile = value;
            }
        }

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
                instance = this;
        }

        private void Start()
        {
            shipSelector = ShipSelector.Instance;

            if(levelSelected == -1)
            {
                level = levels[0];
            }
            else
            {
                switch (gameMode)
                {
                    case GameMode.StoryMode:
                        level = storyLevels[levelSelected];
                        break;
                    case GameMode.AgainstComputer:
                        level = levels[levelSelected];
                        break;
                    case GameMode.Multiplayer:
                        //Start a multiplayer game later
                        break;
                }
            }

            level.StartLevel();
        }

        private void Update()
        {
            if (level.Phase == GamePhase.ShipPlacement && Input.GetKeyDown(KeyCode.R))
                shipSelector.Rotate();
        }

        internal void OnShipPlaced()
        {
            level.OnShipPlaced();
        }

        internal static void SelectLevel(GameMode mode, int level = 0)
        {
            gameMode = mode;
            levelSelected = level;
        }
    }
}