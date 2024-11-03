using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private GameObject testObject;

    [SerializeField]
    private InteriorDatabaseSO database;
    private int selectedInteriorIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedInteriorIndex = database.interiorData.FindIndex(data => data.ID == ID);
        if(selectedInteriorIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceInterior;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceInterior()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        GameObject newObject = Instantiate(database.interiorData[selectedInteriorIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        Debug.Log("Grid Pos: " + gridPosition);
    }

    private void StopPlacement()
    {
        selectedInteriorIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceInterior;
        inputManager.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if (selectedInteriorIndex < 0)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }

    
}
