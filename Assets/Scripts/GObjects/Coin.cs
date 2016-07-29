using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    public AudioSource audioSource;
    public MeshRenderer mRenderer;

    private GameManager gameManager;
    private Transform t;

    void Start()
    {
        t = this.transform;
        if(gameManager == null)
        {
            GameObject _objGameManager = GameObject.FindGameObjectWithTag("GameManager");
            if(_objGameManager != null)
                gameManager = _objGameManager.GetComponent<GameManager>() as GameManager;
        }
           

        if(audioSource == null)
            audioSource = GetComponent<AudioSource>() as AudioSource;
        if(mRenderer == null)
            mRenderer = GetComponent<MeshRenderer>() as MeshRenderer;
    }

    void Update()
    {
        t.rotation = Quaternion.Lerp(t.rotation, (t.rotation * Quaternion.Euler(0, 5.0f, 0)), Time.deltaTime * 100.0f);
    }

    void OnTriggerEnter(Collider col)
    {
        if(audioSource != null)
            audioSource.Play();

        if(gameManager != null)
        {
            gameManager.playerData.SetScore(1);
            gameManager.gameUIManager.SetScoreUI();
        }
            
        mRenderer.enabled = false;

        StartCoroutine(Off());
    }

    private IEnumerator Off()
    {
        yield return new WaitForSeconds(0.5f);
        mRenderer.enabled = true;
        t.gameObject.SetActive(false);
    }

}
