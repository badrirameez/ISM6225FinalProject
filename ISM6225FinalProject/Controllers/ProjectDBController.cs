using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ISM6225FinalProject.DataAccess;
using ISM6225FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ISM6225FinalProject.Controllers
{
    public class ProjectDBController : Controller
    {
        public ApplicationDbContext dbContext;
        private const string V = "next_page_token";
        HttpClient httpClient;
        static string key = "AIzaSyCrx5fNyMcalr4hY8KJ2C5bDjtp8uNWNfQ"; // Add API key here

        public ProjectDBController(ApplicationDbContext context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(String firstName, String lastName,
                                 String email, String phone,
                                 String userName, String password)
        {
            UserLogin user = new UserLogin();

            Console.WriteLine("firstName " + firstName);
            Console.WriteLine("lastName " + lastName);
            Console.WriteLine("userName " + userName);
            Console.WriteLine("password " + password);

            if (!String.IsNullOrEmpty(firstName) && !String.IsNullOrEmpty(lastName)
                && !String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
            {

                user.firstName = firstName;
                user.lastName = lastName;
                user.email = email;
                user.phoneNumber = phone;
                user.user = userName;
                user.password = password;
                dbContext.Add(user);

                dbContext.SaveChanges();
                ViewBag.MandatoryCheck = "Congratulations, you're in! We're proud to have you as a member.";
            }
            else
            {
                ViewBag.MandatoryCheck = "Fields marked in '*' are mandatory!";
            }

            return View("UserLogin");
        }

        [HttpPost]
        public ActionResult Plan(String user1, String password1, String destination)
        {
            List<Places> FinalResult = new List<Places>();
            DataTable dt = new DataTable();
            List<object> iData_Attraction = new List<object>();
            List<object> iData_Rating = new List<object>();

            var list = (from cust in dbContext.saveSearch
                        where cust.user.Equals(user1)
                        group cust by cust.destination
            into grp
                        select new
                        {
                            destination = grp.Key,
                            Count = grp.Count()
                        }).ToList();


            if (!String.IsNullOrEmpty(user1) || !String.IsNullOrEmpty(password1) && password1 != "")
            {
                Console.WriteLine("Inside Login Call " + user1);
                Console.WriteLine("user1 " + user1);
                Console.WriteLine("usedestinationr1 " + destination);

                int usrCount =
                    (from usr in dbContext.userlogin
                     where usr.user.Equals(user1) && usr.password.Equals(password1)
                     select usr).Count();
                if (usrCount == 0)
                {
                    ViewBag.MandatoryCheck = "Please enter a valid Username or Password.";
                    return View("UserLogin");
                }
                else
                {
                    ViewBag.rep = JsonConvert.SerializeObject(list.Select(x => x.Count));
                    ViewBag.hod = JsonConvert.SerializeObject(list.Select(x => x.destination));
                    return View(FinalResult);
                }

            }

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", key);

            saveSearch save = new saveSearch();
            save.user = user1;
            save.destination = destination;
            dbContext.Add(save);
            dbContext.SaveChanges();

            httpClient.DefaultRequestHeaders.Accept.Add(
              new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string URL = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=%27" + destination + "%27+point+of+interest&language=en&key=" + key;

            string Google_Path = URL;
            string parksdata = "";
            string nextpagetoken = "";
            Places places = null;
            httpClient.BaseAddress = new Uri(Google_Path);
            try
            {

                PlanValidation Plan = new PlanValidation();
                Plan.location2 = destination;
                if (!String.IsNullOrEmpty(Plan.location2))
                {

                    HttpResponseMessage response = httpClient.GetAsync(Google_Path).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        // Asynchronous for wait till get result, common for remote operation                    
                        parksdata = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    }

                    if (!parksdata.Equals(""))
                    {
                        // Deserialize parksdata string into the Parks Object. 
                        //parks = JsonConvert.DeserializeObject<Root>(parksdata);
                        //response_parse = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(parksdata));

                        while (true)
                        {
                            parksdata = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            places = JsonConvert.DeserializeObject<Places>(parksdata);
                            FinalResult.Add(places);

                            object has_next_page = places.GetType().GetProperty("next_page_token");
                            if (!has_next_page.Equals("") || has_next_page is null) /// Check how to handle Null, should we use === while comparing string null (actual NULL)?? . 
                            {
                                Console.WriteLine("the value is:" + has_next_page);
                                break;
                            }
                            else
                            {
                                nextpagetoken = Convert.ToString(places.next_page_token);
                                string URL_Copy = URL;
                                string URL_Final = URL_Copy + "&pagetoken=" + nextpagetoken;
                                response = httpClient.GetAsync(URL_Final).GetAwaiter().GetResult();

                            }
                        }

                    }
                    ViewBag.TopAttraction = "Here are the best places to visit in " + Plan.location2 + ".";



                    foreach (ISM6225FinalProject.Models.Places p in FinalResult)
                    {
                        foreach (Place r in p.results)
                        {
                            iData_Attraction.Add(r.name);
                        }
                    }

                    var json_Attraction = JsonConvert.SerializeObject(iData_Attraction);
                    Console.WriteLine("json_Attraction " + json_Attraction);
                    ViewBag.iData_Attraction = json_Attraction;

                    foreach (ISM6225FinalProject.Models.Places p in FinalResult)
                    {
                        foreach (Place r in p.results)
                        {
                            iData_Rating.Add(r.user_ratings_total);
                        }
                    }
                    var json_Rating = JsonConvert.SerializeObject(iData_Rating);
                    Console.WriteLine("json_Rating " + json_Rating);
                    ViewBag.iData_Rating = json_Rating;

                    list = (from cust in dbContext.saveSearch
                            where cust.user.Equals(user1)
                            group cust by cust.destination
                            into grp
                            select new
                            {
                                destination = grp.Key,
                                Count = grp.Count()
                            }).ToList();

                    ViewBag.rep = JsonConvert.SerializeObject(list.Select(x => x.Count));
                    ViewBag.hod = JsonConvert.SerializeObject(list.Select(x => x.destination));


                    return View(FinalResult);
                }
                else
                {
                    return View(FinalResult);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }



            return View(FinalResult);
        }

    }
}
