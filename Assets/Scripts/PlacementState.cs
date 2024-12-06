using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IPlacementState
{
    private int selectedInteriorIndex = -1;
    int ID;
    Grid grid;
    PlacementSystem placementSystem;
    PreviewSystem previewSystem;
    InteriorDatabaseSO database;
    GridData floorData;
    GridData interiorData;
    ObjectPlacer objectPlacer;
    private const int TABLE_ID = 12;
    private const int CHAIR_ID = 13;

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
        this.placementSystem = GameObject.FindObjectOfType<PlacementSystem>();

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
        Debug.Log($"Grid Position: {gridPosition}");
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedInteriorIndex);
        if (placementValidity == false)
        {
            Debug.Log("Can't place item");
            return;
        }

        Quaternion previewRotation = previewSystem.GetPreviewRotation();

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

        GridData selectedData = database.interiorData[selectedInteriorIndex].ID == 0 ?
            floorData :
            interiorData;
        
        GameObject prefab;
        if(selectedData == floorData){
            if(gridPosition.z >= 0)
            {
                prefab = placementSystem.kitchenFloorPrefab;
            }
            else
            {
                prefab = placementSystem.hallFloorPrefab;
            }

        }
        else{
            prefab = database.interiorData[selectedInteriorIndex].Prefab;
        }

        int index = objectPlacer.PlaceObject(prefab, cellCenterWorldPosition, previewRotation);

        
        selectedData.AddObjectAt(gridPosition,
            database.interiorData[selectedInteriorIndex].Size,
            database.interiorData[selectedInteriorIndex].ID,
            index, 
            previewRotation);
        
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
                if(selectedData.CanPlaceObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size)) // interiorData에 gridPosition이 있는지 확인하기
                {
                    if(database.interiorData[selectedInteriorIndex].ID == TABLE_ID || 
                    database.interiorData[selectedInteriorIndex].ID == CHAIR_ID) // 의자 또는 테이블이면
                    {
                        return gridPosition.z < 0; // 홀에 만 설치 가능
                    }
                    else
                    {
                        return gridPosition.z >= 0; // 아니면 주방에만 설치 가능
                    }
                } 
                else
                {
                    return false;
                }
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
