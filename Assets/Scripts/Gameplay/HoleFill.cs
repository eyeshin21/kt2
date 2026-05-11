using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleFill : MonoBehaviour
{
    public Transform[] fillParts;
    public MeshRenderer[] fillRenderers;

    public void Init(ColorEnum color)
    {
        var mat = MaterialCache.GetHoleMainMat(color);
        for (int i = 0; i < fillRenderers.Length; i++)
        {
            fillRenderers[i].sharedMaterial = mat;
        }

        for (int i = 0; i < fillParts.Length; i++)
        {
            fillParts[i].localScale = new Vector3(0, 1, 1);
        }
    }

    public void Fill(Action onDone)
    {
        StartCoroutine(IE_Fill(onDone));
    }

    IEnumerator IE_Fill(Action onDone)
    {
        float duration = 0.25f;
        float durationEachPart = duration / fillParts.Length;
        WaitForSeconds waitForFill = new WaitForSeconds(durationEachPart);

        for (int i = 0; i < fillParts.Length; i++)
        {
            fillParts[i].DOScaleX(1f, durationEachPart).SetEase(Ease.Linear);
            yield return waitForFill;
        }

        onDone.Invoke();
    }
}
