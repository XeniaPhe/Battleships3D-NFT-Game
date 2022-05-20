using UnityEditor;
using UnityEngine;

namespace BattleShips.GUI.EditorScripting
{
    [CustomEditor(typeof(GameShipWrapperButton))]
    internal class WrapperButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GameShipWrapperButton button = (GameShipWrapperButton)target;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Selected Image");
            button.selectedSprite = (Sprite)EditorGUILayout.ObjectField(button.selectedSprite, typeof(Sprite), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Placed Image");
            button.placedSprite = (Sprite)EditorGUILayout.ObjectField(button.placedSprite, typeof(Sprite), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }
    }
}