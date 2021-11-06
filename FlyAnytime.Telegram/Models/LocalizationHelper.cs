using FlyAnytime.Core.Entity;
using FlyAnytime.Telegram.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Models
{
    public interface ILocalizationHelper
    {
        Task<IEnumerable<LocalizationItem>> GetEntityLocalizations<TEntity>(TEntity entity) where TEntity : class, IEntityWithLocalization;
        Task RemoveLocalization<TEntity>(TEntity entity, Language language) where TEntity : class, IEntityWithLocalization;
        Task RemoveAllLocalizations<TEntity>(TEntity entity) where TEntity : class, IEntityWithLocalization;
        Task AddOrUpdateEntityLocalizations<TEntity>(TEntity entity, Dictionary<string, string> languageCode2value) where TEntity : class, IEntityWithLocalization;

        Task<IEnumerable<(TEntity entity, LocalizationItem localization)>> FindEntitiesByLocalization<TEntity>(Language language, string searchKey) where TEntity : class, IEntityWithLocalization, new();

        Task<LocalizationItem> GetEntityLocalizationForChat<TEntity>(long chatId, TEntity entity) where TEntity : class, IEntityWithLocalization;
        Task<string> GetEntityLocalizationValueForChat<TEntity>(long chatId, TEntity entity, string defValue) where TEntity : class, IEntityWithLocalization;
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

            var currentLocalizations = await GetEntityLocalizations(entityIdDb);

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

        public async Task<IEnumerable<LocalizationItem>> GetEntityLocalizations<TEntity>(TEntity entity)
            where TEntity : class, IEntityWithLocalization
        {
            var localizationItems = await _dbContext.Set<LocalizationItem>()
                .Where(x => x.ItemId == entity.Id.ToString() && x.EntityDescriptor == entity.TypeDescriptor)
                .ToListAsync();

            return localizationItems;
        }

        public async Task RemoveAllLocalizations<TEntity>(TEntity entity)
            where TEntity : class, IEntityWithLocalization
        {
            var allLocalizations = await GetEntityLocalizations(entity);

            _dbContext.RemoveRange(allLocalizations);

            await _dbContext.SaveChangesAsync();
        }

        public Task RemoveLocalization<TEntity>(TEntity entity, Language language)
            where TEntity : class, IEntityWithLocalization
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<(TEntity entity, LocalizationItem localization)>> FindEntitiesByLocalization<TEntity>(Language language, string searchKey)
            where TEntity : class, IEntityWithLocalization, new()
        {
            var entTypeDescr = new TEntity().TypeDescriptor;
            var itemIds2LocValue = await _dbContext.Set<LocalizationItem>()
                .Where(x => x.LanguageId == language.Id && x.EntityDescriptor == entTypeDescr && x.Localization.StartsWith(searchKey))
                .ToListAsync()
                ;

            var itemIds = itemIds2LocValue.Select(x => x.ItemId).Distinct().ToList();

            var items = await _dbContext.Set<TEntity>()
                .Where(x => itemIds.Contains(x.Id.ToString()))
                .ToListAsync()
                ;

            var res = new List<(TEntity entity, LocalizationItem localization)>();

            foreach (var item in items)
            {
                var value = itemIds2LocValue.FirstOrDefault(x => x.ItemId == item.Id.ToString());
                res.Add((item, value));
            }

            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="chatId"></param>
        /// <param name="entity"></param>
        /// <param name="defValue">Return this value if no localization will be found</param>
        /// <returns></returns>
        public async Task<string> GetEntityLocalizationValueForChat<TEntity>(long chatId, TEntity entity, string defValue)
            where TEntity : class, IEntityWithLocalization
        {
            var data = await GetEntityLocalizationForChat(chatId, entity);

            return data?.Localization ?? defValue ?? $"No localization for {entity.TypeDescriptor}:{entity.Id}";
        }

        public async Task<LocalizationItem> GetEntityLocalizationForChat<TEntity>(long chatId, TEntity entity)
            where TEntity : class, IEntityWithLocalization
        {
            var locItems = _dbContext.Set<LocalizationItem>();
            var chats = _dbContext.Set<Chat>();

            var data = await locItems
                .Join
                (
                    chats,
                    locIt => locIt.LanguageId,
                    chat => chat.UserLanguage.Id,
                    (loc, chat) =>
                    new { loc, chatId = chat.Id }
                )
                .Where
                (
                    x =>
                        x.chatId == chatId
                        && x.loc.ItemId == entity.Id.ToString()
                        && x.loc.EntityDescriptor == entity.TypeDescriptor
                )
                .Select(x => x.loc)
                .FirstOrDefaultAsync(); 

            return data;
        }
    }
}
