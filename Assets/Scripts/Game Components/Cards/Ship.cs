using UnityEngine;

namespace BattleShips.GameComponents.Card
{
    [CreateAssetMenu(fileName = "Ship", menuName = "Card/Ship", order = 1)]
    public class Ship : ScriptableObject
    {
        public string baseUID;
        public Sprite cardSprite;
    }
}