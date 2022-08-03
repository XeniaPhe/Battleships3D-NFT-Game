using BattleShips.GameComponents.Ships;
using BattleShips.GameComponents.Tiles;
using BattleShips.Management;
using BattleShips.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleShips.GameComponents.Player.AI
{
    internal class InvestigationGroup
    {
        Stack<Coordinate> coords;

        private InvestigationGroup() { }
        internal InvestigationGroup(List<Coordinate> coordinates)
        {
            coords = new Stack<Coordinate>();
            foreach (var coord in coordinates)
                coords.Push(coord);
        }

        internal InvestigationGroup(Coordinate coordinate)
        {
            coords = new Stack<Coordinate>();
            coords.Push(coordinate);
        }

        internal Coordinate GetCoordinates()
        {
            return IsEmpty() ? null : coords.Pop();
        }

        internal bool Contains(Coordinate coordinate)
        {
            return coords.Contains(coordinate);
        }

        internal bool IsEmpty()
        {
            return coords.Count == 0;
        }
    }
}