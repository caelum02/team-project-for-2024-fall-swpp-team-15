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