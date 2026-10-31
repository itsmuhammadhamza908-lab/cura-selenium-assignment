using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Assignment.LocatorFile;
using Assignment.MiscFile;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.PageClasses
{
    public class AppointmentPage : BaseClass
    {
        public string GetTitle()
        {
            return WaitAndGetText(By.TagName(LocatorClass.PageHeading), GetData("appointmenttitle"));
        }

        public void SelectFacility(string facility)
        {
            new SelectElement(WaitForElement(By.Id(LocatorClass.Facility))).SelectByText(facility);
        }

        public List<string> GetFacilities()
        {
            var dropDown = new SelectElement(WaitForElement(By.Id(LocatorClass.Facility)));
            return dropDown.Options.Select(option => option.Text.Trim()).ToList();
        }

        public void SetReadmission(bool apply)
        {
            IWebElement checkbox = WaitForElement(By.Id(LocatorClass.ReadmissionCheckbox));
            if (checkbox.Selected != apply)
            {
                checkbox.Click();
            }
        }

        public void SelectProgram(string program)
        {
            Click(By.Id(LocatorClass.ProgramPrefix + program.ToLowerInvariant()));
        }

        public void EnterVisitDate(string date)
        {
            Type(By.Id(LocatorClass.VisitDate), date);
            // tab moves focus out of the field and closes the date picker
            WaitForElement(By.Id(LocatorClass.VisitDate)).SendKeys(Keys.Tab);
        }

        public void EnterComment(string comment)
        {
            Type(By.Id(LocatorClass.Comment), comment);
        }

        public void ClickBookAppointment()
        {
            Click(By.Id(LocatorClass.BookAppointmentButton));
        }

        public void BookAppointment(string facility, bool readmission, string program, string visitDate, string comment)
        {
            SelectFacility(facility);
            SetReadmission(readmission);
            SelectProgram(program);
            EnterVisitDate(visitDate);
            EnterComment(comment);
            ClickBookAppointment();
        }
    }
}
