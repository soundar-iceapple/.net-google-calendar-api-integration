using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Newtonsoft.Json;

public class ServiceAccount
{
    public string type { get; set; }
    public string project_id { get; set; }
    public string private_key_id { get; set; }
    public string private_key { get; set; }
    public string client_email { get; set; }
    public string client_id { get; set; }
    public string auth_uri { get; set; }
    public string token_uri { get; set; }
    public string auth_provider_x509_cert_url { get; set; }
    public string client_x509_cert_url { get; set; }
    public string universe_domain { get; set; }
}

namespace Test
{
    /// <summary>
    /// Sample which demonstrates how to use the Books API.
    /// https://developers.google.com/books/docs/v1/getting_started
    /// <summary>
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string impersonateUser = "user-impersonate@test.com";
            string calendarId = "primary";


            var jsonData = System.IO.File.ReadAllText("client_secret.json");
            ServiceAccount serviceAccount = JsonConvert.DeserializeObject<ServiceAccount>(jsonData);

            //var credential = ServiceAccountCredential.FromServiceAccountData(new FileStream("service.json", FileMode.Open));

            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccount.client_email)
                {
                    Scopes = new[] { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents },
                    User = impersonateUser
                }.FromPrivateKey(serviceAccount.private_key));


            //var token = credential.GetAccessTokenForRequestAsync().Result;

            // Create the Calendar service using the provided key file
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "application-name"
            });


            var newEvent = new Event
            {
                Summary = "Google meet",
                Description = "Sample meeting",
                Location = "Virtual Meeting",
                Start = new EventDateTime
                {
                    DateTime = DateTime.Now.AddHours(2), // Set the start time
                    TimeZone = "Asia/Kolkata"
                },
                End = new EventDateTime
                {
                    DateTime = DateTime.Now.AddHours(3), // Set the end time
                    TimeZone = "Asia/Kolkata"
                },
                ConferenceData = new ConferenceData
                {
                    CreateRequest = new CreateConferenceRequest
                    {
                        RequestId = Guid.NewGuid().ToString(),
                        ConferenceSolutionKey = new ConferenceSolutionKey { Type = "hangoutsMeet" }
                    }
                },
                Attendees = new List<EventAttendee>
                {
                    new EventAttendee()
                        {
                            DisplayName = "organizer",
                            Email = "organizer@email.com",
                            Organizer = true,
                            Resource = false,
                        },
                    new EventAttendee()
                        {
                            DisplayName = "test1",
                            Email = "test1@email.com",
                            Organizer = false
                        }
                }
            };



            EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
            request.SendNotifications = true;
            request.ConferenceDataVersion = 1;
            var createdEvent = request.Execute();

        }


    }
}