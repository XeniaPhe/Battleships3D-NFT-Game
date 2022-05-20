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
            EditorGUILayout.PrefixLabel("Source Image");
            button.selectedSprite = (Sprite)EditorGUILayout.ObjectField(button.selectedSprite, typeof(Sprite), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }
    }
}