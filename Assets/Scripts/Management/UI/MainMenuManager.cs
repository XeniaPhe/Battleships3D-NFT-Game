using UnityEngine;
using UnityEngine.UI;
using BattleShips.GameComponents.Player;

namespace BattleShips.Management.UI
{
    internal class MainMenuManager : MonoBehaviour
    {
        #region Singleton

        static MainMenuManager instance;
        internal static MainMenuManager Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] Button playButton;
        [SerializeField] Button dockyardButton;
        [SerializeField] Button quitButton;

        #endregion

        private void Awake()
        {
            if (instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                playButton.onClick.AddListener(Play);
                dockyardButton.onClick.AddListener(GoToDockyard);
                quitButton.onClick.AddListener(Quit);
            }
        }

        private void Play()
        {
            if (HumanPlayer.Instance.IsDeckFull)
                GameSceneManager.Instance.LoadGameScene();
            else
            {
                PopupManager.Instance.LoadPopup("Deck is not Ready", "You should assign a ship to all positions in the deck!",true,
                    GoToDockyard,"OK",false,false);
            }
        }

        private void GoToDockyard() => GameSceneManager.Instance.LoadDockyard();

        private void Quit() => GameSceneManager.Instance.Quit();
    }
}