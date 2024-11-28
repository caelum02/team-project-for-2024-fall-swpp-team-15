using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections;
using Yogaewonsil.Common;
using TMPro;

/// <summary>
/// 쓰레기통(Trashbin)을 제어하는 클래스입니다.
/// 플레이어가 들고 있는 재료를 제거하는 기능을 제공합니다.
/// </summary>
public class TrashbinController : KitchenInteriorBase
{
    private Button deleteButton; // 재료를 삭제하는 버튼

    /// <summary>
    /// 초기화 작업을 수행합니다. 버튼을 설정하고 부모 클래스의 초기화 로직을 호출합니다.
    /// </summary>
    protected override void Start()
    { 
        base.Start();
        
        // InteractionPanel에서 DeleteButton을 찾습니다.
        deleteButton = interactionPanel.Find("DeleteButton")?.GetComponent<Button>();
        if (deleteButton == null)
        {
            Debug.LogError($"RemoveButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        deleteButton.onClick.AddListener(Delete); // DeleteButton 클릭 시 Delete 함수 연결

    }

    /// <summary>
    /// 쓰레기통에서 '버리기'버튼을 누르면 플레이어가 들고 있는 재료를 제거합니다.
    /// </summary>
    private void Delete()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.heldFood = null; // 플레이어가 들고 있는 재료를 삭제
        }
    }

    /// <summary>
    /// 버튼의 활성화/비활성화 상태를 업데이트합니다.
    /// 플레이어가 재료를 들고 있을 때만 버튼이 활성화됩니다.
    /// </summary>
    protected override void UpdateAllButtons()  // private일지 protected일지 고려 -> 조리대에서 버튼 하나 추가되면 바뀔 수 있을 듯
    {
        // 현재 플레이어가 들고 있는 재료를 확인
        Food? heldFood = PlayerController.Instance.GetHeldFood();

        // DeleteButton은 플레이어가 재료를 들고 있을 때만 활성화
        deleteButton.interactable = heldFood != null;  // RemoveButton: 재료가 있으면 활성화
    }
}
