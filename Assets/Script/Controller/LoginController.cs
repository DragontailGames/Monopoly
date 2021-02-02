using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginController : MonoBehaviour
{
    public Button hiddenPasswordBtn;

    public Sprite hiddenPasswordIcon, showPasswordIcon;

    public bool passwordHidden = true;

    public TMP_InputField emailInput, passwordInput, usernameInput;

    public Button signUp, signIn;

    public void EndEditInput()
    {
    }

    public void ShowPassword()
    {
        if(passwordHidden)
        {
            passwordHidden = false;
            hiddenPasswordBtn.image.sprite = showPasswordIcon;
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
            passwordInput.ForceLabelUpdate();
        }
        else
        {
            passwordHidden = true;
            hiddenPasswordBtn.image.sprite = hiddenPasswordIcon;
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }
    }

    public async void SignIn()
    {
        //await UserManager.instance.Login(emailInput.text, passwordInput.text);
    }

    public async void SignUp()
    {
/*        User user = new User()
        {
            email = emailInput.text,
            password = PasswordHashDt.Hash(passwordInput.text),
            username = usernameInput.text,
            coins = 10000
        };
        await UserManager.instance.CreateNewUser(user);*/
    }
}
