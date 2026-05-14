using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDHoleLayer : MonoBehaviour
{
    public MeshRenderer wallRenderer;
    public MeshRenderer insideRenderer;
    public MeshRenderer capRenderer;

    public GameObject objHidden;

    public void Init(HoleLayerData data)
    {
        if (data != null)
        {
            SetColor(data.color);
            objHidden.SetActive(data.isHidden);
        }
        else
        {
            SetColor(ColorEnum.None);
            objHidden.SetActive(false);
        }
    }

    public void SetColor(ColorEnum color)
    {
        var insideMat = MaterialCache.GetHoleInsideMat(color);
        var mainMat = MaterialCache.GetHoleMainMat(color);

        wallRenderer.sharedMaterial = mainMat;
        insideRenderer.sharedMaterial = insideMat;
        capRenderer.sharedMaterial = insideMat;
    }
}
