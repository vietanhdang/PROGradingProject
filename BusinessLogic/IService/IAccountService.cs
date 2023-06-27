using Common.Models;
using static Common.Enumeration.Enumeration;

namespace BusinessLogic
{
    public interface IAccountService
    {
        public ServiceResponse Register(RegisterRequest account, int role = (int)Role.Student);
        public ServiceResponse Update(int accId, AccountRequest account);
        public ServiceResponse UpdateRole(List<RoleUpdate> roles);
        public ServiceResponse UpdatePassword(int accId, string oldPassword, string newPassword);
        public ServiceResponse GetById(int accId);
        public ServiceResponse Login(LoginRequest model);
    }
}
