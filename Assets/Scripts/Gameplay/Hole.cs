using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public List<HoleLayer> holeLayers;
    public List<HoleFill> holeFills;

    public int realAmount = 0;
    public int currentFill = 0;

    public Queue<HoleLayerData> queueHoleLayersData = new Queue<HoleLayerData>();

    public void Init(HoleData holeData)
    {
        for (int i = 0; i < holeData.holeLayersData.Count; i++)
        {
            queueHoleLayersData.Enqueue(holeData.holeLayersData[i]);
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
    }

    public void Fill(int index)
    {
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

        if (currentFill >= holeFills.Count)
        {
            Break();
            GameplayController.Instance.CheckWin();
        }
    }

    public void Break()
    {
        for (int i = 0; i < holeFills.Count; i++)
        {
            holeFills[i].gameObject.SetActive(false);
        }
        currentFill = 0;
        realAmount = 0;

        for (int i = 0; i < holeLayers.Count; i++)
        {
            HoleLayer holeLayer = holeLayers[i];
            if (i == 0)
            {
                holeLayer.ActiveNextColor(holeLayers[i + 1].color, false);
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
    }

    public bool IsFull()
    {
        return realAmount >= holeFills.Count;
    }

    public void Recycle()
    {
        currentFill = 0;
        realAmount = 0;

        queueHoleLayersData.Clear();

        for (int i = 0; i < holeFills.Count; i++)
        {
            holeFills[i].gameObject.SetActive(false);
        }

        gameObject.Recycle();
    }
}
