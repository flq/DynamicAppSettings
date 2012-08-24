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
