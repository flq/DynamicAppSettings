using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;

namespace DynamicAppSettings.Tests
{
    public abstract class DynamicAppSettingsContext
    {
        private dynamic _appSettings;

        protected dynamic AppSettings
        {
            get { return _appSettings; }
        }

        [TestFixtureSetUp]
        public void Given()
        {
            var values = GetConfigurationContents();
            NameValueCollection c = new NameValueCollection();
            foreach (var v in values)
                c.Add(v.Key, v.Value);

            _appSettings = new AppSettings(c);
        }

        protected abstract IEnumerable<KeyValuePair<string, string>> GetConfigurationContents();

        [TestFixtureTearDown]
        public void End()
        {
            DynamicAppSettings.AppSettings.Separator = ".";
        }
    }
}