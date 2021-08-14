using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{

    private IEnumerator coroutine;

    private Tetromino[] tetrominos;
    private Block[,] matrix;

    private GameObject tilesPivot;

    public int numColumns;
    public int numRows;
    public GameObject blockPrefab;


    // Start is called before the first frame update
    void Start()
    {
        tilesPivot = new GameObject("Tiles Pivot");
        matrix     = new Block[numColumns, numRows];

        CreateBoundaryBlocks();

        // Update Tiles once per second
        coroutine = UpdateTilesRoutine(1.0f);
        StartCoroutine(coroutine);

    }

    void CreateBoundaryBlocks() 
    {
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                if (j == 0 || j == numRows - 1 || i == 0 || i == numColumns - 1)
                {
                    matrix[i, j] = new Block(new Vector2Int(i, j), Color.white);
                }
            }
        }
    }

    private IEnumerator UpdateTilesRoutine(float refreshTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(refreshTime);

            Debug.Log("Updating Tiles... ");

            UpdatePositions();
            UpdateTiles();

        }
    }

    void UpdatePositions()
    {

    }

    void ClearTiles()
    {
        foreach (Transform child in tilesPivot.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateTiles()
    {

        ClearTiles();

        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                Block block = matrix[i, j];

                if (block == null) continue;

                Vector3 blockPosition = new Vector3(block.position.x, block.position.y, 0);
                GameObject blockInstance = Instantiate(blockPrefab, blockPosition, Quaternion.identity);

                blockInstance.name = "Tile";

                blockInstance.transform.SetParent(tilesPivot.transform);
            }
        }
    }
}   

