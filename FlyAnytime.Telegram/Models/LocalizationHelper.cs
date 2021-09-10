using FlyAnytime.Core.Entity;
using FlyAnytime.Telegram.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public interface ILocalizationHelper
    {
        IEnumerable<LocalizationItem> GetEntityLocalizations<TEntity>(TEntity entity) where TEntity : class, IEntityWithLocalization;
        Task RemoveLocalization<TEntity>(TEntity entity, Language language) where TEntity : class, IEntityWithLocalization;
        Task RemoveAllLocalizations<TEntity>(TEntity entity) where TEntity : class, IEntityWithLocalization;
        Task AddOrUpdateEntityLocalizations<TEntity>(TEntity entity, Dictionary<string, string> languageCode2value) where TEntity : class, IEntityWithLocalization;
    }

    public class LocalizationHelper : ILocalizationHelper
    {
        private readonly TelegramContext _dbContext;

        public LocalizationHelper(TelegramContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddOrUpdateEntityLocalizations<TEntity>(TEntity entity, Dictionary<string, string> languageCode2Value)
            where TEntity : class, IEntityWithLocalization
        {
            var entityIdDb = _dbContext.Set<TEntity>().Find(entity.Id);

            if (entityIdDb == null)
                return;

            var currentLocalizations = GetEntityLocalizations(entityIdDb);

            foreach (var t in languageCode2Value)
            {
                var langCode = t.Key;
                var lang = _dbContext.Set<Language>().FirstOrDefault(x => x.Code == langCode);
                if (lang == null)
                    continue;

                var loc = currentLocalizations.FirstOrDefault(x => x.LanguageId == lang.Id);
                if (loc == null)
                    _dbContext.Add(LocalizationItem.Create(entityIdDb, lang, t.Value));
                else
                    loc.Localization = t.Value;
            }

            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<LocalizationItem> GetEntityLocalizations<TEntity>(TEntity entity)
            where TEntity : class, IEntityWithLocalization
        {
            var localizationItems = _dbContext.Set<LocalizationItem>()
                .Where(x => x.ItemId == entity.Id.ToString() && x.EntityDescriptor == entity.TypeDescriptor)
                .ToList();

            return localizationItems;
        }

        public async Task RemoveAllLocalizations<TEntity>(TEntity entity)
            where TEntity : class, IEntityWithLocalization
        {
            var allLocalizations = GetEntityLocalizations(entity);

            _dbContext.RemoveRange(allLocalizations);

            await _dbContext.SaveChangesAsync();
        }

        public Task RemoveLocalization<TEntity>(TEntity entity, Language language)
            where TEntity : class, IEntityWithLocalization
        {
            throw new NotImplementedException();
        }
    }
}
