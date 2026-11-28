using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Aureo.Models;
using Aureo.Services;
using Microsoft.Win32;

namespace Aureo.ViewModels
{
    /// <summary>
    /// Manages profile settings, avatar selection, theme switching, backup operations,
    /// bank account CRUD, and inter-account transfers.
    /// </summary>
    public class ProfileViewModel : BaseViewModel
    {
        // Observable collection bound to lists and ComboBoxes in the UI.

        // Bank Account CRUD
        public ObservableCollection<BankAccount> Accounts { get; } = new();
        private BankAccount? _selectedAccount;
        private string _accountName = string.Empty;
        private string _accountBalanceInput = string.Empty;
        private bool _accountIsReserve;

        // Inter-Account Transfer
        private BankAccount? _sourceAccount;
        private BankAccount? _destinationAccount;
        private string _transferAmountInput = string.Empty;

        public BankAccount? SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (SetProperty(ref _selectedAccount, value) && value != null)
                {
                    // Populate form fields when an account is selected for editing.
                    AccountName = value.Name;
                    AccountBalanceInput = value.Balance.ToString("F2");
                    AccountIsReserve = value.IsReserve;

                }
            }
        }

        public string AccountName
        {
            get => _accountName;
            set => SetProperty(ref _accountName, value);
        }

        public string AccountBalanceInput
        {
            get => _accountBalanceInput;
            set => SetProperty(ref _accountBalanceInput, value);
        }

        public bool AccountIsReserve
        {
            get => _accountIsReserve;
            set => SetProperty(ref _accountIsReserve, value);
        }

        public BankAccount? SourceAccount
        {
            get => _sourceAccount;
            set => SetProperty(ref _sourceAccount, value);
        }

        public BankAccount? DestinationAccount
        {
            get => _destinationAccount;
            set => SetProperty(ref _destinationAccount, value);
        }

        public string TransferAmountInput
        {
            get => _transferAmountInput;
            set => SetProperty(ref _transferAmountInput, value);
        }


        // Commands
        public ICommand SaveAccountCommand { get; }
        public ICommand ClearAccountFormCommand { get; }
        public ICommand DeleteAccountCommand { get; }
        public ICommand TransferCommand { get; }

        private readonly StorageService _storageService;
        private readonly AppData _appData;
        private readonly Action? _onProfileChanged;


        // Profile & Appearance
        private string _name = string.Empty;
        private string _profession = string.Empty;
        private string _salaryInput = string.Empty;
        private string? _photoPath;
        private bool _isDarkTheme;
        private string _statusMessage = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Profession
        {
            get => _profession;
            set => SetProperty(ref _profession, value);
        }

        public string SalaryInput
        {
            get => _salaryInput;
            set => SetProperty(ref _salaryInput, value);
        }

        public string? PhotoPath
        {
            get => _photoPath;
            set => SetProperty(ref _photoPath, value);
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetProperty(ref _isDarkTheme, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }


        // Commands
        public ICommand SaveProfileCommand { get; }
        public ICommand SelectPhotoCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand ExportBackupCommand { get; }
        public ICommand RestoreBackupCommand { get; }

        public ProfileViewModel(AppData appData, StorageService storageService, Action? onProfileChanged = null) 
        {
            _appData = appData;
            _storageService = storageService;
            _onProfileChanged = onProfileChanged;

            LoadProfileFields();

            SaveProfileCommand = new RelayCommand(_ => SaveProfile());
            SelectPhotoCommand = new RelayCommand(_ => SelectPhoto());
            ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
            ExportBackupCommand = new RelayCommand(_ => ExportBackup());
            RestoreBackupCommand = new RelayCommand(_ => RestoreBackup());

            LoadBankAccounts();

            SaveAccountCommand = new RelayCommand(_ => SaveAccount());
            ClearAccountFormCommand = new RelayCommand(_ => ClearAccountForm());
            DeleteAccountCommand = new RelayCommand(param => DeleteAccount(param as BankAccount));
            TransferCommand = new RelayCommand(_ => Transfer());
        }

        private void LoadProfileFields()
        {
            Name = _appData.Profile.Name;
            Profession = _appData.Profile.Profession ?? string.Empty;
            SalaryInput = _appData.Profile.Salary.ToString("F2"); // Formats decimal with 2 decimal places.
            PhotoPath = _appData.Profile.PhotoPath;
            IsDarkTheme = _appData.Profile.ActiveTheme == "DarkTheme.xaml";
        }

        private void SaveProfile()
        {
            // Validates text input safely into a decimal without throwing exceptions.
            if (!decimal.TryParse(SalaryInput, out decimal parsedSalary) || parsedSalary < 0) 
            {
                StatusMessage = "Please enter a valid non-negative amount.";
                return;
            }

            _appData.Profile.Name = Name.Trim();
            _appData.Profile.Profession = Profession.Trim();
            _appData.Profile.Salary = parsedSalary;
            _storageService.Save(_appData);

            StatusMessage = "Settings saved successfully.";
            _onProfileChanged?.Invoke(); // Notifies parent ViewModel if callback was provided.
        }

        private void SelectPhoto() 
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Avatar Image",
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog() == true)
            {
                PhotoPath = dialog.FileName;
                _appData.Profile.PhotoPath = dialog.FileName;
                _storageService.Save(_appData);
                StatusMessage = "Avatar updated.";
                _onProfileChanged?.Invoke();
            }
        }

        private void ToggleTheme()
        {
            string nextTheme = IsDarkTheme ? "LightTheme.xaml" : "DarkTheme.xaml";
            _appData.Profile.ActiveTheme = nextTheme;
            IsDarkTheme = nextTheme == "DarkTheme.xaml";

            ThemeService.ApplyTheme(nextTheme);
            _storageService.Save(_appData);
            StatusMessage = $"Switched to {(IsDarkTheme ? "Dark" : "Light")} theme.";
        }

        private void ExportBackup() 
        {
            var dialog = new SaveFileDialog
            {
                Title = "Export Backup",
                Filter = "JSON Files (*.json)|*.json",
                FileName = "backup.json"
            };

            if (dialog.ShowDialog() == true)
            {
                _storageService.ExportBackup(dialog.FileName, _appData);
                StatusMessage = "Backup exported successfully.";
            }
        }

        private void RestoreBackup()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Restore Backup",
                Filter = "JSON Files (*.json)|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                AppData restored = _storageService.ImportBackup(dialog.FileName);
                // Mutate the existing _appData instance so MainViewModel keeps sharing
                // the same reference.
                _appData.Profile = restored.Profile;
                _appData.BankAccounts = restored.BankAccounts;
                _appData.Categories = restored.Categories;
                _appData.Transactions = restored.Transactions;

                _storageService.Save(_appData);
                LoadProfileFields();
                LoadBankAccounts();
                ClearAccountForm();

                ThemeService.ApplyTheme(_appData.Profile.ActiveTheme);
                StatusMessage = "Backup restored successfully.";
                _onProfileChanged?.Invoke();
            }
        }

        // Rebuilds the ObservableCollection so the UI refreshes updated account balances.
        private void LoadBankAccounts()
        {
            Accounts.Clear();
            foreach (var account in _appData.BankAccounts)
            {
                Accounts.Add(account);
            }
        }

        private void ClearAccountForm()
        {
            SelectedAccount = null;
            AccountName = string.Empty;
            AccountBalanceInput = "0.00";
            AccountIsReserve = false;
        }

        // Null means creating a new account; non-null means updating the selected one.
        private void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(AccountName))
            {
                StatusMessage = "Account name is required.";
                return;
            }

            if (!decimal.TryParse(AccountBalanceInput, out decimal parsedBalance))
            {
                StatusMessage = "Please enter a valid numeric balance.";
                return;
            }

            if (SelectedAccount == null)
            {
                var newBankAccount = new BankAccount
                {
                    Name = AccountName.Trim(),
                    Balance = parsedBalance,
                    IsReserve = AccountIsReserve
                };
                _appData.BankAccounts.Add(newBankAccount);
                StatusMessage = "Account created successfully.";
            }
            else
            {
                SelectedAccount.Name = AccountName.Trim();
                SelectedAccount.Balance = parsedBalance;
                SelectedAccount.IsReserve = AccountIsReserve;
                StatusMessage = "Account updated successfully.";
            }

            _storageService.Save(_appData);
            LoadBankAccounts();
            ClearAccountForm();
        }

        // Supports both a row-level Delete button {CommandParameter} and the selected item.
        private void DeleteAccount(BankAccount? account)
        {
            var target = account ?? SelectedAccount;
            if (target == null) return;

            _appData.BankAccounts.Remove(target);
            _storageService.Save(_appData);
            LoadBankAccounts();
            ClearAccountForm();
            StatusMessage = "Account deleted.";
        }

        // Logged as Transfer so income and expense reports remain accurate.
        private void Transfer()
        {
            if (SourceAccount == null || DestinationAccount == null)
            {
                StatusMessage = "Select both source and destination accounts.";
                return;
            }

            if (SourceAccount.Id == DestinationAccount.Id)
            {
                StatusMessage = "Source and destination accounts must be different.";
                return;
            }

            if (!decimal.TryParse(TransferAmountInput, out decimal amount) || amount <= 0)
            {
                StatusMessage = "Transfer amount must be greater than zero.";
                return;
            }

            SourceAccount.Balance -= amount;
            DestinationAccount.Balance += amount;

            _appData.Transactions.Add(new Transaction
            {
                Type = TransactionType.Transfer,
                Amount = amount,
                Date = DateTime.Now,
                Description = $"Transfer from {SourceAccount.Name} to {DestinationAccount.Name}",
                BankAccountId = SourceAccount.Id,
                DestinationBankAccountId = DestinationAccount.Id
            });

            _storageService.Save(_appData);
            LoadBankAccounts();
            TransferAmountInput = string.Empty;
            StatusMessage = "Transfer completed successfully.";
        }
    }
}
