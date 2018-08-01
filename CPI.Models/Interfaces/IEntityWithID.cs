using System;

namespace CPI.Models
{
    public interface IEntityWithID
    {
        Guid ID { set; get; }
    }
}
