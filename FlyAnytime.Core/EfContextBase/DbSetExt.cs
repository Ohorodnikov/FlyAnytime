using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Core.EfContextBase
{
    public static class DbSetExt
    {
        public static async Task RemoveAll<TEnt>(this DbSet<TEnt> set)
            where TEnt : class, IEntity
        {
            using var cmd = set.CreateDbCommand();

            var need2CloseConn = false;
            switch (cmd.Connection.State)
            {
                case System.Data.ConnectionState.Closed:
                case System.Data.ConnectionState.Broken:
                    await cmd.Connection.OpenAsync();
                    need2CloseConn = true;
                    break;
                case System.Data.ConnectionState.Open:
                case System.Data.ConnectionState.Connecting:
                case System.Data.ConnectionState.Executing:
                case System.Data.ConnectionState.Fetching:
                default:
                    break;
            }

            cmd.CommandText = $"delete from [{set.EntityType.GetSchema() ?? "dbo"}].[{set.EntityType.GetTableName()}]";

            await cmd.ExecuteNonQueryAsync();

            if (need2CloseConn)
            {
                await cmd.Connection.CloseAsync();
            }
        }
    }
}
