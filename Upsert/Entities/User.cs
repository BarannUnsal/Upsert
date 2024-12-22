using Upsert.Entities.Common;

namespace Upsert.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
