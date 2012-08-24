using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.Linq;

namespace DynamicAppSettings
{
    /// <summary>
    /// The basic class to access AppSettings. Use it as a dynamic Object
    /// </summary>
    public class AppSettings : DynamicObject
    {
        /// <summary>
        /// Change this to the separator you want to use in your app.config keys
        /// </summary>
        public static string Separator = ".";

        private readonly Dictionary<string,string> _settings;
        private readonly Dictionary<string,AppSettings> _nestedValues = new Dictionary<string, AppSettings>();

        /// <summary>
        /// Default ctor. This one loads its values from <see cref="ConfigurationManager.AppSettings"/>
        /// </summary>
        public AppSettings() : this(ConfigurationManager.AppSettings) { }

        /// <summary>
        /// Ctor where you provide the values that should be read
        /// </summary>
        public AppSettings(NameValueCollection settings)
        {
            _settings = new Dictionary<string, string>();
            foreach (string key in settings)
                _settings.Add(key, settings[key]);
        }

        private AppSettings(IDictionary<string, string> nestedValues)
        {
            _settings = new Dictionary<string, string>(nestedValues);
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param><param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string key = binder.Name;
            string tmp;
            if (_settings.TryGetValue(key, out tmp))
            {
                result = new Converter(tmp);
            }
            else
            {
                TryFindNested(key, out result);
            }
            return true;
        }

        private void TryFindNested(string key, out object result)
        {
            AppSettings nested;
            if (!_nestedValues.TryGetValue(key, out nested))
            {
                nested = TryConstructNested(key);
            }
            result = nested;
        }

        private AppSettings TryConstructNested(string key)
        {
            AppSettings nested = null;
            var keyAsRoot = key + Separator;
            if (_settings.Any(kv => kv.Key.StartsWith(keyAsRoot)))
            {
                var nestedValues = from kv in _settings
                                   where kv.Key.StartsWith(keyAsRoot)
                                   let newKey = kv.Key.Substring(keyAsRoot.Length)
                                   select new {newKey, kv.Value};

                nested = new AppSettings(nestedValues.ToDictionary(v => v.newKey, v => v.Value));
                _nestedValues.Add(key, nested);
            }
            return nested;
        }
    }

    internal class Converter : DynamicObject
    {
        private readonly string _value;

        public Converter(string value)
        {
            _value = value;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == typeof(string))
                result = _value;
            if (binder.ReturnType == typeof(object))
                result = _value;

            TypeConverter sC = TypeDescriptor.GetConverter(binder.ReturnType);
            if (sC.CanConvertFrom(typeof(string)))
                result = sC.ConvertFrom(_value);
            else
                throw new InvalidCastException("Cannot convert string to " + binder.ReturnType.Name);
            return true;
        }
    }
}
