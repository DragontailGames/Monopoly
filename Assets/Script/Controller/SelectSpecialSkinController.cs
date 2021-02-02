using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSpecialSkinController : MonoBehaviour
{
    public GameObject diceDisplaySkin;

    public Transform content;

    private Dice selectedDice;

    private GameObject selectedDiceObject;

    public UserManager user;

    public void ShowDiceSkins()
    {
        this.gameObject.SetActive(true);
        DisplayDice(user.dices, user.CurrentDice);
    }

    public void DisplayDice(List<Dice> dices, Dice startDice)
    {
        int childs = content.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        foreach (var aux in dices)
        {
            var diceObject = Instantiate(diceDisplaySkin, content);
            diceObject.GetComponent<Button>().onClick.AddListener(() => { SelectDice(aux, diceObject); });
            diceObject.transform.GetChild(0).GetComponent<Image>().sprite = aux.icon;

            if(startDice.id == aux.id)
            {
                diceObject.transform.GetChild(1).gameObject.SetActive(true);
                selectedDiceObject = diceObject;
            }
        }
    }

    public void SelectDice(Dice dice, GameObject diceObject)
    { 
        selectedDiceObject.transform.GetChild(1).gameObject.SetActive(false);

        selectedDiceObject = diceObject;
        selectedDice = dice;

        user.CurrentDice = dice;
    }
}
