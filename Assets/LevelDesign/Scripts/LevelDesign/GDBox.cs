using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GDBox : MonoBehaviour
{
    public int x;
    public int y;

    public MeshRenderer[] meshRenderers;
    public TextMeshPro txtCapacity;

    public GameObject objLink;

    public GameObject objIce;
    public TextMeshPro txtIceCount;

    public GameObject objCrate;
    public TextMeshPro txtCrateCount;

    public MeshRenderer lockRenderer;
    public MeshRenderer keyRenderer;

    public GameObject objShutter;
    public GameObject objShutterGate;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetUp(BoxData data)
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (data.isHidden)
            {
                meshRenderers[i].material = Resources.Load<Material>(Path.Combine("Materials", "Box", "Inactive", "Hidden"));
                txtCapacity.text = "?";
            }
            else
            {
                meshRenderers[i].material = Resources.Load<Material>(Path.Combine("Materials", "Box", "Active", data.color.ToString()));
                txtCapacity.text = "";
            }
        }

        objIce.SetActive(data.hasIce);
        txtIceCount.text = data.iceCount.ToString();

        objCrate.SetActive(data.hasCrate);
        txtCrateCount.text = data.crateCount.ToString();

        lockRenderer.gameObject.SetActive(data.isLock);
        keyRenderer.gameObject.SetActive(data.isKey);

        lockRenderer.material = Resources.Load<Material>(Path.Combine("Materials", "Locks", data.lockCode.ToString()));
        keyRenderer.material = Resources.Load<Material>(Path.Combine("Materials", "Keys", data.lockCode.ToString()));

        objShutter.SetActive(data.hasShutter);
        objShutterGate.SetActive(!data.isShutterOpen);

        objLink.SetActive(data.isLink);
        if (data.isLink)
        {
            if (x == data.linkedPos.x)
            {
                if (y + 1 == data.linkedPos.y)
                {
                    objLink.transform.localEulerAngles = Vector3.zero;
                }
                else if (y - 1 == data.linkedPos.y)
                {
                    objLink.transform.localEulerAngles = Vector3.forward * 180f;
                }
            }
            else if (y == data.linkedPos.y)
            {
                if (x + 1 == data.linkedPos.x)
                {
                    objLink.transform.localEulerAngles = Vector3.forward * 270f;
                }
                else if (x - 1 == data.linkedPos.x)
                {
                    objLink.transform.localEulerAngles = Vector3.forward * 90f;
                }
            }
        }
    }
}
