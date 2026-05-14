using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public ColorEnum color;
    public MeshRenderer meshRenderer;

    SplineTracer tracer;
    public SplineFollower splineFollower;
    public Ball ballForward;

    float cacheSpeed;

    private void Start()
    {
        tracer = GetComponent<SplineTracer>();
        cacheSpeed = splineFollower.followSpeed;
    }

    public void Init(ColorEnum color)
    {
        this.color = color;

        Material ballMat = MaterialCache.GetBallMat(color);
        meshRenderer.sharedMaterial = ballMat;
    }

    public void StartMove()
    {
        splineFollower.follow = true;
    }

    public void PauseMove()
    {
        splineFollower.follow = false;
    }

    public bool forceStop = false;
    public void StopMove()
    {
        forceStop = true;
        splineFollower.follow = false;
    }

    public void CheckMove()
    {
        if (!forceStop && ballForward != null)
        {
            if (!ballForward.splineFollower.follow)
            {
                double currentPercent = splineFollower.GetPercent();
                double checkPercent = ballForward.splineFollower.GetPercent();
                float distance;
                if (currentPercent < checkPercent)
                {
                    distance = FunnelManager.Instance.spline.CalculateLength(currentPercent, checkPercent);
                }
                else
                {
                    distance = FunnelManager.Instance.spline.CalculateLength(currentPercent, 1);
                    distance += FunnelManager.Instance.spline.CalculateLength(0, checkPercent);
                }

                if (distance <= FunnelManager.Instance.distanceBetweenBall)
                {
                    if (splineFollower.follow)
                    {
                        PauseMove();
                    }
                }
            }
            else
            {
                if (!splineFollower.follow)
                {
                    StartMove();
                }
            }
        }

        if (forceStop && !FunnelManager.Instance.isAddingBall)
        {
            forceStop = false;
            StartMove();
        }
    }

    Hole targetHole = null;
    int fillIndex;
    public void Fill(Hole hole, NodeController nodeController)
    {
        FunnelManager.Instance.RemoveBall(this);

        targetHole = hole;
        fillIndex = hole.realAmount;

        hole.realAmount++;
        Node.Connection[] connections = nodeController.Node.GetConnections();

        splineFollower.wrapMode = SplineFollower.Wrap.Default;
        tracer.spline = connections[1].spline;
        tracer.RebuildImmediate();
        double startpercent = tracer.ClipPercent(connections[1].spline.GetPointPercent(connections[1].pointIndex));
        tracer.SetPercent(startpercent);

        DOVirtual.Float(cacheSpeed, cacheSpeed + 5f, 1f, (result) =>
        {
            splineFollower.followSpeed = result;
        }).SetEase(Ease.InQuad);

        canCheckEnd = true;
    }

    bool canCheckEnd = false;
    public void OnEndReached()
    {
        if (canCheckEnd && targetHole != null)
        {
            targetHole.Fill(fillIndex);

            splineFollower.follow = false;
            splineFollower.enabled = false;

            transform.DOMoveY(transform.position.y - 2f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Recycle();
            });
        }
    }

    public void Recycle()
    {
        transform.DOKill();

        targetHole = null;
        fillIndex = -1;

        splineFollower.follow = false;
        splineFollower.wrapMode = SplineFollower.Wrap.Loop;
        splineFollower.followSpeed = cacheSpeed;
        splineFollower.spline = null;
        splineFollower.enabled = false;
        splineFollower.RebuildImmediate();

        forceStop = false;

        ballForward = null;

        gameObject.Recycle();
    }
}
