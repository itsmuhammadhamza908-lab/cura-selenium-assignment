using OpenQA.Selenium;
using Assignment.LocatorFile;
using Assignment.MiscFile;

namespace Assignment.PageClasses
{
    public class LoginPage : BaseClass
    {
        public string GetTitle()
        {
            return WaitAndGetText(By.TagName(LocatorClass.PageHeading), GetData("logintitle"));
        }

        public void Login(string userName, string password)
        {
            Type(By.Id(LocatorClass.UserName), userName);
            Type(By.Id(LocatorClass.Password), password);
            Click(By.Id(LocatorClass.LoginButton));
        }

        public string GetErrorMessage()
        {
            return GetText(By.CssSelector(LocatorClass.LoginErrorMessage));
        }
    }
}
