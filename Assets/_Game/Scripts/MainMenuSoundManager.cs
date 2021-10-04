using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSoundManager : MonoBehaviour
{
    public static MainMenuSoundManager instance;

    [SerializeField]
    List<AudioSource> audioSources;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void SetSound(bool state) {
        for (int i = 0; i < audioSources.Count; i++) {
            audioSources[i].mute = !state;
        }

        Debug.Log("Sound Setting switched!");
    }
}
