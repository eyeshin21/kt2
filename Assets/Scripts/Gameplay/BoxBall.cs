using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBall : MonoBehaviour
{
    public ColorEnum color;
    public MeshRenderer meshRenderer;
    public SplineFollower follower;

    public void SetColor(ColorEnum color)
    {
        this.color = color;

        Material ballMat = MaterialCache.GetBallMat(color);
        meshRenderer.sharedMaterial = ballMat;
    }

    public void Jump()
    {
        transform.DOScale(1f, 0.1f);
        transform.DOMoveY(transform.position.y + 0.1f, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SplineSample sample = PipeHole.Instance.spline.Evaluate(0f);

            transform.DOJump(sample.position, 2.5f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                follower.spline = PipeHole.Instance.spline;
                follower.SetPercent(0, false);
                follower.autoStartPosition = true;
                follower.RebuildImmediate();
                follower.enabled = true;
                follower.follow = true;

                //transform.DOMoveY(transform.position.y - 2f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                //{
                //  FunnelManager.Instance.AddBall(color);
                //  PipeHole.Instance.boxBalls.Remove(this);
                //  Recycle();
                //});
            });
        });
    }

    public void OnEndReached()
    {
        FunnelManager.Instance.AddBall(color);
        PipeHole.Instance.boxBalls.Remove(this);
        Recycle();
    }

    public void Recycle()
    {
        transform.DOKill();

        follower.follow = false;
        follower.spline = null;
        follower.enabled = false;
        follower.RebuildImmediate();

        gameObject.Recycle();
    }
}
