using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public List<HoleLayer> holeLayers;
    public List<HoleFill> holeFills;

    public int realAmount = 0;
    public int currentFill = 0;

    public void Init(HoleDataDefault holeDataDefault)
    {
        holeLayers[0].Init(holeDataDefault.firstLayerHole);
        holeLayers[1].Init(holeDataDefault.secondLayerHole);
        holeLayers[2].Init(holeDataDefault.thirdLayerHole);

        currentFill = 0;
    }

    public void Fill(int index)
    {
        HoleFill holeFill = holeFills[index];
        holeFill.Init(holeLayers[2].color);
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

        holeLayers[2].ActiveNextColor(holeLayers[1].color);
        holeLayers[1].ActiveNextColor(holeLayers[0].color);
        holeLayers[0].ActiveNextColor(HoleManager.Instance.queueHole.color);

        HoleManager.Instance.ActiveNextColorQueueHole();
    }

    public bool IsFull()
    {
        return realAmount >= holeFills.Count;
    }
}
