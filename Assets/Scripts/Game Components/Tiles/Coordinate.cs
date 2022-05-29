using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleShips.GameComponents.Tiles
{
    [Serializable]
    internal class Coordinate
    {
        [SerializeField] int x;
        [SerializeField] int y;

        #region Properties

        internal int X { get => x; }
        internal int Y { get => y; }

        internal Coordinate Up
        {
            get
            {
                if (!IsValidCoordinate(x, y + 1)) return null;
                return new Coordinate(x, y + 1);
            }
        }

        internal Coordinate Down
        {
            get
            {
                if (!IsValidCoordinate(x, y - 1)) return null;
                return new Coordinate(x, y - 1);
            }
        }

        internal Coordinate Right
        {
            get
            {
                if (!IsValidCoordinate(x + 1, y)) return null;
                return new Coordinate(x + 1, y);
            }
        }

        internal Coordinate Left
        {
            get
            {
                if (!IsValidCoordinate(x - 1, y)) return null;
                return new Coordinate(x - 1, y);
            }
        }

        internal Coordinate Previous
        {
            get
            {
                Coordinate ret;
                if ((ret = Left) is not null) return ret;
                if (IsValidCoordinate(x - 1, 10)) return new Coordinate(x - 1, 10);
                return null;
            }
        }

        internal Coordinate Next
        {
            get
            {
                Coordinate ret;
                if ((ret = Right) is not null) return ret;
                if (IsValidCoordinate(x + 1, 1)) return new Coordinate(X + 1, 1);
                return null;
            }
        }

        #endregion

        private Coordinate() { }
        internal Coordinate(int x, int y, bool zeroBased = false)
        {
            void CheckCoordinate(int c, bool zeroBased = false)
            {
                if ((zeroBased && (c < 0 && c > 9)) || (!zeroBased && (c < 1 || c > 10)))
                    throw new ArgumentOutOfRangeException("c", c, "Coordinates should be between 1 and 10!");
            }

            CheckCoordinate(x, zeroBased);
            CheckCoordinate(y, zeroBased);
            this.x = zeroBased ? x + 1 : x;
            this.y = zeroBased ? y + 1 : y;
        }

        internal List<Coordinate> GetNeighbors()
        {
            List<Coordinate> neighbors = new List<Coordinate>();
            Coordinate temp;
            if ((temp = Left) is not null) neighbors.Add(temp);
            if ((temp = Right) is not null) neighbors.Add(temp);
            if ((temp = Down) is not null) neighbors.Add(temp);
            if ((temp = Up) is not null) neighbors.Add(temp);
            return neighbors;
        }

        internal Coordinate GetCoordinatesAt(Directions direction) => direction switch
        {
            Directions.Up => Up,
            Directions.Down => Down,
            Directions.Left => Left,
            Directions.Right => Right,
            _ => throw new ArgumentException("Undefined Direction!")
        };

        internal static bool IsValidCoordinate(int x, int y, bool zeroBased = false) =>
            zeroBased switch
            {
                true => x > -1 && x < 10 && y > -1 && y < 10,
                false => x > 0 && x < 11 && y > 0 && y < 11 
            };

        internal static bool IsValidCoordinate(Vector2Int coord, bool zeroBased = false) =>
            zeroBased switch
            {
                true => coord.x > -1 && coord.x < 10 && coord.y > -1 && coord.y < 10,
                false => coord.x > 0 && coord.x < 11 && coord.y > 0 && coord.y < 11
            };

        internal Vector2Int GetCoordinateVector(bool zeroBased = false) =>
            zeroBased switch
            {
                true => new Vector2Int(x - 1, y - 1),
                false => new Vector2Int(x, y)
            };

        internal static Directions? GetDirection(Coordinate coordinate1,Coordinate coordinate2)
        {
            if (coordinate1.x == coordinate2.x)
            {
                if (coordinate1.y > coordinate2.y)
                    return Directions.Up;
                else if (coordinate1.y < coordinate2.y)
                    return Directions.Down;
                else
                    return null;
            }
            else if (coordinate1.y == coordinate2.y)
            {
                if (coordinate1.x > coordinate2.x)
                    return Directions.Left;
                else if (coordinate1.x < coordinate2.x)
                    return Directions.Right;
                else
                    return null;
            }
            else
                return null;
        }
    }
}