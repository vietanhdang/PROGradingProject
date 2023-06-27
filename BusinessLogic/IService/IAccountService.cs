using Common.Models;
using static Common.Enumeration.Enumeration;

namespace BusinessLogic
{
    public interface IAccountService
    {
        public ServiceResponse Register(RegisterRequestDTO account, int role = (int)Role.Student);
        public ServiceResponse Update(int accId, AccountRequestDTO account);
        public ServiceResponse UpdateRole(List<RoleUpdate> roles);
        public ServiceResponse UpdatePassword(int accId, string oldPassword, string newPassword);
        public ServiceResponse GetById(int accId);
        public ServiceResponse Login(LoginRequestDTO model);
    }
}
