using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Base
{
    public abstract class BaseLookup
    {
        [Key]
        public int Id { get; set; }              // matches the enum value

        [Required, MaxLength(50)]
        public string? Code { get; set; }        // stable machine key, e.g. "FULL_TIME"

        [Required, MaxLength(100)]
        public string? Name { get; set; }        // display label, e.g. "Full Time"

        public int? SortOrder { get; set; }      // controls dropdown order

        public bool? IsActive { get; set; }      // soft-disable without deleting
    }
}
