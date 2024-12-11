using System.Collections;
using System.Collections.Generic;
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

    private static readonly Vector3Int[] neighborOffsets = new Vector3Int[]
    {
        new Vector3Int(-1, 0, 0), // Left
        new Vector3Int(1, 0, 0),  // Right
        new Vector3Int(0, 0, -1), // Bottom
        new Vector3Int(0, 0, 1)   // Top
    };

    public FloorPlacementState(Grid grid,
                          PreviewSystem previewSystem,
                          InteriorDatabaseSO database,
                          GridData floorData,
                          ObjectPlacer objectPlacer,
                          PlaceSoundFeedback soundFeedback)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
        this.placementSystem = GameObject.FindObjectOfType<PlacementSystem>();

        selectedInteriorIndex = database.interiorData.FindIndex(data => data.ID == ID);
        if (selectedInteriorIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
                database.interiorData[selectedInteriorIndex].Prefab,
                database.interiorData[selectedInteriorIndex].Size);
        }
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
            soundFeedback.PlaySound(SoundType.Error);
            Debug.Log("Can't place item");
            return;
        }

        Quaternion previewRotation = previewSystem.GetPreviewRotation();

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0
        
        GameObject prefab;
        for(int i=-4; i<4; i++)
        {
            if(i >= 0) prefab = placementSystem.kitchenFloorPrefab;
            else prefab = placementSystem.hallFloorPrefab;
            
            Vector3Int gridPos = new Vector3Int(gridPosition.x, 0, i);
            Vector3 placePos = grid.GetCellCenterWorld(gridPos);
            placePos.y = 0; // Ensure the y position is set to 0
            objectPlacer.PlaceObject(prefab, placePos, gridPos, previewRotation, floorData == interiorData);
            floorData.AddObjectAt(gridPos,database.interiorData[selectedInteriorIndex].Size,database.interiorData[selectedInteriorIndex].ID, floorData == interiorData, previewRotation);
        }
        soundFeedback.PlaySound(SoundType.Place);
        previewSystem.UpdatePosition(cellCenterWorldPosition, false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedInteriorIndex)
    {
        bool validFlag = false;
        if(floorData.CanPlaceObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size))
        {
            foreach (var offset in neighborOffsets){
                if(placementSystem.HasNeighbor(gridPosition, offset)){
                    validFlag = true;
                    break;
                }
            }
        }
        return validFlag;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedInteriorIndex);

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

        previewSystem.UpdatePosition(cellCenterWorldPosition, placementValidity);
    }
}
