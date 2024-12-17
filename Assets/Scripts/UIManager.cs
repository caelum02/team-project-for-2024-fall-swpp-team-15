using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yogaewonsil.Common;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI reputationText;
    public Image reputationGauge;
    public GameObject levelUpScreen;
    public GameObject endingScreen;
    public GameObject michelinStar;
    public AudioClip levelUpSound;
    public AudioClip endingSound;
    private AudioSource audioSource;
    public GameObject GourmetNPCIcon;
    public GameObject BadguyNPCIcon;
    public GameObject MichelinNPCIcon;

    // Start is called before the first frame update
    void Start()
    {
        updateMoneyUI();
        updateReputationUI();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateMoneyUI()
    {
        moneyText.text = gameManager.money.ToString();
    }

    public void updateReputationUI()
    {
        reputationText.text = "Lv " + gameManager.reputation.ToString();
        Debug.Log(gameManager.reputationValue);
        float fillAmount = gameManager.reputationForLevelUp != 0 ? ((float)gameManager.reputationValue / gameManager.reputationForLevelUp) : (gameManager.reputationValue / 100f);
        Debug.Log($"fillAmount: {fillAmount}");
        reputationGauge.fillAmount = fillAmount;
    }

    /// <summary>
    /// 미슐랭 스타를 받았을 때의 화면
    /// </summary>
    public void GetMichelinStar()
    {
        endingScreen.SetActive(true);
        michelinStar.SetActive(true);
        PlayEndingSound();
    }
    /// <summary>
    /// 레벨 업 화면 열기 
    /// </summary>
    public void ShowLevelUpScreen()
    {
        levelUpScreen.SetActive(true);
        PlayLevelUpSound();
    }

    /// <summary>
    /// 레벨 업 화면 닫기 
    /// </summary>
    public void CloseLevelUpScreen()
    {
        levelUpScreen.SetActive(false);
    }

    /// <summary>
    /// 엔딩 화면 닫기
    /// </summary>
    public void CloseEndingScreen()
    {
        endingScreen.SetActive(false);
    }

    private void PlayLevelUpSound()
    {
        audioSource.PlayOneShot(levelUpSound);
    }

    private void PlayEndingSound()
    {
        audioSource.PlayOneShot(endingSound);
    }

    public void UpdateNPCIcon(CustomerType customerType, bool isActivate)
    {
        switch(customerType)
        {   
            case CustomerType.일반손님:
                return;
            case CustomerType.음식평론가:
                GourmetNPCIcon.SetActive(isActivate);
                return;
            case CustomerType.진상손님:
                BadguyNPCIcon.SetActive(isActivate);
                return;
            case CustomerType.미슐랭가이드:
                MichelinNPCIcon.SetActive(isActivate);
                return;
            default:
                return;
        }
    }
}
