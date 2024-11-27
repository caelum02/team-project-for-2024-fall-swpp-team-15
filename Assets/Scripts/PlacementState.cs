using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IPlacementState
{
    private int selectedInteriorIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    InteriorDatabaseSO database;
    GridData floorData;
    GridData interiorData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          InteriorDatabaseSO database,
                          GridData floorData,
                          GridData interiorData,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.interiorData = interiorData;
        this.objectPlacer = objectPlacer;

        selectedInteriorIndex = database.interiorData.FindIndex(data => data.ID == ID);
        if (selectedInteriorIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
                database.interiorData[selectedInteriorIndex].Prefab,
                database.interiorData[selectedInteriorIndex].Size);
        }
        else
            throw new System.Exception($"No object with ID {iD}");
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedInteriorIndex);
        if (placementValidity == false)
        {
            Debug.Log("Can't place item");
            return;
        }

        Quaternion previewRotation = previewSystem.GetPreviewRotation();

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

        int index = objectPlacer.PlaceObject(database.interiorData[selectedInteriorIndex].Prefab, cellCenterWorldPosition, previewRotation);

        GridData selectedData = database.interiorData[selectedInteriorIndex].ID == 0 ?
            floorData :
            interiorData;
        selectedData.AddObjectAt(gridPosition,
            database.interiorData[selectedInteriorIndex].Size,
            database.interiorData[selectedInteriorIndex].ID,
            index);
        
        previewSystem.UpdatePosition(cellCenterWorldPosition, false);
        
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedInteriorIndex)
    {
        GridData selectedData = database.interiorData[selectedInteriorIndex].ID == 0 ?
            floorData :
            interiorData;
        
        if (selectedData == interiorData)
        {
            if (!floorData.CanPlaceObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size)) // floorData에 gridPosition이 있는지 확인하기
            {
                return selectedData.CanPlaceObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size); // interiorData에 gridPosition이 있는지 확인하기
            }
            else
            {
                return false;
            }
        }
        else{
            return selectedData.CanPlaceObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size); // floor
        }
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedInteriorIndex);

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

        previewSystem.UpdatePosition(cellCenterWorldPosition, placementValidity);
    }
}
