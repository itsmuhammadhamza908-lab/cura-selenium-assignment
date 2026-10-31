using OpenQA.Selenium;
using Assignment.LocatorFile;
using Assignment.MiscFile;

namespace Assignment.PageClasses
{
    public class HistoryPage : BaseClass
    {
        public string GetTitle()
        {
            return WaitAndGetText(By.TagName(LocatorClass.PageHeading), GetData("historytitle"));
        }
    }
}
