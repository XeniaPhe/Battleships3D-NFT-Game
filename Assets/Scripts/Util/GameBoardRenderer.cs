using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using BattleShips.Management;
using BattleShips.GameComponents;
using BattleShips.GameComponents.Tiles;

namespace BattleShips.Utils
{
    [ExecuteInEditMode]
    internal class GameBoardRenderer : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] Transform emptyGameObject;

        [Header("Grid Specs")]
        [SerializeField] LineRenderer lineTemplate;
        [SerializeField] DefenseTile defenseTile;
        [SerializeField] AttackTile attackTile;
        [SerializeField] float tileSize;
        [SerializeField] float extraLength;
        [SerializeField] float gapBetweenGrids;

        [Header("Line Specs")]
        [SerializeField] float thickness;
        [SerializeField] Color color;

        #endregion

        #region Cached Fields

        Transform parent;
        Transform prevEmpty = null;
        LineRenderer prevLine = null;
        DefenseTile prevDefTile = null;
        AttackTile prevAttTile = null;
        float prevTileSize;
        float prevTinyExtraLength;
        float prevThickness;
        float prevGap;
        Color prevColor;

        List<LineRenderer> lines;
        List<Tile> tiles;
        Vector3 prevRot = Vector3.zero;
        Vector3 prevPos = Vector3.zero;

        #endregion

        private void Update()
        {
            if (Application.isPlaying) return;
            if (!IsInstantiated()) return;
            if (IsBuildNecessary()) BuildGameBoard();
            UpdateTransform();
        }

        bool IsInstantiated()
        {
             return lineTemplate && defenseTile && attackTile && emptyGameObject &&
                tileSize > 0 && extraLength >= 0 && thickness > 0 && gapBetweenGrids >= 0;
        }

        bool IsBuildNecessary()
        {
            bool Compare(object obj1, object obj2)  //Guess I'm not that good at reflection lol
            {
                if (obj1 is null || obj2 is null) return false;
                if (obj1.GetType() != obj2.GetType()) return false;

                var type = obj1.GetType();
                BindingFlags flag = BindingFlags.Instance | BindingFlags.Public;

                var fields = from member in type.GetFields(flag) select member;

                if (fields.Where(m => !object.Equals(m.GetValue(obj1), m.GetValue(obj2))).ToList().Count > 0) return false;

                //var props = from member in type.GetProperties(flag)
                //            where member.CanRead
                //            select member;

                //if (props.Where(p => !object.Equals(p.GetValue(obj1), p.GetValue(obj2))).ToList().Count > 0) return false;

                return true;
            }

            if (emptyGameObject is null) emptyGameObject = prevEmpty;
            if (lineTemplate is null) lineTemplate = prevLine;
            if (defenseTile is null) defenseTile = prevDefTile;
            if (attackTile is null) attackTile = prevAttTile;
            if (tileSize <= 0) tileSize = prevTileSize;
            if (extraLength < 0) extraLength = prevTinyExtraLength;
            if (thickness <= 0) thickness = prevThickness;
            if (gapBetweenGrids < 0) gapBetweenGrids = prevGap;

            if (lines is null) return true;
            if (tiles is null) return true;
            if (lines.Count != 44) return true;
            if (tiles.Count != 200) return true;
            if (!parent) return true;
            if (FindObjectsOfType<Transform>().Where(o => o.name == "Grid Parent").ToList().Count > 1) return true;
            if (!lineTemplate.Equals(prevLine)) return true;
            if (!Compare(lineTemplate, prevLine)) return true;
            if (!defenseTile.Equals(prevDefTile)) return true;
            if (!Compare(defenseTile, prevDefTile)) return true;
            if (!attackTile.Equals(prevAttTile)) return true;
            if (!Compare(attackTile, prevAttTile)) return true;
            if (tileSize != prevTileSize) return true;
            if (extraLength != prevTinyExtraLength) return true;
            if (thickness != prevThickness) return true;
            if (gapBetweenGrids != prevGap) return true;
            if (color != prevColor) return true;

            return false;
        }

        private void BuildGameBoard()
        {
            if (parent) DestroyImmediate(parent.gameObject);
            FindObjectsOfType<Transform>().Where(o => o.name == "Grid Parent").ToList().ForEach(o => DestroyImmediate(o.gameObject));

            parent = Instantiate<Transform>(emptyGameObject, Vector3.zero, Quaternion.Euler(Vector3.zero), null);
            parent.gameObject.AddComponent<GameBoard>();
            parent.name = "Grid Parent";
            defenseTile.transform.localScale = attackTile.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
            prevRot = Vector3.zero;
            prevPos = Vector3.zero;
            lines = new List<LineRenderer>();
            tiles = new List<Tile>();

            LineRenderer line;

            float centerDif = tileSize / 2;
            string tileName = "Defense Tile #";
            Tile tile = defenseTile;
            float len = 10 * tileSize;

            float zMin = -len - gapBetweenGrids/2;
            float zMax = zMin + len;
            float zPos = zMin;

            float xMin = -len / 2;
            float xMax = xMin + len;
            float xPos = xMin;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    line = Instantiate<LineRenderer>(lineTemplate, Vector3.zero, transform.rotation, parent);
                    line.SetPositions(new Vector3[] { new Vector3(xPos, 0, zMin - extraLength), new Vector3(xPos, 0, zMax + extraLength) });
                    line.startWidth = thickness;
                    line.endWidth = thickness;
                    line.startColor = color;
                    line.endColor = color;
                    line.name = $"Line #{22 * i + 2 * j +1}";
                    lines.Add(line);

                    line = Instantiate<LineRenderer>(lineTemplate, Vector3.zero, transform.rotation, parent);
                    line.SetPositions(new Vector3[] { new Vector3(xMin - extraLength, 0, zPos), new Vector3(xMax + extraLength , 0, zPos) });
                    line.startWidth = thickness;
                    line.endWidth = thickness;
                    line.startColor = color;
                    line.endColor = color;
                    line.name = $"Line #{22 * i + 2 * (j + 1)}";
                    lines.Add(line);

                    zPos += tileSize;
                    xPos += tileSize;
                }

                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        tile = Instantiate<Tile>(tile, new Vector3(j * tileSize + xMin + centerDif, 0, k * tileSize + zMin + centerDif), tile.transform.rotation, parent);
                        tile.name = string.Concat(tileName, $"{j + 1}-{k + 1}");
                        tile.GetComponent<CoordinateWrapper>().Coordinates = new Vector2Int(j + 1, k + 1);
                        tiles.Add(tile);
                    }

                zMin = gapBetweenGrids / 2;
                zMax = zMin + len;
                zPos = zMin;
                xPos = xMin;

                tile = attackTile;
                tileName = "Attack Tile #";
            }

            UpdateTransform();

            prevEmpty = emptyGameObject;
            prevLine = lineTemplate;
            prevDefTile = defenseTile;
            prevAttTile = attackTile;
            prevTileSize = tileSize;
            prevTinyExtraLength = extraLength;
            prevThickness = thickness;
            prevGap = gapBetweenGrids;
            prevColor = color;
        }

        private void UpdateTransform()
        {
            var tempParent = Instantiate<Transform>(emptyGameObject, prevPos, Quaternion.Euler(prevRot), null);
            var child1 = Instantiate<Transform>(emptyGameObject, Vector3.zero, Quaternion.Euler(Vector3.zero), tempParent);
            var child2 = Instantiate<Transform>(emptyGameObject, Vector3.zero, Quaternion.Euler(Vector3.zero), tempParent);

            parent.position = transform.position;
            parent.rotation = transform.rotation;

            foreach (var line in lines)
            {
                child1.position = line.GetPosition(0);
                child2.position = line.GetPosition(1);

                tempParent.position = transform.position;
                tempParent.rotation = transform.rotation;

                line.SetPosition(0, child1.position);
                line.SetPosition(1, child2.position);

                tempParent.position = prevPos;
                tempParent.rotation = Quaternion.Euler(prevRot);
            }

            prevPos = transform.position;
            prevRot = transform.rotation.eulerAngles;
            DestroyImmediate(child1.gameObject);
            DestroyImmediate(child2.gameObject);
            DestroyImmediate(tempParent.gameObject);
        }
    }
}