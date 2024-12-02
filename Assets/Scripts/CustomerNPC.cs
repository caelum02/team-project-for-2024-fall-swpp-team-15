using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 각 손님 개인의 이동, 착석, 주문, 퇴장과 같은 동작 관리
/// </summary>
public class CustomerNPC : MonoBehaviour
{
    /// <summary>
    /// 손님의 이동을 처리하는 NavMeshAgent
    /// </summary>
    public NavMeshAgent customerAgent;

    /// <summary>
    /// 손님에게 할당된 테이블
    /// </summary>
    private Table assignedTable;

    /// <summary>
    /// 모든 손님을 관리하는 CustomerManager
    /// </summary>
    private CustomerManager customerManager;

    /// <summary>
    /// 손님이 레스토랑 입장하는 위치
    /// </summary>
    public Vector3 spawnPosition;

    /// <summary>
    /// 손님이 테이블에 앉아있는지 여부
    /// </summary>
    private bool isSeated = false;

    /// <summary>
    /// 주문 버튼
    /// </summary>
    private Button orderButton;

    /// <summary>
    /// 손님이 주문한 요리 데이터.
    /// FoodDatabase에서 데이터 받아옴
    /// </summary>
    private FoodData orderedDish;

    /// <summary>
    /// 주문 버튼 수락 후 버튼 색 변환 (노랑 -> 주황)
    /// </summary>
    public Sprite orangeButton;

    /// <summary>
    /// 손님이 음식을 성공적으로 받았는지 여부.
    /// 우선 true로 설정. 이후 플레이어가 음식을 성공적으로 가져다주는 로직과 연결할 예정
    /// </summary>
    public bool isFoodReceived = true;
    
    // Start is called before the first frame update
    void Start()
    {
        customerManager = GameObject.Find("CustomerManager").GetComponent<CustomerManager>();
        FindAndMoveToTable();
        spawnPosition = new Vector3(-10, 0, 5); // 나가는 문 위치 
        orderButton = GetComponentInChildren<Button>();
        orderButton.gameObject.SetActive(false);
        orderButton.onClick.AddListener(OnOrderButtonClick);
        GetRandomDishFromCustomerManager();
    }

    /// <summary>
    /// Table에 도착했는지 계속 확인 
    /// </summary>
    void Update()
    {
        CheckIfReachedTable();
    }

    /// <summary>
    /// 착석 가능한 Table을 찾아 해당 Table의 위치로 이동
    /// </summary>
    private void FindAndMoveToTable()
    {
        assignedTable = customerManager.GetAvailableTable();

        if (assignedTable != null)
        {
            customerAgent.SetDestination(assignedTable.transform.position);
        }
        else
        {
            Debug.Log("No available tables for this customer.");
        }
    }

    /// <summary>
    /// Table에 도착하면, 주문 버튼 생성 
    /// </summary>
    private void CheckIfReachedTable()
    {
        if (assignedTable != null && !isSeated)
        {
            if (Vector3.Distance(transform.position, assignedTable.transform.position) < 2.0f)
            {
                isSeated = true;
                orderButton.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 주문 버튼 클릭하여 주문 수락.
    /// 주문 버튼 색 노랑 -> 주황으로 변환
    /// </summary>
    private void OnOrderButtonClick()
    {
        Debug.Log($"Order Accepted: {orderedDish}");
        Image buttonImage = orderButton.GetComponent<Image>();
        buttonImage.sprite = orangeButton;
        //orderButton.gameObject.SetActive(false);

        customerManager.HandleOrder(this, orderedDish);
    }

    /// <summary>
    /// 레스토랑 나가기 
    /// </summary>
    public void ExitRestaurant()
    {
        if (assignedTable != null)
        {
            assignedTable.Vacate();
        }

        if (isFoodReceived)
        {
            int points = orderedDish.price / 100;
            customerManager.UpdateGameStats(orderedDish.price, points);
        }

        customerAgent.SetDestination(spawnPosition);
        StartCoroutine(CheckIfReachedExit());
    }

    /// <summary>
    /// 퇴장 지점 도착 여부 확인 후 오브젝트 삭제
    /// </summary>
    private IEnumerator CheckIfReachedExit()
    {
        while (Vector3.Distance(transform.position, spawnPosition) > 1.5f) //정확한 위치는 추후 조정 
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// CustomerManager에서 랜덤한 요리 데이터 받아오기
    /// </summary>
    private void GetRandomDishFromCustomerManager()
    {
        orderedDish = customerManager.GetRandomDish();
        DisplayDishIcon(orderedDish.icon); 
    }

    /// <summary>
    /// 주문 버튼에 요리 아이콘 표시
    /// </summary>
    /// <param name="dishIcon">요리 아이콘 Texture</param>
    private void DisplayDishIcon(Texture dishIcon)
    {
        RawImage buttonRawImage = orderButton.transform.Find("Image").GetComponent<RawImage>();

        if (buttonRawImage != null)
        {
            if (dishIcon != null)
            {
                buttonRawImage.texture = dishIcon;
                buttonRawImage.color = Color.white;
            }
            else
            {
                buttonRawImage.texture = null;
                buttonRawImage.color = new Color(0, 0, 0, 0);
            }
        }
        else
        {
            Debug.LogWarning("Order button does not have a RawImage component to display an icon.");
        }
    }

}