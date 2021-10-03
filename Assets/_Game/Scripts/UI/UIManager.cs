using Assets.TekkTech.Scripts.Language;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    int newGame, howToPlay;
    [SerializeField]
    GameObject SettingsPanel;
    [SerializeField]
    TextMeshProUGUI currentLanguageTextField;

    public void CircleLeft() {
        if(LocalizationManager.instance.currentLanguage > 0) {
            LocalizationManager.ChangeLanguage(LocalizationManager.instance.currentLanguage - 1);
        } else {
            LocalizationManager.ChangeLanguage((Languages)System.Enum.GetValues(typeof(Languages)).Length-1);
        }
        currentLanguageTextField.text = LocalizationManager.instance.currentLanguage.ToString();
    }

    public void CircleRight() {
        if (LocalizationManager.instance.currentLanguage < ((Languages)System.Enum.GetValues(typeof(Languages)).Length - 1)) {
            LocalizationManager.ChangeLanguage(LocalizationManager.instance.currentLanguage + 1);
        } else {
            LocalizationManager.ChangeLanguage(0);
        }
        currentLanguageTextField.text = LocalizationManager.instance.currentLanguage.ToString();
    }



    public void LoadNewGame() {
        SceneManager.LoadScene(newGame, LoadSceneMode.Single);
    }

    public void LoadHowToPlay() {
        SceneManager.LoadScene(howToPlay, LoadSceneMode.Single);
    }

    public void ToggleSettings() {
        if (!SettingsPanel) return;

        if(SettingsPanel.activeSelf == true) {
            SettingsPanel.SetActive(false);
        } else {
            SettingsPanel.SetActive(true);
        }
    }

    public void CloseApplication() {
        Debug.Log("Closing Application...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        
        Application.Quit();
    }

}
