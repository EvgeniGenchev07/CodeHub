using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UserContext
    {
        private readonly ApplicationDbContext _context;

        public UserContext(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _context.Users
                .Include(u => u.Courses)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Courses)
                .FirstOrDefaultAsync(u => u.Email == email.ToUpper());
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Courses)
                .ToListAsync();
        }

        public async Task<List<Course>> GetUserCoursesAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Courses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Courses ?? new List<Course>();
        }

        public async Task AddUserToCourseAsync(string userId, int courseId)
        {
            var user = await _context.Users
                .Include(u => u.Courses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var course = await _context.Courses.FindAsync(courseId);

            if (user != null && course != null && !user.Courses.Any(c => c.Id == courseId))
            {
                user.Courses.Add(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveUserFromCourseAsync(string userId, int courseId)
        {
            var user = await _context.Users
                .Include(u => u.Courses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var course = user?.Courses.FirstOrDefault(c => c.Id == courseId);

            if (course != null)
            {
                user.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateUserPointsAsync(string userId, int pointsToAdd)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Points += pointsToAdd;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateUserLevelAsync(string userId, int newLevel)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Level = newLevel;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
    }
}

