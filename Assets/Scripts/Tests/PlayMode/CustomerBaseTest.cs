using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Yogaewonsil.Common;

public class CustomerBasePlayModeTests
{
    private GameObject customerPrefab;
    private GameObject customerObj;
    private GameObject tablePrefab;
    private GameObject tableObj;
    private CustomerBase customer;
    private CustomerManager customerManager; 
    private GameManager gameManager;   
    private Table testTable;

    // [SetUp]
    // public void Setup()
    // {
    //     // 현재 Scene에서 Customer 프리팹 로드
    //     customerPrefab = Resources.Load<GameObject>("Prefabs/NPCs/Customer_Normal");
    //     Assert.IsNotNull(customerPrefab, "Customer prefab not found. Check Resources path.");

    //     // Table 오브젝트 생성 및 설정
    //     tablePrefab = Resources.Load<GameObject>("Prefabs/Utensils/Table_Circle");
    //     Assert.IsNotNull(tablePrefab, "Table prefab not found. Check Resources path.");

    //     tableObj = Object.Instantiate(tablePrefab);
    //     Assert.IsNotNull(tableObj, "Table object not found in the current Scene.");

    //     testTable = tableObj.GetComponent<Table>();
    //     Assert.IsNotNull(testTable, "Table component not found on Table object.");

    //     // Customer 프리팹 인스턴스화
    //     customerObj = Object.Instantiate(customerPrefab);
    //     customer = customerObj.GetComponent<CustomerBase>();
    //     Assert.IsNotNull(customer, "CustomerBase component not found on prefab.");

    //     // NavMeshAgent 활성화
    //     customer.customerAgent = customerObj.GetComponent<NavMeshAgent>();
    //     customer.customerAgent.enabled = true;
    // }

    [UnitySetUp]
    public IEnumerator Setup()
    {   
        if (SceneManager.GetActiveScene().name != "TestScene")
        {
            SceneManager.LoadScene("TestScene");
            yield return null; // 씬이 로드될 때까지 대기
        }

        // Table 프리팹 로드
        tablePrefab = Resources.Load<GameObject>("Prefabs/Utensils/Table_Circle");
        Assert.IsNotNull(tablePrefab, "Table prefab not found. Check Resources path.");

        // // Table 설치
        // tableObj = Object.Instantiate(tablePrefab);
        // tableObj.name = "Table_test";

        // testTable = tableObj.GetComponent<Table>();

        // GameManager 찾기
        GameObject gameManagerObj = GameObject.Find("GameManager");
        Assert.IsNotNull(gameManagerObj, "GameManager object not found in the current scene.");

        gameManager = gameManagerObj.GetComponent<GameManager>();
        Assert.IsNotNull(gameManager, "CustomerManager component not found.");

        // CustomerManager 찾기
        GameObject managerObj = GameObject.Find("CustomerManager");
        Assert.IsNotNull(managerObj, "CustomerManager object not found in the current scene.");

        customerManager = managerObj.GetComponent<CustomerManager>();
        Assert.IsNotNull(customerManager, "CustomerManager component not found.");

        // NavMeshSurface 빌드
        //customerManager.floor.BuildNavMesh(); // NavMesh 빌드를 포함한 테이블 초기화
        // customerManager.FindTable();
        // testTable = customerManager.tables[0];

        // Customer 프리팹 생성
        // customerManager.SpawnCustomer();
        // customer = customerManager.customers[0];

        // Customer 프리팹 로드 및 인스턴스화
        customerPrefab = Resources.Load<GameObject>("Prefabs/NPCs/Customer_Normal");
        Assert.IsNotNull(customerPrefab, "Customer prefab not found. Check Resources path.");

        // customerObj = Object.Instantiate(customerPrefab);
        // customerObj.name = "Customer_Test";

        // customer = customerObj.GetComponent<CustomerBase>();
        // Assert.IsNotNull(customer, "CustomerBase component not found on prefab.");
    }

    /// <summary>
    /// 테이블 하나, 손님 하나를 소환하고 손님이 테이블을 찾아 이동하는지 확인
    /// </summary>
    [UnityTest]
    public IEnumerator Customer_Find_Table()
    {
        // Table 설치
        tableObj = Object.Instantiate(tablePrefab);
        tableObj.name = "Table_test";

        testTable = tableObj.GetComponent<Table>();

        // NavMeshSurface 빌드
        //customerManager.floor.BuildNavMesh(); // NavMesh 빌드를 포함한 테이블 초기화
        customerManager.FindTable();

        // Customer 생성
        customerManager.SpawnCustomer();
        customer = customerManager.customers[0];

        yield return new WaitForSeconds(5f);

        Assert.IsNotNull(customer.assignedTable, "Customer should be assigned to a table.");
        Assert.IsTrue(customer.assignedTable.isOccupied, "Customer should occupy the table.");
    }

    /// <summary>
    /// 여러 개의 테이블 중 손님이 맞는 테이블을 찾아갔는지 확인
    /// </summary>
    [UnityTest]
    public IEnumerator Customer_Find_Accurate_Table()
    {   

        for (int i = 0; i < 5; i += 1)
        {
            tableObj = Object.Instantiate(tablePrefab, new Vector3(i, 0, 0), Quaternion.identity);
            tableObj.name = $"Table_test{i}";
        }

        customerManager.FindTable();

        // Customer 생성
        for (int i = 0; i < 10; i += 1)
        {
            customerManager.SpawnCustomer();
        }

        // Customer의 수가 Table 수를 초과하지 않는지 확인
        Assert.AreEqual(customerManager.tables.Count, customerManager.customers.Count, "Customers should not be created in excess of the number of tables available.");

        // 5초 대기 후 검증
        yield return new WaitForSeconds(5f);

        for (int i = 0; i < 5; i += 1)
        {
            Assert.IsNotNull(customerManager.customers[i].assignedTable, "Customer should be assigned to a table.");
        }
    }
    
    [UnityTest]
    public IEnumerator Customer_ShouldMoveToAvailableTable()
    {
        // 5초 대기 후 검증
        yield return new WaitForSeconds(5f);

        Assert.IsNotNull(customer.assignedTable, "Customer should be assigned to a table.");
        Assert.AreEqual(testTable, customer.assignedTable, "Customer should move to the correct table.");
    }

    /// <summary>
    /// 손님이 테이블에 도착하면 주문하는지 확인, 인내심이 감소하는지 확인
    /// </summary>
    [UnityTest]
    public IEnumerator Customer_ShouldMakeOrder_WhenSeated()
    {
        // Table 설치
        tableObj = Object.Instantiate(tablePrefab);
        tableObj.name = "Table_test";

        testTable = tableObj.GetComponent<Table>();

        // NavMeshSurface 빌드
        //customerManager.floor.BuildNavMesh(); // NavMesh 빌드를 포함한 테이블 초기화
        customerManager.FindTable();

        // Customer 생성
        customerManager.SpawnCustomer();
        customer = customerManager.customers[0];

        yield return new WaitForSeconds(5f);

        float initialPatience = customer.patienceTimer;

        Assert.IsTrue(customer.hasOrdered, "Customer should place an order when seated."); // 손님이 주문을 했는지 확인
        Assert.IsNotNull(customer.requiredDish, "Customer should make order"); 
        Assert.IsTrue(customer.orderButton.interactable, "Orderbutton should be activated.");
        
        yield return new WaitForSeconds(2f);
        Assert.Less(customer.patienceTimer, initialPatience, "Patience timer should decrease over time.");
    }

    /// <summary>
    /// 인내심이 바닥나면 손님이 퇴장하는지 확인
    /// </summary>
    [UnityTest]
    public IEnumerator Customer_ShouldExit_WhenPatienceRunsOut()
    {   
        // Table 설치
        tableObj = Object.Instantiate(tablePrefab);
        tableObj.name = "Table_test";

        testTable = tableObj.GetComponent<Table>();

        // NavMeshSurface 빌드
        //customerManager.floor.BuildNavMesh(); // NavMesh 빌드를 포함한 테이블 초기화
        customerManager.FindTable();

        // Customer 생성
        customerManager.SpawnCustomer();
        customer = customerManager.customers[0];
        customerObj = customer.gameObject;

        // 초기 돈 확인
        float initialMoney = gameManager.money;

        // 초기 평판 확인
        float initialReputationVale = gameManager.reputationValue;

        yield return new WaitForSeconds(5f); // 주문하기까지 기다리기 
        customer.patienceTimer = 1; // 인내심 즉시 1로 설정

        yield return new WaitForSeconds(2f); // 인내심 바닥날 때까지 대기

        Assert.IsFalse(customer.orderButton.interactable, "Orderbutton should be deactivated when customer is exiting");
        Assert.IsFalse(customer.assignedTable.isOccupied, "Customer should vacate table");

        yield return new WaitForSeconds(3f); // 퇴장까지 대기

        Assert.IsTrue(customerObj == null, "Customer object should be destroyed after reaching the exit.");
        Assert.AreEqual(gameManager.money, initialMoney, "Customer should not pay money");
        Assert.AreEqual(gameManager.reputationValue, initialReputationVale, "Customer should not pay reputation");
    }

    /// <summary>
    /// 테이블에 음식이 놓여지면 손님이 음식을 먹기 시작하는지 확인
    /// </summary>
    [UnityTest]
    public IEnumerator Customer_ShouldStartEating_WhenFoodIsServed()
    {
        // Table 설치
        tableObj = Object.Instantiate(tablePrefab);
        tableObj.name = "Table_test";

        testTable = tableObj.GetComponent<Table>();

        // NavMeshSurface 빌드
        //customerManager.floor.BuildNavMesh(); // NavMesh 빌드를 포함한 테이블 초기화
        customerManager.FindTable();

        // Customer 생성
        customerManager.SpawnCustomer();
        customer = customerManager.customers[0];

        yield return new WaitForSeconds(3f);

        // 테이블에 음식 배치
        testTable.plateFood = customer.requiredDish.food;

        yield return null; // 한 프레임 대기

        Assert.IsTrue(customer.isEating, "Customer should start eating when food is served.");
    }

    /// <summary>
    /// 음식을 다 먹으면 손님이 퇴장하는지 확인 + 만족스러운 음식을 줬을때 돈이 올라가는지 확인
    /// </summary>
    [UnityTest]
    public IEnumerator Customer_AfterEating_AccurateFood()
    {
        // Table 설치
        tableObj = Object.Instantiate(tablePrefab);
        tableObj.name = "Table_test";

        testTable = tableObj.GetComponent<Table>();

        // NavMeshSurface 빌드
        //customerManager.floor.BuildNavMesh(); // NavMesh 빌드를 포함한 테이블 초기화
        customerManager.FindTable();

        // Customer 생성
        customerManager.SpawnCustomer();
        customer = customerManager.customers[0];

        // 초기 돈 확인
        float initialMoney = gameManager.money;

        // 초기 평판 확인
        float initialReputationVale = gameManager.reputationValue;

        yield return new WaitForSeconds(3f);

        // 테이블에 음식 배치
        testTable.plateFood = customer.requiredDish.food;

        yield return new WaitForSeconds(5.5f); // 음식 먹는 시간 대기

        Assert.IsFalse(customer.orderButton.interactable, "Orderbutton should be deactivated when customer is exiting");
        Assert.IsFalse(customer.assignedTable.isOccupied, "Customer should vacate table");
        Assert.IsTrue(customer.isSuccessTreatment, "Customer should be satisfied");

        yield return new WaitForSeconds(3f); // 퇴장까지 대기

        Assert.IsTrue(customerObj == null, "Customer object should be destroyed after reaching the exit.");
        Assert.Greater(gameManager.money, initialMoney, "Customer should pay money");
        Assert.Greater(gameManager.reputationValue, initialReputationVale, "Customer should pay reputation");
    }

        /// <summary>
    /// 음식을 다 먹으면 손님이 퇴장하는지 확인 + 만족스럽지 않은 음식을 줬을때 돈이 올라가는지 확인
    /// </summary>
    [UnityTest]
    public IEnumerator Customer_AfterEating_WrongFood()
    {
        // Table 설치
        tableObj = Object.Instantiate(tablePrefab);
        tableObj.name = "Table_test";

        testTable = tableObj.GetComponent<Table>();

        // NavMeshSurface 빌드
        //customerManager.floor.BuildNavMesh(); // NavMesh 빌드를 포함한 테이블 초기화
        customerManager.FindTable();

        // Customer 생성
        customerManager.SpawnCustomer();
        customer = customerManager.customers[0];

        // 초기 돈 확인
        float initialMoney = gameManager.money;

        // 초기 평판 확인
        float initialReputationVale = gameManager.reputationValue;

        yield return new WaitForSeconds(3f);

        // 테이블에 음식 배치
        testTable.plateFood = Food.쌀;

        yield return new WaitForSeconds(5.5f); // 음식 먹는 시간 대기

        Assert.IsFalse(customer.orderButton.interactable, "Orderbutton should be deactivated when customer is exiting");
        Assert.IsFalse(customer.assignedTable.isOccupied, "Customer should vacate table");
        Assert.IsFalse(customer.isSuccessTreatment, "Customer should not be satisfied");

        yield return new WaitForSeconds(3f); // 퇴장까지 대기

        Assert.IsTrue(customerObj == null, "Customer object should be destroyed after reaching the exit.");
        Assert.AreEqual(gameManager.money, initialMoney, "Customer should pay money");
        Assert.AreEqual(gameManager.reputationValue, initialReputationVale, "Customer should pay reputation");
    }

    /// <summary>
    /// 손님이 퇴장 지점에 도달하면 삭제되는지 확인
    /// </summary>
    [UnityTest]
    public IEnumerator Customer_ShouldBeDestroyed_WhenReachingExit()
    {   
        // Table 설치
        tableObj = Object.Instantiate(tablePrefab);
        tableObj.name = "Table_test";

        testTable = tableObj.GetComponent<Table>();

        // NavMeshSurface 빌드
        //customerManager.floor.BuildNavMesh(); // NavMesh 빌드를 포함한 테이블 초기화
        customerManager.FindTable();

        // Customer 생성
        customerManager.SpawnCustomer();
        customer = customerManager.customers[0];

        customerObj = customer.gameObject;

        yield return new WaitForSeconds(3f);

        // 퇴장 처리
        customer.ExitRestaurant();

        yield return new WaitForSeconds(5f);

        Assert.IsTrue(customerObj == null, "Customer object should be destroyed after reaching the exit.");
    }
}
