using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueueHoleUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtIndex;
    [SerializeField] TMP_Dropdown dropdownColor;
    [SerializeField] Toggle toggleHidden;

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
    }

    public void SetUp(HoleData holeData)
    {
        TMP_Dropdown.OptionData optionData = null;
        foreach (var option in dropdownColor.options)
        {
            if (option.text.Equals(holeData.color.ToString()))
            {
                optionData = option;
                break;
            }
        }

        dropdownColor.value = dropdownColor.options.IndexOf(optionData);
        toggleHidden.isOn = holeData.isHidden;
    }

    public void OnValueChangeDropdownColor(int value)
    {
        if (Enum.TryParse(dropdownColor.options[value].text, out ColorEnum color))
        {
            LevelDesign.Instance.levelData.queueHoles[index].color = color;
            LevelDesign.Instance.UIHole.UpdateTotalColors();
        }
    }

    public void OnValueChangeToggleHidden(bool isOn)
    {
        LevelDesign.Instance.levelData.queueHoles[index].isHidden = isOn;
        LevelDesign.Instance.UIHole.UpdateTotalColors();
    }

    public void OnClickButtonDelete()
    {
        LevelDesign.Instance.UIHole.RemoveQueueHole(index);
        gameObject.Recycle();
    }
}
