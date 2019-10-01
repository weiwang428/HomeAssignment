using System;
using Xunit;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using StudentProject.Models;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace StudentProject.Test
{
    public class ProjectTest
    {
        public ProjectTest()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>());
            Client = server.CreateClient();
        }

        public HttpClient Client { get; }

        /*
[Fact]
public async Task InitShouldOK()
{

    // Act
    var response = await Client.GetAsync($"/Project/Init");

    var val = response.Content.ReadAsStringAsync().Result;

    // Assert the response status
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    // Assert the response content value.
    Assert.Equal("Done", val);
}

[Fact]
public async Task LoadPreparedDataShouldOK()
{

    // Act
    var response = await Client.GetAsync($"/Project/LoadPreparedData");

    var val = response.Content.ReadAsStringAsync();

    // Assert the response status
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    // Assert the response content value.
    Assert.Equal("Successfully load the prepared data.", val.Result);
}

[Fact]
public async Task ListProjectShouldOK()
{
    // Act
    var response = await Client.GetAsync($"/Project/LoadPreparedData");

    var val = response.Content.ReadAsStringAsync();

    // Assert the response status
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    // Assert the response content value.
    Assert.Equal("Successfully load the prepared data.", val.Result);

    // Act
    response = await Client.GetAsync($"/Project/List");

    val = response.Content.ReadAsStringAsync();

    // Convert it to a project list.
    var jsonSerializerSettings = new JsonSerializerSettings();
    jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
    var lst = JsonConvert.DeserializeObject<List<Project>>(val.Result, jsonSerializerSettings);

    // Assert the response status
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    // Size has to be exactly 2 elements.
    Assert.Equal(2, lst.Count);

    // Should return exactly those 2 projects.
    Assert.NotNull(lst.Find(s => s.ProjectId.ToString() == "161ede81-fb20-471d-9605-07ec56a66819"));
    Assert.NotNull(lst.Find(s => s.ProjectId.ToString() == "11684e7d-1c22-4762-8f9c-10b0a32c7228"));
}

[Fact]
public async Task AddStudentToGroupShouldOK()
{
    // Act
    var response = await Client.GetAsync($"/Project/LoadPreparedData");

    var val = response.Content.ReadAsStringAsync();

    // Assert the response status
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    // Assert the response content value.
    Assert.Equal("Successfully load the prepared data.", val.Result);


    string Group_Id = "a6f2175e-6ea4-4402-a2bb-6578da738969";
    string Student_Id = "af040dd5-37b9-43c5-b6bb-dfb53e6cd318";

    // Act
    response = await Client.PostAsync($"/project/AddStudentToGroup?groupId={Group_Id}&studentId={Student_Id}", null);

    val = response.Content.ReadAsStringAsync();
    Assert.Equal("True", val.Result);
}

*/
    }
}
