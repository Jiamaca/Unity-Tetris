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
    private GameObject mainMenu;
    private GameObject currentScoreText;
    private GameObject nextTetromino;

    private Transform nextTetrominoMiniature;

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
        mainMenu = mainCanvas.transform.Find("PresentationMenu").gameObject;
        currentScoreText = mainCanvas.transform.Find("ScorePainel/CurrentScoreText").gameObject;
        nextTetrominoMiniature = GameObject.Find("MiniaturePivot").transform;

        //SpawnTetromino();
        mainMenu.SetActive(true);
    }

    public void SpawnTetromino()
    {

        int index = Random.Range(0, tetrominos.Count);

        if (nextTetromino == null) nextTetromino = tetrominos[index];


        Instantiate(nextTetromino, spawnPosition, Quaternion.identity);

        if (nextTetrominoMiniature.childCount == 1) Destroy(nextTetrominoMiniature.GetChild(0).gameObject);
        
        int nextIndex = Random.Range(0, tetrominos.Count);
        nextTetromino = tetrominos[nextIndex];

        GameObject miniature = Instantiate(nextTetromino, nextTetrominoMiniature);
        Tetromino t = miniature.GetComponent<Tetromino>();
        miniature.transform.localPosition = Vector3.zero - (t.massCenter * 0.85f);
        miniature.transform.localScale *= 0.85f;
        t.enabled = false;
    }

    public void StartGame()
    {
        Debug.Log("Starting new game...");
        mainMenu.SetActive(false);
        SpawnTetromino();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");

        PlayAudioClip(gameOverSound);

        gameOverMenu.SetActive(true);
        Destroy(nextTetrominoMiniature.gameObject);

        Transform gameOverScoreText = gameOverMenu.transform.Find("GameOverPanel/GameOverScoreText");
        TextMeshProUGUI tmp = gameOverScoreText.GetComponent<TextMeshProUGUI>();
        tmp.text = score.ToString("000000");

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

        textMeshPro.text = score.ToString("000000");
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (source.isPlaying && (clip.name != "Line Clear" || clip.name != "Fail")) return;

        source.clip = clip;
        source.Play();
    }
}   

