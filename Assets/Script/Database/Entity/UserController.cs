using UnityEngine;
using Dragontailgames.Utils;
using MongoDB.Bson;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UserController : MonoBehaviour
{
    public DatabaseAccess<User> dbUser;

    public User user;

    public void Start()
    {
        dbUser = new DatabaseAccess<User>("mongodb+srv://dragontailgames:Fepe0715@cluster0.wk5ax.mongodb.net/Monopoly?retryWrites=true&w=majority", "Monopoly", "User");
    }

    public async Task<string> SaveUser(User user)
    {
        try
        {
            var cursor = await dbUser.GetAsyncDocumentFromDatabase("email", user.email);
            while (await cursor.MoveNextAsync())
            {
                var userNew = ((List<User>)cursor.Current)[0];
                if (userNew != null)
                {
                    return "1 - Email ja cadastrado";
                }
            }

            await dbUser.SaveDocumentToDatabaseAsync(user);
        }
        catch(Exception e)
        {
            Debug.LogError("Erro ao cadastrar " + e);
            return "2 - Erro desconhecido tente novamente mais tarde";
        }

        return "0 - Deu tudo certo";
    }

    public async Task<bool> Login(string email, string password)
    {
        var cursor = await dbUser.GetAsyncDocumentFromDatabase("email", email);
        while (await cursor.MoveNextAsync())
        {
            user = ((List<User>)cursor.Current)[0];
        }

        if (user != null)
        {
            if (PasswordHashDt.Verify(password, user.password))
            {
                return true;
            }
        }
        return false;
    }
}
