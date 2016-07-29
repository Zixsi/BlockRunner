using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharactersSlider : MonoBehaviour
{
    public MainSceneManager mainGameManager;
    private Transform t;

    // Персонажи
    public Character[] characters;
    // Расстояние между персонажами
    public float distance = 1.0f;
    // Индекс текущего персонажа
    [HideInInspector]
    public int index = 0;

    // Кнопка купить
    public GameObject bayButton;
    // Значек блокировки
    public GameObject lockImg;

    // Анимируем
    private bool _animate = false;
    // Следующая позиция
    private float _nextPositonX = 0;

    void Start()
    {
        t = transform;

        if(characters.Length > 0)
        {
            Init();
        }
    }

    // Инициализация слайдера
    private void Init()
    {
        for(int i = 0; i < characters.Length; i++)
        {
            GameObject character = Instantiate(characters[i].model);
            character.transform.parent = t;
            character.transform.localPosition = new Vector3(i * distance, 0, 0);
        }

        _nextPositonX = index * -distance;
        SetButtonPrice();
    }

    // Следующий персонаж
    public void Next()
    {
        if(!_animate)
        {
            index++;
            if(index < characters.Length)
                _nextPositonX = index * -distance;
            else
                index = characters.Length - 1;

            SetButtonPrice();
            _animate = true;
        }
    }

    // Предыдущий персонаж
    public void Prev()
    {
        if(!_animate)
        {
            index--;
            if(index >= 0)
                _nextPositonX = index * -distance;
            else
                index = 0;

            SetButtonPrice();
            _animate = true;
        }
    }

    // Установить цену на кнопке
    private void SetButtonPrice()
    {
        if(bayButton != null)
        {
            if(characters[index].price > 0)
            {
                StartCoroutine(ShowButton());
                Text _text = bayButton.GetComponentInChildren<Text>();
                Button _button = bayButton.GetComponentInChildren<Button>();
                if(_text != null)
                {
                    _text.text = "" + characters[index].price;
                }

                if(mainGameManager.playerData.Balans < characters[index].price && _button.IsInteractable())
                {
                    _button.interactable = false;
                }
                else if(mainGameManager.playerData.Balans >= characters[index].price && !_button.IsInteractable())
                {
                    _button.interactable = true;
                }
            }
            else
            {
                StartCoroutine(HideButton());
            }
        }
    }

    IEnumerator ShowButton()
    {
        if(bayButton != null)
        {
            CanvasRenderer[] canvasRenderers = bayButton.GetComponentsInChildren<CanvasRenderer>();
            foreach(CanvasRenderer cr in canvasRenderers)
            {
                cr.SetAlpha(1.0f);
            }
            /*float _alpha = 0;
            while(_alpha < 1.0f)
            {
                foreach(CanvasRenderer cr in canvasRenderers)
                {
                    cr.SetAlpha(_alpha);

                }
                _alpha = Mathf.MoveTowards(_alpha, 1.0f, Time.deltaTime * 3.0f);
                yield return null;
            }*/
        }
        yield return null;
    }

    IEnumerator HideButton()
    {
        if(bayButton != null)
        {
            CanvasRenderer[] canvasRenderers = bayButton.GetComponentsInChildren<CanvasRenderer>();
            foreach(CanvasRenderer cr in canvasRenderers)
            {
                cr.SetAlpha(0.0f);
            }
        }
        yield return null;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Next();
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Prev();
        }


        if(_animate)
        {
            float _distance = t.position.x - _nextPositonX;
            if(_distance > -0.2f && _distance < 0.2f)
            {
                t.position = new Vector3(_nextPositonX, t.position.y, t.position.z);
                _animate = false;
            }
            else
            {
                t.position = new Vector3(Mathf.MoveTowards(t.position.x, _nextPositonX, Time.deltaTime * 20.0f), t.position.y, t.position.z);
            }
        }
    }

}
