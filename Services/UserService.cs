using System;
using Npgsql;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;


public class UserService : IUserService
{
    public string ConnectionString {get; set;}
    private NpgsqlConnection con;
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

        con = new NpgsqlConnection(ConnectionString);
            NpgsqlCommand cmd = new NpgsqlCommand(query);
            cmd.Connection = con;
            con.Open();
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
            con.Close();
        }

        return users;
    }

}