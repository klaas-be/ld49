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
    [SerializeField, ReadOnly] private GameState stateBeforePause;
    [Space(20)]
    [SerializeField] private UiTextSetter uiTextSetter;
    [SerializeField] private List<LanguageTags> languageTagsLevelStory;
    [SerializeField, ReadOnly] private int storyIndex = -1;

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
        if (storyIndex == languageTagsLevelStory.Count - 1)
        {
            gameState = GameState.StartUp;
            return;
        }

        storyIndex++;

        uiTextSetter.localizedText.languageTag = languageTagsLevelStory[storyIndex];
        uiTextSetter.SetText();
    }
}
