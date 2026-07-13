using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController(
    IEnrollmentService enrollmentService,
    TmsDbContext context) : ControllerBase
{
    private readonly TmsDbContext _context = context;


    // GET /api/enrollments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enrollments = await enrollmentService.GetAllAsync();
        return Ok(enrollments);
    }


    // GET /api/enrollments/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var record = await enrollmentService.GetByIdAsync(id);

        return record is not null 
            ? Ok(record) 
            : NotFound();
    }


    // POST /api/enrollments
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEnrollmentRequest request)
    {
        var record = await enrollmentService.EnrollAsync(
            request.StudentId,
            request.CourseCode);


        if (record is null)
        {
            return BadRequest("Enrollment failed.");
        }


        return CreatedAtAction(
            nameof(GetById),
            new { id = record.Id },
            record);
    }


    // DELETE /api/enrollments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await enrollmentService.DeleteAsync(id);

        return deleted 
            ? NoContent() 
            : NotFound();
    }



    // Exercise 7 - Part A
    // Intentional N+1 query
    [HttpGet("nplus1")]
    public async Task<IActionResult> NPlusOne(
        CancellationToken cancellationToken)
    {
        var students = await _context.Students
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        var result = new List<object>();


        foreach (var s in students)
        {
            var count = await _context.Enrollments
                .AsNoTracking()
                .CountAsync(
                    e => e.StudentId == s.Id,
                    cancellationToken);


            result.Add(new
            {
                s.Name,
                EnrollmentCount = count
            });
        }


        return Ok(result);
    }



    // Exercise 7 - Part B
    // Single SQL query
    [HttpGet("optimized")]
    public async Task<IActionResult> Optimized(
        CancellationToken cancellationToken)
    {
        var report = await _context.Students
            .AsNoTracking()
            .Select(s => new
            {
                s.Name,
                EnrollmentCount = s.Enrollments.Count
            })
            .ToListAsync(cancellationToken);


        return Ok(report);
    }



    // Exercise 9 - Bulk archive
    [HttpPost("archive-old")]
    public async Task<IActionResult> ArchiveOldEnrollments(
        CancellationToken cancellationToken)
    {

        var cutoffYear = 2025;


        var updated = await _context.Enrollments
            .Where(e => e.Year < cutoffYear && !e.IsArchived)
            .ExecuteUpdateAsync(
                s => s.SetProperty(
                    e => e.IsArchived,
                    true),
                cancellationToken);



        return Ok(new
        {
            Archived = updated
        });
    }



    // Admin restore example
    [HttpGet("all-including-archived")]
    public async Task<IActionResult> GetAllIncludingArchived(
        CancellationToken cancellationToken)
    {
        var enrollments = await _context.Enrollments
            .IgnoreQueryFilters()
            .ToListAsync(cancellationToken);


        return Ok(enrollments);
    }
}



public record CreateEnrollmentRequest(
    string StudentId,
    string CourseCode);