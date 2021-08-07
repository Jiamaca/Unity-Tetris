using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{

    private Tetromino[] tetrominos;
    private Block[,] matrix;
    private List<Block> fixedBlocks;

    public int numColumns;
    public int numRows;
    public GameObject blockPrefab;

    // Start is called before the first frame update
    void Start(){
        matrix = new Block[numRows, numColumns];
        fixedBlocks = new List<Block>();
        CreateBoundaryBlocks();

    }

    // Update is called once per frame
    void Update() {
        UpdatePositions();
        DrawMatrix();
    }

    void UpdatePositions(){

    }

    void DrawMatrix(){
        foreach(var item in fixedBlocks){
            matrix[item.position.x, item.position.y] = item;
        }

        for (int i = 0; i < numColumns; i++){
            for (int j = 0; j < numRows; j++){
                if (j == 0 || j == numRows - 1 || i == 0|| i == numColumns - 1){
                    Block block = matrix[j,i];
                    GameObject instance = Instantiate(blockPrefab, 
                                                      new Vector3(block.position.x,block.position.y,0),
                                                      Quaternion.identity);
                }
            }
        }
        
    }    

    void CreateBoundaryBlocks(){
        for (int i = 0; i < numColumns ; i++){
            for (int j = 0; j < numRows ; j++){
                if (j == 0 || j == numRows - 1 || i == 0||i == numColumns - 1){
                    fixedBlocks.Add(new Block(new Vector2Int(j,i), Color.white));
                }
            }
        }
    }
}   

