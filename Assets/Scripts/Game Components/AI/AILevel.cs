using BattleShips.GameComponents.Ships;
using BattleShips.GameComponents.Tiles;
using BattleShips.Management;
using BattleShips.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleShips.GameComponents.AI
{
    [CreateAssetMenu(fileName = "Leveli",menuName = "AI/Level")]
    internal class AILevel : ScriptableObject
    {
        #region Serialized Fields

        [SerializeField] ShipBundle shipBundle;
        [SerializeField] bool parity = false;

        //These fields aim to distort the correctness of the AI and make it look more natural
        [SerializeField] [Range(0.0f, 1.0f)] double minDistortion = 0.63;
        [SerializeField] [Range(1.0f, 2.0f)] double maxDistortion = 1.37;
        [SerializeField] [Range(1.0f, 2.0f)] double edgeMultiplier = 1.17;
        [SerializeField] [Range(0.0f, 1.0f)] double cornerMultiplier = 0.71;
        [SerializeField] [Range(1.0f, 2.0f)] double maxCentrality = 1.11;
        [SerializeField] [Range(0.0f, 1.0f)] double offParityChanceMultiplier = 0.11;

        #endregion

        #region Properties

        internal ShipBundle ShipBundle { get { return shipBundle; } }
        internal bool Parity { get { return parity; } }
        internal double MinDistortion { get { return minDistortion; } }
        internal double MaxDistortion { get { return maxDistortion; } }
        internal double EdgeMultiplier { get { return edgeMultiplier; } }
        internal double CornerMultiplier { get { return cornerMultiplier; } }
        internal double MaxCentrality { get { return maxCentrality; } }
        internal double OffParityChanceMultiplier { get { return offParityChanceMultiplier; } }

        #endregion
    }
}
