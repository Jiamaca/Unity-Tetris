using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tetromino : MonoBehaviour 
{
    public Vector3 massCenter;

    public float moveInterval = 0.25f;
    public float fallTime = 0.8f;

    //public PlayerInput playerInput;

    public AudioClip rotateSound;
    public AudioClip moveSound;
    public AudioClip clearSound;

    public static int tileRows = 21;
    public static int tileCollumns = 10;

    private static Transform[,] tilesMatrix = new Transform[tileCollumns, tileRows];

    private bool freezed = false;
    private bool hasMoved = false;

    private float previousTime = 0;

    private GameObject tilesGroup;

    private GameManager gameManager = GameManager.Main;
    PlayerActions actions;

    private Coroutine delayedMoveRoutine;

    void Start()
    {
        tilesGroup = GameObject.Find("/Blocks");
    }

    private void OnEnable()
    {
        actions = new PlayerActions();
        actions.Enable();
        actions.PlayerController.Acelerate.performed += OnMovement;
    }

    private void OnDisable()
    {
        actions.PlayerController.Acelerate.performed -= OnMovement;
        actions.Disable();
    }

    public void Update()
    {
        if (freezed) return;

        Rotate();

        Move();

        if (Time.time - previousTime > (InputHandler.inputAxis.y < 0 ? fallTime / 10 : fallTime))
        {
            Vector3 newPosition = transform.position + Vector3.down;

            if (CanMove(newPosition))
            {
                transform.position = newPosition;
                gameManager.PlayAudioClip(moveSound);
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

    public void OnDrawGizmos()
    {
        Gizmos.DrawIcon(massCenter + transform.position, "Light Gizmo.tiff");
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        float inputMovement = value.ReadValue<float>();
    }

    private void Move()
    {
        float input = InputHandler.inputAxis.x;
        bool hasPressedKey = input != 0;
        Vector3 newPosition = transform.position;

        if (hasPressedKey && !hasMoved)
        {
            hasMoved = true;
            newPosition += Vector3.right * input;
            delayedMoveRoutine = StartCoroutine(DelayedMove(moveInterval));

            if (CanMove(newPosition))
            {
                transform.position = newPosition;
                gameManager.PlayAudioClip(moveSound);
            }
        }

        if (input == 0) {
            hasMoved = false;

            if (delayedMoveRoutine != null)
            {
                StopCoroutine(delayedMoveRoutine);
            }
        }
    }

    private IEnumerator DelayedMove(float waitTime)
    {
        while (!freezed)
        {
            yield return new WaitForSeconds(waitTime);

            float input = InputHandler.inputAxis.x;
            bool hasPressedKey = input != 0;
            Vector3 newPosition = transform.position;

            newPosition += Vector3.right * input;

            if (hasPressedKey && CanMove(newPosition))
            {
                transform.position = newPosition;
                gameManager.PlayAudioClip(moveSound);
            }
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

        if (Input.GetKeyDown(KeyCode.Q) || InputHandler.rotateDirection < 0)
        {
            InputHandler.rotateDirection = 0;
            newRotation = Quaternion.Euler(0, 0, 90);
            hasPressedKey = true;
        }

        if (Input.GetKeyDown(KeyCode.E) || InputHandler.rotateDirection > 0)
        {
            InputHandler.rotateDirection = 0;
            newRotation = Quaternion.Euler(0, 0, -90);
            hasPressedKey = true;
        }


        if (hasPressedKey && CanRotate(newRotation))
        {
            foreach (Transform child in transform)
            {
                child.localPosition -= massCenter;
                Vector3 newBlockPosition = newRotation * child.localPosition;
                child.localPosition = newBlockPosition + massCenter;

                int newPositionX = Mathf.RoundToInt(child.localPosition.x);
                int newPositionY = Mathf.RoundToInt(child.localPosition.y);

                child.localPosition = new Vector3(newPositionX, newPositionY, 0);
            }
            gameManager.PlayAudioClip(rotateSound);
        }
    }

    private bool CanRotate(Quaternion newRotation)
    {
        foreach (Transform child in transform)
        {
            Vector3 newBlockPosition = newRotation * (child.localPosition - massCenter) + transform.position;

            int newPositionX = Mathf.RoundToInt(newBlockPosition.x + massCenter.x);
            int newPositionY = Mathf.RoundToInt(newBlockPosition.y + massCenter.y);

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
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            int positionX = Mathf.RoundToInt(child.position.x);
            int positionY = Mathf.RoundToInt(child.position.y);

            tilesMatrix[positionX, positionY] = child;
            child.parent = tilesGroup.transform;
        }

        Destroy(gameObject);
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
            gameManager.PlayAudioClip(clearSound);

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
