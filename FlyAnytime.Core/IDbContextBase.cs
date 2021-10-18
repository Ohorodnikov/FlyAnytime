using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Core
{
    public interface IDbContextBase
    {
        Task ReCreateDb();
    }
}
