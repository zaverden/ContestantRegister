using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;
using OfficeOpenXml;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.QueryHandlers
{
    public class GetExportedTeamContestParticipantsQueryHandler : ReadRepositoryQueryHandler<GetExportedTeamContestParticipantsQuery, ExcelPackage>
    {
        public GetExportedTeamContestParticipantsQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<ExcelPackage> HandleAsync(GetExportedTeamContestParticipantsQuery query)
        {
            var contestExists = await ReadRepository.Set<Contest>()
                .AnyAsync(x => x.Id == query.ContestId);
            if (!contestExists) throw new EntityNotFoundException();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Participants");

            var registrations = await ReadRepository.Set<TeamContestRegistration>()
                .Include(r => r.Participant1)
                .Include(r => r.Participant2)
                .Include(r => r.Participant3)
                .Include(r => r.ReserveParticipant)
                .Include(r => r.Trainer)
                .Include(r => r.Manager)
                .Include(r => r.StudyPlace)
                .Include(r => r.StudyPlace.City)
                .Include(r => r.StudyPlace.City.Region)
                .Where(r => r.ContestId == query.ContestId)
                .ToListAsync();

            worksheet.Cells["A1"].Value = "Email";
            worksheet.Cells["B1"].Value = "DisplayTeamName";
            worksheet.Cells["C1"].Value = "FirstName";
            worksheet.Cells["D1"].Value = "LastName";
            worksheet.Cells["E1"].Value = "Surname";
            worksheet.Cells["F1"].Value = "Name";
            worksheet.Cells["G1"].Value = "Patronymic";
            worksheet.Cells["H1"].Value = "StudyPlace_FullName";
            worksheet.Cells["I1"].Value = "StudyPlace_FullName_En";
            worksheet.Cells["J1"].Value = "StudyPlace_BaylorFullName";
            worksheet.Cells["K1"].Value = "IsOutOfCompetition";
            worksheet.Cells["L1"].Value = "DateOfBirth";
            worksheet.Cells["M1"].Value = "EducationStartDate";
            worksheet.Cells["N1"].Value = "EducationEndDate";
            worksheet.Cells["O1"].Value = "City";
            worksheet.Cells["P1"].Value = "Role";
            worksheet.Cells["Q1"].Value = "PhoneNumber";
            worksheet.Cells["R1"].Value = "IsBaylorRegistrationCompleted";
            worksheet.Cells["S1"].Value = "StudentType";
            worksheet.Cells["T1"].Value = "Region";
            worksheet.Cells["U1"].Value = "TeamRegistrationStatus";

            var row = 2;
            foreach (var registration in registrations)
            {
                if (registration.Participant1 != null)
                {
                    row = AddTeamMember(worksheet, registration.Participant1, row, registration, "Contestant");
                }

                if (registration.Participant2 != null)
                {
                    row = AddTeamMember(worksheet, registration.Participant2, row, registration, "Contestant");
                }

                if (registration.Participant3 != null)
                {
                    row = AddTeamMember(worksheet, registration.Participant3, row, registration, "Contestant");
                }

                if (registration.ReserveParticipant != null)
                {
                    row = AddTeamMember(worksheet, registration.ReserveParticipant, row, registration, "Reserve");
                }

                row = AddTeamMember(worksheet, registration.Trainer, row, registration, "Coach");
            }

            return package;
        }

        private int AddTeamMember(ExcelWorksheet worksheet, ApplicationUser user, int row, TeamContestRegistration registration, string role)
        {
            worksheet.Cells[row, 1].Value = user.Email;
            worksheet.Cells[row, 2].Value = registration.DisplayTeamName;
            worksheet.Cells[row, 3].Value = user.FirstName;
            worksheet.Cells[row, 4].Value = user.LastName;
            worksheet.Cells[row, 5].Value = user.Surname;
            worksheet.Cells[row, 6].Value = user.Name;
            worksheet.Cells[row, 7].Value = user.Patronymic;
            worksheet.Cells[row, 8].Value = registration.StudyPlace.FullName;
            if (registration.StudyPlace is Institution institution)
            {
                worksheet.Cells[row, 9].Value = institution.FullNameEnglish;
                worksheet.Cells[row, 10].Value = string.IsNullOrEmpty(institution.BaylorFullName) ? institution.FullNameEnglish : institution.BaylorFullName;
            }
            worksheet.Cells[row, 11].Value = registration.IsOutOfCompetition;
            worksheet.Cells[row, 12].Value = user.DateOfBirth.HasValue ? user.DateOfBirth.Value.ToString("dd.MM.yyyy") : string.Empty;
            worksheet.Cells[row, 13].Value = user.EducationStartDate.HasValue ? user.EducationStartDate.Value.ToString("dd.MM.yyyy") : string.Empty;
            worksheet.Cells[row, 14].Value = user.EducationEndDate.HasValue ? user.EducationEndDate.Value.ToString("dd.MM.yyyy") : string.Empty;
            worksheet.Cells[row, 15].Value = registration.StudyPlace.City.Name;
            worksheet.Cells[row, 16].Value = role;
            worksheet.Cells[row, 17].Value = user.PhoneNumber;
            worksheet.Cells[row, 18].Value = user.IsBaylorRegistrationCompleted;
            worksheet.Cells[row, 19].Value = user.StudentType ?? StudentType.Student;
            worksheet.Cells[row, 20].Value = registration.StudyPlace.City.Region?.Name;
            worksheet.Cells[row, 21].Value = registration.Status;

            return row + 1;
        }
    }
}
