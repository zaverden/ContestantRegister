using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Controllers._Common.Commands;

namespace ContestantRegister.Cqrs.Features._Common.Commands
{
    public class EditMappedEntityCommand<TEntity, TViewModel> : EditEntityCommand<TViewModel>
    {
        public int Id { get; set; }
    }
}
