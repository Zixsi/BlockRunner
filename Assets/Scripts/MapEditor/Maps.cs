using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maps : ScriptableObject
{
    public List<MapGO> mapGameObjects = new List<MapGO>();

    // Простые
    public List<Map> roadMapEasy = new List<Map>();
    // Средние
    public List<Map> roadMapMedium = new List<Map>();
    // Сложные
    public List<Map> roadMapHard = new List<Map>();

    // Поиск объекта в списке коллекции объектов
    public MapGO FindMapGO(GameObject obj)
    {
        MapGO _res = null;

        for(int i = 0; i < mapGameObjects.Count; i++)
        {
            if(mapGameObjects[i] != null && mapGameObjects[i].go == obj && mapGameObjects[i].index > 0)
            {
                _res = mapGameObjects[i];
                break;
            }
        }

        return _res;
    }

    public MapGO FindMapGOByIndex(int index = -999)
    {
        MapGO _res = null;

        for(int i = 0; i < mapGameObjects.Count; i++)
        {
            if(mapGameObjects[i] != null && mapGameObjects[i].index == index && mapGameObjects[i].index > 0)
            {
                _res = mapGameObjects[i];
                break;
            }
        }

        return _res;
    }

}
