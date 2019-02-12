using System;
using Npgsql;


public class LikePost
{
    public LikePost()
    {
    }

    public LikePost(NpgsqlDataReader reader)
    {
    
    }


    public int Id {get; set;}
    public string Username {get; set;}
}