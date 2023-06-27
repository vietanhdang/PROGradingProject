using Common.Models;

namespace BusinessLogic
{
    public interface IAutoMarkService
    {
        public ServiceResponse Mark(string submittedFolderPath, string testCaseDirectory, int examId, int studentId);
    }
}
