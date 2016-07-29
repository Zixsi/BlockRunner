using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    // Объекты уровня
    [HideInInspector]
    public GObjects gObjects = new GObjects();
    // Карты
    [HideInInspector]
    public Maps maps;

    // Последний блок
    [HideInInspector]
    public GameObject LastBlock = null;
    // Тег контейнера сетки объектов дороги
    [HideInInspector]
    public string TagContainerRoadSpawnPoint = "";
    // Тег сетки объектов дороги
    [HideInInspector]
    public string TagRoadSpawnPoint = "";
    // Тег контейнера сетки объектов блока
    [HideInInspector]
    public string TagContainerBlockSpawnPoint = "";
    // Тег сетки объектов блока
    [HideInInspector]
    public string TagBlockSpawnPoint = "";
    // Контейнер пула объектов уровня
    [HideInInspector]
    public Transform PollContainer;

    // Пулл объектов уровня
    private PollObjects PollGameObjects;

    public GameManager gameManager;

    void Awake() 
    {
        PollGameObjects = gameObject.AddComponent<PollObjects>() as PollObjects;
    }

	void Start()
    {
        // Пул блоков
        foreach(GameObject _type in gObjects.blocks)
        {
            if(_type != null)
                PollGameObjects.FillPool(_type, 10, PollContainer);
        }

        // Пул мостов
        foreach(GameObject _type in gObjects.briges)
        {
            if(_type != null)
                PollGameObjects.FillPool(_type, 10, PollContainer);
        }

        // Пул объектов уровня
        foreach(MapGO _type in maps.mapGameObjects)
        {
            if(_type != null && _type.go != null)
                PollGameObjects.FillPool(_type.go, 10, PollContainer);
        }

    }

    // Генерация уровня
    public void Generate()
    {
        // Генерируем часть уровня
        for (int i = 0; i < 4; i++)
        {
            AddBlock();
        }
    }

    // Добавить блок уровня
    private void AddBlock()
    {
        GameObject _Block = gObjects.blocks[Random.Range(0, gObjects.blocks.Count)];

        if (_Block != null)
        {
            GameObject _go = PollGameObjects.Get(_Block);
            if (_go != null)
            {
                _go.transform.parent = transform;
                _go.SetActive(true);

                BlockRotator _goBlockRotator = _go.GetComponent<BlockRotator>() as BlockRotator;
                if (_goBlockRotator != null)
                {
                    _goBlockRotator.beforeBlock = LastBlock.transform;
                }

                SetBlockBrige(_go, true);
                FillSpawnPointRoad(_go);
                FillSpawnPointBlock(_go);

                Bounds _goBounds = GetBounds(_go);
                Vector3 _size = _goBounds.size;

                _go.transform.rotation = new Quaternion(0, 0, 0, 0);
                if (LastBlock == null)
                {
                    _go.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    _go.transform.localPosition = new Vector3(0, 0, LastBlock.transform.localPosition.z + _size.z);
                    // Поворот блока от 18 до 342 градусов
                    _go.transform.localRotation *= Quaternion.Euler(0, 0, Mathf.Ceil(Random.Range(1.0f, 18.0f)) * 18.0f);
                    //_go.transform.localRotation *= Quaternion.Euler(0, 0, -50.0f);
                }

                LastBlock = _go;
            }
        }
    }

    // Задать новые мосты для блока
    private void SetBlockBrige(GameObject block, bool random = false)
    {
        Transform _blockT = block.transform;
        GameObject _typeBrige = null;
        GameObject _goBrige;

        if (!random)
            _typeBrige = gObjects.briges[Random.Range(0, gObjects.briges.Count)];

        foreach(Transform _brige in _blockT)
        {
            if(_brige.CompareTag("Brige") && _brige.gameObject.activeSelf == true)
            {
                if(random)
                    _typeBrige = gObjects.briges[Random.Range(0, gObjects.briges.Count)];

                if(_typeBrige.name != _brige.name)
                {
                    _goBrige = PollGameObjects.Get(_typeBrige);
                    if(_goBrige != null)
                    {
                        _goBrige.transform.parent = _blockT;
                        _goBrige.transform.localPosition = _brige.localPosition;
                        _goBrige.transform.localRotation = _brige.localRotation;
                        _goBrige.SetActive(true);

                        _brige.parent = PollContainer.transform;
                        _brige.localPosition = Vector3.zero;
                        _brige.localRotation = Quaternion.identity;
                        _brige.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // Получить размеры составного объекта
    private Bounds GetBounds(GameObject block)
    {
        Renderer[] renderers = block.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1, ni = renderers.Length; i < ni; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            return bounds;
        }
        else
        {
            return new Bounds();
        }
    }

    // Заселить внешние точки блока
    public void FillSpawnPointBlock(GameObject block)
    {
        /* if(block != null && TagBlockSpawnPoint != "")
         {
             Transform _blockT = block.transform;
             int i = 1;

             foreach(Transform _point in block.GetComponentsInChildren<Transform>())
             {
                 // Только внешние точки спавна
                 if (_point.CompareTag(TagBlockSpawnPoint))
                 {
                   
                     i++;
                 }
             }
         }*/
    }

    // Заселить дорожки
    public void FillSpawnPointRoad(GameObject block)
    {
        if(block != null && TagRoadSpawnPoint != "")
        {
            int _inRow = 10;
            int _inRoadRow = 3;

            int _road = 1;
            int _row = 0;
            int _coll = 0;

            int[,] _map = SelectMap();
            BlockObjects _goBlock = block.GetComponent<BlockObjects>() as BlockObjects;

            foreach(Transform _point in block.GetComponentsInChildren<Transform>())
            {
                // Только точки спавна дороги
                if (_point.CompareTag(TagRoadSpawnPoint))
                {
                    MapGO _mapGo = maps.FindMapGOByIndex(_map[_coll, _row]);
                    if(_mapGo != null && _mapGo.go != null)
                    {
                        GameObject _go = (GameObject) PollGameObjects.Get(_mapGo.go);
                        _go.transform.parent = _point.parent;
                        _go.transform.localPosition = _point.localPosition;
                        _go.transform.localRotation = _point.localRotation;
                        _go.SetActive(true);

                        if(_goBlock != null)
                            _goBlock.objects.Add(_go);
                    }

                    _coll++;

                    // Если перебрали все точки на линии дорожки
                    if(_coll >= _inRow)
                    {
                        _coll = 0;
                        _row++;
                    }

                    // Если перебрали все линии дорожки
                    if(_row >= _inRoadRow)
                    {
                        _row = 0;
                        _road++;
                        _map = SelectMap();
                    }
                }
            }
           
        }
    }

    // Выбор карты объектов
    private int[,] SelectMap()
    {
        List<Map> _difficultMap = null;
        int _difficult = 1;

        switch(_difficult)
        {
            case 2:
                _difficultMap = maps.roadMapMedium;
            break;
            case 3:
                _difficultMap = maps.roadMapHard;
            break;
            default:
                _difficultMap = maps.roadMapEasy;
            break;
        }

        Map _mapObj = _difficultMap[Random.Range(0, maps.roadMapEasy.Count)];
        return _mapObj.GetMap();
    }


    // Проверка на блоки которые слишком далеко
    public void CheckBlocks(Vector3 playerPosition)
    {
        float _dist = Vector3.Magnitude(playerPosition - LastBlock.transform.position);
        // Добавляем новые блоки если игрок близко к концу
        if (_dist < 50.0f)
        {
            // Убираем старые блоки если они слишком далеко
            // Запускаем 3 раза, т.к. могли что то пропустить
            for (int i = 0; i < 3; i++)
            {
                foreach (Transform _block in transform)
                {
                    if (_block.CompareTag("Block"))
                    {
                        if (Vector3.Magnitude(playerPosition - _block.position) > 50.0f)
                            ResetBlock(_block);
                    }
                }
            }

            // Генерируем новые блоки
            Generate();
        }   

    }


    // Сброс блока (перемещение объектов в пул)
    private void ResetBlock(Transform block)
    {
        if(block != null)
        {
            BlockRotator _goBlockRotator = block.GetComponent<BlockRotator>() as BlockRotator;
            if(_goBlockRotator != null && !_goBlockRotator.rotate)
            {
                // Очищаем от объектов
                BlockObjects _goBlock = block.GetComponent<BlockObjects>() as BlockObjects;
                if(_goBlock != null)
                {
                    for(int n = 0; n < 5; n++)
                    {
                        for(int i = 0; i < _goBlock.objects.Count; i++)
                        {
                            if(_goBlock.objects[i] != null)
                            {
                                Transform _go = _goBlock.objects[i].transform;
                                _go.parent = PollContainer.transform;
                                _go.position = Vector3.zero;
                                _go.rotation = Quaternion.identity;
                                _go.gameObject.SetActive(false);
                            }

                            if(_goBlock.objects[i].transform.parent == PollContainer.transform)
                                _goBlock.objects.RemoveAt(i);
                        }
                    }

                    _goBlock.objects.Clear();
                }

                // Перемещаем блок в пул
                block.parent = PollContainer.transform;
                block.localPosition = Vector3.zero;
                block.localRotation = Quaternion.identity;
                _goBlockRotator.Reset();
                block.gameObject.SetActive(false);
            }
        }
    }

}
