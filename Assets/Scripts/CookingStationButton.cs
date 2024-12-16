using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CookingStationButton : MonoBehaviour
{
    public AudioClip clickSound; // 버튼 클릭 사운드
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        audioSource = FindObjectOfType<AudioSource>();
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            Debug.Log("Hello");
        }
    }
}
