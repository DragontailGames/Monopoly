using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dragontailgames.Utils;

public class RankedMatchPanelController : MonoBehaviour
{
    public SelectSpecialSkinController selectSpecialSkinController;

    public UserManager user;

    public Image diceIcon;

    public int betValue;

    private int betIndex;

    public TextMeshProUGUI txtBet;

    public LobbyManager lobbyManager;

    public MatchActions actions;

    public void Start()
    {
        betValue = MathDt.betValue[betIndex];
    }

    private void Update()
    {
        diceIcon.sprite = user.CurrentDice.icon;
        txtBet.text = MathDt.ConfigureBet(betValue);
    }

    public void NextBet()
    {
        betIndex = ((betIndex + 1) < MathDt.betValue.Length) ? betIndex+1 : 0;
        betValue = MathDt.betValue[betIndex];
    }

    public void PrevBet()
    {
        betIndex = ((betIndex - 1) > 0)  ? betIndex-1:MathDt.betValue.Length-1;
        betValue = MathDt.betValue[betIndex];
    }

    public GameObject blockPanel;

    public void FindGame(Button currentButton)
    {
        blockPanel.SetActive(true);

        currentButton.onClick.RemoveAllListeners();
        currentButton.onClick.AddListener(() => { lobbyManager.btn_LeftRoom(); });

        Color mainColor = new Color32(255, 172, 11,255);

        currentButton.image.color = Color.cyan;
        currentButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connecting...";
        currentButton.GetComponentInChildren<TranslationText>().SetText();

        actions = new MatchActions()
        {
            onJoinedRoom = () =>
            {
                currentButton.image.color = new Color32(11, 169, 255, 255);
                currentButton.GetComponentInChildren<TextMeshProUGUI>().text = "Finding Rrivals...";
                currentButton.GetComponentInChildren<TranslationText>().SetText();
            },
            onLeftRoom = () =>
            {
                currentButton.image.color = mainColor;
                currentButton.GetComponentInChildren<TextMeshProUGUI>().text = "Find Game";
                currentButton.GetComponentInChildren<TranslationText>().SetText();
                currentButton.onClick.RemoveAllListeners();
                currentButton.onClick.AddListener(() => { FindGame(currentButton); });

                blockPanel.SetActive(false);
            }
        };
        lobbyManager.btn_ConnectToLobbyRanked(actions, betValue);
    }
}
