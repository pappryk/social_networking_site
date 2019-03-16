using System;
using Npgsql;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public interface IUserService
{
    string ConnectionString {get; set;}
    List<User> GetAll();
    User GetUser(string username);
    bool IsUserRegistered(string username);
    void RegisterUser(User user);
}