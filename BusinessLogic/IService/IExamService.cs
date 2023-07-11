using Common.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic
{
    public interface IExamService
    {
        #region STUDENT
        public ServiceResponse GetAllStudentExam(int page = 1, int pageSize = 10, string search = "");
        public ServiceResponse GetDetailExam(int examId);
        public ServiceResponse JoinExam(ExamRequestJoinDTO requestJoin);
        public Task<ServiceResponse> SubmitExam(ExamStudentSubmitDTO requestSubmit, List<IFormFile> files);
        public ServiceResponse StartExam(int examId);
        public ServiceResponse GetStudentExam(int examId);
        #endregion


        #region TEACHER
        public ServiceResponse GetDetailExamByTeacher(int examId);
        public ServiceResponse GetAllTeacherExam();
        public ServiceResponse ChangeExamPassword(ExamChangePasswordDTO examChangePassword);
        public ServiceResponse DeleteExam(int examId);
        public ServiceResponse ChangeShowHideScore(ExamShowHideScoreDTO examParam);
        public ServiceResponse CreateExam(ExamRequestDTO exam);
        public ServiceResponse UpdateExam(ExamRequestDTO exam, int examId);
        #endregion


        #region COMMON
        public Task<ServiceResponse> DownloadFile(string fileName);
        #endregion
    }
}
