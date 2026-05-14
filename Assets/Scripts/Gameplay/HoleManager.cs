using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : Singleton<HoleManager>
{
    public GameObject holeFullPrefab;
    public GameObject holeHalfPrefab;
    public GameObject holeQuarterPrefab;
    public Transform holeParent;
    public List<Hole> holes = new List<Hole>();

    public void Init(List<HoleData> holesData)
    {
        switch (holesData.Count)
        {
            case 1:
                {
                    GameObject obj = holeFullPrefab.Spawn(holeParent);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localEulerAngles = Vector3.zero;
                    obj.transform.localScale = Vector3.one;

                    Hole hole = obj.GetComponent<Hole>();
                    hole.Init(holesData[0]);

                    holes.Add(hole);

                    for (int i = 0; i < hole.holeFills.Count; i++)
                    {
                        NodeController node = FunnelManager.Instance.nodes[i];
                        FunnelManager.Instance.AddTrigger((user) =>
                        {
                            OnTriggerSpline(user, node.index);
                        }, node.transform.position, name: $"Node_{node.index}");
                    }

                    break;
                }
            case 2:
                {
                    int nodeIndex = 0;
                    for (int i = 0; i < holesData.Count; i++)
                    {
                        float rotationY = i * (360f / holesData.Count);

                        GameObject obj = holeHalfPrefab.Spawn(holeParent);
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localEulerAngles = new Vector3(0, rotationY, 0);
                        obj.transform.localScale = Vector3.one;

                        Hole hole = obj.GetComponent<Hole>();
                        hole.Init(holesData[i]);

                        holes.Add(hole);

                        for (int j = 0; j < hole.holeFills.Count; j++)
                        {
                            NodeController node = FunnelManager.Instance.nodes[j + nodeIndex];
                            FunnelManager.Instance.AddTrigger((user) =>
                            {
                                OnTriggerSpline(user, node.index);
                            }, node.transform.position, name: $"Node_{node.index}");
                        }

                        nodeIndex += hole.holeFills.Count;
                    }
                    break;
                }
            case 4:
                {
                    int nodeIndex = 0;
                    for (int i = 0; i < holesData.Count; i++)
                    {
                        float rotationY = i * (360f / holesData.Count);

                        GameObject obj = holeQuarterPrefab.Spawn(holeParent);
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localEulerAngles = new Vector3(0, rotationY, 0);
                        obj.transform.localScale = Vector3.one;

                        Hole hole = obj.GetComponent<Hole>();
                        hole.Init(holesData[i]);

                        holes.Add(hole);

                        for (int j = 0; j < hole.holeFills.Count; j++)
                        {
                            NodeController node = FunnelManager.Instance.nodes[j + nodeIndex];
                            FunnelManager.Instance.AddTrigger((user) =>
                            {
                                OnTriggerSpline(user, node.index);
                            }, node.transform.position, name: $"Node_{node.index}");
                        }

                        nodeIndex += hole.holeFills.Count;
                    }
                    break;
                }
        }
    }

    public void OnTriggerSpline(SplineUser user, int nodeIndex)
    {
        if (user.TryGetComponent(out Ball ball))
        {
            switch (holes.Count)
            {
                case 1:
                    {
                        Hole hole = holes[0];
                        if (!hole.IsFull() && hole.realAmount == nodeIndex && hole.holeLayers[0].color == ball.color)
                        {
                            NodeController node = FunnelManager.Instance.nodes[nodeIndex];
                            ball.Fill(hole, node);
                        }
                        break;
                    }
                case 2:
                case 4:
                    {
                        int indexTemp = 0;
                        for (int i = 0; i < holes.Count; i++)
                        {
                            Hole hole = holes[i];

                            if (!hole.IsFull() && hole.realAmount + indexTemp == nodeIndex && hole.holeLayers[0].color == ball.color)
                            {
                                NodeController node = FunnelManager.Instance.nodes[nodeIndex];
                                ball.Fill(hole, node);
                                break;
                            }

                            indexTemp += hole.holeFills.Count;
                        }

                        break;
                    }
            }

        }
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < holes.Count; i++)
        {
            Hole hole = holes[i];
            if (hole.queueHoleLayersData.Count > 0)
            {
                return false;
            }

            for (int j = 0; j < hole.holeLayers.Count; j++)
            {
                HoleLayer holeLayer = hole.holeLayers[j];
                if (holeLayer.color != ColorEnum.None)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void Recycle()
    {
        for (int i = 0; i < holes.Count; i++)
        {
            holes[i].Recycle();
        }
        holes.Clear();
    }
}
