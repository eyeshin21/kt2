using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GDTunnel : MonoBehaviour
{
    public int x;
    public int y;

    public TextMeshPro txtTotalBox;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetUp(TunnelData data)
    {
        SetTotalBox(data.boxesData.Count);
        SetDirection(data.direction);
    }

    public void SetTotalBox(int totalBox)
    {
        txtTotalBox.text = totalBox.ToString();
    }

    public void SetDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                transform.localEulerAngles = Vector3.forward * 90f;
                txtTotalBox.transform.localEulerAngles = Vector3.forward * -90f;
                break;
            case Direction.Right:
                transform.localEulerAngles = Vector3.forward * 270f;
                txtTotalBox.transform.localEulerAngles = Vector3.forward * -270f;
                break;
            case Direction.Up:
                transform.localEulerAngles = Vector3.zero;
                txtTotalBox.transform.localEulerAngles = Vector3.zero;
                break;
            case Direction.Down:
                transform.localEulerAngles = Vector3.forward * 180f;
                txtTotalBox.transform.localEulerAngles = Vector3.forward * -180f;
                break;
        }
    }
}
