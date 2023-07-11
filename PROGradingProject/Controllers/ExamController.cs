using AutoMapper;
using BusinessLogic;
using Common.Attributes;
using Common.Models;
using DataAccess.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROGradingAPI.Controllers.Base;
using static Common.Enumeration.Enumeration;

namespace PROGradingAPI.Controllers
{
    [Authorize]
    public class ExamController : BaseNewController
    {
        private readonly IExamService _examService;
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IAutoMarkService _autoMarkService;

        public ExamController(AppDbContext context,
                IMapper mapper,
                IAuthService authService,
                IHttpContextAccessor httpContextAccessor,
                IExamService examService,
                IConfiguration configuration,
                IAutoMarkService autoMarkService) : base(httpContextAccessor, configuration)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
            _autoMarkService = autoMarkService;
            _examService = examService;
        }

        /// <summary>
        /// Get all exams theo student
        /// </summary>
        [HttpGet("GetAllStudentExam")]
        public IActionResult GetAllExamByUser(int page = 1, int pageSize = 10, string search = "")
        {
            var res = _examService.GetAllStudentExam(page, pageSize, search);
            return Ok(res);
        }

        /// <summary>
        /// Change exam password
        /// </summary>
        [HttpPut("ChangeExamPassword")]
        public IActionResult ChangeExamPassword(ExamChangePasswordDTO examChangePassword)
        {
            var res = _examService.ChangeExamPassword(examChangePassword);
            return Ok(res);
        }

        /// <summary>
        /// Get detail exam theo student
        /// </summary>
        [HttpGet("{examId}")]
        public IActionResult GetDetailExam(int examId)
        {
            var res = _examService.GetDetailExam(examId);
            return Ok(res);
        }

        /// <summary>
        /// Get detail exam theo student
        /// </summary>
        [HttpGet("teacher/{examId}")]
        public IActionResult GetDetailExamByTeacher(int examId)
        {
            var res = _examService.GetDetailExamByTeacher(examId);
            return Ok(res);
        }

        /// <summary>
        /// Student join exam
        /// </summary>
        [HttpPost("JoinExam")]
        public IActionResult JoinExam(ExamRequestJoinDTO requestJoin)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            if (!ModelState.IsValid)
            {
                serviceResponse.OnError(message: "Exam code not empty");
                return Ok(serviceResponse);
            }
            var result = _examService.JoinExam(requestJoin);
            return Ok(result);
        }
        /// <summary>
        /// Student submit exam
        /// </summary>
        [HttpPost("SubmitExam")]
        public async Task<IActionResult> SubmitExam([FromForm] ExamStudentSubmitDTO requestSubmit, List<IFormFile> files)
        {
            var res = await _examService.SubmitExam(requestSubmit, files);
            return Ok(res);
        }

        /// <summary>
        /// Start exam
        /// </summary>
        [HttpPost("StartExam")]
        public IActionResult StartExam([FromBody] int examId)
        {
            var res = _examService.StartExam(examId);
            return Ok(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllTeacherExam")]
        [CustomAuthorize(Role.Teacher)]
        public IActionResult GetAllExamByTeacher()
        {
            var res = _examService.GetAllTeacherExam();
            return Ok(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpGet("GetStudentExam")]
        [CustomAuthorize(Role.Teacher)]
        public IActionResult GetStudentExam(int examId)
        {
            var res = _examService.GetStudentExam(examId);
            return Ok(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteExam")]
        [CustomAuthorize(Role.Teacher)]
        public IActionResult DeleteExam(int examId)
        {
            var res = _examService.DeleteExam(examId);
            return Ok(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examParam"></param>
        /// <returns></returns>
        [HttpPut("ChangeShowHideScore")]
        [CustomAuthorize(Role.Teacher)]
        public IActionResult ChangeShowHideScore(ExamShowHideScoreDTO examParam)
        {
            var res = _examService.ChangeShowHideScore(examParam);
            return Ok(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exam"></param>
        /// <returns></returns>
        [HttpPost("CreateExam")]
        [CustomAuthorize(Role.Teacher)]
        public IActionResult CreateExam([FromForm] ExamRequestDTO exam)
        {
            var res = _examService.CreateExam(exam);
            return Ok(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpPut("UpdateExam")]
        [CustomAuthorize(Role.Teacher)]
        public IActionResult UpdateExam([FromForm] ExamRequestDTO exam, int examId)
        {
            var res = _examService.UpdateExam(exam, examId);
            return Ok(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("DownloadFile")]
        [Authorize]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var res = await _examService.DownloadFile(fileName);
            if (res.Success)
            {
                var memory = res.Data?.GetType()?.GetProperty("Memory")?.GetValue(res.Data) as MemoryStream;
                var fullPath = res.Data?.GetType()?.GetProperty("FullPath")?.GetValue(res.Data) as string;
                return File(memory, GetContentType(fullPath), Path.GetFileName(fullPath));
            }
            else
            {
                return Ok(res);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".rar", "application/x-rar-compressed" }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private string GetContentType(string fullPath)
        {
            var types = GetMimeTypes(); // chứa các kiểu file và kiểu content của nó
            var ext = Path.GetExtension(fullPath).ToLowerInvariant();
            return types[ext];
        }

    }
}
