using Common.Models;

namespace DataAccess.IRepository
{
    public interface IAccountRepository
    {
        public Account GetById(int accId);
        public Account GetByEmail(string username);
        public bool CheckEmailOrCodeExist(string email, string code, int accId = 0);
        public Account Register(Account account);
        public Account Update(Account account);
        public bool UpdatePassword(Account account);
        public bool UpdateRole(List<RoleUpdate> roles);

        public bool DeleteUser(int accId);
    }
}
