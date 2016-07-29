using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockRotator : MonoBehaviour
{
    // Контроллер ввода
    [HideInInspector]
    public InputController inputController;

    // Разрешить глобальное вращение
    public bool rotate = true;
    // Предыдущий блок
    //[HideInInspector]
    public Transform beforeBlock;
    // Прослушивать события вращения
    [HideInInspector]
    public bool rotateListen = false;
    // Блок вращался игроком
    private bool rotatedPlayer = false;

    // Глобальная скорость вращения
    private float rotateSpeed = 50.0f;
    // Локальная скорость вращения
    private float rotateSpeedLocal = 100.0f;
    private float maxLocalAngle = 15.0f;
    // Стартовая ротация (для локального вращения)
    private Quaternion localRotationAngle;
    // Точка относительно которой идет расчет угла
    private Transform localJointPoint;
    // Центральные точки дорожек
    private List<Transform> allCenterJointPoints;
    // Текущий индекс точки
    private int currentJointPointIndex = -1;

    // Длина слеша по экрану
    private float dragLength = 0;
    // Максимальная длина слеша по экрану
    private float dragLengthMax = 5.0f;

    // Конечный угол локального поворота
    private Quaternion localEndAngle = Quaternion.identity;
    private bool localRotate = false;

    private Quaternion globalEndAngle = Quaternion.identity;
    private float globalEndEulerAngle = 0;
    private bool globalRotate = false;
    private float globalRotateFactor = 5.0f;
    private bool globalEasing = false;
    private float globalSwipeDirection = 0.0f;

    private float movieDeltaCount = 0;
    private float deltaLength = 0;
    private float deltaSpeed = 0;

    void Start()
    {
        if(allCenterJointPoints == null)
            allCenterJointPoints = GetAllJointPointsCenter(gameObject);

        float _minAngle = -1;
        foreach(Transform _tJP in allCenterJointPoints)
        {
            int _index = allCenterJointPoints.IndexOf(_tJP);
            float _a = QuaternionDiff(_tJP.position);
            _a = (_a < 0)?(-_a):_a;

            if(_minAngle < 0 || _a < _minAngle)
            {
                _minAngle = _a;
                currentJointPointIndex = _index;
            }
        }
    }

    void Update ()
    {

        // Если разрешен глобальный боворот (полный поворот вокруг оси)
        if(rotate)
        {
            // Если включено управление данным блоком
            if(rotateListen)
            {
                if(!rotatedPlayer)
                    rotatedPlayer = true;

                float _mouseX = inputController.DeltaX();
                dragLength += _mouseX * rotateSpeed * Time.deltaTime;

                if(inputController.TouchTime() > 0.4f)
                {
                    if(dragLength >= dragLengthMax || dragLength <= -dragLengthMax)
                    {
                        float _mod = (dragLength > 0) ? 1.0f : -1.0f;
                        globalSwipeDirection = _mod;
                        dragLength = 0;
                        float _angle = _mod * maxLocalAngle;
                        Transform _JointPointTransformA = GetJointPoints(this.gameObject, true);
                        float _QABDiff = QuaternionDiff(_JointPointTransformA.position);

                        _QABDiff -= _angle;
                        if(_QABDiff > maxLocalAngle || _QABDiff < -maxLocalAngle)
                        {
                            if(_QABDiff > maxLocalAngle)
                                globalEndEulerAngle = maxLocalAngle - (_QABDiff - maxLocalAngle);
                            else if(_QABDiff < -maxLocalAngle)
                                globalEndEulerAngle = -(maxLocalAngle + (_QABDiff + maxLocalAngle));
                            else
                                globalEndEulerAngle = 0;
                        }
                        else
                        {
                            globalEndEulerAngle = _angle;
                        }

                        globalRotateFactor = 50.0f;
                        globalRotate = true;
                    }
                }
            }

            if(globalRotate)
            {
                float globalEndEulerAngleTmp = 0;
                // Равномерное вращение
                globalEndEulerAngleTmp = Mathf.MoveTowards(globalEndEulerAngle, 0, Time.deltaTime * globalRotateFactor * 120.0f);
                // Вращение с замедлением
                // globalEndEulerAngleTmp = Mathf.Lerp(globalEndEulerAngle, 0, Time.deltaTime * globalRotateFactor);
                
                float angleRotate = 0;

                if(globalEndEulerAngleTmp > -0.3f && globalEndEulerAngleTmp < 0.3f)
                {
                    if(globalEasing)
                    {
                        angleRotate = globalEndEulerAngle - globalEndEulerAngleTmp;
                        globalEndEulerAngleTmp = (globalSwipeDirection > 0) ? (globalEndEulerAngleTmp - 15.0f) : (globalEndEulerAngleTmp + 15.0f);
                        globalEasing = false;
                        globalRotateFactor = 1.0f;
                        globalEndEulerAngle = globalEndEulerAngleTmp;
                    }
                    else
                    {
                        angleRotate = globalEndEulerAngle;
                        globalEndEulerAngle = 0;
                        globalRotate = false;
                    }
                }
                else
                {
                    angleRotate = globalEndEulerAngle - globalEndEulerAngleTmp;
                    globalEndEulerAngle = globalEndEulerAngleTmp;
                }
                transform.Rotate(Vector3.forward, angleRotate);
            }
        }
        // Если это локальное вращение (на определенный минимальный / максимальный угол)
        else
        {
            if (localJointPoint == null)
                SetLocalRotationAngle();

            // Если включено управление данным блоком
            if(rotateListen)
            {
                float _mouseX = inputController.DeltaX();
                dragLength += _mouseX * rotateSpeedLocal * Time.deltaTime;

                if(dragLength >= dragLengthMax * 2 || dragLength <= -dragLengthMax * 2)
                {
                    float _mod = (dragLength > 0) ? 1.0f : -1.0f;
                    dragLength = 0;
                    float _angle = _mod * maxLocalAngle;
                    float _QABDiff = QuaternionDiff(localJointPoint.position);
                    _QABDiff -= _angle;

                    if(_QABDiff >= maxLocalAngle)
                    {
                        localEndAngle = localRotationAngle * Quaternion.Euler(0, 0, -maxLocalAngle);
                    }
                    else if(_QABDiff <= -maxLocalAngle)
                    {
                        localEndAngle = localRotationAngle * Quaternion.Euler(0, 0, maxLocalAngle);
                    }
                    else
                    {
                        localEndAngle = localRotationAngle * Quaternion.Euler(0, 0, 0);
                    }

                    localRotate = true;
                }
            }
            else
            {
                dragLength = 0;
            }

            if(localRotate)
            {
               
                float _AB = AngleAroundAxis(transform.rotation.eulerAngles, localEndAngle.eulerAngles, Vector3.forward);
                if(_AB >= -0.05f && _AB <= 0.05f)
                {
                    transform.rotation = localEndAngle;
                    localRotate = false;
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, localEndAngle, Time.deltaTime * 20.0f);
                }  
            }
                
        }
    }

    public void RotateEnd()
    {

        if(rotate)
        {
            float swipeLength = inputController.TouchLength();
            float swipeTime = inputController.TouchTime();

            if(swipeTime < 0.4 && swipeLength > 60.0f)
            {
                Vector3 swipeVector = new Vector3(inputController.TouchVector().x, inputController.TouchVector().y, 0);
                float swipeDirection = (AngleAroundAxis(swipeVector, Vector3.forward, Vector3.up) > 0) ? -1 : 1;
                globalSwipeDirection = swipeDirection;

                int countJP = allCenterJointPoints.Count;
                if(countJP > 1)
                {
                    float currentJointPointAngle = QuaternionDiff(allCenterJointPoints[currentJointPointIndex].position);
                    //Debug.Log("currentJointPointAngle => " + currentJointPointAngle);
                    //Debug.Log("swipe direction => " + swipeDirection);
                    if(currentJointPointAngle > maxLocalAngle)
                    {
                        currentJointPointIndex = (swipeDirection > 0) ? currentJointPointIndex : currentJointPointIndex - 1;
                    }
                    else if(currentJointPointAngle < -maxLocalAngle)
                    {
                        currentJointPointIndex = (swipeDirection > 0) ? currentJointPointIndex + 1 : currentJointPointIndex;
                    }
                    else
                    {
                        currentJointPointIndex = (swipeDirection > 0) ? currentJointPointIndex + 1 : currentJointPointIndex - 1;
                    }
  
                    if(currentJointPointIndex >= countJP)
                        currentJointPointIndex = 0;
                    else if(currentJointPointIndex < 0)
                        currentJointPointIndex = countJP - 1;
                }

                float nextJointPointAngle = QuaternionDiff(allCenterJointPoints[currentJointPointIndex].position);
                if(nextJointPointAngle < 0)
                    nextJointPointAngle = 180 + (180 + nextJointPointAngle);

                if(countJP == 1)
                {
                    if(nextJointPointAngle == 0)
                    {
                        if(swipeDirection > 0)
                        {
                            nextJointPointAngle = 360.0f;
                        }
                        else
                        {
                            nextJointPointAngle = 0f;
                        }
                    }
                }

                if(swipeDirection > 0)
                {
                    globalEndEulerAngle = nextJointPointAngle;
                }
                else
                {
                    globalEndEulerAngle = -(360.0f - nextJointPointAngle);
                }

                if(globalEndEulerAngle > 0)
                {
                    globalEndEulerAngle += 15.0f;
                }
                else if(globalEndEulerAngle < 0)
                {
                    globalEndEulerAngle -= 15.0f;
                }
                else
                {
                    globalEndEulerAngle += (swipeDirection > 0) ? 15.0f : -15.0f;
                }

                globalEasing = true;
                globalRotateFactor = 5.0f;
                globalRotate = true;
            }
        }

        
    }

    public void SetLocalRotationAngle()
    {
        if(localJointPoint == null)
            localJointPoint = GetJointPoints(gameObject, true);


        float _QABDiff = QuaternionDiff(localJointPoint.position);
        localRotationAngle = transform.rotation * Quaternion.Euler(0, 0, _QABDiff);

        if(_QABDiff >= (maxLocalAngle / 2.0f))
            localEndAngle = localRotationAngle * Quaternion.Euler(0, 0, -maxLocalAngle);
        else if(_QABDiff <= (-maxLocalAngle / 2.0f))
            localEndAngle = localRotationAngle * Quaternion.Euler(0, 0, maxLocalAngle);
        else
            localEndAngle = localRotationAngle * Quaternion.Euler(0, 0, 0);

        localRotate = true;
    }


    // Угол между вектором и осью Y
    private float QuaternionDiff(Vector3 a)
    {
        Vector3 _centerPoint = transform.position;
        _centerPoint = new Vector3(_centerPoint.x, _centerPoint.y, a.z);
        Vector3 _jointVectorL = a - _centerPoint;
        return AngleAroundAxis(_jointVectorL, Vector3.up, Vector3.forward);
    }


    // Угол между векторами, с точкой вращения по оси
    public float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);
        float angle = Vector3.Angle(dirA, dirB);
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }


    // Получить точку стыковки (получаем только самую верхнюю крайнюю точку)
    public Transform GetJointPoints(GameObject obj, bool center = false)
    {
        int cntCenter = 0;
        Transform _JointPoint = null;
        if(obj != null)
        {
            foreach(Transform _T in obj.transform)
            {
                if(_T.CompareTag("JointPoint"))
                {
                    if(_JointPoint == null)
                    {
                        if(!center)
                            _JointPoint = _T;
                        else if(_T.name == "JointPoint_Center")
                        {
                            _JointPoint = _T;
                        }
                            
                    }
                    else
                    {
                        // Все точки
                        if(!center && _T.position.y >= _JointPoint.position.y)
                            _JointPoint = _T;
                        // Центральная точка
                        else if(_T.name == "JointPoint_Center" && _T.position.y >= _JointPoint.position.y)
                        {
                            _JointPoint = _T;
                        }
                    }
                }
            }
        }

        return _JointPoint;
    }


    // Получить все центральные точки дорожек стыковки
    public List<Transform> GetAllJointPointsCenter(GameObject obj)
    {
        List<Transform> listPoints = new List<Transform>();

        if(obj != null)
        {
            foreach(Transform _T in obj.transform)
            {
                if(_T.CompareTag("JointPoint"))
                {
                    if(_T.name == "JointPoint_Center")
                    {
                        listPoints.Add(_T);
                    }
                }
            }
        }

        return listPoints;
    }


    // Сброс параметров
    public void Reset()
    {
        rotate = true;
        beforeBlock = null;
        rotatedPlayer = false;
        rotateListen = false;
        localEndAngle = Quaternion.identity;
        localRotate = false;
        localRotationAngle = Quaternion.identity;
        localJointPoint = null;
    }

}
