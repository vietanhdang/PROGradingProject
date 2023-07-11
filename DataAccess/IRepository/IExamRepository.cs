using Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IExamRepository
    {
        #region STUDENT
        public ExamStudent GetAllStudentExam();
        public ExamStudent GetDetailExam(int examId);
        public ServiceResponse JoinExam(ExamRequestJoinDTO requestJoin);
        public ServiceResponse SubmitExam(ExamStudentSubmitDTO requestSubmit, List<IFormFile> files);
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
    }
}
