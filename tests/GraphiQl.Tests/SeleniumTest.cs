using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GraphiQl.Tests
{
    public abstract class SeleniumTest
    {
        private ChromeDriver Driver { get; }
        protected bool RunHeadless { get; set;  } = false;

        protected SeleniumTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--no-sandbox");
    
            if (RunHeadless)
                options.AddArgument("--headless");
            
            Driver = new ChromeDriver(options);
        }

        protected void RunTest(Action<ChromeDriver> execute)
        {
            try
            {
                execute(Driver);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Driver.Quit();
            }
        }
    }
}