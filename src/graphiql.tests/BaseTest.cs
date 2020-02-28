using System;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace GraphiQl.tests
{
    public abstract class BaseTest
    {
        protected ChromeDriver Driver { get; }

        protected BaseTest(bool runHeadless)
        {
            var options = new ChromeOptions();
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument("--no-sandbox");
            if (runHeadless)
                options.AddArgument("headless");
            
            /*
            options.add_argument('headless')
            options.add_argument('--disable-infobars')
            options.add_argument('--disable-dev-shm-usage')
            options.add_argument('--no-sandbox')
            options.add_argument('--remote-debugging-port=9222')
            */
            
            
            Driver = new ChromeDriver(options);
        }

        protected async Task RunTest(Func<ChromeDriver, Task> execute)
        {
            try
            {
                await execute(Driver);
            }
            catch (Exception e)
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