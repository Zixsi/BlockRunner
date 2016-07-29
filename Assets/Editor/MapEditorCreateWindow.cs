using UnityEngine;
using UnityEditor;
using System.Collections;

public class MapEditorCreateWindow : EditorWindow
{
    private MapEditor mapEditor;
    private int width = 10;
    private int height = 3;
    private int size = 3;
    private string nameMap = "Новая карта";

    public void Init(MapEditor editor)
    {
        mapEditor = editor;
        titleContent = new GUIContent("Новая карта");
        minSize = new Vector2(250.0f, 180.0f);
        maxSize = minSize;
    }
    
	void OnGUI ()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        nameMap = EditorGUILayout.TextField("Название карты", nameMap);
        EditorGUILayout.Space();
        /*width = EditorGUILayout.IntField("Ширина", width);
        EditorGUILayout.Space();
        height = EditorGUILayout.IntField("Высота", height);
        EditorGUILayout.Space();*/
        size = EditorGUILayout.IntField("Размер ячеек", size);
        EditorGUILayout.Space();

        if(GUILayout.Button("Ок"))
        {
            mapEditor.size = size;
            mapEditor.width = width;
            mapEditor.height = height;
            mapEditor.nameMap = nameMap;

            mapEditor.Create();
            Close();
        }
    }

}
