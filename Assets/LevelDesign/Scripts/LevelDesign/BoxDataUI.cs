using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoxDataUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtIndex;
    [SerializeField] TMP_Dropdown dropdownColor;

    int index;

    public void Init(int index)
    {
        this.index = index;
        txtIndex.text = (index + 1).ToString();

        ColorEnum[] colorEnums = (ColorEnum[])Enum.GetValues(typeof(ColorEnum));

        dropdownColor.options.Clear();
        for (int i = 0; i < colorEnums.Length; i++)
        {
            dropdownColor.options.Add(new TMP_Dropdown.OptionData(colorEnums[i].ToString()));
        }

        //dropdownColor.value = 0;
        //dropdownColor.RefreshShownValue();
    }

    public void SetUp(BoxData data)
    {
        TMP_Dropdown.OptionData optionData = null;
        foreach (var option in dropdownColor.options)
        {
            if (option.text.Equals(data.color.ToString()))
            {
                optionData = option;
                break;
            }
        }
        dropdownColor.value = dropdownColor.options.IndexOf(optionData);
    }

    public ColorEnum GetColor()
    {
        return Enum.Parse<ColorEnum>(dropdownColor.options[dropdownColor.value].text);
    }

    public void OnValueChangeDropdownColor(int value)
    {
        if (LevelDesign.Instance.UIBox.selectedTile != null)
        {
            if (LevelDesign.Instance.UIBox.selectedTile.tunnel != null)
            {
                foreach (var data in LevelDesign.Instance.levelData.tunnelsData)
                {
                    if (data.coordinateX == LevelDesign.Instance.UIBox.selectedTile.x && data.coordinateY == LevelDesign.Instance.UIBox.selectedTile.y)
                    {
                        if (Enum.TryParse(dropdownColor.options[value].text, out ColorEnum color))
                        {
                            data.boxesData[index].color = color;
                        }
                        break;
                    }
                }
            }
        }

        LevelDesign.Instance.UIBox.UpdateTotalColor();
    }

    public void OnClickButtonDelete()
    {
        LevelDesign.Instance.UIBox.RemoveBoxData(index);
        gameObject.Recycle();
    }
}
