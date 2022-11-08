using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public interface IQualityVisitService
    {
        List<QualityVisit> GetAll();
        List<QualityVisit> GetAllByPractice(int practiceId);
        List<QualityVisit> GetAllByVisitType(int visitTypeId);
        QualityVisit GetById(int id);
        QualityVisit AddQualityVisit(QualityVisit qualityVisit);
        QualityVisit EditQualityVisit(QualityVisit qualityVisit);



    }
}