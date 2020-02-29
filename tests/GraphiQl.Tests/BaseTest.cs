using System;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace GraphiQl.Tests
{
    public abstract class BaseTest
    {
        protected ChromeDriver Driver { get; }

        protected BaseTest(bool runHeadless)
        {
            var options = new ChromeOptions();
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--no-sandbox");
    
            if (runHeadless)
                options.AddArgument("--headless");
            
            Driver = new ChromeDriver(options);
        }

        protected async Task RunTest(Func<ChromeDriver, Task> execute)
        {
            try
            {
                await execute(Driver);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Driver.Close();
            }
        }
    }
}