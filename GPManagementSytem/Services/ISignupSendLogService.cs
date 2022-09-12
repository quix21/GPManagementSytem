using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public interface ISignupSendLogService
    {
        List<Signupsendlog> GetAll();
        List<Signupsendlog> GetAllNoActivity(string academicYear);
        List<Signupsendlog> GetAllByPratice(int practiceId);
        List<Signupsendlog> GetBySendCode(string sendCode);
        Signupsendlog AddSignupSendLog(Signupsendlog signupsendlog);
        Signupsendlog EditSignupSendLog(Signupsendlog signupsendlog);
    }
}