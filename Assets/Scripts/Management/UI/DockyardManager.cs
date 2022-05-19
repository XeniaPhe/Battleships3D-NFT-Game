using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using BattleShips.GUI;
using BattleShips.GameComponents;

namespace BattleShips.Management.UI
{
    internal class DockyardManager : MonoBehaviour
    {
        #region Singleton

        static DockyardManager instance;
        internal static DockyardManager Instance { get => instance; }

        #endregion

        #region Serialized Fields

        [SerializeField] RectTransform contentParent;
        [SerializeField] OwnedShipWrapper ownedShipSample;
        [SerializeField] List<DeckShipWrapper> deck;
        [SerializeField] Button saveButton;
        [SerializeField] Button discardButton;
        [SerializeField] Button mainMenuButton;

        #endregion

        #region Cached Fields

        List<OwnedShipWrapper> ownedShipWrappers;
        Player player;

        #endregion

        #region Nonserialized Public Fields/Properties

        bool isReady;
        internal bool IsReady => isReady;

        #endregion

        private void Awake()
        {
            if(instance)
                Destroy(gameObject);
            else
            {
                instance = this;
                saveButton.onClick.AddListener(SaveChanges);
                discardButton.onClick.AddListener(DiscardChanges);
                mainMenuButton.onClick.AddListener(GoToMainMenu);
                saveButton.interactable = false;
                discardButton.interactable = false;
            }
        }

        private void Start()
        {
            ownedShipWrappers = new List<OwnedShipWrapper>();
            player = Player.Instance;

            foreach (var ship in player.ShipsOwned)    
            {
                var wrapper = Instantiate<OwnedShipWrapper>(ownedShipSample, contentParent);
                wrapper.SetConstraint(ship.Type);
                wrapper.Ship = ship;
                ownedShipWrappers.Add(wrapper);
            }

            isReady = true;
        }

        internal void Equip(OwnedShipWrapper wrapper)
        {
            deck.Find(d => d.Constraint == wrapper.Constraint).Ship = wrapper.Ship;
            saveButton.interactable = (discardButton.interactable = HasAChangeBeenMade());
        }

        internal void Equip(DeckShipWrapper wrapper)
        {
            ownedShipWrappers.Find(o => o.Ship == wrapper.Ship)?.Equip();
        }

        internal void Unequip(OwnedShipWrapper wrapper)
        {
            deck.Find(d => d.Ship == wrapper.Ship).Ship = null;
            saveButton.interactable = (discardButton.interactable = HasAChangeBeenMade());
        }

        internal void Unequip(DeckShipWrapper wrapper)
        {
            ownedShipWrappers.Find(o => o.Ship == wrapper.Ship)?.Unequip();
        }

        private bool HasAChangeBeenMade()
        {
            var allTypes = Enum.GetValues(typeof(ShipType)).Cast<ShipType>();

            foreach (var type in allTypes)
                if (deck.Find(d => d.Constraint == type).Ship != player.GetShip(type))
                    return true;

            return false;
        }

        internal void SaveChanges()
        {
            foreach (var wrapper in deck)
                player.AssignShip(wrapper.Ship);

            saveButton.interactable = false;
            discardButton.interactable= false;
        }

        internal void DiscardChanges()
        {
            Ship temp;
            foreach (var wrapper in deck)
            {
                if (wrapper.Ship == (temp = player.GetShip(wrapper.Constraint)))
                    continue;

                if (wrapper.Ship)
                    Unequip(wrapper);

                ownedShipWrappers.Find(o => o.Ship == temp)?.Equip();
            }

            saveButton.interactable = false;
            discardButton.interactable = false;
        }

        internal void GoToMainMenu()
        {
            if (HasAChangeBeenMade())
            {
                UnityAction saveAndGoToMainMenu = () =>
                {
                    SaveChanges();
                    GameSceneManager.Instance.LoadMainMenu();
                };

                UnityAction discardAndGoToMainMenu = () =>
                {
                    DiscardChanges();
                    GameSceneManager.Instance.LoadMainMenu();
                };

                PopupManager.Instance.LoadPopup("Unsaved changes", "Do you want to save the deck?", saveAndGoToMainMenu,
                    "SAVE", discardAndGoToMainMenu, "DISCARD");
            }
            else
                GameSceneManager.Instance.LoadMainMenu();
        }
    }
}