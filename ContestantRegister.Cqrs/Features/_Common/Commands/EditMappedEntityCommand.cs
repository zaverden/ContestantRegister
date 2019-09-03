using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Controllers._Common.Commands;

namespace ContestantRegister.Cqrs.Features._Common.Commands
{
    public class EditMappedEntityCommand<TEntity, TViewModel, TKey> : EditEntityCommand<TViewModel>
    {
        public TKey Id { get; set; }
    }
}
