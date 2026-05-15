using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleLayer : MonoBehaviour
{
    public ColorEnum color;
    public bool isHidden;
    public float scaleFrom;
    public bool activeCap;

    public GameObject objHidden;
    public MeshRenderer wallRenderer;
    public MeshRenderer insideRenderer;
    public MeshRenderer capRenderer;

    public void Init(HoleLayerData data)
    {
        if (data != null)
        {
            isHidden = data.isHidden;
            SetColor(data.color);
        }
        else
        {
            isHidden = false;
            SetColor(ColorEnum.None);
        }
    }

    public void SetColor(ColorEnum color)
    {
        this.color = color;
        objHidden.SetActive(isHidden);
        objHidden.transform.eulerAngles = new Vector3(90f, 0f, 0f);

        Material insideMat;
        Material mainMat;

        if (!isHidden)
        {
            insideMat = MaterialCache.GetHoleInsideMat(color);
            mainMat = MaterialCache.GetHoleMainMat(color);
        }
        else
        {
            insideMat = MaterialCache.GetHoleInsideHiddenMat();
            mainMat = MaterialCache.GetHoleMainHiddenMat();
        }

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

    public void ActiveNextColor(ColorEnum color, bool isHidden, System.Action onComplete = null)
    {
        if (this.color == ColorEnum.None) return;

        this.isHidden = isHidden;

        SetColor(color);

        transform.localScale = Vector3.one * scaleFrom;
        transform.DOScale(1f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void Recycle()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
    }
}
