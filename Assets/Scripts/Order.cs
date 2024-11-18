using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    public GameObject customer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    // 버튼 UI 손님 따라 움직이기 
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(customer.transform.position + Vector3.up);
    }
}
