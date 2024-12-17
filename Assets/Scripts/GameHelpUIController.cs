using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHelpUIController : MonoBehaviour
{
    public List<GameObject> screens = new List<GameObject>();
    private int currentScreenIndex = 0;

    void Start()
    {
        // Ensure only the first screen is active at the start
        ShowScreen(currentScreenIndex);
    }

    void ShowScreen(int index)
    {
        // Hide all screens
        foreach (var screen in screens)
        {
            screen.SetActive(false);
        }

        // Show the selected screen
        if (index >= 0 && index < screens.Count)
        {
            screens[index].SetActive(true);
        }
    }

    public void NextScreen()
    {
        currentScreenIndex = (currentScreenIndex + 1) % screens.Count;
        ShowScreen(currentScreenIndex);
    }

    public void PreviousScreen()
    {
        currentScreenIndex = (currentScreenIndex - 1 + screens.Count) % screens.Count;
        ShowScreen(currentScreenIndex);
    }

    public void OnClickClose()
    {
        // RecipeList.gameObject.SetActive(true);
        // RecipeTree.gameObject.SetActive(false);
        gameObject.SetActive(false);

        ResumeGame();
    }

    public void OnClickOpen()
    {
        // 매개변수 없이 호출된 경우의 동작
        gameObject.SetActive(true);

        PauseGame(); // 게임 일시정지
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // 게임 일시정지
        Debug.Log("게임이 일시정지되었습니다.");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // 게임 재개
        Debug.Log("게임이 재개되었습니다.");
    }
}
