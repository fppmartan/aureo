using System;
using System.Windows;

namespace Aureo.Services
{
    /// <summary>
    /// Provides methods for applying application themes.
    /// </summary>
    public static class ThemeService
    {
        public static void ApplyTheme(string themeFileName)
        {
            if (Application.Current == null) return; // Safety guard if app is not initialized.

            // Build the relative URI pointing to the XAML dictionary inside the Themes folder.
            var newTheme = new ResourceDictionary
            {
                Source = new Uri($"Themes/{themeFileName}", UriKind.Relative)
            };

            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            mergedDictionaries.Clear(); // Removes the previous theme dictionary.
            mergedDictionaries.Add(newTheme); // Applies the new theme dictionary to the application's resources.
        }
    }
}
