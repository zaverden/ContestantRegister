using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.ListItem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Controllers
{
    public abstract class BaseController : Controller
    {
        protected async Task<List<TVM>> GetListItemsAsync<TDB, TVM>(ApplicationDbContext context, IMapper mapper) where TDB : class
        {
            List<TDB> dbList = await context.Set<TDB>().ToListAsync();
            List<TVM> viewModels = mapper.Map<List<TVM>>(dbList);
            return viewModels;
        }

        protected List<TViewModel> GetListItems<TItem, TViewModel>(IEnumerable<TItem> items, IMapper mapper, int? selectedId = null) 
            where TItem : DomainObject
            where TViewModel : SelectedListItemViewModelBase
        {
            List<TViewModel> viewModels = mapper.Map<List<TViewModel>>(items);
            if (selectedId.HasValue)
            {
                foreach (var viewModel in viewModels.Where(vm => vm.Value == selectedId.Value))
                {
                    viewModel.Selected = true;
                }
            }
            return viewModels;
        }

        protected List<TViewModel> GetListItems<TItem, TViewModel>(IEnumerable<TItem> items, IMapper mapper, int[] selectedIds = null)
            where TItem : DomainObject
            where TViewModel : SelectedListItemViewModelBase
        {
            List<TViewModel> viewModels = mapper.Map<List<TViewModel>>(items);
            if (selectedIds != null)
            {
                foreach (var viewModel in viewModels.Where(vm => selectedIds.Contains(vm.Value)))
                {
                    viewModel.Selected = true;
                }
            }
            return viewModels;
        }
    }
}
