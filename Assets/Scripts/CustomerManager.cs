using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    public NavMeshSurface floor;
    public List<Table> tables = new List<Table>();
    public List<GameObject> customers = new List<GameObject>();
    public GameObject customerPrefab;
    public OrderManager orderManager;

    // 손님 생성 Wave 인터페이스
    // 외부에서 설정 가능하도록 public으로 선언 ex) 평판 변경으로 난이도 상승 시 손님 생성 Wave 변경
    public ICustomerWave customerWave;
    // 손님 생성 Coroutine
    private Coroutine customerSpawnCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        FindTable();

        // 손님 생성 Wave 설정
        // 최초에는 4초 후부터 4초 간격으로 손님 생성하는 로직 적용
        customerWave = new UniformCustomerWave(4f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //손님 들어오기 
    public void StartCustomerEnter()
    {
        Debug.Log("영업 시작");
        FindTable();

        // 손님 생성 coroutine 시작
        customerSpawnCoroutine = StartCoroutine(customerWave.SpawnCustomers(this));
    }

    //손님 나가기 
    public void StartCustomerExit()
    {
        // 손님 생성 coroutine 중지
        StopCoroutine(customerSpawnCoroutine);
        customerSpawnCoroutine = null; 
        
        FindCustomerExit();
    }

    // 영업 시간 시작 직후 모든 Table 찾기 
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

    // 테이블 리스트 랜덤으로 섞기 
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

    // 착석 가능한 Table 찾기 
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

    // 손님 한 명 생성 
    public void SpawnCustomer()
    {
        GameObject customer = Instantiate(customerPrefab);
        customers.Add(customer);
    }

    // 주문 관리 
    public void HandleOrder(CustomerNPC customer, string dishName)
    {
        Debug.Log($"Processing Order for {customer.name}: {dishName}");

        Order newOrder = new Order(customer.name, dishName);
        orderManager.SaveOrder(newOrder);

        // 음식 조리 시작하기 
    }

    // 손님 나가기 
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
}

// 손님이 들어오는 로직을 관리하는 인터페이스
// SpawnCustomer: 손님을 생성하는 Coroutine
public interface ICustomerWave {
    // 손님 생성 Coroutine
    public abstract IEnumerator SpawnCustomers(CustomerManager CustomerManager);
}

// 일정한 시간 간격으로 손님을 생성하는 클래스
// startDelay: 손님 생성 시작까지의 대기 시간
// spawnInterval: 손님 생성 간격
public class UniformCustomerWave: ICustomerWave {

    private float startDelay;
    private float spawnInterval;

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