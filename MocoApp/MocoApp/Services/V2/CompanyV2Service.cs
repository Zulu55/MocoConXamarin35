using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Services.V2
{
    public class CompanyV2Service : BaseHttpService
    {
        public async Task UpdateCurrentCompany(string id)
        {
            var endpoint = $"manager/updateCompany/{id}";
            await Get<string>(endpoint);
        }
    }
}
