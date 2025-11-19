using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonSceneChange : MonoBehaviour
{
    // Start is called before the first frame update

    public void SetLastScene()
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
    }

    public void ReturnToLastScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("LastScene"));
    }
    public void PlayerVPlayer()
    {
        SceneManager.LoadScene("PVPScene");
    }

    public void DeckSelectorScene()
    {
        SceneManager.LoadScene("DeckSelectionScene");
    }

    public void DeckEditorScene()
    {
        SceneManager.LoadScene("DeckEditorScene");
    }
    public void GameScene()
    {
        SceneManager.LoadScene("GameScene");

    }

    public void GameSceneAI()
    {
        SceneManager.LoadScene("GameSceneAI");
    }
    public void PlayerVAI()
    {
        SceneManager.LoadScene("PlayerVAIScene");
    }
    public void Settings()
    {
        SceneManager.LoadScene("SettingsScene");
    }
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void Tutorial2()
    {
        SceneManager.LoadScene("Tutorial2");
    }
    public void Tutorial3()
    {
        SceneManager.LoadScene("Tutorial3");
    }


    public void StartScreen()
    {
        SceneManager.LoadScene("StartScene");
    }
}
