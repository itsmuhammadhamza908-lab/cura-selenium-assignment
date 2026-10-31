using OpenQA.Selenium;
using Assignment.LocatorFile;
using Assignment.MiscFile;

namespace Assignment.PageClasses
{
    public class SummaryPage : BaseClass
    {
        public string GetTitle()
        {
            return WaitAndGetText(By.TagName(LocatorClass.PageHeading), GetData("summarytitle"));
        }

        public string GetFacility()
        {
            return GetText(By.Id(LocatorClass.SummaryFacility));
        }

        public string GetReadmission()
        {
            return GetText(By.Id(LocatorClass.SummaryReadmission));
        }

        public string GetProgram()
        {
            return GetText(By.Id(LocatorClass.SummaryProgram));
        }

        public string GetVisitDate()
        {
            return GetText(By.Id(LocatorClass.SummaryVisitDate));
        }

        public string GetComment()
        {
            return GetText(By.Id(LocatorClass.SummaryComment));
        }

        public void ClickGoToHomepage()
        {
            Click(By.LinkText(LocatorClass.GoToHomepageLink));
        }
    }
}
