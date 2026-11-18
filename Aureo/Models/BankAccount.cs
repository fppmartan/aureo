using System;
using System.Collections.Generic;
using System.Text;

namespace Aureo.Models
{
    /// <summary>
    /// Represents a bank account with a unique identifier, name, reserve status, and balance.
    /// </summary>
    public class BankAccount
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public bool IsReserve { get; set; } = false; // Indicates whether the account is a reserve account (e.g., for emergency funds).
        public decimal Balance { get; set; } = 0m;
    }
}
