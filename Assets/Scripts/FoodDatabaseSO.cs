using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 음식(Food) 데이터베이스(재료, 중간단계, 요리)를 관리하는 ScriptableObject
/// 음식 정보를 저장하고 에디터에서 관리할 수 있음 
/// </summary>
[CreateAssetMenu]
public class FoodDatabaseSO : ScriptableObject
{
    /// <summary>
    /// 음식 데이터 리스트
    /// 각각의 항목은 <see cref="FoodData"/> 객체로 구성됨
    /// </summary>
    public List<FoodData> foodData;
}

/// <summary>
/// 개별 음식 데이터 정보를 저장하는 클래스
/// </summary>
[Serializable]
public class FoodData
{
    /// <summary>
    /// 음식 이름
    /// </summary>
    [field: SerializeField]
    public string name { get; private set; }
    
    /// <summary>
    /// 음식
    /// </summary>
    [field: SerializeField]
    public Yogaewonsil.Common.Food food { get; private set; }

    /// <summary>
    /// 음식 태크 (재료, 중간단계, 요리)
    /// </summary>
    [field: SerializeField]
    public FoodTag tag { get; private set; }

    /// <summary>
    /// 음식 고유 ID
    /// </summary>
    [field: SerializeField]
    public int id { get; private set; }
    
    /// <summary>
    /// 음식 가격 
    /// </summary>
    [field: SerializeField]
    public int price { get; private set; }

    /// <summary>
    /// 음식 해금 상태 
    /// </summary>
    [field: SerializeField]
    public bool isUnlocked { get; private set; } 
    
    /// <summary>
    /// 음식 구매 상태 
    /// </summary>
    [field: SerializeField]
    public bool isBought { get; private set; }  

    /// <summary>
    /// 음식에 필요한 레벨  
    /// </summary>
    [field: SerializeField]
    public int level { get; private set; }
    
    /// <summary>
    /// 음식 프리팹 
    /// </summary>
    [field: SerializeField]
    public GameObject prefab { get; private set; }

    /// <summary>
    /// 음식 아이콘
    /// </summary>
    [field: SerializeField]
    public Texture icon { get; private set; }

    /// <summary>
    /// 평판 상승 후 음식 해금 상태 변경 
    /// </summary>
    public void UpdateLockingStatus(bool status)
    {
        isUnlocked = status;
    }

    /// <summary>
    /// 음식 구매 후 음식 구매 상태 변경 
    /// </summary>
    public void UpdateBuyingStatus(bool status)
    {
        isBought = status;
    }
}
