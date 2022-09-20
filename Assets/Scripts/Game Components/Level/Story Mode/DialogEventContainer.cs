using System;
using UnityEngine;
using BattleShips.GUI;

namespace BattleShips.GameComponents.Levels.StoryMode
{
    [Serializable]
    internal class DialogEventContainer
    {
        [SerializeField] internal Dialog b_ShipPlacement;

        [SerializeField] internal Dialog b_FirstMove;
        [SerializeField] internal Dialog a_FirstMove;

        [SerializeField] internal Dialog a_FirstMiss;
        [SerializeField] internal Dialog a_FirstHit;

        [SerializeField] internal Dialog a_DestroyFirstShip;
        [SerializeField] internal Dialog a_DestroyDestroyer;
        [SerializeField] internal Dialog a_DestroySubmarine;
        [SerializeField] internal Dialog a_DestroyCruiser;
        [SerializeField] internal Dialog a_DestroyBattleship;
        [SerializeField] internal Dialog a_DestroyCarrier;

        [SerializeField] internal Dialog b_Win;
    }
}