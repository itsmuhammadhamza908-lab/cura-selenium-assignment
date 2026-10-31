using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Assignment.LocatorFile;
using Assignment.MiscFile;
using System;
using System.Threading;

namespace Assignment.PageClasses
{
    public class SideMenu : BaseClass
    {
        public void Open()
        {
            Click(By.Id(LocatorClass.MenuToggle));

            // wait for the menu to open fully, otherwise click misses the link
            var wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Id(LocatorClass.Sidebar)).GetAttribute("class").Contains("active"));
            Thread.Sleep(800);
        }

        public void ClickHome()
        {
            Open();
            Click(By.LinkText(LocatorClass.HomeLink));
        }

        public void ClickHistory()
        {
            Open();
            Click(By.LinkText(LocatorClass.HistoryLink));
        }

        public void ClickProfile()
        {
            Open();
            Click(By.LinkText(LocatorClass.ProfileLink));
        }

        public void ClickLogout()
        {
            Open();
            Click(By.LinkText(LocatorClass.LogoutLink));
        }
    }
}
