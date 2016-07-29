using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Префаб игрока
    public GameObject playerPrefab;
    // Игрок
    [HideInInspector]
    public GameObject player;
    // Физическое тело игрока
    private Rigidbody playerRig;
    // Скрипт игрока
    private Player playerScript;

    // Фоновая музыка
    public AudioSource backgroundMusic;

    // Данные игрока
    public PlayerData playerData;
    // Менеджер сохранений
    public SaveManager saveManager;

    // Камера
    public Camera cam = null;
    private CameraLook cameraLookScript;
    // Контроллер управления
    public InputController inputController;
    // Менеджер уровня
    public LevelManager levelManager;
    // Стартовая точка
    public Transform startPoint;
    // Менеджер интерфейса
    public GameUIManager gameUIManager;

    // Статус игры
    public GameStateList gameState = GameStateList.Play;
    public enum GameStateList {Play, Pause, GameOver};

    void Awake()
    {

    }

    void Start()
    {
        if(levelManager != null)
        {
            levelManager.gameManager = this;
            levelManager.Generate();
        }

        if(playerPrefab != null)
        {
            player = Instantiate(playerPrefab);
            if(player != null)
            {
                player.transform.position = startPoint.position;
                player.transform.rotation = startPoint.rotation;
                playerRig = player.GetComponent<Rigidbody>() as Rigidbody;
                playerScript = player.GetComponent<Player>() as Player;

                cameraLookScript = cam.GetComponent<CameraLook>() as CameraLook;
                if (cameraLookScript != null)
                    cameraLookScript.target = player.transform;
            }
        }

        inputController.cam = (Camera) cam;

        saveManager = ScriptableObject.CreateInstance<SaveManager>();
        saveManager.Init();
        StartCoroutine(GameLoop());
	}

    void OnApplicationPause(bool status)
    {
        if(status)
        {
            if(gameState == GameStateList.Play)
                GamePause();
        }
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(GameStart());
        yield return StartCoroutine(GamePlaying());
        yield return StartCoroutine(GameEnd());
    }

    // Начало игры
    private IEnumerator GameStart()
    {
        yield return StartCoroutine(gameUIManager.ShowStartCounterUI());

        gameUIManager.SetActivePauseButton(true);
        inputController.SetControl(true);
        playerScript.SetMovie(true);
        yield return null;
    }

    // Игра
    private IEnumerator GamePlaying()
    {
        // Пока игрок жив
        while(!playerScript.die)
        {
            levelManager.CheckBlocks(player.transform.position);
            yield return null;
        }
    }

    // Конец игры
    private IEnumerator GameEnd()
    {
        GameOver();
        yield return null;
    }

    // Перезагрузка уровня
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Проигрыш
    public void GameOver()
    {
        gameState = GameStateList.GameOver;
        inputController.SetControl(false);
        playerScript.SetMovie(false);
        backgroundMusic.Pause();


        int _score = playerData.GetScore();
        int _hightScore = int.Parse(saveManager.Get("highscore"));
        if(_score > _hightScore)
            saveManager.Set("highscore", _score.ToString());

        gameUIManager.GameOverMenu();
    }

    // Пауза
    public void GamePause()
    {
        gameState = GameStateList.Pause;
        inputController.SetControl(false);
        playerScript.SetMovie(false);
        backgroundMusic.Pause();
        gameUIManager.PauseMenu(true);
    }

    // Продолжить игру
    public void GameResume()
    {
        gameState = GameStateList.Play;
        gameUIManager.PauseMenu(false);
        backgroundMusic.Play();
        inputController.SetControl(true);
        playerScript.SetMovie(true);
    }

    // Главный экран
    public void GameHome()
    {
        SceneManager.LoadScene("Main");
    }

}
