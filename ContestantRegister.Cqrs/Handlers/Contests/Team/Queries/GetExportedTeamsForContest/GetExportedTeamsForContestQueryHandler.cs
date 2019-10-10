using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Framework.Extensions;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;
using OfficeOpenXml;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.QueryHandlers
{
    internal class GetExportedTeamsForContestQueryHandler : ReadRepositoryQueryHandler<GetExportedTeamsForContestQuery, ExportTeamsResult>
    {
        public GetExportedTeamsForContestQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<ExportTeamsResult> HandleAsync(GetExportedTeamsForContestQuery forContestQuery)
        {
            var contestName = await ReadRepository.Set<Contest>()
                .Where(x => x.Id == forContestQuery.ContestId)
                .Select(x => x.Name)
                .SingleOrDefaultAsync();

            if (contestName.IsEmpty()) throw new EntityNotFoundException();

            var registrations = await ReadRepository.Set<TeamContestRegistration>()
                .Include(r => r.Contest)
                .Include(r => r.StudyPlace.City.Region)
                .Include(r => r.Participant1)
                .Include(r => r.Participant2)
                .Include(r => r.Participant3)
                .Include(r => r.Trainer)
                .Include(r => r.Trainer2)
                .Include(r => r.Trainer3)
                .Include(r => r.Manager)
                .Include(r => r.ContestArea.Area)
                .Where(r => r.ContestId == forContestQuery.ContestId)
                .OrderBy(r => r.Number)
                .ToListAsync();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Teams");

            worksheet.Cells["A1"].Value = "Area";
            worksheet.Cells["B1"].Value = "StudyPlace";
            worksheet.Cells["C1"].Value = "TeamName";
            worksheet.Cells["D1"].Value = "Status";

            worksheet.Cells["E1"].Value = "Email1";
            worksheet.Cells["F1"].Value = "Surname1";
            worksheet.Cells["G1"].Value = "Name1";
            worksheet.Cells["H1"].Value = "Patronymic1";

            worksheet.Cells["I1"].Value = "Email2";
            worksheet.Cells["J1"].Value = "Surname2";
            worksheet.Cells["K1"].Value = "Name2";
            worksheet.Cells["L1"].Value = "Patronymic2";

            worksheet.Cells["M1"].Value = "Email3";
            worksheet.Cells["N1"].Value = "Surname3";
            worksheet.Cells["O1"].Value = "Name3";
            worksheet.Cells["P1"].Value = "Patronymic3";

            worksheet.Cells["Q1"].Value = "TrainerEmail";
            worksheet.Cells["R1"].Value = "TrainerSurname";
            worksheet.Cells["S1"].Value = "TrainerName";
            worksheet.Cells["T1"].Value = "TrainerPatronymic";

            worksheet.Cells["U1"].Value = "ManagerEmail";
            worksheet.Cells["V1"].Value = "ManagerSurname";
            worksheet.Cells["W1"].Value = "ManagerName";
            worksheet.Cells["X1"].Value = "ManagerPatronymic";

            worksheet.Cells["Y1"].Value = "Region";
            worksheet.Cells["Z1"].Value = "City";
            worksheet.Cells["AA1"].Value = "YaContestLogin";
            worksheet.Cells["AB1"].Value = "YaContestPassword";

            worksheet.Cells["AC1"].Value = "Number";
            worksheet.Cells["AD1"].Value = "ComputerName";
            worksheet.Cells["AE1"].Value = "ProgrammingLanguage";
            worksheet.Cells["AF1"].Value = "OfficialTeamName";
            worksheet.Cells["AG1"].Value = "DisplayTeamName";
            worksheet.Cells["AH1"].Value = "StudyPlace_FullName";
            worksheet.Cells["AI1"].Value = "StudyPlace_ShortName_En";
            worksheet.Cells["AJ1"].Value = "StudyPlace_FullName_En";
            worksheet.Cells["AK1"].Value = "IsOutOfCompetition";

            worksheet.Cells["AL1"].Value = "FirstName1";
            worksheet.Cells["AM1"].Value = "LastName1";

            worksheet.Cells["AN1"].Value = "FirstName2";
            worksheet.Cells["AO1"].Value = "LastName2";

            worksheet.Cells["AP1"].Value = "FirstName3";
            worksheet.Cells["AQ1"].Value = "LastName3";

            worksheet.Cells["AR1"].Value = "TrainerFirstName";
            worksheet.Cells["AS1"].Value = "TrainerLastName";

            worksheet.Cells["AT1"].Value = "ManagerFirstName";
            worksheet.Cells["AU1"].Value = "ManagerLastName";

            worksheet.Cells["AV1"].Value = "StudyPlace_BaylorFullName";

            worksheet.Cells["AW1"].Value = "DateOfBirth1";
            worksheet.Cells["AX1"].Value = "EducationStartDate1";
            worksheet.Cells["AY1"].Value = "EducationEndDate1";

            worksheet.Cells["AZ1"].Value = "DateOfBirth2";
            worksheet.Cells["BA1"].Value = "EducationStartDate2";
            worksheet.Cells["BB1"].Value = "EducationEndDate2";

            worksheet.Cells["BC1"].Value = "DateOfBirth3";
            worksheet.Cells["BD1"].Value = "EducationStartDate3";
            worksheet.Cells["BE1"].Value = "EducationEndDate3";

            worksheet.Cells["BF1"].Value = "Trainer2Email";
            worksheet.Cells["BG1"].Value = "Trainer2Surname";
            worksheet.Cells["BH1"].Value = "Trainer2Name";
            worksheet.Cells["BI1"].Value = "Trainer2Patronymic";
            worksheet.Cells["BJ1"].Value = "Trainer2FirstName";
            worksheet.Cells["BK1"].Value = "Trainer2LastName";

            worksheet.Cells["BL1"].Value = "Trainer3Email";
            worksheet.Cells["BM1"].Value = "Trainer3Surname";
            worksheet.Cells["BN1"].Value = "Trainer3Name";
            worksheet.Cells["BO1"].Value = "Trainer3Patronymic";
            worksheet.Cells["BP1"].Value = "Trainer3FirstName";
            worksheet.Cells["BQ1"].Value = "Trainer3LastName";

            int row = 1;
            foreach (var registration in registrations)
            {
                row++;

                worksheet.Cells[row, 1].Value = registration.ContestArea?.Area.Name;
                worksheet.Cells[row, 2].Value = registration.StudyPlace.ShortName;
                worksheet.Cells[row, 3].Value = registration.TeamName;
                worksheet.Cells[row, 4].Value = registration.Status;

                if (registration.Participant1 != null)
                {
                    worksheet.Cells[row, 5].Value = registration.Participant1.Email;
                    worksheet.Cells[row, 6].Value = registration.Participant1.Surname;
                    worksheet.Cells[row, 7].Value = registration.Participant1.Name;
                    worksheet.Cells[row, 8].Value = registration.Participant1.Patronymic;

                    worksheet.Cells[row, 49].Value = registration.Participant1.DateOfBirth.HasValue ? registration.Participant1.DateOfBirth.Value.ToString("dd.MM.yyyy") : string.Empty;
                    worksheet.Cells[row, 50].Value = registration.Participant1.EducationStartDate.HasValue ? registration.Participant1.EducationStartDate.Value.ToString("dd.MM.yyyy") : string.Empty;
                    worksheet.Cells[row, 51].Value = registration.Participant1.EducationEndDate.HasValue ? registration.Participant1.EducationEndDate.Value.ToString("dd.MM.yyyy") : string.Empty;
                }

                if (registration.Participant2 != null)
                {
                    worksheet.Cells[row, 9].Value = registration.Participant2.Email;
                    worksheet.Cells[row, 10].Value = registration.Participant2.Surname;
                    worksheet.Cells[row, 11].Value = registration.Participant2.Name;
                    worksheet.Cells[row, 12].Value = registration.Participant2.Patronymic;

                    worksheet.Cells[row, 52].Value = registration.Participant2.DateOfBirth.HasValue ? registration.Participant2.DateOfBirth.Value.ToString("dd.MM.yyyy") : string.Empty;
                    worksheet.Cells[row, 53].Value = registration.Participant2.EducationStartDate.HasValue ? registration.Participant2.EducationStartDate.Value.ToString("dd.MM.yyyy") : string.Empty;
                    worksheet.Cells[row, 54].Value = registration.Participant2.EducationEndDate.HasValue ? registration.Participant2.EducationEndDate.Value.ToString("dd.MM.yyyy") : string.Empty;
                }

                if (registration.Participant3 != null)
                {
                    worksheet.Cells[row, 13].Value = registration.Participant3.Email;
                    worksheet.Cells[row, 14].Value = registration.Participant3.Surname;
                    worksheet.Cells[row, 15].Value = registration.Participant3.Name;
                    worksheet.Cells[row, 16].Value = registration.Participant3.Patronymic;

                    worksheet.Cells[row, 55].Value = registration.Participant3.DateOfBirth.HasValue ? registration.Participant3.DateOfBirth.Value.ToString("dd.MM.yyyy") : string.Empty;
                    worksheet.Cells[row, 56].Value = registration.Participant3.EducationStartDate.HasValue ? registration.Participant3.EducationStartDate.Value.ToString("dd.MM.yyyy") : string.Empty;
                    worksheet.Cells[row, 57].Value = registration.Participant3.EducationEndDate.HasValue ? registration.Participant3.EducationEndDate.Value.ToString("dd.MM.yyyy") : string.Empty;
                }

                worksheet.Cells[row, 17].Value = registration.Trainer.Email;
                worksheet.Cells[row, 18].Value = registration.Trainer.Surname;
                worksheet.Cells[row, 19].Value = registration.Trainer.Name;
                worksheet.Cells[row, 20].Value = registration.Trainer.Patronymic;

                if (registration.Manager != null)
                {
                    worksheet.Cells[row, 21].Value = registration.Manager.Email;
                    worksheet.Cells[row, 22].Value = registration.Manager.Surname;
                    worksheet.Cells[row, 23].Value = registration.Manager.Name;
                    worksheet.Cells[row, 24].Value = registration.Manager.Patronymic;
                }

                worksheet.Cells[row, 25].Value = registration.StudyPlace.City.Region.Name;
                worksheet.Cells[row, 26].Value = registration.StudyPlace.City.Name;
                worksheet.Cells[row, 27].Value = registration.YaContestLogin;
                worksheet.Cells[row, 28].Value = registration.YaContestPassword;

                worksheet.Cells[row, 29].Value = registration.Number;
                worksheet.Cells[row, 30].Value = registration.ComputerName;
                worksheet.Cells[row, 31].Value = registration.ProgrammingLanguage;
                worksheet.Cells[row, 32].Value = registration.OfficialTeamName;
                worksheet.Cells[row, 33].Value = registration.DisplayTeamName;
                worksheet.Cells[row, 34].Value = registration.StudyPlace.FullName;
                if (registration.StudyPlace is Institution institution)
                {
                    worksheet.Cells[row, 35].Value = institution.ShortNameEnglish;
                    worksheet.Cells[row, 36].Value = institution.FullNameEnglish;
                    worksheet.Cells[row, 48].Value = string.IsNullOrEmpty(institution.BaylorFullName) ? institution.FullNameEnglish : institution.BaylorFullName;
                }

                worksheet.Cells[row, 37].Value = registration.IsOutOfCompetition;

                if (registration.Contest.ParticipantType == ParticipantType.Student && registration.Contest.IsEnglishLanguage)
                {
                    worksheet.Cells[row, 38].Value = registration.Participant1?.FirstName;
                    worksheet.Cells[row, 39].Value = registration.Participant1?.LastName;

                    worksheet.Cells[row, 40].Value = registration.Participant2?.FirstName;
                    worksheet.Cells[row, 41].Value = registration.Participant2?.LastName;

                    worksheet.Cells[row, 42].Value = registration.Participant3?.FirstName;
                    worksheet.Cells[row, 43].Value = registration.Participant3?.LastName;

                    worksheet.Cells[row, 44].Value = registration.Trainer.FirstName;
                    worksheet.Cells[row, 45].Value = registration.Trainer.LastName;

                    worksheet.Cells[row, 46].Value = registration.Manager?.FirstName;
                    worksheet.Cells[row, 47].Value = registration.Manager?.LastName;

                }

                //48 колонка занята

                worksheet.Cells[row, 58].Value = registration.Trainer2?.Email;
                worksheet.Cells[row, 59].Value = registration.Trainer2?.Surname;
                worksheet.Cells[row, 60].Value = registration.Trainer2?.Name;
                worksheet.Cells[row, 61].Value = registration.Trainer2?.Patronymic;
                worksheet.Cells[row, 62].Value = registration.Trainer2?.FirstName;
                worksheet.Cells[row, 63].Value = registration.Trainer2?.LastName;

                worksheet.Cells[row, 64].Value = registration.Trainer3?.Email;
                worksheet.Cells[row, 65].Value = registration.Trainer3?.Surname;
                worksheet.Cells[row, 66].Value = registration.Trainer3?.Name;
                worksheet.Cells[row, 67].Value = registration.Trainer3?.Patronymic;
                worksheet.Cells[row, 68].Value = registration.Trainer3?.FirstName;
                worksheet.Cells[row, 69].Value = registration.Trainer3?.LastName;
            }

            return new ExportTeamsResult
            {
                ContestName = contestName,
                ExcelPackage = package
            };
        }
    }
}
