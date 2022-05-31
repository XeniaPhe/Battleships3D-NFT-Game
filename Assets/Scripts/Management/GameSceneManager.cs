using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleShips.Management
{
    internal class GameSceneManager : MonoBehaviour
    {
        #region Singleton

        static GameSceneManager instance;
        public static GameSceneManager Instance { get => instance; }

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void LoadDeck()
        {
            SceneManager.LoadScene("Loading Deck");
        }

        public void LoadGameScene()
        {
            var operation = SceneManager.LoadSceneAsync("3D");
        }

        public void LoadMainMenu()
        {
            var operation = SceneManager.LoadSceneAsync("Main Menu");
        }

        public void LoadDockyard()
        {
            var operation = SceneManager.LoadSceneAsync("Dockyard");
        }

        public void LoadNextLevel()
        {
            var operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}