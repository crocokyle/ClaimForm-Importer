using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClaimForm_Importer
{
    public interface IDatabaseClient
    {
        Task UpdateAsync(Dictionary<string, string> data);
    }
}
