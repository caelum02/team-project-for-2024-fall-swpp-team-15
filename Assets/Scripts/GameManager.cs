using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Timer timer;
    public CustomerManager customerManager;
    public Button startGame; //게임 시작 버튼 (테스트용)
    public Button openRestaurantButton;
    public TMP_Text openOrCloseText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        OpenRestaurant();
    }

    //영업 시간 시작
    public void OpenRestaurant()
    {
        timer.StartTimer();
        openOrCloseText.text = "영업 시간";
        //영업 시작 버튼 비활성화
        openRestaurantButton.gameObject.SetActive(false);
        //손님 prefab 들어오기 시작
        customerManager.StartCustomerEnter();
    }

    //정비 시간 시작 
    public void CloseRestaurant()
    {
        openOrCloseText.text = "정비 시간";
        //영업 시작 버튼 생성
        openRestaurantButton.gameObject.SetActive(true);
        //손님 prefab 멈추기
        customerManager.StopCustomerEnter();
        //인테리어 수정 가능 모드로 전환 
    }
}
