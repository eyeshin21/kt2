using Lean.Touch;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

public class GameplayController : Singleton<GameplayController>
{
    [SerializeField] private LeanTouch leanTouch;

    [Header("Camera")]
    [SerializeField] private Camera _cam;
    public Camera cam
    {
        get
        {
            if (_cam == null)
            {
                _cam = Camera.main;
            }

            return _cam;
        }
    }

    [Header("Canvas")]
    [SerializeField] private Canvas _canvas;
    public Canvas canvas
    {
        get
        {
            return _canvas;
        }
    }

    [HideInInspector] public bool doneLoadLevel;
    private int amountTap = 0;

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerTap += HandleFingerTap;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening(EventVariables.RecycleLevel, OnReplay);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        leanTouch.ReferenceDpi = 20;
        leanTouch.TapThreshold = 0.1f;
#else
        leanTouch.ReferenceDpi = 200;
        leanTouch.TapThreshold = 0.2f;
#endif
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.EndGame(true);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UserConfig.Instance.CurLevel++;
            UIManager.Instance.ingameMenu.PressedReplayBtn();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UserConfig.Instance.CurLevel--;
            UIManager.Instance.ingameMenu.PressedReplayBtn();
        }
    }
#endif

    public void LoadLevel()
    {
        StartCoroutine(IE_LoadLevel());
    }

    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    WaitForSeconds waitForWarning = new WaitForSeconds(1.5f);
    IEnumerator IE_LoadLevel()
    {
        yield return waitForEndOfFrame;

        int level = GameManager.Instance.prefabLevel;

        TextAsset data = Resources.Load<TextAsset>($"LevelData/{level}");
        LevelData levelData;
        if (data.text.Contains("gridWidth"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(data.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(data.bytes), "DeoDucDu0cD@u"));
        }

        FunnelManager.Instance.Init();
        HoleManager.Instance.Init(levelData.holesData);
        BoxManager.Instance.Init(new Vector2Int(levelData.gridWidth, levelData.gridHeight), levelData.gridSlotsData, levelData.boxesData, levelData.tunnelsData, levelData.pinsData, levelData.clothsData, levelData.lockChainsData);

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);

        doneLoadLevel = true;

        yield return new WaitUntil(() => FadeMenu.Instance.fadeOut);

        CheckTut();

        BoxManager.Instance.CheckActiveBox();

        GameManager.Instance.canControl = true;

        if (eTypeLevel != eTypeLevel.Normal && eTypeLevel != eTypeLevel.Tutorial)
        {
            UIManager.Instance.ShowNoticeHardMenu();
            yield return waitForWarning;
        }

        CheckShowBoosterAndNewElement();
    }

    void HandleFingerDown(LeanFinger finger)
    {
        //if (finger.IsOverGui || !GameManager.Instance.canControl) return;

        /*
        if (UserConfig.Instance.CurLevel == 1)
        {
            if (amountTap == 1 && UIManager.Instance.ingameMenu.tutorialUI.gameObject.activeSelf)
            {
                amountTap++;
                UIManager.Instance.ingameMenu.tutorialUI.Hide();

                string content = "Tap the cat to enter the road";
                Vector2 contentPos = new Vector2(0, 300f);
                Vector2 handAnchorPos = GameplayController.Instance.canvas.WorldToCanvasPosition(BlockManager.Instance.blocks[0].transform.position, GameplayController.Instance.cam);

                Vector2 sizeUnmask = new Vector2(142f, 142f * 6.25f);
                Vector2 middle = (BlockManager.Instance.blocks[2].transform.position + BlockManager.Instance.blocks[3].transform.position) / 2f;
                Vector2 pointUnmask = GameplayController.Instance.canvas.WorldToCanvasPosition(middle, GameplayController.Instance.cam);

                UIManager.Instance.ingameMenu.tutorialUI.ShowHandTutorial(content, sizeUnmask, pointUnmask, contentPos, handAnchorPos, 0);
            }
        }
        */
    }

    void HandleFingerTap(LeanFinger finger)
    {
        //if (finger.IsOverGui || !GameManager.Instance.canControl) return;

        Ray ray = finger.GetRay();
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform.parent.TryGetComponent(out Box box))
            {
                //if (UserConfig.Instance.CurLevel == 1)
                //{
                //    int index = BlockManager.Instance.blocks.IndexOf(block);
                //    if (index <= 5 && (amountTap == 0 || amountTap == 2))
                //    {
                //        amountTap++;
                //        UIManager.Instance.ingameMenu.tutorialUI.Hide();
                //    }
                //    else
                //    {
                //        return;
                //    }
                //}

                /*
                if (conveyorManager.PreCheckFullConveyor())
                {
                    WarningMenu.Instance.Show(Vector2.zero, "The road is full!");
                    block.Shake();
                    SoundManager.instance.PlaySound("Select");
                    HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
                    return;
                }
                */

                box.OnTap();
                //SoundManager.instance.PlaySound("Select");
                //HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
            }
        }
    }

    public void RecycleLevel()
    {
        PipeHole.Instance.Recycle();
        FunnelManager.Instance.Recycle();
        HoleManager.Instance.Recycle();
        BoxManager.Instance.Recycle();
    }

    public void OnReplay()
    {
        GameManager.Instance.canControl = false;
    }

    public void CheckTut()
    {
        amountTap = 0;

        if (UserConfig.Instance.CurLevel == 1)
        {
            //ShooterController tutShooter = ShooterManager.Instance.boxQueues[1].shooters[0];

            //string content = "Tap to move.";
            //Vector2 contentPos = new Vector2(0, 200f);
            //Vector2 handAnchorPos = canvas.WorldToCanvasPosition(tutShooter.txtAmount.transform.position, cam);

            //Vector2 sizeUnmask = new Vector2(250f, 250f);
            //Vector2 pointUnmask = canvas.WorldToCanvasPosition(tutShooter.txtAmount.transform.position, cam);

            //UIManager.Instance.ingameMenu.tutorialUI.ShowHandTutorial(content, sizeUnmask, pointUnmask, contentPos, handAnchorPos, 0);

            CGTeamBridge.Instance.TrackTutAction("start");
        }
        else
        {
            UIManager.Instance.ingameMenu.tutorialUI.Hide();
        }
    }

    public void Revive()
    {

        EventManager.EmitEvent(EventVariables.Revive);
    }

    public void CheckWin()
    {
        if (BoxManager.Instance.boxes.Count == 0 && HoleManager.Instance.IsEmpty())
        {
            GameManager.Instance.EndGame(true);
        }
    }

    public void CheckLose()
    {
        if (GameManager.Instance.canControl)
        {
            if (BoxManager.Instance.boxes.Count == 0) return;

            bool isFunnelFull = false;
            bool isAnyBallMatchAnyHole = FunnelManager.Instance.IsAnyBallMatchAnyHole();
            bool isAnyBoxCanTap = IsAnyBoxCanTap();

            if (FunnelManager.Instance.IsFull(out int totalFreeSlot) || totalFreeSlot < 9)
            {
                isFunnelFull = true;
            }

            if (!isAnyBallMatchAnyHole && (isFunnelFull || !isAnyBoxCanTap))
            {
                GameManager.Instance.EndGame(false);
            }
        }
    }

    public bool IsAnyBoxCanTap()
    {
        List<Box> boxes = BoxManager.Instance.boxes;

        if (boxes.Count > 0)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].CanTap())
                {
                    return true;
                }
            }
        }

        return false;
    }

    void CheckShowBoosterAndNewElement()
    {
        //if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_ADD_SLOT && !PlayerPrefs.HasKey("Introduce_" + eTypeBooster.AddSlot))
        //{
        //    UIManager.Instance.ShowNewBoosterMenu(eTypeBooster.AddSlot);
        //    PlayerPrefs.SetInt("Introduce_" + eTypeBooster.AddSlot, 1);
        //}
        //else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_HAND_SELECT && !PlayerPrefs.HasKey("Introduce_" + eTypeBooster.HandSelect))
        //{
        //    UIManager.Instance.ShowNewBoosterMenu(eTypeBooster.HandSelect);
        //    PlayerPrefs.SetInt("Introduce_" + eTypeBooster.HandSelect, 1);

        //}
        //else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET && !PlayerPrefs.HasKey("Introduce_" + eTypeBooster.Magnet))
        //{
        //    UIManager.Instance.ShowNewBoosterMenu(eTypeBooster.Magnet);
        //    PlayerPrefs.SetInt("Introduce_" + eTypeBooster.Magnet, 1);
        //}

        //if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_HIDDENBOX && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.HiddenBox))
        //{
        //    UIManager.Instance.ShowNewElementMenu(eTypeElement.HiddenBox);
        //    PlayerPrefs.SetInt("Introduce_" + eTypeElement.HiddenBox, 1);
        //}
        //else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_LARGEBOX && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.LargeBox))
        //{
        //    UIManager.Instance.ShowNewElementMenu(eTypeElement.LargeBox);
        //    PlayerPrefs.SetInt("Introduce_" + eTypeElement.LargeBox, 1);
        //}
        //else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_STARBOX && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.StarBox))
        //{
        //    UIManager.Instance.ShowNewElementMenu(eTypeElement.StarBox);
        //    PlayerPrefs.SetInt("Introduce_" + eTypeElement.StarBox, 1);
        //}
        //else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_LINKEDBOX && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.LinkedBox))
        //{
        //    UIManager.Instance.ShowNewElementMenu(eTypeElement.LinkedBox);
        //    PlayerPrefs.SetInt("Introduce_" + eTypeElement.LinkedBox, 1);
        //}
    }
}
