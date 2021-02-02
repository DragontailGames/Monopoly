using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dragontailgames.Utils;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UserManager : MonoBehaviour
{
    public string usernamePlayerPrefs = "username", coinsPlayerPrefs = "coins", diamondsPlayerPrefs = "diamonds", productsPlayerPrefs = "products";

    public GameObject usernamePanel;

    public TMP_InputField usernameInput;

    public TextMeshProUGUI txtProblem, txtUsername, txtCoins, txtDiamonds;

    private int coins, diamonds;

    public List<Dice> dices = new List<Dice>();

    private Dice currentDice;

    public string nickname;

    public Dice CurrentDice { get => this.currentDice; set 
        { 
            this.currentDice = value;
            PlayerPrefs.SetString("currentDice", value.id);
        }
    }

    private void Awake()
    {
        LoadAllProducts();
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(usernamePlayerPrefs))
        {
            usernamePanel.SetActive(true);
            usernameInput.text = "RichUncle_" + Random.Range(100, 999);
        }
        else
        {
            usernamePanel.SetActive(false);
            txtUsername.text = PlayerPrefs.GetString(usernamePlayerPrefs);

            nickname = PlayerPrefs.GetString(usernamePlayerPrefs);

            coins = PlayerPrefs.GetInt(coinsPlayerPrefs);
            txtCoins.text = MathDt.ConfigureCoins(coins);

            diamonds = PlayerPrefs.GetInt(diamondsPlayerPrefs);
            txtDiamonds.text = MathDt.ConfigureCoins(diamonds);

            currentDice = ((Resources.FindObjectsOfTypeAll(typeof(Dice)) as Dice[]).ToList()).Find(n=>n.id == PlayerPrefs.GetString("currentDice"));

        }
    }

    public void Btn_SetupUsername()
    {
        if (usernameInput.text.Trim() == "")
        {
            txtProblem.enabled = true;
            txtProblem.text = "Username not be empty";
            return;
        }
        txtProblem.enabled = false;

        PlayerPrefs.SetString(usernamePlayerPrefs, usernameInput.text.Trim());
        nickname = PlayerPrefs.GetString(usernameInput.text.Trim());

        PlayerPrefs.SetInt(coinsPlayerPrefs, 100000);
        PlayerPrefs.SetInt(diamondsPlayerPrefs, 500);
        usernamePanel.SetActive(false);
        SceneLoadManager.instance.Reload();
    }

    public bool UpdateCoins(int updateCoins)
    {
        if (this.coins + updateCoins >= 0)
        {
            this.coins += updateCoins;
            txtCoins.text = MathDt.ConfigureCoins(coins);
            PlayerPrefs.SetInt(coinsPlayerPrefs, coins);
        }
        else
        {
            return false;
        }

        return true;
    }

    public bool BuyDice(Dice dice)
    {
        if(!currentDice)
        {
            CurrentDice = dice;
        }
        if (this.diamonds - dice.diamondCost >= 0)
        {
            diamonds -= dice.diamondCost;
            dices.Add(dice);
            string saveProduct = "";
            foreach (var aux in dices)
            {
                saveProduct += aux.id + "|";
            }
            txtDiamonds.text = MathDt.ConfigureCoins(diamonds);
            PlayerPrefs.SetInt(diamondsPlayerPrefs, diamonds);
            PlayerPrefs.SetString(productsPlayerPrefs, saveProduct.Remove(saveProduct.Length - 1));
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LoadAllProducts()
    {
        var loadedDices = Resources.FindObjectsOfTypeAll<Dice>().OfType<Dice>().ToList();
        var buyed = PlayerPrefs.GetString(productsPlayerPrefs).Split('|');
        foreach(var aux in buyed)
        {
            var product = loadedDices.Find(n => n.id == aux);

            if (product != null)
            {
                dices.Add(product);
            }
        }
    }
}
