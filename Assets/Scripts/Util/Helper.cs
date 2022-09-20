using System;
using UnityEngine;
using BattleShips.GameComponents.Tiles;

namespace BattleShips.Utils
{
    internal static class Helper
    {
        static System.Random rnd = new System.Random();

        internal static double Random(double low,double high) => rnd.NextDouble() * (high - low) + low;

        internal static Direction GetOppositeDirection(Direction direction) => (Direction)(((int)direction + 2) % 4);

        internal static Direction[] GetOrthogonalDirections(Direction direction)
        {
            Direction[] directions = new Direction[2];

            switch (direction)
            {
                case Direction.Right:
                case Direction.Left:
                    directions[0] = Direction.Up;
                    directions[1] = Direction.Down;
                    break;
                case Direction.Up:
                case Direction.Down:
                    directions[0] = Direction.Right;
                    directions[1] = Direction.Left;
                    break;
            }

            return directions;
        }

        internal static Vector3 GetDirectionVector(Direction direction) => direction switch
        {
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            Direction.Right => Vector3.right,
            Direction.Left => Vector3.left,
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

        internal static void Try(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception) { }
        }
    }
}