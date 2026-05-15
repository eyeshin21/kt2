using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

public class HoleBreakEffect : MonoBehaviour
{
    public Rigidbody[] rbParts;
    public Transform[] explosionPoints;

    Vector3[] cachePos;

    [Header("Config")]
    public float radius = 10f;
    public float power = 20f;

    public void Init(ColorEnum color)
    {
        if (cachePos == null)
        {
            cachePos = new Vector3[rbParts.Length];
            for (int i = 0; i < rbParts.Length; i++)
            {
                cachePos[i] = rbParts[i].transform.localPosition;
            }
        }

        Material mat = MaterialCache.GetHoleMainMat(color);
        for (int i = 0; i < rbParts.Length; i++)
        {
            rbParts[i].GetComponent<MeshRenderer>().sharedMaterial = mat;
            rbParts[i].velocity = Vector3.zero;
            rbParts[i].angularVelocity = Vector3.zero;
            rbParts[i].transform.localPosition = cachePos[i];
        }
    }

    public void Explode()
    {
        gameObject.SetActive(true);

        for (int i = 0; i < explosionPoints.Length; i++)
        {
            Vector3 explosionPos = explosionPoints[i].position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(power, explosionPos, radius, 3f);
                }
            }
        }

        coroutineCollect = StartCoroutine(IE_Collect());
    }

    WaitForSeconds waitForExplode = new WaitForSeconds(0.5f);
    WaitForSeconds delayCollect = new WaitForSeconds(0.03f);
    Coroutine coroutineCollect;
    IEnumerator IE_Collect()
    {
        yield return waitForExplode;

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, UIManager.Instance.ingameMenu.trsfAddProgressPoint.position);
        Vector3 worldPos = GameplayController.Instance.cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 5f));

        for (int i = 0; i < rbParts.Length; i++)
        {
            Rigidbody rb = rbParts[i];
            rb.DOMove(worldPos, 0.3f).OnComplete(() =>
            {
                rb.gameObject.SetActive(false);
                UIManager.Instance.ingameMenu.PunchScaleProgress();

                GameObject obj = GameManager.Instance.InstantiatePrefab("VFX/VFX_AddHole");
                obj.transform.position = worldPos;
                obj.transform.eulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
            });

            yield return delayCollect;
        }

        yield return new WaitForSeconds(0.25f);

        EventManager.EmitEvent(EventVariables.SetProgressLevel);

        coroutineCollect = null;

        Recycle();
    }

    public void Recycle()
    {
        if (coroutineCollect != null)
        {
            StopCoroutine(coroutineCollect);
            coroutineCollect = null;
        }

        for (int i = 0; i < rbParts.Length; i++)
        {
            rbParts[i].DOKill();
            rbParts[i].velocity = Vector3.zero;
            rbParts[i].angularVelocity = Vector3.zero;
            rbParts[i].gameObject.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
