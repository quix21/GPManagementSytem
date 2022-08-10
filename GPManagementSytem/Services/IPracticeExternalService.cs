using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public interface IPracticeExternalService
    {
        List<PracticesExternal> GetAllApproved();
        List<PracticesExternal> GetAllPending();
        PracticesExternal GetById(int id);
        PracticesExternal AddPractice(PracticesExternal practice);
        PracticesExternal EditPractice(PracticesExternal practice);
    }
}