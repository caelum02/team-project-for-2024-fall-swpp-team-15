using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public NavMeshSurface floor;
    public List<Table> tables = new List<Table>();
    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
    }

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
}