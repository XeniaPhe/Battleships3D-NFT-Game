using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleShips.GUI.Canvas
{
    public class Inventory : MonoBehaviour
    {
        [System.Serializable]
        public class Tab
        {
            public static Tab activeTab;

            public enum TabType { Ships, Weapons, Admirals, Equipment, Nfts }

            public TabType tabType;
            public GameObject tabGameObject;
        }

        public List<Tab> tabs;

        public void Start() => Tab.activeTab = tabs[0];

        public void MainMenuReturnButtonClick() => SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);

        public void GoBackToMainMenu() => SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);

        public void ChangeTab(Tab.TabType tabType)
        {
            Tab.activeTab.tabGameObject.SetActive(false);
            foreach(Tab tab in tabs)
            {
                if(tab.tabType == tabType)
                {
                    Tab.activeTab = tab;
                    tab.tabGameObject.SetActive(true);
                    break;
                }
            }
        }

        public void OpenShipsTab() => ChangeTab(Tab.TabType.Ships);

        public void OpenWeaponsTab() => ChangeTab(Tab.TabType.Weapons);

        public void OpenAdmiralsTab() => ChangeTab(Tab.TabType.Admirals);

        public void OpenEquipmentTab() => ChangeTab(Tab.TabType.Equipment);

        public void OpenNftsTab() => ChangeTab(Tab.TabType.Nfts);

    }
}