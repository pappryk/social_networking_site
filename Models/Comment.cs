using System;
using Npgsql;


public class Comment
{
    public Comment()
    {
    }

    public Comment(NpgsqlDataReader reader)
    {
        Id = reader.GetInt32(0);
        Text = (string)reader[1];
        Username = (string)reader[2];
        DatePublished = reader.GetDateTime(3);
        PostId = reader.GetInt32(4);
        LikesCounter = reader.GetInt32(5); 
    }


    public int Id {get; set;}
    public string Text {get; set;}
    public string Username {get; set;}
    public DateTime DatePublished{get; set;}
    public int PostId {get; set;}
    public int LikesCounter {get; set;}

}