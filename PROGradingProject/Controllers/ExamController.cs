using System.Net.Http.Headers;
using Aspose.Zip.Rar;
using AutoMapper;
using BusinessLogic;
using Common.Attributes;
using Common.Models;
using DataAccess.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROGradingAPI.Controllers.Base;
using static Common.Enumeration.Enumeration;

namespace PROGradingAPI.Controllers
{
    //[Authorize]
    public class ExamController : BaseNewController
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IAutoMarkService _autoMarkService;

        public ExamController(AppDbContext context,
                IMapper mapper,
                IAuthService authService,
                IHttpContextAccessor httpContextAccessor,
                IConfiguration configuration,
                IAutoMarkService autoMarkService) : base(httpContextAccessor, configuration)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
            _autoMarkService = autoMarkService;
        }
        /// <summary>
        /// Get all exams theo student
        /// </summary>
        [HttpGet("GetAllStudentExam")]
        [Authorize]
        public IActionResult GetAllExamByUser()
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            var result = _context.ExamStudents.Include(x => x.Exam).Where(x => x.StudentId == _authService.GetUserId()).OrderByDescending(x => x.CreatedDate);
            serviceResponse.OnSuccess(_mapper.ProjectTo<ExamStudentList>(result));
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Change exam password
        /// </summary>
        [HttpPut("ChangeExamPassword")]
        [Authorize]
        public IActionResult ChangeExamPassword(ExamChangePasswordDTO examChangePassword)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examChangePassword.ExamId && x.TeacherId == _authService.GetUserId());
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return Ok(serviceResponse);
            }
            if (!string.IsNullOrEmpty(examChangePassword.Password))
            {
                exam.Password = BCrypt.Net.BCrypt.HashPassword(examChangePassword.Password);
            }
            else
            {
                exam.Password = null;
            }
            _context.Exams.Update(exam);
            _context.SaveChanges();
            serviceResponse.OnSuccess(message: "Change password success");
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Get detail exam theo student
        /// </summary>
        [HttpGet("{examId}")]
        [Authorize]
        public IActionResult GetDetailExam(int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            var exam = _context.ExamStudents.Include(x => x.Exam).FirstOrDefault(x => x.ExamId == examId && x.StudentId == _authService.GetUserId());
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return Ok(serviceResponse);
            }
            var examDTO = _mapper.Map<ExamStudentResponse>(exam);
            serviceResponse.OnSuccess(examDTO);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Get detail exam theo student
        /// </summary>
        [HttpGet("teacher/{examId}")]
        [Authorize]
        public IActionResult GetDetailExamByTeacher(int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examId && x.TeacherId == _authService.GetUserId());
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return Ok(serviceResponse);
            }
            // var examDTO = _mapper.Map<Exam>(exam);
            serviceResponse.OnSuccess(exam);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Student join exam
        /// </summary>
        [HttpPost("JoinExam")]
        [Authorize]
        public IActionResult JoinExam(ExamRequestJoinDTO requestJoin)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            if (!ModelState.IsValid)
            {
                serviceResponse.OnError(message: "Exam code not empty");
                return Ok(serviceResponse);
            }
            var exam = _context.Exams.FirstOrDefault(x => x.ExamCode == requestJoin.ExamCode);
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return Ok(serviceResponse);
            }
            if (!string.IsNullOrEmpty(exam.Password) && string.IsNullOrEmpty(requestJoin.Password))
            {
                serviceResponse.OnError(message: "Password not empty");
                return Ok(serviceResponse);
            }
            if (!string.IsNullOrEmpty(exam.Password) && !string.IsNullOrEmpty(requestJoin.Password))
            {
                bool checkPassword = BCrypt.Net.BCrypt.Verify(requestJoin.Password, exam.Password);
                if (!checkPassword)
                {
                    serviceResponse.OnError(message: "Password is wrong");
                    return Ok(serviceResponse);
                }
            }
            var userId = _authService.GetUserId();
            var examStudent = _context.ExamStudents.FirstOrDefault(x => x.ExamId == exam.ExamId && x.StudentId == userId);
            if (examStudent != null)
            {
                serviceResponse.OnError(message: "You have joined this exam");
                return Ok(serviceResponse);
            }
            var examStudentEntity = new ExamStudent()
            {
                ExamId = exam.ExamId,
                StudentId = userId,
                Status = 0,
            };
            _context.ExamStudents.Add(examStudentEntity);
            _context.SaveChanges();
            serviceResponse.OnSuccess(message: "Join exam success");
            return Ok(serviceResponse);
        }
        /// <summary>
        /// Student submit exam
        /// </summary>
        [HttpPost("SubmitExam")]
        public async Task<IActionResult> SubmitExam([FromForm] ExamStudentSubmitDTO requestSubmit, List<IFormFile> files)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            if (requestSubmit?.ExamId == null)
            {
                serviceResponse.OnError(message: "ExamId not empty");
                return Ok(serviceResponse);
            }
            if (files.Count == 0)
            {
                serviceResponse.OnError(message: "Submit folder not empty");
                return Ok(serviceResponse);
            }
            long size = files.Sum(f => f.Length);
            if (size > 20971520)
            {
                serviceResponse.OnError(message: "Submit folder not greater than 20MB");
                return Ok(serviceResponse);
            }
            var exam = _context.Exams.FirstOrDefault(x => x.ExamId == requestSubmit.ExamId);
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return Ok(serviceResponse);
            }

            var examStatus = exam.Status; // 0: Not start, 1: Started, 2: Ended
            if (examStatus == 0)
            {
                // exam not start
                serviceResponse.OnError(message: "Exam not start");
                return Ok(serviceResponse);
            }
            if (examStatus == 2)
            {
                // exam ended
                serviceResponse.OnError(message: "Exam ended");
                return Ok(serviceResponse);
            }
            var examStudent = _context.ExamStudents.FirstOrDefault(x => x.ExamId == exam.ExamId && x.StudentId == _authService.GetUserId());
            if (examStudent == null)
            {
                serviceResponse.OnError(message: "You have not joined this exam");
                return Ok(serviceResponse);
            }
            // TODO: check submit exam status (0: Not submit, 1: Start 2: Submit, 3: Submit late)
            // if (examStudent.Status == 2)
            // {
            //     serviceResponse.OnError(message: "You have submitted this exam");
            //     return Ok(serviceResponse);
            // }
            var submitedTime = DateTime.Now; // time student submit exam
            var examStartTime = exam.StartTime; // start time of exam
            var examEndTime = exam.EndTime; // end time of exam
            var submitStatus = 2; // 0: Not submit, 1: Start 2: Submit, 3: Submit late
            var countTimeSubmit = examStudent.CountTimeSubmit == null ? 1 : examStudent.CountTimeSubmit + 1; // count time student submit exam
            if (submitedTime < examStartTime)
            {
                // submit early exam
                serviceResponse.OnError(message: "Exam not start");
                return Ok(serviceResponse);
            }
            if (submitedTime > examEndTime)
            {
                // submit late exam
                submitStatus = 3;
            }
            var pathToSave = GetFilePath("ExamStudent");
            pathToSave = Path.Combine(pathToSave, exam.ExamCode);
            pathToSave = Path.Combine(pathToSave, _authService.GetUserId().ToString());
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
            else
            {
                Directory.Delete(pathToSave, true);
                Directory.CreateDirectory(pathToSave);
            }
            List<string> filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    filePaths.Add(fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            foreach (var filePath in filePaths)
            {
                var fullPath = Path.Combine(pathToSave, filePath);
                UnzipFile(fullPath, pathToSave);
            }
            // câp nhật lại vào bảng exam student
            examStudent.SubmitedTime = submitedTime;
            examStudent.SubmitedFolder = Path.Combine(pathToSave, filePaths[0]);
            examStudent.Status = submitStatus;
            examStudent.CountTimeSubmit = countTimeSubmit;
            _context.ExamStudents.Update(examStudent);
            _autoMarkService.Mark(pathToSave, exam.TestCaseFolder, examStudent.ExamId, examStudent.StudentId);
            _context.SaveChanges();
            serviceResponse.OnSuccess(message: "Submit exam success");
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Start exam
        /// </summary>
        [HttpPost("StartExam")]
        public IActionResult StartExam([FromBody] int? examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            if (examId == null)
            {
                serviceResponse.OnError(message: "ExamId not empty");
                return Ok(serviceResponse);
            }
            var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examId);
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return Ok(serviceResponse);
            }

            var examStatus = exam.Status; // 0: Not start, 1: Started, 2: Ended
            var currentTime = DateTime.Now;
            if (examStatus == 0 || currentTime < exam.StartTime)
            {
                // exam not start
                serviceResponse.OnError(message: "Exam not start");
                return Ok(serviceResponse);
            }
            if (examStatus == 2 || currentTime > exam.EndTime)
            {
                // exam ended
                serviceResponse.OnError(message: "Exam ended");
                return Ok(serviceResponse);
            }
            var examStudent = _context.ExamStudents.FirstOrDefault(x => x.ExamId == exam.ExamId && x.StudentId == _authService.GetUserId());
            if (examStudent == null)
            {
                serviceResponse.OnError(message: "You have not joined this exam");
                return Ok(serviceResponse);
            }
            if (examStudent.Status == 2)
            {
                serviceResponse.OnError(message: "You have submitted this exam");
                return Ok(serviceResponse);
            }
            if (examStudent.Status == 3)
            {
                serviceResponse.OnError(message: "You have submitted late this exam");
                return Ok(serviceResponse);
            }
            if (examStudent.Status == 0)
            {
                examStudent.StartTime = DateTime.Now;
                examStudent.Status = 1;
                _context.ExamStudents.Update(examStudent);
                _context.SaveChanges();
                var examStudentDTO = _mapper.Map<ExamRequestDTO>(exam);
                serviceResponse.OnSuccess(new
                {
                    exam = examStudentDTO,
                    startTime = examStudent.StartTime
                });
                return Ok(serviceResponse);
            }
            serviceResponse.OnError(message: "Something went wrong");
            return Ok(serviceResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllTeacherExam")]
        [CustomAuthorize(Role.Teacher)]
        public IActionResult GetAllExamByTeacher()
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            var exams = _context.Exams
                .Include(x => x.ExamStudents)
                .Where(x => x.TeacherId == _authService.GetUserId())
                .OrderByDescending(x => x.ExamId);
            serviceResponse.OnSuccess(_mapper.ProjectTo<ExamListDTO>(exams));
            return Ok(serviceResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpGet("GetStudentExam")]
        public IActionResult GetStudentExam(int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            var exam = _context.Exams.Include(x => x.ExamStudents).ThenInclude(x => x.Student).FirstOrDefault(x => x.ExamId == examId);
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return Ok(serviceResponse);
            }
            var examDTO = _mapper.Map<ExamDetailDTO>(exam);
            serviceResponse.OnSuccess(examDTO);
            return Ok(serviceResponse);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteExam")]
        public IActionResult DeleteExam(int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examId);
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return Ok(serviceResponse);
            }
            var examStudent = _context.ExamStudents.FirstOrDefault(x => x.ExamId == exam.ExamId);
            if (examStudent != null)
            {
                serviceResponse.OnError(message: "Exam has taken by student");
                return Ok(serviceResponse);
            }
            _context.Exams.Remove(exam);
            _context.SaveChanges();
            var pathToSave = GetFilePath("MaterialFolder");
            pathToSave = Path.Combine(pathToSave, exam.ExamCode);
            if (Directory.Exists(pathToSave))
            {
                Directory.Delete(pathToSave, true);
            }
            serviceResponse.OnSuccess();
            return Ok(serviceResponse);
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
            ServiceResponse serviceResponse = new ServiceResponse();
            var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examParam.ExamId && x.TeacherId == _authService.GetUserId());
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return Ok(serviceResponse);
            }
            exam.IsShowScore = examParam.IsShowScore;
            _context.Exams.Update(exam);
            _context.SaveChanges();
            serviceResponse.OnSuccess();
            return Ok(serviceResponse);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exam"></param>
        /// <returns></returns>
        [HttpPost("CreateExam")]
        public IActionResult CreateExam([FromForm] ExamRequestDTO exam)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            _context.Database.BeginTransaction();
            try
            {
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not empty");
                    return Ok(serviceResponse);
                }
                var examCode = _context.Exams.Any(x => x.ExamCode == exam.ExamCode);
                if (examCode)
                {
                    serviceResponse.OnError(message: "Exam code is exist");
                    return Ok(serviceResponse);
                }
                var pathToSave = GetFilePath("MaterialFolder");
                pathToSave = Path.Combine(pathToSave, exam.ExamCode);
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                else
                {
                    Directory.Delete(pathToSave, true);
                    Directory.CreateDirectory(pathToSave);
                }
                List<IFormFile> files = new List<IFormFile>();
                List<string> filePaths = new List<string>();
                if (exam.QuestionFile != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(exam.QuestionFile.ContentDisposition).FileName.Trim('"');
                    var questionPath = Path.Combine(pathToSave, "Question");
                    var fullPath = Path.Combine(questionPath, fileName);
                    files.Add(exam.QuestionFile);
                    filePaths.Add(fullPath);
                    exam.QuestionFolder = fullPath;
                }
                if (exam.AnswerFile != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(exam.AnswerFile.ContentDisposition).FileName.Trim('"');
                    var answerPath = Path.Combine(pathToSave, "Answer");
                    var fullPath = Path.Combine(answerPath, fileName);
                    files.Add(exam.AnswerFile);
                    filePaths.Add(fullPath);
                    exam.AnswerFolder = fullPath;
                }
                else
                {
                    exam.AnswerFolder = "";
                }
                if (exam.TestCaseFile != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(exam.TestCaseFile.ContentDisposition).FileName.Trim('"');
                    var testCasePath = Path.Combine(pathToSave, "TestCase");
                    var fullPath = Path.Combine(testCasePath, fileName);
                    files.Add(exam.TestCaseFile);
                    filePaths.Add(fullPath);
                    exam.TestCaseFolder = fullPath;
                }
                var examEntity = _mapper.Map<Exam>(exam);
                if (!string.IsNullOrEmpty(examEntity.Password))
                {
                    examEntity.Password = BCrypt.Net.BCrypt.HashPassword(examEntity.Password);
                }
                examEntity.TeacherId = _authService.GetUserId();
                _context.Exams.Add(examEntity);
                _context.SaveChanges();
                for (int i = 0; i < filePaths.Count; i++)
                {
                    var fullPath = filePaths[i];
                    // nếu fullpath chưa tồn tại thì tạo mới
                    if (!System.IO.File.Exists(fullPath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    }
                    var formFile = files[i];
                    if (formFile.Length > 0)
                    {
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            formFile.CopyTo(stream);
                        }
                    }
                }
                serviceResponse.OnSuccess(examEntity);
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                serviceResponse.OnError(message: ex.Message);
                return Ok(serviceResponse);
            }
            finally
            {
                _context.Database.CommitTransaction();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpPut("UpdateExam")]
        public IActionResult UpdateExam([FromForm] ExamRequestDTO exam, int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            exam.ExamId = examId;
            _context.Database.BeginTransaction();
            try
            {
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not empty");
                    return Ok(serviceResponse);
                }
                var orgExam = _context.Exams.FirstOrDefault(x => x.ExamId == exam.ExamId && x.TeacherId == _authService.GetUserId());
                if (orgExam == null)
                {
                    serviceResponse.OnError(message: "Exam not found");
                    return Ok(serviceResponse);
                }
                var tempPassword = orgExam.Password;
                var pathToSave = GetFilePath("MaterialFolder");
                pathToSave = Path.Combine(pathToSave, orgExam.ExamCode);
                List<IFormFile> files = new List<IFormFile>();
                List<string> filePaths = new List<string>();
                List<string> deleteFiles = new List<string>();
                if (exam.QuestionFile != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(exam.QuestionFile.ContentDisposition).FileName.Trim('"');
                    var questionPath = Path.Combine(pathToSave, "Question");
                    var fullPath = Path.Combine(questionPath, fileName);
                    files.Add(exam.QuestionFile);
                    filePaths.Add(fullPath);
                    deleteFiles.Add(orgExam.QuestionFolder);
                    orgExam.QuestionFolder = fullPath;
                }
                if (exam.AnswerFile != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(exam.AnswerFile.ContentDisposition).FileName.Trim('"');
                    var answerPath = Path.Combine(pathToSave, "Answer");
                    var fullPath = Path.Combine(answerPath, fileName);
                    files.Add(exam.AnswerFile);
                    filePaths.Add(fullPath);
                    deleteFiles.Add(orgExam.AnswerFolder);
                    orgExam.AnswerFolder = fullPath;
                }
                else
                {
                    orgExam.AnswerFolder = "";
                }
                if (exam.TestCaseFile != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(exam.TestCaseFile.ContentDisposition).FileName.Trim('"');
                    var testCasePath = Path.Combine(pathToSave, "TestCase");
                    var fullPath = Path.Combine(testCasePath, fileName);
                    files.Add(exam.TestCaseFile);
                    filePaths.Add(fullPath);
                    deleteFiles.Add(orgExam.TestCaseFolder);
                    orgExam.TestCaseFolder = fullPath;
                }
                orgExam.Status = exam.Status;
                orgExam.ExamName = exam.ExamName;
                orgExam.ExamCode = exam.ExamCode;
                orgExam.StartTime = exam.StartTime;
                orgExam.EndTime = exam.EndTime;
                orgExam.TotalQuestions = exam.TotalQuestions;
                orgExam.TotalScore = exam.TotalScore;
                orgExam.UpdatedDate = DateTime.Now;
                _context.Exams.Update(orgExam);
                _context.SaveChanges();
                for (int i = 0; i < filePaths.Count; i++)
                {
                    var fullPath = filePaths[i];
                    // nếu fullpath chưa tồn tại thì tạo mới
                    if (!System.IO.File.Exists(fullPath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    }
                    var formFile = files[i];
                    if (formFile.Length > 0)
                    {
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            formFile.CopyTo(stream);
                        }
                    }
                }
                foreach (var deleteFile in deleteFiles)
                {
                    if (!string.IsNullOrEmpty(deleteFile))
                    {
                        System.IO.File.Delete(deleteFile);
                    }
                }
                serviceResponse.OnSuccess(orgExam);
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                serviceResponse.OnError(message: ex.Message);
                return Ok(serviceResponse);
            }
            finally
            {
                _context.Database.CommitTransaction();
            }
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
            ServiceResponse res = new ServiceResponse();
            if (string.IsNullOrEmpty(fileName))
            {
                res.OnError(message: "File name is null or empty");
                return Ok(res);
            }
            var fullPath = fileName;
            if (!System.IO.File.Exists(fullPath))
            {
                res.OnError(message: "File not found");
                return Ok(res);
            }
            var memory = new MemoryStream();
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(fullPath), Path.GetFileName(fullPath));
        }
        
        /// <summary>
        /// Unzip file
        /// </summary>
        private void UnzipFile(string filePath, string directory)
        {
            if (filePath.Contains(".rar") || filePath.Contains(".zip"))
            {
                using (RarArchive archive = new RarArchive(filePath))
                {
                    archive.ExtractToDirectory(directory);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetFilePath(string path)
        {
            string dirPath = "";
            switch (path)
            {
                case "Student":
                    dirPath = _configuration["ConfigFolderSave:StudentFolder"];
                    break;
                case "Teacher":
                    dirPath = _configuration["ConfigFolderSave:TeacherFolder"];
                    break;
                case "Exam":
                    dirPath = _configuration["ConfigFolderSave:ExamFolder"];
                    break;
                case "MaterialFolder":
                    dirPath = _configuration["ConfigFolderSave:MaterialFolder"];
                    break;
                case "ExamStudent":
                    dirPath = _configuration["ConfigFolderSave:ExamStudentFolder"];
                    break;
                default:
                    break;
            }
            return dirPath;
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
