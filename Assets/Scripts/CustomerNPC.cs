using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using Yogaewonsil.Common;

public class CustomerNPC : MonoBehaviour
{   
    public CustomerType customerType;
    public NavMeshAgent customerAgent;
    private Table assignedTable;
    private CustomerManager customerManager;

    public Vector3 spawnPosition; // 손님이 레스토랑을 떠날 위치
    private bool isSeated = false;
    private bool hasOrdered = false;
    private bool isEating = false;
    private bool isDrinking = false;
    public bool isFoodReceived = false; // 손님이 음식을 성공적으로 받았는지 여부.

    private FoodData orderedDish;
    public float patience = 90f; // 인내심 게이지
    private float patienceTimer;

    public Image patienceGauge; // 인내심 게이지 UI
    public Sprite orangeButton;
    [SerializeField] private Button orderButton;
    [SerializeField] private TMP_Text menuText;

    [Header("Audio Settings")]
    [SerializeField] protected AudioSource audioSource; // 요리 사운드를 재생할 AudioSource
    [SerializeField] private AudioClip eatSound; // 요리 시작 시 재생할 사운드
    [SerializeField] private AudioClip drinkSound; // 요리 시작 시 재생할 사운드

    [Header("Icons")]
    [SerializeField] private Texture HappyIcon;
    [SerializeField] private Texture DisappointIcon;
    [SerializeField] private Texture BadguyIcon;
    [SerializeField] private Texture DeadIcon;

    [Header("Database")]
    [SerializeField] private FoodDatabaseSO foodDatabase; // 음식 데이터베이스

    [Header("Animation")]
    [SerializeField] public Animator customerAnimator;

    void Start()
    {   
        // CustomerCore의 Animator 컴포넌트를 가져오기
        Transform customerCoreTransform = transform.Find("CustomerCore");
        if (customerCoreTransform != null)
        {
            customerAnimator = customerCoreTransform.GetComponent<Animator>();
        }

        if (customerAnimator == null)
        {
            Debug.LogError("CustomerCore의 Animator를 찾을 수 없습니다.");
        }

        // NavMeshAgent 설정 조정(충돌없이 서로 통과할 수 있도록)
        customerAgent.avoidancePriority = 0; // 가장 높은 우선순위로 경로를 확보
        customerAgent.radius = 0.1f; // 충돌 감지 반경 최소화 


        customerManager = GameObject.Find("CustomerManager").GetComponent<CustomerManager>();
        FindAndMoveToTable();
        patienceTimer = patience;

        orderButton = GetComponentInChildren<Button>();
        orderButton.gameObject.SetActive(false);
        orderButton.onClick.AddListener(OnOrderButtonClick);

        menuText = orderButton.transform.GetComponentInChildren<TMP_Text>();
        if (menuText == null)
        {
            Debug.LogError($"menuText not found in {gameObject.name}");
            return;
        }

        patienceGauge = transform.Find("Canvas/OrderButton/PatienceGauge").GetComponent<Image>();

        GetRandomDishFromCustomerManager();
    }

    void Update()
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
                StartEating(); // 테이블에 음식이 있을 경우 확인
            }
        }

        UpdateRotation(); // 이동 방향으로 회전
        CheckIfReachedTable();
    }

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

    private void UpdatePatienceGauge()
    {
        if (patienceGauge != null)
        {
            patienceGauge.fillAmount = patienceTimer / patience;
        }
    }

    private void HandleUnhappyExit()
    {
        Debug.Log("Customer left due to impatience.");
        // 주문목록에서 삭제
        customerManager.orderManager.RemoveOrder(this, orderedDish);
        DisplayIcon(DisappointIcon);
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
        // customerManager.orderManager.RemoveOrder(this, orderedDish);
        ExitRestaurant();
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

    private void CheckIfReachedTable()
    {
        if (assignedTable != null && !isSeated)
        {
            if (Vector3.Distance(transform.position, assignedTable.transform.position) < 2.0f)
            {   
                isSeated = true;
                orderButton.gameObject.SetActive(true);
                hasOrdered = true;

                Debug.Log($"Order Accepted: {orderedDish}");
                if(customerType != CustomerType.진상손님) customerManager.HandleOrder(this, orderedDish);
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

    private void OnOrderButtonClick()
    {
        // if (hasOrdered) return;
        // hasOrdered = true;

        // Debug.Log($"Order Accepted: {orderedDish}");

        // customerManager.HandleOrder(this, orderedDish);
    }

    private void StartEating()
    {   
        if (isEating || isDrinking || assignedTable.plateFood == null) return;

        if (assignedTable.plateFood == Food.차 || assignedTable.plateFood == Food.미소국)
        {   
            isDrinking = true;
            StartCoroutine(DrinkTea());
            return;
        }

        isEating = true;

        orderButton.gameObject.SetActive(false);

        // 주문목록에서 삭제
        if (customerType != CustomerType.진상손님) customerManager.orderManager.RemoveOrder(this, orderedDish);

        // 카운트다운 종료
        patienceTimer = 0;
        UpdatePatienceGauge();

        Debug.Log("StartEating");
        StartCoroutine(EatFood());
    }

    private IEnumerator DrinkTea()
    {
        // 마시는 사운드 재생
        if (audioSource != null && drinkSound != null)
        {
            audioSource.clip = drinkSound;
            audioSource.loop = true; // 필요 시 루프 설정
            audioSource.Play(); // 사운드 재생
        }

        if (assignedTable.plateFood == Food.차)
        {
            patienceTimer += 30.0f; 
        }
        else
        {
            patienceTimer += 60.0f;
        }
        UpdatePatienceGauge();

        yield return new WaitForSeconds(3f); // 음식을 먹는 시간

        audioSource.Stop(); // 오디오 종료;

        assignedTable.plateFood = null; // 테이블 비우기
        Destroy(assignedTable.currentPlateObject); // 프리팹 삭제

        isDrinking = false;
    }

    private IEnumerator EatFood()
    {   
        // 먹는 애니메이션 재생
        if (customerAnimator != null)
        {
            customerAnimator.SetBool("isEating", true);
        }

        // 먹는 사운드 재생
        if (audioSource != null && eatSound != null)
        {
            audioSource.clip = eatSound;
            audioSource.loop = true; // 필요 시 루프 설정
            audioSource.Play(); // 사운드 재생
        }
        
        yield return new WaitForSeconds(10f); // 음식을 먹는 시간

        IsCorrectFood();

        audioSource.Stop(); // 오디오 종료;
        
        assignedTable.plateFood = null; // 테이블 비우기
        Destroy(assignedTable.currentPlateObject); // 프리팹 삭제

        ExitRestaurant();
    }

    public void ExitRestaurant()
    {   
        EvaluateFood();

        if (assignedTable != null)
        {
            assignedTable.Vacate(); // 테이블 상태 비우기
        }
        
        if (customerAnimator != null)
        {   
            customerAnimator.SetBool("isEating", false); // 먹는 애니메이션 적용중이라면 정지
            customerAnimator.SetBool("isWalking", true);
        }

        customerAgent.SetDestination(spawnPosition);
        StartCoroutine(CheckIfReachedExit());
    }

    private void IsCorrectFood()
    {
        // 음식이 맞게 왔는지 판단하는 함수
        FoodData servedFood = FindFoodDataByType((Food)assignedTable.plateFood);

        // 진상 손님의 경우 로직이 다름
        if (customerType == CustomerType.진상손님)
        {
            if (servedFood.food == Food.실패요리)
            {
                isFoodReceived = true;
                Debug.Log("해치웠다!");
            }
            customerManager.UpdateGameStats(0, -10);
            return;
        }

        // 일반적인 손님
        if (servedFood == orderedDish)
        {
            Debug.Log("Correct dish received.");
            // customerManager.UpdateGameStats(servedFood.price, 10); // 평판 증가
            isFoodReceived = true;
        }
        else
        {
            Debug.Log("Wrong dish received.");
            //customerManager.UpdateGameStats(0, -5); // 평판 감소
        }
    }

    private void EvaluateFood()
    {   
        orderButton.gameObject.SetActive(true);

        if (customerType == CustomerType.진상손님)
        {   
            if (isFoodReceived) DisplayIcon(DeadIcon);
            return;
        }
        if (isFoodReceived)
        {
            DisplayIcon(HappyIcon);
            orderButton.interactable = false;
        }
        else
        {
            DisplayIcon(DisappointIcon);
            orderButton.interactable = false;
        }
    }

    private void PayMoneyAndReputation()
    {
        if (isFoodReceived)
        {   
            int price = customerType == CustomerType.진상손님 ? 10000 : orderedDish.price;
            int points = customerType == CustomerType.진상손님 ? 20 : orderedDish.level * 25; 

            if (customerType == CustomerType.음식평론가) 
            {   
                price = 2 * price;
                points = 2 * points;
            }
            else if (customerType == CustomerType.미슐랭가이드)
            {   
                Debug.Log("미슐랭 별을 받았습니다!");
                customerManager.GetMichelinStar();
            }
            Debug.Log($"You get {points}points");

            customerManager.UpdateGameStats(price, points);
        }
        else if (customerType == CustomerType.진상손님)
        {
            customerManager.UpdateGameStats(0, -10); // 평판 하락
        }
    }

    private IEnumerator CheckIfReachedExit()
    {
        while (Vector3.Distance(transform.position, spawnPosition) > 1.5f)
        {
            yield return null;
        }

        PayMoneyAndReputation();

        customerManager.RemoveCustomer(this); // CustomerManager에서 삭제
        Destroy(gameObject); // 오브젝트 삭제
    }

    private void GetRandomDishFromCustomerManager()
    {   
        if (customerType == CustomerType.진상손님)
        {
            DisplayIcon(BadguyIcon);
            orderButton.interactable = false;
            return;
        }
        orderedDish = customerManager.GetRandomDish();
        UpdateMenuText(orderedDish.food);
        DisplayIcon(orderedDish.icon);
    }

    private void UpdateMenuText(Food food)
    {
        menuText.text = food.ToString();
    }

    private void DisplayIcon(Texture Icon)
    {   
        RawImage buttonRawImage = orderButton.transform.Find("Image").GetComponent<RawImage>();
        if (buttonRawImage != null)
        {
            buttonRawImage.texture = Icon;
            buttonRawImage.color = Color.white;
        }
    }

    /// <summary>
    /// 특정 Food 타입에 해당하는 FoodData를 검색합니다.
    /// </summary>
    private FoodData FindFoodDataByType(Food foodType)
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
