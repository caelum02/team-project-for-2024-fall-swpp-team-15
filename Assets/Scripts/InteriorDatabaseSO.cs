using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InteriorDatabaseSO : ScriptableObject
{
    public List<InteriorData> interiorData;
}

[Serializable]
public class InteriorData
{
    [field: SerializeField]
    public string name { get; private set; }

    [field: SerializeField]
    public Interior interior { get; private set; } 

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public int price { get; private set; }

    [field: SerializeField]
    public int stock { get; private set; }

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}