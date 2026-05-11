using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleLayer : MonoBehaviour
{
    public ColorEnum color;
    public float scaleFrom;
    public bool activeCap;

    public MeshRenderer wallRenderer;
    public MeshRenderer insideRenderer;
    public MeshRenderer capRenderer;

    public void Init(HoleData data)
    {
        if (data != null)
        {
            SetColor(data.color);
        }
        else
        {
            SetColor(ColorEnum.None);
        }
    }

    public void SetColor(ColorEnum color)
    {
        this.color = color;

        var insideMat = MaterialCache.GetHoleInsideMat(color);
        var mainMat = MaterialCache.GetHoleMainMat(color);

        wallRenderer.sharedMaterial = mainMat;
        insideRenderer.sharedMaterial = insideMat;
        capRenderer.sharedMaterial = insideMat;

        if (color != ColorEnum.None)
        {
            capRenderer.gameObject.SetActive(activeCap);
        }
        else
        {
            capRenderer.gameObject.SetActive(true);
        }
    }

    public void ActiveNextColor(ColorEnum color)
    {
        if (this.color == ColorEnum.None) return;

        SetColor(color);

        transform.localScale = Vector3.one * scaleFrom;
        transform.DOScale(1f, 0.25f).SetEase(Ease.Linear);
    }
}
