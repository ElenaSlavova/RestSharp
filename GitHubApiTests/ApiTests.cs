using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace GitHubApiTests
{
    public class ApiTests
    {
        private RestClient client;
        private const string baseUrl = "https://api.github.com";
        private const string partialUrl = "/repos/ElenaSlavova/postman/issues";
        private const string username = "ElenaSlavova";
        private const string password = "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";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);
            // client.TimeOut= 3000;
            client.Authenticator = new HttpBasicAuthenticator(username,password);
        }
        
        [Test]
        [Timeout(1000)] 
        public void Test_GetSingleIssue()
        {

            var request = new RestRequest($"{partialUrl}/50", Method.Get);

            var response = this.client.Execute(request); 

            Assert.That (response.StatusCode, Is .EqualTo (HttpStatusCode.OK),"HTTP Status Code property");

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Assert.That(issue.title, Is.EqualTo("Forth issue"));
            Assert.That(issue.number, Is.EqualTo(50));

        }

        [Test]
        public void Test_GetSingleIssueComment()
        {

            var request = new RestRequest($"{partialUrl}/1/comments", Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");

            var comment = JsonSerializer.Deserialize<List<Comment>>(response.Content);

            Assert.That(comment[0].body, Is.EqualTo("other comments"));
            Assert.That(comment[1].body, Is.Not.EqualTo("other comments")); 
        }

        [Test]
        public void Test_GetSingleIssueById()
        {

            var request = new RestRequest($"{partialUrl}/comments/1415816415", Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");

            var comment = JsonSerializer.Deserialize<Comment>(response.Content);

            Assert.That(comment.id, Is.EqualTo(1415816415));
            Assert.That(comment.id, Is.Not.EqualTo(1234));
        }

        [Test]
        public void Test_GetAllIssue()
        {

            var request = new RestRequest($"{partialUrl}", Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");

            var issues = JsonSerializer.Deserialize<List<Issue>>(response.Content);

            foreach (var issue in issues)
            {
                Assert.That(issue.title, Is.Not.Empty);
                Assert.That(issue.number, Is.GreaterThan(0));
            }

        }

        [Test]
        public void Test_GetSingleIssueLabels()
        {

            var request = new RestRequest($"{partialUrl}/6", Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            string[] allLabelsName = new string[] { "bug", "documentation", "duplicate" };
            long[] allLabelsId = new long[] { 5101172515, 5101172521, 5101172526 };
            string[] allLabelsColor = new string[] { "d73a4a", "0075ca", "cfd3d7" };
            string[] allLabelsDescription = new string[] { "Something isn't working", "Improvements or additions to documentation", "This issue or pull request already exists" };

            foreach (var label in issue.labels)
            {
                Assert.That(label, Is.Not.Null);
                Assert.Contains(label.name, allLabelsName);

                Assert.Contains(label.id, allLabelsId);

                Assert.Contains(label.color, allLabelsColor);

                Assert.Contains(label.description, allLabelsDescription);
            }

            Assert.That(issue.labels[1].name, Is.EqualTo("documentation"));
            Assert.That(issue.labels[0].name, Is.Not.EqualTo("documentation"));
            Assert.That(issue.labels[1].name, Is.EqualTo(allLabelsName[1]));

            Assert.That(issue.labels[1].id, Is.EqualTo(5101172521));
            Assert.That(issue.labels[0].id, Is.Not.EqualTo(5101172521));
            Assert.That(issue.labels[1].id, Is.EqualTo(allLabelsId[1]));

            Assert.That(issue.labels[1].color, Is.EqualTo("0075ca"));
            Assert.That(issue.labels[0].color, Is.Not.EqualTo("0075ca"));
            Assert.That(issue.labels[1].color, Is.EqualTo(allLabelsColor[1]));

            Assert.That(issue.labels[2].description, Is.EqualTo("This issue or pull request already exists"));
            Assert.That(issue.labels[1].description, Is.Not.EqualTo("This issue or pull request already exists"));
            Assert.That(issue.labels[2].description, Is.EqualTo(allLabelsDescription[2]));
        }

        [Test]
        public void Test_GetSingleIssueWithLabels()
        {

            var request = new RestRequest($"{partialUrl}/6/labels", Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");

            var labels= JsonSerializer.Deserialize<List<Labels>>(response.Content);

            foreach (var label in labels)
            {
                Assert.That(label, Is.Not.Null);
  
            }

            Assert.That(labels[1].name, Is.EqualTo("documentation"));
            Assert.That(labels[0].name, Is.Not.EqualTo("documentation"));

            Assert.That(labels[1].color, Is.EqualTo("0075ca"));
            Assert.That(labels[2].color, Is.Not.EqualTo("0075ca"));

            Assert.That(labels[1].id, Is.EqualTo(5101172521));
            Assert.That(labels[0].id, Is.Not.EqualTo(5101172521));

            Assert.That(labels[1].description, Is.EqualTo("Improvements or additions to documentation"));
            Assert.That(labels[2].description, Is.Not.EqualTo("Improvements or additions to documentation"));
          

        }

        [Test]
        public void Test_CreateNewIssue()
        {
            var request = new RestRequest($"{partialUrl}", Method.Post);
            var issueBody = new
            {
                title = "Test issue from RestSharp" + DateTime.Now.Ticks,
                body = "some body for my issue",
                labels = new string[] { "bug", "documentation", "duplicate" }

            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "HTTP Status Code property");
            Assert.That(issue.number, Is.GreaterThan(0));
            Assert.That(issue.title, Is.EqualTo(issueBody.title));
            Assert.That (issue.body, Is.EqualTo(issueBody.body));   
        }

        [Test]
        public void Test_CreateNewIssueComment()
        {
            var request = new RestRequest($"{partialUrl}/1/comments", Method.Post);
            var issueBody = new
            {
                body = "CREATED: Hello comment"
                
            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "HTTP Status Code property");
            Assert.That(issue.body, Is.EqualTo(issueBody.body));
        }

        [Test]
        public void Test_CreateOnIssueLabels()
        {
            var request = new RestRequest($"{partialUrl}/68", Method.Post);
            var issueBody = new
            {
                 labels = new string[] { "bug", "moqtlable", "alabala" }

            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");
            Assert.That(issue.number, Is.GreaterThan(0));
            Assert.That(issue.labels[1].name, Is.EqualTo(issueBody.labels[1]));
            Assert.That(issue.labels[2].name, Is.Not.EqualTo(issueBody.labels[1]));
            Assert.Contains(issue.labels[0].name, issueBody.labels);

        }

        [Test]
        public void Test_EditIssue()
        {
            var request = new RestRequest($"{partialUrl}/1", Method.Patch);
            var issueBody = new
            {
                title = "EDITED: Test issue from RestSharp" + DateTime.Now.Ticks,
                body = "EDITED: some body for my issue"
            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");
            Assert.That(issue.number, Is.GreaterThan(0));
            Assert.That(issue.title, Is.EqualTo(issueBody.title));
            Assert.That(issue.body, Is.EqualTo(issueBody.body));
        }

        [Test]
        public void Test_EditIssueLabel ()
        {
            var request = new RestRequest($"{partialUrl}/67", Method.Patch);
            var issueBody = new
            {
               // title = "EDITED: Test issue from RestSharp" + DateTime.Now.Ticks,
                //body = "EDITED: some body for my issue"
                labels = new string[] { "NewL1", "NewL2", "NewL3" }

            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");
            Assert.That(issue.number, Is.GreaterThan(0));
            Assert.That(issue.labels[1].name, Is.EqualTo(issueBody.labels[1]));
            Assert.That(issue.labels[2].name, Is.Not.EqualTo(issueBody.labels[1]));
            Assert.Contains(issue.labels[0].name, issueBody.labels);
        }

        [Test]
        public void Test_EditSingleIssueCommentById()
        {

            var request = new RestRequest($"{partialUrl}/comments/1426852121", Method.Patch);
            var issueBody = new
            {
                // title = "EDITED: Test issue from RestSharp" + DateTime.Now.Ticks,
                body = "EDITED: OrangeAndBanana"
                //labels = new string[] { "NewL1", "NewL2", "NewL3" }

            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");
            Assert.That(issue.body, Is.EqualTo(issueBody.body));
           
        }

        [Test]
        public void Test_EditSingleIssueTitleAndBody()
        {

            var request = new RestRequest($"{partialUrl}/102", Method.Patch);
            var issueBody = new
            {
                title = "EDITED: Srawberry" + DateTime.Now.Ticks,
                body = "EDITED: Cherry"
                //labels = new string[] { "NewL1", "NewL2", "NewL3" }

            };
            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");
            Assert.That(issue.body, Is.EqualTo(issueBody.body));
            Assert.That(issue.title, Is.EqualTo(issueBody.title));

        }

        [Test]
        public void Test_DeleteSingleIssueComment()
        {

            var request = new RestRequest($"{partialUrl}/comments/1426854733", Method.Delete);
           
            var response = this.client.Execute(request);
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "HTTP Status Code property");
         
        }

        [TestCase("US","90210","United States")]
        [TestCase("BG","1000","Bulgaria")]
        [TestCase("DE","01067","Germany")]
       
        public void Test_Zippopotamus_DDS(string countryCode, string zipCode, string expectedCountry )
        {
            var restClient=new RestClient("https://api.zippopotam.us/");
            var request = new RestRequest($"{countryCode}/{zipCode}", Method.Get);

            var response = restClient.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");

            var location = JsonSerializer.Deserialize<Location>(response.Content);
            Assert.That(location.Country, Is.EqualTo(expectedCountry));
            Assert.That(location.ctryAbbreviation, Is.EqualTo(countryCode));
        }
  	

        [TestCase("BG", "1000", "Sofija")]
        [TestCase("BG", "8600", "Jambol")]
        [TestCase("CA", "M5S", "Toronto")]
        [TestCase("GB", "B1", "Birmingham")]

        public void Test_Zippopotamus_DDSS(string countryCode, string zipCode, string expectedCity)
        {
            var restClient = new RestClient("https://api.zippopotam.us/");
            var request = new RestRequest($"{countryCode}/{zipCode}", Method.Get);

            var response = restClient.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");

            var location = JsonSerializer.Deserialize<Location>(response.Content);
            Assert.That(location.ctryAbbreviation, Is.EqualTo(countryCode));
            Assert.That(location.ctryZipCode, Is.EqualTo(zipCode));

            foreach (var info in location.countryInfo)
            {
                var city = info.city.Split(' ');
                 Assert.Contains(expectedCity, city);
            }
        
        }
    }
}
