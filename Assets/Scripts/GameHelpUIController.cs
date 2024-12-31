using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHelpUIController : MonoBehaviour
{
    public List<GameObject> screens = new List<GameObject>();
    private int currentScreenIndex = 0;

    [Header("GameManager")]
    [SerializeField] GameManager gameManager;

    void Start()
    {
        // Ensure only the first screen is active at the start
        ShowScreen(currentScreenIndex);
    }

    void Update()
    {
        if (gameManager != null) gameManager.PauseGame();
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

        if (gameManager != null) gameManager.ResumeGame(); // 게임 재생
    }

    public void OnClickOpen()
    {
        // 매개변수 없이 호출된 경우의 동작
        gameObject.SetActive(true);

        if (gameManager != null) gameManager.PauseGame(); // 게임 일시정지
    }
}
