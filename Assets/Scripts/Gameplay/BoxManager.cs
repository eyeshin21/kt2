using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxManager : Singleton<BoxManager>
{
    [Header("Grid")]
    public Vector2Int gridSize;
    public bool[,] walkableGrid;
    public bool[,] groundGrid;
    public Box[,] boxGrid;
    public GameObject gridPiecePrefab;
    public Transform gridPieceParent;
    public List<GameObject> gridPieces = new List<GameObject>();

    [Header("Corners")]
    public GameObject inCornerPrefab;
    public GameObject outCornerPrefab;
    public GameObject crossCornerPrefab;
    public GameObject straightCornerPrefab;
    public Transform cornerParent;
    public List<GameObject> corners = new List<GameObject>();

    [Header("Box")]
    public GameObject boxPrefab;
    public Transform boxParent;
    public List<Box> boxes = new List<Box>();

    [Header("Tunnel")]
    public GameObject tunnelPrefab;
    public List<TunnelController> tunnels = new List<TunnelController>();
    public TunnelController[,] tunnelGrid;

    [Header("Pin")]
    public GameObject pinPrefab;
    public List<PinController> pins = new List<PinController>();

    [Header("Cloth")]
    public GameObject cloth2x2Prefab;
    public GameObject cloth2x3Prefab;
    public GameObject cloth3x2Prefab;
    public List<ClothController> clothes = new List<ClothController>();

    [Header("Lock Chain")]
    public GameObject lockChainPrefab;
    public List<LockChainController> lockChains = new List<LockChainController>();

    [Header("Cell")]
    public GameObject cellPrefab;
    public List<GameObject> cells;
    public Transform cellParent;
    public float cellSize = 1f;

    private float startX;
    private float startY;

    public void Init(Vector2Int gridSize, List<GridSlotData> gridSlotsData, List<BoxData> boxesData, List<TunnelData> tunnelsData, List<PinData> pinsData, List<ClothData> clothsData, List<LockChainData> lockChainsData)
    {
        this.gridSize = gridSize;

        boxGrid = new Box[gridSize.x, gridSize.y];
        tunnelGrid = new TunnelController[gridSize.x, gridSize.y];
        groundGrid = new bool[gridSize.x, gridSize.y];
        walkableGrid = new bool[gridSize.x, gridSize.y];

        startX = -(gridSize.x - 1) / 2f * cellSize;
        //startY = -(gridSize.y - 1) / 2f * cellSize;
        startY = -(cellSize / 2f);

        for (int i = 0; i < gridSlotsData.Count; i++)
        {
            GridSlotData data = gridSlotsData[i];
            if (data.gridSlotType == GridSlotType.Road)
            {
                SpawnCell(data);
                groundGrid[data.coordinateX, data.coordinateY] = true;
            }
        }

        for (int i = 0; i < boxesData.Count; i++)
        {
            BoxData data = boxesData[i];

            boxGrid[data.coordinateX, data.coordinateY] = SpawnBox(data);
        }

        for (int i = 0; i < tunnelsData.Count; i++)
        {
            TunnelData data = tunnelsData[i];

            GameObject newTunnel = tunnelPrefab.Spawn(boxParent);
            newTunnel.name = $"Tunnel_{data.coordinateX}_{data.coordinateY}";
            newTunnel.transform.localPosition = GetWorldPos(data.coordinateX, data.coordinateY);

            TunnelController tunnel = newTunnel.GetComponent<TunnelController>();
            tunnel.Init(data);
            tunnels.Add(tunnel);

            for (int j = 0; j < data.boxesData.Count; j++)
            {
                BoxData boxData = data.boxesData[j];
                boxData.coordinateX = data.coordinateX;
                boxData.coordinateY = data.coordinateY;
                Box box = SpawnBox(boxData);
                box.gameObject.name = $"Shooter_{j} ({newTunnel.name})";
                box.gameObject.SetActive(false);
                tunnel.boxes.Add(box);
            }

            tunnelGrid[data.coordinateX, data.coordinateY] = tunnel;
        }

        for (int i = 0; i < pinsData.Count; i++)
        {
            PinData data = pinsData[i];
            GameObject newPin = pinPrefab.Spawn(boxParent);
            newPin.name = $"Pin_{data.coordinateX}_{data.coordinateY}";
            newPin.transform.localPosition = GetWorldPos(data.coordinateX, data.coordinateY);

            PinController pin = newPin.GetComponent<PinController>();
            pin.Init(data);
            pins.Add(pin);
        }

        for (int i = 0; i < clothsData.Count; i++)
        {
            ClothData data = clothsData[i];

            GameObject newCloth;

            switch (data.clothType)
            {
                case ClothType.TwoxTwo:
                    newCloth = cloth2x2Prefab.Spawn(boxParent);
                    break;
                case ClothType.TwoxThree:
                    newCloth = cloth2x3Prefab.Spawn(boxParent);
                    break;
                case ClothType.ThreexTwo:
                    newCloth = cloth3x2Prefab.Spawn(boxParent);
                    break;
                default:
                    newCloth = cloth2x2Prefab.Spawn(boxParent);
                    break;
            }

            newCloth.name = $"Cloth_{data.coordinateX}_{data.coordinateY}";
            newCloth.transform.localPosition = GetWorldPos(data.coordinateX, data.coordinateY);

            ClothController cloth = newCloth.GetComponent<ClothController>();
            cloth.Init(data);
            clothes.Add(cloth);
        }

        for (int i = 0; i < lockChainsData.Count; i++)
        {
            LockChainData data = lockChainsData[i];

            GameObject newLockChain = lockChainPrefab.Spawn(boxParent);
            newLockChain.name = $"LockChain_{data.coordinateX}_{data.coordinateY}";
            newLockChain.transform.localPosition = GetWorldPos(data.coordinateX, data.coordinateY);

            LockChainController lockChain = newLockChain.GetComponent<LockChainController>();
            lockChain.Init(data);
            lockChains.Add(lockChain);
        }

        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].CheckLink();
        }

        SpawnCorner();

        CheckTunnel();
        CheckPin();
        CheckCloth();
        CheckLockChain();
        //CheckActiveBox();
    }

    public Box SpawnBox(BoxData data)
    {
        GameObject obj = boxPrefab.Spawn(boxParent);

        obj.name = $"Box_{data.coordinateX}_{data.coordinateY}";
        obj.transform.localPosition = GetWorldPos(data.coordinateX, data.coordinateY);

        Box box = obj.GetComponent<Box>();
        box.Init(data);

        boxes.Add(box);

        return box;
    }

    public void SpawnCell(GridSlotData data)
    {
        GameObject newCell = cellPrefab.Spawn(cellParent);
        newCell.name = $"Cell_{data.coordinateX}_{data.coordinateY}";
        newCell.transform.localPosition = GetWorldPos(data.coordinateX, data.coordinateY);
        newCell.transform.localScale = Vector3.one * cellSize;

        cells.Add(newCell);
    }

    public void SpawnGridPiece(int x, int y)
    {
        GameObject newGridPiece = gridPiecePrefab.Spawn(gridPieceParent);
        newGridPiece.name = $"Piece_{x}_{y}";
        newGridPiece.transform.localPosition = GetWorldPos(x, y);
        newGridPiece.transform.localScale = Vector3.one * cellSize;

        if (y == -1 && x >= 0 && x < gridSize.x)
        {
            newGridPiece.transform.localPosition += Vector3.back * (cellSize * 0.25f);
        }

        gridPieces.Add(newGridPiece);
    }

    public void SpawnCorner()
    {
        int total = 12;

        for (int y = -total; y < gridSize.y; y++)
        {
            for (int x = -(total / 2); x <= gridSize.x + (total / 2); x++)
            {
                bool center = IsValidCell(x, y);

                bool left = IsValidCell(x - 1, y);
                bool right = IsValidCell(x + 1, y);
                bool up = IsValidCell(x, y + 1);
                bool down = IsValidCell(x, y - 1);

                bool upLeft = IsValidCell(x - 1, y + 1);
                bool downLeft = IsValidCell(x - 1, y - 1);

                bool upRight = IsValidCell(x + 1, y + 1);
                bool downRight = IsValidCell(x + 1, y - 1);

                if (!center)
                {
                    SpawnGridPiece(x, y);

                    if (left)
                    {
                        if (upLeft)
                        {
                            if (downLeft)
                            {
                                if (!up)
                                {
                                    if (!down)
                                    {
                                        SpawnStraightCorner(x, y, Direction.Left);
                                    }
                                    else
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Left);
                                        straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner.transform.localPosition += Vector3.forward * (cellSize / 4f);

                                        SpawnInCorner(x, y, Direction.Down, Direction.Left);

                                        if (downRight)
                                        {
                                            straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                            straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                            straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                        }
                                    }
                                }
                                else
                                {
                                    if (y != gridSize.y - 1)
                                    {
                                        SpawnInCorner(x, y, Direction.Up, Direction.Left);
                                    }

                                    if (!down)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Left);
                                        straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner.transform.localPosition -= Vector3.forward * (cellSize / 4f);
                                    }
                                    else
                                    {
                                        SpawnInCorner(x, y, Direction.Down, Direction.Left);

                                        if (downRight)
                                        {
                                            if (!right)
                                            {
                                                GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                                straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                                straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (down)
                                {
                                    if (!right)
                                    {
                                        if (downRight)
                                        {
                                            GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                            straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                            straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                        }
                                    }
                                }
                                else
                                {
                                    GameObject outCorner = SpawnOutCorner(x, y, Direction.Down, Direction.Left);

                                    if (y == 0)
                                    {
                                        outCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);

                                        GameObject straightCorner2 = SpawnStraightCorner(x, y, Direction.Left);
                                        straightCorner2.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner2.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                    }
                                }

                                if (up)
                                {
                                    SpawnInCorner(x, y, Direction.Up, Direction.Left);
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Left);
                                    straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                    straightCorner.transform.localPosition += Vector3.forward * (cellSize / 4f);

                                    GameObject outCorner = SpawnOutCorner(x, y, Direction.Down, Direction.Left);

                                    if (y == 0)
                                    {
                                        outCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);

                                        GameObject straightCorner2 = SpawnStraightCorner(x, y, Direction.Left);
                                        straightCorner2.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner2.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (y != gridSize.y - 1)
                            {
                                SpawnOutCorner(x, y, Direction.Up, Direction.Left);
                            }
                            else
                            {
                                SpawnInCorner(x, y, Direction.Up, Direction.Left);

                                if (!right)
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Up);
                                    straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                    straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                }
                            }

                            if (downLeft)
                            {
                                if (down)
                                {
                                    SpawnInCorner(x, y, Direction.Down, Direction.Left);

                                    if (downRight)
                                    {
                                        if (!right)
                                        {
                                            GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                            straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                            straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                        }
                                    }
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Left);
                                    straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                    straightCorner.transform.localPosition -= Vector3.forward * (cellSize / 4f);
                                }
                            }
                            else
                            {
                                GameObject outCorner = SpawnOutCorner(x, y, Direction.Down, Direction.Left);

                                if (y == 0)
                                {
                                    outCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);

                                    GameObject straightCorner2 = SpawnStraightCorner(x, y, Direction.Left);
                                    straightCorner2.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                    straightCorner2.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                }
                            }

                            if (up)
                            {
                                SpawnCrossCorner(x, y, Direction.Up, Direction.Left);
                            }
                        }

                    }
                    else
                    {
                        if (downLeft)
                        {
                            if (down)
                            {
                                if (downRight)
                                {
                                    if (!right)
                                    {
                                        SpawnStraightCorner(x, y, Direction.Down);
                                    }
                                    else
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                        straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);
                                    }
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                    straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                    straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);
                                }
                            }
                        }
                        else
                        {
                            if (down)
                            {
                                if (downRight)
                                {
                                    if (!right)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                        straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                    }
                                }
                            }
                        }

                        if (y == gridSize.y - 1)
                        {
                            GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Up);

                            if (right)
                            {
                                straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);
                            }
                        }
                    }
                }
                else
                {
                    if (!left)
                    {
                        if (!upLeft)
                        {
                            if (!downLeft)
                            {
                                if (up)
                                {
                                    if (down)
                                    {
                                        SpawnStraightCorner(x - 1, y, Direction.Right);
                                    }
                                    else
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                        straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner.transform.localPosition += Vector3.forward * (cellSize / 4f);

                                        GameObject outCorner = SpawnOutCorner(x - 1, y, Direction.Down, Direction.Right);

                                        if (y == 0)
                                        {
                                            outCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);

                                            GameObject straightCorner2 = SpawnStraightCorner(x - 1, y, Direction.Right);
                                            straightCorner2.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                            straightCorner2.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                        }

                                        if (!downRight)
                                        {
                                            if (right)
                                            {
                                                straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                                straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                                straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                                if (y == 0)
                                                {
                                                    straightCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (y != gridSize.y - 1)
                                    {
                                        SpawnOutCorner(x - 1, y, Direction.Up, Direction.Right);
                                    }
                                    else
                                    {
                                        SpawnInCorner(x - 1, y, Direction.Up, Direction.Right);
                                    }

                                    if (down)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                        straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner.transform.localPosition -= Vector3.forward * (cellSize / 4f);
                                    }
                                    else
                                    {
                                        GameObject outCorner = SpawnOutCorner(x - 1, y, Direction.Down, Direction.Right);

                                        if (y == 0)
                                        {
                                            outCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);

                                            GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                            straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                            straightCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                        }

                                        if (!downRight)
                                        {
                                            GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                            straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                            straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                            if (y == 0)
                                            {
                                                straightCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SpawnInCorner(x - 1, y, Direction.Down, Direction.Right);

                                if (y == gridSize.y - 1)
                                {
                                    SpawnInCorner(x - 1, y, Direction.Up, Direction.Right);
                                    continue;
                                }

                                if (!up)
                                {
                                    SpawnOutCorner(x - 1, y, Direction.Up, Direction.Right);
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                    straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                    straightCorner.transform.localPosition += Vector3.forward * (cellSize / 4f);
                                }
                            }
                        }
                        else
                        {
                            SpawnInCorner(x - 1, y, Direction.Up, Direction.Right);

                            if (!downLeft)
                            {
                                if (!down)
                                {
                                    GameObject outCorner = SpawnOutCorner(x - 1, y, Direction.Down, Direction.Right);

                                    if (y == 0)
                                    {
                                        outCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);

                                        GameObject straightCorner2 = SpawnStraightCorner(x - 1, y, Direction.Right);
                                        straightCorner2.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner2.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                    }

                                    if (right)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                        straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                        if (y == 0)
                                        {
                                            straightCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                        }
                                    }
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                    straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                    straightCorner.transform.localPosition -= Vector3.forward * (cellSize / 4f);
                                }
                            }
                            else
                            {
                                if (!down)
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                    straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                    straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                    if (y == 0)
                                    {
                                        straightCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                    }
                                }
                                else
                                {
                                    SpawnInCorner(x - 1, y, Direction.Down, Direction.Right);
                                }
                            }

                            if (!up)
                            {
                                SpawnCrossCorner(x, y + 1, Direction.Down, Direction.Left);
                            }
                        }
                    }
                    else
                    {
                        if (!downLeft)
                        {
                            if (!down)
                            {
                                if (!downRight)
                                {
                                    if (right)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);

                                        if (y == 0)
                                        {
                                            straightCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                        }
                                    }
                                    else
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                        straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);

                                        if (y == 0)
                                        {
                                            straightCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                        }
                                    }
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                    straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                    straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);

                                    if (y == 0)
                                    {
                                        straightCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!down)
                            {
                                if (!downRight)
                                {
                                    if (right)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                        straightCorner.transform.localScale = new Vector3(1, 1, cellSize / 2f);
                                        straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                        if (y == 0)
                                        {
                                            straightCorner.transform.localPosition += Vector3.back * (cellSize * 0.25f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    GameObject SpawnStraightCorner(int x, int y, Direction direction)
    {
        GameObject newCorner = straightCornerPrefab.Spawn(cornerParent);
        newCorner.name = $"StraightCorner_{x}_{y}";
        newCorner.transform.localScale = Vector3.one * cellSize;

        switch (direction)
        {
            case Direction.Left:
                newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
                break;
            case Direction.Right:
                newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 180f;
                break;
            case Direction.Up:
                newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.forward * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 90f;
                break;
            case Direction.Down:
                newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.back * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 270f;
                break;
        }

        corners.Add(newCorner);

        return newCorner;
    }

    GameObject SpawnInCorner(int x, int y, Direction vertical, Direction horizontal)
    {
        GameObject newCorner = inCornerPrefab.Spawn(cornerParent);
        newCorner.name = $"InCorner_{x}_{y}";
        newCorner.transform.localScale = Vector3.one * cellSize;

        if (vertical == Direction.Up)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.forward * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 90f;
            }
        }
        else if (vertical == Direction.Down)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.back * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 270f;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 180f;

            }
        }

        corners.Add(newCorner);
        return newCorner;
    }

    GameObject SpawnOutCorner(int x, int y, Direction vertical, Direction horizontal)
    {
        GameObject newCorner = outCornerPrefab.Spawn(cornerParent);
        newCorner.name = $"OutCorner_{x}_{y}";
        newCorner.transform.localScale = Vector3.one * cellSize;

        if (vertical == Direction.Up)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.forward * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 270f;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 180f;
            }
        }
        else if (vertical == Direction.Down)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.back * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 90f;

            }
        }

        corners.Add(newCorner);
        return newCorner;
    }

    GameObject SpawnCrossCorner(int x, int y, Direction vertical, Direction horizontal)
    {
        GameObject newCorner = crossCornerPrefab.Spawn(cornerParent);
        newCorner.name = $"CrossCorner_{x}_{y}";
        newCorner.transform.localScale = Vector3.one * cellSize;

        if (vertical == Direction.Up)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.forward * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 270f;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
            }
        }
        else if (vertical == Direction.Down)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.back * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.up * 270f;

            }
        }

        corners.Add(newCorner);
        return newCorner;
    }

    public void RemoveBox(Box boxToRemove, bool checkTunnel = true, bool checkActive = true)
    {
        boxes.Remove(boxToRemove);
        boxGrid[boxToRemove.pos.x, boxToRemove.pos.y] = null;

        if (checkTunnel)
        {
            CheckTunnel();
        }

        if (boxGrid[boxToRemove.pos.x, boxToRemove.pos.y] == null)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (boxGrid[x, y] || !groundGrid[x, y])
                    {
                        walkableGrid[x, y] = false;
                    }
                    else
                    {
                        walkableGrid[x, y] = groundGrid[x, y];
                    }
                }
            }

            if (checkActive)
            {
                Point source = new Point(boxToRemove.pos.x, boxToRemove.pos.y);
                List<Point> points = BFS.SearchShooter(walkableGrid, source);

                if (points.Count > 0)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        Point point = points[i];
                        Box box = boxGrid[point.x, point.y];
                        if (box != null && !box.isActive)
                        {
                            box.Active2();
                        }
                    }
                }
            }
        }
    }

    public void CheckActiveBox()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].CheckActive();
        }
    }

    public void CheckTunnel()
    {
        for (int i = 0; i < tunnels.Count; i++)
        {
            tunnels[i].CheckActiveNextBox();
        }
    }

    public void CheckPin()
    {
        for (int i = 0; i < pins.Count; i++)
        {
            pins[i].CheckBox();
        }
    }

    public void CheckCloth()
    {
        for (int i = 0; i < clothes.Count; i++)
        {
            clothes[i].CheckBox();
        }
    }

    public void CheckLockChain()
    {
        for (int i = 0; i < lockChains.Count; i++)
        {
            lockChains[i].CheckBox();
        }
    }

    public bool CanMoveOut(Vector2Int startPos)
    {
        int maxY = gridSize.y - 1;

        if (startPos.y == maxY)
        {
            return true;
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (boxGrid[x, y] || !groundGrid[x, y])
                {
                    walkableGrid[x, y] = false;
                }
                else
                {
                    walkableGrid[x, y] = groundGrid[x, y];
                }
            }
        }

        Point source = new Point(startPos.x, startPos.y);
        List<Point> points = BFS.Search(walkableGrid, source);

        return points != null;
    }

    public bool CanMoveOut(Vector2Int startPos, out List<Point> points)
    {
        int maxY = gridSize.y - 1;

        if (startPos.y == maxY)
        {
            points = new List<Point>();
            return true;
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (boxGrid[x, y] || !groundGrid[x, y])
                {
                    walkableGrid[x, y] = false;
                }
                else
                {
                    walkableGrid[x, y] = groundGrid[x, y];
                }
            }
        }

        Point source = new Point(startPos.x, startPos.y);
        points = BFS.Search(walkableGrid, source);

        return points != null;
    }

    public bool IsValidCell(int x, int y)
    {
        return (x >= 0) && (x < gridSize.x) && (y >= 0) && (y < gridSize.y) && groundGrid[x, y];
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(startX + x * cellSize, 0, (startY + y - (gridSize.y - 1)) * cellSize);
    }

    public void Recycle()
    {
        for (int i = 0; i < corners.Count; i++)
        {
            corners[i].Recycle();
        }
        corners.Clear();

        for (int i = 0; i < gridPieces.Count; i++)
        {
            gridPieces[i].Recycle();
        }
        gridPieces.Clear();

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Recycle();
        }
        cells.Clear();

        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].Recycle();
        }
        boxes.Clear();

        for (int i = 0; i < tunnels.Count; i++)
        {
            tunnels[i].Recycle();
        }
        tunnels.Clear();

        for (int i = 0; i < pins.Count; i++)
        {
            pins[i].Recycle();
        }
        pins.Clear();

        for (int i = 0; i < clothes.Count; i++)
        {
            clothes[i].Recycle();
        }
        clothes.Clear();

        for (int i = 0; i < lockChains.Count; i++)
        {
            lockChains[i].Recycle();
        }
        lockChains.Clear();
    }
}
