using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IPlacementState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData interiorData;
    ObjectPlacer objectPlacer;
    PlaceSoundFeedback soundFeedback;
    InteriorDatabaseSO database;
    InteriorUI interiorUI;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData interiorData,
                         ObjectPlacer objectPlacer,
                         PlaceSoundFeedback soundFeedback,
                         InteriorUI interiorUI,
                         InteriorDatabaseSO database)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.interiorData = interiorData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
        this.interiorUI = interiorUI;
        this.database = database;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = GetSelectedData(gridPosition);
        if (selectedData != interiorData)
        {
            HandleInvalidSelection();
            return;
        }

        int selectedInteriorIndex = selectedData.placedObjects[gridPosition].ID;
        RemoveObject(selectedData, gridPosition, selectedInteriorIndex);
        UpdateUIAndPreview(gridPosition, selectedInteriorIndex);
    }

    private GridData GetSelectedData(Vector3Int gridPosition)
    {
        if (!interiorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            return interiorData;
        }
        if (!floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            return floorData;
        }
        return null;
    }

    private void HandleInvalidSelection()
    {
        Debug.Log("This is not interior data");
        soundFeedback.PlaySound(SoundType.Error);
    }

    private void RemoveObject(GridData selectedData, Vector3Int gridPosition, int selectedInteriorIndex)
    {
        if (selectedData == interiorData)
        {
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gridPosition, true);
            soundFeedback.PlaySound(SoundType.Remove);
        }
    }

    private void UpdateUIAndPreview(Vector3Int gridPosition, int selectedInteriorIndex)
    {
        database.interiorData[selectedInteriorIndex].ChangeInStock(1);
        interiorUI.UpdateStock();

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

        previewSystem.UpdatePosition(cellCenterWorldPosition, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(interiorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

        previewSystem.UpdatePosition(cellCenterWorldPosition, validity);
    }
}
