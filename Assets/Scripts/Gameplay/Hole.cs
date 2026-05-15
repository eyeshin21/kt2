using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public List<HoleLayer> holeLayers;
    public List<HoleFill> holeFills;
    public HoleBreakEffect holeBreakEffect;
    public List<ParticleSystem> vfxCompletes;

    public int realFillAmount = 0;
    public int currentFill = 0;
    public TextMeshPro txtCapacity;

    public Queue<HoleLayerData> queueHoleLayersData = new Queue<HoleLayerData>();

    List<Ball> cacheBalls = new List<Ball>();

    public void Init(HoleData holeData)
    {
        for (int i = 0; i < holeData.holeLayersData.Count; i++)
        {
            queueHoleLayersData.Enqueue(holeData.holeLayersData[i]);
            HoleManager.Instance.totalBalls += HoleManager.Instance.ballStep;
        }

        for (int i = 0; i < holeLayers.Count; i++)
        {
            HoleLayerData holeLayerData = null;
            if (queueHoleLayersData.Count > 0)
            {
                holeLayerData = queueHoleLayersData.Dequeue();
            }
            holeLayers[i].Init(holeLayerData);
        }

        currentFill = 0;

        txtCapacity.text = $"{holeFills.Count}";
        txtCapacity.transform.eulerAngles = new Vector3(90f, 0f, 0f);
        txtCapacity.transform.localScale = Vector3.one;
        txtCapacity.gameObject.SetActive(true);
    }

    public void PreFill(Ball ball)
    {
        realFillAmount++;
        cacheBalls.Add(ball);
    }

    public void Fill(int index, Ball ball)
    {
        cacheBalls.Remove(ball);
        HoleFill holeFill = holeFills[index];
        holeFill.Init(holeLayers[0].color);
        holeFill.gameObject.SetActive(true);
        holeFill.Fill(() =>
        {
            DoneFill();
        });
    }

    public void DoneFill()
    {
        currentFill++;

        txtCapacity.text = $"{holeFills.Count - currentFill}";

        if (currentFill >= holeFills.Count)
        {
            for (int i = 0; i < vfxCompletes.Count; i++)
            {
                vfxCompletes[i].Play();
            }
            Invoke(nameof(Break), 0.1f);
        }
    }

    public void Break()
    {
        txtCapacity.gameObject.SetActive(false);

        for (int i = 0; i < holeFills.Count; i++)
        {
            holeFills[i].gameObject.SetActive(false);
        }

        holeBreakEffect.Init(holeLayers[0].color);
        holeBreakEffect.Explode();

        Invoke(nameof(ActiveNextLayer), 0.5f);
    }

    public void ActiveNextLayer()
    {
        currentFill = 0;
        realFillAmount = 0;

        for (int i = 0; i < holeLayers.Count; i++)
        {
            HoleLayer holeLayer = holeLayers[i];
            if (i == 0)
            {
                ColorEnum nextColor = holeLayers[i + 1].color;
                holeLayer.ActiveNextColor(nextColor, false, () =>
                {
                    if (nextColor != ColorEnum.None)
                    {
                        txtCapacity.gameObject.SetActive(true);
                        txtCapacity.text = $"{holeFills.Count - currentFill}";
                        txtCapacity.transform.localScale = Vector3.zero;
                        txtCapacity.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                    }
                });
            }
            if (i == holeLayers.Count - 1)
            {
                HoleLayerData holeLayerData = null;
                if (queueHoleLayersData.Count > 0)
                {
                    holeLayerData = queueHoleLayersData.Dequeue();
                }

                holeLayer.Init(holeLayerData);
                if (holeLayerData != null && holeLayerData.color != ColorEnum.None)
                {
                    holeLayer.ActiveNextColor(holeLayerData.color, holeLayerData.isHidden);
                }
            }
            else
            {
                holeLayer.ActiveNextColor(holeLayers[i + 1].color, holeLayers[i + 1].isHidden);
            }
        }

        GameplayController.Instance.CheckLose();
        GameplayController.Instance.CheckWin();
    }

    public bool IsFull()
    {
        return realFillAmount >= holeFills.Count;
    }

    public void Recycle()
    {
        for (int i = 0; i < cacheBalls.Count; i++)
        {
            cacheBalls[i].Recycle();
        }
        cacheBalls.Clear();

        currentFill = 0;
        realFillAmount = 0;

        txtCapacity.transform.DOKill();

        queueHoleLayersData.Clear();

        for (int i = 0; i < holeLayers.Count; i++)
        {
            holeLayers[i].Recycle();
        }

        for (int i = 0; i < holeFills.Count; i++)
        {
            holeFills[i].gameObject.SetActive(false);
        }

        holeBreakEffect.Recycle();
        gameObject.Recycle();
    }
}
