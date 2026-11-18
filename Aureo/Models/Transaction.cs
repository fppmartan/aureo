using System;
using System.Collections.Generic;
using System.Text;

namespace Aureo.Models
{
    /// <summary>
    /// Represents a financial transaction, including its type, date, description, accounts, and optional category
    /// assignments.
    /// </summary>
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public TransactionType Type { get; set; } = TransactionType.Expense; // Indicates whether the transaction is an income, expense, or transfer.
        public decimal Amount { get; set; } = 0m;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public Guid BankAccountId { get; set; } // The ID of the bank account associated with this transaction.
        public Guid? DestinationBankAccountId { get; set; } // Nullable to accommodate transactions that don't involve a destination account.
        public Guid? CategoryId { get; set; } // Nullable to accommodate transactions that don't involve a category.
        public Guid? SubcategoryId { get; set; } // Nullable to accommodate transactions that don't involve a subcategory.

    }
}
