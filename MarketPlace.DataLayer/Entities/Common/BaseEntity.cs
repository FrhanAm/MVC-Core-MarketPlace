using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.Entities.Common;

public class BaseEntity
{
    [Key]
    public long Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastUpdateDate { get; set; }

}
