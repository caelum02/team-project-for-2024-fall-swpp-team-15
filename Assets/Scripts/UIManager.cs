using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI reputationText;
    public Image reputationGauge;

    // Start is called before the first frame update
    void Start()
    {
        updateMoneyUI();
        updateReputationUI();
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
        float fillAmount = gameManager.reputationValue / 100f;
        reputationGauge.fillAmount = fillAmount;
        Debug.Log(gameManager.reputationValue);
    }
}
