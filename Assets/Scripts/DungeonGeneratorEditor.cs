using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    private DungeonGeneratorType _dungeonGeneratorType = DungeonGeneratorType.Empty;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUILayout.BeginHorizontal("box");
        
        // Add list of DungeonGeneratorType to select from in the inspector
        _dungeonGeneratorType = (DungeonGeneratorType) EditorGUILayout.EnumPopup("Dungeon Generator Type", _dungeonGeneratorType);
        
        if (GUILayout.Button("Generate Dungeon", GUILayout.MaxWidth(150)))
        {
            var dungeonGenerator = (DungeonGenerator) target;
            dungeonGenerator.GenerateDungeon(_dungeonGeneratorType);
        }

        GUILayout.EndHorizontal();
    }
}