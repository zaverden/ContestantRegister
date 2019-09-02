using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
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
        TEntity, TListItemViewModel,
        TGetEntitiesQuery, TGetEntityByIdQuery, TGetEntityByIdForDeleteQuery,
        TCreateEntityCommand, TEditEntityCommand, TDeleteEntityByIdCommand> 
            : Controller
        where TEntity : DomainObject
        
        where TGetEntitiesQuery : GetEntitiesWithMappingQuery<TEntity, TListItemViewModel>
        where TGetEntityByIdQuery : GetEntityByIdQuery<TEntity>, new()
        where TGetEntityByIdForDeleteQuery : GetEntityByIdForDeleteQuery<TEntity>, new()

        where TCreateEntityCommand : CreateEntityCommand<TEntity>, new()
        where TEditEntityCommand : EditEntityCommand<TEntity>, new()
        where TDeleteEntityByIdCommand : DeleteEntityByIdCommand<TEntity>, new()
    {
        protected readonly IHandlerDispatcher HandlerDispatcher;

        protected CrudController(IHandlerDispatcher dispatcher)
        {
            HandlerDispatcher = dispatcher;
        }

        // GET: Entities
        public async Task<IActionResult> Index(TGetEntitiesQuery filter)
        {
            MapFilterToViewData(filter);

            var entities = await HandlerDispatcher.ExecuteQueryAsync(filter);

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
        public async Task<IActionResult> Create(TEntity entity)
        {
            if (ModelState.IsValid)
            {
                await HandlerDispatcher.ExecuteCommandAsync(new TCreateEntityCommand { Entity = entity });
                return RedirectToAction(nameof(Index));
            }

            await FillViewDataDetailFormAsync(entity);
            return View(entity);
        }

        // GET: Entities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entity = await HandlerDispatcher.ExecuteQueryAsync(new TGetEntityByIdQuery { Id = id.Value });
            if (entity == null) return NotFound();

            await FillViewDataDetailFormAsync(entity);
            return View(entity);
        }

        // POST: Entities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TEntity entity)
        {
            if (id != entity.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await HandlerDispatcher.ExecuteCommandAsync(new TEditEntityCommand { Entity = entity });
                }
                catch (EntityNotFoundException)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            await FillViewDataDetailFormAsync(entity);
            return View(entity);
        }

        // GET: Entities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entity = await HandlerDispatcher.ExecuteQueryAsync(new TGetEntityByIdForDeleteQuery { Id = id.Value });
            if (entity == null) return NotFound();

            return View(entity);
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

        protected virtual Task FillViewDataDetailFormAsync(TEntity item = null)
        {
            return Task.FromResult(0);
        }
    }
}
