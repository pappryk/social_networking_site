using System;
using Npgsql;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Identity;



public class UserService : IUserService
{
    public string ConnectionString {get; set;}
    private NpgsqlConnection _Con;
    private void ExecuteNonQuery(string query)
    {
        using (NpgsqlConnection con = new NpgsqlConnection(ConnectionString))
        {
            NpgsqlCommand cmd = new NpgsqlCommand(query);
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }


    private NpgsqlDataReader ExecuteReader(string query)
    {
        NpgsqlDataReader reader;

        _Con = new NpgsqlConnection(ConnectionString);
        NpgsqlCommand cmd = new NpgsqlCommand(query);
        cmd.Connection = _Con;
        _Con.Open();
        reader = cmd.ExecuteReader();
            

        return reader;
    }



    public List<User> GetAll()
    {
        List<User> users = new List<User>();        
        try
        {
            string queryString = "SELECT * FROM uzytkownicy;";
            NpgsqlDataReader reader = ExecuteReader(queryString);

            while (reader.Read())
            {
                users.Add(new User(reader));
            }

        }
        catch(Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
        finally
        {
            _Con.Close();
        }

        return users;
    }


    public User GetUser(string username)
    {
        User user = null;
        try
        {
            string queryString = $"SELECT * FROM uzytkownicy WHERE nazwa_uzytkownika = '{username}';";
            NpgsqlDataReader reader = ExecuteReader(queryString);
            reader.Read();
            if (reader.HasRows)
                user = new User(reader);
        }
        catch(Exception)
        {

        }
        finally
        {
            _Con.Close();
        }

        return user;
    }


    public bool IsUserRegistered(string username)
    {
        User user = GetUser(username);
        bool canRegister = false;

        if (user == null)
            canRegister = true;

        return canRegister;
    }


    public void RegisterUser(User user)
    {
        PasswordHasher<User> ph = new PasswordHasher<User>();
        string queryString = $"INSERT INTO uzytkownicy(nazwa_uzytkownika, imie, nazwisko, haslo_hash) VALUES ('{user.Username}', '{user.FirstName}', '{user.LastName}', '{ph.HashPassword(user, user.Password)}');";
        ExecuteNonQuery(queryString);
    }

}