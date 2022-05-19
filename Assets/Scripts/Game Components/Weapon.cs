using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleShips.GameComponents
{
    [Serializable]
    internal class Weapon : ScriptableObject
    {
        [SerializeField] Sprite image;
        [SerializeField] int attackPower;
        [SerializeField] int cooldown;  //In terms of rounds
    }
}