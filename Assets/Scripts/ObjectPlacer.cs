using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private Dictionary<Vector3Int, PositionObjectData> placedGameObjects = new Dictionary<Vector3Int, PositionObjectData>();

    public void PlaceObject(GameObject prefab, Vector3 position, Vector3Int gridPosition, Quaternion rotation, bool isInterior)
    {
        GameObject newObject = Instantiate(prefab, position, rotation);
        if(placedGameObjects.ContainsKey(gridPosition))
        {
            if (isInterior)
            {
                placedGameObjects[gridPosition].interiorObject = newObject;
            }
            else
            {
                placedGameObjects[gridPosition].floorObject = newObject;
            }
        }
        else
        {
            PositionObjectData data = new PositionObjectData();
            if (isInterior)
            {
                data.interiorObject = newObject;
            }
            else
            {
                data.floorObject = newObject;
            }
            placedGameObjects.Add(gridPosition, data);
        }
    }

    internal void RemoveObjectAt(Vector3Int position, bool isInterior)
    {
        if (isInterior)
        {
            Destroy(placedGameObjects[position].interiorObject);
            placedGameObjects[position].interiorObject = null;
        }
        else
        {
            Destroy(placedGameObjects[position].floorObject);
            placedGameObjects[position].floorObject = null;
        }
    }

    public void ClearAllObjects()
    {
        foreach (var item in placedGameObjects)
        {
            if (item.Value.interiorObject != null)
            {
                Destroy(item.Value.interiorObject);
            }
            if (item.Value.floorObject != null)
            {
                Destroy(item.Value.floorObject);
            }
        }
        placedGameObjects.Clear();
    }
}

public class PositionObjectData
{
    public GameObject interiorObject;
    public GameObject floorObject;

}
