using System;
using UnityEditor;
using System.Reflection;
using BattleShips.GUI;

namespace BattleShips.GUI
{
    internal class GameShipWrapperButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GameShipWrapperButton butt = (GameShipWrapperButton)target;

            butt.up = EditorGUILayout.ObjectField("Up",butt.up, typeof(GameShipWrapperButton),true) as GameShipWrapperButton;
            butt.down = EditorGUILayout.ObjectField("Down",butt.down, typeof(GameShipWrapperButton), true) as GameShipWrapperButton;
            butt.isSelectedByDefault = EditorGUILayout.Toggle("Is Selected by Default", butt.isSelectedByDefault);
        }
    }
}