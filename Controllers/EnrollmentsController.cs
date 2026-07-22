using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Dtos;
using TmsApi.Services;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/enrollments")]
[Tags("Enrollments")]
public class EnrollmentsController(
    IEnrollmentService enrollmentService,
    ICourseService courseService) : ControllerBase
{
    [HttpGet(Name = "ListCourseEnrollments")]
    public async Task<IActionResult> GetEnrollments(
        int courseId,
        CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(courseId, ct);
        if (course is null)
            return NotFound();

        var result = await enrollmentService.GetByCourseAsync(courseId, ct);
        return Ok(result);
    }

    // 2. [HttpGet("{id:int}")] ተጨምሯል
    [HttpGet("{id:int}", Name = nameof(GetEnrollment))]
    public async Task<IActionResult> GetEnrollment(
        int courseId,
        int id,
        CancellationToken ct)
    {
        var result = await enrollmentService.GetByIdAsync(courseId, id, ct);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnrollment(
        int courseId,
        [FromBody] EnrollStudentRequest request,
        CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(courseId, ct);
        if (course is null)
            return NotFound();

        var result = await enrollmentService.CreateAsync(courseId, request, ct);

        return CreatedAtAction(
            nameof(GetEnrollment),
            new { courseId = courseId, id = result.Id },
            result);
    }
}