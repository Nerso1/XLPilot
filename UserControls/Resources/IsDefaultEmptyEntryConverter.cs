using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using XLPilot.Models;

namespace XLPilot.UserControls
{
    public class IsDefaultEmptyEntryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is XLPaths selectedEntry)
            {
                // Return false (disabled) if the selected entry is the empty row
                return !string.IsNullOrEmpty(selectedEntry.Name) || !string.IsNullOrEmpty(selectedEntry.Path);
            }
            // Disable if no selection
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
