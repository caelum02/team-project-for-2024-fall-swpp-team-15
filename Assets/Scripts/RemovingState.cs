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

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData interiorData,
                         ObjectPlacer objectPlacer,
                         PlaceSoundFeedback soundFeedback)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.interiorData = interiorData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (interiorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = interiorData;
        }
        else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = floorData;
        }

        if (selectedData == null)
        {

        }
        else
        {
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gridPosition, selectedData == interiorData);
            soundFeedback.PlaySound(SoundType.Remove);
            
        }

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0
        
        previewSystem.UpdatePosition(cellCenterWorldPosition, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(interiorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

        previewSystem.UpdatePosition(cellCenterWorldPosition, validity);
    }
}
