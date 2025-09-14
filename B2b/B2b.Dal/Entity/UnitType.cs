using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class UnitType
    {
        [Key] 
        public int UnitTypeId { get; set; }
        public string UnitTypeName { get; set; }
    }
}
