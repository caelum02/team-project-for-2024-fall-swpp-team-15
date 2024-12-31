using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_Text text;
    public float time;
    public float curTime;

    int minute;
    int second;

    // RecipeHelpUI 참조
    // public GameObject recipeHelpUI;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void StartTimer()
    {
        StartCoroutine(RunTimer());
    }

    IEnumerator RunTimer()
    {
        curTime = time;
        while (curTime > 0)
        {
            // RecipeHelpUI가 활성화되어 있으면 타이머 정지
            // if (recipeHelpUI.activeSelf)
            // {
            //     yield return null;
            //     continue; // 활성화 상태일 때 타이머는 진행되지 않음
            // }

            // 타이머 감소
            curTime -= Time.deltaTime;
            minute = (int)curTime / 60;
            second = (int)curTime % 60;
            text.text = minute.ToString("00") + " : " + second.ToString("00");
            yield return null;

            // 시간이 0 이하가 되면 종료
            if (curTime <= 0)
            {
                curTime = 0;
                gameManager.CloseRestaurant();
                yield break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
