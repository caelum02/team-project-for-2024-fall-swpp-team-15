using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 게임 내 객체 배치를 관리하는 클래스입니다.
/// </summary>
/// <remarks>
/// 그리드 상의 객체 배치, 회전 및 제거를 처리합니다.
/// </remarks>
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

    public GameObject kitchenFloorPrefab;
    public GameObject hallFloorPrefab;

    private List<GameObject> instantiatedWalls = new List<GameObject>();

    private string dataPath = "Assets/States";

    private static readonly Vector3Int[] neighborOffsets = new Vector3Int[]
    {
        new Vector3Int(-1, 0, 0), // Left
        new Vector3Int(1, 0, 0),  // Right
        new Vector3Int(0, 0, -1), // Bottom
        new Vector3Int(0, 0, 1)   // Top
    };

    private void Start()
    {
        
    }

    public void LoadGame()
    {
        string floorDataPath = dataPath + "/floorData.json";
        string interiorDataPath = dataPath + "/interiorData.json";
        
        floorData = DataManager.LoadGridData(floorDataPath);
        interiorData = DataManager.LoadGridData(interiorDataPath);

        foreach (var kvp in floorData.placedObjects)
        {
            Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(kvp.Key);
            cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

            GameObject prefab;

            if(kvp.Key.z >= 0)
            {
                prefab = kitchenFloorPrefab;
            }
            else
            {
                prefab = hallFloorPrefab;
            }

            objectPlacer.PlaceObject(prefab, cellCenterWorldPosition, kvp.Value.rotation);
        }

        foreach (var kvp in interiorData.placedObjects)
        {
            Vector3 cellCenterWorldPosition = grid.GetCellCenterWorld(kvp.Key);
            cellCenterWorldPosition.y = 0; // Ensure the y position is set to 0

            objectPlacer.PlaceObject(database.interiorData[kvp.Value.ID].Prefab, cellCenterWorldPosition, kvp.Value.rotation);
        }
        //floorData = new GridData();
        //interiorData = new();
        StopPlacement();
        gridVisualization.SetActive(false);
    }

    public void SaveGame()
    {
        string floorDataPath = dataPath + "/floorData.json";
        string interiorDataPath = dataPath + "/interiorData.json";

        DataManager.SaveGridData(floorData, floorDataPath);
        DataManager.SaveGridData(interiorData, interiorDataPath);
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

    /// <summary>
    /// 미리보기 객체를 회전시킵니다.
    /// </summary>
    private void RotateInterior()
    {
        if(preview != null)
        {
            preview.RotatePreview();
        }
    }

    /// <summary>
    /// 제거 작업을 시작합니다.
    /// </summary>
    /// <remarks>
    /// 현재 배치를 중지하고, 모든 벽을 제거하며, 제거 상태를 설정합니다.
    /// </remarks>
    public void StartRemoving()
    {
        StopPlacement();
        DestroyAllWalls();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, interiorData, objectPlacer);
        inputManager.OnClicked += PlaceInterior;
        inputManager.OnExit += StopPlacement;
    }

    /// <summary>
    /// 선택한 위치에 내부 객체를 배치합니다.
    /// </summary>
    /// <remarks>
    /// 포인터가 UI 위에 있는지 확인하고, 유효한 경우 객체를 배치합니다.
    /// </remarks>
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

    /// <summary>
    /// 현재 배치 작업을 중지합니다.
    /// </summary>
    /// <remarks>
    /// 모든 벽을 제거하고, 그리드 시각화를 숨기며, 빌딩 상태를 초기화합니다.
    /// </remarks>
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

    /// <summary>
    /// 배치 시스템을 업데이트합니다.
    /// </summary>
    /// <remarks>
    /// 현재 마우스 위치를 기반으로 상태를 업데이트합니다.
    /// </remarks>
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

    /// <summary>
    /// 지정된 방향으로 타일에 이웃이 있는지 확인합니다.
    /// </summary>
    /// <param name="gridPosition">확인할 타일의 위치입니다.</param>
    /// <param name="offset">이웃을 확인할 방향입니다.</param>
    /// <returns>이웃이 있으면 true, 없으면 false를 반환합니다.</returns>
    private bool HasNeighbor(Vector3Int gridPosition, Vector3Int offset)
    {
        Vector3Int neighborPosition = gridPosition + offset;
        return floorData.IsOccupied(neighborPosition);
    }

    /// <summary>
    /// 이웃이 없는 가장자리 타일에 벽을 생성합니다.
    /// </summary>
    private void CreateWallsForEdgeTiles()
    {
        foreach (var tile in floorData.GetAllOccupiedTiles())
        {
            foreach (var offset in neighborOffsets)
            {
                if (!HasNeighbor(tile, offset))
                {
                    Vector3 wallPosition = grid.GetCellCenterWorld(tile) + new Vector3(offset.x, 0, offset.z);
                    wallPosition.y = 0; // y 위치를 0으로 설정

                    Quaternion wallRotation = Quaternion.identity;
                    if (offset == new Vector3Int(-1, 0, 0)) // 왼쪽
                    {
                        wallRotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (offset == new Vector3Int(1, 0, 0)) // 오른쪽
                    {
                        wallRotation = Quaternion.Euler(0, -90, 0);
                    }
                    else if (offset == new Vector3Int(0, -1, 0)) // 아래
                    {
                        wallRotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (offset == new Vector3Int(0, 1, 0)) // 위
                    {
                        wallRotation = Quaternion.Euler(0, 180, 0);
                    }

                    GameObject wall = Instantiate(wallPrefab, wallPosition, wallRotation);
                    instantiatedWalls.Add(wall);
                }
            }
        }
    }

    /// <summary>
    /// 모든 생성된 벽 객체를 제거합니다.
    /// </summary>
    private void DestroyAllWalls()
    {
        foreach (var wall in instantiatedWalls)
        {
            Destroy(wall);
        }
        instantiatedWalls.Clear();
    }
}

public class DataManager : MonoBehaviour
{
    public static void SaveGridData(GridData gridData, string filePath)
    {
        GridDataState state = new GridDataState();
        foreach (var kvp in gridData.GetAllOccupiedTiles())
        {
            state.placedObjects.Add(new PlacementData(gridData.placedObjects[kvp].occupiedPositions, gridData.placedObjects[kvp].ID, gridData.placedObjects[kvp].PlacedObjectIndex, gridData.placedObjects[kvp].rotation));
        }

        string json = JsonUtility.ToJson(state);
        File.WriteAllText(filePath, json);
    }

    public static GridData LoadGridData(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new GridData();
        }

        string json = File.ReadAllText(filePath);
        GridDataState state = JsonUtility.FromJson<GridDataState>(json);

        GridData gridData = new GridData();
        foreach (var data in state.placedObjects)
        {
            gridData.AddObjectAt(data.occupiedPositions[0], new Vector2Int(data.occupiedPositions.Count, 1), data.ID, data.PlacedObjectIndex, data.rotation);
        }

        return gridData;
    }
}
