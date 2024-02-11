using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiData.Shared
{
    public static class DefaultConfig
    {
        public static string ConnectionString { get; } = "Server=tcp:;Initial Catalog=;Persist Security Info=True;User ID=;Password=;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=300;";
    }
}
