using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIWindow : MonoBehaviour
{
    // Показать окно
    virtual public void Show()
    {
        this.gameObject.SetActive(true);
    }

    // Скрыть окно
    virtual public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
