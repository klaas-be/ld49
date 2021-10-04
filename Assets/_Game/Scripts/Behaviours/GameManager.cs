using _Game.Scripts.UI;
using Assets.TekkTech.Scripts.Utility;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int LevelIndex = 0;
    [SerializeField] TimeEventController craterTimeController;
    [Space(20)]
    [SerializeField, ReadOnly] private GameState gameState;
    private GameState stateBeforePause;
    [Space(20)]
    [SerializeField] private GameObject storyCanvasGameobject;
    [SerializeField] private UiTextSetter uiTextSetter;
    [SerializeField] private List<LanguageTags> languageTagsLevelStory;
    [SerializeField, ReadOnly] private int storyIndex = -1;
    [Space(20)]
    [SerializeField, ReadOnly] private int startupCounter = 3;

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
    }

    private void Start()
    {
        gameState = GameState.StoryIntro;
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
                storyCanvasGameobject.SetActive(false);
                break;
            case GameState.Gameover:
                break;
            case GameState.Win:
                break;
            default:
                break;
        }
    }

    public void AdvanceStory()
    {
        storyIndex++;
        if (storyIndex >= languageTagsLevelStory.Count)
        {
            gameState = GameState.StartUp;
            Debug.Log(gameState.ToString());
            InvokeRepeating("StartUpTimer", 0f,1f);
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
    }
}
