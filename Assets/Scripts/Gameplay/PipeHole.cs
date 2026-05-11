using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeHole : Singleton<PipeHole>
{
    public SplineComputer spline;
    public Transform[] ballPos;

    public GameObject boxBallPrefab;
    public Transform boxBallParent;
    public List<BoxBall> boxBalls = new List<BoxBall>();
    
    public BoxBall SpawnBoxBall(ColorEnum color, Vector3 position)
    {
        GameObject obj = boxBallPrefab.Spawn(boxBallParent);
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * 0.7f;

        BoxBall boxBall = obj.GetComponent<BoxBall>();
        boxBall.SetColor(color);
        boxBall.Jump();

        boxBalls.Add(boxBall);
        return boxBall;
    }

    public Transform GetJumpPos()
    {
        return ballPos[Random.Range(0, ballPos.Length)];
    }

    public void Recycle()
    {
        for (int i = 0; i < boxBalls.Count; i++)
        {
            boxBalls[i].Recycle();
        }
        boxBalls.Clear();
    }
}
