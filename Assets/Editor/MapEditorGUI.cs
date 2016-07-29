using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(MapEditor))]
public class MapEditorGUI : Editor
{
    // Редактор карт (скрипт)
    private MapEditor mapEditor;

    // Тип карты
    private int typeMap = 0;
    // Набор карт
    private List<Map> listMap = null;
    // Индекс карты
    private int indexMap = -1;

    // Простые карты дорог (тригер)
    private bool rMapEasy = false;
    // Средние карты дорог (тригер)
    private bool rMapMedium = false;
    // Сложные карты дорог (тригер)
    private bool rMapHard = false;

    // Длок предпросмотра
    public GameObject previewBlock;
    // Объекты предпросмотра
    private List<GameObject> previewBlockGo = new List<GameObject>();

    // Объект для расстановки
    private GameObject goSelected = null;
    // Выбранный объект из колекции
    private MapGO selectedMapGO = null;
    // Режим просмотра
    private bool viewMapMode = true;

    void OnEnable()
    {
        mapEditor = (MapEditor)target;

        if(listMap == null && mapEditor.maps != null)
            listMap = mapEditor.maps.roadMapEasy;

        if(previewBlock == null)
            previewBlock = GameObject.FindWithTag("EditorPreviewBlock");
    }

    [MenuItem("Assets/Create/Карта объектов")]
    static void CreateMaps()
    {
        Maps _maps = ScriptableObject.CreateInstance<Maps>();
        string _mapsPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Maps/Maps.asset");

        AssetDatabase.CreateAsset(_maps, _mapsPath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = _maps;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        mapEditor.maps = (Maps)EditorGUILayout.ObjectField("Карты объектов", mapEditor.maps, typeof(Maps), true);
        EditorGUILayout.Space();
        previewBlock = (GameObject)EditorGUILayout.ObjectField("Блок просмотра", previewBlock, typeof(GameObject), true);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if(GUILayout.Button("Новая карта"))
        {
            listMap = null;
            indexMap = -1;

            MapEditorCreateWindow windowCreateMap = (MapEditorCreateWindow)EditorWindow.GetWindow<MapEditorCreateWindow>();
            windowCreateMap.Init(mapEditor);
        }

        Line();

        if(string.IsNullOrEmpty(mapEditor.nameMap))
            mapEditor.nameMap = "Новая карта";

        mapEditor.nameMap = (string)EditorGUILayout.TextField("Название карты", mapEditor.nameMap);
        EditorGUILayout.LabelField("Ширина карты: " + mapEditor.width);
        EditorGUILayout.LabelField("Высота карты: " + mapEditor.height);

        EditorGUI.BeginChangeCheck();
        typeMap = EditorGUILayout.IntPopup("Тип карты", typeMap, mapEditor.nameTypesMap, mapEditor.valTypesMap);
        if(EditorGUI.EndChangeCheck())
        {
            listMap = GetMapType();
            indexMap = -1;
        }
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        goSelected = (GameObject) EditorGUILayout.ObjectField("Объект", goSelected, typeof(GameObject), true);
        if(EditorGUI.EndChangeCheck())
        {
            selectedMapGO = mapEditor.maps.FindMapGO(goSelected);
            if(selectedMapGO == null)
                goSelected = null;

            Selection.activeGameObject = mapEditor.gameObject;
        }

        if(selectedMapGO != null && selectedMapGO.index > 0)
        {
            EditorGUILayout.LabelField("Объект: " + selectedMapGO.name);
            EditorGUILayout.LabelField("Индекс: " + selectedMapGO.index);
        }

        EditorGUILayout.Space();


        if(GUILayout.Button((viewMapMode)?"Предпросмотр":"Вернутся к редактированию"))
        {
            viewMapMode = !viewMapMode;
            SceneView scView = SceneView.lastActiveSceneView;

            if(!viewMapMode)
            {
                if(previewBlock != null)
                {
                    PreviewMap();
                    scView.pivot = previewBlock.transform.position;
                    scView.in2DMode = false;
                }
            }
            else
            {
                scView.pivot = mapEditor.transform.position;
                scView.in2DMode = true;
            }
            SceneView.RepaintAll();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if(GUILayout.Button("Сохранить карту"))
        {
            if(listMap != null)
                SaveMap(listMap, indexMap);
            else
                Debug.LogError("Тип карты не указан!");
        }

        Line();

        // Простые карты
        if(rMapEasy = EditorGUILayout.Foldout(rMapEasy, "Простые карты"))
        {
            EditorGUI.indentLevel++;
            if(mapEditor.maps.roadMapEasy.Count > 0)
            {
                for(int i = 0; i < mapEditor.maps.roadMapEasy.Count; i++)
                {
                    List<Map> maps = mapEditor.maps.roadMapEasy;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(maps[i].title);
                    if(GUILayout.Button("Редактировать"))
                    {
                        LoadMap(maps, i);
                    }
                    if(GUILayout.Button("Удалить"))
                    {
                        DeleteMap(maps, i);
                    }
                    EditorGUILayout.EndHorizontal();
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
            if(mapEditor.maps.roadMapMedium.Count > 0)
            {
                for(int i = 0; i < mapEditor.maps.roadMapMedium.Count; i++)
                {
                    List<Map> maps = mapEditor.maps.roadMapMedium;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(maps[i].title);
                    if(GUILayout.Button("Редактировать"))
                    {
                        LoadMap(maps, i);
                    }
                    if(GUILayout.Button("Удалить"))
                    {
                        DeleteMap(maps, i);
                    }
                    EditorGUILayout.EndHorizontal();
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
            if(mapEditor.maps.roadMapHard.Count > 0)
            {
                for(int i = 0; i < mapEditor.maps.roadMapHard.Count; i++)
                {
                    List<Map> maps = mapEditor.maps.roadMapHard;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(maps[i].title);
                    if(GUILayout.Button("Редактировать"))
                    {
                        LoadMap(maps, i);
                    }
                    if(GUILayout.Button("Удалить"))
                    {
                        DeleteMap(maps, i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
                EditorGUILayout.LabelField("Карты отсутствуют");
            EditorGUI.indentLevel--;
        }


        //base.DrawDefaultInspector();
    }

    void OnSceneGUI()
    {
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        Event _event = Event.current;
        Ray _ray = Camera.current.ScreenPointToRay(new Vector3(_event.mousePosition.x, -_event.mousePosition.y + Camera.current.pixelHeight));
        Vector3 _mousePos = _ray.origin;

        // Обрабатываем события мыши
        if(_event.isMouse && _event.type == EventType.MouseDown)
        {
            Vector3 _posMap = MouseGridPos(_mousePos);

            // Устанавливаем объект
            if(_event.button == 0)
            {
                GUIUtility.hotControl = controlId;
                _event.Use();
                if(selectedMapGO != null && selectedMapGO.index > 0)
                {
                    mapEditor.SetVal((int)_posMap.x, (int)_posMap.y, selectedMapGO.index);
                }
                SceneView.RepaintAll();
            }
            // Удаляем объект
            else if(_event.button == 1)
            {
                //GUIUtility.hotControl = controlId;
                //_event.Use();
                mapEditor.SetVal((int)_posMap.x, (int)_posMap.y, 0);
                SceneView.RepaintAll();
            }
            
        }

    }

    void OnDisable()
    {
        for(int i = 0; i < previewBlockGo.Count; i++)
        {
            if(previewBlockGo[i] != null)
                DestroyImmediate(previewBlockGo[i]);
        }
        previewBlockGo.Clear();
    }

    // Позиция мыши на сетке
    private Vector3 MouseGridPos(Vector3 mousePos)
    {
        float _xPos = Mathf.Floor(mousePos.x / mapEditor.size) + (mapEditor.size / 2.0f);
        float _yPos = Mathf.Floor(mousePos.y / mapEditor.size) + (mapEditor.size / 2.0f);

        return new Vector3(_xPos - (mapEditor.size / 2.0f), _yPos - (mapEditor.size / 2.0f), 0f);
    }


    // Получить список карт (определенный тип)
    private List<Map> GetMapType(int type = -1)
    {
        List<Map> _type = null;
        int _typeMap = typeMap;
        if(type >= 0)
            _typeMap = type;

        switch(_typeMap)
        {
            case 0:
                _type = mapEditor.maps.roadMapEasy;
            break;
            case 1:
                _type = mapEditor.maps.roadMapMedium;
            break;
            case 2:
                _type = mapEditor.maps.roadMapHard;
            break;
        }

        return _type;
    }

    // Получить список карт (индекс)
    private int GetMapTypeIndex(List<Map> type)
    {
        int _index = -1;

        if(type == mapEditor.maps.roadMapEasy)
            _index = 0;
        else if(type == mapEditor.maps.roadMapMedium)
            _index = 1;
        else if(type == mapEditor.maps.roadMapHard)
            _index = 2;

        return _index;
    }

    // Удалить карту
    private void DeleteMap(List<Map> maps, int index)
    {
        if(maps != null && index >= 0 && index <= maps.Count)
            maps.RemoveAt(index);
        EditorUtility.SetDirty(mapEditor.maps);
        EditorApplication.SaveAssets();
    }

    // Загрузить карту
    private void LoadMap(List<Map> maps, int index)
    {
        if(maps != null && index >= 0 && index <= maps.Count && maps[index].map != null)
        {
            mapEditor.width = maps[index].sizeX;
            mapEditor.height = maps[index].sizeY;
            mapEditor.nameMap = maps[index].title;
            mapEditor.Create();

            for(int i = 0; i < maps[index].map.Count; i++)
            {
                MapCell _mCell = maps[index].map[i];
                mapEditor.map[_mCell.x, _mCell.y] = _mCell.go;
            }

            listMap = maps;
            indexMap = index;
            typeMap = GetMapTypeIndex(maps);

            SceneView.RepaintAll();
        }
    }

    // Сохранить карту
    private void SaveMap(List<Map> maps, int index = -1)
    {
        if(maps != null)
        {
            Map _map = new Map();
            _map.title = mapEditor.nameMap;
            _map.sizeX = mapEditor.width;
            _map.sizeY = mapEditor.height;
            _map.map.Clear();

            if(mapEditor.map != null)
            {
                for(int x = 0; x < mapEditor.map.GetLength(0); x++)
                {
                    for(int y = 0; y < mapEditor.map.GetLength(1); y++)
                    {
                        if(mapEditor.map[x, y] > 0)
                        {
                            MapCell _mCell = new MapCell();
                            _mCell.x = x;
                            _mCell.y = y;
                            _mCell.go = mapEditor.map[x, y];

                            _map.map.Add(_mCell);
                        }
                            
                    }
                }
            }

            // Если есть - обновляем
                if(index >= 0 && index <= maps.Count)
                maps[index] = _map;
            else  // Иначе добавляем
            {
                maps.Add(_map);
                indexMap = (maps.Count - 1);
            }
            
            EditorUtility.SetDirty(mapEditor.maps);
            EditorApplication.SaveAssets();
        }
    }

    // Предпросмотр карты
    private void PreviewMap()
    {
        for(int i = 0; i < previewBlockGo.Count; i++)
        {
            if(previewBlockGo[i] != null)
                DestroyImmediate(previewBlockGo[i]);
        }
        previewBlockGo.Clear();

        string TagRoadSpawnPoint = "RoadSpawnPoint";

        int _inRow = 10;
        int _inRoadRow = 3;

        int _road = 1;
        int _row = 1;
        int _coll = 0;

        foreach(Transform _point in previewBlock.GetComponentsInChildren<Transform>())
        {
            // Только точки спавна дороги
            if(_point.CompareTag(TagRoadSpawnPoint))
            {

                int _index = mapEditor.map[_coll, (_row - 1)];
                if(_index > 0)
                {
                    MapGO _mapGO = mapEditor.maps.FindMapGOByIndex(_index);
                    if(_mapGO != null)
                    {
                        GameObject _go = Instantiate(_mapGO.go);
                        _go.transform.parent = _point.parent;
                        _go.transform.localPosition = _point.localPosition;
                        _go.transform.localRotation = _point.localRotation;
                        previewBlockGo.Add(_go);
                    }
                }

                _coll++;

                // Если перебрали все точки на линии дорожки
                if(_coll >= _inRow)
                {
                    _coll = 0;
                    _row++;
                }

                // Если перебрали все линии дорожки
                /*if(_row >= _inRoadRow)
                {
                    _row = 0;
                    _road++;
                }*/
            }
        }
    }

    // Линия (для редактора)
    private void Line()
    {
        EditorGUILayout.Space();
        //EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1f));
        EditorGUILayout.Space();
    }

}
