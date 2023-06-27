using AutoMapper;
using BusinessLogic.Base;

using Common.Helpers;
using Common.Models;
using DataAccess.IRepository;
using static Common.Enumeration.Enumeration;

namespace BusinessLogic.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly IJwtHelper _jwtUltil;
        private readonly IAuthService _authService;
        private readonly ICache<Account> _cache;
        public AccountService(IAccountRepository accountRepository, IMapper mapper, ICache<Account> cache, IAuthService authService, IJwtHelper jwtUltil)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _cache = cache;
            _authService = authService;
            _jwtUltil = jwtUltil;
        }

        public ServiceResponse Login(LoginRequestDTO model)
        {
            ServiceResponse response = new ServiceResponse();
            var acc = _accountRepository.GetByEmail(model.Email);
            if (acc == null)
            {
                response.OnError(message: "Email Not Found");
                return response;
            }
            bool validPassword = BCrypt.Net.BCrypt.Verify(model.Password, acc.Password);
            if (!validPassword)
            {
                response.OnError(message: "Wrong Password");
                return response;
            }
            var userInfo = _mapper.Map<UserInfo>(acc);
            userInfo.Token = _jwtUltil.GenerateToken(userInfo);
            response.OnSuccess(userInfo);
            return response;
        }

        public ServiceResponse GetById(int accId)
        {
            ServiceResponse response = new ServiceResponse();
            var acc = _accountRepository.GetById(accId);
            if (acc == null)
            {
                response.OnError(message: "Account Not Found");
                return response;
            }
            response.OnSuccess(_mapper.Map<UserInfo>(acc));
            return response;
        }

        public ServiceResponse Register(RegisterRequestDTO account, int role = (int)Role.Student)
        {
            ServiceResponse response = new ServiceResponse();
            bool isExist = _accountRepository.CheckEmailOrCodeExist(account.Email, account.Code);
            if (isExist)
            {
                response.OnError(message: "Email or Code already exist");
                return response;
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(account.Password);
            Account newAccount = new Account()
            {
                Password = hashedPassword,
                Email = account.Email,
                Role = role,
                CreatedBy = account.Email,
                UpdatedBy = account.Email,
                CreatedDate = DateTime.Now,
            };
            if (role == (int)Role.Student)
            {
                Student studentInfo = new Student()
                {
                    Name = account.Fullname,
                    Code = account.Code,
                    Phone = account.Phone,
                    Address = account.Address,
                    CreatedBy = account.Email,
                    UpdatedBy = account.Email,
                    CreatedDate = DateTime.Now,
                };
                newAccount.Student = studentInfo;
            }
            else if (role == (int)Role.Teacher)
            {
                Teacher teacherInfo = new Teacher()
                {
                    Account = newAccount,
                    Name = account.Fullname,
                    Code = account.Code,
                    Phone = account.Phone,
                    Address = account.Address,
                    CreatedBy = account.Email,
                    UpdatedBy = account.Email,
                    CreatedDate = DateTime.Now,
                };
                newAccount.Teacher = teacherInfo;
            }
            var result = _accountRepository.Register(newAccount);
            if (result == null)
            {
                response.OnError(message: "Register Failed");
                return response;
            }
            var userInfo = _mapper.Map<UserInfo>(result);
            if (account.ToLogin)
            {
                userInfo.Token = _jwtUltil.GenerateToken(userInfo);
            }
            response.OnSuccess(userInfo);
            return response;
        }

        public ServiceResponse Update(int accId, AccountRequestDTO account)
        {
            ServiceResponse response = new ServiceResponse();
            bool isExist = _accountRepository.CheckEmailOrCodeExist(account.Email, account.Code, accId);
            if (isExist)
            {
                response.OnError(message: "Email or Code already exist");
                return response;
            }
            Account existingAccount = _accountRepository.GetById(accId);
            account.Id = accId;
            _mapper.Map(account, existingAccount);
            var updatedAccount = _accountRepository.Update(existingAccount);
            if (updatedAccount == null)
            {
                response.OnError(message: "Update Account Failed");
                return response;
            }
            response.OnSuccess(_mapper.Map<UserInfo>(updatedAccount));
            return response;
        }

        public ServiceResponse UpdatePassword(int accId, string oldPassword, string newPassword)
        {
            ServiceResponse response = new ServiceResponse();
            Account existingAccount = _accountRepository.GetById(accId);
            if (existingAccount == null)
            {
                response.OnError(message: "Account Not Found");
                return response;
            }
            bool validPassword = BCrypt.Net.BCrypt.Verify(oldPassword, existingAccount.Password);
            if (!validPassword)
            {
                response.OnError(message: "Wrong Old Password");
                return response;
            }
            existingAccount.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var updatedAccount = _accountRepository.UpdatePassword(existingAccount);
            if (updatedAccount == null)
            {
                response.OnError(message: "Change Password Failed");
                return response;
            }
            response.OnSuccess(message: "Change Password Success");
            return response;
        }

        public ServiceResponse UpdateRole(List<RoleUpdate> roles)
        {
            ServiceResponse response = new ServiceResponse();
            var updatedAccount = _accountRepository.UpdateRole(roles);
            if (!updatedAccount)
            {
                response.OnError(message: "Update Role Failed");
                return response;
            }
            response.OnSuccess(message: "Update Role Success");
            return response;
        }
    }
}
