using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private GameObject testObject;

    [SerializeField]
    private InteriorDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData, interiorData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;


    private void Start()
    {
        floorData = new GridData();
        interiorData = new();
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           floorData,
                                           interiorData,
                                           objectPlacer);
        inputManager.OnClicked += PlaceInterior;
        inputManager.OnExit += StopPlacement;
        inputManager.OnRotate += RotateInterior;
    }

    private void RotateInterior()
    {
        if(preview != null)
        {
            preview.RotatePreview();
        }
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, interiorData, objectPlacer);
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

        buildingState.OnAction(gridPosition);
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();

        inputManager.OnClicked -= PlaceInterior;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRotate -= RotateInterior;

        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    
}
