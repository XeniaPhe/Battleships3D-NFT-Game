using System;
using UnityEngine;

namespace BattleShips.GameComponents.Tiles
{
    [Serializable]
    internal class TileData
    {
        [SerializeField] private Coordinate coordinates;
        internal Coordinate Coordinates { get => coordinates; }

        internal TileState tileState;
        internal Ship ship;
        internal int shipIndex;
        internal TileData startTile;
        internal Directions shipDirection;

        internal TileData(Coordinate coordinates)
        {
            this.coordinates=coordinates;
            this.tileState = TileState.Normal;
        }

        internal TileData(int x,int y,bool zeroBased = false)
        {
            this.coordinates = new Coordinate(x,y,zeroBased);
            this.tileState=TileState.Normal;
        }
    }
}