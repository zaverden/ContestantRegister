using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ContestantRegister.Models;

namespace ContestantRegister.Services.ApplicationServices.Reg
{
    public interface IContestRegistration
    {
        string Participant1Id { get; set; }

        string TrainerId { get; set; }

        string ManagerId { get; set; }

        int StudyPlaceId { get; set; }

        bool IsProgrammingLanguageNeeded { get; set; }

        string ProgrammingLanguage { get; set; }

        int CityId { get; set; }

        bool IsAreaRequired { get; set; }

        int? ContestAreaId { get; set; }

        string ComputerName { get; set; }

        string YaContestLogin { get; set; }

        string YaContestPassword { get; set; }

        DateTime? RegistrationDateTime { get; set; }

        string RegistredByName { get; set; }

        int? Number { get; set; }

        ContestRegistrationStatus Status { get; set; }

        ParticipantType ParticipantType { get; set; }

        bool IsOutOfCompetition { get; set; }

        bool IsOutOfCompetitionAllowed { get; set; }

        bool IsEnglishLanguage { get; set; }

        int RegistrationId { get; set; }

        string ContestName { get; set; }

        int ContestId { get; set; }

        int ContestTrainerCont { get; set; }

        ContestRegistrationStatus CheckRegistrationStatus();
    }
}
