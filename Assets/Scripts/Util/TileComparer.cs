using System.Collections.Generic;
using UnityEngine;

namespace BattleShips.Utils
{
    internal class TileComparer : IComparer<Transform>
    {
        public int Compare(Transform x, Transform y)
        {
            Vector2 xPos = x.position;
            Vector2 yPos = y.position;

            if (xPos.y > yPos.y) return -1;
            else if (xPos.y < yPos.y) return 1;
            else if (xPos.x < yPos.x) return -1;
            else if (xPos.x > yPos.x) return 1;
            else return 0;
        }
    }
}