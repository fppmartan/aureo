using System;
using System.Collections.Generic;
using System.Text;

namespace Aureo.Models
{
    /// <summary>
    /// Represents a subcategory with its identity, name, planned and spent amounts, and optional renewal day.
    /// </summary>
    public class Subcategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; } = 0m;
        public decimal SpentAmount { get; set; } = 0m;
        public int? RenewalDay { get; set; }
    }
}
