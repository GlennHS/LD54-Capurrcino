using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public float gridSize = 1;

    public static BoardManager instance;
    public List<List<GameObject>> underBoard;
    public List<List<GameObject>> board;
    public List<List<bool>> logicalBoard;

    public int width, height;

    [SerializeField]
    public Vector2 boardBottomLeft = Vector2.zero;

    [SerializeField]
    public List<GameObject> catominoPrefabs;
    [SerializeField]
    public List<GameObject> bonusPrefabs;
    [SerializeField]
    public List<GameObject> negativePrefabs;

    public GameObject audioManager;

    public GameObject emptyTilePrefab;
    public GameObject customerPrefab;

    [HideInInspector]
    public Image uiNextCatominoImage;

    private GameObject _nextCatomino;
    private GameObject _guideline;
    private List<GameObject> _instantiatedBoardObjects;
    private List<int[]> _customerLocations;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        // Variable Initialsation
        
        _instantiatedBoardObjects = new List<GameObject>();
        _customerLocations = new List<int[]>();
        InitialiseBoard();

        // Warnings conditions
        if (catominoPrefabs.Count == 0)
            Debug.LogWarning("ERROR: No catomino prefabs added in editor!");
        if(!emptyTilePrefab)
            Debug.LogWarning("ERROR: No empty tile prefab added in editor!");
    }

    private void Start()
    {
        GenerateUnderboard();
        AddCustomer();
        ChooseNextCatomino();

        _guideline = Instantiate(_nextCatomino, new Vector3(-100f, -100f, 0), Quaternion.identity);
        _guideline.GetComponent<SpriteRenderer>().sortingOrder = 5;
    }

    public void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.x = Mathf.Floor(pos.x + 0.5f);
        pos.y = Mathf.Floor(pos.y + 0.5f);

        pos.x += _nextCatomino.GetComponent<Catomino>().placementOffset.x;
        pos.y += _nextCatomino.GetComponent<Catomino>().placementOffset.y;

        // if(pos.x > boardBottomLeft.x &&  pos.y > boardBottomLeft.y)

        UpdateCatominoGuidePosition(pos);
    }

    public void DestroyBoard()
    {
        if (board == null || !board.Any()) return;

        InitialiseBoard();

        foreach (GameObject tile in _instantiatedBoardObjects)
        {
            Destroy(tile);
        }
    }

    // I'm aware implicitly filling the logical array with false may be redundant but better safe than sorry eh?
    public void InitialiseBoard()
    {
        board = new List<List<GameObject>>();
        logicalBoard = new List<List<bool>>();

        List<List<GameObject>> newBoard = new List<List<GameObject>>();
        List<List<bool>> newlBoard = new List<List<bool>>();
        for (int i = 0; i < height; i++)
        {
            List<GameObject> row = new List<GameObject>();
            List<bool> lRow = new List<bool>();
            for (int j = 0; j < width; j++)
            {
                row.Add(null);
                lRow.Add(false);
            }
            newBoard.Add(row);
            newlBoard.Add(lRow);
        }

        board = newBoard;
        logicalBoard = newlBoard;
    }

    public void VisualizeBoard()
    {
        foreach (GameObject tile in _instantiatedBoardObjects)
        {
            Destroy(tile);
        }

        _instantiatedBoardObjects.Clear();

        for (int rowNum = 0; rowNum < board.Count; rowNum++)
        {
            List<GameObject> row = board[rowNum];
            for (int colNum = 0; colNum < row.Count; colNum++)
            {
                GameObject tile = row[colNum];

                if (tile != null)
                {
                    InstantiateSingleGameObject(tile, colNum, rowNum);
                }
            }
        }
    }

    public void InstantiateSingleGameObject(GameObject tile, int _xPos, int _yPos)
    {
        float xPos = boardBottomLeft.x + _xPos;
        float yPos = boardBottomLeft.y + _yPos;

        if (tile.CompareTag("Catomino"))
        {
            xPos += tile.GetComponent<Catomino>().placementOffset.x;
            yPos += tile.GetComponent<Catomino>().placementOffset.y;
        }

        _instantiatedBoardObjects.Add(Instantiate(tile, new Vector2(xPos, yPos), Quaternion.identity));

        if (_instantiatedBoardObjects.Last().CompareTag("Catomino"))
            _instantiatedBoardObjects.Last().GetComponent<Catomino>().myBoardLocation = new int[] { _xPos, _yPos };
        else if (_instantiatedBoardObjects.Last().CompareTag("Customer"))
            _instantiatedBoardObjects.Last().GetComponent<Customer>().myBoardLocation = new int[] { _xPos, _yPos };

        board[_yPos][_xPos] = _instantiatedBoardObjects.Last();
    }
    
    public void VisualizeUnderboard()
    {
        for (int rowNum = 0; rowNum < underBoard.Count; rowNum++)
        {
            List<GameObject> row = underBoard[rowNum];
            for (int colNum = 0; colNum < row.Count; colNum++)
            {
                GameObject tile = row[colNum];
                float xPos = boardBottomLeft.x + colNum;
                float yPos = boardBottomLeft.y + rowNum;
                Instantiate(tile, new Vector2(xPos, yPos), Quaternion.identity);
            }
        }
    }

    public void GenerateUnderboard()
    {
        List<List<GameObject>> newBoard = new List<List<GameObject>>();
        for (int i = 0; i < height; i++)
        {
            List<GameObject> row = new List<GameObject>();
            for (int j = 0; j < width; j++)
            {
                row.Add(emptyTilePrefab);
            }
            newBoard.Add(row);
        }

        underBoard = newBoard;

        VisualizeUnderboard();
    }

    public void AddCustomer()
    {
        List<int[]> emptyPositions = new List<int[]>();

        for (int y = 0; y < logicalBoard.Count; y++)
        {
            List<bool> list = logicalBoard[y];
            for (int x = 0; x < list.Count; x++)
            {
                if(!list[x]) emptyPositions.Add(new int[] { x, y });
            }
        }

        int[] newPos = emptyPositions[Random.Range(0,emptyPositions.Count)];

        int xPos = newPos[0];
        int yPos = newPos[1];

        if(CheckTileEmpty(xPos, yPos))
        {
            Debug.Log($"Position of new customer: X: {xPos} Y: {yPos}");

            board[yPos][xPos] = customerPrefab;
            board[yPos][xPos].GetComponent<Customer>().turnsUntilLeaves = GameManager.instance.turnsCustomerWillWait;
            logicalBoard[yPos][xPos] = true;
            _customerLocations.Add(new int[] { xPos, yPos });

            VisualizeBoard();

            CheckCustomersSurrounded();
        }
    }

    public bool CheckTileEmpty(int x, int y)
    {
        try
        {
            return !logicalBoard[y][x];
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
            Debug.LogWarning($"Failed to check {x},{y}");
            return false;
        }
    }

    public void ChooseNextCatomino()
    {
        _nextCatomino = catominoPrefabs[Random.Range(0, catominoPrefabs.Count)];

        Debug.Log($"Next Catomino is {_nextCatomino.tag}");

        uiNextCatominoImage.sprite = _nextCatomino.GetComponent<SpriteRenderer>().sprite;

        Image uiImage = uiNextCatominoImage.GetComponent<Image>();
        SpriteRenderer spriteRenderer = _nextCatomino.GetComponent<SpriteRenderer>();

        uiImage.sprite = spriteRenderer.sprite;

        Vector2 originalSize = spriteRenderer.sprite.bounds.size;

        float newHeight = (originalSize.y / originalSize.x) * uiImage.rectTransform.rect.width;

        uiImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

        if (checkNoValidPlaces()) GameManager.instance.GameOver();
    }

    public void PlaceNextCatomino(int[] boardPos)
    {
        // Check we can even place here successfully
        if (!CheckTileEmpty(boardPos[0], boardPos[1])) return;

        List<ListWrapper> blockedPositions = _nextCatomino.GetComponent<Catomino>().relativeBlockPositions;

        bool validPos = true;
        if(blockedPositions.Count > 0 )
        {
            foreach (ListWrapper blockPos in blockedPositions)
            {
                List<int> bp = blockPos.myList;

                int xToCheck = boardPos[0] + bp[0];
                int yToCheck = boardPos[1] + bp[1];

                Debug.Log($"Checking tile at X:{xToCheck} Y:{yToCheck}");

                if(xToCheck < 0 || yToCheck < 0 || xToCheck >= width || yToCheck >= height)
                {
                    validPos = false;
                    break;
                }

                if (!CheckTileEmpty(xToCheck, yToCheck))
                {
                    Debug.LogWarning("Uh oh! Shouldn't place there!");
                    validPos = false;
                    break;
                }
            }
        }

        if (!validPos) return;

        board[boardPos[1]][boardPos[0]] = _nextCatomino;
        Debug.Log($"Dropping next piece at: {boardPos[0]},{boardPos[1]}");
        //board[boardPos[1]][boardPos[0]].GetComponent<Catomino>().myBoardLocation = new int[] { boardPos[0], boardPos[1] };

        logicalBoard[boardPos[1]][boardPos[0]] = true;

        if (blockedPositions.Count > 0)
        {
            foreach (ListWrapper blockPos in blockedPositions)
            {
                List<int> bp = blockPos.myList;

                int xToUpdate = boardPos[0] + bp[0];
                int yToUpdate = boardPos[1] + bp[1];

                logicalBoard[yToUpdate][xToUpdate] = true;
            }
        }

        GameManager.instance.turnsSinceCustomerLastPaid++;

        audioManager.GetComponent<AudioManager>().PlayMeow();

        GameManager.instance.IncreaeCatsPlaced();

        VisualizeBoard();

        CheckCustomersSurrounded();

        ReduceAllCustomersPatience();

        ChooseNextCatomino();

        if (GetNumberOfCustomersOnBoard() == 0)
        {
            AddCustomer();
            GameManager.instance.ResetCustomerWaitCounter();
        }
    }

    public void CheckCustomersSurrounded()
    {
        List<int[]> customersToRemove = new List<int[]>();
        if(_customerLocations.Count > 0)
        {
            foreach (int[] loc in _customerLocations)
            {
                //Debug.Log($"Checking customer at location X:{loc[0]} Y:{loc[1]}");

                List<int> xAugs = new List<int>() { -1, 0, 1};
                List<int> yAugs = new List<int>() { -1, 0, 1};

                bool isSurrounded = true;

                foreach(int xAug in xAugs)
                {
                    int xToCheck = loc[0] + xAug;
                    if (xToCheck >= 0 && xToCheck < width)
                    {
                        foreach (int yAug in yAugs)
                        {
                            int yToCheck = loc[1] + yAug;

                            if(yToCheck >= 0 && yToCheck < height)
                            {
                                if (!logicalBoard[yToCheck][xToCheck])
                                    isSurrounded = false;
                            }
                        }
                    }
                }

                if (isSurrounded)
                {
                    GameObject c = GetCustomerFromLocation(loc);
                    GameManager.instance.CustomerPaid(c.GetComponent<Customer>().turnsUntilLeaves);
                    board[loc[1]][loc[0]] = null;
                    logicalBoard[loc[1]][loc[0]] = false;
                    customersToRemove.Add(loc);
                }
            }

            foreach (int[] custLoc in customersToRemove)
            {
                // Get all surrounding squares
                List<int[]> surroundingSquares = new List<int[]>();

                List<int> xAugs = new List<int>() { -1, 0, 1 };
                List<int> yAugs = new List<int>() { -1, 0, 1 };

                foreach (int xAug in xAugs)
                {
                    int xToCheck = custLoc[0] + xAug;
                    if (xToCheck >= 0 && xToCheck < width)
                    {
                        foreach (int yAug in yAugs)
                        {
                            int yToCheck = custLoc[1] + yAug;

                            if (yToCheck >= 0 && yToCheck < height)
                            {
                                surroundingSquares.Add(new int[] { xToCheck, yToCheck });
                            }
                        }
                    }
                }

                Debug.LogWarning($"Customer removed. Checking these squares: {string.Join(" || ", surroundingSquares.Select(arr => string.Join(',', arr)))}");

                // Iterate over all catominos and check if they have a square within surroundingSquares
                // Efficiency is my passion...
                List<int[]> catominoLocationsToRemove = new List<int[]>();

                foreach(GameObject tile in _instantiatedBoardObjects)
                {
                    if (tile.CompareTag("Catomino"))
                    {
                        List<int[]> catominoSquares = tile.GetComponent<Catomino>().GetOverlappingSquares();
                        //Debug.Log($"catominoSquares = {string.Join(" || ", catominoSquares.Select(arr => string.Join(',', arr)))}");

                        bool match = false;
                        foreach (int[] a in catominoSquares)
                        {
                            if (match) break;

                            foreach (int[] b in surroundingSquares)
                            {
                                if (match) break;

                                //Debug.Log($"Checking if {a[0]},{a[1]} == {b[0]}{b[1]}");

                                if (a[0] == b[0] && a[1] == b[1])
                                {
                                    //Debug.Log($"Checking cat located at ({tile.GetComponent<Catomino>().myBoardLocation[0]},{tile.GetComponent<Catomino>().myBoardLocation[1]})");
                                    //Debug.Log("Cat detected, adding to list...");
                                    catominoLocationsToRemove.Add(tile.GetComponent<Catomino>().myBoardLocation);
                                    match = true;
                                }
                            }
                        }

                        //if (surroundingSquares.Intersect(catominoSquares).Count() > 0)
                        //{
                        //    Debug.Log("Cat detected, adding to list...");
                        //    catominoLocationsToRemove.Add(tile.GetComponent<Catomino>().myBoardLocation);
                        //}
                    }
                }

                foreach (int[] catLoc in catominoLocationsToRemove)
                {
                    //Debug.Log("Killing neko...");
                    RemoveCatomino(catLoc[0], catLoc[1]);
                }

                _customerLocations.Remove(custLoc);

                audioManager.GetComponent<AudioManager>().PlayCustomerPaid();
            }
        }

        VisualizeBoard();
    }

    public int GetNumberOfCustomersOnBoard()
    {
        return _customerLocations.Count;
    }

    public void RemoveCatomino(int xPos, int yPos)
    {
        Debug.Log($"Trying to remove catomino at ({xPos},{yPos})");

        if (board[yPos][xPos] == null) return;

        // Iterate over _iBO and find catomino with position matching x/y
        GameObject matchingCat = null;
        foreach (GameObject cat in _instantiatedBoardObjects)
        {
            if (cat.CompareTag("Catomino"))
            {
                int[] cLoc = cat.GetComponent<Catomino>().myBoardLocation;
                if (cLoc[0] == xPos && cLoc[1] == yPos)
                {
                    matchingCat = cat;
                    break;
                }
            }
        }

        if(matchingCat != null )
        {
            List<int[]> catominoSquares = matchingCat.GetComponent<Catomino>().GetOverlappingSquares();

            foreach (int[] square in catominoSquares)
            {
                logicalBoard[square[1]][square[0]] = false;
            }

            board[yPos][xPos] = null;
        }
    }

    public void ReduceAllCustomersPatience()
    {
        foreach (GameObject obj in _instantiatedBoardObjects)
        {
            if (obj.CompareTag("Customer"))
            {
                Debug.Log("Reducing patience...");
                obj.GetComponent<Customer>().LowerPatience();
            }
        }
    }

    public bool checkNoValidPlaces()
    {
        List<int[]> validDrops = new List<int[]>();

        for (int rowNum = 0; rowNum < logicalBoard.Count; rowNum++)
        {
            for (int colNum = 0; colNum < logicalBoard[rowNum].Count; colNum++)
            {
                if (!logicalBoard[rowNum][colNum]) validDrops.Add(new int[] { colNum, rowNum });
            }
        }

        if( validDrops.Count > 32 ) { return false; }

        Debug.LogWarning($"Checking these places: {string.Join(" || ", validDrops.Select(x => string.Join(",", x)))}");

        bool foundDropLoc = false;

        foreach (int[] dropLoc in validDrops)
        {
            List<int[]> overlappedSquares = _nextCatomino.GetComponent<Catomino>().GetOverlappingSquares(dropLoc);

            bool validDropLocation = true;
            Debug.Log($"Drop location being checked: ({dropLoc[0]},{dropLoc[1]})");
            foreach (int[] sq in overlappedSquares)
            {
                Debug.Log($"Checking overlap square ({sq[0]},{sq[1]})");
                if (sq[0] < 0 || sq[0] >= width || sq[1] < 0 || sq[1] >= height) validDropLocation = false;
                else if (logicalBoard[sq[1]][sq[0]]) validDropLocation = false;
            }

            if (validDropLocation)
            {
                Debug.LogWarning($"You can drop it at ({dropLoc[0]},{dropLoc[1]})");
                foundDropLoc = true;
                break;
            }
        }

        return !foundDropLoc;
    }

    public Vector2 BoardPositionToGOPosition(int x, int y)
    {
        float xPos = boardBottomLeft.x + x;
        float yPos = boardBottomLeft.y + y;

        return new Vector2(xPos, yPos);
    }

    public void UpdateCatominoGuidePosition(Vector2 loc)
    {
        if(_guideline != null)
        {
            _guideline.transform.position = loc;
            _guideline.GetComponent<SpriteRenderer>().sprite = _nextCatomino.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public int[] GOPosToBoardPosition(Vector2 position)
    {
        int xPos = (int)Mathf.Floor(position.x - boardBottomLeft.x);
        int yPos = (int)Mathf.Floor(position.y - boardBottomLeft.y);

        return new int[] { xPos, yPos };
    }

    public GameObject GetCustomerFromLocation(int[] loc)
    {
        GameObject cust = null;

        foreach (GameObject go in _instantiatedBoardObjects)
        {
            if (go.CompareTag("Customer"))
            {
                if(go.GetComponent<Customer>().myBoardLocation[0] == loc[0] && go.GetComponent<Customer>().myBoardLocation[1] == loc[1])
                {
                    cust = go;
                    break;
                }
            }
        }

        return cust;
    }
}
