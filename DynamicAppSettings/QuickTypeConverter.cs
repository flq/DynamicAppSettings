using System;
using System.ComponentModel;
using System.Globalization;

namespace DynamicAppSettings
{
    /// <summary>
    /// A simple class that you can use as a base class for a <see cref="TypeConverter"/>.
    /// Typically, you inherit from this class and use that class in conjunction with the
    /// <see cref="TypeConverterAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The type that should be supported for conversion from and to strings</typeparam>
    public class QuickConverter<T> : TypeConverter
    {
        private readonly Func<string, T> _convertFrom;
        private readonly Func<T, string> _convertTo;

        /// <summary>
        /// Provide a converter from string to the desired type, optionally provide the reverse way.
        /// </summary>
        /// <param name="convertFrom"></param>
        /// <param name="convertTo"></param>
        public QuickConverter(Func<string,T> convertFrom, Func<T,string> convertTo = null)
        {
            _convertFrom = convertFrom;
            _convertTo = convertTo;
        }


        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from. </param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture. </param><param name="value">The <see cref="T:System.Object"/> to convert. </param><exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
                return _convertFrom((string) value);
            return base.ConvertFrom(context, culture, value);
        }


        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed. </param><param name="value">The <see cref="T:System.Object"/> to convert. </param><param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to. </param><exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception><exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
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