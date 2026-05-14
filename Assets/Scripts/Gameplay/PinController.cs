using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

public class PinController : MonoBehaviour
{
    public GameObject objPin;

    public Vector2Int pos;
    public Vector2Int checkPos;
    public Direction direction;
    public List<Box> boxes = new List<Box>();

    bool isUnpin = false;

    private void Start()
    {
        EventManager.StartListening(EventVariables.CheckPin, CheckPin);
    }

    public void Init(PinData data)
    {
        pos = new Vector2Int(data.coordinateX, data.coordinateY);
        direction = data.direction;

        isUnpin = false;

        switch (direction)
        {
            case Direction.Left:
                checkPos = new Vector2Int(pos.x + 1, pos.y);
                transform.localEulerAngles = Vector3.up * 270f;
                break;
            case Direction.Right:
                checkPos = new Vector2Int(pos.x - 1, pos.y);
                transform.localEulerAngles = Vector3.up * 90f;
                break;
            case Direction.Up:
                transform.localEulerAngles = Vector3.zero;
                checkPos = new Vector2Int(pos.x, pos.y - 1);
                break;
            case Direction.Down:
                transform.localEulerAngles = Vector3.up * 180f;
                checkPos = new Vector2Int(pos.x, pos.y + 1);
                break;
        }
    }

    public void CheckBox()
    {
        switch (direction)
        {
            case Direction.Left:
                for (int x = pos.x; x >= pos.x - 2; x--)
                {
                    Box box = BoxManager.Instance.boxGrid[x, pos.y];
                    if (box != null)
                    {
                        boxes.Add(box);
                    }
                }
                break;
            case Direction.Right:
                for (int x = pos.x; x <= pos.x + 2; x++)
                {
                    Box box = BoxManager.Instance.boxGrid[x, pos.y];
                    if (box != null)
                    {
                        boxes.Add(box);
                    }
                }
                break;
            case Direction.Up:
                for (int y = pos.y; y <= pos.y + 2; y++)
                {
                    Box box = BoxManager.Instance.boxGrid[pos.x, y];
                    if (box != null)
                    {
                        boxes.Add(box);
                    }
                }
                break;
            case Direction.Down:
                for (int y = pos.y; y >= pos.y - 2; y--)
                {
                    Box box = BoxManager.Instance.boxGrid[pos.x, y];
                    if (box != null)
                    {
                        boxes.Add(box);
                    }
                }
                break;
        }

        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].hasPin = true;
        }
    }

    public void CheckPin()
    {
        if (!isUnpin)
        {
            if (BoxManager.Instance.boxGrid[checkPos.x, checkPos.y] == null)
            {
                Unpin();
            }
        }
    }

    public void Unpin()
    {
        isUnpin = true;

        objPin.transform.DOScale(0f, 0.25f).SetEase(Ease.Linear);
        transform.DOLocalMove(BoxManager.Instance.GetWorldPos(checkPos.x, checkPos.y), 0.25f).SetEase(Ease.Linear).OnComplete(()=>
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].hasPin = false;

                if (boxes[i].isActive)
                {
                    boxes[i].Active2();
                }
            }

            Recycle();
        });
    }

    public void Recycle()
    {
        transform.DOKill();
        objPin.transform.localScale = Vector3.one;
        boxes.Clear();
        isUnpin = true;

        gameObject.Recycle();
    }
}
