using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Controllers
{
    public abstract class BaseController : Controller
    {
        protected async Task<string> GetListItemsJsonAsync<TDB, TVM>(ApplicationDbContext context, IMapper mapper) where TDB : class
        {
            List<TDB> dbList = await context.Set<TDB>().ToListAsync();
            List<TVM> viewModels = mapper.Map<List<TVM>>(dbList);
            return viewModels.ToJson();
        }
    }
}
