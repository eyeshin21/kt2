using Newtonsoft.Json;
using System.Collections;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public const int MAX_LEVEL = 150;
    [HideInInspector]
    public bool canControl;
    //[HideInInspector]
    //public LevelController currentLevel;
    [HideInInspector]
    public int prefabLevel;

    [HideInInspector]
    public LevelInfo[] levelInfos;

    [HideInInspector]
    public float startTime;
    [HideInInspector]
    public int tapCheat;
    [HideInInspector]
    public float startTimeCheat;

    [HideInInspector]
    public float screenHeight;
    [HideInInspector]
    public float screenWidth;

    Rect safeArea;
    [HideInInspector]
    public Vector2 minAnchor;
    [HideInInspector]
    public Vector2 maxAnchor;
    [HideInInspector]
    public float safeAreaYBottom;
    [HideInInspector]
    public float safeAreaYTop;

    [HideInInspector]
    public int amountBusServe;

    private void Start()
    {
        safeArea = Screen.safeArea;
        minAnchor = safeArea.position;
        maxAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        float height = (float)Screen.height;
        float width = (float)Screen.width;

        float ratio = width / height;
        float ratioDefault = 720.0f / 1280.0f;
        if (ratio <= ratioDefault)
        {
            screenWidth = 1080.0f;
            screenHeight = Screen.height * (screenWidth / Screen.width);
        }
        else if (ratio > ratioDefault)
        {
            screenHeight = 1920.0f;
            screenWidth = Screen.width * (screenHeight / Screen.height);
        }

        safeAreaYBottom = minAnchor.y * screenHeight;
        safeAreaYTop = (1 - maxAnchor.y) * screenHeight;

        SoundManager.InitInstance(transform);

        TextAsset textAssetLevelInfo = Resources.Load<TextAsset>("LevelInfo");
        levelInfos = JsonConvert.DeserializeObject<LevelInfo[]>(textAssetLevelInfo.text);
    }

    public void LoadLevel()
    {
        //canControl = true;
        startTime = Time.time;

        EventManager.EmitEvent(EventVariables.LoadLevel);
        prefabLevel = UserConfig.Instance.CurLevel;

        //prefabLevel = UserConfig.Instance.CurLevel % MAX_LEVEL;
        //if (prefabLevel == 0)
        //{
        //    prefabLevel = MAX_LEVEL;
        //}

        //if (RemoteConfig.Instance.levelConfig != null)
        //{
        //    if (RemoteConfig.Instance.levelConfig.levels != null)
        //    {
        //        if (UserConfig.Instance.CurLevel - 1 < RemoteConfig.Instance.levelConfig.levels.Length)
        //        {
        //            prefab = RemoteConfig.Instance.levelConfig.levels[UserConfig.Instance.CurLevel - 1].prefab;
        //        }
        //    }
        //}

        if (prefabLevel > MAX_LEVEL)
        {
            if (UserConfig.Instance.NextLevel == -1 || UserConfig.Instance.NextLevel > MAX_LEVEL)
            {
                UserConfig.Instance.NextLevel = UserConfig.Instance.RandomNextLevel();
            }

            prefabLevel = UserConfig.Instance.NextLevel;
        }

        //string assetPath = "Levels/" + prefabLevel;
        //RecycleLevel();

        //GameObject obj = InstantiatePrefab(assetPath);
        //currentLevel = obj.GetComponent<LevelController>();

        GameplayController.Instance.RecycleLevel();
        GameplayController.Instance.LoadLevel();

        UserConfig.Instance.PlayCount++;

        int curLevel = UserConfig.Instance.CurLevel;
        eTypeLevel eTypeLevel = GetTypeLevel(curLevel);
        CGTeamBridge.Instance.OnGameStarted(curLevel, eTypeLevel.ToString());
    }

    public eTypeLevel GetTypeLevel(int level)
    {
        int prefab = level;
        //if (RemoteConfig.Instance.levelConfig != null)
        //{
        //    if (RemoteConfig.Instance.levelConfig.levels != null)
        //    {
        //        if (UserConfig.Instance.CurLevel - 1 < RemoteConfig.Instance.levelConfig.levels.Length)
        //        {
        //            prefab = RemoteConfig.Instance.levelConfig.levels[UserConfig.Instance.CurLevel - 1].prefab;
        //        }
        //    }
        //}
        if (prefab > MAX_LEVEL)
        {
            return eTypeLevel.Normal;
        }

        return levelInfos[prefab - 1].eTypeLevel;
        //return scriptableObjectLevel.levelInfos[prefab - 1].eTypeLevel;
    }

    public int GetCoinRewardLevel(int level)
    {
        int prefab = level;
        //if (RemoteConfig.Instance.levelConfig != null)
        //{
        //    if (RemoteConfig.Instance.levelConfig.levels != null)
        //    {
        //        if (UserConfig.Instance.CurLevel - 1 < RemoteConfig.Instance.levelConfig.levels.Length)
        //        {
        //            prefab = RemoteConfig.Instance.levelConfig.levels[UserConfig.Instance.CurLevel - 1].prefab;
        //        }
        //    }
        //}
        if (prefab > MAX_LEVEL)
        {
            return 10;
        }

        return levelInfos[prefab - 1].coin;
        //return scriptableObjectLevel.levelInfos[prefab - 1].coin;
    }

    public ProgressElement GetProgressElement()
    {
        if (UserConfig.Instance.CurLevel < MAX_LEVEL - 1)
        {
            eTypeElement newElement = eTypeElement.None;
            int prevIntroElement = 1;
            int nextIntroElement = 1;
            for (int i = UserConfig.Instance.CurLevel; i < levelInfos.Length; i++)
            {
                if (levelInfos[i].eTypeElement != eTypeElement.None)
                {
                    newElement = levelInfos[i].eTypeElement;
                    nextIntroElement = i + 1;
                    break;
                }
            }

            if (newElement != eTypeElement.None)
            {
                for (int i = UserConfig.Instance.CurLevel - 1; i >= 0; i--)
                {
                    if (levelInfos[i].eTypeElement != eTypeElement.None || i == 0)
                    {
                        prevIntroElement = i + 1;
                        break;
                    }
                }

                return new ProgressElement(prevIntroElement, nextIntroElement, newElement);
            }
        }

        return null;
    }

    public void OutOfSpace()
    {
        EventManager.EmitEvent(EventVariables.EndGame);
        canControl = false;
        Invoke(nameof(OnOutOfSpace), 0.1f);
    }

    public void EndGame(bool win, float delay = 1.0f)
    {
        if (canControl)
        {
            EventManager.EmitEvent(EventVariables.EndGame);
            canControl = false;
            if (win)
            {
                Invoke(nameof(OnWin), delay);
                UIManager.Instance.vfxWin.Play();
                SoundManager.instance.PlaySound("Confetti");
            }
            else
            {
                //if (ParkingManager.Instance.CanAddSlot())
                //{
                //    Invoke(nameof(OnOutOfSpace), delay);
                //}
                //else
                //{
                    Invoke(nameof(OnLose), delay);
                //}
            }
        }
    }

    void OnWin()
    {
        SoundManager.instance.PlaySound("Win");
        UIManager.Instance.ShowWinMenu();

        int levelWin = UserConfig.Instance.CurLevel - 1;
        CGTeamBridge.Instance.OnGameFinished(true, 1, levelWin);
    }

    public void OnLose()
    {
        SoundManager.instance.PlaySound("Lose");
        UIManager.Instance.ShowLoseMenu();

        int totalItems = 0;
        int cleardItems = 0;
        float progress = cleardItems / (float)totalItems;
        CGTeamBridge.Instance.OnGameFinished(false, progress, UserConfig.Instance.CurLevel);
    }

    void OnOutOfSpace()
    {
        SoundManager.instance.PlaySound("Lose");
        UIManager.Instance.ShowOutOfSpaceMenu();

        int totalItems = 0;
        int cleardItems = 0;
        float progress = cleardItems / (float)totalItems;
        CGTeamBridge.Instance.OnGameStep(UserConfig.Instance.CurLevel, progress, "soft_fail");
    }

    public void RecycleLevel()
    {
        //if (currentLevel != null)
        //{
        //    currentLevel.RecycleLevel();
        //}
    }

    private GameObject LoadAsset(string assetName)
    {
        GameObject gameObject = null;
        gameObject = Resources.Load<GameObject>(assetName);

        if (gameObject != null)
        {
            return gameObject;
        }
        return null;
    }

    public GameObject InstantiatePrefab(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            return null;
        }
        GameObject gameObject = LoadAsset(assetName);
        if (gameObject != null)
        {
            GameObject gameObject2 = gameObject.Spawn();
            return gameObject2;
        }
        return gameObject;
    }

    public Sprite LoadSprite(string assetName)
    {
        Sprite sprite = null;
        sprite = Resources.Load<Sprite>(assetName);

        if (sprite != null)
        {
            return sprite;
        }
        return null;
    }

    public void LoadScene(int id, UnityEvent evt)
    {
        UnityEvent e = new UnityEvent();
        e.AddListener(() =>
        {
            StartCoroutine(LoadAsyncGame(id));

            if (evt != null)
            {
                evt.Invoke();
            }
        });
        FadeMenu.Instance.Fade(e);
    }

    IEnumerator LoadAsyncGame(int scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = operation.progress;
            if (progress == 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
