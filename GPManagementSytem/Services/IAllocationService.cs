using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public interface IAllocationService
    {
        List<Allocations> GetAll();
        Allocations GetByPracticeAndYear(int PracticeId, string year);
        List<Allocations> GetByAcademicYear(string year);
        List<Allocations> GetByPractice(int PracticeId);
        Allocations AddAllocation(Allocations allocations);
        Allocations EditAllocation(Allocations allocations);
    }
}