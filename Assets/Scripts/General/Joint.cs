using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Joints have two types. The two types are different in the ways they look for matches and the points thay are allowed to be initialized at
public enum JointType
{
    right,
    left
}

[RequireComponent(typeof(SpriteRenderer))]
public class Joint : MonoBehaviour
{
    public List<Tile> jointTiles;           //Each joint has 3 tiles. This is utilized for switching Pieces inside the tiles and looking for matches
    private Board m_board;

    public JointType jointType;

    public void Init(List<Tile> tiles, JointType jType, Board board)        //Init method called from the Board class to keep the object updated
    {
        jointTiles = tiles;
        jointType = jType;
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
