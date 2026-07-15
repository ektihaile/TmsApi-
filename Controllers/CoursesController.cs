using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Entities;
using TmsApi.Services;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    // TODO 3: GET /api/courses/{id}
    [HttpGet("{id:int}", Name = nameof(GetCourseById))]
    public async Task<ActionResult<Course>> GetCourseById(int id, CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(id, ct);
        
        if (course == null)
        {
            return NotFound(); // ኮርሱ ካልተገኘ 404 Not Found ይመልሳል
        }
        
        return Ok(course); // ከተገኘ 200 OK ከመረጃው ጋር ይመልሳል
    }

    // TODO 4: POST /api/courses
    [HttpPost]
    public async Task<ActionResult<Course>> CreateCourse(Course course, CancellationToken ct)
    {
        var result = await courseService.CreateAsync(course, ct);
        
        // ስኬታማ ሲሆን 201 Created ይመልሳል፤ እንዲሁም Location ራስጌ ላይ አዲሱን የኮርስ መፈለጊያ አድራሻ ያዘጋጃል
        return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, result);
    }
}