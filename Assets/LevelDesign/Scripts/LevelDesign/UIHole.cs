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

    [Header("Hole Data")]
    public ScrollRect scrollHoleData;
    public GameObject holeDataUIPrefab;
    public List<HoleDataUI> holesDataUI = new List<HoleDataUI>();
    public int indexHoleData = -1;

    [Header("Hole Layer Data")]
    public ScrollRect scrollHoleLayerData;
    public GameObject holeLayerDataUIPrefab;
    public List<HoleLayerDataUI> holeLayersDataUI;
    public Transform buttonAddHoleLayerData;
    public Transform buttonAddHoleLayerDataToTop;

    [Header("Grid")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GridLayoutGroup gridLayoutGroup;

    [Header("Cell")]
    public GameObject boxTilePrefab;
    public BoxTile[,] boxTileGrid;

    [Header("Hole Preview")]
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

        indexHoleData = -1;

        if (LevelDesign.Instance.levelData.holesData.Count > 0)
        {
            int index = 0;
            foreach (var holeData in LevelDesign.Instance.levelData.holesData)
            {
                GameObject obj = holeDataUIPrefab.Spawn(scrollHoleData.content);
                obj.transform.localScale = Vector3.one;

                HoleDataUI holeDataUI = obj.GetComponent<HoleDataUI>();
                holeDataUI.Init(index);
                holesDataUI.Add(holeDataUI);

                index++;
            }
        }

        UpdateTotalColors();
    }

    public void RecycleAll()
    {
        ClearHoleDataUI();
        ClearHoleLayerDataUI();
    }

    public void ClearHoleDataUI()
    {
        foreach (var holeDataUI in holesDataUI)
        {
            holeDataUI.gameObject.Recycle();
        }
        holesDataUI.Clear();
    }

    public void ClearHoleLayerDataUI()
    {
        foreach (var holeLayerDataUI in holeLayersDataUI)
        {
            holeLayerDataUI.gameObject.Recycle();
        }
        holeLayersDataUI.Clear();
    }

    public void SpawnHoleLayerData(int holeDataIndex)
    {
        ClearHoleLayerDataUI();

        indexHoleData = holeDataIndex;

        int index = 0;
        foreach (var holeLayerData in LevelDesign.Instance.levelData.holesData[indexHoleData].holeLayersData)
        {
            GameObject obj = holeLayerDataUIPrefab.Spawn(scrollHoleLayerData.content);
            obj.transform.localScale = Vector3.one;

            HoleLayerDataUI holeLayerDataUI = obj.GetComponent<HoleLayerDataUI>();
            holeLayerDataUI.Init(index);
            holeLayerDataUI.SetUp(holeLayerData);

            holeLayersDataUI.Add(holeLayerDataUI);
            index++;
        }

        buttonAddHoleLayerData.SetAsLastSibling();
        buttonAddHoleLayerDataToTop.SetAsFirstSibling();
    }

    public void RemoveHoleLayerDataUI(int index)
    {
        holeLayersDataUI.RemoveAt(index);

        LevelDesign.Instance.levelData.holesData[indexHoleData].holeLayersData.RemoveAt(index);

        for (int i = 0; i < holeLayersDataUI.Count; i++)
        {
            holeLayersDataUI[i].Init(i);
        }

        UpdateTotalColors();
    }

    public void OnClickButtonAddHoleLayerData()
    {
        if (indexHoleData == -1) return;

        int index = holeLayersDataUI.Count;

        GameObject obj = holeLayerDataUIPrefab.Spawn(scrollHoleLayerData.content);
        obj.transform.localScale = Vector3.one;

        HoleLayerDataUI holeLayerDataUI = obj.GetComponent<HoleLayerDataUI>();
        holeLayersDataUI.Add(holeLayerDataUI);

        HoleLayerData holeLayerData = new HoleLayerData();
        LevelDesign.Instance.levelData.holesData[indexHoleData].holeLayersData.Add(holeLayerData);

        holeLayerDataUI.Init(index);
        holeLayerDataUI.SetUp(holeLayerData);

        buttonAddHoleLayerData.SetAsLastSibling();
        buttonAddHoleLayerDataToTop.SetAsFirstSibling();

        UpdateTotalColors();
    }

    public void OnClickButtonAddHoleLayerDataToTop()
    {
        if (indexHoleData == -1) return;

        GameObject obj = holeLayerDataUIPrefab.Spawn(scrollHoleLayerData.content);
        obj.transform.localScale = Vector3.one;
        obj.transform.SetAsFirstSibling();

        HoleLayerDataUI holeLayerDataUI = obj.GetComponent<HoleLayerDataUI>();
        holeLayersDataUI.Insert(0, holeLayerDataUI);

        HoleLayerData holeLayerData = new HoleLayerData();
        LevelDesign.Instance.levelData.holesData[indexHoleData].holeLayersData.Insert(0, holeLayerData);

        for (int i = 0; i < holeLayersDataUI.Count; i++)
        {
            holeLayersDataUI[i].Init(i);
        }

        holeLayerDataUI.SetUp(holeLayerData);

        buttonAddHoleLayerData.SetAsLastSibling();
        buttonAddHoleLayerDataToTop.SetAsFirstSibling();

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
        int totalHole = LevelDesign.Instance.levelData.holesData.Count;
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

        foreach (var holeData in LevelDesign.Instance.levelData.holesData)
        {
            foreach (var holeLayerData in holeData.holeLayersData)
            {
                if (holeLayerData.color != ColorEnum.None)
                {
                    if (!allColors.Contains(holeLayerData.color))
                    {
                        allColors.Add(holeLayerData.color);
                    }

                    if (!holeDicts.ContainsKey(holeLayerData.color))
                    {
                        holeDicts.Add(holeLayerData.color, 0);
                    }
                    holeDicts[holeLayerData.color] += targetAmountHole;
                }
            }
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

    public void UpdateHolePreview()
    {
        objHoleFull.SetActive(false);
        objHoleHalf.SetActive(false);
        objHoleQuarter.SetActive(false);

        int totalHole = LevelDesign.Instance.levelData.holesData.Count;
        switch (totalHole)
        {
            case 1:
                {
                    objHoleFull.SetActive(true);
                    holeFull.Init(LevelDesign.Instance.levelData.holesData[0]);
                }
                break;
            case 2:
                {
                    objHoleHalf.SetActive(true);
                    for (int i = 0; i < totalHole; i++)
                    {
                        HoleData holeData = LevelDesign.Instance.levelData.holesData[i];
                        holeHalf[i].Init(holeData);
                    }
                }
                break;
            case 4:
                {
                    objHoleQuarter.SetActive(true);
                    for (int i = 0; i < totalHole; i++)
                    {
                        HoleData holeData = LevelDesign.Instance.levelData.holesData[i];
                        holeQuarter[i].Init(holeData);
                    }
                }
                break;
        }
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

    public void OnClickButtonCreateHoleData()
    {
        if (LevelDesign.Instance.levelData.holesData.Count >= 4) return;

        int index = holesDataUI.Count;

        GameObject obj = holeDataUIPrefab.Spawn(scrollHoleData.content);
        obj.transform.localScale = Vector3.one;

        HoleData holeData = new HoleData();
        LevelDesign.Instance.levelData.holesData.Add(holeData);

        HoleDataUI holeDataUI = obj.GetComponent<HoleDataUI>();
        holeDataUI.Init(index);
        holesDataUI.Add(holeDataUI);

        UpdateTotalColors();
    }

    public void OnClickButtonRemoveHoleData()
    {
        if (indexHoleData == -1) return;

        holesDataUI[indexHoleData].gameObject.Recycle();
        holesDataUI.RemoveAt(indexHoleData);

        for (int i = 0; i < holesDataUI.Count; i++)
        {
            holesDataUI[i].Init(i);
        }

        ClearHoleLayerDataUI();

        LevelDesign.Instance.levelData.holesData.RemoveAt(indexHoleData);

        indexHoleData = -1;

        UpdateTotalColors();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
