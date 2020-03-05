using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GraphiQl.Tests
{
    public abstract class BaseTest
    {
        protected ChromeDriver Driver { get; }
        protected bool RunHeadless { get; set;  } = false;

        protected BaseTest()
        {
            var options = new ChromeOptions();
            /*
            options.AddArgument("--remote-debugging-port=9222");
            */
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--no-sandbox");
    
            if (RunHeadless)
                options.AddArgument("--headless");
            
            Driver = new ChromeDriver(options);
        }

        protected void RunTest(Action<IWebDriver> execute)
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
                Driver.Close();
            }
        }
    }
}