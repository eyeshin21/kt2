using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoleDataUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] Button btn;
    [SerializeField] Image image;

    int index;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("ClickHoleDataBtn", ClickBtn);
    }

    void ClickBtn()
    {
        int data = EventManager.GetInt("ClickHoleDataBtn");
        if (data != index)
        {
            image.color = Color.white;
        }
    }

    public void Init(int index)
    {
        this.index = index;
        this.name = (index + 1) + "";
        nameTxt.text = (index + 1) + "";
        Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
        buttonClickedEvent.AddListener(() =>
        {
            image.color = Color.green;
            LevelDesign.Instance.UIHole.SpawnHoleLayerData(index);
            EventManager.EmitEventData("ClickHoleDataBtn", index);
        });

        btn.onClick = buttonClickedEvent;
    }
}
