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
    GameObject MenuCanvas;
    [SerializeField]
    GameObject SettingsPanel;
    [SerializeField]
    GameObject CreditsPanel;
    [SerializeField]
    TextMeshProUGUI currentLanguageTextField;

    private void Start()
    {
        Invoke("CanvasSetActive", 0.1f);
    }

    private void CanvasSetActive()
    {
        MenuCanvas.SetActive(true);
    }

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
            CreditsPanel.SetActive(false);
        }
    }

    public void ToggleCredits() {
        if (!CreditsPanel) return;

        if (CreditsPanel.activeSelf == true) {
            CreditsPanel.SetActive(false);
        } else {
            CreditsPanel.SetActive(true);
            SettingsPanel.SetActive(false);
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
