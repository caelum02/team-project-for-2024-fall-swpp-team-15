using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerNPC : MonoBehaviour
{
    NavMeshAgent customerAgent;
    private Table assignedTable;
    private CustomerManager customerManager;
    // Start is called before the first frame update
    void Start()
    {
        //customerAgent = GetComponent<NavMeshAgent>();
        //customerManager = GameObject.Find("CustomerManager").GetComponent<CustomerManager>();
        FindAndMoveToTable();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FindAndMoveToTable()
    {
        //assignedTable = customerManager.GetAvailableTable();

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
