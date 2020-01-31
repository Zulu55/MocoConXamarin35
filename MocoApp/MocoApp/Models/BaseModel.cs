using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocoApp.Models
{
    public class BaseModel
    {
        public string Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public void GenerateId()
        {
            Id = Guid.NewGuid().ToString();

            CreatedAt = DateTime.Now;
        }
    }
}
