using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] private int width = 9;
    [SerializeField] private int height = 8;
    [SerializeField] [Tooltip("In Unity units")] private float borderSize = 1;


    [Header("Prefabs")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject jointPrefab;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private GameObject[] hexPrefabs;
    [SerializeField] private GameObject[] bombPrefabs;
    

    public Tile[,] allTiles;
    public Hex[,] allHexes;
    private List<Joint> allJoints;
    private Joint selectedJoint;
    private Vector3 offset = new Vector3(0.2f, 0, 0);
    private int speed = 480;
    private bool matchMade;
    private bool inputBlocked;
    private bool bombSpawn;
    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        matchMade = false;
        inputBlocked = false;
        bombSpawn = false;
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

    //Setup joints at appropriate tile intersections
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

    //Each joint will have 3 tiles associated with it.
    private void SetupJointAt(Tile tile, bool isRightJoint)
    {
        int rInt = isRightJoint ? 1 : -1;
        JointType jType = isRightJoint ? JointType.right : JointType.left;

        GameObject joint = Instantiate(jointPrefab, tile.transform.position + new Vector3(rInt * 0.5f, 0.86f, -1f), Quaternion.identity) as GameObject;
        joint.name = "Joint (" + tile.xIndex + "," + tile.yIndex + "-" + jType + ")";
        joint.transform.parent = transform.Find("Joints");
        joint.GetComponent<SpriteRenderer>().enabled = false;

        Joint jointComp = joint.GetComponent<Joint>();
        allJoints.Add(jointComp);

        List<Tile> pieces = AddTilesToJoint(tile, isRightJoint);

        jointComp.Init(pieces, jType, this);
    }

    //The adding of tiles differs according to the joint type
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

    //The orthographic size and position components are adjusted according to board dimensions
    private void SetupCamera()
    {
        float aspectRatio = ((float)Screen.width / (float)Screen.height);
        float horizontalSize = ((width)) / aspectRatio;
        float verticalSize = (height * 1.33f);
        Camera.main.orthographicSize = Mathf.Max(horizontalSize, verticalSize);
        Camera.main.transform.position = new Vector3((float)(width / 1.5f), (float)(height / 1.5f), -10f);
    }


    private void SetupBoard()
    {
        for (int y = 0; y < height; y++)
        {
            SetupRow(y);
        }
    }

    //Position, name and child the tiles appropriately for one row
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

    //Fill in hexes at tiles that do not have hexes associated with them, drop them from a Y offset once Initialized
    private void SetupHexes(int offset = 5)
    {
        if(gameManager.gameState != GameState.Running)
        {
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(allTiles[x, y].currentHex == null)
                {
                    Hex newHex;
                    if (!bombSpawn)
                    {
                        newHex = PlaceHexAt(x, y, offset);
                        int iters = 0;
                        while (FindMatches() != null && iters < 20)
                        {
                            iters++;
                            Destroy(newHex.gameObject);
                            newHex = PlaceHexAt(x, y);
                        }
                    }
                    else
                    {
                        bombSpawn = false;
                        newHex = PlaceHexAt(x, y, offset, true);
                    }
                    

                    if (offset != 0)
                    {
                        newHex.MoveHex(allTiles[x, y].transform.position.x, allTiles[x, y].transform.position.y, 0.15f);
                    }
                }
               
            }
        }
    }

    //Creates and initializes a random hex from the given options. Will create a regular hex or a bomb according to specification.
    private Hex PlaceHexAt(int x, int y, int offset = 5, bool bomb = false)
    {
        Tile tile = allTiles[x, y];
        int rand = UnityEngine.Random.Range(0, hexPrefabs.Length);

        GameObject hex;
        if (!bomb)
        {
            hex = Instantiate(hexPrefabs[rand], tile.transform.position + new Vector3(0, offset, 0), Quaternion.identity);
        }
        else
        {
            hex = Instantiate(bombPrefabs[rand], tile.transform.position + new Vector3(0, offset, 0), Quaternion.identity); 
        }

        hex.transform.parent = transform.Find("Hexes");
        hex.name = "Hex(" + x + "," + y + ")";
        hex.GetComponent<Hex>().Init(x, y, this);
        tile.currentHex = hex.GetComponent<Hex>();
        allHexes[x, y] = hex.GetComponent<Hex>();
        return hex.GetComponent<Hex>();
    }

    //Called by the InputManager and selects the closest Joint object to the player Touch
    public Joint FindAndSelectNearestJoint(Vector3 pos)
    {
        if (!inputBlocked)
        {
            if (selectedJoint != null)
            {
                selectedJoint.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                var highLight = GameObject.FindGameObjectWithTag("Highlight");
                DestroyImmediate(highLight);
            }
            Vector2 twoDimPos = pos;
            Joint nearestJoint = null;
            float minDist = Mathf.Infinity;

            foreach (Joint joint in allJoints)
            {
                Vector2 jointPos = joint.transform.position;
                float newDist = Vector2.Distance(twoDimPos, jointPos);
                if (newDist < minDist)
                {
                    minDist = newDist;
                    nearestJoint = joint;
                }
            }

            if (nearestJoint != null)
            {
                SpriteRenderer jointSprite = nearestJoint.gameObject.GetComponent<SpriteRenderer>();
                jointSprite.enabled = true;
                selectedJoint = nearestJoint;
                if (nearestJoint.jointType == JointType.left)
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

        return null;
        
    }

    //Rotates all the hexes of the currently selected joint clockwise and checks for matches afterwards 
    internal void RotateClockwise()
    {
        if(selectedJoint != null)
        {
            Tile tile0 = selectedJoint.jointTiles[0];
            Hex hex0 = allHexes[tile0.xIndex, tile0.yIndex];
            Tile tile1 = selectedJoint.jointTiles[1];
            Hex hex1 = allHexes[tile1.xIndex, tile1.yIndex];
            Tile tile2 = selectedJoint.jointTiles[2];
            Hex hex2 = allHexes[tile2.xIndex, tile2.yIndex];

            StartCoroutine(ShiftHex(hex0, tile1, -1));
            StartCoroutine(ShiftHex(hex1, tile2, -1));
            StartCoroutine(ShiftHex(hex2, tile0, -1));
            StartCoroutine(ClearCollapseAndRefillRoutine());
        }
    }

    //Rotates all the hexes of the currently selected joint counter-clockwise and checks for matches afterwards 
    internal void RotateCounterClockwise()
    {
        if(selectedJoint != null)
        {
            Tile tile0 = selectedJoint.jointTiles[0];
            Hex hex0 = allHexes[tile0.xIndex, tile0.yIndex];
            Tile tile1 = selectedJoint.jointTiles[1];
            Hex hex1 = allHexes[tile1.xIndex, tile1.yIndex];
            Tile tile2 = selectedJoint.jointTiles[2];
            Hex hex2 = allHexes[tile2.xIndex, tile2.yIndex];

            StartCoroutine(ShiftHex(hex0, tile2, 1));
            StartCoroutine(ShiftHex(hex1, tile0, 1));
            StartCoroutine(ShiftHex(hex2, tile1, 1));
            StartCoroutine(ClearCollapseAndRefillRoutine());
        }
    }

    //Destroys the matchşng pieces, clears the selection highlight and increases the score accordingly
    private void ClearMatch()
    {
        Joint matchJoint = FindMatches();
        if (matchJoint != null)
        {
            matchMade = true;
            AudioManager.Instance.PlayScoreSFX();
            GameObject particle = Instantiate(particlePrefab, matchJoint.transform.position, Quaternion.Euler(0, 180, 0));
            particle.GetComponent<ParticleSystem>().startColor = allHexes[matchJoint.jointTiles[0].xIndex, matchJoint.jointTiles[0].yIndex].GetComponent<SpriteRenderer>().color;
            if (selectedJoint != null)
            {
                selectedJoint.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            }
            selectedJoint = null;
            Destroy(GameObject.FindGameObjectWithTag("Highlight"));

            foreach (Tile tile in matchJoint.jointTiles)
            {
                Destroy(allHexes[tile.xIndex, tile.yIndex].gameObject);
                allHexes[tile.xIndex, tile.yIndex] = null;
                gameManager.IncreaseScore();
            }
        }
    }

    //Rotates and initializes the positions of all the Hexes that surround a joint
    private IEnumerator ShiftHex(Hex hex, Tile tile, int dir)
    {
        if(hex != null)
        {
            yield return StartCoroutine(Rotate120(hex.gameObject, selectedJoint, tile, dir));
            allHexes[tile.xIndex, tile.yIndex] = hex;
            hex.Init(tile.xIndex, tile.yIndex, this);
            tile.currentHex = hex;
        }
    }

    //Helper function that returns width and height
    public Vector2 GetBoardBounds()
    {
        return new Vector2(width, height);
    }

    //Looks for potential matches on the board by looking through the joints
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

    //Checks for a match condition on a particular joint
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

    //Attempts to rotate a joint clockwise 3 times unless interrupted by a match condition, calls to clear, collapse and refill in the case of a match
    public IEnumerator RotateClockwiseAndClearRoutine()
    {
        if (!inputBlocked)
        {
            inputBlocked = true;
            yield return null;
            matchMade = false;
            int cycles = 0;
            while (!matchMade && cycles < 3)
            {
                cycles++;
                RotateClockwise();
                yield return new WaitForSeconds(0.5f);
            };


            if (matchMade)
            {
                gameManager.IncreaseMoves();
                Bomb bomb = FindObjectOfType<Bomb>();
                if(bomb!= null)
                {
                    bomb.DecreaseTimer();
                }
            }
            matchMade = false;
            inputBlocked = false;
        }
        
    }

    //Attempts to rotate a joint counter-clockwise 3 times unless interrupted by a match condition, calls to clear, collapse and refill in the case of a match
    public IEnumerator RotateCounterAndClearRoutine()
    {
        if (!inputBlocked)
        {
            yield return null;
            inputBlocked = true;
            matchMade = false;
            int cycles = 0;
            do
            {
                cycles++;
                RotateCounterClockwise();
                yield return new WaitForSeconds(0.5f);
            }
            while (!matchMade && cycles < 3);

            if (matchMade)
            {
                gameManager.IncreaseMoves();
                Bomb bomb = FindObjectOfType<Bomb>();
                if (bomb != null)
                {
                    bomb.DecreaseTimer();
                }
            }
            matchMade = false;
            inputBlocked = false;
        }
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

    //The gaps in a column are filled by bringing down hexes at the top
    private void CollapseColumn(int column, float collapseTime)
    {

        for (int i = 0; i < height; i++)
        {
            if (allHexes[column, i] == null)
            {
                for (int j = i + 1; j < height; j++)
                {
                    if (allHexes[column, j] != null)
                    {
                        allHexes[column, j].MoveHex(allTiles[column,i].transform.position.x, allTiles[column, i].transform.position.y, collapseTime * (j - i));
                        allHexes[column, i] = allHexes[column, j];
                        allHexes[column, i].Init(column, i, this);
                        allTiles[column, i].currentHex = allHexes[column, i];
                        allHexes[column, j] = null;
                        allTiles[column, j].currentHex = null;
                        break;
                    }
                }
            }
        }
    }

    //Coroutine that calls to destroy matches and then calls to collapse appropriate columns
    private IEnumerator ClearAndCollapseRoutine()
    {
        int num_iters = 0;
        while(FindMatches() != null && num_iters < 10)
        {
            matchMade = true;
            ClearMatch();
            yield return new WaitForSeconds(0.2f);
            CollapseAllColumns();
            yield return new WaitForSeconds(0.3f);
            num_iters++;
            if (num_iters == 10)
            {
                print("I am stuck!");
            }
        }
    }

    //Coroutine that waits for clear and collapses to be finished, then calls to instantiate new Hexes
    private IEnumerator ClearCollapseAndRefillRoutine()
    {
        bombSpawn = false;
        yield return StartCoroutine(ClearAndCollapseRoutine());
        yield return new WaitForSeconds(0.25f);
        SetupHexes(5);
    }


    internal void SpawnBomb()
    {
        bombSpawn = true;
    }

    //Blows up all the active Hexes. Called when the game is lost
    internal void ClearBoard()
    {
        GameObject[] hexes = GameObject.FindGameObjectsWithTag("Hex");
        foreach(GameObject hex in hexes)
        {
            GameObject particle = Instantiate(particlePrefab, hex.transform.position, Quaternion.Euler(0, 180, 0));
            ParticleSystem.MainModule main = particle.GetComponent<ParticleSystem>().main;
            main.startColor = hex.GetComponent<SpriteRenderer>().color;
            Destroy(hex);
            AudioManager.Instance.PlayBoomSFX();
        }
    }

    //Simple turn animation for Hexes using RotateAround
    public IEnumerator Rotate120(GameObject obj, Joint joint, Tile tile, int dir)
    {
        float timer = 0;
        while (timer < 0.25f && !matchMade)
        {
            obj.transform.RotateAround(joint.transform.position, new Vector3(0, 0, dir), speed * Time.deltaTime);
            yield return null;
            timer += Time.deltaTime;
        }

        if(obj!= null)
        {
            obj.transform.position = tile.transform.position;
            obj.transform.rotation = Quaternion.identity;
        }

    }
}
