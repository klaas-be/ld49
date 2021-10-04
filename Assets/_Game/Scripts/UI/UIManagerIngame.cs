using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerIngame : MonoBehaviour
{
    [SerializeField]
    private Canvas IngameMenu;
    bool toggleCanvas;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || toggleCanvas) {
            IngameMenu.gameObject.SetActive(!IngameMenu.gameObject.activeSelf);
            toggleCanvas = false;
        }

        if(IngameMenu.gameObject.activeSelf) {
            //gamemanager event "pause" mit "true" aufrufen
            //timescale 0
            Time.timeScale = 0;
        } else {
            //gamemanager event "pause" mit "false" aufrufen
            //timescale normal
            Time.timeScale = 1;
        }
    }

    public void ResumeGame() {
        toggleCanvas = true;
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
