using System;
using System.Collections;
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

    public Tile[,] allTiles;
    public Hex[,] allHexes;
    private List<Joint> allJoints;
    private Joint selectedJoint;
    private Vector3 offset = new Vector3(0.2f, 0, 0);
    private bool matchMade;

    void Start()
    {
        matchMade = false;
        allTiles = new Tile[width, height];
        allHexes = new Hex[width, height];
        allJoints = new List<Joint>();
        SetupCamera();
        SetupBoard();
        SetupJoints();
        SetupHexes();
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
        joint.name = "Joint (" + tile.xIndex + "," + tile.yIndex + "-" + jType + ")";
        joint.transform.parent = transform.Find("Joints");

        Joint jointComp = joint.GetComponent<Joint>();
        allJoints.Add(jointComp);

        List<Tile> pieces = AddTilesToJoint(tile, isRightJoint);

        jointComp.Init(pieces, jType, this);
    }

    private List<Tile> AddTilesToJoint(Tile tile, bool isRightJoint)
    {
        List<Tile> pieces = new List<Tile>();
        pieces.Add(tile);

        if(!isRightJoint)
        {
            if(tile.xIndex % 2 == 0)
            {
                pieces.Add(allTiles[tile.xIndex - 1, tile.yIndex + 1]);
                pieces.Add(allTiles[tile.xIndex, tile.yIndex + 1]);
            }
            else
            {
                pieces.Add(allTiles[tile.xIndex - 1, tile.yIndex]);
                pieces.Add(allTiles[tile.xIndex, tile.yIndex + 1]);
            }
        }
        else
        {
            if(tile.xIndex % 2 == 0)
            {
                pieces.Add(allTiles[tile.xIndex, tile.yIndex + 1]);
                pieces.Add(allTiles[tile.xIndex + 1, tile.yIndex + 1]);
            }
            else
            {
                pieces.Add(allTiles[tile.xIndex, tile.yIndex + 1]);
                pieces.Add(allTiles[tile.xIndex + 1, tile.yIndex]);
            }
        }
        return pieces;
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
                Hex newHex = PlaceHexAt(x, y);
                while(FindMatches() != null)
                {
                    print("Found match, reshuffling");
                    Destroy(newHex.gameObject);
                    newHex = PlaceHexAt(x, y);
                }
            }
        }
    }

    private Hex PlaceHexAt(int x, int y)
    {
        Tile tile = allTiles[x, y];
        int rand = UnityEngine.Random.Range(0, hexPrefabs.Length);
        GameObject hex = Instantiate(hexPrefabs[rand], tile.transform.position, Quaternion.identity);
        hex.transform.parent = transform.Find("Hexes");
        hex.name = "Hex(" + x + "," + y + ")";
        hex.GetComponent<Hex>().Init(x, y, this);
        tile.currentHex = hex.GetComponent<Hex>();
        allHexes[x, y] = hex.GetComponent<Hex>();
        return hex.GetComponent<Hex>();
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

    internal void RotateClockwise()
    {
        Tile tile0 = selectedJoint.jointTiles[0];
        Hex hex0 = allHexes[tile0.xIndex, tile0.yIndex];
        Tile tile1 = selectedJoint.jointTiles[1];
        Hex hex1 = allHexes[tile1.xIndex, tile1.yIndex];
        Tile tile2 = selectedJoint.jointTiles[2];
        Hex hex2 = allHexes[tile2.xIndex, tile2.yIndex];

        ShiftHex(hex0, tile1);
        ShiftHex(hex1, tile2);
        ShiftHex(hex2, tile0);
        StartCoroutine(ClearAndCollapseRoutine());

    }

    private void ClearMatch()
    {
        Joint matchJoint = FindMatches();
        if (matchJoint != null)
        {
            matchMade = true;
            print("Destroyed");
            foreach (Tile tile in matchJoint.jointTiles)
            {
                Destroy(allHexes[tile.xIndex, tile.yIndex].gameObject);
                allHexes[tile.xIndex, tile.yIndex] = null;
                Destroy(GameObject.FindGameObjectWithTag("Highlight"));
                selectedJoint.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            }
        }
    }

    internal void RotateCounterClockwise()
    {

        Tile tile0 = selectedJoint.jointTiles[0];
        Hex hex0 = allHexes[tile0.xIndex, tile0.yIndex];
        Tile tile1 = selectedJoint.jointTiles[1];
        Hex hex1 = allHexes[tile1.xIndex, tile1.yIndex];
        Tile tile2 = selectedJoint.jointTiles[2];
        Hex hex2 = allHexes[tile2.xIndex, tile2.yIndex];

        ShiftHex(hex0, tile2);
        ShiftHex(hex1, tile0);
        ShiftHex(hex2, tile1);
        StartCoroutine(ClearAndCollapseRoutine());

    }

    private void ShiftHex(Hex hex, Tile tile)
    {
        if(hex != null)
        {
            hex.transform.position = tile.transform.position;
            allHexes[tile.xIndex, tile.yIndex] = hex;
            hex.Init(tile.xIndex, tile.yIndex, this);
            tile.currentHex = hex;
        }
    }

    public Vector2 GetBoardBounds()
    {
        return new Vector2(width, height);
    }

    private Joint FindMatches()
    {
        foreach(Joint joint in allJoints)
        {
            if(FindMatchAtJoint(joint))
            {
                return joint;
            }
        }

        return null;
    }

    private bool FindMatchAtJoint(Joint joint)
    {
        Tile tile0 = joint.jointTiles[0];
        Tile tile1 = joint.jointTiles[1];
        Tile tile2 = joint.jointTiles[2];

        if(allHexes[tile1.xIndex, tile1.yIndex] != null && allHexes[tile2.xIndex, tile2.yIndex] != null)
        {
            return tile0.currentHex.hexType == tile1.currentHex.hexType && tile1.currentHex.hexType == tile2.currentHex.hexType;
        }
        else
        {
            return false;
        }
        
    }

    public IEnumerator RotateClockwiseAndClearRoutine()
    {
        yield return null;
        RotateClockwise();
        if (!matchMade)
        {
            yield return new WaitForSeconds(0.5f);
            RotateClockwise();
            if (!matchMade)
            {
                yield return new WaitForSeconds(0.5f);
                RotateClockwise();
            }
        }

        matchMade = false;
    }

    public IEnumerator RotateCounterAndClearRoutine()
    {
        yield return null;
        RotateCounterClockwise();
        if (!matchMade)
        {
            yield return new WaitForSeconds(0.5f);
            RotateCounterClockwise();
            if (!matchMade)
            {
                yield return new WaitForSeconds(0.5f);
                RotateCounterClockwise();
            }
        }

        matchMade = false;
    }

    public void PlaceHex(Hex hex, Vector3 target)
    {
        hex.transform.position = target;
        hex.transform.rotation = Quaternion.identity;
    }

    public void CollapseAllColumns()
    {
        for (int i = 0; i < width; i++)
        {
            CollapseColumn(i, 0.2f);
        }
    }

    private void CollapseColumn(int column, float collapseTime)
    {

        for (int i = 0; i < height; i++)
        {
            if (allHexes[column, i] == null)
            {
                print(allTiles[column, i].name);
                for (int j = i + 1; j < height; j++)
                {
                    if (allHexes[column, j] != null)
                    {
                        allHexes[column, j].MoveHex(allTiles[column,i].transform.position.x, allTiles[column, i].transform.position.y, collapseTime * (j - i));
                        allHexes[column, i] = allHexes[column, j];
                        allHexes[column, i].Init(column, i, this);
                        allTiles[column, i].currentHex = allHexes[column, i];
                        allHexes[column, j] = null;
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator ClearAndCollapseRoutine()
    {
        while(FindMatches() != null)
        {
            ClearMatch();
            yield return new WaitForSeconds(0.5f);
            CollapseAllColumns();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
