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
    public class StudentTest
    {
        public StudentTest()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>());
            Client = server.CreateClient();
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task InitShouldOK()
        {

            // Act
            var response = await Client.GetAsync($"/Student/Init");

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
            var response = await Client.GetAsync($"/Student/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);
        }

        [Fact]
        public async Task ListStudentShouldOK()
        {
            // Act
            var response = await Client.GetAsync($"/Student/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // Act
            response = await Client.GetAsync($"/Student/List");

            val = response.Content.ReadAsStringAsync();
            var lst = JsonConvert.DeserializeObject<List<Student>>(val.Result);

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Size has to be exactly 4 elements.
            Assert.Equal(4, lst.Count);
            Assert.NotNull(lst.Find(s => s.StudentId.ToString() == "e0c0cb8d-bed6-4806-92fc-1ef2c261e055"));
            Assert.NotNull(lst.Find(s => s.StudentId.ToString() == "58ad0314-b9c9-4fa7-8aa7-ae0bedf72cf4"));
            Assert.NotNull(lst.Find(s => s.StudentId.ToString() == "507c4f21-7d77-47a0-af67-3cae5956ac0a"));
            Assert.NotNull(lst.Find(s => s.StudentId.ToString() == "af040dd5-37b9-43c5-b6bb-dfb53e6cd318"));
        }

        [Fact]
        public async Task ListStudentShouldNotOK()
        {
            // Act
            var response = await Client.GetAsync($"/Student/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // Act
            response = await Client.GetAsync($"/Student/List");

            val = response.Content.ReadAsStringAsync();
            var lst = JsonConvert.DeserializeObject<List<Student>>(val.Result);

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // This Student should not be included, since it is not defined in the json file.
            Assert.Null(lst.Find(s => s.StudentId.ToString() == "e0c0cb8d-bed6-4806-92fc-1ef2c261e056"));
        }

        [Fact]
        public async Task GetProjectsShouldOK()
        {
            // Act
            var response = await Client.GetAsync($"/Student/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            string Student_Id = "e0c0cb8d-bed6-4806-92fc-1ef2c261e055";

            // Act
            response = await Client.GetAsync($"/Student/GetProjects?Id={Student_Id}");

            var val2 = await response.Content.ReadAsStringAsync();


            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var lst = JsonConvert.DeserializeObject<List<Project>>(val2, jsonSerializerSettings);

            // Size has to be exactly 2 elements, only 2 groups include this student.
            Assert.Equal(2, lst.Count);

            var proj_lst = lst.Select(p => p.ProjectId.ToString()).ToList();
            // This group needs to be included.
            Assert.Contains("161ede81-fb20-471d-9605-07ec56a66819", proj_lst);
            // This group needs to be included as well.
            Assert.Contains("11684e7d-1c22-4762-8f9c-10b0a32c7228", proj_lst);

            // Each project should contain only 1 group with the matched student.
            lst.ForEach(p => Assert.Equal(1, p.Groups.Count));
        }
    }
}
