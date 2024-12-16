using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// 이 클래스는 게임의 전반적인 상태와 흐름을 관리합니다.
/// 주요 기능:
/// - 정비 시간과 영업 시간 관리
/// - 돈과 평판 관리
/// - 손님, 플레이어, UI 관련 초기화 및 업데이트
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 게임 시간 관리 타이머
    /// </summary>
    public Timer timer;

    /// <summary>
    /// 손님 관리
    /// </summary>
    public CustomerManager customerManager;

    /// <summary>
    /// 인테리어 UI 관리
    /// </summary>
    public InteriorUI interiorUI;

    /// <summary>
    /// 조리도구 배치 관리
    /// </summary>
    public PlacementSystem placementSystem;

    /// <summary>
    /// 게임 시작 버튼 (테스트용)
    /// </summary>
    public Button startGame;

    /// <summary>
    /// 영업 시작 버튼
    /// </summary>
    public Button openRestaurantButton;

    /// <summary>
    /// 영업 상태 표시 텍스트
    /// </summary>
    public TMP_Text openOrCloseText;

    /// <summary>
    /// 돈 
    /// </summary>
    public int money;

    /// <summary>
    /// 현재 평판 레벨 (1~8)
    /// </summary>
    public int reputation;

    /// <summary>
    /// 현재 평판 포인트 (0~100) 각 레벨에서 0 ~ 100 값에 따라 게이지바 이동 (100 도달하면 다음 레벨로 평판 상승)
    /// </summary>
    public int reputationValue;
    public GameObject playerPrefab;
    [SerializeField] private Vector3 playerSpawnPoint = new Vector3(0,1,-20);
    private GameObject player;
    public RecipeUI recipeUI;
    public Button gameStartButton;

    /// <summary>
    /// 주문 관리
    /// </summary>
    public OrderManager orderManager;

    /// <summary>
    /// 전반적인 UI 업데이트 관리
    /// </summary>
    public UIManager uiManager;

    /// <summary>
    /// 초기 값 설정
    /// </summary>
    void Start()
    {
        money = 10000;
        reputation = 1;
        reputationValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 정비 시간 시작
    /// - 영업 준비를 위한 로직 수행
    /// </summary>
    public void StartGame()
    {  
        // 게임 시작 버튼 사라짐
        gameStartButton.gameObject.SetActive(false);
        // 정비 시간 코드 중 일부
        openOrCloseText.text = "정비 시간";
        //영업 시작 버튼 생성
        openRestaurantButton.gameObject.SetActive(true);
        //인테리어 버튼 활성화
        interiorUI.MakeInteriorButtonVisible();
        placementSystem.ResetGame();
        placementSystem.LoadGame();
    }

    /// <summary>
    /// 영업 시간 시작
    /// </summary>
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

    /// <summary>
    /// 정비 시간 시작 
    /// </summary>
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

    /// <summary>
    /// 돈 추가
    /// </summary>
    /// <param name="amount">추가할 금액</param>
    public void UpdateMoney(int amount, bool isPlus)
    {
        if (isPlus)
        {
            money += amount;
        }
        else
        {
            money -= amount;
        }
        Debug.Log($"Money updated: {money}");
        uiManager.updateMoneyUI();
    }

    /// <summary>
    /// 평판 포인트 추가 및 레벨 업 처리
    /// </summary>
    /// <param name="points">추가할 평판 포인트</param>
    public void AddReputation(int points)
    {
        reputationValue += points;
        Debug.Log($"Reputation value increased: {reputationValue}");

        while (reputationValue >= 100)
        {
            reputationValue -= 100;
            LevelUp();
        }
        uiManager.updateReputationUI();
    }

    public void LevelUp()
    {
        reputation += 1;
        Debug.Log($"Reputation level increased! New level: {reputation}");
        recipeUI.UpdateAllPriceAndLevel();
        uiManager.ShowLevelUpScreen();

    }
}