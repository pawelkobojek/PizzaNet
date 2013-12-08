using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace PizzaNetControls.Validation
{
    public class PasswordConfig : FrameworkElement, INotifyPropertyChanged
    {
        public PasswordConfig()
        {
            AllowEmpty = true;
            Visibility = System.Windows.Visibility.Hidden;
        }

        private PropertyInfo PropertyInfo { get; set; }
        private object Target { get; set; }
        public bool AllowEmpty { get; set; }
        public void SetTarget(Type targetType, object target, string propertyName)
        {
            PropertyInfo=targetType.GetProperty(propertyName);
            Target = target;
        }

        public string Value 
        {
            get 
            {
                if (PropertyInfo == null) return null;
                if (Target == null) return null;
                try
                {
                    var result = PropertyInfo.GetValue(Target);
                    if (result == null) return null;
                    return result.ToString();
                }
                catch(Exception)
                {
                    return null;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
