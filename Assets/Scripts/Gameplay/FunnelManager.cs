using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
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
    public float moveTime = 0.3f;
    public float distanceBetweenBall = 1f;
    float splineLength;
    float minPercentGap;
    double insertPercent;

    public GameObject ballPrefab;
    public List<Ball> balls = new List<Ball>();
    public List<ColorEnum> preBalls = new List<ColorEnum>();
    public Queue<ColorEnum> queueBalls = new Queue<ColorEnum>();
    public bool isAddingBall = false;

    private void LateUpdate()
    {
        if (balls.Count > 1)
        {
            //ResetBallOrder();
            //for (int i = 0; i < balls.Count; i++)
            //{
            //    balls[i].CheckMove();
            //}

            balls.Sort((o1, o2) => { return o1.splineFollower.GetPercent().CompareTo(o2.splineFollower.GetPercent()); });
            HandleTraffic();
            ResolveOverlap();
        }
    }

    public void Init()
    {
        SetTextCapacity();

        spline.RebuildImmediate();
        spline.triggerGroups = new TriggerGroup[0];
        AddTrigger(OnBallCrossStartLine, startPos.GetPercent(), name: "OnBallCrossStartLine");

        splineLength = spline.CalculateLength();
        minPercentGap = distanceBetweenBall / splineLength;
        insertPercent = startPos.GetPercent();
    }

    public bool IsFull(out int totalFreeSlot)
    {
        int totalBalls = balls.Count + preBalls.Count + queueBalls.Count;

        totalFreeSlot = maxCapacity - totalBalls;
        return totalBalls >= maxCapacity;
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
    WaitForSeconds waitForAddBall = new WaitForSeconds(0.05f);
    WaitForSeconds waitForDoneAddBall = new WaitForSeconds(0.25f);
    bool isWaitingForDoneAddBall = false;
    IEnumerator IE_AddBall()
    {
        isWaitingForDoneAddBall = false;
        while (queueBalls.Count > 0)
        {
            if (balls.Count < maxCapacity)
            {
                isAddingBall = true;

                ColorEnum color = queueBalls.Dequeue();

                GameObject obj = ballPrefab.Spawn(ballParent);
                obj.transform.position = ballFrom.position;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;

                Ball ball = obj.GetComponent<Ball>();
                ball.Init(color);

                SplineSample sample = spline.Evaluate(insertPercent);
                ball.transform.DOJump(sample.position, 0.5f, 1, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    ball.splineFollower.spline = spline;
                    ball.splineFollower.SetPercent(insertPercent);
                    ball.splineFollower.autoStartPosition = true;
                    ball.splineFollower.enabled = true;
                    ball.splineFollower.RebuildImmediate();

                    balls.Add(ball);
                    SetTextCapacity();

                    GameplayController.Instance.CheckLose();
                });

            }
            else
            {
                if (isAddingBall)
                {
                    yield return waitForDoneAddBall;
                    isAddingBall = false;
                }
            }

            yield return waitForAddBall;
        }

        isWaitingForDoneAddBall = true;
        yield return waitForDoneAddBall;

        isWaitingForDoneAddBall = false;
        isAddingBall = false;
        coroutineAddBall = null;
    }

    public void RemoveBall(Ball ball)
    {
        balls.Remove(ball);
        SetTextCapacity();
    }

    public bool IsAnyBallMatchAnyHole()
    {
        List<Hole> holes = HoleManager.Instance.holes;
        for (int i = 0; i < holes.Count; i++)
        {
            if (holes[i].IsFull())
            {
                return true;
            }
        }

        for (int i = 0; i < balls.Count; i++)
        {
            for (int j = 0; j < holes.Count; j++)
            {
                if (!holes[j].IsFull() && holes[j].holeLayers[0].color == balls[i].color)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void OnBallCrossStartLine(SplineUser splineUser)
    {
        if (isAddingBall)
        {
            //Debug.Log("Cross Start Line");

            //Ball ball = splineUser.GetComponent<Ball>();
            //ball.StopMove();
            //ball.splineFollower.SetPercent(startPos.GetPercent(), false);
        }
    }

    public void SetTextCapacity()
    {
        txtCapacity.text = $"{balls.Count}/{maxCapacity}";
    }

    private void HandleTraffic()
    {
        int count = balls.Count;

        foreach (Ball ball in balls)
        {
            ball.splineFollower.follow = true;
        }

        for (int i = 0; i < count; i++)
        {
            SplineFollower current = balls[i].splineFollower;
            SplineFollower front = balls[(i + 1) % count].splineFollower;

            bool shouldStop = false;

            if (isAddingBall)
            {
                double gapToInsert = DeltaLoopPercent(current.result.percent, insertPercent);

                if (gapToInsert <= minPercentGap)
                {
                    shouldStop = true;

                    double target = insertPercent - minPercentGap;

                    target = NormalizePercent(target);

                    current.SetPercent(target);
                }
            }

            if (!front.follow)
            {
                double gapToFront =
                    DeltaLoopPercent(current.result.percent, front.result.percent);

                if (gapToFront <= minPercentGap)
                {
                    shouldStop = true;

                    double target = front.result.percent - minPercentGap;

                    target = NormalizePercent(target);

                    current.SetPercent(target);
                }
            }

            current.follow = !shouldStop;
        }
    }

    private void ResolveOverlap()
    {
        int count = balls.Count;

        for (int i = 0; i < count; i++)
        {
            SplineFollower current = balls[i].splineFollower;
            SplineFollower next = balls[(i + 1) % count].splineFollower;

            double gap = DeltaLoopPercent(current.result.percent, next.result.percent);

            if (gap < minPercentGap)
            {
                double target = current.result.percent + minPercentGap;

                target = NormalizePercent(target);

                if (next.follow)
                {
                    next.SetPercent(target);
                }
            }
        }
    }

    private double DeltaLoopPercent(double a, double b)
    {
        double delta = b - a;

        if (delta < 0)
            delta += 1.0;

        return delta;
    }

    private double NormalizePercent(double percent)
    {
        while (percent < 0.0)
            percent += 1.0;

        while (percent > 1.0)
            percent -= 1.0;

        return percent;
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

        isWaitingForDoneAddBall = false;

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
