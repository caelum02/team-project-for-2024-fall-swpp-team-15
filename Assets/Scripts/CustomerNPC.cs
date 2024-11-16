using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerNPC : MonoBehaviour
{
    public NavMeshAgent customerAgent;
    private Table assignedTable;
    private CustomerManager customerManager;
    
    // Start is called before the first frame update
    void Start()
    {
        customerManager = GameObject.Find("CustomerManager").GetComponent<CustomerManager>();
        FindAndMoveToTable();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //착석 가능한 Table을 찾아 해당 Table의 위치로 이동
    private void FindAndMoveToTable()
    {
        assignedTable = customerManager.GetAvailableTable();

        if (assignedTable != null)
        {
            customerAgent.SetDestination(assignedTable.transform.position);
        }
        else
        {
            Debug.LogWarning("No available tables for this customer.");
        }
    }

}
