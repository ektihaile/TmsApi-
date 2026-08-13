using Microsoft.EntityFrameworkCore;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Repositories;

public class EnrollmentRepository(TmsDbContext db)
    : IEnrollmentRepository
{
    public async Task<bool> ExistsAsync(
        int studentId,
        string courseCode,
        CancellationToken ct)
    {
        return await db.Enrollments
            .AnyAsync(
                e => e.StudentId == studentId
                     && e.Course.Code == courseCode,
                ct);
    }

    public async Task AddAsync(
        Enrollment enrollment,
        CancellationToken ct)
    {
        await db.Enrollments.AddAsync(enrollment, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task<List<Enrollment>> GetByStudentIdAsync(
        int studentId,
        CancellationToken ct)
    {
        return await db.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(ct);
    }
}