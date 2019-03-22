using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField]
    GameObject mainMenu, pauseMenu, inGameInfo, ganaste, perdiste, pauseButton, marckButton;

    GameState gameState = GameState.InLobby;

    public enum GameState {InLobby, InGame }

	// Use this for initialization
	void Start () {

        MainMenuGui.instance.OnStartGame += GoGame;
        Celda.OnBombActive += LoseGame;
        Level.instance.OnWin += WinGame;

        pauseMenu.SetActive(false);
        ganaste.SetActive(false);
        perdiste.SetActive(false);
        inGameInfo.SetActive(false);
        pauseButton.SetActive(false);
        marckButton.SetActive(false);


    }

    public void PauseButon()
    {
        if(gameState == GameState.InGame)
        {
            if (pauseMenu.activeSelf)
                Continue();
            else
                Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }
    public void Continue()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    void GoGame(int n, string s, string s2)
    {
        gameState = GameState.InGame;
        mainMenu.SetActive(false);
        inGameInfo.SetActive(true);
        pauseButton.SetActive(true);
        marckButton.SetActive(true);
    }

    public void GoLobby()
    {
        pauseMenu.SetActive(false);
        ganaste.SetActive(false);
        perdiste.SetActive(false);
        gameState = GameState.InLobby;
        mainMenu.SetActive(true);
        inGameInfo.SetActive(false);
        pauseButton.SetActive(false);
        marckButton.SetActive(false);
    }

    
    public void Restart()
    {
        Level.instance.Reiniciar();
    }
    void LoseGame()
    {
            StartCoroutine(LoseGameCorrutine());
    }
    void WinGame()
    {
        StartCoroutine(WinGameCorrutine());
    }

    IEnumerator WinGameCorrutine()
    {
        ganaste.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        ganaste.SetActive(false);
        GoLobby();


    }
    IEnumerator LoseGameCorrutine()
    {
        perdiste.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        perdiste.SetActive(false);
        GoLobby();

    }

}
