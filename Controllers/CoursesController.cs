using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TmsApi.Dtos;
using TmsApi.Services;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/courses")]
[Tags("Courses")] 
public class CoursesController(
    ICourseService courseService,
    LinkGenerator linkGenerator) : ControllerBase
{

    [HttpGet("{id:int}", Name = nameof(GetCourseById))]
    [EndpointSummary("Get course details with HATEOAS links")]
    [EndpointDescription("Fetches full details of a course by ID and appends standard and conditional HATEOAS links.")]
    [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseById(int id, CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(id, ct);
        if (course is null)
        {
            return NotFound();
        }

      
        var selfUrl = linkGenerator.GetPathByName(HttpContext, nameof(GetCourseById), new { id })
                      ?? $"/api/courses/{id}";

        var enrollmentsUrl = linkGenerator.GetPathByName(HttpContext, "ListCourseEnrollments", new { courseId = id })
                             ?? $"/api/courses/{id}/enrollments";

       
        var links = new List<LinkDto>
        {
            new(selfUrl, "self", "GET"),
            new(selfUrl, "update", "PUT"),
            new(selfUrl, "delete", "DELETE"),
            new(enrollmentsUrl, "enrollments", "GET")
        };

        
        if (course.EnrollmentCount < course.MaxCapacity)
        {
            links.Add(new LinkDto(enrollmentsUrl, "enroll", "POST"));
        }

        
        var detailDto = new CourseDetailDto
        {
            Id = course.Id,
            Code = course.Code,
            Title = course.Title,
            MaxCapacity = course.MaxCapacity,
            EnrollmentCount = course.EnrollmentCount,
            Links = links
        };

        return Ok(detailDto);
    }

    
    [HttpPost]
    [EndpointSummary("Create a new course")]
    [EndpointDescription("Registers a new course if the code does not already exist.")]
    [ProducesResponseType(typeof(CourseResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CourseResponseDto>> CreateCourse(CreateCourseRequest request, CancellationToken ct)
    {
        if (await courseService.CodeExistsAsync(request.Code, ct))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course code already exists",
                Detail = $"A course with code '{request.Code}' is already registered.",
                Status = StatusCodes.Status409Conflict,
                Instance = HttpContext.Request.Path
            });
        }

        var result = await courseService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, result);
    }

    
   
    [HttpGet]
    [EndpointSummary("Get paginated list of courses")]
    [EndpointDescription("Retrieves courses using page and pageSize query parameters.")]
    [ProducesResponseType(typeof(PagedResponse<CourseResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(
        [FromQuery] PagedRequest request,
        CancellationToken ct)
    {
        var result = await courseService.GetCoursesAsync(request, ct);

        return Ok(result);
    }
}

