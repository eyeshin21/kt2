using DG.Tweening;
using DG.Tweening.Core.Easing;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

public class FunnelManager : Singleton<FunnelManager>
{
    public SplineComputer spline;
    public SplinePositioner startPos;
    public NodeController[] nodes;

    public Transform ballFrom;
    public Transform ballParent;
    public int maxCapacity;
    public TextMeshPro txtCapacity;
    public float moveTime = 0.2f;
    public float distanceBetweenBall = 1f;

    public GameObject ballPrefab;
    public List<Ball> balls = new List<Ball>();
    public List<ColorEnum> preBalls = new List<ColorEnum>();
    public Queue<ColorEnum> queueBalls = new Queue<ColorEnum>();
    public bool isAddingBall = false;

    private void Update()
    {
        if (balls.Count > 0)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].CheckMove();
            }
        }
    }

    public void Init()
    {
        SetTextCapacity();

        spline.triggerGroups = new TriggerGroup[0];
        AddTrigger(OnBallCrossStartLine, startPos.GetPercent(), name: "OnBallCrossStartLine");
    }

    public bool IsFull()
    {
        return balls.Count == maxCapacity;
    }

    public bool PreCheckFullConveyor()
    {
        int total = balls.Count + preBalls.Count + queueBalls.Count;

        return total >= maxCapacity;
    }

    public void PreAddBall(ColorEnum color)
    {
        preBalls.Add(color);
    }

    public void AddBall(ColorEnum color)
    {
        preBalls.Remove(color);
        queueBalls.Enqueue(color);

        if (!isWaitingForDoneAddBall)
        {
            if (coroutineAddBall == null)
            {
                coroutineAddBall = StartCoroutine(IE_AddBall());
            }
        }
        else
        {
            if (coroutineAddBall != null)
            {
                StopCoroutine(coroutineAddBall);
                coroutineAddBall = StartCoroutine(IE_AddBall());
            }
        }
    }

    Coroutine coroutineAddBall;
    WaitForSeconds waitForAddBall = new WaitForSeconds(0.1f);
    WaitForSeconds waitForDoneAddBall = new WaitForSeconds(0.2f);
    WaitForSeconds waitForResetBallOrder = new WaitForSeconds(0.01f);
    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    bool isWaitingForDoneAddBall = false;
    IEnumerator IE_AddBall()
    {
        isWaitingForDoneAddBall = false;
        while (queueBalls.Count > 0)
        {
            if (balls.Count < maxCapacity)
            {
                isAddingBall = true;

                double startPercent = spline.Travel(startPos.GetPercent(), distanceBetweenBall);

                if (IsAnyBallCrossingTheStartLine(out Ball ballToWait))
                {
                    WaitUntil waitUntilBlockCrossedStartLine = new WaitUntil(() => ballToWait.splineFollower.GetPercent() > startPercent);
                    yield return waitUntilBlockCrossedStartLine;
                }

                ColorEnum color = queueBalls.Dequeue();

                GameObject obj = ballPrefab.Spawn(ballParent);
                obj.transform.position = ballFrom.position;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;

                Ball ball = obj.GetComponent<Ball>();
                ball.Init(color);

                SplineSample sample = spline.Evaluate(startPercent);
                ball.transform.DOJump(sample.position, 0.25f, 1, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    ball.splineFollower.spline = spline;
                    ball.splineFollower.SetPercent(startPercent, false);
                    ball.splineFollower.autoStartPosition = true;
                    ball.splineFollower.RebuildImmediate();
                    ball.splineFollower.enabled = true;
                    ball.StartMove();

                    balls.Add(ball);

                    Invoke(nameof(ResetBallOrder), 0.01f);
                });

                //GameManager.Instance.currentLevel.CheckLose();
            }
            else
            {
                isAddingBall = false;
            }

            yield return waitForAddBall;
        }

        isWaitingForDoneAddBall = true;
        yield return waitForDoneAddBall;
        yield return waitForResetBallOrder;

        isWaitingForDoneAddBall = false;
        isAddingBall = false;
        coroutineAddBall = null;
    }

    public void StopMove()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].StopMove();
        }

        if (coroutineAddBall != null)
        {
            StopCoroutine(coroutineAddBall);
            coroutineAddBall = null;
        }

        isAddingBall = true;
    }

    public void RemoveBall(Ball ball)
    {
        balls.Remove(ball);
        ResetBallOrder();
    }

    public bool IsAnyBallCrossingTheStartLine(out Ball ball)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            double percent = balls[i].splineFollower.GetPercent();
            double startPercent = spline.Travel(startPos.GetPercent(), distanceBetweenBall);

            if (percent > startPos.GetPercent() && percent <= startPercent)
            {
                ball = balls[i];
                return true;
            }
        }

        ball = null;
        return false;
    }

    public void ResetBallOrder()
    {
        if (balls.Count > 1)
        {
            balls.Sort((o1, o2) => { return o1.splineFollower.GetPercent().CompareTo(o2.splineFollower.GetPercent()); });

            //List<Ball> cacheBalls = new List<Ball>(balls);
            //List<Ball> ballsToInsert = new List<Ball>();
            //for (int i = 0; i < cacheBalls.Count; i++)
            //{
            //    if (cacheBalls[i].splineFollower.GetPercent() < startPos.GetPercent())
            //    {
            //        ballsToInsert.Add(cacheBalls[i]);
            //        balls.Remove(cacheBalls[i]);
            //    }
            //}

            //if (ballsToInsert.Count > 0)
            //{
            //    balls.AddRange(ballsToInsert);
            //}

            for (int i = 0; i < balls.Count; i++)
            {
                if (i == balls.Count - 1)
                {
                    balls[i].ballForward = balls[0];
                }
                else
                {
                    balls[i].ballForward = balls[i + 1];
                }
            }
        }
        else if (balls.Count == 1)
        {
            balls[0].ballForward = null;
        }

        SetTextCapacity();
    }

    public void OnBallCrossStartLine(SplineUser splineUser)
    {
        if (isAddingBall)
        {
            //Debug.Log("Cross Start Line");

            Ball ball = splineUser.GetComponent<Ball>();
            ball.StopMove();
            ball.splineFollower.SetPercent(startPos.GetPercent(), false);
        }
    }

    public void SetTextCapacity()
    {
        txtCapacity.text = $"{balls.Count}/{maxCapacity}";
    }

    public void AddTrigger(UnityEngine.Events.UnityAction<SplineUser> action, double position, SplineTrigger.Type type = SplineTrigger.Type.Forward, string name = "API Trigger")
    {
        SplineTrigger trigger = spline.AddTrigger(0, position, type, name, Color.white);
        trigger.AddListener(action);

        spline.RebuildImmediate();
    }

    public void AddTrigger(UnityEngine.Events.UnityAction<SplineUser> action, Vector3 position, SplineTrigger.Type type = SplineTrigger.Type.Forward, string name = "API Trigger")
    {
        spline.RebuildImmediate();
        SplineSample sample = spline.Project(position);

        SplineTrigger trigger = spline.AddTrigger(0, sample.percent, type, name, Color.white);
        trigger.AddListener(action);

        spline.RebuildImmediate();
    }

    public void Recycle()
    {
        isAddingBall = false;

        if (coroutineAddBall != null)
        {
            StopCoroutine(coroutineAddBall);
            coroutineAddBall = null;
        }

        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i] != null)
            {
                balls[i].Recycle();
            }
        }
        balls.Clear();

        queueBalls.Clear();
        preBalls.Clear();
    }
}
