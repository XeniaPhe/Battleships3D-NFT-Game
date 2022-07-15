using System;
using UnityEngine;
using BattleShips.GameComponents.Tiles;

namespace BattleShips.Utils
{
    internal static class Helper
    {
        static System.Random rnd = new System.Random();

        internal static double Random(double low,double high) => rnd.NextDouble() * (high - low) + low;

        internal static Directions GetOppositeDirection(Directions direction) => (Directions)(((int)direction + 2) % 4);

        internal static Directions[] GetOrthogonalDirections(Directions direction)
        {
            Directions[] directions = new Directions[2];

            switch (direction)
            {
                case Directions.Right:
                case Directions.Left:
                    directions[0] = Directions.Up;
                    directions[1] = Directions.Down;
                    break;
                case Directions.Up:
                case Directions.Down:
                    directions[0] = Directions.Right;
                    directions[1] = Directions.Left;
                    break;
            }

            return directions;
        }

        internal static Vector3 GetDirectionVector(Directions direction) => direction switch
        {
            Directions.Up => Vector3.up,
            Directions.Down => Vector3.down,
            Directions.Right => Vector3.right,
            Directions.Left => Vector3.left,
            _ => throw new NotImplementedException("Undefined Direction!")
        };

        internal static Vector3 ConvertVectorIntoGridVector(Vector3 vector)
        {
            float x = vector.x,y = vector.y, z = vector.z;

            vector.x = z;
            vector.y = x;
            vector.z = y;

            return vector;
        }
    }
}