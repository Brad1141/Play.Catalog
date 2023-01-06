// Entites: classes used by the item repository to store data

using Play.Common;

namespace Play.Catelog.Service.Entities
{

    public class Item : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}