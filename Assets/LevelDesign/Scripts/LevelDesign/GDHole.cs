using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDHole : MonoBehaviour
{
    public List<GDHoleLayer> holeLayers;

    public void Init(HoleData holeData)
    {
        for (int i = 0; i < holeLayers.Count; i++)
        {
            if (holeData.holeLayersData.Count > i)
            {
                holeLayers[i].Init(holeData.holeLayersData[i]);
            }
            else
            {
                holeLayers[i].Init(null);
            }
        }
    }
}
