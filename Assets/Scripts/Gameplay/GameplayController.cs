using DG.Tweening.Core.Easing;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : Singleton<GameplayController>
{
    public LevelData levelData;

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
        LoadLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel()
    {
        FunnelManager.Instance.Init();
        HoleManager.Instance.Init(levelData.holesDataDefault, levelData.queueHoles);
        BoxManager.Instance.Init(new Vector2Int(levelData.gridWidth, levelData.gridHeight), levelData.gridSlotsData, levelData.boxesData);
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
}
