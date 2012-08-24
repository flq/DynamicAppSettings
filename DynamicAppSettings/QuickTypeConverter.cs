using System;
using System.ComponentModel;
using System.Globalization;

namespace DynamicAppSettings
{
    public class QuickConverter<T> : TypeConverter
    {
        private readonly Func<string, T> _convertFrom;
        private readonly Func<T, string> _convertTo;

        public QuickConverter(Func<string,T> convertFrom, Func<T,string> convertTo = null)
        {
            _convertFrom = convertFrom;
            _convertTo = convertTo;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        
        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
                return _convertFrom((string) value);
            return base.ConvertFrom(context, culture, value);
        }
        

        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && _convertTo != null)
            {
                return _convertTo((T)value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}