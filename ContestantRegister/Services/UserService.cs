using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContestantRegister.Data;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Services
{
    public interface IUserService
    {
        bool IsPupil(ClaimsPrincipal user);
        bool IsStudent(ClaimsPrincipal user);
        bool IsTrainer(ClaimsPrincipal user);

        bool IsPupil(ContestantUser user);
        bool IsStudent(ContestantUser user);
        bool IsTrainer(ContestantUser user);

        Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsPupil(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated) throw new InvalidOperationException("User not authentificated");

            return _context.Users.OfType<Pupil>().Any(u => u.UserName == user.Identity.Name);
        }

        public bool IsStudent(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated) throw new InvalidOperationException("User not authentificated");

            return _context.Users.OfType<Student>().Any(u => u.UserName == user.Identity.Name);
        }

        public bool IsTrainer(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated) throw new InvalidOperationException("User not authentificated");

            return _context.Users.OfType<Trainer>().Any(u => u.UserName == user.Identity.Name);
        }

        public bool IsPupil(ContestantUser user)
        {
            return user is Pupil;
        }

        public bool IsStudent(ContestantUser user)
        {
            return user is Student;
        }

        public bool IsTrainer(ContestantUser user)
        {
            return user is Trainer;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            return await _context.Users.SingleAsync(u => u.UserName == user.Identity.Name);
        }
    }
}
