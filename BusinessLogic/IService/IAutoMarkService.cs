using Common.Models;
using DataAccess.DatabaseContext;

namespace BusinessLogic
{
    public interface IAutoMarkService
    {
        public ServiceResponse Mark(AppDbContext context, string submittedFolderPath, string testCaseDirectory, int examId, int studentId);

    }
}
