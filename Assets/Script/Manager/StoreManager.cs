using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoreManager : MonoBehaviour
{
    public GameObject diceObject;

    public Transform storeDiceContent;

    public string diceStorePath = "Dice";

    public GameObject dice;

    public GameObject store;

    public UserManager user;

    public List<Dice> dices;

    public void Start()
    {
        LoadStore();
    }

    public void LoadStore()
    {
        var dices = Resources.LoadAll("Store/" + diceStorePath);
        foreach(var aux in dices)
        {
            var auxDice = (Dice)aux;

            this.dices.Add(auxDice);

            if (auxDice.diamondCost > 0)
            {

                GameObject dice = Instantiate(diceObject, storeDiceContent);

                dice.transform.Find("DiceName").GetComponent<TextMeshProUGUI>().text = auxDice.productName;
                dice.transform.Find("DiceMoney").GetComponent<TextMeshProUGUI>().text = MathDt.ConfigureCoins(auxDice.diamondCost);
                dice.transform.Find("DiceIcon").GetComponent<Image>().sprite = auxDice.icon;

                Button buyButton = dice.transform.Find("DiceBuy").GetComponent<Button>();
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() => ShowDiceStore(auxDice, buyButton));
                buyButton.interactable = !user.dices.Contains(auxDice);
            }
            else
            {
                if(!user.dices.Contains(auxDice))
                {
                    user.BuyDice(auxDice);
                }
            }
        }
    }

    public void ShowDiceStore(Dice dice, Button buyButton)
    {
        store.SetActive(true);

        store.transform.GetChild(0).Find("DiceName").GetComponent<TextMeshProUGUI>().text = dice.productName;
        store.transform.GetChild(0).Find("DiceMoney").GetComponent<TextMeshProUGUI>().text = MathDt.ConfigureCoins(dice.diamondCost);
        store.transform.GetChild(0).Find("DiceBuy").GetComponent<Button>().onClick.RemoveAllListeners();
        store.transform.GetChild(0).Find("DiceBuy").GetComponent<Button>().onClick.AddListener(() => {
            user.BuyDice(dice);
            buyButton.interactable = false;
        }) ;
        this.dice.GetComponent<MeshRenderer>().material = dice.material;
    }
}
