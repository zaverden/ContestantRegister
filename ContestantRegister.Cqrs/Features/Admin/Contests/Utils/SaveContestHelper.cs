using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels;
using ContestantRegister.Features;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Contests.Utils
{
    static class SaveContestHelper
    {
        public static void RemoveWindowsLineEnds(this ContestDetailsViewModel contest)
        {
            if (!string.IsNullOrEmpty(contest.YaContestAccountsCSV) && contest.YaContestAccountsCSV.Contains('\r'))
            {
                contest.YaContestAccountsCSV = contest.YaContestAccountsCSV.Replace("\r", "");
            }
        }

        public static async Task SyncManyToMany<TEntity, TRelationEntity>(int[] selectedIds, IQueryable<TEntity> dbSet, ICollection<TRelationEntity> items, Func<TEntity, Contest, TRelationEntity> relationFactory, Func<TEntity, TRelationEntity, bool> comparator, Contest contest)
            where TEntity : DomainObject
            where TRelationEntity : DomainObject
        {
            var selectedItems = new List<TEntity>();
            if (selectedIds != null)
            {
                selectedItems = await dbSet.Where(item => selectedIds.Contains(item.Id)).ToListAsync();
            }

            foreach (var currentItem in items.ToList())
            {
                if (!selectedItems.Any(item => comparator(item, currentItem)))
                {
                    items.Remove(currentItem);
                }
            }

            foreach (var selectedItem in selectedItems)
            {
                if (!items.Any(item => comparator(selectedItem, item)))
                {
                    items.Add(relationFactory(selectedItem, contest));
                }
            }
        }

        public static ContestArea CreateContestAreaRelation(Area area, Contest contest)
        {
            return new ContestArea
            {
                Area = area,
                Contest = contest,
            };
        }

        public static bool CmpArea(Area area, ContestArea contestArea)
        {
            return area.Id == contestArea.AreaId;
        }
    }
}
