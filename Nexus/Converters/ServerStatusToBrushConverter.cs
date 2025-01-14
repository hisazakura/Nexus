using Nexus.Enum;
using Nexus.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Nexus.Converters
{
    public class ServerStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ServerStatus status)
            {
                return status switch
                {
                    ServerStatus.Online => App.Current.Resources["Green600"],
                    ServerStatus.Inactive => App.Current.Resources["Yellow600"],
                    _ => App.Current.Resources["Red600"],
                };
            }

            return App.Current.Resources["Gray600"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
