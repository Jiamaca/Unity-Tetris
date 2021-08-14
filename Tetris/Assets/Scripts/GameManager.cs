using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Main { get { return main; } }

    private static GameManager main;

    public Vector3 spawnPosition;
    public List<GameObject> tetrominos;

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
        SpawnTetromino();
    }

    public void SpawnTetromino()
    {
        int currIndex = Random.Range(0, tetrominos.Count);
        Instantiate(tetrominos[currIndex], spawnPosition, Quaternion.identity);
    }
}   

