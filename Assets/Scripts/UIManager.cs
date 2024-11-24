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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void updateMoneyUI()
    {
        moneyText.text = gameManager.money.ToString();
    }

    private void updateReputationUI()
    {
        reputationText.text = "Level " + gameManager.reputation.ToString();
    }
}
