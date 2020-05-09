/*
 * Project: ShoePalaceBot
 * File Name: ShoePalaceTask.cs
 * 
 * Written by Ascending
 * 
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

namespace ShoePalaceBot
{
    class ShoePalaceTask
    {
        private ChromeDriver Driver;
        public string productImage = string.Empty;
        public string productName = string.Empty;
        public FlatTextBox textBox;
        private bool isItemAvailable;

        public ShoePalaceTask() {
            var Options = new ChromeOptions();
            Options.AddArguments(new List<string>() { "--aggressive", "--fast-start", "--fast", "--enable-precise-memory-info", "--ignore-certificate-errors", "--enable-fast-web-scroll-view-insets", "--enable-auto-reload" });
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

        public void getProductDetails(string ProductLink) {

            if (ProductLink.Contains("shoepalace")) {
                Driver.Navigate().GoToUrl(ProductLink);

                Output("Extracting product details.");
                productName = Driver.FindElement(By.XPath("//*[@id='background']/div[4]/h2")).Text;

                Output("Product detected: " + productName);
                checkAvailability();
                if (!isItemAvailable) { Output("Item is currently not available. No sizes are loaded."); Driver.Quit(); return; }
            }
        }

        public void addToCart(string Size) {

            Output("Attempting to add to cart.");
            while (isElementPresent(By.XPath("//button[text()=\"" + Size + "\"]")) == true) {
                Driver.FindElement(By.XPath("//button[text()=\"" + Size + "\"]")).Click();
                while (isElementPresent(By.XPath("//*[@id='oCartSubmit']")) == true) {
                    Driver.FindElement(By.XPath("//*[@id='oCartSubmit']")).Click();
                }
            }
        }
        public void Checkout() {
            Output("Attempting to checkout.");
            while (Driver.Url.Contains("/checkout/cart")) { Driver.Navigate().GoToUrl("https://www.shoepalace.com/onestepcheckout/"); break; }
        }
    }
}
