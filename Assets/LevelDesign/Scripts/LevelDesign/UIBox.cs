using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBox : UI, IPointerDownHandler, IPointerUpHandler
{
    public bool canPickTile = false;

    [Header("UI")]
    public GameObject btnCreate;
    public GameObject btnRemove;
    public TextMeshProUGUI txtTotalColor;
    public TextMeshProUGUI txtColorCount;

    [Header("Element")]
    public TMP_Dropdown dropdownElement;

    [Header("Element Box")]
    public GameObject elementBox;
    public TMP_Dropdown dropdownBoxColor;

    public Toggle toggleBoxHidden;

    public Toggle toggleHasIce;
    public TMP_InputField inputIceCount;

    public TMP_InputField inputLinkedPosX;
    public TMP_InputField inputLinkedPosY;
    public TextMeshProUGUI txtCurrentPos;

    public Toggle toggleHasCrate;
    public TMP_InputField inputCrateCount;

    public Toggle toggleHasLock;
    public Toggle toggleHasKey;
    public TMP_InputField inputLockCode;

    public Toggle toggleHasShutter;
    public Toggle toggleIsShutterOpen;

    [Header("Element Tunnel")]
    public GameObject elementTunnel;
    public TMP_Dropdown dropdownTunnelDirection;
    public ScrollRect scrollBoxData;
    public GameObject boxDataUIPrefab;
    public List<BoxDataUI> boxDataUIs = new List<BoxDataUI>();
    public GameObject btnAddBoxData;

    [Header("Element Pin")]
    public GameObject elementPin;
    public TMP_Dropdown dropdownPinDirection;

    [Header("Element Cloth")]
    public GameObject elementCloth;
    public TMP_Dropdown dropdownClothType;
    public TMP_InputField inputClothCount;

    [Header("Element Lock Chain")]
    public GameObject elementLockChain;
    public TMP_Dropdown dropdownLockChainAxis;
    public TMP_InputField inputLockChainLength;
    public TMP_InputField inputLockChainCode;

    [Header("Grid")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GridLayoutGroup gridLayoutGroup;

    [Header("Cell")]
    public GameObject boxTilePrefab;
    public BoxTile[,] boxTileGrid;
    public BoxTile selectedTile;

    public override void Start()
    {
        base.Start();

        ColorEnum[] colorEnums = (ColorEnum[])System.Enum.GetValues(typeof(ColorEnum));

        dropdownBoxColor.options.Clear();
        for (int i = 0; i < colorEnums.Length; i++)
        {
            dropdownBoxColor.options.Add(new TMP_Dropdown.OptionData(colorEnums[i].ToString()));
        }

        dropdownBoxColor.value = 0;
        dropdownBoxColor.RefreshShownValue();

        btnCreate.SetActive(false);
        btnRemove.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        foreach (var boxDataUI in boxDataUIs)
        {
            boxDataUI.gameObject.Recycle();
        }
        boxDataUIs.Clear();

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

        UpdateTotalColor();
    }

    public void UpdateTotalColor()
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

        txtColorCount.text = txt;

        txtTotalColor.text = $"Total Color: {allColors.Count}";
    }

    public void ResetSelectedTile()
    {
        if (selectedTile != null)
        {
            EventManager.SetDataGroup("OnSelectTile", -1, -1);
            EventManager.EmitEvent("OnSelectTile");
            selectedTile = null;

            txtCurrentPos.text = $"Current Pos:";

            btnCreate.SetActive(false);
            btnRemove.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canPickTile = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canPickTile = false;
    }

    public void ActiveScroll(bool active)
    {
        scrollRect.enabled = active;
    }

    public void OnSelectTile(int x, int y)
    {
        EventManager.SetDataGroup("OnSelectTile", x, y);
        EventManager.EmitEvent("OnSelectTile");

        selectedTile = boxTileGrid[x, y];

        txtCurrentPos.text = $"Current Pos: {x}x{y}";

        foreach (var dataUI in boxDataUIs)
        {
            dataUI.gameObject.Recycle();
        }
        boxDataUIs.Clear();

        BoxData boxData = null;
        foreach (var data in LevelDesign.Instance.levelData.boxesData)
        {
            if (data.coordinateX == x && data.coordinateY == y)
            {
                boxData = data;
                break;
            }
        }

        TunnelData tunnelData = null;
        foreach (var data in LevelDesign.Instance.levelData.tunnelsData)
        {
            if (data.coordinateX == x && data.coordinateY == y)
            {
                tunnelData = data;
                break;
            }
        }

        if (boxData != null)
        {
            if (dropdownElement.value == 1)
            {
                dropdownElement.value = 0;
            }

            TMP_Dropdown.OptionData optionData = null;
            foreach (var option in dropdownBoxColor.options)
            {
                if (option.text.Equals(boxData.color.ToString()))
                {
                    optionData = option;
                }
            }

            dropdownBoxColor.value = dropdownBoxColor.options.IndexOf(optionData);
            toggleBoxHidden.isOn = boxData.isHidden;
            toggleHasIce.isOn = boxData.hasIce;
            inputIceCount.text = boxData.iceCount.ToString();
            toggleHasCrate.isOn = boxData.hasCrate;
            inputCrateCount.text = boxData.crateCount.ToString();
            toggleHasKey.isOn = boxData.isKey;
            toggleHasLock.isOn = boxData.isLock;
            inputLockCode.text = boxData.lockCode.ToString();
            toggleHasShutter.isOn = boxData.hasShutter;
            toggleIsShutterOpen.isOn = boxData.isShutterOpen;

            if (boxData.isLink)
            {
                inputLinkedPosX.text = boxData.linkedPos.x.ToString();
                inputLinkedPosY.text = boxData.linkedPos.y.ToString();
            }
            else
            {
                inputLinkedPosX.text = string.Empty;
                inputLinkedPosY.text = string.Empty;
            }
        }
        else if (tunnelData != null)
        {
            dropdownElement.value = 1;

            dropdownTunnelDirection.value = (int)tunnelData.direction;
            foreach (var data in tunnelData.boxesData)
            {
                int index = boxDataUIs.Count;

                GameObject obj = boxDataUIPrefab.Spawn(scrollBoxData.content);
                obj.transform.localScale = Vector3.one;

                BoxDataUI boxDataUI = obj.GetComponent<BoxDataUI>();
                boxDataUI.Init(index);
                boxDataUI.SetUp(data);

                boxDataUIs.Add(boxDataUI);

                btnAddBoxData.transform.parent.SetAsLastSibling();
            }
        }

        CheckActiveCreateBtn(x, y);
    }

    public void CheckActiveCreateBtn(int x, int y)
    {
        bool activeCreateBtn = true;
        switch (dropdownElement.value)
        {
            case 0:
                {
                    foreach (var boxData in LevelDesign.Instance.levelData.boxesData)
                    {
                        if (boxData.coordinateX == x && boxData.coordinateY == y)
                        {
                            activeCreateBtn = false;
                            break;
                        }
                    }
                    break;
                }
            case 1:
                {
                    foreach (var tunnelData in LevelDesign.Instance.levelData.tunnelsData)
                    {
                        if (tunnelData.coordinateX == x && tunnelData.coordinateY == y)
                        {
                            activeCreateBtn = false;
                            break;
                        }
                    }
                    break;
                }
            case 2: // Pin
                {
                    foreach (var pinData in LevelDesign.Instance.levelData.pinsData)
                    {
                        if (pinData.coordinateX == x && pinData.coordinateY == y)
                        {
                            dropdownPinDirection.value = (int)pinData.direction;
                            activeCreateBtn = false;
                            break;
                        }
                    }
                    break;
                }
            case 3: // Cloth
                {
                    foreach (var clothData in LevelDesign.Instance.levelData.clothsData)
                    {
                        if (clothData.coordinateX == x && clothData.coordinateY == y)
                        {
                            dropdownClothType.value = (int)clothData.clothType;
                            inputClothCount.text = clothData.clothCount.ToString();
                            activeCreateBtn = false;
                            break;
                        }
                    }
                    break;
                }
            case 4: // Lock Chain
                {
                    foreach (var lockChainData in LevelDesign.Instance.levelData.lockChainsData)
                    {
                        if (lockChainData.coordinateX == x && lockChainData.coordinateY == y)
                        {
                            dropdownLockChainAxis.value = (int)lockChainData.lockChainAxis;
                            inputLockChainLength.text = lockChainData.lockChainLength.ToString();
                            inputLockChainCode.text = lockChainData.lockChainCode.ToString();
                            activeCreateBtn = false;
                            break;
                        }
                    }
                    break;
                }
        }

        btnCreate.SetActive(activeCreateBtn);
        btnRemove.SetActive(!activeCreateBtn);
    }

    public void OnClickButtonLink()
    {
        int linkedPosX = int.Parse(inputLinkedPosX.text);
        int linkedPosY = int.Parse(inputLinkedPosY.text);

        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.isLink = true;
                    data.linkedPos = new Vector2IntS(linkedPosX, linkedPosY);
                    selectedTile.box.SetUp(data);
                }

                if (data.coordinateX == linkedPosX && data.coordinateY == linkedPosY)
                {
                    data.isLink = true;
                    data.linkedPos = new Vector2IntS(selectedTile.x, selectedTile.y);
                    boxTileGrid[linkedPosX, linkedPosY].box.SetUp(data);
                }
            }
        }
    }

    public void OnClickButtonUnlink()
    {
        int linkedPosX = int.Parse(inputLinkedPosX.text);
        int linkedPosY = int.Parse(inputLinkedPosY.text);

        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.isLink = false;
                    data.linkedPos = new Vector2IntS();
                    selectedTile.box.SetUp(data);
                }

                if (data.coordinateX == linkedPosX && data.coordinateY == linkedPosY)
                {
                    data.isLink = false;
                    data.linkedPos = new Vector2IntS();
                    boxTileGrid[linkedPosX, linkedPosY].box.SetUp(data);
                }
            }
        }
    }

    public void OnDropdownElementValueChange(int option)
    {
        elementBox.SetActive(option == 0);
        elementTunnel.SetActive(option == 1);
        elementPin.SetActive(option == 2);
        elementCloth.SetActive(option == 3);
        elementLockChain.SetActive(option == 4);

        if (selectedTile != null)
        {
            switch (option)
            {
                case 2: // Pin
                    {
                        bool activeCreateBtn = true;
                        foreach (var data in LevelDesign.Instance.levelData.pinsData)
                        {
                            if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                            {
                                dropdownPinDirection.value = (int)data.direction;
                                activeCreateBtn = false;
                                break;
                            }
                        }

                        btnCreate.SetActive(activeCreateBtn);
                        btnRemove.SetActive(!activeCreateBtn);
                        break;
                    }
                case 3: // Cloth
                    {
                        bool activeCreateBtn = true;
                        foreach (var data in LevelDesign.Instance.levelData.clothsData)
                        {
                            if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                            {
                                dropdownClothType.value = (int)data.clothType;
                                inputClothCount.text = data.clothCount.ToString();
                                activeCreateBtn = false;
                                break;
                            }
                        }

                        btnCreate.SetActive(activeCreateBtn);
                        btnRemove.SetActive(!activeCreateBtn);
                        break;
                    }
                case 4: // Lock Chain
                    {
                        bool activeCreateBtn = true;
                        foreach (var data in LevelDesign.Instance.levelData.lockChainsData)
                        {
                            if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                            {
                                if (data.lockChainAxis != Axis.None)
                                {
                                    dropdownLockChainAxis.value = (int)data.lockChainAxis;
                                    inputLockChainLength.text = data.lockChainLength.ToString();
                                    inputLockChainCode.text = data.lockChainCode.ToString();

                                    activeCreateBtn = false;
                                }
                                break;
                            }
                        }

                        btnCreate.SetActive(activeCreateBtn);
                        btnRemove.SetActive(!activeCreateBtn);
                        break;
                    }
            }
        }
    }

    public void OnToggleHidden(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.isHidden = isOn;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnToggleHasIce(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.hasIce = isOn;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnInputIceCountValueChange(string value)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    int iceCount = int.Parse(value);
                    data.iceCount = iceCount;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnToggleHasCrate(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.hasCrate = isOn;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnInputCrateCountValueChange(string value)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    int crateCount = int.Parse(value);
                    data.crateCount = crateCount;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnToggleHasLock(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.isLock = isOn;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnToggleHasKey(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.isKey = isOn;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnInputLockCodeValueChange(string value)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    int lockCode = int.Parse(value);
                    data.lockCode = lockCode;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnToggleHasShutter(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.hasShutter = isOn;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnToggleIsShutterOpen(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.isShutterOpen = isOn;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnDropdownBoxColorValueChange(int option)
    {
        if (option == 0) return;

        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.boxesData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    ColorEnum color = System.Enum.Parse<ColorEnum>(dropdownBoxColor.options[option].text);
                    data.color = color;
                    selectedTile.box.SetUp(data);
                    break;
                }
            }

            UpdateTotalColor();
        }
    }

    public void OnDropdownValueChangeTunnelDirection(int option)
    {
        if (option == 0) return;

        if (selectedTile != null && selectedTile.tunnel != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.tunnelsData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.direction = (Direction)option;
                    selectedTile.tunnel.SetDirection((Direction)option);
                    break;
                }
            }
        }
    }

    public void OnDropdownValueChangePinDirection(int option)
    {
        if (option == 0) return;

        if (selectedTile != null && selectedTile.pin != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.pinsData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.direction = (Direction)option;
                    selectedTile.pin.SetDirection((Direction)option);
                    break;
                }
            }
        }
    }

    public void OnDropdownValueChangeClothType(int option)
    {
        if (option == 0) return;

        if (selectedTile != null && selectedTile.cloth != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.clothsData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.clothType = (ClothType)option;
                    selectedTile.cloth.SetClothType((ClothType)option);
                    break;
                }
            }
        }
    }

    public void OnInputClothCountValueChange(string value)
    {
        if (selectedTile != null && selectedTile.cloth != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.clothsData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.clothCount = int.Parse(value);
                    selectedTile.cloth.SetClothCount(int.Parse(value));
                    break;
                }
            }
        }
    }

    public void OnDropdownValueChangeLockChainAxis(int option)
    {
        if (option == 0) return;

        if (selectedTile != null && selectedTile.lockChain != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.lockChainsData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.lockChainAxis = (Axis)option;
                    selectedTile.lockChain.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnInputLockChainLengthValueChange(string value)
    {
        if (selectedTile != null && selectedTile.lockChain != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.lockChainsData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.lockChainLength = int.Parse(value);
                    selectedTile.lockChain.SetUp(data);
                    break;
                }
            }
        }
    }

    public void OnInputLockChainCodeValueChange(string value)
    {
        if (selectedTile != null && selectedTile.lockChain != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.lockChainsData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.lockChainCode = int.Parse(value);
                    selectedTile.lockChain.SetLockCode(int.Parse(value));
                    break;
                }
            }
        }
    }

    public void OnClickButtonCreate()
    {
        if (selectedTile != null)
        {
            switch (dropdownElement.value)
            {
                case 0: // Shooter
                    {
                        selectedTile.SpawnBox();

                        BoxData boxData = new BoxData();
                        boxData.coordinateX = selectedTile.x;
                        boxData.coordinateY = selectedTile.y;
                        boxData.color = System.Enum.Parse<ColorEnum>(dropdownBoxColor.options[dropdownBoxColor.value].text);
                        boxData.isHidden = toggleBoxHidden.isOn;
                        boxData.hasIce = toggleHasIce.isOn;
                        boxData.iceCount = int.Parse(inputIceCount.text);
                        boxData.hasCrate = toggleHasCrate.isOn;
                        boxData.crateCount = int.Parse(inputCrateCount.text);
                        boxData.isKey = toggleHasKey.isOn;
                        boxData.isLock = toggleHasLock.isOn;
                        boxData.lockCode = int.Parse(inputLockCode.text);
                        boxData.hasShutter = toggleHasShutter.isOn;
                        boxData.isShutterOpen = toggleIsShutterOpen.isOn;

                        selectedTile.box.SetUp(boxData);

                        LevelDesign.Instance.levelData.boxesData.Add(boxData);
                        break;
                    }
                case 1: // Tunnel
                    {
                        selectedTile.SpawnTunnel();

                        TunnelData tunnelData = new TunnelData();
                        tunnelData.coordinateX = selectedTile.x;
                        tunnelData.coordinateY = selectedTile.y;
                        tunnelData.direction = (Direction)dropdownTunnelDirection.value;

                        selectedTile.tunnel.SetUp(tunnelData);

                        LevelDesign.Instance.levelData.tunnelsData.Add(tunnelData);
                        break;
                    }
                case 2: // Pin
                    {
                        selectedTile.SpawnPin();

                        PinData pinData = new PinData();
                        pinData.coordinateX = selectedTile.x;
                        pinData.coordinateY = selectedTile.y;
                        pinData.direction = (Direction)dropdownPinDirection.value;

                        selectedTile.pin.SetUp(pinData);

                        LevelDesign.Instance.levelData.pinsData.Add(pinData);
                        break;
                    }
                case 3: // Cloth
                    {
                        selectedTile.SpawnCloth();

                        ClothData clothData = new ClothData();
                        clothData.coordinateX = selectedTile.x;
                        clothData.coordinateY = selectedTile.y;
                        clothData.clothType = (ClothType)dropdownClothType.value;
                        clothData.clothCount = int.Parse(inputClothCount.text);

                        selectedTile.cloth.SetUp(clothData);

                        LevelDesign.Instance.levelData.clothsData.Add(clothData);
                        break;
                    }
                case 4: // Lock Chain
                    {
                        selectedTile.SpawnLockChain();

                        LockChainData lockChainData = new LockChainData();
                        lockChainData.lockChainAxis = (Axis)dropdownLockChainAxis.value;
                        lockChainData.lockChainLength = int.Parse(inputLockChainLength.text);
                        lockChainData.lockChainCode = int.Parse(inputLockChainCode.text);

                        selectedTile.lockChain.SetUp(lockChainData);

                        LevelDesign.Instance.levelData.lockChainsData.Add(lockChainData);
                        break;
                    }
            }

            btnCreate.SetActive(false);
            btnRemove.SetActive(true);

            UpdateTotalColor();
        }
    }

    public void OnClickButtonRemove()
    {
        if (selectedTile != null)
        {
            switch (dropdownElement.value)
            {
                case 0: // Shooter
                    {
                        BoxData dataToRemove = null;

                        foreach (var data in LevelDesign.Instance.levelData.boxesData)
                        {
                            if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                            {
                                dataToRemove = data;
                                break;
                            }
                        }

                        if (dataToRemove != null)
                        {
                            LevelDesign.Instance.levelData.boxesData.Remove(dataToRemove);

                            if (selectedTile.box != null)
                            {
                                selectedTile.box.gameObject.Recycle();
                            }

                            btnRemove.SetActive(false);
                            btnCreate.SetActive(true);
                        }
                        break;
                    }
                case 1: // Tunnel
                    {
                        TunnelData dataToRemove = null;
                        foreach (var data in LevelDesign.Instance.levelData.tunnelsData)
                        {
                            if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                            {
                                dataToRemove = data;
                                break;
                            }
                        }

                        if (dataToRemove != null)
                        {
                            LevelDesign.Instance.levelData.tunnelsData.Remove(dataToRemove);

                            if (selectedTile.tunnel != null)
                            {
                                selectedTile.tunnel.gameObject.Recycle();
                            }

                            btnRemove.SetActive(false);
                            btnCreate.SetActive(true);
                        }
                        break;
                    }
                case 2: // Pin
                    {
                        PinData dataToRemove = null;
                        foreach (var data in LevelDesign.Instance.levelData.pinsData)
                        {
                            if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                            {
                                dataToRemove = data;
                                break;
                            }
                        }

                        if (dataToRemove != null)
                        {
                            LevelDesign.Instance.levelData.pinsData.Remove(dataToRemove);

                            if (selectedTile.pin != null)
                            {
                                selectedTile.pin.gameObject.Recycle();
                            }

                            btnRemove.SetActive(false);
                            btnCreate.SetActive(true);
                        }
                        break;
                    }
                case 3: // Cloth
                    {
                        ClothData dataToRemove = null;
                        foreach (var data in LevelDesign.Instance.levelData.clothsData)
                        {
                            if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                            {
                                dataToRemove = data;
                                break;
                            }
                        }

                        if (dataToRemove != null)
                        {
                            LevelDesign.Instance.levelData.clothsData.Remove(dataToRemove);

                            if (selectedTile.cloth != null)
                            {
                                selectedTile.cloth.gameObject.Recycle();
                            }

                            btnRemove.SetActive(false);
                            btnCreate.SetActive(true);
                        }
                        break;
                    }
                case 4: // Lock Chain
                    {
                        LockChainData dataToRemove = null;
                        foreach (var data in LevelDesign.Instance.levelData.lockChainsData)
                        {
                            if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                            {
                                dataToRemove = data;
                                break;
                            }
                        }

                        if (dataToRemove != null)
                        {
                            LevelDesign.Instance.levelData.lockChainsData.Remove(dataToRemove);

                            if (selectedTile.lockChain != null)
                            {
                                selectedTile.lockChain.gameObject.Recycle();
                            }

                            btnRemove.SetActive(false);
                            btnCreate.SetActive(true);
                        }
                        break;
                    }
            }

            UpdateTotalColor();
        }
    }

    public void OnClickButtonAddBoxData()
    {
        int index = boxDataUIs.Count;

        GameObject obj = boxDataUIPrefab.Spawn(scrollBoxData.content);
        obj.transform.localScale = Vector3.one;

        BoxData boxData = new BoxData();

        BoxDataUI boxDataUI = obj.GetComponent<BoxDataUI>();
        boxDataUIs.Add(boxDataUI);

        btnAddBoxData.transform.parent.SetAsLastSibling();

        if (selectedTile != null && selectedTile.tunnel != null)
        {
            selectedTile.tunnel.SetTotalBox(boxDataUIs.Count);
            foreach (var data in LevelDesign.Instance.levelData.tunnelsData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.boxesData.Add(boxData);
                    break;
                }
            }
        }

        boxDataUI.Init(index);
        boxDataUI.SetUp(boxData);

        UpdateTotalColor();
    }

    public void RemoveBoxData(int index)
    {
        boxDataUIs.RemoveAt(index);

        if (selectedTile != null && selectedTile.tunnel != null)
        {
            selectedTile.tunnel.SetTotalBox(boxDataUIs.Count);

            foreach (var data in LevelDesign.Instance.levelData.tunnelsData)
            {
                if (data.coordinateX == selectedTile.x && data.coordinateY == selectedTile.y)
                {
                    data.boxesData.RemoveAt(index);
                    break;
                }
            }
        }

        for (int i = 0; i < boxDataUIs.Count; i++)
        {
            boxDataUIs[i].Init(i);
        }

        UpdateTotalColor();
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

public enum ElementType
{
    Box = 0,
    Tunnel = 1,
    Link = 2
}