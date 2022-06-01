using System.Runtime.InteropServices;
using UnityEngine;
using BattleShips.Firebase.Json;
using TMPro;
using System.Collections.Generic;

namespace BattleShips.Firebase.Load
{
    public class LoadBattleDeck : MonoBehaviour
    {
        public TMP_Text text;

        public static UserBattleDeck userBattleDeck;

        public static List<GameComponents.Card.Admiral> admiralCards;
        public static List<GameComponents.Card.Ship> shipCards;

        [DllImport("__Internal")] public static extern void SendLoadBattleDeckRequest(string objectName, string callback, string fallback);

        public void Start()
        {
            SendLoadBattleDeckRequest(gameObject.name, "OnLoadBattleDeckRequestSuccess", "OnRequestFailed");
            //string jsonString = "{ \"ships\":[{ \"slot\":\"1\",\"baseUID\":\"Bismarck\",\"level\":1,\"armorLevel\":0,\"powerLevel\":0,\"speedLevel\":0}]}";
            //userBattleDeck = JsonUtility.FromJson<UserBattleDeck>(jsonString);
            //Debug.Log(userBattleDeck.userBattleDeckShips[0].slot);
        }

        private void OnLoadBattleDeckRequestSuccess(string jsonBattleDeck)
        {
            userBattleDeck = JsonUtility.FromJson<UserBattleDeck>(jsonBattleDeck);
            foreach(Ship ship in userBattleDeck.userBattleDeckShips)
            {
                foreach (GameComponents.Card.Ship shipCard in shipCards)
                {
                    if (shipCard.baseUID == ship.baseUID)
                    {
                        Debug.Log("There is a match!");
                        continue;
                    }
                }
            }
        }

        private void OnRequestFailed()
        {

        }
    }
}