using System;
using UnityEngine;
using BattleShips.GameComponents.Tiles;

namespace BattleShips.Utils
{
    internal static class Helper
    {
        static System.Random rnd = new System.Random();

        internal static double Root2n(double num, uint n)
        {
            if (n % 2 != 0) throw new ArgumentException("This function can't calculate an odd root of a number", "n");
            if (n == 0) return 1;

            n = n / 2;

            for (int i = 0; i < n; i++)
                num = Math.Sqrt(num);

            return num;
        }

        internal static double Random(double low,double high) => rnd.NextDouble() * (high - low) + low;

        internal static double Random100() => Random(0, 100);

        internal static Directions GetOppositeDirection(Directions direction) => (Directions)(((int)direction + 2) % 4);

        internal static Vector3 GetDirectionVector(Directions direction) => direction switch
        {
            Directions.Up => Vector3.up,
            Directions.Down => Vector3.down,
            Directions.Right => Vector3.right,
            Directions.Left => Vector3.left,
            _ => throw new NotImplementedException("Undefined Direction!")
        };
    }
}
