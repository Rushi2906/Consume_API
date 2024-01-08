using Fetch_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Fetch_API.Controllers
{
    public class PersonController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        Uri baseAddress = new Uri("http://localhost:5186/api/Person");
        private readonly HttpClient _httpClient;

        public PersonController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        #region GetAll

        [HttpGet]
        public IActionResult Get()
        {
            List<PersonModel> persons = new List<PersonModel>();
            HttpResponseMessage response = _httpClient.GetAsync(baseAddress).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;

                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataOfObject = jsonobject.data;
                var extractDataJson = JsonConvert.SerializeObject(dataOfObject);

                persons = JsonConvert.DeserializeObject<List<PersonModel>>(extractDataJson);
            }
            return View("PersonList", persons);
        }

        #endregion

        #region Add / Edit

        [HttpGet]
        public IActionResult Edit(int PersonID)
        {
            PersonModel persons = new PersonModel();

            Console.WriteLine(_httpClient.BaseAddress);
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/{PersonID}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;

                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataOfObject = jsonobject.data;
                var extractDataJson = JsonConvert.SerializeObject(dataOfObject, Formatting.Indented);

                persons = JsonConvert.DeserializeObject<PersonModel>(extractDataJson);
            }
            return View("Create", persons);



        }

        [HttpPost]
        public async Task<IActionResult> Save(PersonModel model)
        {
            try
            {
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StringContent(model.Name), "Name");
                formData.Add(new StringContent(model.Contact), "Contact");
                formData.Add(new StringContent(model.Email), "Email");

                if (model.PersonID == 0)
                {
                    HttpResponseMessage res = await _httpClient.PostAsync($"{_httpClient.BaseAddress}", formData);
                    if (res.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Data Added Successfully.";
                        return RedirectToAction("Get");
                    }

                }
                else
                {
                    HttpResponseMessage res = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/{model.PersonID}", formData);
                    if (res.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Data Updated Successfully.";
                        return RedirectToAction("Get");
                    }
                }
                return View("Create");
            }
            catch (Exception e)
            {
                TempData["Message"] = e.Message;

            }

            return View("PersonList");
        }

        #endregion

        #region Delete

        [HttpGet("{PersonID}")]
        public IActionResult Delete(int PersonID)
        {
            HttpResponseMessage response = _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/{PersonID}").Result;
            Console.WriteLine(PersonID);
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Person Deleted successfully...!!!";
            }
            return RedirectToAction("Get");
        }

        #endregion


    }
}
