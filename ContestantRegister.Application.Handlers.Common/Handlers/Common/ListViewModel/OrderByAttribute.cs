using System;

namespace ContestantRegister.Cqrs.Features._Common.ListViewModel
{
    public class OrderByAttribute : Attribute
    {
        public bool IsDesc { get; set; }
    }
}
