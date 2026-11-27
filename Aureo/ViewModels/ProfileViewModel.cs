using System;
using System.Windows.Input;
using Aureo.Models;
using Aureo.Services;
using Microsoft.Win32;

namespace Aureo.ViewModels
{
    /// <summary>
    /// Manages profile settings, avatar selection, theme switching, and backup operations.
    /// </summary>
    public class ProfileViewModel : BaseViewModel
    {
        private readonly StorageService _storageService;
        private readonly AppData _appData;
        private readonly Action? _onProfileChanged;

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
                _appData.Profile = restored.Profile;
                _appData.BankAccounts = restored.BankAccounts;
                _appData.Categories = restored.Categories;
                _appData.Transactions = restored.Transactions;

                _storageService.Save(_appData);
                LoadProfileFields();

                ThemeService.ApplyTheme(_appData.Profile.ActiveTheme);
                StatusMessage = "Backup restored successfully.";
                _onProfileChanged?.Invoke();
            }
        }
    }
}
