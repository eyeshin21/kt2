using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("Data")]
    public bool isActive = false;
    public ColorEnum color;
    public Vector2Int pos;

    [Header("Model")]
    public Animator anim;
    public MeshRenderer boxRenderer;
    public MeshRenderer capRenderer;
    public Collider tapColl;

    [Header("Balls")]
    public MeshRenderer[] ballsRenderer;
    public GameObject[] objBalls;

    public void Init(BoxData data)
    {
        pos = new Vector2Int(data.coordinateX, data.coordinateY);
        color = data.color;

        Material boxMat = MaterialCache.GetBoxInactiveMat(color);

        Material[] boxMats = boxRenderer.sharedMaterials;
        for (int i = 0; i < boxMats.Length; i++)
        {
            boxMats[i] = boxMat;
        }

        boxRenderer.sharedMaterials = boxMats;
        capRenderer.sharedMaterial = boxMat;

        for (int i = 0; i < ballsRenderer.Length; i++)
        {
            ballsRenderer[i].sharedMaterial = boxMat;
            ballsRenderer[i].gameObject.SetActive(true);
        }
    }

    public void OnTap()
    {
        if (isActive)
        {
            if (!FunnelManager.Instance.IsFull())
            {
                Move();
            }
            else
            {
                NoMove(true);
            }
        }
        else
        {
            NoMove(false);
        }
    }

    public void Move()
    {
        Material boxMat = MaterialCache.GetBoxInactiveMat(color);
        Material[] boxMats = boxRenderer.sharedMaterials;
        for (int i = 0; i < boxMats.Length; i++)
        {
            boxMats[i] = boxMat;
        }
        boxRenderer.sharedMaterials = boxMats;

        tapColl.enabled = false;

        for (int i = 0; i < objBalls.Length; i++)
        {
            FunnelManager.Instance.PreAddBall(color);
        }

        StartCoroutine(IE_Move());
    }

    WaitForSeconds waitForSpawnBall = new WaitForSeconds(0.05f);
    WaitForSeconds waitForDisappear = new WaitForSeconds(0.417f);
    IEnumerator IE_Move()
    {
        Material boxMat = MaterialCache.GetBoxInactiveMat(color);

        for (int i = 0; i < objBalls.Length; i++)
        {
            objBalls[i].SetActive(false);
            PipeHole.Instance.SpawnBoxBall(color, objBalls[i].transform.position);
            yield return waitForSpawnBall;
        }

        BoxManager.Instance.RemoveBox(this);

        anim.SetTrigger("EmptyOut");

        yield return waitForDisappear;

        Recycle();
    }

    public void NoMove(bool isMaxBall)
    {
        if (isMaxBall)
        {
            anim.SetTrigger("CantClickMaxBalls");
        }
        else
        {
            anim.SetTrigger("CantClick");
        }
    }

    public bool CanMoveOut()
    {
        if (BoxManager.Instance.CanMoveOut(pos))
        {
            return true;
        }

        return false;
    }

    public bool CanTap()
    {
        if (isActive)
        {
            return true;
        }

        return false;
    }

    public void Active()
    {
        isActive = true;
        anim.SetTrigger("Activate");

        Material boxMat = MaterialCache.GetBoxActiveSelectableMat(color);
        boxRenderer.sharedMaterial = boxMat;
    }

    public void Active2()
    {
        isActive = true;
        anim.SetTrigger("Activate");

        Material boxMat = MaterialCache.GetBoxActiveSelectableMat(color);
        boxRenderer.sharedMaterial = boxMat;
    }

    public void CheckActive()
    {
        if (CanMoveOut())
        {
            if (!isActive)
            {
                isActive = true;
                anim.SetTrigger("Activate");

                Material boxMat = MaterialCache.GetBoxActiveSelectableMat(color);
                boxRenderer.sharedMaterial = boxMat;
            }
        }
        else
        {
            if (!isActive)
            {

            }
            else
            {
                isActive = false;
                anim.SetTrigger("DeActivate");
            }
        }
    }

    public void Recycle()
    {
        StopAllCoroutines();

        gameObject.Recycle();
    }
}
