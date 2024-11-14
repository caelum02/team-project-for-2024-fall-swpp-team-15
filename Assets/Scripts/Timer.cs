using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private TMP_Text text;

    [SerializeField] private float time;
    [SerializeField] private float curTime;

    int minute;
    int second;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void StartTimer()
    {
        //5분은 time = 300으로 설정. 테스트 위해 10으로 설정 
        time = 10;
        StartCoroutine(RunTimer());
    }

    IEnumerator RunTimer()
    {
        curTime = time;
        while(curTime > 0)
        {
            curTime -= Time.deltaTime;
            minute = (int)curTime / 60;
            second = (int)curTime % 60;
            text.text = minute.ToString("00") + " : " + second.ToString("00");
            yield return null;

            if(curTime <= 0)
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