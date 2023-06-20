using Common.Enumeration;
using Common.Models;
using DataAccess.DatabaseContext;
using DataAccess.IRepository;
using Microsoft.EntityFrameworkCore;
using static Common.Enumeration.Enumeration;

namespace DataAccess.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _appDbContext;
        public AccountRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public bool CheckEmailOrCodeExist(string email, string code, int accId = 0)
        {
            email = email.Trim().ToLower();
            code = code.Trim().ToLower();
            if (accId > 0)
            {
                return _appDbContext.Accounts.Any(x => (x.Email == email || x.Student.Code == code || x.Teacher.Code == code) && x.AccountId != accId);
            }
            return _appDbContext.Accounts.Any(x => x.Email == email || x.Student.Code == code || x.Teacher.Code == code);
        }

        public Account GetByEmail(string email)
        {
            return _appDbContext.Accounts.Include(x => x.Student).Include(x => x.Teacher).FirstOrDefault(x => x.Email == email);
        }

        public Account GetById(int accId)
        {
            return _appDbContext.Accounts.Include(x => x.Student).Include(x => x.Teacher).FirstOrDefault(x => x.AccountId == accId);
        }

        public Account Register(Account account)
        {
            _appDbContext.Database.BeginTransaction();
            try
            {
                _appDbContext.Accounts.Add(account);
                _appDbContext.SaveChanges();
                _appDbContext.Database.CommitTransaction();
                return account;
            }
            catch (Exception ex)
            {
                _appDbContext.Database.RollbackTransaction();
                throw ex;
            }
        }

        public Account Update(Account account)
        {
            _appDbContext.Database.BeginTransaction();
            try
            {
                _appDbContext.Accounts.Update(account);
                _appDbContext.SaveChanges();
                _appDbContext.Database.CommitTransaction();
                return account;
            }
            catch (Exception ex)
            {
                _appDbContext.Database.RollbackTransaction();
                throw ex;
            }
        }


        public bool UpdateRole(List<RoleUpdate> roles)
        {
            _appDbContext.Database.BeginTransaction();
            try
            {
                if (roles.Count > 0)
                {
                    foreach (var item in roles)
                    {
                        var acc = new Account { AccountId = item.AccountId, Role = (int)item.role };
                        _appDbContext.Accounts.Attach(acc);
                        _appDbContext.Entry(acc).Property(x => x.Role).IsModified = true;
                    }
                    _appDbContext.SaveChanges();
                }
                _appDbContext.Database.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                _appDbContext.Database.RollbackTransaction();
                throw ex;
            }
        }

        public bool UpdatePassword(Account account)
        {
            _appDbContext.Database.BeginTransaction();
            try
            {
                _appDbContext.Accounts.Attach(account);
                _appDbContext.Entry(account).Property(x => x.Password).IsModified = true;
                _appDbContext.SaveChanges();
                _appDbContext.Database.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                _appDbContext.Database.RollbackTransaction();
                throw ex;
            }
        }
    }
}
