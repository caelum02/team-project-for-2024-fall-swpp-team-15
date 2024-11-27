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

    IPlacementState buildingState;

    [SerializeField]
    private GameObject wallPrefab;

    private List<GameObject> instantiatedWalls = new List<GameObject>();

    private static readonly Vector3Int[] neighborOffsets = new Vector3Int[]
    {
        new Vector3Int(-1, 0, 0), // Left
        new Vector3Int(1, 0, 0),  // Right
        new Vector3Int(0, 0, -1), // Bottom
        new Vector3Int(0, 0, 1)   // Top
    };

    private void Start()
    {
        floorData = new GridData();
        interiorData = new();
        StopPlacement();
        gridVisualization.SetActive(false);
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        DestroyAllWalls();
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
        DestroyAllWalls();
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

    public void StopPlacement()
    {
        DestroyAllWalls();
        CreateWallsForEdgeTiles();
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

    private bool HasNeighbor(Vector3Int gridPosition, Vector3Int offset)
    {
        Vector3Int neighborPosition = gridPosition + offset;
        return floorData.IsOccupied(neighborPosition);
    }

    private void CreateWallsForEdgeTiles()
    {
        foreach (var tile in floorData.GetAllOccupiedTiles())
        {
            foreach (var offset in neighborOffsets)
            {
                if (!HasNeighbor(tile, offset))
                {
                    Vector3 wallPosition = grid.GetCellCenterWorld(tile)+ new Vector3(offset.x, 0, offset.z);
                    wallPosition.y = 0; // Ensure the y position is set to 0

                    Quaternion wallRotation = Quaternion.identity;
                    if (offset == new Vector3Int(-1, 0, 0)) // Left
                    {
                        wallRotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (offset == new Vector3Int(1, 0, 0)) // Right
                    {
                        wallRotation = Quaternion.Euler(0, -90, 0);
                    }
                    else if (offset == new Vector3Int(0, 0, -1)) // Bottom
                    {
                        wallRotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (offset == new Vector3Int(0, 0, 1)) // Top
                    {
                        wallRotation = Quaternion.Euler(0, 180, 0);
                    }

                    GameObject wall = Instantiate(wallPrefab, wallPosition, wallRotation);
                    instantiatedWalls.Add(wall);
                }
            }
        }
    }

    private void DestroyAllWalls()
    {
        foreach (var wall in instantiatedWalls)
        {
            Destroy(wall);
        }
        instantiatedWalls.Clear();
    }
}
