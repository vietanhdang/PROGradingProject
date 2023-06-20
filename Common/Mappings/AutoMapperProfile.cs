using AutoMapper;
using Common.Models;
using static Common.Enumeration.Enumeration;

namespace Common.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // CreateMap<Account, LoginResponse>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            //    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            //    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            //    .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Student != null ? src.Student.Code : src.Teacher.Code))
            //    .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : src.Teacher.Name))
            //    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Student != null ? src.Student.Phone : src.Teacher.Phone))
            //    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Student != null ? src.Student.Address : src.Teacher.Address))
            //    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<RegisterRequest, Account>()
               .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => System.DateTime.Now))
               .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.Role == (int)Role.Student ? new Student()
               {
                   Name = src.Fullname,
                   Code = src.Code,
                   Phone = src.Phone,
                   Address = src.Address,
                   CreatedBy = src.Email,
                   UpdatedBy = src.Email,
                   CreatedDate = System.DateTime.Now,
               } : null))
               .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.Role == (int)Role.Teacher ? new Teacher()
               {
                   Name = src.Fullname,
                   Code = src.Code,
                   Phone = src.Phone,
                   Address = src.Address,
                   CreatedBy = src.Email,
                   UpdatedBy = src.Email,
                   CreatedDate = System.DateTime.Now,
               } : null)); ;

            CreateMap<UserInfo, Account>()
               .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.Role == (int)Role.Student ? new Student()
               {
                   Name = src.Fullname,
                   Code = src.Code,
                   Phone = src.Phone,
                   Address = src.Address,
               } : null))
               .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.Role == (int)Role.Teacher ? new Teacher()
               {
                   Name = src.Fullname,
                   Code = src.Code,
                   Phone = src.Phone,
                   Address = src.Address,
               } : null));

            CreateMap<Account, UserInfo>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : src.Teacher.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Student != null ? src.Student.Code : src.Teacher.Code))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Student != null ? src.Student.Phone : src.Teacher.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Student != null ? src.Student.Address : src.Teacher.Address))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
        }
    }
}
