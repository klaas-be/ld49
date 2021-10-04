using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.TekkTech.Scripts.Language;
using UnityEngine.SceneManagement;

public class StateHolder : MonoBehaviour
{

    public static StateHolder instance;

    public bool playSoundSetting = true;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void ToggleSound() {
        playSoundSetting = !playSoundSetting;
        MainMenuSoundManager.instance.SetSound(playSoundSetting);
    }


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(LocalizationManager.instance.gameObject);
        
        MainMenuSoundManager.instance.SetSound(playSoundSetting);
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        MainMenuSoundManager.instance.SetSound(playSoundSetting);
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
