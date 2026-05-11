using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGround : UI, IPointerDownHandler, IPointerUpHandler
{
    public bool canPickGround = false;

    [Header("Grid")]
    [SerializeField] TMP_InputField gridSizeXInput;
    [SerializeField] TMP_InputField gridSizeYInput;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GridLayoutGroup gridLayoutGroup;

    [Header("Cell")]
    public GameObject groundCellPrefab;
    public GroundCell[,] groundCellGrid;
    public GridSlotType selectedGroundType = GridSlotType.None;

    public override void Show()
    {
        base.Show();

        if (LevelDesign.Instance.levelData.gridSlotsData.Count > 0)
        {
            if (groundCellGrid != null)
            {
                foreach (var cell in groundCellGrid)
                {
                    cell.gameObject.Recycle();
                }

                groundCellGrid = null;
            }

            int gridSizeX = LevelDesign.Instance.levelData.gridWidth;
            int gridSizeY = LevelDesign.Instance.levelData.gridHeight;

            gridSizeXInput.text = gridSizeX.ToString();
            gridSizeYInput.text = gridSizeY.ToString();

            GenerateGrid();

            foreach (var data in LevelDesign.Instance.levelData.gridSlotsData)
            {
                groundCellGrid[data.coordinateX, data.coordinateY].SetType(data.gridSlotType);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canPickGround = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canPickGround = false;
    }

    public void OnToggleGrass(bool isOn)
    {
        selectedGroundType = GridSlotType.None;
    } 
    
    public void OnToggleRoad(bool isOn)
    {
        selectedGroundType = GridSlotType.Road;
    }

    public void ActiveScroll(bool active)
    {
        scrollRect.enabled = active;
    }

    public void OnSetGroundType(int x, int y, GridSlotType type)
    {
        int index = -1;
        foreach (var data in LevelDesign.Instance.levelData.gridSlotsData)
        {
            if (data.coordinateX == x && data.coordinateY == y)
            {
                data.gridSlotType = type;
                break;
            }
        }
    }

    public void OnClickButtonGenGrid()
    {
        int xInput = int.Parse(gridSizeXInput.text);
        int yInput = int.Parse(gridSizeYInput.text);

        LevelDesign.Instance.levelData.gridWidth = xInput;
        LevelDesign.Instance.levelData.gridHeight = yInput;
        LevelDesign.Instance.levelData.gridSlotsData.Clear();

        for (int y = 0; y < yInput; y++)
        {
            for (int x = 0; x < xInput; x++)
            {
                GridSlotData gridSlotData = new GridSlotData();
                gridSlotData.coordinateX = x;
                gridSlotData.coordinateY = y;
                gridSlotData.gridSlotType = GridSlotType.None;
                LevelDesign.Instance.levelData.gridSlotsData.Add(gridSlotData);
            }
        }

        if (groundCellGrid != null)
        {
            foreach (var cell in groundCellGrid)
            {
                cell.gameObject.Recycle();
            }

            groundCellGrid = null;
        }

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        int xInput = int.Parse(gridSizeXInput.text);
        int yInput = int.Parse(gridSizeYInput.text);
        gridLayoutGroup.constraintCount = xInput;

        groundCellGrid = new GroundCell[xInput, yInput];

        for (int y = yInput - 1; y >= 0; y--)
        {
            for (int x = 0; x < xInput; x++)
            {
                GameObject obj = groundCellPrefab.Spawn(scrollRect.content);
                obj.transform.localScale = Vector3.one;

                GroundCell groundCell = obj.GetComponent<GroundCell>();
                groundCell.Init(x, y);

                groundCellGrid[x, y] = groundCell;
            }
        }
    }

    public override void Hide()
    {
        base.Hide();
    }
}
