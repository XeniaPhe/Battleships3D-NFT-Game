namespace BattleShips.GameComponents
{
    internal enum AttackResult
    {
        DestroyerDestroyed = 0,
        SubmarineDestroyed,
        CruiserDestroyed,
        BattleshipDestroyed,
        CarrierDestroyed,
        AllDestroyed,
        Hit,
        Miss
    }
}