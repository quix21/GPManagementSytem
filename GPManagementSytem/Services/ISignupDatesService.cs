using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public interface ISignupDatesService
    {
        List<SignupDates> GetAll();
        SignupDates AddSignupDate(SignupDates signupDates);
        SignupDates EditSignupDate(SignupDates signupDates);
        SignupDates GetByAcademicYear(string year);
    }
}