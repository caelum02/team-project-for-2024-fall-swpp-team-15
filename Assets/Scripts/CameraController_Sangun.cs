using UnityEngine;
using UnityEngine.UI;

public class CameraController_Sangun : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;       // 줌인/줌아웃 속도
    public float minZoom = 15f;         // 최소 줌 (FOV 또는 거리)
    public float maxZoom = 60f;         // 최대 줌 (FOV 또는 거리)

    [Header("Zoom Mode")]
    public bool useFOVZoom = true;      // FOV를 변경하여 줌을 구현할지 여부 (true면 FOV 방식, false면 거리 방식)
    public Transform target;            // 카메라가 바라보는 대상 (거리 줌 모드에서 사용)

    private Camera cam;
    private float currentZoom;          // 현재 줌 상태
    private Vector3 initialOffset;      // 초기 오프셋 (거리 줌 모드에서 사용)

    void Start()
    {
        // 카메라 컴포넌트 가져오기
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraController: Camera component not found!");
            return;
        }

        // 초기 줌 상태 설정
        currentZoom = useFOVZoom ? cam.fieldOfView : Vector3.Distance(transform.position, target.position);

        // 거리 줌 모드일 경우 초기 위치 설정
        if (!useFOVZoom && target != null)
        {
            initialOffset = transform.position - target.position;
        }
    }

    void Update()
    {
        HandleZoom();
    }

    private void HandleZoom()
    {   
        if (!IsActiveScrollbarInScene())
        {
          float scrollInput = Input.GetAxis("Mouse ScrollWheel");
          if (Mathf.Abs(scrollInput) > 0.01f) // 입력이 미미하지 않은 경우
          {
              currentZoom -= scrollInput * zoomSpeed;
              currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

              if (useFOVZoom)
              {
                  // FOV를 이용한 줌인/줌아웃
                  cam.fieldOfView = currentZoom;
              }
              else if (target != null)
              {
                  // 거리 기반 줌인/줌아웃
                  Vector3 direction = transform.position - target.position;
                  transform.position = target.position + direction.normalized * currentZoom;
              }
          }
        }
    }

    // GameScene에 활성화된 Scrollbar가 있는지 확인하는 함수
    public bool IsActiveScrollbarInScene()
    {
        Scrollbar[] scrollbars = FindObjectsOfType<Scrollbar>();
        foreach (Scrollbar scrollbar in scrollbars)
        {
            if (scrollbar.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }
}
