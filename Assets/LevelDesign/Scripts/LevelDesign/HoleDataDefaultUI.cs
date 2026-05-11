using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoleDataDefaultUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtIndex;
    [SerializeField] TMP_Dropdown dropdownFirstLayerColor;
    [SerializeField] TMP_Dropdown dropdownSecondLayerColor;
    [SerializeField] TMP_Dropdown dropdownThirdLayerColor;

    [SerializeField] Toggle toggleHiddenFirstLayer;
    [SerializeField] Toggle toggleHiddenSecondLayer;
    [SerializeField] Toggle toggleHiddenThirdLayer;

    int index;

    public void Init(int index)
    {
        this.index = index;
        txtIndex.text = (index + 1).ToString();

        ColorEnum[] colorEnums = (ColorEnum[])Enum.GetValues(typeof(ColorEnum));

        dropdownFirstLayerColor.options.Clear();
        dropdownSecondLayerColor.options.Clear();
        dropdownThirdLayerColor.options.Clear();

        for (int i = 0; i < colorEnums.Length; i++)
        {
            dropdownFirstLayerColor.options.Add(new TMP_Dropdown.OptionData(colorEnums[i].ToString()));
            dropdownSecondLayerColor.options.Add(new TMP_Dropdown.OptionData(colorEnums[i].ToString()));
            dropdownThirdLayerColor.options.Add(new TMP_Dropdown.OptionData(colorEnums[i].ToString()));
        }
    }

    public void SetUp(HoleDataDefault data)
    {
        TMP_Dropdown.OptionData optionFirstData = null;
        foreach (var option in dropdownFirstLayerColor.options)
        {
            if (option.text.Equals(data.firstLayerHole.color.ToString()))
            {
                optionFirstData = option;
                break;
            }
        }

        TMP_Dropdown.OptionData optionSecondData = null;
        foreach (var option in dropdownSecondLayerColor.options)
        {
            if (option.text.Equals(data.secondLayerHole.color.ToString()))
            {
                optionSecondData = option;
                break;
            }
        }

        TMP_Dropdown.OptionData optionThirdData = null;
        foreach (var option in dropdownThirdLayerColor.options)
        {
            if (option.text.Equals(data.thirdLayerHole.color.ToString()))
            {
                optionThirdData = option;
                break;
            }
        }

        dropdownFirstLayerColor.value = dropdownFirstLayerColor.options.IndexOf(optionFirstData);
        dropdownSecondLayerColor.value = dropdownSecondLayerColor.options.IndexOf(optionSecondData);
        dropdownThirdLayerColor.value = dropdownThirdLayerColor.options.IndexOf(optionThirdData);

        toggleHiddenFirstLayer.isOn = data.firstLayerHole.isHidden;
        toggleHiddenSecondLayer.isOn = data.secondLayerHole.isHidden;
        toggleHiddenThirdLayer.isOn = data.thirdLayerHole.isHidden;
    }

    public void OnValueChangeDropdownFirstLayerColor(int value)
    {
        if (Enum.TryParse(dropdownFirstLayerColor.options[value].text, out ColorEnum color))
        {
            LevelDesign.Instance.levelData.holesDataDefault[index].firstLayerHole.color = color;
            LevelDesign.Instance.UIHole.UpdateTotalColors();
        }
    }

    public void OnValueChangeDropdownSecondLayerColor(int value)
    {
        if (Enum.TryParse(dropdownSecondLayerColor.options[value].text, out ColorEnum color))
        {
            LevelDesign.Instance.levelData.holesDataDefault[index].secondLayerHole.color = color;
            LevelDesign.Instance.UIHole.UpdateTotalColors();
        }
    }

    public void OnValueChangeDropdownThirdLayerColor(int value)
    {
        if (Enum.TryParse(dropdownThirdLayerColor.options[value].text, out ColorEnum color))
        {
            LevelDesign.Instance.levelData.holesDataDefault[index].thirdLayerHole.color = color;
            LevelDesign.Instance.UIHole.UpdateTotalColors();
        }
    }

    public void OnValueChangeToggleHiddenFirstLayer(bool isOn)
    {
        LevelDesign.Instance.levelData.holesDataDefault[index].firstLayerHole.isHidden = isOn;
        LevelDesign.Instance.UIHole.UpdateTotalColors();
    }

    public void OnValueChangeToggleHiddenSecondLayer(bool isOn)
    {
        LevelDesign.Instance.levelData.holesDataDefault[index].secondLayerHole.isHidden = isOn;
        LevelDesign.Instance.UIHole.UpdateTotalColors();
    }

    public void OnValueChangeToggleHiddenThirdLayer(bool isOn)
    {
        LevelDesign.Instance.levelData.holesDataDefault[index].thirdLayerHole.isHidden = isOn;
        LevelDesign.Instance.UIHole.UpdateTotalColors();
    }

    public void OnClickButtonDelete()
    {
        LevelDesign.Instance.UIHole.RemoveHoleDataDefault(index);
        gameObject.Recycle();
    }
}
