using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour
{
    // Камера
    public Camera cam;
    // Блок которым управляем
    private BlockRotator block;
    // Управление (вкл / выкл)
    private bool enabledControl = false;

    // Id пальца
    private int touchId = -1;
    // Кол-во нажатий
    private int tapCount = 0;
    // Двойное нажатие
    private bool doubleTap = false;
    // Начальная позиция пальца
    private Vector2 touchStartPos = Vector2.zero;
    // Текущее положение пальца
    private Vector2 touchPos = Vector2.zero;
    // Дельта
    private Vector2 touchDelta = Vector2.zero;
    // Длина вектора
    private float touchLength = 0;
    // Время движения пальца
    [HideInInspector]
    public float touchTime = 0;
    // Вектор (напрвление)
    private Vector2 touchVector = Vector2.zero;

    // Смещение по оси X
    [HideInInspector]
    public float deltaPosX = 0;
    // Смещение по оси Y
    [HideInInspector]
    public float deltaPosY = 0;

    void Update()
    {
        #if(UNITY_ANDROID || UNITY_IOS)
            DetectTouch();
        #endif

        #if(UNITY_EDITOR || UNITY_STANDALONE)
            DetectMouseTouch();
        #endif
    }

    // Установить управление
    public void SetControl(bool val = false)
    {
        enabledControl = (val == true) ? true : false;

        if(!enabledControl)
        {
            if(block != null)
                block.rotateListen = false;
        }
    }

    // Обработка мыши.
    private void DetectMouseTouch()
    {
        // Нажали клавишу
        if(Input.GetMouseButtonDown(0))
        {
            touchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            touchStartPos = touchPos;

            OnTouchStart();
        }
        // Отпустили клавишу
        else if(Input.GetMouseButtonUp(0))
        {
            // Двойной тап
            if(doubleTap && tapCount >= 2)
            {
                OnDoubleTap();

                doubleTap = false;
                tapCount = 0;
            }
            else
            {
                doubleTap = true;
                tapCount += 1;
            }

            OnTouchEnd();
            Reset();
        }
        // Движение
        else if(Input.GetMouseButton(0))
        {
            touchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            touchDelta = touchPos - touchStartPos;
            touchLength = touchDelta.magnitude;
            touchVector = touchDelta / touchLength;

            deltaPosX = Input.GetAxis("Mouse X");
            deltaPosY = Input.GetAxis("Mouse Y");

            OnTouchMovie();
        }
    }

    // Обработка жестов экрана.
    private void DetectTouch()
    {

        // Перебор всех нажатий на экран
        foreach(var _touchVar in Input.touches)
        {
            touchPos = _touchVar.position;
            tapCount = _touchVar.tapCount;
            // Фаза нажатия
            TouchPhase _touchVarPhase = _touchVar.phase;

            deltaPosX = _touchVar.deltaPosition.x / 6.6f;
            deltaPosY = _touchVar.deltaPosition.y / 6.6f;

            // Начало тача
            if(_touchVarPhase == TouchPhase.Began)
            {
                // Запоминаем палец
                touchId = _touchVar.fingerId;
                touchStartPos = touchPos;

                OnTouchStart();
            }
            // Работаем с запомненым пальцем
            else if(_touchVar.fingerId == touchId)
            {
                touchDelta = touchPos - touchStartPos;

                if(_touchVarPhase == TouchPhase.Moved)
                {
                    touchLength = touchDelta.magnitude;
                    touchVector = touchDelta / touchLength;

                    OnTouchMovie();
                }
                else if(_touchVarPhase == TouchPhase.Canceled || _touchVarPhase == TouchPhase.Ended)
                {
                    // Двойной тап
                    if(doubleTap && tapCount >= 2)
                    {
                        OnDoubleTap();

                        doubleTap = false;
                        tapCount = 0;
                    }
                    else
                    {
                        doubleTap = true;
                        tapCount += 1;
                    }

                    OnTouchEnd();
                    Reset();
                }
            }

        }
        // End foreach
    }


    // Начало клика
    public virtual void OnTouchStart()
    {
        deltaPosX = 0;
        deltaPosY = 0;

        if(enabledControl)
        {
            // Если нет блока
            if(block == null)
                OnTouchMovie();
        }
    }

    // Двойной клика
    public virtual void OnDoubleTap()
    {

    }

    // Конец клика
    public virtual void OnTouchEnd()
    {
        if(enabledControl)
        {
            // Снимаем действия с текущего блока
            if(block != null)
            {
                block.RotateEnd();
                block.inputController = null;
                block.rotateListen = false;
                block = null;
            }
        }
    }

    // Перемещение по экрану (после нажатия)
    public virtual void OnTouchMovie()
    {
        touchTime += Time.deltaTime;

        if(enabledControl)
        {
            // Если нет блока
            if(block == null && cam != null)
            {
                Ray _Ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit _RayHit;
                if(Physics.Raycast(_Ray, out _RayHit))
                {
                    if(_RayHit.transform.parent != null)
                    {
                        block = _RayHit.transform.parent.GetComponent<BlockRotator>() as BlockRotator;
                        if(block != null)
                        {
                            block.inputController = (InputController) this;
                            block.rotateListen = true;
                        }
                    }
                   
                }
            }
        }
    }

    // Сброс переменных
    private void Reset()
    {
        touchId = -1;
        touchStartPos = Vector2.zero;
        touchDelta = Vector2.zero;
        touchVector = Vector2.zero;
        touchLength = 0;
        deltaPosX = 0;
        deltaPosY = 0;
        touchTime = 0;
    }

    public Vector2 TouchStartPosition()
    {
        return touchStartPos;
    }

    public Vector2 TouchCurrentPosition()
    {
        return touchPos;
    }

    public Vector2 TouchDelta()
    {
        return touchDelta;
    }

    public Vector2 TouchVector()
    {
        return touchVector;
    }

    public float TouchLength()
    {
        return touchLength;
    }

    public float TouchTime()
    {
        return touchTime;
    }

    public float DeltaX()
    {
        return deltaPosX;
    }

    public float DeltaY()
    {
        return deltaPosY;
    }

}
