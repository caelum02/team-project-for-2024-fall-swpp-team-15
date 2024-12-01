using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public Timer timer;
    public CustomerManager customerManager;
    public InteriorUI interiorUI;
    public PlacementSystem placementSystem;
    public Button startGame; //게임 시작 버튼 (테스트용)
    public Button openRestaurantButton;
    public TMP_Text openOrCloseText;
    public int money;
    public int reputation; // 레벨 1 ~ 레벨 8
    public int reputationValue; // 각 레벨에서 0 ~ 100 값에 따라 게이지바 이동 (100 도달하면 다음 레벨로 평판 상승)
    public GameObject playerPrefab;
    [SerializeField] private Vector3 playerSpawnPoint = new Vector3(0,1,0);
    private GameObject player;
    public OrderManager orderManager;
    public UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        money = 0;
        reputation = 1;
        reputationValue = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {   
        // 정비 시간 코드 중 일부
        openOrCloseText.text = "정비 시간";
        //영업 시작 버튼 생성
        openRestaurantButton.gameObject.SetActive(true);
        //인테리어 버튼 활성화
        interiorUI.MakeInteriorButtonVisible();
    }

    //영업 시간 시작
    public void OpenRestaurant()
    {
        timer.StartTimer();
        openOrCloseText.text = "영업 시간";
        //영업 시작 버튼 비활성화
        openRestaurantButton.gameObject.SetActive(false);
        //인테리어 버튼 비활성화 
        interiorUI.MakeInteriorButtonInvisible();
        //조리도구 배치 비활성화 
        placementSystem.StopPlacement();
        //손님 prefab 들어오기 시작
        customerManager.StartCustomerEnter();

        //플레이어 prefab 생성하기
        player = Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity);
    }

    //정비 시간 시작 
    public void CloseRestaurant()
    {
        openOrCloseText.text = "정비 시간";
        //영업 시작 버튼 생성
        openRestaurantButton.gameObject.SetActive(true);
        //인테리어 버튼 활성화
        interiorUI.MakeInteriorButtonVisible();
        //손님 prefab 멈추기
        customerManager.StartCustomerExit();
        Destroy(player);
        orderManager.ClearOrder();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"Money updated: {money}");
        uiManager.updateMoneyUI();
    }

    public void AddReputation(int points)
    {
        reputationValue += points;

        // Check if reputation reaches 100
        while (reputationValue >= 100)
        {
            reputationValue -= 100; // Deduct 100 reputation points
            reputation += 1; // Increment reputation level
            Debug.Log($"Reputation level increased! New level: {reputation}");
        }

        Debug.Log($"Reputation updated: {reputationValue}");
        uiManager.updateReputationUI();
    }
}
