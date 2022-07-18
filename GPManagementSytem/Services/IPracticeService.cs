using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public interface IPracticeService
    {
        List<Practices> GetAll();
        Practices GetById(int id);
        Practices AddPractice(Practices practice);
        Practices EditPractice(Practices practice);
    }
}