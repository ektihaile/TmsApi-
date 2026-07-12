using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly TmsDbContext _context;

    public CoursesController(TmsDbContext context)
    {
        _context = context;
    }

    [HttpGet("top")]
public async Task<IActionResult> GetTopCourses(
    CancellationToken cancellationToken)
{
    var topcourses = await _context.Enrollments
        .GroupBy(e => e.Course.Title)
        .Select(g => new
        {
            CourseTitle = g.Key,
            EnrollmentCount = g.Count()
        })
        .OrderByDescending(x => x.EnrollmentCount)
        .Take(5)
        .ToListAsync(cancellationToken);

    return Ok(topcourses);
}
}