using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    public Button gameStartButton;
    public Button gameExplainButton;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("StartScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickGameStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickGameExplain()
    {

    }
}
