using System;
using System.Collections.Generic;
using System.Text;

namespace Aureo.Models
{
    /// <summary>
    /// Represents a financial category for income or expenses, with associated subcategories.
    /// </summary>
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public bool IsIncome { get; set; } = false; // Indicates whether the category is for income (true) or expenses (false).
        public List<Subcategory> Subcategories { get; set; } = new(); // Initializes a new list of subcategories associated with this category.
    }
}
