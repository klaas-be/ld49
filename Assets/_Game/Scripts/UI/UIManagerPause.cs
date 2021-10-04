using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerPause : MonoBehaviour
{
    [SerializeField]
    private Canvas PauseMenu;
    bool toggleCanvas;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || toggleCanvas) {
            PauseMenu.gameObject.SetActive(!PauseMenu.gameObject.activeSelf);

            if (PauseMenu.gameObject.activeSelf)
            {
                //gamemanager event "pause" mit "true" aufrufen
                //timescale 0
                GameManager.instance.Pause(true);
                Time.timeScale = 0;
            }
            else
            {
                //gamemanager event "pause" mit "false" aufrufen
                //timescale normal
                GameManager.instance.Pause(false);
                Time.timeScale = 1;
            }

            toggleCanvas = false;
        }
    }

    public void ResumeGame() {
        toggleCanvas = true;
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
