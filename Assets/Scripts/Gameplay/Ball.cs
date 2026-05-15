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

    Tween tweenSpeedUp;
    Hole targetHole = null;
    int fillIndex;
    public void Fill(Hole hole, NodeController nodeController)
    {
        FunnelManager.Instance.RemoveBall(this);

        targetHole = hole;
        fillIndex = hole.realFillAmount;

        hole.PreFill(this);
        Node.Connection[] connections = nodeController.Node.GetConnections();

        splineFollower.wrapMode = SplineFollower.Wrap.Default;
        tracer.spline = connections[1].spline;
        tracer.RebuildImmediate();
        double startpercent = tracer.ClipPercent(connections[1].spline.GetPointPercent(connections[1].pointIndex));
        tracer.SetPercent(startpercent);

        tweenSpeedUp = DOVirtual.Float(cacheSpeed, cacheSpeed + 5f, 1f, (result) =>
        {
            splineFollower.followSpeed = result;
        }).SetEase(Ease.InQuad).OnComplete(() =>
        {
            tweenSpeedUp = null;
        });

        canCheckEnd = true;
    }

    bool canCheckEnd = false;
    public void OnEndReached()
    {
        if (canCheckEnd && targetHole != null)
        {
            targetHole.Fill(fillIndex, this);

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

        if (tweenSpeedUp != null)
        {
            tweenSpeedUp.Kill();
            tweenSpeedUp = null;
        }

        targetHole = null;
        fillIndex = -1;

        splineFollower.follow = false;
        splineFollower.wrapMode = SplineFollower.Wrap.Loop;
        splineFollower.followSpeed = cacheSpeed;
        splineFollower.spline = null;
        splineFollower.RebuildImmediate();
        splineFollower.enabled = false;

        ballForward = null;

        gameObject.Recycle();
    }
}
