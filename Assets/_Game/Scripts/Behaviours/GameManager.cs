using _Game.Scripts.Behaviours;
using _Game.Scripts.UI;
using Assets.TekkTech.Scripts.Utility;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Space(20), Header("Links")]
    [SerializeField, ReadOnly] private CharacterMovement PlayerController;
    [SerializeField, ReadOnly] private RecipeQueue CraterQueueController;
    [SerializeField, ReadOnly] private Crater CraterController;
    [SerializeField] TimeEventController craterTimeController;

    [Space(20), Header("Level Settings")]
    [SerializeField, ReadOnly] private GameState gameState;
    [SerializeField, ReadOnly] private GameState stateBeforePause;
    [SerializeField] private bool isTutorial = false;
    [SerializeField] private int nextSceneAfterWinID;
    [SerializeField] private int loseSceneID;
    [SerializeField] private int secondsUntilWin = 120;
    [SerializeField, ReadOnly] private float secondsTimer = 0;
    [Space(10)]
    [SerializeField] private float craterMaxInstability = 100f;
    [SerializeField, ReadOnly] private float craterCurrentInstability;
    [SerializeField] private float instabilityDowngradeMultiplier = 1f;

    [Space(20), Header("Story UI Settings")]
    [SerializeField] private GameObject storyCanvasGameobject;
    [SerializeField] private UiTextSetter uiTextSetter;
    [SerializeField] private List<LanguageTags> languageTagsLevelStory;
    [SerializeField, ReadOnly] private int storyIndex = -1;

    [Space(20), Header("Ingame UI Settings")]
    [SerializeField] private GameObject IngameUI;
    [SerializeField] private GameObject CraterScannerUI;
    [SerializeField] private GameObject InstabilityUI;
    [SerializeField] private RectTransform boostBarTransform;
    [SerializeField] private TMPro.TextMeshProUGUI TimeLeftTimeText;
    [SerializeField] private RectTransform instabilityBarTransform;

    [Space(20), Header("Win Menu Settings")]
    [SerializeField] private GameObject WinMenu;
    [SerializeField] private GameObject WinMenuKeyText;
    [SerializeField] private float timeToContinue = 2f;
    [SerializeField, ReadOnly] private bool canContinue = false;

    [Space(20), Header("Gameover Menu Settings")]
    [SerializeField] private GameObject GameoverMenu;
    [SerializeField] private GameObject GameoverMenuKeyText;

    [Space(20)]
    private int startupCounter = 3;

    public bool CanPlayerMove { get { return gameState == GameState.InGame; } }

    public enum GameState
    {
        StoryIntro,
        StartUp,
        Pause,
        InGame,
        Gameover,
        Win,
    }

    private void Awake()
    {
        if (!instance)
            instance = this;

        PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        CraterQueueController = GameObject.FindGameObjectWithTag("Crater").GetComponent<RecipeQueue>();
        CraterController = GameObject.FindGameObjectWithTag("Crater").GetComponent<Crater>();
    }

    private void Start()
    {
        IngameUI.SetActive(false);

        if (isTutorial)
        {
            SetIngame();
            return;
        }

        AdvanceStory();
    }

    public void Pause(bool isPaused)
    {
        Debug.Log("Pause");
        if (isPaused)
        {
            stateBeforePause = gameState;
            gameState = GameState.Pause;
        }
        else
        {
            gameState = stateBeforePause;
        }
    }

    public void Update()
    {
        switch (gameState)
        {
            case GameState.StoryIntro:
                if (Input.anyKeyDown)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                        break;

                    AdvanceStory();
                }
                break;
            case GameState.StartUp:
                break;
            case GameState.Pause:
                break;
            case GameState.InGame:
                IngameUpdate();
                break;
            case GameState.Win:
                if (Input.anyKeyDown && canContinue)
                {
                    SetTransition(nextSceneAfterWinID);
                }
                break;
            case GameState.Gameover:
                SetTransition(loseSceneID);
                break;
            default:
                break;
        }
    }

    private void IngameUpdate()
    {
        boostBarTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, -boostBarTransform.sizeDelta.y), PlayerController._dashCooldownTimer / PlayerController._dashCooldown);

        if (isTutorial)
            return;

        secondsTimer += Time.deltaTime;

        if (secondsTimer >= secondsUntilWin)
        {
            SetWinLevel();
        }

        //Timer
        TimeSpan time = TimeSpan.FromSeconds(secondsUntilWin - secondsTimer);
        TimeLeftTimeText.text = time.ToString(@"mm\:ss\:fff");

        //Crater Instabilty
        craterCurrentInstability -= Time.deltaTime * instabilityDowngradeMultiplier * CraterQueueController.queue.Count;
        if (craterCurrentInstability <= 0f)
        {
            SetGameOver();
        }

        craterCurrentInstability = Mathf.Clamp(craterCurrentInstability, 0, craterMaxInstability);
        float instabiltity01 = 1 - craterCurrentInstability / craterMaxInstability;
        instabilityBarTransform.localScale = new Vector3(instabiltity01, 1, 1);
        CraterController.SetLavaLevel(instabiltity01);
    }

    public void AdvanceStory()
    {
        storyCanvasGameobject.SetActive(true);
        storyIndex++;
        if (storyIndex >= languageTagsLevelStory.Count)
        {
            gameState = GameState.StartUp;
            InvokeRepeating("StartUpTimer", 0f, 1f);
            return;
        }


        uiTextSetter.localizedText.languageTag = languageTagsLevelStory[storyIndex];
        uiTextSetter.SetText();
    }

    public void StartUpTimer()
    {
        switch (startupCounter)
        {
            case 3:
                uiTextSetter.localizedText.languageTag = LanguageTags.StartupCount_3;
                break;
            case 2:
                uiTextSetter.localizedText.languageTag = LanguageTags.StartupCount_2;
                break;
            case 1:
                uiTextSetter.localizedText.languageTag = LanguageTags.StartupCount_1;
                break;
            case 0:
                uiTextSetter.localizedText.languageTag = LanguageTags.StartupCount_Go;
                break;
            default:
                break;
        }

        uiTextSetter.SetText();
        startupCounter--;

        if (startupCounter < -1)
        {
            SetIngame();
            CancelInvoke("StartUpTimer");
        }
    }

    public void SetIngame()
    {
        gameState = GameState.InGame;
        storyCanvasGameobject.SetActive(false);
        IngameUI.SetActive(true);
        craterCurrentInstability = craterMaxInstability;

        if (isTutorial)
        {
            CraterScannerUI.SetActive(true);
            InstabilityUI.SetActive(false);
        }
        else
        {
            craterTimeController.isOn = true;
        }
    }
    [Button()]
    public void SetGameOver()
    {
        gameState = GameState.Gameover;
    }

    [Button()]
    public void SetWinLevel()
    {
        canContinue = false;
        gameState = GameState.Win;
        craterTimeController.isOn = false;
        IngameUI.SetActive(false);

        WinMenu.SetActive(true);
        WinMenuKeyText.SetActive(false);
        Invoke("SetWinKeyTextOn", timeToContinue);
    }
    private void SetWinKeyTextOn()
    {
        canContinue = true;
        WinMenuKeyText.SetActive(true);
    }

    public void SetTransition(int id)
    {
        SceneManager.LoadScene(id, LoadSceneMode.Single);
    }

    public void CraterAddBonusFromItem(float bonus)
    {
        craterCurrentInstability += bonus;
        craterCurrentInstability = Mathf.Clamp(craterCurrentInstability, 0, craterMaxInstability);
    }
}
