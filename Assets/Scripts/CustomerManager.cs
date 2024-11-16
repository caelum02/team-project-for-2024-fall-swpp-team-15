using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public NavMeshSurface floor;
    public List<Table> tables = new List<Table>();
    public List<GameObject> customers = new List<GameObject>();
    public GameObject customerPrefab;
    private float startDelay;
    private float spawnInterval;
    private bool isRestaurantOpen;

    // Start is called before the first frame update
    void Start()
    {
        FindTable();
        startDelay = 4; // 변경 가능 
        spawnInterval = 4; // 변경 가능 
        isRestaurantOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //손님 들어오기 
    public void StartCustomerEnter()
    {
        Debug.Log("영업 시작");
        isRestaurantOpen = true;
        FindTable();
        StartCoroutine(SpawnCustomers());
    }

    //손님 나가기 
    public void StartCustomerExit()
    {
        isRestaurantOpen = false;
        FindCustomerExit();
    }

    //영업 시간 시작 직후 모든 Table 찾기 
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
    }

    //착석 가능한 Table 찾기 
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

    //손님 계속 생성 
    private IEnumerator SpawnCustomers()
    {
        yield return new WaitForSeconds(startDelay);

        while (isRestaurantOpen)
        {
            SpawnCustomer();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    //손님 한 명 생성 
    private void SpawnCustomer()
    {
        GameObject customer = Instantiate(customerPrefab);
        customers.Add(customer);
    }

    //
    private void FindCustomerExit()
    {
        foreach (GameObject customer in customers)
        {
            CustomerNPC customerNPC = customer.GetComponent<CustomerNPC>();
            customerNPC.ExitRestaurant();
        }
    }
}