using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Models;
using ContestantRegister.Utils.Exceptions;
using ContestantRegister.Utils.Filter;
using Microsoft.AspNetCore.Mvc;

namespace ContestantRegister.Controllers._Common
{
    //мда, этот класс побил мой предыдущий личный рекорд по числу генерик-параметров :) https://youtu.be/HvSaBVSAkz4?t=1705
    //но было интересно, как будет выглядеть cqrs в crud сценарии
    //TODO если будет не лень, сделать вариант реализации просто через генерики, будет на порядок проще 
    public abstract class CrudController<
        TEntity, TListItemViewModel, TDetailsViewModel,
        TGetEntitiesQuery, TGetEntityByIdQuery, TGetEntityByIdForDeleteQuery,
        TCreateEntityCommand, TEditEntityCommand, TDeleteEntityByIdCommand> 
            : Controller
        where TEntity : DomainObject
        where TDetailsViewModel : class

        where TGetEntitiesQuery : GetMappedEntitiesQuery<TEntity, TListItemViewModel>
        where TGetEntityByIdQuery : GetEntityByIdQuery<TEntity>, new()
        where TGetEntityByIdForDeleteQuery : GetEntityByIdForDeleteQuery<TEntity>, new()

        where TCreateEntityCommand : CreateMappedEntityCommand<TEntity, TDetailsViewModel>, new()
        where TEditEntityCommand : EditMappedEntityCommand<TEntity, TDetailsViewModel>, new()
        where TDeleteEntityByIdCommand : DeleteEntityByIdCommand<TEntity>, new()
    {
        protected readonly IHandlerDispatcher HandlerDispatcher;
        private readonly IMapper _mapper;

        protected CrudController(IHandlerDispatcher dispatcher, IMapper mapper)
        {
            HandlerDispatcher = dispatcher;
            _mapper = mapper;
        }

        // GET: Entities
        public async Task<IActionResult> Index(TGetEntitiesQuery query)
        {
            MapFilterToViewData(query);

            var entities = await HandlerDispatcher.ExecuteQueryAsync(query);

            return View(entities);
        }

        // GET: Entities/Create
        public async Task<IActionResult> Create()
        {
            await FillViewDataDetailFormAsync();

            return View();
        }

        // POST: Entities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await HandlerDispatcher.ExecuteCommandAsync(new TCreateEntityCommand { Entity = viewModel });
                return RedirectToAction(nameof(Index));
            }

            await FillViewDataDetailFormAsync(viewModel);

            return View(viewModel);
        }

        // GET: Entities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entity = await HandlerDispatcher.ExecuteQueryAsync(new TGetEntityByIdQuery
            {
                Id = id.Value,
                IncludeProperties = GetIncludePropertiesForEdit()
            });
            if (entity == null) return NotFound();
            
            var viewModel = _mapper.Map<TDetailsViewModel>(entity);

            await FillViewDataDetailFormAsync(viewModel);

            return View(viewModel);
        }

        protected virtual string[] GetIncludePropertiesForEdit()
        {
            return null;
        }

        // POST: Entities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TDetailsViewModel viewModel)
        {
            //if (id != entity.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await HandlerDispatcher.ExecuteCommandAsync(new TEditEntityCommand
                    {
                        Entity = viewModel,
                        Id = id,
                    });
                }
                catch (EntityNotFoundException)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            await FillViewDataDetailFormAsync(viewModel);

            return View(viewModel);
        }

        // GET: Entities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entity = await HandlerDispatcher.ExecuteQueryAsync(new TGetEntityByIdForDeleteQuery
            {
                Id = id.Value,
                IncludeProperties = GetIncludePropertiesForDelete()
            });
            if (entity == null) return NotFound();

            return View(entity);
        }

        protected virtual string[] GetIncludePropertiesForDelete()
        {
            return null;
        }

        // POST: Entities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await HandlerDispatcher.ExecuteCommandAsync(new TDeleteEntityByIdCommand { Id = id });

            return RedirectToAction(nameof(Index));
        }

        protected void MapFilterToViewData(TGetEntitiesQuery filter)
        {
            var properties = FastTypeInfo
                .GetPublicProperties(filter.GetType())
                .Select(x => new
                {
                    x.Name,
                    Value = x.GetValue(filter)
                })
                .Where(x => x.Value != null);

            foreach (var property in properties)
            {
                ViewData[property.Name] = property.Value;
            }
        }

        protected virtual Task FillViewDataDetailFormAsync(TDetailsViewModel viewModel = null)
        {
            return Task.FromResult(0);
        }
    }
}
