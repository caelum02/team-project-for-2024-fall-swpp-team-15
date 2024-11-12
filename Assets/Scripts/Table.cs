using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public bool isOccupied = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Occupy()
    {
        isOccupied = true;
    }

    public void Vacate()
    {
        isOccupied = false;
    }
}
