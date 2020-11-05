using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JointType
{
    right,
    left
}

public class Joint : MonoBehaviour
{
    private List<Tile> jointTiles;

    public JointType jointType;

    public void Init(List<Tile> tiles, JointType jType)
    {
        jointTiles = tiles;
        jointType = jType;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotate()
    {
        print("Rotating");
    }
}
