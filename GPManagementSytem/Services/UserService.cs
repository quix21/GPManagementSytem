using GPManagementSytem.Database;
using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.DirectoryServices;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public class UserService: IUserService
    {
        private readonly IDatabaseEntities _databaseEntities;

        public UserService(IDatabaseEntities databaseEntities)
        {
            _databaseEntities = databaseEntities;
        }

        public IQueryable<Users> AllNoTracking()
        {
            return _databaseEntities.Users.AsNoTracking();
        }

        public List<Users> GetAll()
        {
            return AllNoTracking().ToList();
        }

        public Users GetById(int id)
        {
            return AllNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        public Users GetByUsername(string userName)
        {
            return AllNoTracking().Where(x => x.Username.ToLower() == userName.ToLower()).FirstOrDefault();
        }

        public Users GetByUsernamePracticeOnly(string userName)
        {
            return AllNoTracking().Where(x => x.Username.ToLower() == userName.ToLower() && x.UserType == (int)UserTypes.Practice).FirstOrDefault();
        }

        public List<Users> GetAllPracticeUsers()
        {
            return AllNoTracking().Where(x => x.UserType == (int)UserTypes.Practice && x.IsActive == true).OrderBy(x => x.Email).ToList();
        }

        public Users LoginUser(string uname, string pwd, bool isImpersonate = false)
        {
            Users myUser = null;

            myUser = AllNoTracking().Where(x => x.Username == uname && x.IsActive == true).FirstOrDefault();

            if (myUser != null)
            {
                if (!isImpersonate)
                {
                    if (AuthenticateMWS(uname, pwd))
                    {
                        return myUser;
                    }
                    else
                    {
                        return null;
                    }
                }

            }

            return myUser;
        }

        public Users LoginUserPractice(string uname, string pwd, bool isImpersonate = false)
        {
            Users myUser = null;

            myUser = AllNoTracking().Where(x => x.Username == uname && x.IsActive == true).FirstOrDefault();

            if (myUser != null)
            {
                if (!isImpersonate)
                {
                    if (myUser.Pwd == pwd)
                    {
                        return myUser;
                    }
                    else
                    {
                        return null;
                    }
                }

            }

            return myUser;
        }

        private bool AuthenticateMWS(string username, string password)
        {
            //authenticate against MWS
            bool authentic = false;
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://livad.liv.ac.uk:389/OU=UOL,DC=livad,DC=liv,DC=ac,DC=uk", username, password);
                object nativeObject = entry.NativeObject;

                authentic = true;

            }
            catch (DirectoryServicesCOMException e)
            {
                Console.Write(e);
            }

            return authentic;
        }

        public Users AddUser(Users user)
        {
            return UpdateUser(user);
        }

        public Users EditUser(Users user)
        {
            return UpdateUser(user);
        }

        private Users UpdateUser(Users user)
        {
            var existingEntity = _databaseEntities.Users.FirstOrDefault(x => x.Id == user.Id);

            Users entityToUpdate;

            if (existingEntity != null)
            {
                entityToUpdate = existingEntity;
            }
            else
            {
                entityToUpdate = user;
            }

            entityToUpdate.Firstname = user.Firstname;
            entityToUpdate.Surname = user.Surname;
            entityToUpdate.Username = user.Username;
            entityToUpdate.Email = user.Email;
            entityToUpdate.Pwd = user.Pwd;
            entityToUpdate.UserType = user.UserType;
            entityToUpdate.Year2 = user.Year2;
            entityToUpdate.Year3 = user.Year3;
            entityToUpdate.Year4 = user.Year4;
            entityToUpdate.Year5 = user.Year5;
            entityToUpdate.PracticeId = user.PracticeId;
            entityToUpdate.DateCreated = user.DateCreated;
            entityToUpdate.DateUpdated = user.DateUpdated;
            entityToUpdate.UpdatedBy = user.UpdatedBy;

            if (existingEntity == null)
            {
                _databaseEntities.Users.Add(entityToUpdate);
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