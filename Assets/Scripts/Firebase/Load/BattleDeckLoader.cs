using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using BattleShips.Firebase.Json;
using BattleShips.GameComponents;

namespace BattleShips.Firebase.Load
{
    internal class BattleDeckLoader : MonoBehaviour
    {
        internal TMP_Text text;

        UserBattleDeck userBattleDeck;      //Ships in the battle deck (from server)
        [SerializeField] internal List<Admiral> admiralCards;
        [SerializeField] internal List<BattleShips.GameComponents.Ships.Ship> shipCards;     //All ships

        [DllImport("__Internal")] public static extern void SendLoadBattleDeckRequest(string objectName, string callback, string fallback);

        private void Start()
        {
            //SendLoadBattleDeckRequest(gameObject.name, "OnRequestSuccess", "OnRequestFailed");
            //string jsonString = "{ \"ships\":[{ \"slot\":\"1\",\"baseUID\":\"Bismarck\",\"level\":1,\"armorLevel\":0,\"powerLevel\":0,\"speedLevel\":0}]}";
            //userBattleDeck = JsonUtility.FromJson<UserBattleDeck>(jsonString);
            //Debug.Log(userBattleDeck.userBattleDeckShips[0].slot);
        }

        private void OnRequestSuccess(string jsonBattleDeck)
        {
            int counter = 0;
            Deck deck = Deck.Instance;
            BattleShips.GameComponents.Ships.Ship temp;

            userBattleDeck = JsonUtility.FromJson<UserBattleDeck>(jsonBattleDeck);
            foreach(var ship in userBattleDeck.userBattleDeckShips)
            {
                foreach (var shipCard in shipCards)
                {
                    if (shipCard.baseUID == ship.baseUID)
                    {
                        ++counter;
                        temp = Instantiate<BattleShips.GameComponents.Ships.Ship>(shipCard,null);
                        for (int i = 0; i < temp.Length; i++)
                            temp[i] = temp.Armour;
                        deck.Assign(temp);
                        continue;
                    }
                }
            }

            if (counter != 5) Debug.LogWarning("There aren't 5 cards in the deck!");

        }

        private void OnRequestFailed()
        {

        }
    }
}