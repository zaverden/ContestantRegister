using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Users.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;
using OfficeOpenXml;

namespace ContestantRegister.Cqrs.Features.Admin.Users.QueryHandlers
{
    public class GetExportedUsersQueryHandler : ReadRepositoryQueryHandler<GetExportedUsersQuery, ExcelPackage>
    {
        public GetExportedUsersQueryHandler(IRepository repository) : base(repository)
        {
        }

        public override async Task<ExcelPackage> HandleAsync(GetExportedUsersQuery query)
        {
            var users = await ReadRepository.Set<ApplicationUser>()
                .Include(r => r.StudyPlace.City.Region)
                .ToListAsync();

            var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Users");
            worksheet.Cells["A1"].Value = "Email";
            worksheet.Cells["B1"].Value = "EmailConfirmed";
            worksheet.Cells["C1"].Value = "Surname";
            worksheet.Cells["D1"].Value = "Name";
            worksheet.Cells["E1"].Value = "Patronymic";
            worksheet.Cells["F1"].Value = "FirstName";
            worksheet.Cells["G1"].Value = "LastName";
            worksheet.Cells["H1"].Value = "UserType";
            worksheet.Cells["I1"].Value = "StudyPlace";
            worksheet.Cells["J1"].Value = "StudyPlaceType";
            worksheet.Cells["K1"].Value = "City";
            worksheet.Cells["L1"].Value = "Region";
            worksheet.Cells["M1"].Value = "StudentType";

            int row = 1;
            foreach (var user in users)
            {
                row++;

                worksheet.Cells[row, 1].Value = user.Email;
                worksheet.Cells[row, 2].Value = user.EmailConfirmed;

                worksheet.Cells[row, 3].Value = user.Surname;
                worksheet.Cells[row, 4].Value = user.Name;
                worksheet.Cells[row, 5].Value = user.Patronymic;
                worksheet.Cells[row, 6].Value = user.FirstName;
                worksheet.Cells[row, 7].Value = user.LastName;

                worksheet.Cells[row, 8].Value = user.UserType;
                worksheet.Cells[row, 9].Value = user.StudyPlace.ShortName;
                worksheet.Cells[row, 10].Value = user.StudyPlace is School ? "School" : "Institution";
                worksheet.Cells[row, 11].Value = user.StudyPlace.City.Name;
                worksheet.Cells[row, 12].Value = user.StudyPlace.City.Region.Name;
                worksheet.Cells[row, 13].Value = user.StudentType;
            }

            return package;
        }
    }
}
