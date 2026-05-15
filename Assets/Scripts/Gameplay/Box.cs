using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("Data")]
    public bool isActive = false;
    public ColorEnum color;
    public Vector2Int pos;

    [Header("Model")]
    public Animator anim;
    public Transform shake;
    public Transform scale;
    public MeshRenderer boxRenderer;
    public MeshRenderer capRenderer;
    public GameObject objOutline;
    public Collider tapColl;

    [Header("Balls")]
    public MeshRenderer[] ballsRenderer;
    public GameObject[] objBalls;

    [Header("Element Hidden")]
    public bool isHidden;
    public GameObject objHidden;

    [Header("Element Link")]
    public bool isLinked;
    public Vector2Int linkedPos;
    public Box linkedBox;
    public GameObject objLink;

    [Header("Element Ice")]
    public bool hasIce;
    public int iceCount;
    public GameObject objIce;
    public TextMeshPro txtIceCount;
    public GameObject vfxIceBreak;
    public GameObject vfxIceBreakEnd;

    [Header("Element Crate")]
    public bool hasCrate;
    public int crateCount;
    public GameObject objCrate;
    public TextMeshPro txtCrateCount;
    public GameObject vfxCrateBreak;
    public GameObject vfxCrateBreakEnd;

    [Header("Element Pin")]
    public bool hasPin = false;

    [Header("Element Cloth")]
    public bool hasCloth = false;

    [Header("Element LockChain")]
    public bool isLockChain = false;

    [Header("Element Shutter")]
    public bool hasShutter;
    public bool isShutterOpen;
    public GameObject objShutter;
    public Transform[] shutterDoors;

    [Header("Element Lock & Key")]
    public bool isLock;
    public bool isKey;
    public int lockCode;
    public GameObject objLock;
    public GameObject objKey;
    public Transform unlockPos;
    public MeshRenderer lockRenderer;
    public MeshRenderer keyRenderer;
    public MeshRenderer keyAnimRenderer;
    public Animator animLock;
    public Animator animKey;

    private void Start()
    {
        EventManager.StartListening(EventVariables.OnShooterMoveOut, OnShooterMoveOut);
        EventManager.StartListening(EventVariables.CheckShutter, CheckShutter);
        EventManager.StartListening(EventVariables.UnlockShooter, UnlockShooter);
    }

    public void Init(BoxData data)
    {
        pos = new Vector2Int(data.coordinateX, data.coordinateY);
        color = data.color;

        isHidden = data.isHidden;

        isLinked = data.isLink;
        if (isLinked)
        {
            linkedPos = data.linkedPos.ToVector2Int();
        }

        hasIce = data.hasIce;
        iceCount = data.iceCount;
        objIce.SetActive(hasIce);
        txtIceCount.text = iceCount.ToString();

        hasCrate = data.hasCrate;
        crateCount = data.crateCount;
        objCrate.SetActive(hasCrate);
        txtCrateCount.text = crateCount.ToString();

        hasShutter = data.hasShutter;
        isShutterOpen = data.isShutterOpen;
        objShutter.SetActive(data.hasShutter);
        for (int i = 0; i < shutterDoors.Length; i++)
        {
            shutterDoors[i].localScale = isShutterOpen ? new Vector3(0, 1, 1) : Vector3.one;
        }

        if (hasShutter)
        {
            scale.localScale = isShutterOpen ? Vector3.one : Vector3.zero;
            scale.gameObject.SetActive(isShutterOpen);
        }
        else
        {
            scale.gameObject.SetActive(true);
            scale.localScale = Vector3.one;
        }

        isLock = data.isLock;
        isKey = data.isKey;
        lockCode = data.lockCode;
        objLock.SetActive(isLock);
        objKey.SetActive(isKey);

        objKey.transform.localPosition = Vector3.back * 0.7f;
        objKey.transform.localEulerAngles = Vector3.forward * 45f;

        if (isLock || isKey)
        {
            lockRenderer.sharedMaterial = MaterialCache.GetLockMat(lockCode);
            keyRenderer.sharedMaterial = MaterialCache.GetKeyMat(lockCode);
            keyAnimRenderer.sharedMaterial = MaterialCache.GetKeyMat(lockCode);
        }

        if (isKey)
        {
            animKey.SetTrigger("Action");
        }

        transform.localScale = Vector3.one;

        objHidden.SetActive(isHidden);

        Material boxMat;
        if (!isHidden)
        {
            boxMat = MaterialCache.GetBoxInactiveMat(color);
        }
        else
        {
            boxMat = MaterialCache.GetBoxHiddenMat();
        }

        boxRenderer.sharedMaterial = boxMat;
        capRenderer.sharedMaterial = boxMat;
        objOutline.SetActive(false);

        Material ballMat = MaterialCache.GetBallMat(color);
        for (int i = 0; i < ballsRenderer.Length; i++)
        {
            ballsRenderer[i].sharedMaterial = ballMat;
            ballsRenderer[i].gameObject.SetActive(true);
        }
    }

    public void OnShooterMoveOut()
    {
        if (hasIce)
        {
            if (isActive)
            {
                iceCount--;
                txtIceCount.text = iceCount.ToString();

                if (iceCount == 0)
                {
                    hasIce = false;
                    objIce.SetActive(false);

                    Active2();

                    GameObject obj = vfxIceBreakEnd.Spawn(transform);
                    obj.transform.localPosition = Vector3.up * 0.75f;
                    obj.transform.localEulerAngles = Vector3.left * 90f;
                    obj.transform.localScale = Vector3.one;
                }
                else
                {
                    GameObject obj = vfxIceBreak.Spawn(transform);
                    obj.transform.localPosition = Vector3.up * 0.75f;
                    obj.transform.localEulerAngles = Vector3.left * 90f;
                    obj.transform.localScale = Vector3.one;
                    PunchScale();
                }
            }
        }

        if (hasCrate)
        {
            if (isActive)
            {
                crateCount--;
                txtCrateCount.text = crateCount.ToString();

                if (crateCount == 0)
                {
                    hasCrate = false;
                    objCrate.SetActive(false);

                    Active2();

                    GameObject obj = vfxCrateBreakEnd.Spawn(transform);
                    obj.transform.localPosition = Vector3.up * 0.75f;
                    obj.transform.localEulerAngles = Vector3.left * 90f;
                    obj.transform.localScale = Vector3.one;
                }
                else
                {
                    GameObject obj = vfxCrateBreak.Spawn(transform);
                    obj.transform.localPosition = Vector3.up * 0.75f;
                    obj.transform.localEulerAngles = Vector3.left * 90f;
                    obj.transform.localScale = Vector3.one;
                    PunchScale();
                }
            }
        }
    }

    public void CheckShutter()
    {
        if (hasShutter)
        {
            isShutterOpen = !isShutterOpen;

            if (isShutterOpen)
            {
                for (int i = 0; i < shutterDoors.Length; i++)
                {
                    shutterDoors[i].DOKill(true);
                    shutterDoors[i].DOScaleX(0f, 0.25f).SetEase(Ease.Linear);
                }
                scale.transform.DOKill(true);
                scale.gameObject.SetActive(true);
                scale.DOScale(1f, 0.25f).SetDelay(0.1f).SetEase(Ease.OutBack);
            }
            else
            {
                for (int i = 0; i < shutterDoors.Length; i++)
                {
                    shutterDoors[i].DOKill(true);
                    shutterDoors[i].DOScaleX(1f, 0.25f).SetDelay(0.1f).SetEase(Ease.Linear);
                }

                scale.transform.DOKill(true);
                scale.DOScale(0f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    scale.gameObject.SetActive(false);
                });
            }
        }
    }

    public void UnlockShooter()
    {
        if (isLock)
        {
            Box keyBox = (Box)EventManager.GetData(EventVariables.UnlockShooter);
            if (keyBox.lockCode == lockCode)
            {
                objKey.transform.position = keyBox.objKey.transform.position;
                objKey.transform.eulerAngles = keyBox.objKey.transform.eulerAngles;
                objKey.SetActive(true);

                Vector3[] path = new Vector3[]
                {
                    ((objKey.transform.position + unlockPos.position) / 2) + Vector3.back * 2f,
                    unlockPos.position
                };

                objKey.transform.DOScale(2f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    objKey.transform.DOScale(1f, 0.25f).SetEase(Ease.Linear);
                });
                objKey.transform.DORotateQuaternion(unlockPos.rotation, 0.5f).SetEase(Ease.Linear);
                objKey.transform.DOPath(path, 0.5f, PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.Linear).OnComplete(() =>
                {
                    objKey.SetActive(false);
                    animLock.SetTrigger("Unlock");
                    DOVirtual.DelayedCall(0.5f, OnEndAnimUnlock, false);
                });
            }
        }
    }

    public void OnEndAnimUnlock()
    {
        isLock = false;
        objLock.SetActive(false);
        if (isActive)
        {
            Active2();
        }
    }

    public void CheckLink()
    {
        if (isLinked)
        {
            if (linkedBox == null)
            {
                linkedBox = BoxManager.Instance.boxGrid[linkedPos.x, linkedPos.y];
                linkedBox.linkedBox = this;

                objLink.SetActive(true);

                if (!isActive)
                {
                    objLink.transform.GetChild(0).localPosition = new Vector3(0, 0.5f, -0.273f);
                }
                else
                {
                    objLink.transform.GetChild(0).localPosition = new Vector3(0, 0.5f, -0.387f);
                }

                if (pos.x == linkedPos.x)
                {
                    if (pos.y + 1 == linkedPos.y)
                    {
                        objLink.transform.localEulerAngles = Vector3.zero;
                    }
                    else if (pos.y - 1 == linkedPos.y)
                    {
                        objLink.transform.localEulerAngles = Vector3.forward * 180f;
                    }
                }
                else if (pos.y == linkedPos.y)
                {
                    if (pos.x + 1 == linkedPos.x)
                    {
                        objLink.transform.localEulerAngles = Vector3.forward * 270f;
                    }
                    else if (pos.x - 1 == linkedPos.x)
                    {
                        objLink.transform.localEulerAngles = Vector3.forward * 90f;
                    }
                }
            }
        }
    }

    public void ShowHidden()
    {
        if (isHidden)
        {
            isHidden = false;
            objHidden.SetActive(false);

            GameObject obj = GameManager.Instance.InstantiatePrefab("VFX/VFX_Show");
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.up * 0.5f;
            obj.transform.localScale = Vector3.one;

            if (isActive && !hasIce && !isLock && !hasCrate && !hasPin && !hasCloth && !isLockChain)
            {
                Material boxMat = MaterialCache.GetBoxActiveMat(color);
                boxRenderer.sharedMaterial = boxMat;
                capRenderer.sharedMaterial = boxMat;
                objOutline.SetActive(true);
            }
            else
            {
                Material boxMat = MaterialCache.GetBoxInactiveMat(color);
                boxRenderer.sharedMaterial = boxMat;
                capRenderer.sharedMaterial = boxMat;
                objOutline.SetActive(false);
            }
        }
    }

    public void OnTap()
    {
        if (isActive && !isHidden && !hasIce && !isLock && !hasCrate && !hasPin && !hasCloth && !isLockChain)
        {
            if (hasShutter && !isShutterOpen) return;

            if (!isLinked)
            {
                if (!FunnelManager.Instance.IsFull(out int totalFreeSlot) && totalFreeSlot >= 9)
                {
                    Move();
                }
                else
                {
                    Shake();
                }
            }
            else
            {
                if (!FunnelManager.Instance.IsFull(out int totalFreeSlot) && totalFreeSlot >= 18)
                {
                    Move();
                    linkedBox.Move();

                    objLink.SetActive(false);
                    linkedBox.objLink.SetActive(false);

                    isLinked = false;
                    linkedBox.isLinked = false;
                    linkedBox.linkedBox = null;
                    linkedBox = null;
                }
                else
                {
                    Shake();
                    linkedBox.Shake();
                }
            }
        }
        else
        {
            Shake();
        }
    }

    public void MoveToSpawnPos(bool fromTunnel = false)
    {
        Vector3 spawnWorldPos = BoxManager.Instance.GetWorldPos(pos.x, pos.y);
        transform.DOLocalMove(spawnWorldPos, 10f).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
        {
            Active2();
        });

        if (fromTunnel)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }
    }

    public void CheckKey()
    {
        if (isKey)
        {
            isKey = false;
            objKey.SetActive(false);
            EventManager.EmitEventData(EventVariables.UnlockShooter, this);
        }
    }

    public void DisableShutter()
    {
        if (hasShutter)
        {
            hasShutter = false;
            objShutter.SetActive(false);
        }
    }

    public void Move()
    {
        EventManager.EmitEvent(EventVariables.OnShooterMoveOut);

        CheckKey();
        DisableShutter();

        Material boxMat = MaterialCache.GetBoxInactiveMat(color);
        boxRenderer.sharedMaterial = boxMat;
        capRenderer.sharedMaterial = boxMat;
        objOutline.SetActive(false);

        tapColl.enabled = false;

        for (int i = 0; i < objBalls.Length; i++)
        {
            FunnelManager.Instance.PreAddBall(color);
        }

        StartCoroutine(IE_Move());

        EventManager.EmitEvent(EventVariables.CheckShutter);
        EventManager.EmitEvent(EventVariables.CheckCloth);
    }

    WaitForSeconds waitForSpawnBall = new WaitForSeconds(0.05f);
    WaitForSeconds waitForDisappear = new WaitForSeconds(0.3f);
    IEnumerator IE_Move()
    {
        Material boxMat = MaterialCache.GetBoxInactiveMat(color);

        for (int i = 0; i < objBalls.Length; i++)
        {
            objBalls[i].SetActive(false);
            PipeHole.Instance.SpawnBoxBall(color, objBalls[i].transform.position);
            yield return waitForSpawnBall;
        }

        transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);

        yield return waitForDisappear;

        BoxManager.Instance.RemoveBox(this);
        EventManager.EmitEvent(EventVariables.CheckPin);

        Recycle();
    }

    public bool CanMoveOut()
    {
        if (BoxManager.Instance.CanMoveOut(pos) || (isLinked && BoxManager.Instance.CanMoveOut(linkedPos)))
        {
            return true;
        }

        return false;
    }

    public bool CanTap()
    {
        if (isActive && !hasIce && !isLock && !hasCrate && !hasPin && !hasCloth && !isLockChain)
        {
            if (hasShutter && !isShutterOpen) return false;

            if (!FunnelManager.Instance.IsFull(out int totalFreeSlot))
            {
                if (!isLinked)
                {
                    return totalFreeSlot >= 9;
                }
                else
                {
                    return totalFreeSlot >= 18;
                }
            }
        }

        return false;
    }

    public void Active2()
    {
        isActive = true;

        if (hasIce)
        {

        }
        else if (hasCrate)
        {

        }
        else if (hasPin)
        {

        }
        else if (hasCloth)
        {

        }
        else if (isLock)
        {

        }
        else if (isLockChain)
        {

        }
        else if (isHidden)
        {

        }
        else
        {
            anim.SetTrigger("Active");

            Material boxMat = MaterialCache.GetBoxActiveMat(color);
            boxRenderer.sharedMaterial = boxMat;
            capRenderer.sharedMaterial = boxMat;
            objOutline.SetActive(true);

            if (isHidden)
            {
                isHidden = false;
                objHidden.SetActive(false);

                GameObject obj = GameManager.Instance.InstantiatePrefab("VFX/VFX_Show");
                obj.transform.parent = transform;
                obj.transform.localPosition = Vector3.up * 0.5f;
                obj.transform.localScale = Vector3.one;
            }

            if (isLinked)
            {
                Box linkedBox = BoxManager.Instance.boxGrid[linkedPos.x, linkedPos.y];
                if (!linkedBox.isActive)
                {
                    linkedBox.Active2();
                }

                //if (objLink.activeSelf)
                //{
                //    objLink.transform.GetChild(0).localPosition = new Vector3(0, 0.5f, -0.387f);
                //}
            }
        }
    }

    public void CheckActive()
    {
        if (CanMoveOut())
        {
            if (!isActive)
            {
                isActive = true;

                if (hasIce)
                {

                }
                else if (hasCrate)
                {

                }
                else if (hasPin)
                {

                }
                else if (hasCloth)
                {

                }
                else if (isLock)
                {

                }
                else if (isLockChain)
                {

                }
                else if (isHidden)
                {

                }
                else
                {
                    anim.SetTrigger("Active");

                    Material boxMat = MaterialCache.GetBoxActiveMat(color);
                    boxRenderer.sharedMaterial = boxMat;
                    capRenderer.sharedMaterial = boxMat;
                    objOutline.SetActive(true);

                    //if (isLinked && objLink.activeSelf)
                    //{
                    //    objLink.transform.GetChild(0).localPosition = new Vector3(0, 0.5f, -0.387f);
                    //}
                }
            }
        }
        else
        {
            if (!isActive)
            {
                objOutline.SetActive(false);
            }
            else
            {
                isActive = false;
                anim.SetTrigger("DeActive");

                Material boxMat = MaterialCache.GetBoxInactiveMat(color);
                boxRenderer.sharedMaterial = boxMat;
                capRenderer.sharedMaterial = boxMat;
                objOutline.SetActive(false);
            }
        }
    }

    public void PunchScale()
    {
        scale.DOKill(true);
        scale.DOPunchScale(Vector3.one * 0.3f, 0.25f).SetEase(Ease.Linear);
    }

    public void Shake()
    {
        shake.DOKill(true);
        shake.DOPunchRotation(Vector3.up * 20f, 0.3f).SetEase(Ease.Linear);
    }

    public void Recycle()
    {
        tapColl.enabled = true;
        transform.DOKill();
        StopAllCoroutines();

        isActive = false;
        hasIce = false;
        iceCount = 0;
        isLock = false;
        isKey = false;
        isHidden = false;
        hasCrate = false;
        crateCount = 0;
        hasPin = false;
        hasCloth = false;
        hasShutter = false;
        isShutterOpen = false;
        isLockChain = false;

        objHidden.SetActive(false);
        objLink.SetActive(false);
        objIce.SetActive(false);
        objCrate.SetActive(false);
        objShutter.SetActive(false);
        linkedBox = null;

        scale.localScale = Vector3.one;

        gameObject.Recycle();
    }
}
