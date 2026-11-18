using System;
using System.Collections.Generic;
using System.Text;

namespace Aureo.Models
{
    /// <summary>
    /// Contains the user profile and collections of financial accounts, categories, and transactions.
    /// </summary>
    public class AppData
    {
        public UserProfile Profile { get; set; } = new(); // Initializes a new instance of UserProfile to hold user-specific data.
        public List<BankAccount> BankAccounts { get; set; } = new(); // Initializes a new list to hold bank accounts associated with the user.
        public List<Category> Categories { get; set; } = new(); // Initializes a new list to hold financial categories for income and expenses.
        public List<Transaction> Transactions { get; set; } = new(); // Initializes a new list to hold financial transactions made by the user.

    }
}
