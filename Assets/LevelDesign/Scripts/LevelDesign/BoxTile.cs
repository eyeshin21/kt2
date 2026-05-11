using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoxTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public int x;
    public int y;
    public Image img;

    [Header("Box")]
    public GameObject boxPrefab;
    public GDBox box;

    [Header("Tunnel")]
    public GameObject tunnelPrefab;
    public GDTunnel tunnel;

    [Header("Pin")]
    public GameObject pinPrefab;
    public GDPin pin;

    [Header("Cloth")]
    public GameObject clothPrefab;
    public GDCloth cloth;

    [Header("Lock Chain")]
    public GameObject lockChainPrefab;
    public GDLockChain lockChain;

    private void Start()
    {
        EventManager.StartListening("OnSelectTile", Select);
    }

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void ActiveTile(bool active)
    {
        img.enabled = active;
    }

    public void Select()
    {
        EventManager.DataGroup[] data = EventManager.GetDataGroup("OnSelectTile");

        switch (LevelDesign.Instance.UILevelDesign.currentTab)
        {
            case Tabs.Box:
                if (x == data[0].ToInt() && y == data[1].ToInt())
                {
                    img.color = Color.green;
                }
                else
                {
                    img.color = Color.white;
                }
                break;
        }
    }

    public void Added()
    {
        img.color = Color.green;
    }

    public void SpawnBox()
    {
        GameObject obj = boxPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 100f;

        box = obj.GetComponent<GDBox>();
        box.Init(x, y);
    }

    public void SpawnTunnel()
    {
        GameObject obj = tunnelPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 80f;

        tunnel = obj.GetComponent<GDTunnel>();
        tunnel.Init(x, y);
    }

    public void SpawnPin()
    {
        GameObject obj = pinPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 100f;

        pin = obj.GetComponent<GDPin>();
        pin.Init(x, y);
    }

    public void SpawnCloth()
    {
        GameObject obj = clothPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 100f;

        cloth = obj.GetComponent<GDCloth>();
        cloth.Init(x, y);
    }

    public void SpawnLockChain()
    {
        GameObject obj = lockChainPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 85f;

        lockChain = obj.GetComponent<GDLockChain>();
        lockChain.Init(x, y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (LevelDesign.Instance.UILevelDesign.currentTab)
        {
            case Tabs.Box:
                LevelDesign.Instance.UIBox.canPickTile = true;
                LevelDesign.Instance.UIBox.OnSelectTile(x, y);
                LevelDesign.Instance.UIBox.ActiveScroll(false);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (LevelDesign.Instance.UILevelDesign.currentTab)
        {
            case Tabs.Box:
                if (LevelDesign.Instance.UIBox.canPickTile)
                {
                    LevelDesign.Instance.UIBox.OnSelectTile(x, y);
                }
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (LevelDesign.Instance.UILevelDesign.currentTab)
        {
            case Tabs.Box:
                LevelDesign.Instance.UIBox.canPickTile = false;
                LevelDesign.Instance.UIBox.ActiveScroll(true);
                break;
        }
    }
}
