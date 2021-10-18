using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Test
{
    public class LanguageTestCase
    {
        public static async Task TestSaveLanguage()
        {
            var test = new LanguageTest();

            var eng = new Language
            {
                Code = "En",
                Name = "English"
            };

            var ru = new Language
            {
                Code = "Ru",
                Name = "Русский"
            };

            var idEng = await test.CreateLanguage(eng);
            var idRu = await test.CreateLanguage(ru);

            var engSaved = await test.GetById(idEng);

            Assert.AreEqual(eng.Code, engSaved.Code);
            Assert.AreEqual(eng.Name, engSaved.Name);
        }

        public static async Task TestPagination()
        {
            var test = new LanguageTest();

            var arr = new List<Language>();

            for (int i = 0; i < 15; i++)
            {
                var l = new Language
                {
                    Code = $"Code{i}",
                    Name = $"Name{i}"
                };
                arr.Add(l);
            }

            foreach (var l in arr)
            {
                await test.CreateLanguage(l);
            }

            var pageSize = 6;
            var (total, items) = await test.GetMany(1, pageSize);

            Assert.AreEqual(arr.Count, total);

            var lastPage = arr.Count / pageSize + 1;
            var lasPageItemsCount = arr.Count % pageSize;

            var (total1, items1) = await test.GetMany(lastPage, pageSize);

            Assert.AreEqual(lasPageItemsCount, items1.Count());
        }
    }
}
