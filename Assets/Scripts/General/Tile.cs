using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for the Tiles that inhabit Hexes. These objects are linked to Joints
[SelectionBase]
public class Tile : MonoBehaviour, IGridElement
{
    public int xIndex { get; set; }
    public int yIndex { get; set; }
    private Board m_board;
    public Hex currentHex;

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;
        m_board = board;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
