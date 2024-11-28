using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections; // 코루틴에서 필요한 네임스페이스
using Yogaewonsil.Common;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스

public class TrashbinController : KitchenInteriorBase
{
    private Button deleteButton;

    protected override void Start()
    { 
        base.Start();
        // RemoveButton 찾기
        deleteButton = interactionPanel.Find("DeleteButton")?.GetComponent<Button>();
        if (deleteButton == null)
        {
            Debug.LogError($"RemoveButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        deleteButton.onClick.AddListener(Delete); 

    }

    private void Delete()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.heldFood = null;
        }
    }

    protected override void UpdateAllButtons()  // private일지 protected일지 고려 -> 조리대에서 버튼 하나 추가되면 바뀔 수 있을 듯
    {
        Food? heldFood = PlayerController.Instance.GetHeldFood();

        // 버튼 활성화/비활성화 상태 업데이트
        deleteButton.interactable = heldFood != null;  // RemoveButton: 재료가 있으면 활성화
    }
}
