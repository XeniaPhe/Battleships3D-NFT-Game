using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace BattleShips.Utils
{
    /// <summary>
    /// Just attach to the parent gameobject of the tiles and remove the script
    /// </summary>
    internal class TileNamer : MonoBehaviour
    {
        private void OnValidate()
        {
            var children = GetComponentsInChildren<Transform>().Where(i => !i.Equals(transform)).ToList();
            children.Sort(new TileComparer());

            for (int i = 0; i < children.Count; i++)
                children[i].SetSiblingIndex(i);
            for (int i = 0; i < children.Count; i++)
                children[i].name = $"Tile {i / 10 + 1}-{i % 10 + 1}";
        }
    }
}