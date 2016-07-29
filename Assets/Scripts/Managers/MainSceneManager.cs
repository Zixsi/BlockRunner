using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainSceneManager : MonoBehaviour
{
    // Загрузчик
    public UILoaderWindow loaderWindowUI;
    // Пустое окно
    public UISimpleWindow emptyWindowUI;
    // Баланс игрока
    public Text balansUI;

    // Контроллер ввода
    public MainSceneInputController inputController;
    // Данные игрока
    public PlayerData playerData;
    // Менеджер сохранений
    [HideInInspector]
    public SaveManager saveManager;

    void Start()
    {
        saveManager = ScriptableObject.CreateInstance<SaveManager>();
        saveManager.Init();

        if(balansUI != null)
        {
            balansUI.text = playerData.Balans.ToString();
        }
    }

    // Старт игры
    public void StartGame()
    {
        if(loaderWindowUI != null)
        {
            loaderWindowUI.sceneName = "Level";
            loaderWindowUI.Show();
        } 
    }

    // Окно с предупреждением об отсутствии функционала
    public void EmptyWindow(bool state = false)
    {
        if(emptyWindowUI != null)
        {
            if(state)
                emptyWindowUI.Show();
            else
                emptyWindowUI.Hide();
        }
    }
}
