using System;
using Npgsql;



public class Group
{
    public Group()
    {
        
    }

    public Group(NpgsqlDataReader reader)
    {
        Id = (int)reader[0];
        Name = (string)reader[1];
    }


    public int Id {get; set;}
    public string Name {get; set;}
}