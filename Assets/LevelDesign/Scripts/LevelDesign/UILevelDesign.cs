using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelDesign : MonoBehaviour
{
    public TMP_InputField levelInput;

    [Header("Tab")]
    public Tabs currentTab;
    public Toggle toggleTabGround;
    
    public void OnToggleTabGround(bool isOn)
    {
        if (isOn)
        {
            if (currentTab != Tabs.Ground)
            {
                currentTab = Tabs.Ground;
                LevelDesign.Instance.UIGround.Show();
            }
        }
        else
        {
            LevelDesign.Instance.UIGround.Hide();
        }
    }
    
    public void OnToggleTabBox(bool isOn)
    {
        if (isOn)
        {
            if (currentTab != Tabs.Box)
            {
                currentTab = Tabs.Box;
                LevelDesign.Instance.UIBox.Show();
            }
        }
        else
        {
            LevelDesign.Instance.UIBox.Hide();
        }
    }

    public void OnToggleTabHole(bool isOn)
    {
        if (isOn)
        {
            if (currentTab != Tabs.Hole)
            {
                currentTab = Tabs.Hole;
                LevelDesign.Instance.UIHole.Show();
            }
        }
        else
        {
            LevelDesign.Instance.UIHole.Hide();
        }
    }

    public void OnClickButtonLoad()
    {
        if (int.TryParse(levelInput.text, out int level))
        {
            LevelDesign.Instance.LoadLevel(level);
            toggleTabGround.isOn = true;
        }
    }

    public void OnClickButtonSave()
    {
        if (int.TryParse(levelInput.text, out int level))
        {
            LevelDesign.Instance.SaveLevel(level);
        }
    }
}

public enum Tabs
{
    Ground = 0,
    Box = 1,
    Hole = 2
}