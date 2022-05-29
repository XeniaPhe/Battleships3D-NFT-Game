using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleShips.Firebase.Json
{
    [System.Serializable]
    public class Ship
    {
        public string slot;
        public string baseUID;
        public int level;
        public int armorLevel;
        public int powerLevel;
        public int speedLevel;

        public Ship(string _slot, string _baseUID, int _level, int _armorLevel, int _powerLevel, int _speedLevel)
        {
            slot = _slot;
            baseUID = _baseUID;
            level = _level;
            armorLevel = _armorLevel;
            powerLevel = _powerLevel;
            speedLevel = _speedLevel;
        }
    }

    [System.Serializable]
    public class UserBattleDeck
    {
        public List<Ship> userBattleDeckShips;
    }
}