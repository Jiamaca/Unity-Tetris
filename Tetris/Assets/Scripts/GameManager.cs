using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public  static GameManager Main { get { return main; } }
    private static GameManager main;

    public int score = 0;

    public Vector3 spawnPosition;
    public List<GameObject> tetrominos;

    public GameObject mainCanvas;

    public AudioClip gameOverSound;

    private GameObject gameOverMenu;
    private GameObject currentScoreText;

    private AudioSource source;

    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject);
        }
        else
        {
            main = this;
        }
    }

    public void Start()
    {
        source = GetComponent<AudioSource>();

        gameOverMenu = mainCanvas.transform.Find("GameOverMenu").gameObject;
        currentScoreText = mainCanvas.transform.Find("CurrentScoreText").gameObject;

        SpawnTetromino();
    }

    public void SpawnTetromino()
    {
        int currIndex = Random.Range(0, tetrominos.Count);
        Instantiate(tetrominos[currIndex], spawnPosition, Quaternion.identity);
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");

        source.clip = gameOverSound;
        source.Play();

        gameOverMenu.SetActive(true);

        Transform gameOverScoreText = gameOverMenu.transform.Find("GameOverPanel/GameOverScoreText");
        TextMeshProUGUI tmp = gameOverScoreText.GetComponent<TextMeshProUGUI>();
        tmp.SetText(string.Format("SCORE: {0}", score));
        tmp.text = string.Format("SCORE: {0}", score);

    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game");
        SceneManager.LoadScene("GameScene");
    }

    public void AddScore(int _score)
    {
        score += _score;

        Debug.Log("New Score: " + _score);
        TextMeshProUGUI textMeshPro = currentScoreText.GetComponent<TextMeshProUGUI>();
        textMeshPro.text = string.Format("SCORE: {0}", score);

    }
}   

