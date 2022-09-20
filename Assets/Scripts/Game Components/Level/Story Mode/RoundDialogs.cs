using System;
using UnityEngine;
using BattleShips.GUI;

namespace BattleShips.GameComponents.Levels.StoryMode
{
    [Serializable]
    internal struct RoundDialogs
    {
        [SerializeField] internal DialogEventContainer playerTriggeredDialogs;
        [SerializeField] internal DialogEventContainer AiTriggeredDialogs;

        [SerializeField] internal Dialog b_RoundEnd;
    }
}