using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;
using Yogaewonsil.Common;

/// <summary>
/// 손님 로직을 관리하는 클래스.
/// 손님 생성, 착석, 퇴장 관리. 
/// </summary>
public class CustomerManager : MonoBehaviour
{
    public NavMeshSurface floor; // 손님 경로 탐색을 위한 NavMeshSurface
    public List<Table> tables = new List<Table>(); // 레스토랑의 테이블 리스트
    public List<CustomerNPC> customers = new List<CustomerNPC>(); // 현재 레스토랑에 있는 손님 NPC 리스트
    public GameObject customerPrefab; // 손님 GameObject 프리팹
    public OrderManager orderManager; // 손님 '주문 관리'를 위한 OrderManager
    public GameManager gameManager; // 게임 상태(돈, 평판) 관리를 위한 GameManager
    public ICustomerWave customerWave; // 손님 생성 Wave 인터페이스

    private Coroutine customerSpawnCoroutine; // 손님 생성 Coroutine
    private int customerCounter = 1; // 고유한 손님 이름 생성을 위한 카운터

    void Start()
    {
        FindTable();
        customerWave = new UniformCustomerWave(4f, 4f); // 초기 손님 생성 Wave 설정
    }

    /// <summary>
    /// 손님 입장 시작
    /// </summary>
    public void StartCustomerEnter()
    {
        Debug.Log("영업 시작");
        FindTable();
        customerSpawnCoroutine = StartCoroutine(customerWave.SpawnCustomers(this));
    }

    /// <summary>
    /// 손님 퇴장 및 레스토랑 초기화
    /// </summary>
    public void StartCustomerExit()
    {   
        Debug.Log("Start Customer Exit!");
        if (customerSpawnCoroutine != null)
        {
            StopCoroutine(customerSpawnCoroutine);
            customerSpawnCoroutine = null;
        }

        ClearAllCustomers();
    }

    /// <summary>
    /// 영업 종료 시 모든 손님 제거
    /// </summary>
    private void ClearAllCustomers()
    {
        foreach (CustomerNPC customer in customers)
        {
            Destroy(customer.gameObject);
        }
        customers.Clear();
        tables.Clear();
    }

    /// <summary>
    /// 특정 손님 퇴장 처리
    /// </summary>
    /// <param name="customer">퇴장할 손님</param>
    public void RemoveCustomer(CustomerNPC customer)
    {
        if (customers.Contains(customer))
        {
            customers.Remove(customer);
        }
    }

    /// <summary>
    /// 영업 시간 시작 직후 모든 Table 찾기
    /// </summary>
    public void FindTable()
    {
        floor.BuildNavMesh();
        GameObject[] tableObjects = GameObject.FindGameObjectsWithTag("Table");
        foreach (GameObject tableObj in tableObjects)
        {
            Table table = tableObj.GetComponent<Table>();
            if (table != null)
            {
                tables.Add(table);
            }
        }
        ShuffleTables();
    }

    /// <summary>
    /// 테이블 리스트 랜덤으로 섞기 
    /// </summary>
    public void ShuffleTables()
    {
        System.Random random = new System.Random();
        for (int i = tables.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            Table temp = tables[i];
            tables[i] = tables[j];
            tables[j] = temp;
        }
    }

    /// <summary>
    /// 사용(착석) 가능한 테이블 반환 
    /// </summary>
    /// <returns>사용 가능한 <see cref="Table"/> 또는 없을 경우 null.</returns>
    public Table GetAvailableTable()
    {
        foreach (Table table in tables)
        {
            if (!table.isOccupied)
            {
                table.Occupy();
                return table;
            }
        }
        return null;
    }

    /// <summary>
    /// 손님 한 명 생성 
    /// </summary>
    public void SpawnCustomer()
    {   
        // 손님의 수가 테이블의 수보다 많으면 생성하지 않음
        if (customers.Count >= tables.Count)
        {
            Debug.LogWarning("Cannot spawn more customers. All tables are occupied.");
            return;
        }
        
        // 손님 오브젝트 생성
        GameObject customerObject = Instantiate(customerPrefab);
        CustomerNPC customer = customerObject.GetComponent<CustomerNPC>();
        
        // 고유 이름 생성
        string customerName = $"Customer_{customerCounter++}";
        customer.name = customerName;

        customers.Add(customer);

        Debug.Log($"Spawned customer with name: {customerName}");
    }

    /// <summary>
    /// 손님 주문 처리
    /// </summary>
    /// <param name="customer">주문하는 손님</param>
    /// <param name="dishName">주문한 음식 데이터</param>
    public void HandleOrder(CustomerNPC customer, FoodData dish)
    {
        Debug.Log($"Processing Order for {customer.name}: {dish}");

        Order newOrder = new Order(customer.name, dish);
        orderManager.SaveOrder(newOrder);

        // 음식 조리 시작하기 
    }

    /// <summary>
    /// 가능한 음식 중 랜덤으로 음식 데이터 반환
    /// </summary>
    /// <returns>주문 가능한 <see cref="FoodData"/></returns>
    public FoodData GetRandomDish()
    {
        return orderManager.GetRandomEligibleFood();
    }

    /// <summary>
    /// 게임 통계 (돈, 평판) 업데이트
    /// </summary>
    /// <param name="moneyToAdd">추가할 돈</param>
    /// <param name="reputationToAdd">추가할 평판</param>
    public void UpdateGameStats(int moneyToAdd, int reputationToAdd)
    {
        gameManager.UpdateMoney(moneyToAdd,true);
        gameManager.AddReputation(reputationToAdd);
    }

    /// <summary>
    /// 특정 Food 타입에 해당하는 FoodData를 검색합니다.
    /// </summary>
    public FoodData FindFoodDataByType(Food foodType)
    {
        return orderManager.foodDatabase.foodData.Find(foodData => foodData.food == foodType);
    }
}

/// <summary>
/// 손님이 들어오는 로직을 관리하는 인터페이스
/// </summary>
// SpawnCustomer: 손님을 생성하는 Coroutine
public interface ICustomerWave {
    /// <summary>
    /// 손님을 생성하는 코루틴
    /// </summary>
    /// <param name="customerManager"><see cref="CustomerManager"/> 인스턴스.</param>
    /// <returns>코루틴 Eumerator </returns>
    public abstract IEnumerator SpawnCustomers(CustomerManager CustomerManager);
}

/// <summary>
/// 일정한 시간 간격으로 손님을 생성하는 클래스
/// </summary>
public class UniformCustomerWave: ICustomerWave {

    private float startDelay;
    private float spawnInterval;
    /// <summary>
    /// <see cref="UniformCustomerWave"/>의 새 인스턴스를 초기화
    /// </summary>
    /// <param name="startDelay">손님 생성 시작 전 대기 시간</param>
    /// <param name="spawnInterval">손님 생성 간격</param>
    public UniformCustomerWave(float startDelay, float spawnInterval) {
        this.startDelay = startDelay;
        this.spawnInterval = spawnInterval;
    }

    /// <summary>
    /// 손님 생성 코루틴.
    /// 
    /// 이 코루틴은 일정한 시간 간격(`spawnInterval`)으로 손님 생성. 
    /// 첫 생성 전에는 `startDelay`만큼 대기.
    /// </summary>
    /// <param name="CustomerManager">손님을 생성할 <see cref="CustomerManager"/> 인스턴스</param>
    /// <returns>손님 생성 과정에서 사용되는 <see cref="IEnumerator"/></returns>
    public IEnumerator SpawnCustomers(CustomerManager CustomerManager)
    {
        yield return new WaitForSeconds(startDelay);

        while (true) {
            CustomerManager.SpawnCustomer();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
} 