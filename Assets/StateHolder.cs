using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.TekkTech.Scripts.Language;

public class StateHolder : MonoBehaviour
{

    public bool playSoundSetting = true;

    public void ToggleSound() {
        playSoundSetting = !playSoundSetting;
    }


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(LocalizationManager.instance.gameObject);
    }
    
}
