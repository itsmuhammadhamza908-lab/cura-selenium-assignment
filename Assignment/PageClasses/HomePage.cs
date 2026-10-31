using OpenQA.Selenium;
using Assignment.LocatorFile;
using Assignment.MiscFile;

namespace Assignment.PageClasses
{
    public class HomePage : BaseClass
    {
        public string GetTitle()
        {
            return WaitAndGetText(By.TagName(LocatorClass.HomeHeading), GetData("hometitle"));
        }

        public bool IsMakeAppointmentButtonDisplayed()
        {
            return IsDisplayed(By.Id(LocatorClass.MakeAppointmentButton));
        }

        public void ClickMakeAppointment()
        {
            Click(By.Id(LocatorClass.MakeAppointmentButton));
        }
    }
}
