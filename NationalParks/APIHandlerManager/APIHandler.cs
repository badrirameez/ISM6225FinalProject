using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using ISM6225FinalProject.Models;

namespace ISM6225FinalProject.APIHandlerManager
{
  public class APIHandler : IEnumerator, IEnumerable
    {
    // Obtaining the API key is easy. The same key should be usable across the entire
    // data.gov developer network, i.e. all data sources on data.gov.
    // https://www.nps.gov/subjects/developer/get-started.htm

    static string BASE_URL = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=tampa+point+of+interest&language=en&key=";
    static string API_KEY = "AIzaSyCrx5fNyMcalr4hY8KJ2C5bDjtp8uNWNfQ"; //Add your API key here inside ""
     
    HttpClient httpClient;

    /// <summary>
    ///  Constructor to initialize the connection to the data source
    /// </summary>
    public APIHandler()
    {
      httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Accept.Clear();
      httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
      httpClient.DefaultRequestHeaders.Accept.Add(
          new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Method to receive data from API end point as a collection of objects
    /// 
    /// JsonConvert parses the JSON string into classes
    /// </summary>
    /// <returns></returns>
    public Places GetPlaces()
    {
      string NATIONAL_PARK_API_PATH = BASE_URL + API_KEY;
      string placeData = "";
      string nextpagetoken = "";
      Places places = null;

      httpClient.BaseAddress = new Uri(NATIONAL_PARK_API_PATH);
        // It can take a few requests to get back a prompt response, if the API has not received
      //  calls in the recent past and the server has put the service on hibernation
      try
      {
        HttpResponseMessage response = httpClient.GetAsync(NATIONAL_PARK_API_PATH).GetAwaiter().GetResult();
        if (response.IsSuccessStatusCode)
        {
          placeData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        if (!placeData.Equals(""))
        {
                    // Deserialize parksdata string into the Places Object. 
                    //places = JsonConvert.DeserializeObject<Root>(parksdata);
                    //response_parse = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(parksdata));

                    while (true)
                    {
                        placeData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        places = JsonConvert.DeserializeObject<Places>(placeData);



                        object has_next_page = places.GetType().GetProperty("next_page_token");
                        if (!has_next_page.Equals("") || has_next_page is null) /// Check how to handle Null, should we use is null while comparing string null (actual NULL)?? . 
                        {
                            Console.WriteLine("the value is:" + has_next_page);
                            break;
                        }
                        else
                        {
                            nextpagetoken = Convert.ToString(places.next_page_token);
                            string URL_Copy = NATIONAL_PARK_API_PATH;
                            string URL_Final = URL_Copy + "&pagetoken=" + nextpagetoken;
                            response = httpClient.GetAsync(URL_Final).GetAwaiter().GetResult();

                        }

                    }
                }

     }
      catch (Exception e)
      {
        // This is a useful place to insert a breakpoint and observe the error message
        Console.WriteLine(e.Message);
      }

      return places;
    }
  }
}