namespace BattleShips.GameComponents.Tiles
{
    internal enum TileState
    {
        Normal = 0,
        Miss,
        HasShip,
        HasHitShip,
        HasDestroyedShipPart,
        HasSunkenShip,
    }
}