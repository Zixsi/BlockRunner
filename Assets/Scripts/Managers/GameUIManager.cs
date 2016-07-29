using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    public GameManager gameManager;

    //=== Графические элементы экрана ===//

    // Счетчик отсчета
    public GameObject startCounterUI;
    // Кнопка вызова меню паузы
    public GameObject pauseButtonUI;
    // Счет
    public GameObject scoreUI;
    private Text scoreUIText;

    //=== End Графические элементы экрана ===//

    // Меню паузы
    public GameObject menuPause;
    // Меню проигрыша
    public GameObject menuGameover;
    public Text gameoverScoreUIText;
    public Text gameoverHightScoreUIText;

    void Start()
    {
        if(gameManager == null)
        {
            GameObject _objGameManager = (GameObject) GameObject.FindGameObjectWithTag("GameManager");
            if(_objGameManager != null)
                gameManager = (GameManager) _objGameManager.GetComponent<GameManager>();
        }

        if(scoreUI != null)
            scoreUIText = (Text) scoreUI.GetComponentInChildren<Text>();
    }

    // Меню паузы
    public void PauseMenu(bool show = true)
    {
        pauseButtonUI.SetActive(!show);
        menuPause.SetActive(show);
    }

    // Активировать кнопку паузы
    public void SetActivePauseButton(bool stat = true)
    {
        Button but = pauseButtonUI.GetComponent<Button>();
        but.interactable = stat;
    }

    // Меню проигрыша
    public void GameOverMenu()
    {
        menuPause.SetActive(false);
        menuGameover.SetActive(true);

        int _score = gameManager.playerData.GetScore();
        int _hightScore = int.Parse(gameManager.saveManager.Get("highscore"));

        if(gameoverScoreUIText != null)
            gameoverScoreUIText.text = _score.ToString();

        if(gameoverHightScoreUIText != null)
            gameoverHightScoreUIText.text = _hightScore.ToString();
    }

    // Отсчет времени до начала
    public IEnumerator ShowStartCounterUI()
    {
        yield return new WaitForSeconds(1.5f);
        while(gameManager.gameState != GameManager.GameStateList.Play)
        {
            yield return null;
        }
        yield return null;
    }


    // Задать значение для очков
    public void SetScoreUI()
    {
        if(scoreUIText != null)
            scoreUIText.text = gameManager.playerData.GetScore().ToString();
    }

}
