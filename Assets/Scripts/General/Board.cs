using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int width = 9;
    [SerializeField] private int height = 8;
    [SerializeField] private float borderSize = 1;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject jointPrefab;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private GameObject[] hexPrefabs;

    private Tile[,] allTiles;
    private Hex[,] allHexes;
    private List<Joint> allJoints;
    private Joint selectedJoint;
    private Vector3 offset = new Vector3(0.2f, 0, 0);

    void Start()
    {
        allTiles = new Tile[width, height];
        allHexes = new Hex[width, height];
        allJoints = new List<Joint>();
        SetupCamera();
        SetupBoard();
        SetupHexes();
        SetupJoints();
    }

    void Update()
    {
        
    }

    private void SetupJoints()
    {
        foreach (Tile tile in allTiles)
        {
            if(tile.yIndex < height - 1)
            {
                if(tile.xIndex < width - 1)
                {
                    SetupJointAt(tile, true);
                }
                if (tile.xIndex > 0)
                {
                    SetupJointAt(tile, false);
                }
            }
           
        }
    }

    private void SetupJointAt(Tile tile, bool isRightJoint)
    {
        int rInt = isRightJoint ? 1 : -1;
        JointType jType = isRightJoint ? JointType.right : JointType.left;
        GameObject joint = Instantiate(jointPrefab, tile.transform.position + new Vector3(rInt * 0.5f, 0.86f, -1f), Quaternion.identity) as GameObject;
        joint.transform.parent = transform.Find("Joints");
        Joint jointComp = joint.GetComponent<Joint>();
        allJoints.Add(jointComp);
        List<Tile> pieces = new List<Tile>();
        pieces.Add(tile);
        pieces.Add(allTiles[tile.xIndex, tile.yIndex + 1]);
        pieces.Add(allTiles[tile.xIndex + rInt, tile.yIndex + 1]);
        jointComp.Init(pieces, jType);
    }

    private void SetupCamera()
    {
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        Camera.main.orthographicSize /= aspectRatio;
    }

    private void SetupBoard()
    {
        for (int y = 0; y < height; y++)
        {
            SetupRow(y);
        }
    }

    private void SetupRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            GameObject tile;
            float xCoord = x * 1.5f;
            float yCoord = y * 1.72f;
            if ((x % 2) == 0)
            {
                tile = Instantiate(tilePrefab, new Vector3(xCoord, yCoord, 0), Quaternion.identity); 
            }
            else
            {
                tile = Instantiate(tilePrefab, new Vector3(xCoord, yCoord - 0.866f, 0), Quaternion.identity);
            }

            tile.transform.parent = transform.Find("Tiles");
            tile.name = "Tile(" + x + "," + y + ")";
            tile.GetComponent<Tile>().Init(x, y, this);
            allTiles[x, y] = tile.GetComponent<Tile>();
        }
    }

    private void SetupHexes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = allTiles[x, y];
                int rand = UnityEngine.Random.Range(0, hexPrefabs.Length);
                GameObject hex = Instantiate(hexPrefabs[rand], tile.transform.position, Quaternion.identity);
                hex.transform.parent = transform.Find("Hexes");
                hex.name = "Hex(" + x + "," + y + ")";
                hex.GetComponent<Hex>().Init(x, y, this);
                allHexes[x, y] = hex.GetComponent<Hex>();
            }
        }
    }

    public Joint FindAndSelectNearestJoint(Vector3 pos)
    {
        if(selectedJoint != null)
        {
            selectedJoint.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            var highLight = GameObject.FindGameObjectWithTag("Highlight");
            DestroyImmediate(highLight);
        }
        Vector2 twoDimPos = pos;
        Joint nearestJoint = null;
        float minDist = Mathf.Infinity;

        foreach(Joint joint in allJoints)
        {
            Vector2 jointPos = joint.transform.position;
            float newDist = Vector2.Distance(twoDimPos, jointPos);
            if(newDist < minDist)
            {
                minDist = newDist;
                nearestJoint = joint;
            }
        }

        if(nearestJoint != null)
        {
            SpriteRenderer jointColor = nearestJoint.gameObject.GetComponent<SpriteRenderer>();
            jointColor.color = new Color(255, 255, 255, 255);
            selectedJoint = nearestJoint;
            if(nearestJoint.jointType == JointType.left)
            {
                GameObject jointHighlight = Instantiate(highlightPrefab, selectedJoint.transform.position - offset, Quaternion.identity);
                jointHighlight.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GameObject jointHighlight = Instantiate(highlightPrefab, selectedJoint.transform.position + offset, Quaternion.identity);
            }

            return nearestJoint;
        }

        else
        {
            throw new ApplicationException();
        }
    }
}
