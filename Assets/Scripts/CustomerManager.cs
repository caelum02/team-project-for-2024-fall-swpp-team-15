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
    /// <summary>
    /// 손님 경로 탐색을 위한 NavMeshSurface
    /// </summary>
    public NavMeshSurface floor;

    /// <summary>
    /// 레스토랑의 테이블 리스트
    /// </summary>
    public List<Table> tables = new List<Table>();

    /// <summary>
    /// 현재 레스토랑에 있는 손님 GameObject 리스트
    /// </summary>
    public List<GameObject> customers = new List<GameObject>();

    /// <summary>
    /// 손님 GameObject 프리팹
    /// </summary>
    public GameObject customerPrefab;

    /// <summary>
    /// 손님 '주문 관리'를 위한 OrderManager
    /// </summary>
    public OrderManager orderManager;

    /// <summary>
    /// 손님 생성 Wave 인터페이스.
    /// 외부에서 설정 가능하도록 public으로 선언 ex) 평판 변경으로 난이도 상승 시 손님 생성 Wave 변경
    /// </summary>
    public ICustomerWave customerWave;

    /// <summary>
    /// 손님 생성 Coroutine
    /// </summary>
    private Coroutine customerSpawnCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        FindTable();
        /// <summary>
        /// 손님 생성 Wave 설정
        /// 최초에는 4초 후부터 4초 간격으로 손님 생성하는 로직 적용
        /// </summary>
        customerWave = new UniformCustomerWave(4f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 손님 입장 시작
    /// </summary>
    public void StartCustomerEnter()
    {
        Debug.Log("영업 시작");
        FindTable();

        // 손님 생성 coroutine 시작
        customerSpawnCoroutine = StartCoroutine(customerWave.SpawnCustomers(this));
    }

    /// <summary>
    /// 손님 퇴장 및 레스토랑 초기화
    /// </summary>
    public void StartCustomerExit()
    {
        // 손님 생성 coroutine 중지
        StopCoroutine(customerSpawnCoroutine);
        customerSpawnCoroutine = null; 
        
        FindCustomerExit();
    }

    /// <summary>
    /// 영업 시간 시작 직후 모든 Table 찾기
    /// </summary>
    public void FindTable()
    {
        // TODO: NavMesh 초기화하는 코드는 다른 곳으로 옮기기
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
        GameObject customer = Instantiate(customerPrefab);
        customers.Add(customer);
    }

    /// <summary>
    /// 손님 주문 처리
    /// </summary>
    /// <param name="customer">주문하는 손님</param>
    /// <param name="dishName">주문한 음식 이름</param>
    public void HandleOrder(CustomerNPC customer, FoodData dish)
    {
        Debug.Log($"Processing Order for {customer.name}: {dish}");

        Order newOrder = new Order(customer.name, dish);
        orderManager.SaveOrder(newOrder);

        // 음식 조리 시작하기 
    }

    /// <summary>
    /// 손님 퇴장  
    /// </summary>
    private void FindCustomerExit()
    {
        foreach (GameObject customer in customers)
        {
            CustomerNPC customerNPC = customer.GetComponent<CustomerNPC>();
            customerNPC.ExitRestaurant();
        }
        customers.Clear();
        tables.Clear();
    }

    public FoodData GetRandomDish()
    {
        FoodData orderedDish = orderManager.GetRandomEligibleFood();
        return orderedDish;
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

    public IEnumerator SpawnCustomers(CustomerManager CustomerManager)
    {
        yield return new WaitForSeconds(startDelay);

        while (true) {
            CustomerManager.SpawnCustomer();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
} 