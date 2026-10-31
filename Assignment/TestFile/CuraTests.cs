using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Assignment.MiscFile;
using Assignment.PageClasses;
using System;

namespace Assignment.TestFile
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("CURA Healthcare Automation")]
    public class CuraTests : BaseClass
    {
        private readonly HomePage homePage = new HomePage();
        private readonly LoginPage loginPage = new LoginPage();
        private readonly AppointmentPage appointmentPage = new AppointmentPage();
        private readonly SummaryPage summaryPage = new SummaryPage();
        private readonly HistoryPage historyPage = new HistoryPage();
        private readonly SideMenu sideMenu = new SideMenu();

        [SetUp]
        public void Setup()
        {
            ChromeDriverInitialized();
            OpenWindowAndAccessURL();
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                // attach screenshot to the allure report if test fails
                if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed && chromeDriver != null)
                {
                    byte[] screenshot = chromeDriver.GetScreenshot().AsByteArray;
                    AllureApi.AddAttachment("Screenshot on failure", "image/png", screenshot, ".png");
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                CloseWindow();
            }
        }

        // helper methods

        private void OpenLoginPage()
        {
            homePage.ClickMakeAppointment();
        }

        // login with the valid user
        private void LoginWithValidUser()
        {
            OpenLoginPage();
            loginPage.Login(GetData("username"), GetData("password"));
        }

        // returns the visit date so the test can compare it
        private string BookAppointment(string facility, bool readmission, string program, string comment)
        {
            string visitDate = GetVisitDate(GetDataInt("visitdaysoffset"));
            appointmentPage.BookAppointment(facility, readmission, program, visitDate, comment);
            return visitDate;
        }

        // Login

        [Test]
        [AllureSeverity(SeverityLevel.blocker)]
        [AllureFeature("Login")]
        [AllureDescription("TC-101: verify valid login details")]
        public void TC101_ValidLogin()
        {
            OpenLoginPage();

            loginPage.Login(GetData("username"), GetData("password"));

            Assert.AreEqual(GetData("appointmenttitle"), appointmentPage.GetTitle());
        }

        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureFeature("Login")]
        [AllureDescription("TC-102: verify invalid login details")]
        public void TC102_InvalidLogin()
        {
            OpenLoginPage();

            loginPage.Login(GetData("InvalidUserName"), GetData("InvalidPassword"));

            Assert.AreEqual(GetData("loginerror"), loginPage.GetErrorMessage());
        }

        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureFeature("Login")]
        [AllureDescription("TC-103: verify valid username with wrong password cannot login")]
        public void TC103_WrongPasswordCannotLogin()
        {
            OpenLoginPage();

            loginPage.Login(GetData("username"), GetData("InvalidPassword"));

            Assert.AreEqual(GetData("loginerror"), loginPage.GetErrorMessage());
        }

        [Test]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureFeature("Login")]
        [AllureDescription("TC-104: verify the user must login before making an appointment")]
        public void TC104_AppointmentRequiresLogin()
        {
            homePage.ClickMakeAppointment();

            Assert.AreEqual(GetData("logintitle"), loginPage.GetTitle());
        }

        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureFeature("Login")]
        [AllureDescription("TC-105: verify logout button logs out from the application")]
        public void TC105_Logout()
        {
            LoginWithValidUser();
            Assert.AreEqual(GetData("appointmenttitle"), appointmentPage.GetTitle());

            sideMenu.ClickLogout();

            Assert.IsTrue(homePage.IsMakeAppointmentButtonDisplayed(), "Home page not shown after logout");
            homePage.ClickMakeAppointment();
            Assert.AreEqual(GetData("logintitle"), loginPage.GetTitle());
        }

        // Make Appointment form

        [Test]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureFeature("Make Appointment")]
        [AllureDescription("TC-106: verify the facilities offered in the Facility drop down")]
        public void TC106_FacilityListIsCorrect()
        {
            LoginWithValidUser();

            CollectionAssert.AreEquivalent(GetDataList("facilities"), appointmentPage.GetFacilities());
        }

        [TestCase("Tokyo CURA Healthcare Center")]
        [TestCase("Hongkong CURA Healthcare Center")]
        [TestCase("Seoul CURA Healthcare Center")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureFeature("Appointment Confirmation")]
        [AllureDescription("TC-107: verify the facility on the confirmation page is the selected facility")]
        public void TC107_ConfirmationShowsSelectedFacility(string facility)
        {
            LoginWithValidUser();

            BookAppointment(facility, GetDataBool("readmission"), GetData("program"), GetData("comment"));

            Assert.AreEqual(GetData("summarytitle"), summaryPage.GetTitle());
            Assert.AreEqual(facility, summaryPage.GetFacility());
        }

        [TestCase("Medicare")]
        [TestCase("Medicaid")]
        [TestCase("None")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureFeature("Appointment Confirmation")]
        [AllureDescription("TC-108: verify the healthcare program on the confirmation page is the selected program")]
        public void TC108_ConfirmationShowsSelectedProgram(string program)
        {
            LoginWithValidUser();

            BookAppointment(GetData("facility"), GetDataBool("readmission"), program, GetData("comment"));

            Assert.AreEqual(GetData("summarytitle"), summaryPage.GetTitle());
            Assert.AreEqual(program, summaryPage.GetProgram());
        }

        [TestCase(true, "Yes")]
        [TestCase(false, "No")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureFeature("Appointment Confirmation")]
        [AllureDescription("TC-109: verify the hospital readmission option is reflected on the confirmation page")]
        public void TC109_ConfirmationShowsReadmission(bool readmission, string expected)
        {
            LoginWithValidUser();

            BookAppointment(GetData("facility"), readmission, GetData("program"), GetData("comment"));

            Assert.AreEqual(GetData("summarytitle"), summaryPage.GetTitle());
            Assert.AreEqual(expected, summaryPage.GetReadmission());
        }

        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureFeature("Appointment Confirmation")]
        [AllureDescription("TC-110: verify the visit date on the confirmation page is the date entered")]
        public void TC110_ConfirmationShowsVisitDate()
        {
            LoginWithValidUser();

            string visitDate = BookAppointment(GetData("facility"), GetDataBool("readmission"), GetData("program"), GetData("comment"));

            Assert.AreEqual(GetData("summarytitle"), summaryPage.GetTitle());
            Assert.AreEqual(visitDate, summaryPage.GetVisitDate());
        }

        [Test]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureFeature("Appointment Confirmation")]
        [AllureDescription("TC-111: verify the comment on the confirmation page is the comment entered")]
        public void TC111_ConfirmationShowsComment()
        {
            LoginWithValidUser();

            BookAppointment(GetData("facility"), GetDataBool("readmission"), GetData("program"), GetData("comment"));

            Assert.AreEqual(GetData("summarytitle"), summaryPage.GetTitle());
            Assert.AreEqual(GetData("comment"), summaryPage.GetComment());
        }

        [Test]
        [AllureSeverity(SeverityLevel.blocker)]
        [AllureFeature("Appointment Confirmation")]
        [AllureDescription("TC-112: verify all the data on the confirmation page is the same as entered")]
        public void TC112_ConfirmationShowsAllEnteredData()
        {
            LoginWithValidUser();

            string visitDate = BookAppointment(GetData("facility"), GetDataBool("readmission"), GetData("program"), GetData("comment"));

            Assert.AreEqual(GetData("summarytitle"), summaryPage.GetTitle());
            Assert.AreEqual(GetData("facility"), summaryPage.GetFacility());
            Assert.AreEqual(GetDataBool("readmission") ? "Yes" : "No", summaryPage.GetReadmission());
            Assert.AreEqual(GetData("program"), summaryPage.GetProgram());
            Assert.AreEqual(visitDate, summaryPage.GetVisitDate());
            Assert.AreEqual(GetData("comment"), summaryPage.GetComment());
        }

        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureFeature("Make Appointment")]
        [AllureDescription("TC-113: verify an appointment cannot be booked without a visit date")]
        public void TC113_VisitDateIsRequired()
        {
            LoginWithValidUser();
            Assert.AreEqual(GetData("appointmenttitle"), appointmentPage.GetTitle());

            appointmentPage.SelectFacility(GetData("facility"));
            appointmentPage.SelectProgram(GetData("program"));
            appointmentPage.ClickBookAppointment();

            Assert.AreEqual(GetData("appointmenttitle"), appointmentPage.GetTitle());
            Assert.IsFalse(chromeDriver.Url.Contains("summary"), "Confirmation page opened without visit date");
        }

        // Navigation

        [Test]
        [AllureSeverity(SeverityLevel.minor)]
        [AllureFeature("Navigation")]
        [AllureDescription("TC-114: verify Go to Homepage on the confirmation page opens the home page")]
        public void TC114_GoToHomepageFromConfirmation()
        {
            LoginWithValidUser();
            BookAppointment(GetData("facility"), GetDataBool("readmission"), GetData("program"), GetData("comment"));
            Assert.AreEqual(GetData("summarytitle"), summaryPage.GetTitle());

            summaryPage.ClickGoToHomepage();

            Assert.AreEqual(GetData("hometitle"), homePage.GetTitle());
        }

        [Test]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureFeature("Navigation")]
        [AllureDescription("TC-115: verify the History page opens from the side menu")]
        public void TC115_HistoryPageOpensFromMenu()
        {
            LoginWithValidUser();

            sideMenu.ClickHistory();

            Assert.AreEqual(GetData("historytitle"), historyPage.GetTitle());
        }

        [Test]
        [AllureSeverity(SeverityLevel.minor)]
        [AllureFeature("Navigation")]
        [AllureDescription("TC-116: verify the title of every page reflects its objective")]
        public void TC116_PageTitlesAreCorrect()
        {
            Assert.AreEqual(GetData("hometitle"), homePage.GetTitle());

            homePage.ClickMakeAppointment();
            Assert.AreEqual(GetData("logintitle"), loginPage.GetTitle());

            loginPage.Login(GetData("username"), GetData("password"));
            Assert.AreEqual(GetData("appointmenttitle"), appointmentPage.GetTitle());

            BookAppointment(GetData("facility"), GetDataBool("readmission"), GetData("program"), GetData("comment"));
            Assert.AreEqual(GetData("summarytitle"), summaryPage.GetTitle());

            sideMenu.ClickHistory();
            Assert.AreEqual(GetData("historytitle"), historyPage.GetTitle());
        }
    }
}
