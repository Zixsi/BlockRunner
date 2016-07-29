using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Maps))]
public class MapsGUI : Editor
{
    // Карты объектов (скрипт)
    private Maps maps;

    // Объекты карты
    private bool goMap = false;
    // Простые карты дорог (тригер)
    private bool rMapEasy = false;
    // Средние карты дорог (тригер)
    private bool rMapMedium = false;
    // Сложные карты дорог (тригер)
    private bool rMapHard = false;

    void OnEnable()
    {
        maps = (Maps)target;
    }

    public override void OnInspectorGUI()
    {
        // Объекты карты
        if(goMap = EditorGUILayout.Foldout(goMap, "Объекты карты"))
        {
            EditorGUI.indentLevel++;
            if(maps.mapGameObjects.Count > 0)
            {
                for(int i = 0; i < maps.mapGameObjects.Count; i++)
                {
                    EditorGUI.BeginChangeCheck();

                    maps.mapGameObjects[i].index = EditorGUILayout.IntField("Индекс", maps.mapGameObjects[i].index);
                    maps.mapGameObjects[i].name = EditorGUILayout.TextField("Название", maps.mapGameObjects[i].name);
                    maps.mapGameObjects[i].go = (GameObject) EditorGUILayout.ObjectField("Объект", maps.mapGameObjects[i].go, typeof(GameObject), true);
                    EditorGUILayout.Space();
                    if(GUILayout.Button("Удалить"))
                    {
                        maps.mapGameObjects.RemoveAt(i);
                    }
                    Line();

                    if(EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(maps);
                        EditorApplication.SaveAssets();
                    }
                }
            }
            EditorGUI.indentLevel--;
        }

        if(GUILayout.Button("Добавить"))
        {
            maps.mapGameObjects.Add(new MapGO());
        }
        if(GUILayout.Button("Сохранить"))
        {
            EditorUtility.SetDirty(maps);
            EditorApplication.SaveAssets();
        }
        EditorGUILayout.Space();

        // Простые карты
        if(rMapEasy = EditorGUILayout.Foldout(rMapEasy, "Простые карты"))
        {
            EditorGUI.indentLevel++;
            if(maps.roadMapEasy.Count > 0)
            {
                for(int i = 0; i < maps.roadMapEasy.Count; i++)
                {
                    EditorGUILayout.LabelField("Название: " + (string) ((!string.IsNullOrEmpty(maps.roadMapEasy[i].title))?maps.roadMapEasy[i].title: "Отсутствует"));
                }
            }
            else
                EditorGUILayout.LabelField("Карты отсутствуют");
            EditorGUI.indentLevel--;
        }

        // Средние карты
        if(rMapMedium = EditorGUILayout.Foldout(rMapMedium, "Средние карты"))
        {
            EditorGUI.indentLevel++;
            if(maps.roadMapEasy.Count > 0)
            {
                for(int i = 0; i < maps.roadMapMedium.Count; i++)
                {
                    EditorGUILayout.LabelField("Название: " + (string)((!string.IsNullOrEmpty(maps.roadMapMedium[i].title)) ? maps.roadMapMedium[i].title : "Отсутствует"));
                }
            }
            else
                EditorGUILayout.LabelField("Карты отсутствуют");
            EditorGUI.indentLevel--;
        }

        // Сложные карты
        if(rMapHard = EditorGUILayout.Foldout(rMapHard, "Сложные карты"))
        {
            EditorGUI.indentLevel++;
            if(maps.roadMapEasy.Count > 0)
            {
                for(int i = 0; i < maps.roadMapHard.Count; i++)
                {
                    EditorGUILayout.LabelField("Название: " + (string)((!string.IsNullOrEmpty(maps.roadMapHard[i].title)) ? maps.roadMapHard[i].title : "Отсутствует"));
                }
            }
            else
                EditorGUILayout.LabelField("Карты отсутствуют");
            EditorGUI.indentLevel--;
        }

        //base.DrawDefaultInspector();
    }


    // Линия (для редактора)
    private void Line()
    {
        EditorGUILayout.Space();
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1f));
        EditorGUILayout.Space();
    }
}