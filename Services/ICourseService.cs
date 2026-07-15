using System.Threading;
using System.Threading.Tasks;
using TmsApi.Entities;

namespace TmsApi.Services;

public interface ICourseService
{
    // የኮርሱን መረጃ በ ID ለመፈለግ
    Task<Course?> GetByIdAsync(int id, CancellationToken ct);
    
    // አዲስ ኮርስ ለመፍጠር
    Task<Course> CreateAsync(Course course, CancellationToken ct);
}