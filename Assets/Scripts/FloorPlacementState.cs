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
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedInteriorIndex);
        if (placementValidity == false)
        {
            soundFeedback.PlaySound(SoundType.Error);
            Debug.Log("Can't place item");
            return;
        }

        placementSystem.floorPlacePosition = gridPosition;
        interiorUI.OnClickFloorBuy();
        placementSystem.pauseUpdate = true;
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedInteriorIndex)
    {
        bool validFlag = false;
        if(floorData.CanPlaceObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size))
        {
            if(gridPosition.x <= 6 && gridPosition.x >= -7 && gridPosition.z <= 4 && gridPosition.z >= -4)
            {
                foreach (var offset in neighborOffsets){
                    if(placementSystem.HasNeighbor(gridPosition, offset)){
                        validFlag = true;
                        break;
                    }
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
        cellCenterWorldPosition.z = 1.0f;

        previewSystem.UpdatePosition(cellCenterWorldPosition, placementValidity);
    }
}
