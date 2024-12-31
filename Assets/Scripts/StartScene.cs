using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    /// <summary>
    /// 게임 시작 버튼 
    /// </summary>
    public Button gameStartButton;

    /// <summary>
    /// 게임 설명 버튼 
    /// </summary>
    public Button gameExplainButton;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 게임 시작 버튼 클릭 시 GameScene 로드
    /// </summary>
    public void OnClickGameStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// 게임 설명 버튼 클릭 시 게임 튜토리얼 화면 생성
    /// </summary>
    public void OnClickGameExplain()
    {

    }
}
