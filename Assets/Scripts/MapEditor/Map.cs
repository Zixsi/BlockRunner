using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Map
{
    public string title = "Новая карта";
    public int sizeX = 0;
    public int sizeY = 0;
    public List<MapCell> map = new List<MapCell>();

    public MapCell Cell(int x, int y)
    {
        MapCell _cell = null;

        for(int i = 0; i < map.Count; i++)
        {
            if(map[i] != null && map[i].x == x && map[i].y == y)
                _cell = map[i];
        }

        return _cell;
    }

    public int[,] GetMap()
    {
        int[,] _map = new int[sizeX,sizeY];

        for(int i = 0; i < map.Count; i++)
        {
            if(map[i] != null)
                _map[map[i].x, map[i].y] = map[i].go;
        }

        return _map;
    }

}


[System.Serializable]
public class MapCell
{
    public int x = 0;
    public int y = 0;
    public int go = 0;
}