using Aspose.Zip.Rar;
using AutoMapper;
using BusinessLogic.Hubs;
using Common.Models;
using Common.Models.DTO;
using DataAccess.DatabaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace BusinessLogic.Service
{
    public class ExamService : IExamService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IAutoMarkService _autoMarkService;
        public readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private IHubContext<GradingSignalR> _hubContext;
        public ExamService(AppDbContext context,
                IMapper mapper,
                IAuthService authService,
                IConfiguration configuration,
                IServiceProvider serviceProvider,
                IAutoMarkService autoMarkService,
                IHubContext<GradingSignalR> hubContext)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
            _autoMarkService = autoMarkService;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examChangePassword"></param>
        /// <returns></returns>
        public ServiceResponse ChangeExamPassword(ExamChangePasswordDTO examChangePassword)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examChangePassword.ExamId && x.TeacherId == _authService.GetUserId());
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return serviceResponse;
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
            return serviceResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examParam"></param>
        /// <returns></returns>
        public ServiceResponse ChangeShowHideScore(ExamShowHideScoreDTO examParam)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examParam.ExamId && x.TeacherId == _authService.GetUserId());
            if (exam == null)
            {
                serviceResponse.OnError(message: "Exam not found");
                return serviceResponse;
            }
            exam.IsShowScore = examParam.IsShowScore;
            _context.Exams.Update(exam);
            _context.SaveChanges();
            _hubContext.Clients.Group(exam.ExamId.ToString()).SendAsync("UpdateExamToClients", exam.ExamId);
            serviceResponse.OnSuccess();
            return serviceResponse;
        }

        /// <summary>
        /// Create exam
        /// </summary>
        /// <param name="exam"></param>
        public ServiceResponse CreateExam(ExamRequestDTO exam)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not empty");
                    return serviceResponse;
                }
                var examCode = _context.Exams.Any(x => x.ExamCode == exam.ExamCode);
                if (examCode)
                {
                    serviceResponse.OnError(message: "Exam code is exist");
                    return serviceResponse;
                }
                _context.Database.BeginTransaction();
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
                _context.Database.CommitTransaction();
                serviceResponse.OnSuccess(examEntity);
                return serviceResponse;
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                serviceResponse.OnError(message: ex.Message);
                return serviceResponse;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public ServiceResponse DeleteExam(int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            _context.Database.BeginTransaction();
            try
            {
                var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examId);
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not found");
                    return serviceResponse;
                }
                var examStudent = _context.ExamStudents.FirstOrDefault(x => x.ExamId == exam.ExamId);
                if (examStudent != null)
                {
                    serviceResponse.OnError(message: "Exam has taken by student");
                    return serviceResponse;
                }
                _context.Exams.Remove(exam);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
                var pathToSave = GetFilePath("MaterialFolder");
                pathToSave = Path.Combine(pathToSave, exam.ExamCode);
                if (Directory.Exists(pathToSave))
                {
                    Directory.Delete(pathToSave, true);
                }
                serviceResponse.OnSuccess();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                serviceResponse.OnError(message: ex.Message);
            }
            return serviceResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> DownloadFile(string fileName)
        {
            ServiceResponse res = new ServiceResponse();
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    res.OnError(message: "File name is null or empty");
                    return res;
                }
                var fullPath = fileName;
                if (!File.Exists(fullPath))
                {
                    res.OnError(message: "File not found");
                    return res;
                }
                var memory = new MemoryStream();
                using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                res.OnSuccess(new
                {
                    Memory = memory,
                    FullPath = fullPath
                });
            }
            catch (Exception ex)
            {
                res.OnError(message: ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ServiceResponse GetAllStudentExam(int page = 1, int pageSize = 10, string search = "")
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                var result = _context.ExamStudents.Include(x => x.Exam).Where(x => x.StudentId == _authService.GetUserId()).OrderByDescending(x => x.CreatedDate);
                if (!string.IsNullOrEmpty(search))
                {
                    result = result.Where(e => e.Exam.ExamCode.Contains(search) || e.Exam.ExamName.Contains(search)).OrderByDescending(x => x.CreatedDate);
                }
                var totalCount = result.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var exams = result.Skip((page - 1) * pageSize).Take(pageSize);

                var response = new PagingModel<ExamStudentList>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Data = _mapper.ProjectTo<ExamStudentList>(exams)?.ToList()
                };
                serviceResponse.OnSuccess(response);
            }
            catch (Exception ex)
            {
                serviceResponse.OnError(message: ex.Message);
            }
            return serviceResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ServiceResponse GetAllTeacherExam()
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                var exams = _context.Exams
                .Include(x => x.ExamStudents)
                .Where(x => x.TeacherId == _authService.GetUserId())
                .OrderByDescending(x => x.ExamId);
                serviceResponse.OnSuccess(_mapper.ProjectTo<ExamListDTO>(exams));
            }
            catch (Exception ex)
            {
                serviceResponse.OnError(message: ex.Message);
            }
            return serviceResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public ServiceResponse GetDetailExam(int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                var exam = _context.ExamStudents.Include(x => x.Exam).FirstOrDefault(x => x.ExamId == examId && x.StudentId == _authService.GetUserId());
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not found");
                    return serviceResponse;
                }
                if (exam.Status == 0)
                {
                    serviceResponse.OnError(message: "Start exam to view detail");
                    return serviceResponse;
                }
                var examDTO = _mapper.Map<ExamStudentResponseDTO>(exam);
                serviceResponse.OnSuccess(examDTO);
            }
            catch (Exception ex)
            {
                serviceResponse.OnError(message: ex.Message);
            }
            return serviceResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public ServiceResponse GetDetailExamByTeacher(int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examId && x.TeacherId == _authService.GetUserId());
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not found");
                    return serviceResponse;
                }
                // var examDTO = _mapper.Map<Exam>(exam);
                serviceResponse.OnSuccess(exam);
            }
            catch (Exception ex)
            {
                serviceResponse.OnError(message: ex.Message);
            }
            return serviceResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public ServiceResponse GetStudentExam(int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                var exam = _context?.Exams?.Include(x => x.ExamStudents).ThenInclude(x => x.Student).FirstOrDefault(x => x.ExamId == examId);
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not found");
                    return serviceResponse;
                }
                var examDTO = _mapper.Map<ExamDetailDTO>(exam);
                serviceResponse.OnSuccess(examDTO);
            }
            catch (Exception ex)
            {
                serviceResponse.OnError(message: ex.Message);
            }
            return serviceResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestJoin"></param>
        /// <returns></returns>
        public ServiceResponse JoinExam(ExamRequestJoinDTO requestJoin)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                var exam = _context.Exams.FirstOrDefault(x => x.ExamCode == requestJoin.ExamCode);
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not found");
                    return serviceResponse;
                }
                if (!string.IsNullOrEmpty(exam.Password) && string.IsNullOrEmpty(requestJoin.Password))
                {
                    serviceResponse.OnError(message: "Password not empty");
                    return serviceResponse;
                }
                if (!string.IsNullOrEmpty(exam.Password) && !string.IsNullOrEmpty(requestJoin.Password))
                {
                    bool checkPassword = BCrypt.Net.BCrypt.Verify(requestJoin.Password, exam.Password);
                    if (!checkPassword)
                    {
                        serviceResponse.OnError(message: "Password is wrong");
                        return serviceResponse;
                    }
                }
                var userId = _authService.GetUserId();
                var examStudent = _context.ExamStudents.FirstOrDefault(x => x.ExamId == exam.ExamId && x.StudentId == userId);
                if (examStudent != null)
                {
                    serviceResponse.OnError(message: "You have joined this exam");
                    return serviceResponse;
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
            }
            catch (Exception ex)
            {
                serviceResponse.OnError(message: ex.Message);
            }
            return serviceResponse;
        }

        /// <summary>
        /// Start exam
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public ServiceResponse StartExam(int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                // validate ExamId
                if (examId == 0)
                {
                    serviceResponse.OnError(message: "ExamId not empty");
                    return serviceResponse;
                }

                // validate Exam exist
                var exam = _context.Exams.FirstOrDefault(x => x.ExamId == examId);
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not found");
                    return serviceResponse;
                }

                // validate Exam status
                var examStatus = exam.Status; // 0: Not start, 1: Started, 2: Ended

                var currentTime = DateTime.Now;

                if (examStatus == 0 || currentTime < exam.StartTime)
                {
                    // exam not start
                    serviceResponse.OnError(message: "Exam not start");
                    return serviceResponse;
                }

                if (examStatus == 2 || currentTime > exam.EndTime)
                {
                    // exam ended
                    serviceResponse.OnError(message: "Exam ended");
                    return serviceResponse;
                }

                // validate ExamStudent exist and status
                var examStudent = _context.ExamStudents.FirstOrDefault(x => x.ExamId == exam.ExamId && x.StudentId == _authService.GetUserId());
                if (examStudent == null)
                {
                    serviceResponse.OnError(message: "You have not joined this exam");
                    return serviceResponse;
                }

                if (examStudent.Status == 2)
                {
                    serviceResponse.OnError(message: "You have submitted this exam");
                    return serviceResponse;
                }

                if (examStudent.Status == 3)
                {
                    serviceResponse.OnError(message: "You have submitted late this exam");
                    return serviceResponse;
                }

                // update ExamStudent
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
                    return serviceResponse;
                }
                serviceResponse.OnError(message: "Something went wrong");
            }
            catch (Exception ex)
            {
                serviceResponse.OnError(message: ex.Message);
            }
            return serviceResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestSubmit"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> SubmitExam(ExamStudentSubmitDTO requestSubmit, List<IFormFile> files)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                // validate ExamId
                if (requestSubmit?.ExamId == null)
                {
                    serviceResponse.OnError(message: "ExamId not empty");
                    return serviceResponse;
                }

                // validate Submit folder
                if (files.Count == 0)
                {
                    serviceResponse.OnError(message: "Submit folder not empty");
                    return serviceResponse;
                }

                // validate Submit folder size < 20MB
                long size = files.Sum(f => f.Length);
                if (size > 20971520)
                {
                    serviceResponse.OnError(message: "Submit folder not greater than 20MB");
                    return serviceResponse;
                }

                // validate Exam exist
                var exam = _context.Exams.FirstOrDefault(x => x.ExamId == requestSubmit.ExamId);
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not found");
                    return serviceResponse;
                }

                // validate Exam status
                var examStatus = exam.Status; // 0: Not start, 1: Started, 2: Ended
                if (examStatus == 0)
                {
                    // exam not start
                    serviceResponse.OnError(message: "Exam not start");
                    return serviceResponse;
                }

                if (examStatus == 2)
                {
                    // exam ended
                    serviceResponse.OnError(message: "Exam ended");
                    return serviceResponse;
                }

                // validate ExamStudent exist and status
                var examStudent = _context.ExamStudents.FirstOrDefault(x => x.ExamId == exam.ExamId && x.StudentId == _authService.GetUserId());
                if (examStudent == null)
                {
                    serviceResponse.OnError(message: "You have not joined this exam");
                    return serviceResponse;
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

                // validate submit time of student
                if (submitedTime < examStartTime)
                {
                    // submit early exam
                    serviceResponse.OnError(message: "Exam not start");
                    return serviceResponse;
                }
                if (submitedTime > examEndTime)
                {
                    // submit late exam
                    submitStatus = 3;
                }

                // save file to folder ExamStudent
                var pathToSave = GetFilePath("ExamStudent");

                // create folder ExamCode in folder ExamStudent
                pathToSave = Path.Combine(pathToSave, exam.ExamCode);

                // create folder StudentId in folder ExamCode
                pathToSave = Path.Combine(pathToSave, _authService.GetUserId().ToString());

                // create folder ExamId in folder StudentId
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                else
                {
                    Directory.Delete(pathToSave, true);
                    Directory.CreateDirectory(pathToSave);
                }

                // save file to folder ExamId
                List<string> filePaths = new List<string>();

                foreach (var formFile in files)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    filePaths.Add(fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
                foreach (var filePath in filePaths)
                {
                    var fullPath = Path.Combine(pathToSave, filePath);
                    UnzipFile(fullPath, pathToSave);
                }

                // update ExamStudent
                examStudent.SubmitedTime = submitedTime; // time student submit exam
                examStudent.SubmitedFolder = Path.Combine(pathToSave, filePaths[0]); // path to folder submit
                examStudent.Status = submitStatus; // 0: Not submit, 1: Start 2: Submit, 3: Submit late
                examStudent.CountTimeSubmit = countTimeSubmit; // count time student submit exam
                _context.ExamStudents.Update(examStudent); // update ExamStudent
                new Task(() =>
                {
                    // create new scope to get new instance of AppDbContext to avoid dispose exception
                    var instance = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
                    _autoMarkService.Mark(instance, pathToSave, exam.TestCaseFolder, examStudent.ExamId, examStudent.StudentId);
                }).Start();
                _context.SaveChanges();
                serviceResponse.OnSuccess(message: "Submit exam success");
            }
            catch (Exception ex)
            {
                serviceResponse.OnError(message: ex.Message);
            }
            return serviceResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="examId"></param>
        /// <returns></returns>
        public ServiceResponse UpdateExam(ExamRequestDTO exam, int examId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            exam.ExamId = examId;
            _context.Database.BeginTransaction();
            try
            {
                if (exam == null)
                {
                    serviceResponse.OnError(message: "Exam not empty");
                    return serviceResponse;
                }
                var orgExam = _context.Exams.FirstOrDefault(x => x.ExamId == exam.ExamId && x.TeacherId == _authService.GetUserId());
                if (orgExam == null)
                {
                    serviceResponse.OnError(message: "Exam not found");
                    return serviceResponse;
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
                    if (!File.Exists(fullPath))
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
                        File.Delete(deleteFile);
                    }
                }
                _hubContext.Clients.Group(examId.ToString()).SendAsync("UpdateExamToClients", examId);
                serviceResponse.OnSuccess(orgExam);
                return serviceResponse;
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                serviceResponse.OnError(message: ex.Message);
                return serviceResponse;
            }
            finally
            {
                _context.Database.CommitTransaction();
            }
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
    }
}
