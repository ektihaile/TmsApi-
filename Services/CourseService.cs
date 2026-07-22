using Microsoft.EntityFrameworkCore;
using TmsApi.Dtos;
using TmsApi.Data;
using TmsApi.Entities;

namespace TmsApi.Services;

public class CourseService(TmsDbContext context, ILogger<CourseService> logger) : ICourseService
{
    public Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct) =>
        context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Code,
                c.Title,
                c.MaxCapacity,
                c.Enrollments.Count))
            .FirstOrDefaultAsync(ct);


    public async Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct)
    {
        var course = new Course
        {
            Code = request.Code,
            Title = request.Title,
            MaxCapacity = request.MaxCapacity
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Created course {CourseId} ({Code})", course.Id, course.Code);

        return (await GetByIdAsync(course.Id, ct))!;
    }


    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct) =>
        await context.Courses
            .AsNoTracking()
            .AnyAsync(c => c.Code == code, ct);


    public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
        PagedRequest request,
        CancellationToken ct)
    {
        const int maxPageSize = 50;
        var pageSize = request.PageSize > 0 ? Math.Min(request.PageSize, maxPageSize) : 10;
        var page = request.Page < 1 ? 1 : request.Page;

        // 2. Filter / Search
        IQueryable<Course> query = context.Courses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.Title, $"%{request.Search}%") ||
                EF.Functions.ILike(c.Code, $"%{request.Search}%"));
        }

    
        var totalCount = await query.CountAsync(ct);

        // 4. Sorting
        IQueryable<Course> sortedQuery = request.OrderBy switch
        {
            "Code" => request.Descending
                ? query.OrderByDescending(c => c.Code)
                : query.OrderBy(c => c.Code),

            "MaxCapacity" => request.Descending
                ? query.OrderByDescending(c => c.MaxCapacity)
                : query.OrderBy(c => c.MaxCapacity),

            _ => request.Descending
                ? query.OrderByDescending(c => c.Title)
                : query.OrderBy(c => c.Title)
        };

        var items = await sortedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Code,
                c.Title,
                c.MaxCapacity,
                c.Enrollments.Count))
            .ToListAsync(ct);

        return new PagedResponse<CourseResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}