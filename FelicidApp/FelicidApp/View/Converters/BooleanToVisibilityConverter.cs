using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FelicidApp.View.Converters
{
    /// <summary>
    /// Converts True to Visible and False to Collapsed
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts True to Visible and False to Collapsed
        /// </summary>
        /// <param name="value">The boolean value</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">If it is equals to "Invert", it does the opposite operation</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>The correspondent Visibility value</returns>
        public object Convert(object value, Type targetType, object parameter, string language) 
            => (((bool)value) ^ ((parameter != null) && ((string)parameter).Equals("Invert"))) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>It always returns null</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language) 
            => null;
    }
}
