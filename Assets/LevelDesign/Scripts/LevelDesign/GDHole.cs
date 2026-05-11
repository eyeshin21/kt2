using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDHole : MonoBehaviour
{
    public List<GDHoleLayer> holeLayers;

    public void Init(HoleDataDefault holeDataDefault)
    {
        holeLayers[0].Init(holeDataDefault.firstLayerHole);
        holeLayers[1].Init(holeDataDefault.secondLayerHole);
        holeLayers[2].Init(holeDataDefault.thirdLayerHole);
    }
}
