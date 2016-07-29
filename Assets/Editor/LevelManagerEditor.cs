using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    // Менеджер уровня
    private LevelManager manager;

    // Отображать объекты уровня
    private bool showGo = true;
    // Отображать блоки уровня
    private bool showBlocks = false;
    // Отображать мосты уровня
    private bool showBriges = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        manager = target as LevelManager;

        // Объекты уровня
        EditorGUILayout.Foldout(showGo, "Объекты уровня");
        if(showGo)
        {
            EditorGUI.indentLevel++;

            // Блоки уровня
            showBlocks = EditorGUILayout.Foldout(showBlocks, "Блоки уровня");
            if(showBlocks)
            {
                EditorGUI.indentLevel++;
                int countBlocks = manager.gObjects.blocks.Count;
                countBlocks = EditorGUILayout.IntField("Кол-во", countBlocks);
                EditorGUILayout.Space();

                if(countBlocks < 0)
                    countBlocks = 0;

                while(countBlocks != manager.gObjects.blocks.Count)
                {
                    if(countBlocks > manager.gObjects.blocks.Count)
                        manager.gObjects.blocks.Add(null);
                    else
                        manager.gObjects.blocks.RemoveAt(manager.gObjects.blocks.Count - 1);
                }

                for(int i = 0; i < manager.gObjects.blocks.Count; i++)
                {
                    manager.gObjects.blocks[i] = (GameObject)EditorGUILayout.ObjectField(manager.gObjects.blocks[i], typeof(GameObject), false);
                }

                EditorGUI.indentLevel--;
            }

            // Мосты уровня
            showBriges = EditorGUILayout.Foldout(showBriges, "Мосты уровня");
            if(showBriges)
            {
                EditorGUI.indentLevel++;
                int countBriges = manager.gObjects.briges.Count;
                countBriges = EditorGUILayout.IntField("Кол-во", countBriges);
                EditorGUILayout.Space();

                if(countBriges < 0)
                    countBriges = 0;

                while(countBriges != manager.gObjects.briges.Count)
                {
                    if(countBriges > manager.gObjects.briges.Count)
                        manager.gObjects.briges.Add(null);
                    else
                        manager.gObjects.briges.RemoveAt(manager.gObjects.briges.Count - 1);
                }

                for(int i = 0; i < manager.gObjects.briges.Count; i++)
                {
                    manager.gObjects.briges[i] = (GameObject)EditorGUILayout.ObjectField(manager.gObjects.briges[i], typeof(GameObject), false);
                }

                EditorGUI.indentLevel--;
            }

        }

        manager.maps = (Maps) EditorGUILayout.ObjectField("Карты", manager.maps, typeof(Maps), true);

        EditorGUILayout.LabelField("Последний установленный блок (для расстановки)");
        manager.LastBlock = (GameObject) EditorGUILayout.ObjectField(manager.LastBlock, typeof(GameObject), false);

        EditorGUILayout.LabelField("Тег контейнера сетки объектов дороги");
        manager.TagContainerRoadSpawnPoint = EditorGUILayout.TextField(manager.TagContainerRoadSpawnPoint);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Тег сетки объектов дороги");
        manager.TagRoadSpawnPoint = EditorGUILayout.TextField(manager.TagRoadSpawnPoint);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Тег контейнера сетки объектов блока");
        manager.TagContainerBlockSpawnPoint = EditorGUILayout.TextField(manager.TagContainerBlockSpawnPoint);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Тег сетки объектов блока");
        manager.TagBlockSpawnPoint = EditorGUILayout.TextField(manager.TagBlockSpawnPoint);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Контейнер объектов пула");
        manager.PollContainer = (Transform) EditorGUILayout.ObjectField(manager.PollContainer, typeof(Transform), false);


        if(GUI.changed)
            EditorUtility.SetDirty(manager);
    }

    private void Line()
    {
        EditorGUILayout.Space();
        //EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1f));
        EditorGUILayout.Space();
    }
}
