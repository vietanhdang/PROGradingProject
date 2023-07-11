using Common.Models;
using static Common.Enumeration.Enumeration;

namespace BusinessLogic
{
    public interface IAccountService
    {
        public ServiceResponse Register(RegisterRequestDTO account, int role = (int)Role.Student);
        public ServiceResponse Update(AccountRequestDTO account);
        public ServiceResponse UpdateRole(List<RoleUpdate> roles);
        public ServiceResponse UpdatePassword(string oldPassword, string newPassword);
        public ServiceResponse GetById();
        public ServiceResponse GetUserInfor();
        public ServiceResponse DeleteUser();
        public ServiceResponse Login(LoginRequestDTO model);
    }
}
