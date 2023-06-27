using AutoMapper;
using Common.Models;
using static Common.Enumeration.Enumeration;

namespace Common.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AccountMapper();
            ExamMapper();
        }

        private void ExamMapper()
        {
            CreateMap<ExamRequestDTO, Exam>();

            CreateMap<Exam, ExamRequestDTO>()
                .ForMember(dest => dest.IsStudentTakeExam, opt => opt.MapFrom(src => src.ExamStudents != null && src.ExamStudents.Count > 0));

            CreateMap<Exam, ExamDetailDTO>()
                .ForMember(dest => dest.IsStudentTakeExam, opt => opt.MapFrom(src => src.ExamStudents != null && src.ExamStudents.Count > 0));

            CreateMap<Exam, ExamListDTO>()
                .ForMember(dest => dest.IsStudentTakeExam, opt => opt.MapFrom(src => src.ExamStudents != null && src.ExamStudents.Count > 0));

            // Binding trên danh sách chi tiết bài thi của sinh viên
            CreateMap<ExamStudent, ExamStudentResponse>()
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam.ExamName))
                .ForMember(dest => dest.ExamCode, opt => opt.MapFrom(src => src.Exam.ExamCode))
                .ForMember(dest => dest.ExamStartTime, opt => opt.MapFrom(src => src.Exam.StartTime))
                .ForMember(dest => dest.ExamEndTime, opt => opt.MapFrom(src => src.Exam.EndTime))
                .ForMember(dest => dest.ExamStatus, opt => opt.MapFrom(src => src.Exam.Status))
                .ForMember(dest => dest.ExamQuestionFolder, opt => opt.MapFrom(src => src.Exam.QuestionFolder))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Exam.IsShowScore ? src.Score : null))
                .ForMember(dest => dest.MarkLog, opt => opt.MapFrom(src => src.Exam.IsShowScore ? src.MarkLog : null));
            
            // Binding trên danh sách các bài thi của sinh viên
            CreateMap<ExamStudent, ExamStudentList>()
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam.ExamName))
                .ForMember(dest => dest.ExamCode, opt => opt.MapFrom(src => src.Exam.ExamCode))
                .ForMember(dest => dest.ExamStartTime, opt => opt.MapFrom(src => src.Exam.StartTime))
                .ForMember(dest => dest.ExamEndTime, opt => opt.MapFrom(src => src.Exam.EndTime))
                .ForMember(dest => dest.ExamStatus, opt => opt.MapFrom(src => src.Exam.Status));

            CreateMap<ExamStudent, ExamStudentCustomDTO>();

            CreateMap<Student, ExamStudentCustomDTO>()
                .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Name));
        }
        private void AccountMapper()
        {
            CreateMap<RegisterRequest, Account>()
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
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : src.Teacher.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Student != null ? src.Student.Code : src.Teacher.Code))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Student != null ? src.Student.Phone : src.Teacher.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Student != null ? src.Student.Address : src.Teacher.Address));
        }
    }
}
