using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using Yogaewonsil.Common;

public abstract class TableBase : KitchenInteriorBase
{   
    [SerializeField] private Transform platePoint; // 객체를 붙일 위치
    public Food? plateFood = null; 
    protected Button putButton;
    public GameObject currentPlateObject; // 테이블이 현재 들고 있는 프리팹 객체

    [Header("Audio Settings")]
    [SerializeField] protected AudioSource audioSource; // 사운드를 재생할 AudioSource
    [SerializeField] private AudioClip putSound; // 음식 놓을 때 재생할 사운드

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // InteractionPanel에서 OpenButton을 찾습니다.
        putButton = interactionPanel.Find("PutButton")?.GetComponent<Button>();
        if (putButton == null)
        {
            Debug.LogError($"putButton not found in Table");
            return;
        }
        putButton.onClick.AddListener(PutDish); //putButton에 putDish()함수 연결
    }

    protected virtual void PutDish()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.heldFood != null)
        {   
            if (audioSource != null && putSound != null)
            {
                audioSource.clip = putSound;
                audioSource.loop = false; // 필요 시 루프 설정
                audioSource.Play(); // 사운드 재생
            }

            plateFood = (Food)PlayerController.Instance.heldFood; // 재료 추가
            PlayerController.Instance.DropFood(); // 플레이어가 들고 있는 재료 제거
            InstantiateFoodPrefab();
        }

        UpdateAllButtons();
    } 

    /// <summary>
    /// plateFood에 해당하는 프리팹을 생성하고 플레이어의 자식으로 설정합니다.
    /// </summary>
    protected void InstantiateFoodPrefab()
    {
        if (plateFood == null)
        {
            Debug.LogWarning("No plated food to instantiate.");
            return;
        }

        FoodData foodData = FoodDataUtility.FindFoodDataByType(foodDatabase, (Food)plateFood);
        if (foodData == null || foodData.prefab == null)
        {
            Debug.LogWarning($"No prefab found for plate food: {plateFood}");
            return;
        }

        // 프리팹 생성
        currentPlateObject = Instantiate(foodData.prefab, platePoint.position, Quaternion.identity, platePoint);

        Debug.Log($"Spawned prefab for {plateFood} at {platePoint.position}");
    }
}
