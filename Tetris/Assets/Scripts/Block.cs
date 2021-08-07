using UnityEngine;

public class Block
{

    public Vector2Int position {get; set;}
    public Color color {get; set;}

    public Block(Vector2Int _position, Color _color){
        position = _position;
        color = _color;
    }
}
