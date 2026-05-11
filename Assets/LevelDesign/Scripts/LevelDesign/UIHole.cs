using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHole : UI
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI txtTotalColors;

    [Header("Hole Data Default")]
    public ScrollRect scrollHoleDataDefault;
    public GameObject holeDataDefaultUIPrefab;
    public List<HoleDataDefaultUI> holesDataDefaultUI;
    public Transform buttonAddHoleDataDefault;

    [Header("Queue Hole")]
    public ScrollRect scrollQueueHole;
    public GameObject queueHoleUIPrefab;
    public List<QueueHoleUI> queueHolesUI;
    public Transform buttonAddQueueHole;

    [Header("Grid")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GridLayoutGroup gridLayoutGroup;

    [Header("Cell")]
    public GameObject boxTilePrefab;
    public BoxTile[,] boxTileGrid;

    [Header("Hole Preview")]
    public GameObject objQueueHole;
    public GDHoleLayer queueHole;

    public GameObject objHoleFull;
    public GDHole holeFull;

    public GameObject objHoleHalf;
    public List<GDHole> holeHalf;

    public GameObject objHoleQuarter;
    public List<GDHole> holeQuarter;

    public override void Show()
    {
        base.Show();
        Init();
        InitGrid();
        UpdateHolePreview();
    }

    public void UpdateHolePreview()
    {
        objQueueHole.SetActive(false);
        objHoleFull.SetActive(false);
        objHoleHalf.SetActive(false);
        objHoleQuarter.SetActive(false);

        if (LevelDesign.Instance.levelData.queueHoles.Count > 0)
        {
            objQueueHole.SetActive(true);
            queueHole.Init(LevelDesign.Instance.levelData.queueHoles[0]);
        }

        int totalHole = LevelDesign.Instance.levelData.holesDataDefault.Count;
        switch (totalHole)
        {
            case 1:
                {
                    objHoleFull.SetActive(true);
                    holeFull.Init(LevelDesign.Instance.levelData.holesDataDefault[0]);
                }
                break;
            case 2:
                {
                    objHoleHalf.SetActive(true);
                    for (int i = 0; i < totalHole; i++)
                    {
                        HoleDataDefault holeDataDefault = LevelDesign.Instance.levelData.holesDataDefault[i];
                        holeHalf[i].Init(holeDataDefault);
                    }
                }
                break;
            case 4:
                {
                    objHoleQuarter.SetActive(true);
                    for (int i = 0; i < totalHole; i++)
                    {
                        HoleDataDefault holeDataDefault = LevelDesign.Instance.levelData.holesDataDefault[i];
                        holeQuarter[i].Init(holeDataDefault);
                    }
                }
                break;
        }
    }

    public void InitGrid()
    {
        if (LevelDesign.Instance.levelData.gridSlotsData.Count > 0)
        {
            if (boxTileGrid != null)
            {
                foreach (var tile in boxTileGrid)
                {
                    tile.gameObject.Recycle();
                }

                boxTileGrid = null;
            }

            int gridSizeX = LevelDesign.Instance.levelData.gridWidth;
            int gridSizeY = LevelDesign.Instance.levelData.gridHeight;

            GenerateGrid(gridSizeX, gridSizeY);

            foreach (var data in LevelDesign.Instance.levelData.gridSlotsData)
            {
                boxTileGrid[data.coordinateX, data.coordinateY].ActiveTile(data.gridSlotType == GridSlotType.Road);
            }

            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                boxTileGrid[data.coordinateX, data.coordinateY].SpawnBox();
                boxTileGrid[data.coordinateX, data.coordinateY].box.SetUp(data);
            }

            foreach (var data in LevelDesign.Instance.levelData.tunnelsData)
            {
                boxTileGrid[data.coordinateX, data.coordinateY].SpawnTunnel();
                boxTileGrid[data.coordinateX, data.coordinateY].tunnel.SetUp(data);
            }

            foreach (var data in LevelDesign.Instance.levelData.pinsData)
            {
                boxTileGrid[data.coordinateX, data.coordinateY].SpawnPin();
                boxTileGrid[data.coordinateX, data.coordinateY].pin.SetUp(data);
            }

            foreach (var data in LevelDesign.Instance.levelData.clothsData)
            {
                boxTileGrid[data.coordinateX, data.coordinateY].SpawnCloth();
                boxTileGrid[data.coordinateX, data.coordinateY].cloth.SetUp(data);
            }

            foreach (var data in LevelDesign.Instance.levelData.lockChainsData)
            {
                boxTileGrid[data.coordinateX, data.coordinateY].SpawnLockChain();
                boxTileGrid[data.coordinateX, data.coordinateY].lockChain.SetUp(data);
            }
        }
    }

    public void Init()
    {
        RecycleAll();

        if (LevelDesign.Instance.levelData.holesDataDefault.Count > 0)
        {
            int index = 0;
            foreach (var holeData in LevelDesign.Instance.levelData.holesDataDefault)
            {
                GameObject obj = holeDataDefaultUIPrefab.Spawn(scrollHoleDataDefault.content);
                obj.transform.localScale = Vector3.one;

                HoleDataDefaultUI holeDataUI = obj.GetComponent<HoleDataDefaultUI>();
                holeDataUI.Init(index);
                holeDataUI.SetUp(holeData);
                holesDataDefaultUI.Add(holeDataUI);

                index++;
            }

            buttonAddHoleDataDefault.SetAsLastSibling();
        }

        if (LevelDesign.Instance.levelData.queueHoles.Count > 0)
        {
            int index = 0;
            foreach (var queueHole in LevelDesign.Instance.levelData.queueHoles)
            {
                GameObject obj = queueHoleUIPrefab.Spawn(scrollQueueHole.content);
                obj.transform.localScale = Vector3.one;

                QueueHoleUI colorQueueUI = obj.GetComponent<QueueHoleUI>();
                colorQueueUI.Init(index);
                colorQueueUI.SetUp(queueHole);
                queueHolesUI.Add(colorQueueUI);

                index++;
            }

            buttonAddQueueHole.SetAsLastSibling();
        }

        UpdateTotalColors();
    }

    public void RecycleAll()
    {
        ClearHoleDataUI();
        ClearQueueHoleUI();
    }

    public void ClearHoleDataUI()
    {
        foreach (var holeDataDefaultUI in holesDataDefaultUI)
        {
            holeDataDefaultUI.gameObject.Recycle();
        }
        holesDataDefaultUI.Clear();
    }

    public void ClearQueueHoleUI()
    {
        foreach (var queueHole in queueHolesUI)
        {
            queueHole.gameObject.Recycle();
        }
        queueHolesUI.Clear();
    }

    public void RemoveHoleDataDefault(int index)
    {
        holesDataDefaultUI.RemoveAt(index);

        LevelDesign.Instance.levelData.holesDataDefault.RemoveAt(index);

        for (int i = 0; i < holesDataDefaultUI.Count; i++)
        {
            holesDataDefaultUI[i].Init(i);
        }

        UpdateTotalColors();
    }

    public void RemoveQueueHole(int index)
    {
        queueHolesUI.RemoveAt(index);

        LevelDesign.Instance.levelData.queueHoles.RemoveAt(index);

        for (int i = 0; i < queueHolesUI.Count; i++)
        {
            queueHolesUI[i].Init(i);
        }

        UpdateTotalColors();
    }

    public void OnClickButtonAddHoleDataDefault()
    {
        int index = holesDataDefaultUI.Count;

        GameObject obj = holeDataDefaultUIPrefab.Spawn(scrollHoleDataDefault.content);
        obj.transform.localScale = Vector3.one;

        HoleDataDefaultUI holeDataDefaultUI = obj.GetComponent<HoleDataDefaultUI>();
        holesDataDefaultUI.Add(holeDataDefaultUI);

        HoleDataDefault holeDataDefault = new HoleDataDefault();
        LevelDesign.Instance.levelData.holesDataDefault.Add(holeDataDefault);

        holeDataDefaultUI.Init(index);
        holeDataDefaultUI.SetUp(holeDataDefault);

        buttonAddHoleDataDefault.SetAsLastSibling();

        UpdateTotalColors();
    }

    public void OnClickButtonAddQueueHole()
    {
        int index = queueHolesUI.Count;

        GameObject obj = queueHoleUIPrefab.Spawn(scrollQueueHole.content);
        obj.transform.localScale = Vector3.one;

        QueueHoleUI queueHoleUI = obj.GetComponent<QueueHoleUI>();
        queueHolesUI.Add(queueHoleUI);

        HoleData holeData = new HoleData();
        LevelDesign.Instance.levelData.queueHoles.Add(holeData);

        queueHoleUI.Init(index);
        queueHoleUI.SetUp(holeData);

        buttonAddQueueHole.SetAsLastSibling();

        UpdateTotalColors();
    }

    public void UpdateTotalColors()
    {
        List<ColorEnum> allColors = new List<ColorEnum>();
        Dictionary<ColorEnum, int> boxDicts = new Dictionary<ColorEnum, int>();
        Dictionary<ColorEnum, int> holeDicts = new Dictionary<ColorEnum, int>();

        foreach (var boxData in LevelDesign.Instance.levelData.boxesData)
        {
            if (boxData.color != ColorEnum.None)
            {
                if (!allColors.Contains(boxData.color))
                {
                    allColors.Add(boxData.color);
                }

                if (!boxDicts.ContainsKey(boxData.color))
                {
                    boxDicts.Add(boxData.color, 0);
                }

                boxDicts[boxData.color] += 9;
            }
        }

        foreach (var tunnelData in LevelDesign.Instance.levelData.tunnelsData)
        {
            foreach (var boxData in tunnelData.boxesData)
            {
                if (boxData.color != ColorEnum.None)
                {
                    if (!allColors.Contains(boxData.color))
                    {
                        allColors.Add(boxData.color);
                    }

                    if (!boxDicts.ContainsKey(boxData.color))
                    {
                        boxDicts.Add(boxData.color, 0);
                    }

                    boxDicts[boxData.color] += 9;
                }
            }
        }

        int targetAmountHole = 0;
        int totalHole = LevelDesign.Instance.levelData.holesDataDefault.Count;
        switch (totalHole)
        {
            case 1:
                targetAmountHole = 12;
                break;
            case 2:
                targetAmountHole = 6;
                break;
            case 4:
                targetAmountHole = 3;
                break;
        }

        foreach (var holeDataDefault in LevelDesign.Instance.levelData.holesDataDefault)
        {
            if (!allColors.Contains(holeDataDefault.firstLayerHole.color))
            {
                allColors.Add(holeDataDefault.firstLayerHole.color);
            }
            if (!allColors.Contains(holeDataDefault.secondLayerHole.color))
            {
                allColors.Add(holeDataDefault.secondLayerHole.color);
            }
            if (!allColors.Contains(holeDataDefault.thirdLayerHole.color))
            {
                allColors.Add(holeDataDefault.thirdLayerHole.color);
            }

            if (!holeDicts.ContainsKey(holeDataDefault.firstLayerHole.color))
            {
                holeDicts.Add(holeDataDefault.firstLayerHole.color, 0);
            }
            holeDicts[holeDataDefault.firstLayerHole.color] += targetAmountHole;

            if (!holeDicts.ContainsKey(holeDataDefault.secondLayerHole.color))
            {
                holeDicts.Add(holeDataDefault.secondLayerHole.color, 0);
            }
            holeDicts[holeDataDefault.secondLayerHole.color] += targetAmountHole;

            if (!holeDicts.ContainsKey(holeDataDefault.thirdLayerHole.color))
            {
                holeDicts.Add(holeDataDefault.thirdLayerHole.color, 0);
            }
            holeDicts[holeDataDefault.thirdLayerHole.color] += targetAmountHole;
        }

        foreach (var holeData in LevelDesign.Instance.levelData.queueHoles)
        {
            if (!holeDicts.ContainsKey(holeData.color))
            {
                holeDicts.Add(holeData.color, 0);
            }
            holeDicts[holeData.color] += targetAmountHole;
        }

        string txt = "";
        foreach (var color in allColors)
        {
            int amountBox = 0;
            int amountHole = 0;

            if (boxDicts.ContainsKey(color))
            {
                amountBox = boxDicts[color];
            }

            if (holeDicts.ContainsKey(color))
            {
                amountHole = holeDicts[color];
            }

            if (amountBox == 0 && amountHole == 0) continue;

            string colorText = "white";
            if (amountBox == amountHole)
            {
                colorText = "green";
            }
            else
            {
                colorText = "red";
            }

            txt += $"{color.ToString()}: <color={colorText}>{amountBox}</color>/{amountHole}<br>";
        }

        txtTotalColors.text = txt;

        UpdateHolePreview();
    }

    public void GenerateGrid(int gridSizeX, int gridSizeY)
    {
        gridLayoutGroup.constraintCount = gridSizeX;

        boxTileGrid = new BoxTile[gridSizeX, gridSizeY];

        for (int y = gridSizeY - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                GameObject obj = boxTilePrefab.Spawn(scrollRect.content);
                obj.transform.localScale = Vector3.one;

                BoxTile boxTile = obj.GetComponent<BoxTile>();
                boxTile.Init(x, y);
                boxTile.ActiveTile(false);

                boxTileGrid[x, y] = boxTile;
            }
        }
    }

    public override void Hide()
    {
        base.Hide();
    }
}
