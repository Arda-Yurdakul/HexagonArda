using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JointType
{
    right,
    left
}

[RequireComponent(typeof(SpriteRenderer))]
public class Joint : MonoBehaviour
{
    public List<Tile> jointTiles;
    private Board m_board;

    public JointType jointType;

    public void Init(List<Tile> tiles, JointType jType, Board board)
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
