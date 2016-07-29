using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class MapEditor : MonoBehaviour
{
    // Кол-во колонок
    public int width = 3;
    // Кол-во строк
    public int height = 3;
    // Размер плиток
    public int size = 1;
    // Карта объектов
    public int[,] map;
    // Название карты
    public string nameMap = "Новая карта";
    // Карты объектов
    public Maps maps;

    public string[] nameTypesMap = new string[3] { "Простая", "Средняя", "Сложная" };
    public int[] valTypesMap = new int[3] { 0, 1, 2 };

    // Вывод сетки
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if(map == null)
            Create();
        
        for(int _x = 0; _x < width; _x++)
        {
            for(int _y = 0; _y < height; _y++)
            {
                float _xPos = (_x + 1) * size / 2.0f + (_x * size / 2.0f);
                float _yPos = (_y + 1) * size / 2.0f + (_y * size / 2.0f);

                Gizmos.DrawWireCube(new Vector3(_xPos, _yPos, 0), new Vector3(size, size, 0));

                #if UNITY_EDITOR
                UnityEditor.Handles.Label(new Vector3(_xPos, _yPos, 0), "" + map[_x, _y]);
                #endif
            }
        }
    }

    // Создаем карту
    public void Create()
    {
        map = new int[width, height];
        Clear();
    }

    // Установить значение
    public void SetVal(int x, int y, int val)
    {
        if(x >= 0 && x < width && y >= 0 && y < height)
            map[x, y] = val;
    }

    // Очистить карту
    public void Clear()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                map[x, y] = 0;
            }
        }

        #if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
        #endif
    }

}
