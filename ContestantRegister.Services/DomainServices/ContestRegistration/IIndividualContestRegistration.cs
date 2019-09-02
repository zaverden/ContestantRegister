using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Models;

namespace ContestantRegister.Services.ApplicationServices.Reg
{
    public interface ICreateIndividualContestRegistration : IIndividualContestRegistration
    {

    }
    public interface IEditIndividualContestRegistration : IIndividualContestRegistration
    {
        string ParticipantName { get; set; }
    }

    public interface IIndividualContestRegistration : IContestRegistration
    {
        int? Course { get; set; }

        int? Class { get; set; }

        StudentType? StudentType { get; set; }
    }
}
