/*
 * Selenium based script | https://github.com/SeleniumHQ/selenium
 * Why is this open sourced?
 * - Anyone with a brain cell can make a selenium script.
 * - If I sold this script and knew if I was using selenium,
 * theres no point of selling it if it's this easy to make a ShoePalace selenium script.
 * This posed no challenge to me at all. Anyone that is learning about Selenium can easily replicate this in a better fashion.
 *
 * Auto checkout is not complete. I haven't even began working on it. 
 * Next push to the repo should have it. 
 * 
 * Yeah if you think I stole any part of this project, LOL what you gotta say, compare your project to mine you fucking skid
 * Shame that people are selling selenium based scripts in the first place.
 * 
 * 
 * Message me if you have any questions. I will respond to your questions. 
 * After I'm done with this project, I am working on a private script using Puppeteer. 
 * 
 * 
 * Todo:
 * - Checkout using the data identifier 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using FlatUI;
using OpenQA.Selenium.Support.UI;

namespace ShoePalaceBot
{
    class ShoePalaceTask
    {
        private ChromeDriver Driver;
        public string productName = string.Empty;
        public FlatTextBox textBox;
        public int monitorDelay;
        public int retryDelay;
        private bool isItemAvailable;

        public ShoePalaceTask() {
            var Options = new ChromeOptions();
            Options.AddArguments(new List<string>() { "--enable-precise-memory-info", "--ignore-certificate-errors", "--enable-fast-web-scroll-view-insets", "--enable-auto-reload" });
            Driver = new ChromeDriver(Options); 
        }
        private void Output(string Data) => textBox.Invoke(new Action(() => { textBox.Text += "\r\n" + Data; }));
        private bool isElementPresent(By Element) { /* Shout out to Stack, who the fuck knew Selenium didn't have a function to check if a element exists or not? */
            try {
                Driver.FindElement(Element);
                return true;
            } catch (NoSuchElementException) { return false; }
        }

        private void checkAvailability() {

            Output("Checking availability.");
            if (isElementPresent(By.XPath("//*[@id='background']/div[4]/h2[2]")) == true) {
                if (Driver.FindElement(By.XPath("//*[@id='background']/div[4]/h2[2]")).Text.Contains("CURRENTLY OUT-OF-STOCK")) { Output("Item is currently out of stock."); isItemAvailable = false; }
            }
            else { isItemAvailable = true; Output("Stock detected."); };
        }

        public void getProductDetails(string ProductLink)
        {
            Driver.Navigate().GoToUrl(ProductLink);

            Output("Waiting..");
            while (isElementPresent(By.XPath("//*[@id='background']/div[4]/h2")) == false) // Let's just wait for this element to appear! Delay by 100 milliseconds until it appears. This basically handles cloudflare.
                // Any one with a brain cell can easily just wait for it to be present instead of fucking waiting/delaying up to 5 seconds.
                Task.Delay(100);

            while (Driver.PageSource.Contains("Error 1015") || Driver.PageSource.Contains("Service Unavailable") || Driver.PageSource.Contains("Enchance your calm"))
            {
                Output("Access denied.");
                Task.Delay(retryDelay);
                Driver.Navigate().Refresh();
            }

            Output("Extracting product details.");
            productName = Driver.FindElement(By.XPath("//*[@id='background']/div[4]/h2")).Text;

            Output("Product detected: " + productName);
            checkAvailability();
            if (!isItemAvailable) { Output("Item is currently not available. No sizes are loaded."); Driver.Quit(); return; }
        }

        private void checkIfSizeAvailability(string Size) // A shitty monitoring fucking function
        {
            while (isElementPresent(By.XPath("//button[text()=\"" + Size + "\"]")) == false) {
                Output("Monitoring."); 
                Task.Delay(monitorDelay);
            }
        }

        public void addToCart(string Size)  {
            checkIfSizeAvailability(Size);
            Output("Attempting to add to cart.");
            while (isElementPresent(By.XPath("//button[text()=\"" + Size + "\"]"))) // Before I was using the full xpath and parsing all buttons if it contains the size.
                                                                                            // Researching more sure works.
            {
                Output("Selecting offer.");
                Driver.FindElement(By.XPath("//button[text()=\"" + Size + "\"]")).Click();
                while (isElementPresent(By.XPath("//*[@id='oCartSubmit']"))) {
                    Driver.FindElement(By.XPath("//*[@id='oCartSubmit']")).Click(); 
                }
            }
        }


        private void checkAndHandleTimer()
        {
            while (Driver.Url.Contains("https://www.shoepalace.com/checkout/cart/add/product/")) { // Just fucking refresh the page when it hits the timer page you dumb shit
                Thread.Sleep(retryDelay);
                Driver.Navigate().Refresh(); // Refresh the fucking page
            }
        }

        public void Checkout() {
            checkAndHandleTimer();
            while (Driver.Url == "https://shoepalace.com/checkout/cart")
                Driver.Navigate().GoToUrl("https://www.shoepalace.com/onestepcheckout/");
        }
    }
}
