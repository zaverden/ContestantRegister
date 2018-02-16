using System;
using System.Linq;
using System.Security.Claims;
using ContestantRegister.Data;
using ContestantRegister.Models;

namespace ContestantRegister.Services
{
    public interface IUserService
    {
        bool IsPupil(ClaimsPrincipal user);
        bool IsStudent(ClaimsPrincipal user);
        bool IsTrainer(ClaimsPrincipal user);
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
    }
}
