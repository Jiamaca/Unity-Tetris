using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour 
{
    public float fallTime = 0.8f;

    private float previousTime = 0;
    private bool freezed = false;

    public static int tileRows = 21;
    public static int tileCollumns = 10;

    private static Transform[,] tilesMatrix = new Transform[tileCollumns, tileRows];

    private AudioSource source;

    public AudioClip rotateSound;
    public AudioClip moveSound;
    public AudioClip clearSound;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (freezed) return;

        Rotate();

        Move();

        if (Time.time - previousTime > (Input.GetKey(KeyCode.S) ? fallTime / 10 : fallTime))
        {
            Vector3 newPosition = transform.position + Vector3.down;

            if (CanMove(newPosition))
            {
                transform.position = newPosition;
                PlayMoveSound();
            } else
            {
                freezed = true;

                AddChildsToMatrix();

                CheckLine();

                if (transform.position.y >= GameManager.Main.spawnPosition.y)
                {
                    GameManager.Main.GameOver();
                } else
                {
                    GameManager.Main.SpawnTetromino();
                }
            }

            previousTime = Time.time;
        }

        SnapToGrid();
    }

    private void Move()
    {
        bool hasPressedKey = false;

        Vector3 newPosition = transform.position;

        if (Input.GetKeyDown(KeyCode.A))
        {
            newPosition += Vector3.left;
            hasPressedKey = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            newPosition += Vector3.right;
            hasPressedKey = true;
        }

        if (CanMove(newPosition) && hasPressedKey)
        {
            transform.position = newPosition;
            PlayMoveSound();
        }
    }

    private bool CanMove(Vector3 newPosition)
    {
        foreach (Transform child in transform)
        {
            Vector3 newBlockPosition = child.localPosition + newPosition;

            int newPositionX = Mathf.RoundToInt(newBlockPosition.x);
            int newPositionY = Mathf.RoundToInt(newBlockPosition.y);

            if (newPositionX > 9 || newPositionX < 0 || newPositionY < 0)
            {
                return false;
            }

            if (tilesMatrix[newPositionX, newPositionY] != null)
            {
                return false;
            }
        }

        return true;
    }

    private void Rotate()
    {
        bool hasPressedKey = false;

        Quaternion newRotation = transform.rotation;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            newRotation = Quaternion.Euler(0, 0, 90);
            hasPressedKey = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            newRotation = Quaternion.Euler(0, 0, -90);
            hasPressedKey = true;
        }


        if (CanRotate(newRotation) && hasPressedKey == true)
        {
            foreach (Transform child in transform)
            {
                Vector3 newBlockPosition = newRotation * child.localPosition;
                child.transform.localPosition = newBlockPosition;
            }
            PlayRotateSound();
        }
    }

    private bool CanRotate(Quaternion newRotation)
    {
        foreach (Transform child in transform)
        {
            Vector3 newBlockPosition = (newRotation * child.localPosition) + transform.position;

            int newPositionX = Mathf.RoundToInt(newBlockPosition.x);
            int newPositionY = Mathf.RoundToInt(newBlockPosition.y);

            if (newPositionX > 9 || newPositionX < 0 || newPositionY < 0)
            {
                return false;
            }

            if (tilesMatrix[newPositionX, newPositionY] != null)
            {
                return false;
            }
        }

        return true;
    }

    private void AddChildsToMatrix()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            int positionX = Mathf.RoundToInt(child.position.x);
            int positionY = Mathf.RoundToInt(child.position.y);

            tilesMatrix[positionX, positionY] = child;
        }
    }

    private void CheckLine()
    {
        int currentSequence = 0;
        for (int i = 0; i < tileRows; i++)
        {
            bool isComplete = true;
            for (int j = 0; j < tileCollumns; j++)
            {
                if (tilesMatrix[j, i] == null)
                {
                    isComplete = false;
                    break;
                }
            }
            if (isComplete)
            {
                ClearLine(i);
                RowDown(i);

                currentSequence++;
                i--;
            }
        }

        if (currentSequence > 0)
        {
            Pontuate(currentSequence);
            PlayClearSound();
        }

    }

    private void ClearLine(int line)
    {
        for (int col = 0; col < tileCollumns; col++)
        {
            GameObject tileObject = tilesMatrix[col, line].gameObject;

            Destroy(tileObject);

            tilesMatrix[col, line] = null;
        }
    }

    private void RowDown(int line)
    {
        for (int i = line + 1; i < tileRows - 1; i++)
        {
            for (int j = 0; j < tileCollumns; j++)
            {
                Debug.Log("i = " + i + " j = " + j);

                Transform t = tilesMatrix[j, i];
                if (t != null)
                {
                    tilesMatrix[j, i-1] = t;
                    tilesMatrix[j, i] = null;
                    tilesMatrix[j, i-1].transform.position += Vector3.down;
                }
            }
        }
    }

    void SnapToGrid()
    {
        int newPositionX = Mathf.RoundToInt(transform.position.x);
        int newPositionY = Mathf.RoundToInt(transform.position.y);

        transform.position = new Vector3(newPositionX, newPositionY, 0);
    }

    void PlayRotateSound()
    {
        source.clip = rotateSound;
        source.Play();
    }

    void PlayClearSound()
    {
        source.clip = clearSound;
        source.Play();
    }

    void PlayMoveSound()
    {
        source.clip = moveSound;
        source.Play();
    }

    void Pontuate(int sequence)
    {
        int totalPoints = 0;
        switch(sequence)
        {
            case 1: totalPoints = 40; break;
            case 2: totalPoints = 100; break;
            case 3: totalPoints = 300; break;
            case 4: totalPoints = 400; break;
        }

        GameManager.Main.AddScore(totalPoints);
    }
}
