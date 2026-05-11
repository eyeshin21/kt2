using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelDesign : Singleton<LevelDesign>
{
    public LevelData levelData = new LevelData();

    [Header("UI")]
    public UILevelDesign UILevelDesign;
    public UIGround UIGround;
    public UIBox UIBox;
    public UIHole UIHole;

    [Header("Colors")]
    public List<Color> colors;

    private void Start()
    {
        UIGround.Show();
        UIBox.Hide();
        UIHole.Hide();
    }

    public void LoadLevel(int level)
    {
        TextAsset data = Resources.Load<TextAsset>($"LevelData/{level}");
        if (data.text.Contains("gridWidth"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(data.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(data.bytes), "DeoDucDu0cD@u"));
        }

        UIGround.Show();
        UIBox.Hide();
        UIHole.Hide();
    }

    public void SaveLevel(int level)
    {
        string txt = JsonConvert.SerializeObject(levelData);

        string path = Path.Combine(Application.dataPath, "Resources", "LevelData");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path = Path.Combine(path, string.Concat(string.Format("{0}", level), ".json"));
        File.WriteAllText(path, txt);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log(string.Format("Save level {0} success!!!", level));
    }
}

