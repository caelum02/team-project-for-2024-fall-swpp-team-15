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
    InteriorUI interiorUI;
    InteriorDatabaseSO database;
    GridData floorData;
    GridData interiorData;
    ObjectPlacer objectPlacer;
    PlaceSoundFeedback soundFeedback;
    private const int CIRCLE_TABLE_ID = 12;
    private const int REC_TABLE_ID = 13;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          InteriorDatabaseSO database,
                          GridData floorData,
                          GridData interiorData,
                          ObjectPlacer objectPlacer,
                          PlaceSoundFeedback soundFeedback,
                          InteriorUI interiorUI)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.interiorData = interiorData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
        this.interiorUI = interiorUI;
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
        if (!CheckPlacementValidity(gridPosition, selectedInteriorIndex))
        {
            HandleInvalidPlacement();
            return;
        }

        PlaceObject(gridPosition);
        UpdatePreview(gridPosition);
    }

    private void HandleInvalidPlacement()
    {
        soundFeedback.PlaySound(SoundType.Error);
        Debug.Log("Can't place item");
    }

    private void PlaceObject(Vector3Int gridPosition)
    {
        Quaternion previewRotation = previewSystem.GetPreviewRotation();
        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

        GridData selectedData = GetSelectedData();
        GameObject prefab = GetPrefabForPlacement(selectedData, gridPosition);

        objectPlacer.PlaceObject(prefab, cellCenterWorldPosition, gridPosition, previewRotation, selectedData == interiorData);
        selectedData.AddObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size, database.interiorData[selectedInteriorIndex].ID, selectedData == interiorData, previewRotation);
        soundFeedback.PlaySound(SoundType.Place);
        database.interiorData[selectedInteriorIndex].ChangeInStock(-1);
        interiorUI.UpdateStock();
    }

    private GridData GetSelectedData()
    {
        return database.interiorData[selectedInteriorIndex].ID == 0 ? floorData : interiorData;
    }

    private GameObject GetPrefabForPlacement(GridData selectedData, Vector3Int gridPosition)
    {
        return database.interiorData[selectedInteriorIndex].Prefab;
    }

    private void UpdatePreview(Vector3Int gridPosition)
    {
        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0
        previewSystem.UpdatePosition(cellCenterWorldPosition, false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedInteriorIndex)
    {
        if (IsInvalidPlacement(gridPosition, selectedInteriorIndex)) return false;

        GridData selectedData = interiorData;

        if (selectedData.CanPlaceObjectAt(gridPosition, database.interiorData[selectedInteriorIndex].Size))
        {
            int interiorID = database.interiorData[selectedInteriorIndex].ID;
            if (interiorID == CIRCLE_TABLE_ID || interiorID == REC_TABLE_ID)
            {
                return gridPosition.z < 0 && !selectedData.CheckForNearbyTables(gridPosition); // 홀에만 설치 가능
            }
            else
            {
                return gridPosition.z >= 0; // 주방에만 설치 가능
            }
        }

        return false;
    }

    private bool IsInvalidPlacement(Vector3Int gridPosition, int selectedInteriorIndex)
    {
        return gridPosition == placementSystem.doorPosition || database.interiorData[selectedInteriorIndex].stock <= 0;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedInteriorIndex);

        Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(gridPosition);
        cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

        previewSystem.UpdatePosition(cellCenterWorldPosition, placementValidity);
    }
}
