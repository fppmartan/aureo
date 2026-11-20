using System.Windows.Input;
using Aureo.Models;
using Aureo.Services;

namespace Aureo.ViewModels
{
    /// <summary>
    /// The MainViewModel class serves as the primary view model for the Aureo application, 
    /// managing the state and behavior of the main window. 
    /// It handles navigation between different sections, computes greeting and alert messages based on user data 
    /// and current date, and interacts with the storage service to load application data.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly StorageService _storageService; // Reads/saves local data.
        private AppData _appData; // Holds loaded data in memory.
        private string _currentSection = "Home"; // Tracks the active screen/tab.
        private string _greetingMessage = string.Empty; // Backing field for GreetingMessage.
        private string _smartAlertMessage = string.Empty; // Backing field for SmartAlertMessage.

        public string CurrentSection
        {
            get => _currentSection; // Returns the private field value.
            set => SetProperty(ref _currentSection, value); // Updates field + notifies UI if changed.
        }

        public string GreetingMessage
        {
            get => _greetingMessage;
            set => SetProperty(ref _greetingMessage, value);
        }

        public string SmartAlertMessage 
        {
            get => _smartAlertMessage;
            set => SetProperty(ref _smartAlertMessage, value);
        }

        public ICommand NavigateCommand { get; } // Read-only property; assigned in constructor.

        public MainViewModel() 
        {
            _storageService = new StorageService(); // Creates the storage service instance. 
            _appData = _storageService.Load(); // Loads saved JSON data (or defaults)

            NavigateCommand = new RelayCommand(param => 
            {
                if (param is string section)
                {
                    CurrentSection = section; // Updates the current tab based on the command parameter.
                }
            });

            GreetingMessage = BuildGreeting(); // Computes initial greeting.
            SmartAlertMessage = BuildSmartAlert(); // Computes initial alert/status.
        }

        private string BuildGreeting() 
        {
            int hour = DateTime.Now.Hour; // Current system hour (0 to 23).

            // Ternary chain: if < 12 Morning, else if < 18 Afternoon, else Evening.
            string prefix = hour < 12 ? "Good morning" : hour < 18 ? "Good afternoon" : "Good evening";

            string? userName = _appData?.Profile?.Name; // Safely access the user's name even if _appData or Profile is null.
            return string.IsNullOrWhiteSpace(userName) ? $"{prefix}." : $"{prefix}, {userName}."; // Adds name if available.
        }

        private string BuildSmartAlert() 
        {
            DateTime today = DateTime.Today; // Full date (year, month, day) at 00:00.

            string? smartAlertMessage = null;
            int minDaysLeft = int.MaxValue; // Starts huge so any 1..5 day alert is smaller.
            foreach (var category in _appData.Categories) 
            {
                if (category.IsIncome) continue; // Skip income categories.
                foreach (var subcategory in category.Subcategories) 
                {
                    if (subcategory.RenewalDay is int renewalDay) 
                    {
                        DateTime nextRenewal = GetNextRenewalDay(renewalDay, today);
                        int daysLeft = (nextRenewal - today).Days; // Exact days across month/year boundaries.
                        if (daysLeft == 0) return $"{subcategory.Name} is due today.";
                        if (daysLeft > 0 && daysLeft <= 5 && daysLeft < minDaysLeft) 
                        {
                            minDaysLeft = daysLeft;
                            smartAlertMessage = $"{daysLeft} {(daysLeft == 1 ? "day" : "days")} left until {subcategory.Name}.";
                        }
                    }
                }
            }

            return smartAlertMessage ?? "No bills due today."; // Default fallback when nothing is due soon.
        }

        private DateTime GetNextRenewalDay(int renewalDay, DateTime today) 
        {
            // Ensure the renewal day is valid for the current month (e.g., February has 28 or 29 days).
            int validDay = Math.Min(renewalDay, DateTime.DaysInMonth(today.Year, today.Month));
            DateTime nextDate = new DateTime(today.Year, today.Month, validDay);

            if (nextDate < today) 
            {
                DateTime nextMonth = today.AddMonths(1);
                validDay = Math.Min(renewalDay, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
                nextDate = new DateTime(nextMonth.Year, nextMonth.Month, validDay);
            }
            return nextDate; // Returns the next valid renewal date, either this month or next.
        }

    }
}
