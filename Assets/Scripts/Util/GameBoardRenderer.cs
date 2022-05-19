using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
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

        [Header("Line Specs")]
        [SerializeField] float thickness;
        [SerializeField] Color color;

        #endregion

        Transform parent;
        Transform prevEmpty = null;
        LineRenderer prevLine = null;
        DefenseTile prevDefTile = null;
        AttackTile prevAttTile = null;
        float prevTileSize = -9999123f;
        float prevTinyExtraLength = -12412871293f;
        float prevThickness = -23487982374;
        Color prevColor = new Color(72, 144, 216);

        List<LineRenderer> lines;
        List<Tile> tiles;
        Vector3 prevRot = Vector3.zero;
        Vector3 prevPos = Vector3.zero;

        private void Update()
        {
            if (Application.isPlaying) return;
            if (!IsInstantiated()) return;
            if (IsBuildNecessary()) BuildGameBoard();
            UpdateTransform();
        }

        bool IsInstantiated() => lineTemplate && defenseTile && attackTile && emptyGameObject && tileSize > 0 && extraLength >= 0 && thickness > 0;

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
            if (thickness < 0) thickness = prevThickness;

            if (lines is null) return true;
            if (tiles is null) return true;
            if (lines.Count != 22) return true;
            if (tiles.Count != 100) return true;
            if (!parent) return true;
            if (!lineTemplate.Equals(prevLine)) return true;
            if (!Compare(lineTemplate, prevLine)) return true;
            if (!defenseTile.Equals(prevDefTile)) return true;
            if (!Compare(defenseTile, prevDefTile)) return true;
            if (!attackTile.Equals(prevAttTile)) return true;
            if (!Compare(attackTile, prevAttTile)) return true;
            if (tileSize != prevTileSize) return true;
            if (extraLength != prevTinyExtraLength) return true;
            if (thickness != prevThickness) return true;
            if (color != prevColor) return true;

            return false;
        }

        private void BuildGameBoard()
        {
            if (parent) DestroyImmediate(parent.gameObject);
            FindObjectsOfType<Transform>().Where(o => o.name == "Grid Parent").ToList().ForEach(o => DestroyImmediate(o.gameObject));

            parent = Instantiate<Transform>(emptyGameObject, Vector3.zero, Quaternion.Euler(Vector3.zero), null);
            parent.name = "Grid Parent";
            defenseTile.transform.localScale = attackTile.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
            prevRot = Vector3.zero;
            prevPos = Vector3.zero;
            lines = new List<LineRenderer>();
            tiles = new List<Tile>();

            LineRenderer line;
            Tile tile;

            float pos = 0;
            float min = -5 * tileSize;
            float max = tileSize * 5;
            for (int i = 0; i < 11; i++)
            {
                pos = min + tileSize * i;

                line = Instantiate<LineRenderer>(lineTemplate, Vector3.zero, transform.rotation, parent);
                line.SetPositions(new Vector3[] { new Vector3(pos, 0, min - extraLength), new Vector3(pos, 0, max + extraLength) });
                line.startWidth = thickness;
                line.endWidth = thickness;
                line.startColor = color;
                line.endColor = color;
                line.name = $"Line #{2 * i}";
                lines.Add(line);

                line = Instantiate<LineRenderer>(lineTemplate, Vector3.zero, transform.rotation, parent);
                line.SetPositions(new Vector3[] { new Vector3(min - extraLength, 0, pos), new Vector3(max + extraLength, 0, pos) });
                line.startWidth = thickness;
                line.endWidth = thickness;
                line.startColor = color;
                line.endColor = color;
                line.name = $"Line #{2 * i + 1}";
                lines.Add(line);
            }

            float centerDif = tileSize / 2;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    tile = Instantiate<DefenseTile>(defenseTile, new Vector3(i * tileSize + min + centerDif, 0, j * tileSize + min + centerDif), defenseTile.transform.rotation, parent);
                    tile.name = $"Tile #{i+1}-{j+1}";
                    tile.GetComponent<CoordinateWrapper>().Coordinates = new Vector2Int(i+1, j+1);
                    tiles.Add(tile);
                }

            UpdateTransform();

            prevEmpty = emptyGameObject;
            prevLine = lineTemplate;
            prevDefTile = defenseTile;
            prevAttTile = attackTile;
            prevTileSize = tileSize;
            prevTinyExtraLength = extraLength;
            prevThickness = thickness;
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