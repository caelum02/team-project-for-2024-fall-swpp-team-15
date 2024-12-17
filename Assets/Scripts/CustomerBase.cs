using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yogaewonsil.Common;

/// <summary>
/// CustomerNPC의 기본적인 동작을 다룸
/// (주문 제외) 입장하고, 기다리고, 퇴장하는 공통동작을 다룸
/// </summary>
public abstract class CustomerBase : MonoBehaviour
{   
    [Header("Basic Properties")]
    public CustomerType customerType;
    private Table assignedTable;
    protected float patience;
    public FoodData requiredDish;

    [Header("Customer States")]
    private float patienceTimer;
    private bool isSeated = false;
    private bool hasOrdered = false;
    private bool isEating = false;
    private bool isDrinking = false;
    protected bool isSuccessTreatment = false;

    [Header("Navmesh Settings")]
    public UnityEngine.AI.NavMeshAgent customerAgent;
    public Vector3 spawnPosition = new Vector3(1,0.6f,-7.5f); // 손님이 레스토랑을 떠날 위치

    [Header("UI References")]
    public Image patienceGauge;
    [SerializeField] protected Button orderButton;
    [SerializeField] protected TMP_Text menuText;

    [Header("Audio Settings")]
    [SerializeField] protected AudioSource audioSource; // 요리 사운드를 재생할 AudioSource
    [SerializeField] private AudioClip eatSound; // 요리 시작 시 재생할 사운드
    [SerializeField] private AudioClip drinkSound; // 요리 시작 시 재생할 사운드

    [Header("Icons")]
    [SerializeField] protected Texture SuccessIcon; // 맞이를 성공했을 때의 아이콘
    [SerializeField] protected Texture FailIcon; // 실패했을 때의 아이콘

    [Header("Database")]
    [SerializeField] private FoodDatabaseSO foodDatabase; // 음식 데이터베이스

    [Header("Animation")]
    [SerializeField] private Animator customerAnimator; // 손님 NPC 애니메이터

    [Header("CustomerManager")]
    protected CustomerManager customerManager; // CustomerManager

    /// <summary>
    /// Customer NPC 초기화
    /// </summary>
    private void Start()
    {   
        customerManager = GameObject.Find("CustomerManager").GetComponent<CustomerManager>(); // customerManger 찾기

        SetupNavmesh(); // Navmesh 세팅

        FindAndMoveToTable(); // Table 지정 후 이동

        SetupUI(); // UI 초기화

        SetPatience(); // 초기 인내심 세팅

        patienceTimer = patience; // 초기 인내심 적용
    }

    protected virtual void SetPatience()
    {
        patience = 90f;
    }

    /// <summary>
    /// 상태를 업데이트 하면서 이동 -> 대기 -> 식사
    /// </summary>
    private void Update()
    {
        if (hasOrdered && !isEating && !isDrinking)
        {   
            // Debug.Log($"{this.name} allocated to {assignedTable.name}");
            patienceTimer -= Time.deltaTime;
            UpdatePatienceGauge();

            if (patienceTimer <= 0)
            {
                HandleUnhappyExit(); // 인내심이 0이 되었을 때 퇴장
            }
            else if (assignedTable.plateFood != null)
            {   
                EatOrDrink(); // 테이블에 음식이 있을 경우 확인
            }
        }

        UpdateRotation(); // 이동 방향으로 회전
        CheckIfReachedTable();
    }

    private void UpdatePatienceGauge()
    {
        if (patienceGauge != null)
        {
            patienceGauge.fillAmount = patienceTimer / patience;
        }
    }

    /// <summary>
    /// 음식을 주문하는 함수 (자식 클래스에서 override 해서 사용)
    /// </summary>
    protected virtual FoodData MakeOrder()
    {   
        return null;
    }

    /// <summary>
    /// 주문한 음식을 주문목록에서 삭제하는 함수 (자식 클래스에서 override 해서 사용)
    /// </summary>
    protected virtual void DeleteOrder()
    {

    }

    public virtual void SetupUI()
    {
        patienceGauge = transform.Find("Canvas/OrderButton/PatienceGauge").GetComponent<Image>();
        menuText = transform.Find("Canvas/OrderButton/MenuText").GetComponent<TMP_Text>();
        orderButton = GetComponentInChildren<Button>();
        orderButton.gameObject.SetActive(false);
    }

    private void CheckIfReachedTable()
    {
        if (assignedTable != null && !isSeated)
        {
            if (Vector3.Distance(transform.position, assignedTable.transform.position) < 2.0f)
            {   
                isSeated = true;
                orderButton.gameObject.SetActive(true);
                hasOrdered = true;

                requiredDish = MakeOrder(); // 주문
            }
        }
    }

    private IEnumerator CheckIfReachedExit()
    {
        while (Vector3.Distance(transform.position, spawnPosition) > 1.0f)
        {
            yield return null;
        }

        PayMoneyAndReputation();

        customerManager.RemoveCustomer(this); // CustomerManager에서 삭제
        Destroy(gameObject); // 오브젝트 삭제
    }

    private void HandleUnhappyExit()
    {
        Debug.Log("Customer left due to impatience.");
        DeleteOrder();
        orderButton.interactable = false;
        ExitRestaurant();
    }

    public void HandleRestaurantCloseExit()
    {   
        if (isDrinking || isEating)
        {
            audioSource.Stop();
            assignedTable.plateFood = null; // 테이블 비우기
            Destroy(assignedTable.currentPlateObject); // 프리팹 삭제
        }
        orderButton.gameObject.SetActive(false);
        DeleteOrder();
        ExitRestaurant();
    }

    private void EatOrDrink()
    {   
        if (isEating || isDrinking || assignedTable.plateFood == null) return;

        if (assignedTable.plateFood == Food.차 || assignedTable.plateFood == Food.미소국)
        {   
            StartCoroutine(DrinkTea());
        }
        else
        {
            StartCoroutine(EatFood());
        }
    }

    private IEnumerator DrinkTea()
    {   
        isDrinking = true;

        // 마시는 사운드 재생
        if (audioSource != null && drinkSound != null)
        {
            audioSource.clip = drinkSound;
            audioSource.loop = true; // 루프 설정
            audioSource.Play(); // 사운드 재생
        }
        
        // 인내심 타이머 증가
        if (assignedTable.plateFood == Food.차)
        {
            patienceTimer += 30.0f; 
        }
        else
        {
            patienceTimer += 60.0f;
        }
        UpdatePatienceGauge();

        // 차를 마시는 시간
        yield return new WaitForSeconds(3f); // 음식을 먹는 시간

        audioSource.Stop(); // 오디오 종료;

        assignedTable.plateFood = null; // 테이블 비우기
        Destroy(assignedTable.currentPlateObject); // 프리팹 삭제

        isDrinking = false;
    }

    private IEnumerator EatFood()
    {   
        isEating = true;

        orderButton.gameObject.SetActive(false);

        DeleteOrder(); // 주문을 삭제;

        // 카운트다운 종료를 UI로 나타냄
        patienceTimer = 0;
        UpdatePatienceGauge();

        // 먹는 애니메이션 재생
        if (customerAnimator != null)
        {
            customerAnimator.SetBool("isEating", true);
        }

        // 먹는 사운드 재생
        if (audioSource != null && eatSound != null)
        {
            audioSource.clip = eatSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        yield return new WaitForSeconds(5f); // 음식을 먹는 시간

        StopEating();

        ExitRestaurant();
    }

    private void StopEating()
    {
        CheckSuccess();

        audioSource.Stop(); // 오디오 종료;
        customerAnimator.SetBool("isEating", false); // 애니메이션 종료
        
        assignedTable.plateFood = null; // 테이블 비우기
        Destroy(assignedTable.currentPlateObject); // 프리팹 삭제
    }

    public void ExitRestaurant()
    {   
        ShowSatisfaction();
        LeaveTable();
    }

    private void LeaveTable()
    {   
        // 먹는 애니메이션 종료 후 걷는 애니메이션 재생
        if (customerAnimator != null)
        {   
            customerAnimator.SetBool("isEating", false);
            customerAnimator.SetBool("isWalking", true);
        }

        // 테이블 비우기
        if (assignedTable != null)
        {
            assignedTable.Vacate();
        }

        customerAgent.SetDestination(spawnPosition);
        StartCoroutine(CheckIfReachedExit());
    }

    private void ShowSatisfaction()
    {   
        orderButton.gameObject.SetActive(true); // orderButton 보이게 하기

        if (isSuccessTreatment)
        {   
            DisplayIcon(SuccessIcon);   // 적당한 대우였다면 SuccessIcon
        }
        else
        {   
            DisplayIcon(FailIcon);  // 적당하지 않은 대우였다면 FailIcon
        }
        orderButton.interactable = false;
    }

    private void CheckSuccess()
    {
        FoodData servedFood = FindFoodDataByType((Food)assignedTable.plateFood); // 손님에게 전달된 음식

        if (servedFood == requiredDish)
        {
            isSuccessTreatment = true;
        }
    }

    /// <summary>
    /// 돈과 평판 지불 방법: NPC 별로 상이함
    /// </summary>
    protected virtual void PayMoneyAndReputation()
    {
        if (isSuccessTreatment)
        {   
            Debug.Log("만족스러운 식사였어요!");
        }
        else 
        {
            Debug.Log("요리 실력이 형편없군요!");
        }
    }

    ///  -------------------- 보조 함수들  ------------------------- 

    /// <summary>
    /// NavMeshAgent 설정 조정(충돌없이 서로 통과할 수 있도록)
    /// </summary>
    public void SetupNavmesh()
    {
        customerAgent.avoidancePriority = 0; // 가장 높은 우선순위로 경로를 확보
        customerAgent.radius = 0.1f; // 충돌 감지 반경 최소화 
    }

    private void FindAndMoveToTable()
    {   
        if (customerAnimator != null)
        {
            customerAnimator.SetBool("isWalking", true);
        }

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
    /// NPC가 이동방향을 바라보도록 조정하는 함수
    /// </summary>
    private void UpdateRotation()
    {
        // 현재 NavMeshAgent의 속도 벡터를 가져옵니다.
        Vector3 velocity = customerAgent.velocity;

        // 속도가 거의 없으면(정지 상태) 회전을 멈춥니다.
        if (velocity.magnitude > 0.1f)
        {
            // 속도의 방향을 바라보도록 회전합니다.
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f); // 부드러운 회전
        }
        if (customerAgent.remainingDistance <= customerAgent.stoppingDistance && !customerAgent.pathPending)
        {
            FaceTable();
            if (customerAnimator != null)
            {
                customerAnimator.SetBool("isWalking", false);
            }
        }
    }

    private void FaceTable()
    {
        if (assignedTable == null) return;

        // 테이블의 방향 계산
        Vector3 directionToTable = (assignedTable.transform.position - transform.position).normalized;

        // y축은 고정하고, x와 z만 고려
        directionToTable.y = 0;

        // 방향을 기반으로 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(directionToTable);

        // 즉시 회전 또는 부드럽게 회전 (필요에 따라 선택)
        transform.rotation = targetRotation; // 즉시 회전
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f); // 부드럽게 회전
    }

    /// <summary>
    /// 손님NPC 머리 위에 아이콘을 띄우는 함수
    /// </summary>
    /// <param name="Icon">띄울 아이콘</param>
    protected virtual void DisplayIcon(Texture Icon)
    {   
        RawImage buttonRawImage = orderButton.transform.Find("Image").GetComponent<RawImage>();
        if (buttonRawImage != null)
        {
            buttonRawImage.texture = Icon;
            buttonRawImage.color = Color.white;
        }
    }

    /// <summary>
    /// 특정 Food 타입에 해당하는 FoodData를 검색하는 함수
    /// </summary>
    protected FoodData FindFoodDataByType(Food foodType)
    {
        foreach (FoodData foodData in foodDatabase.foodData)
        {   
            if (foodData.food == foodType)
            {
                return foodData;
            }
        }
        Debug.LogWarning($"Food type {foodType} not found in database.");
        return null;
    }
}
