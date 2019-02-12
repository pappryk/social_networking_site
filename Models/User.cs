using System;
using Npgsql;


public class User
{
    public User(NpgsqlDataReader reader)
    {
        Username = (string)reader[0];
        FirstName = (string)reader[1];
        LastName = (string)reader[2];
        DateJoined = reader.GetDateTime(3);
        Password = (string)reader[4];
    }

    public User(){}

    public string Username {get; set;}
    public string FirstName {get; set;}
    public string Email {get; set;}
    public string LastName {get; set;}
    public DateTime DateJoined {get; set;}
    public string Password {get; set;}
}