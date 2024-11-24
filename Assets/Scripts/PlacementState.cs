using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
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

        int index = objectPlacer.PlaceObject(database.interiorData[selectedInteriorIndex].Prefab, grid.GetCellCenterWorld(gridPosition), previewRotation);

        GridData selectedData = database.interiorData[selectedInteriorIndex].ID == 0 ?
            floorData :
            interiorData;
        selectedData.AddObjectAt(gridPosition,
            database.interiorData[selectedInteriorIndex].Size,
            database.interiorData[selectedInteriorIndex].ID,
            index);

        previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedInteriorIndex)
    {
        GridData selectedData = database.interiorData[selectedInteriorIndex].ID == 0 ?
            floorData :
            interiorData;

        return selectedData.CanPlaceObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedInteriorIndex);

        previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition), placementValidity);
    }
}
