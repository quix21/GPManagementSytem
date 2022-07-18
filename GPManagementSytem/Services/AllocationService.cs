using GPManagementSytem.Database;
using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public class AllocationService: IAllocationService
    {
        private readonly IDatabaseEntities _databaseEntities;

        public AllocationService(IDatabaseEntities databaseEntities)
        {
            _databaseEntities = databaseEntities;
        }

        public IQueryable<Allocations> AllNoTracking()
        {
            return _databaseEntities.Allocations.AsNoTracking();
        }

        public List<Allocations> GetAll()
        {
            return AllNoTracking().ToList();
        }

        public Allocations GetById(int id)
        {
            return AllNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        public Allocations GetByPracticeAndYear(int PracticeId, string year)
        {
            return AllNoTracking().Where(x => x.PracticeId == PracticeId && x.AcademicYear == year).FirstOrDefault();
        }

        public List<Allocations> GetByAcademicYear(string year)
        {
            return AllNoTracking().Where(x => x.AcademicYear == year).ToList();
        }

        public List<Allocations> GetByPractice(int PracticeId)
        {
            return AllNoTracking().Where(x => x.PracticeId == PracticeId).ToList();
        }

        public Allocations AddAllocation(Allocations allocations)
        {
            return UpdateAllocation(allocations);
        }

        public Allocations EditAllocation(Allocations allocations)
        {
            return UpdateAllocation(allocations);
        }

        private Allocations UpdateAllocation(Allocations allocations)
        {
            var existingEntity = _databaseEntities.Allocations.FirstOrDefault(x => x.Id == allocations.Id);

            Allocations entityToUpdate;

            if (existingEntity != null)
            {
                entityToUpdate = existingEntity;
            }
            else
            {
                entityToUpdate = allocations;
            }

            entityToUpdate.PracticeId = allocations.PracticeId;
            entityToUpdate.Year2Wk1Requested = allocations.Year2Wk1Requested;
            entityToUpdate.Year2Wk1Allocated = allocations.Year2Wk1Allocated;
            entityToUpdate.Year2Wk2Requested = allocations.Year2Wk2Requested;
            entityToUpdate.Year2Wk2Allocated = allocations.Year2Wk2Allocated;
            entityToUpdate.Year2Wk3Requested = allocations.Year2Wk3Requested;
            entityToUpdate.Year2Wk3Allocated = allocations.Year2Wk3Allocated;
            entityToUpdate.Year2Wk4Requested = allocations.Year2Wk4Requested;
            entityToUpdate.Year2Wk4Allocated = allocations.Year2Wk4Allocated;
            entityToUpdate.Year2Wk5Requested = allocations.Year2Wk5Requested;
            entityToUpdate.Year2Wk5Allocated = allocations.Year2Wk5Allocated;
            entityToUpdate.Year2Wk6Requested = allocations.Year2Wk6Requested;
            entityToUpdate.Year2Wk6Allocated = allocations.Year2Wk6Allocated;
            entityToUpdate.Year3B1Requested = allocations.Year3B1Requested;
            entityToUpdate.Year3B1Allocated = allocations.Year3B1Allocated;
            entityToUpdate.Year3B2Requested = allocations.Year3B2Requested;
            entityToUpdate.Year3B2Allocated = allocations.Year3B2Allocated;
            entityToUpdate.Year3B3Requested = allocations.Year3B3Requested;
            entityToUpdate.Year3B3Allocated = allocations.Year3B3Allocated;
            entityToUpdate.Year3B4Requested = allocations.Year3B4Requested;
            entityToUpdate.Year3B4Allocated = allocations.Year3B4Allocated;
            entityToUpdate.Year3B5Requested = allocations.Year3B5Requested;
            entityToUpdate.Year3B5Allocated = allocations.Year3B5Allocated;
            entityToUpdate.Year3B6Requested = allocations.Year3B6Requested;
            entityToUpdate.Year3B6Allocated = allocations.Year3B6Allocated;
            entityToUpdate.Year3B7Requested = allocations.Year3B7Requested;
            entityToUpdate.Year3B7Allocated = allocations.Year3B7Allocated;
            entityToUpdate.Year4B1Requested = allocations.Year4B1Requested;
            entityToUpdate.Year4B1Allocated = allocations.Year4B1Allocated;
            entityToUpdate.Year4B2Requested = allocations.Year4B2Requested;
            entityToUpdate.Year4B2Allocated = allocations.Year4B2Allocated;
            entityToUpdate.Year4B3Requested = allocations.Year4B3Requested;
            entityToUpdate.Year4B3Allocated = allocations.Year4B3Allocated;
            entityToUpdate.Year4B4Requested = allocations.Year4B4Requested;
            entityToUpdate.Year4B4Allocated = allocations.Year4B4Allocated;
            entityToUpdate.Year4B5Requested = allocations.Year4B5Requested;
            entityToUpdate.Year4B5Allocated = allocations.Year4B5Allocated;
            entityToUpdate.Year4B6Requested = allocations.Year4B6Requested;
            entityToUpdate.Year4B6Allocated = allocations.Year4B6Allocated;
            entityToUpdate.Year4B7Requested = allocations.Year4B7Requested;
            entityToUpdate.Year4B7Allocated = allocations.Year4B7Allocated;
            entityToUpdate.Year4B8Requested = allocations.Year4B8Requested;
            entityToUpdate.Year4B8Allocated = allocations.Year4B8Allocated;
            entityToUpdate.Year5B1Requested = allocations.Year5B1Requested;
            entityToUpdate.Year5B1Allocated = allocations.Year5B2Allocated;
            entityToUpdate.Year5B2Requested = allocations.Year5B2Requested;
            entityToUpdate.Year5B2Allocated = allocations.Year5B2Allocated;
            entityToUpdate.Year5B3Requested = allocations.Year5B3Requested;
            entityToUpdate.Year5B3Allocated = allocations.Year5B3Allocated;
            entityToUpdate.Year5B4Requested = allocations.Year5B4Requested;
            entityToUpdate.Year5B4Allocated = allocations.Year5B4Allocated;
            entityToUpdate.Year5B5Requested = allocations.Year5B5Requested;
            entityToUpdate.Year5B5Allocated = allocations.Year5B5Allocated;
            entityToUpdate.Year5B6Requested = allocations.Year5B6Requested;
            entityToUpdate.Year5B6Allocated = allocations.Year5B6Allocated;
            entityToUpdate.AcademicYear = allocations.AcademicYear;
            entityToUpdate.ServiceContractReceived = allocations.ServiceContractReceived;
            entityToUpdate.DateCreated = allocations.DateCreated;
            entityToUpdate.DateUpdated = allocations.DateUpdated;
            entityToUpdate.UpdatedBy = allocations.UpdatedBy;

            if (existingEntity == null)
            {
                _databaseEntities.Allocations.Add(entityToUpdate);
            }

            Save();

            return entityToUpdate;

        }

        private bool Save()
        {
            return _databaseEntities.SaveChanges() > 0;
        }
    }
}