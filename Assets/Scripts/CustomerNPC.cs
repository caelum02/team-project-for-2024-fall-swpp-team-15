using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class CustomerNPC : MonoBehaviour
{
    public NavMeshAgent customerAgent;
    private Table assignedTable;
    private CustomerManager customerManager;
    public Vector3 spawnPosition;
    private bool isSeated = false;
    private Button orderButton;
    
    // Start is called before the first frame update
    void Start()
    {
        customerManager = GameObject.Find("CustomerManager").GetComponent<CustomerManager>();
        FindAndMoveToTable();
        spawnPosition = new Vector3(-15, 0, -5); //나가는 문 위치 
        orderButton = GetComponentInChildren<Button>();
        orderButton.gameObject.SetActive(false);
    }

    // Table에 도착했는지 계속 확인 
    void Update()
    {
        CheckIfReachedTable();
    }

    // 착석 가능한 Table을 찾아 해당 Table의 위치로 이동
    private void FindAndMoveToTable()
    {
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

    // Table에 도착하면, 주문 버튼 생성 
    private void CheckIfReachedTable()
    {
        if (assignedTable != null && !isSeated)
        {
            if (Vector3.Distance(transform.position, assignedTable.transform.position) < 1.5f) // Adjust threshold as needed
            {
                isSeated = true;
                orderButton.gameObject.SetActive(true);
                TextMeshProUGUI buttonText = orderButton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "주문 내역"; // 레시피 데이터베이스에서 요리 종류 받아와야 함 
            }
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