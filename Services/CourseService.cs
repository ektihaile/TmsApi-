using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TmsApi.Data;
using TmsApi.Entities;

namespace TmsApi.Services;

public class CourseService(TmsDbContext context, ILogger<CourseService> logger) : ICourseService
{
    // TODO 1: ኮርስ በ ID መፈለግ (AsNoTracking በመጠቀም ፈጣን እንዲሆን ተደርጓል)
    public async Task<Course?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    // TODO 2: አዲስ ኮርስ ወደ ዳታቤዝ መጨመርና ሴቭ ማድረግ
    public async Task<Course> CreateAsync(Course course, CancellationToken ct)
    {
        context.Courses.Add(course);
        await context.SaveChangesAsync(ct);
        
        logger.LogInformation("አዲስ ኮርስ በስኬት ተመዝግቧል! ID: {Id}", course.Id);
        
        return course;
    }
}