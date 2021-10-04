using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.TekkTech.Scripts.Language;

public class StateHolder : MonoBehaviour
{

    public static StateHolder instance;

    public bool playSoundSetting = true;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
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
    }
    
}
