using System;
using UnityEngine;

namespace BattleShips.GameComponents
{
    [Serializable]
    internal class Admiral : ScriptableObject
    {
        [SerializeField] internal string baseUID;
        [SerializeField] internal Sprite cardSprite;
        [SerializeField] new string name;
    }
}