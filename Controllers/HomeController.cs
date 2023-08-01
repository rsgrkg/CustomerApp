using CustomerApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CustomerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<Customer> customerList= new List<Customer>();
            using (var httpClient=new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://getinvoices.azurewebsites.net/api/Customers"))
                {
                    string apiResponse= await response.Content.ReadAsStringAsync();
                    customerList=JsonConvert.DeserializeObject<List<Customer>>(apiResponse);
                }

            }
            TempData["Id"] = customerList.OrderByDescending(u => u.id).FirstOrDefault();
            return View(customerList);
        }

        public ViewResult AddCustomer() => View();

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            Customer receivedCustomer = new Customer();
            if (TempData["Id"] != null)
            {
                customer.id = TempData["Id"].ToString();
            }
                using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://getinvoices.azurewebsites.net/api/Customer", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    receivedCustomer = JsonConvert.DeserializeObject<Customer>(apiResponse);
                }
            }
            return View(receivedCustomer);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer(int id)
        {
            Customer customer = new Customer();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://getinvoices.azurewebsites.net/api/Customer/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    customer = JsonConvert.DeserializeObject<Customer>(apiResponse);
                }
            }
            return View(customer);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            Customer receivedCustomer = new Customer();
            using (var httpClient = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(customer.id.ToString()), "id");
                content.Add(new StringContent(customer.firstname), "firstname");
                content.Add(new StringContent(customer.lastname), "lastname");
                content.Add(new StringContent(customer.email), "email");
                content.Add(new StringContent(customer.phone_Number.ToString()), "phone_Number");
                content.Add(new StringContent(customer.country_code), "country_code");
                content.Add(new StringContent(customer.gender), "gender");
                content.Add(new StringContent(customer.balance.ToString()), "balance");

                using (var response = await httpClient.PutAsync("https://getinvoices.azurewebsites.net/api/Customer/", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    receivedCustomer = JsonConvert.DeserializeObject<Customer>(apiResponse);
                }
            }
            return View(receivedCustomer);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://getinvoices.azurewebsites.net/api/Customer/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}