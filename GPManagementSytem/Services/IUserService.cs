using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public interface IUserService
    {
        List<Users> GetAll();
        Users GetById(int id);
        Users GetByUsername(string userName);
        Users LoginUser(string uname, string pwd, bool isImpersonate = false);
        Users LoginUserPractice(string uname, string pwd, bool isImpersonate = false);
        Users AddUser(Users user);
        Users EditUser(Users user);
    }
}