using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 그리드 데이터를 관리하는 클래스입니다.
/// </summary>
public class GridData
{
    // 배치된 객체들을 저장하는 딕셔너리입니다.
    public Dictionary<Vector3Int, PlacementData> placedObjects = new();

    /// <summary>
    /// 지정된 위치가 점유되었는지 확인합니다.
    /// </summary>
    /// <param name="position">확인할 그리드 위치입니다.</param>
    /// <returns>점유되었으면 true, 그렇지 않으면 false를 반환합니다.</returns>
    public bool IsOccupied(Vector3Int position)
    {
        return placedObjects.ContainsKey(position);
    }

    /// <summary>
    /// 모든 점유된 타일의 위치를 반환합니다.
    /// </summary>
    /// <returns>점유된 타일의 위치 목록입니다.</returns>
    public IEnumerable<Vector3Int> GetAllOccupiedTiles()
    {
        return placedObjects.Keys;
    }


    /// <summary>
    /// 지정된 위치에 객체를 추가합니다.
    /// </summary>
    /// <param name="gridPosition">객체를 추가할 그리드 위치입니다.</param>
    /// <param name="objectSize">객체의 크기입니다.</param>
    /// <param name="ID">객체의 ID입니다.</param>
    /// <param name="placedObjectIndex">배치된 객체의 인덱스입니다.</param>
    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex, Quaternion rotation)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex, rotation);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
        }
    }

    /// <summary>
    /// 객체의 위치를 계산합니다.
    /// </summary>
    /// <param name="gridPosition">기준 그리드 위치입니다.</param>
    /// <param name="objectSize">객체의 크기입니다.</param>
    /// <returns>객체가 차지하는 모든 그리드 위치 목록입니다.</returns>
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    /// <summary>
    /// 지정된 위치에 객체를 배치할 수 있는지 확인합니다.
    /// </summary>
    /// <param name="gridPosition">확인할 그리드 위치입니다.</param>
    /// <param name="objectSize">객체의 크기입니다.</param>
    /// <returns>배치 가능하면 true, 그렇지 않으면 false를 반환합니다.</returns>
    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 지정된 위치에 있는 객체를 제거합니다.
    /// </summary>
    /// <param name="gridPosition">제거할 객체의 그리드 위치입니다.</param>
    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }

    /// <summary>
    /// 지정된 위치에 있는 객체의 인덱스를 반환합니다.
    /// </summary>
    /// <param name="gridPosition">확인할 그리드 위치입니다.</param>
    /// <returns>객체의 인덱스를 반환합니다. 객체가 없으면 -1을 반환합니다.</returns>
    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }
}

/// <summary>
/// 객체 배치 데이터를 저장하는 클래스입니다.
/// </summary>
[Serializable]
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID;
    public int PlacedObjectIndex;
    public Quaternion rotation;

    /// <summary>
    /// PlacementData 생성자입니다.
    /// </summary>
    /// <param name="occupiedPositions">객체가 차지하는 위치 목록입니다.</param>
    /// <param name="ID">객체의 ID입니다.</param>
    /// <param name="placedObjectIndex">배치된 객체의 인덱스입니다.</param>
    public PlacementData(List<Vector3Int> occupiedPositions, int ID, int placedObjectIndex, Quaternion rotation)
    {
        this.occupiedPositions = occupiedPositions;
        this.ID = ID;
        this.PlacedObjectIndex = placedObjectIndex;
        this.rotation = rotation;
    }
}

[Serializable]
public class GridDataState
{
    public List<PlacementData> placedObjects = new List<PlacementData>();
}