using System;
using System.Collections.Generic;
using System.Text;

namespace Aureo.Models
{
    /// <summary>
    /// Represents a user's profile information and appearance preferences.
    /// </summary>
    public class UserProfile
    {
        public string Name { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public string? Profession { get; set; }
        public decimal Salary { get; set; } = 0m;
        public string ActiveTheme { get; set; } = "DarkTheme.xaml";
    }
}
