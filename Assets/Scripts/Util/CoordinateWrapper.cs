using UnityEngine;

namespace BattleShips.Utils
{
    internal class CoordinateWrapper : MonoBehaviour
    {
        [SerializeField] internal int x;
        [SerializeField] internal int y;
        internal Vector2Int Coordinates 
        {
            set
            {
                x = value.x;
                y = value.y;
            }
        }
    }
}