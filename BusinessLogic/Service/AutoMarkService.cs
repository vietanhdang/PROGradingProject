using Common.Models;
using Common.Models.Mark;
using DataAccess.DatabaseContext;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BusinessLogic.Service
{
    /// <summary>
    /// Service chấm điểm tự động bài thi của sinh viên
    /// </summary>
    /// Author: ANHDVHE151359
    public class AutoMarkService : IAutoMarkService
    {
        private readonly AppDbContext _context;
        public AutoMarkService(AppDbContext context)
        {
            _context = context;
        }
        public ServiceResponse Mark(AppDbContext context, string submittedFolderPath, string testCaseDirectory, int examId, int studentId)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                // Thưc mục nộp bài luôn có định dạng .rar hoặc .zip nên cần xóa đuôi file nếu có để lấy đúng đường dẫn
                if (submittedFolderPath.EndsWith(".rar") || submittedFolderPath.EndsWith(".zip"))
                {
                    submittedFolderPath = submittedFolderPath.Substring(0, submittedFolderPath.LastIndexOf("."));
                }
                // nếu tồn tại submittedFolderPath thì mới chạy tiếp
                if (!Directory.Exists(submittedFolderPath))
                {
                    serviceResponse.OnError("Submitted folder not found");
                    return serviceResponse;
                }

                // nếu tồn tại testCaseDirectory thì mới chạy tiếp
                if (!File.Exists(testCaseDirectory))
                {
                    serviceResponse.OnError("Test case file not found");
                    return serviceResponse;
                }
                // Đọc file test case
                string json = File.ReadAllText(testCaseDirectory);

                // Chuyển đổi file test case thành object
                List<Question> questions = null;

                try
                {
                    // Chuyển đổi file test case thành object
                    questions = JsonConvert.DeserializeObject<List<Question>>(json);
                }
                catch (Exception)
                {
                    serviceResponse.OnError("Test case file is not valid json format");
                    return serviceResponse;
                }

                // Số lượng câu hỏi có trong test case
                int totalQuestion = questions.Count;

                // Tổng số lượng test case có trong test case
                int totalTestCase = questions.Sum(q => q.TestCases.Count);

                // Tổng số điểm của bài thi
                int totalMark = 0;

                // Danh sách các log được tạo ra trong quá trình chấm bài
                List<MarkLog> markLogs = new List<MarkLog>();

                // Danh sách các file jar được nộp
                Dictionary<string, string> jarFiles = new Dictionary<string, string>();

                // Tìm tất cả các file jar trong thư mục nộp bài chứa đuôi Q*.jar
                string[] filePaths = Directory.GetFiles(submittedFolderPath, "Q*.jar", SearchOption.AllDirectories);

                // Sắp xếp lại danh sách file jar theo thứ tự tăng dần để đúng Q1.jar, Q2.jar, Q3.jar, ... theo test case
                Array.Sort(filePaths, StringComparer.CurrentCultureIgnoreCase);

                foreach (string filePath in filePaths)
                {
                    // Lấy tên file
                    string fileName = Path.GetFileName(filePath);

                    // Nếu tên file có định dạng Q*.jar thì thêm vào danh sách file jar
                    if (Regex.IsMatch(fileName, @"^Q\d+\.jar$"))
                    {
                        jarFiles.Add(fileName, filePath);
                    }
                }

                if (jarFiles.Count == 0)
                {
                    markLogs.Add(new MarkLog("No jar file found", "Error"));
                }
                else
                {
                    if (jarFiles.Count != totalQuestion)
                    {
                        markLogs.Add(new MarkLog("Warning: Number of jar files is not equal to number of questions", "Warning"));
                        markLogs.Add(new MarkLog("Number of jar files: " + jarFiles.Count, "Warning"));
                        markLogs.Add(new MarkLog("Jar files: " + string.Join(", ", jarFiles.Keys), "Warning"));
                        markLogs.Add(new MarkLog("Number of questions: " + totalQuestion, "Warning"));
                    }
                    // Duyệt qua từng câu hỏi trong test case (cũng chính là duyệt qua từng file jar)
                    for (int i = 0; i < totalQuestion; i++)
                    {
                        // Lấy ra đường dẫn của file jar được nộp theo câu hỏi
                        string submittedJarFile = jarFiles.ContainsKey("Q" + (i + 1) + ".jar") ? jarFiles["Q" + (i + 1) + ".jar"] : "";

                        // Nếu không tìm thấy file jar thì bỏ qua câu hỏi này
                        if (!File.Exists(submittedJarFile))
                        {
                            markLogs.Add(new MarkLog("No jar file found in Q" + (i + 1) + ".jar", "Error"));
                            continue;
                        }
                        else
                        {
                            // Nếu tìm thấy file jar thì bắt đầu chấm điểm
                            markLogs.Add(new MarkLog("Q" + (i + 1) + ".jar found. Start marking Q" + (i + 1) + ".jar"));

                            // Duyệt qua từng test case trong câu hỏi
                            for (int j = 0; j < questions[i].TestCases.Count; j++)
                            {
                                // Lấy ra test case
                                TestCase testCase = questions[i].TestCases[j];

                                // Chấm điểm trên từng test case
                                float markOnATestcase = MarkOnATesecase(submittedJarFile, testCase, questions[i], ref markLogs);

                                // Tổng điểm của bài thi
                                totalMark += (int)markOnATestcase;
                            }
                        }
                    }
                }

                // sau khi chấm điểm xong thì lưu lại kết quả chấm điểm vào database
                if (examId > 0 && studentId > 0)
                {
                    try
                    {
                        if (context == null)
                        {
                            context = _context;
                        }
                        // nếu context đã bị dispose thì lấy context mới
                        var examStudent = context.ExamStudents.FirstOrDefault(x => x.ExamId == examId && x.StudentId == studentId);
                        if (examStudent != null)
                        {
                            examStudent.Score = totalMark;
                            examStudent.MarkLog = JsonConvert.SerializeObject(markLogs);
                            context.ExamStudents.Update(examStudent);
                            context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        markLogs.Add(new MarkLog(ex.Message));
                    }
                }
                var response = new
                {
                    StudentId = studentId,
                    ExamId = examId,
                    FolderMark = submittedFolderPath,
                    FolderTestCase = testCaseDirectory,
                    TotalMark = totalMark,
                    MarkSummary = new MarkSummary()
                    {
                        MarkLogs = markLogs,
                        Score = totalMark,
                        TotalQuestion = totalQuestion,
                        TotalTestCase = totalTestCase
                    },
                };
                serviceResponse.OnSuccess(response);
            }
            catch (Exception ex)
            {
                serviceResponse.OnError(ex);
            }
            return serviceResponse;
        }

        /// <summary>
        /// Hàm này dùng để normalize text thành chuỗi ký tự không có khoảng trắng, xuống dòng, tab
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string NormalizeTextToSequence(string text)
        {
            string result = text;
            result = Regex.Replace(result, @"\s+", " ").Trim();
            result = Regex.Replace(result, @"\r\n?|\n", " ").Trim();
            result = Regex.Replace(result, @"\t", " ").Trim();
            result = Regex.Replace(result, @"\s+", " ").Trim();
            return result;
        }

        /// <summary>
        /// Hàm này dùng để chấm điểm trên một test case
        /// </summary>
        /// <param name="submittedJarFile">Đường dẫn của file jar được nộp </param>
        /// <param name="testCase">Test case </param>
        /// <param name="question">Câu hỏi </param>
        /// <param name="log">Danh sách các log được tạo ra trong quá trình chấm bài </param>
        /// <returns></returns>
        private float MarkOnATesecase(string submittedJarFile, TestCase testCase, Question question, ref List<MarkLog> log)
        {
            // Tạo ra một đối tượng MarkResult để lưu kết quả chấm điểm của test case
            MarkResult markResult = new MarkResult()
            {
                QuestionId = question.QuestionId,
                TestCaseId = testCase.TestCaseId,
                InputValue = testCase?.InputValue?.Trim(),
                ExpectedValue = testCase?.Expected?.Trim(),
                Score = 0.0f
            };

            try
            {
                // Tạo ra một đối tượng Process để chạy file jar
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "java";
                startInfo.Arguments = "-jar " + submittedJarFile;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                // Tạo ra một đối tượng Stopwatch để đo thời gian chạy của file jar
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                // Chạy file jar
                process.StartInfo = startInfo;
                process.Start();

                // Kiểm tra xem test case có phải là kiểu array hay không để xử lý input
                if (testCase.InputType.Contains("array"))
                {
                    // Nếu là kiểu array thì tách chuỗi input thành mảng các chuỗi
                    string[] inputArray = Regex.Replace(testCase.InputValue, @"\s+", " ").Trim().Split(' ');

                    // Duyệt qua từng phần tử trong mảng và ghi vào input của file jar
                    foreach (string input in inputArray)
                    {
                        process.StandardInput.WriteLine(input);
                    }
                }
                else
                {
                    // Nếu không phải là kiểu array thì ghi input vào file jar
                    process.StandardInput.WriteLine(testCase.InputValue.Trim());
                }

                // Đóng input của file jar
                process.StandardInput.Flush();
                process.StandardInput.Close();

                // Đọc output của file jar
                string actualOutput = process.StandardOutput.ReadToEnd();

                // Chờ cho đến khi file jar chạy xong
                process.WaitForExit();

                // Lưu thời gian chạy của file jar (đơn vị là milisecond)
                stopWatch.Stop();
                markResult.Time = stopWatch.ElapsedMilliseconds;

                // Kiểm tra xem output của file jar có chứa chuỗi "OUTPUT:" hay không
                string outputPrefix = "OUTPUT:";
                int index = actualOutput.IndexOf(outputPrefix);

                // Nếu có thì lấy ra chuỗi output
                if (index != -1)
                {
                    int startIndex = index + outputPrefix.Length;
                    string extractedValue = actualOutput.Substring(startIndex);

                    // Chuỗi output có thể chứa các ký tự không cần thiết như khoảng trắng, xuống dòng, tab nên cần normalize chuỗi output
                    extractedValue = NormalizeTextToSequence(extractedValue);

                    // Giá trị expected để so sánh với giá trị output
                    markResult.YourOutput = extractedValue;

                    // So sánh giá trị output với giá trị expected
                    if (extractedValue.Equals(testCase.Expected))
                    {
                        // Nếu giá trị output giống giá trị expected thì gán điểm cho test case
                        markResult.Score = testCase.Score;
                    }
                }
                else
                {
                    // Nếu không có thì gán lỗi vào kết quả chấm điểm
                    markResult.Exception = "No output found";
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra thì gán lỗi vào kết quả chấm điểm và ghi log
                markResult.Exception = ex.Message;
            }

            // Ghi log
            log.Add(new MarkLog(markResult));
            return markResult.Score;
        }
    }
}
