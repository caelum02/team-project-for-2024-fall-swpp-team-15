using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerNPC : MonoBehaviour
{
    public NavMeshAgent customerAgent;
    private Table assignedTable;
    private CustomerManager customerManager;
    public Vector3 spawnPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        customerManager = GameObject.Find("CustomerManager").GetComponent<CustomerManager>();
        FindAndMoveToTable();
        spawnPosition = new Vector3(-15, 0, -5); //나가는 문 위치 
    }

    // Update is called once per frame
    void Update()
    {

    }

    //착석 가능한 Table을 찾아 해당 Table의 위치로 이동
    private void FindAndMoveToTable()
    {
        //assignedTable = customerManager.GetAvailableTable();

        if (assignedTable != null)
        {
            customerAgent.SetDestination(assignedTable.transform.position);
        }
        else
        {
            Debug.Log("No available tables for this customer.");
        }
    }

    //레스토랑 나가기 
    public void ExitRestaurant()
    {
        if (assignedTable != null)
        {
            assignedTable.Vacate();
        }

        customerAgent.SetDestination(spawnPosition);
        StartCoroutine(CheckIfReachedExit());
    }

    //나간 후 사라지기 
    private IEnumerator CheckIfReachedExit()
    {
        while (Vector3.Distance(transform.position, spawnPosition) > 3f) //정확한 위치는 추후 조정 
        {
            yield return null;
        }
        Destroy(gameObject);
    }

}
