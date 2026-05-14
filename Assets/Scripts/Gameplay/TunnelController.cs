using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TunnelController : MonoBehaviour
{
    public Vector2Int pos;
    public Vector2Int spawnPos;
    public Direction direction;
    public TextMeshPro txtCount;
    public List<Box> boxes = new List<Box>();

    public void Init(TunnelData data)
    {
        direction = data.direction;
        pos = new Vector2Int(data.coordinateX, data.coordinateY);

        switch (direction)
        {
            case Direction.Left:
                transform.localEulerAngles = Vector3.up * 270f;
                txtCount.transform.localEulerAngles = new Vector3(90f, 90f, 0);
                spawnPos = new Vector2Int(pos.x - 1, pos.y);
                break;
            case Direction.Right:
                transform.localEulerAngles = Vector3.up * 90f;
                txtCount.transform.localEulerAngles = new Vector3(90f, 270f, 0);
                spawnPos = new Vector2Int(pos.x + 1, pos.y);
                break;
            case Direction.Up:
                transform.localEulerAngles = Vector3.zero;
                txtCount.transform.localEulerAngles = new Vector3(90f, 0f, 0);
                spawnPos = new Vector2Int(pos.x, pos.y + 1);
                break;
            case Direction.Down:
                transform.localEulerAngles = Vector3.up * 180f;
                txtCount.transform.localEulerAngles = new Vector3(90f, 180f, 0);
                spawnPos = new Vector2Int(pos.x, pos.y - 1);
                break;
        }

        txtCount.gameObject.SetActive(true);
        txtCount.text = data.boxesData.Count.ToString();
    }

    public void Recycle()
    {
        if (checkActiveNextBox != null)
        {
            checkActiveNextBox.Kill();
            checkActiveNextBox = null;
        }

        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].Recycle();
        }
        boxes.Clear();

        gameObject.Recycle();
    }

    Tween checkActiveNextBox;
    public void CheckActiveNextBox()
    {
        if (boxes.Count > 0)
        {
            if (BoxManager.Instance.boxGrid[spawnPos.x, spawnPos.y] == null)
            {
                Box nextBox = boxes[0];
                nextBox.isActive = true;
                BoxManager.Instance.boxGrid[spawnPos.x, spawnPos.y] = nextBox;
                boxes.RemoveAt(0);

                checkActiveNextBox = DOVirtual.DelayedCall(0.15f, () =>
                {
                    nextBox.gameObject.SetActive(true);
                    nextBox.pos = spawnPos;
                    nextBox.MoveToSpawnPos(true);

                    txtCount.gameObject.SetActive(boxes.Count > 0);
                    txtCount.text = boxes.Count.ToString();

                    checkActiveNextBox = null;
                }, false);
            }
        }
    }
}
