using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

namespace Assignment.MiscFile
{
    public class BaseClass
    {
        public static ChromeDriver chromeDriver;

        private const int WaitSeconds = 30;
        private static JsonNode jsonData;

        public void ChromeDriverInitialized()
        {
            var options = new ChromeOptions();
            // eager so it doesn't wait for images/fonts, site loads slow
            options.PageLoadStrategy = PageLoadStrategy.Eager;
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddArgument("--incognito");
            options.AddArgument("--disable-notifications");
            if (GetDataBool("blockimages"))
            {
                options.AddUserProfilePreference("profile.managed_default_content_settings.images", 2);
            }

            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            chromeDriver = new ChromeDriver(service, options, TimeSpan.FromMinutes(2));
            chromeDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(2);
            chromeDriver.Manage().Window.Maximize();
        }

        public void OpenWindowAndAccessURL()
        {
            chromeDriver.Navigate().GoToUrl(GetData("url"));
        }

        public void CloseWindow()
        {
            if (chromeDriver == null)
            {
                return;
            }
            try
            {
                chromeDriver.Quit();
            }
            catch (WebDriverException)
            {
            }
            chromeDriver = null;
        }

        // reading values from Data.json
        public static string GetData(string key)
        {
            JsonNode value = LoadData()[key];
            if (value == null)
            {
                throw new KeyNotFoundException("Key '" + key + "' is not in Data.json");
            }
            return value.ToString();
        }

        public static List<string> GetDataList(string key)
        {
            return LoadData()[key].AsArray().Select(item => item.ToString()).ToList();
        }

        public static bool GetDataBool(string key)
        {
            return bool.Parse(GetData(key));
        }

        public static int GetDataInt(string key)
        {
            return int.Parse(GetData(key), CultureInfo.InvariantCulture);
        }

        // site takes date as dd/MM/yyyy
        public static string GetVisitDate(int daysFromToday)
        {
            return DateTime.Today.AddDays(daysFromToday).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private static JsonNode LoadData()
        {
            if (jsonData == null)
            {
                jsonData = JsonNode.Parse(File.ReadAllText(FindDataFile()));
            }
            return jsonData;
        }

        // Data.json is copied to the bin folder, if not found check parent folders
        private static string FindDataFile()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            while (!string.IsNullOrEmpty(dir))
            {
                string file = Path.Combine(dir, "DataFile", "Data.json");
                if (File.Exists(file))
                {
                    return file;
                }
                DirectoryInfo parent = Directory.GetParent(dir);
                dir = parent == null ? null : parent.FullName;
            }
            throw new FileNotFoundException("DataFile\\Data.json not found");
        }

        // common element methods
        private static WebDriverWait NewWait()
        {
            var wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(WaitSeconds));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            return wait;
        }

        public static IWebElement WaitForElement(By by)
        {
            return NewWait().Until(driver =>
            {
                IWebElement element = driver.FindElement(by);
                return element.Displayed ? element : null;
            });
        }

        public static void Click(By by)
        {
            // page can reload between finding and clicking, so try again if element goes stale
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    WaitForElement(by).Click();
                    return;
                }
                catch (StaleElementReferenceException)
                {
                }
            }
            WaitForElement(by).Click();
        }

        public static void Type(By by, string text)
        {
            IWebElement element = WaitForElement(by);
            element.Clear();
            element.SendKeys(text);
        }

        public static string GetText(By by)
        {
            return WaitForElement(by).Text.Trim();
        }

        // after a click the old page can still be showing, so wait till the heading has
        // the text we expect. if it never comes just return what is there, assert will show it
        public static string WaitAndGetText(By by, string expected)
        {
            try
            {
                NewWait().Until(driver => driver.FindElement(by).Text.Trim() == expected);
            }
            catch (WebDriverTimeoutException)
            {
            }
            return GetText(by);
        }

        public static bool IsDisplayed(By by)
        {
            try
            {
                return WaitForElement(by).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}
