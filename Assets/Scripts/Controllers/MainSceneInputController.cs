using UnityEngine;
using System.Collections;

public class MainSceneInputController : InputController
{
    // Начало клика
    public override void OnTouchStart()
    {
        deltaPosX = 0;
        deltaPosY = 0;
    }

    // Двойной клика
    public override void OnDoubleTap()
    {

    }

    // Конец клика
    public override void OnTouchEnd()
    {
      
    }

    // Перемещение по экрану (после нажатия)
    public override void OnTouchMovie()
    {
        touchTime += Time.deltaTime;
    }
}
