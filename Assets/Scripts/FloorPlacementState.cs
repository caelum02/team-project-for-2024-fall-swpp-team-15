using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FloorPlacementState : IPlacementState
{
    private int selectedInteriorIndex = -1;
    int ID = 0;
    Grid grid;
    PlacementSystem placementSystem;
    PreviewSystem previewSystem;
    InteriorDatabaseSO database;
    GridData floorData;
    GridData interiorData;
    ObjectPlacer objectPlacer;
    PlaceSoundFeedback soundFeedback;
    InteriorUI interiorUI;

    private static readonly Vector3Int[] neighborOffsets = new Vector3Int[]
    {
        new Vector3Int(-1, 0, 0), // Left
        new Vector3Int(1, 0, 0),  // Right
        new Vector3Int(0, 0, -1), // Bottom
        new Vector3Int(0, 0, 1)   // Top
    };

    private const int GRID_BOUND_X_MIN = -7;
    private const int GRID_BOUND_X_MAX = 6;
    private const int GRID_BOUND_Z_MIN = -4;
    private const int GRID_BOUND_Z_MAX = 4;

    public FloorPlacementState(Grid grid,
                          PreviewSystem previewSystem,
                          InteriorDatabaseSO database,
                          GridData floorData,
                          ObjectPlacer objectPlacer,
                          PlaceSoundFeedback soundFeedback,
                          InteriorUI interiorUI)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
        this.placementSystem = GameObject.FindObjectOfType<PlacementSystem>();
        this.interiorUI = interiorUI;

        selectedInteriorIndex = database.interiorData.FindIndex(data => data.ID == ID);
        if (selectedInteriorIndex > -1)
        {
            previewSystem.StartShowingFloorPlacementPreview(new Vector2Int(1, 9));
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        Debug.Log($"Grid Position: {gridPosition}");

        if (!CheckPlacementValidity(gridPosition, selectedInteriorIndex))
        {
            HandleInvalidPlacement();
            return;
        }

        HandleSuccessfulPlacement(gridPosition);
    }

    private void HandleInvalidPlacement()
    {
        soundFeedback.PlaySound(SoundType.Error);
        Debug.Log("Can't place item");
    }

    private void HandleSuccessfulPlacement(Vector3Int gridPosition)
    {
        placementSystem.floorPlacePosition = gridPosition;
        interiorUI.OnClickFloorBuy();
        placementSystem.pauseUpdate = true;
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedInteriorIndex)
    {
        if (!floorData.CanPlaceObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size))
            return false;

        if (!IsWithinGridBounds(gridPosition))
            return false;

        return HasFloorNearby(gridPosition);
    }

    private bool IsWithinGridBounds(Vector3Int gridPosition)
    {
        return gridPosition.x <= GRID_BOUND_X_MAX && gridPosition.x >= GRID_BOUND_X_MIN
            && gridPosition.z <= GRID_BOUND_Z_MAX && gridPosition.z >= GRID_BOUND_Z_MIN;
    }

    private bool HasFloorNearby(Vector3Int gridPosition)
    {
        foreach (var offset in neighborOffsets)
        {
            if (placementSystem.HasNeighbor(gridPosition, offset))
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedInteriorIndex);

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0
        cellCenterWorldPosition.z = 1.0f;

        previewSystem.UpdatePosition(cellCenterWorldPosition, placementValidity);
    }
}
