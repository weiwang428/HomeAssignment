using Xunit;
using System;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using StudentProject.Models;
using System.Collections.Generic;

namespace StudentProject.Test
{
    public class StudentProjectTest
    {
        public StudentProjectTest()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                                               .UseStartup<Startup>());
            Client = server.CreateClient();
        }

        public HttpClient Client { get; }


        #region Student Controller Test Case Suite.

        [Fact]
        public async Task StudentInitShouldOK()
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
        public async Task StudentLoadPreparedDataShouldOK()
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
        public async Task StudentListShouldOK()
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
        public async Task StudentListShouldNotIncludeNonExistStudent()
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
        public async Task StudentGetProjectsShouldOK()
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

        [Fact]
        public async Task StudentGetProjectsShouldNotOKWithAInvalidStudentId()
        {
            // Act
            var response = await Client.GetAsync($"/Student/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // Invalid Student_Id
            string Student_Id = "e0c0cb8d-bed6-4806-92fc-1ef2c261e4";

            // Act
            response = await Client.GetAsync($"/Student/GetProjects?Id={Student_Id}");

            var val2 = await response.Content.ReadAsStringAsync();

            Assert.Contains("Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)", val2);
        }

        [Fact]
        public async Task StudentGetProjectsShouldReturnEmptyListWithNotExistStudentId()
        {
            // Act
            var response = await Client.GetAsync($"/Student/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // This is just a student_Id which matches no project.
            string Student_Id = "e0c0cb8d-bed6-4806-92fc-1ef2c261e045";

            // Act
            response = await Client.GetAsync($"/Student/GetProjects?Id={Student_Id}");

            var val2 = await response.Content.ReadAsStringAsync();


            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var lst = JsonConvert.DeserializeObject<List<Project>>(val2, jsonSerializerSettings);

            // It has to be an empty list.
            Assert.Empty(lst);
        }

        #endregion


        #region Project Controller Test Case Suite.

        [Fact]
        public async Task ProjectInitShouldOK()
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
        public async Task ProjectLoadPreparedDataShouldOK()
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
        public async Task ProjectListShouldOK()
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
        public async Task ProjectAddStudentToGroupShouldOK()
        {
            // Act
            var response = await Client.GetAsync($"/Project/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // This student does not belong to this group and it does not belong to any other groups which include
            // this group, it should be OK to add this student to this group.
            string Group_Id = "a6f2175e-6ea4-4402-a2bb-6578da738969";
            string Student_Id = "af040dd5-37b9-43c5-b6bb-dfb53e6cd318";

            // Act
            response = await Client.PostAsync($"/project/AddStudentToGroup?groupId={Group_Id}&studentId={Student_Id}", null);

            val = response.Content.ReadAsStringAsync();
            Assert.Equal("True", val.Result);
        }

        [Fact]
        public async Task ProjectAddStudentToGroupShouldReturnErrorMessageWhenIdIsInvalid()
        {
            // Act
            var response = await Client.GetAsync($"/Project/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);


            // The  Group_Id is a invalid Group GUID.
            string Group_Id = "This is not a valid Group Id";
            string Student_Id = "af040dd5-37b9-43c5-b6bb-dfb53e6cd318";

            // Act
            response = await Client.PostAsync($"/project/AddStudentToGroup?groupId={Group_Id}&studentId={Student_Id}", null);
            val = response.Content.ReadAsStringAsync();
            Assert.Contains("Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)", val.Result);

            // This is not a valid Student GUID.
            Group_Id = "a6f2175e-6ea4-4402-a2bb-6578da738969";
            Student_Id = "This is not a valid Student Id";

            // Act
            response = await Client.PostAsync($"/project/AddStudentToGroup?groupId={Group_Id}&studentId={Student_Id}", null);
            val = response.Content.ReadAsStringAsync();
            Assert.Contains("Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)", val.Result);
        }


        [Fact]
        public async Task ProjectAddStudentToGroupShouldReturnFalseIfStudentAlreadyInGroup()
        {
            // Act
            var response = await Client.GetAsync($"/Project/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // This group already has this student.
            string Group_Id = "a6f2175e-6ea4-4402-a2bb-6578da738969";
            string Student_Id = "e0c0cb8d-bed6-4806-92fc-1ef2c261e055";

            // Act
            response = await Client.PostAsync($"/project/AddStudentToGroup?groupId={Group_Id}&studentId={Student_Id}", null);
            val = response.Content.ReadAsStringAsync();
            Assert.Equal("False", val.Result);
        }

        [Fact]
        public async Task ProjectAddStudentToGroupShouldReturnFalseIfStudentInOtherGroup()
        {
            // Act
            var response = await Client.GetAsync($"/Project/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // This group does not include the student, however the project includes this group has other group include this student.
            // This should also return False.
            string Group_Id = "a6f2175e-6ea4-4402-a2bb-6578da738969";
            string Student_Id = "58ad0314-b9c9-4fa7-8aa7-ae0bedf72cf4";

            // Act
            response = await Client.PostAsync($"/project/AddStudentToGroup?groupId={Group_Id}&studentId={Student_Id}", null);
            val = response.Content.ReadAsStringAsync();
            Assert.Equal("False", val.Result);
        }

        [Fact]
        public async Task ProjectCreateGroupShouldOK()
        {
            // Act
            var response = await Client.GetAsync($"/Project/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // This project is a valid project, and it does not include this group name yet, it should be OK to add this group to the
            // project.
            string Project_Id = "161ede81-fb20-471d-9605-07ec56a66819";
            string Group_Name = "ThisGroupNameIsSoGood";

            // Act
            response = await Client.PostAsync($"/project/Creategroup?projectid={Project_Id}&groupname={Group_Name}", null);
            val = response.Content.ReadAsStringAsync();

            Guid g_guid;
            bool result = Guid.TryParse(val.Result, out g_guid);
            Assert.True(result);
        }

        [Fact]
        public async Task ProjectCreateGroupShouldReturnErrorMessageIfProjectIdIsInvalid()
        {
            // Act
            var response = await Client.GetAsync($"/Project/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // This projectId is not a valid GUID string (missing 1 character), it should return error message.
            string Project_Id = "161ede81-fb20-471d-9605-07ec56a6681";
            string Group_Name = "ThisGroupNameIsSoGood";

            // Act
            response = await Client.PostAsync($"/project/Creategroup?projectid={Project_Id}&groupname={Group_Name}", null);
            val = response.Content.ReadAsStringAsync();
            Assert.Contains("Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)", val.Result);
        }

        [Fact]
        public async Task ProjectCreateGroupShouldReturnMessageIfProjectIdNotExist()
        {
            // Act
            var response = await Client.GetAsync($"/Project/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // This projectId does not exist in the database, it should return message with the information.
            string Project_Id = "161ede81-fb20-471d-9605-07ec56a66815";
            string Group_Name = "ThisGroupNameIsSoGood";

            // Act
            response = await Client.PostAsync($"/project/Creategroup?projectid={Project_Id}&groupname={Group_Name}", null);
            val = response.Content.ReadAsStringAsync();
            Assert.Equal("Creation Fail, no such project.", val.Result);
        }

        [Fact]
        public async Task ProjectCreateGroupShouldReturnMessageIfGroupNameIsDuplicate()
        {
            // Act
            var response = await Client.GetAsync($"/Project/LoadPreparedData");

            var val = response.Content.ReadAsStringAsync();

            // Assert the response status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert the response content value.
            Assert.Equal("Successfully load the prepared data.", val.Result);

            // This project has already include a group with the same name as Group_Name here.
            string Project_Id = "161ede81-fb20-471d-9605-07ec56a66819";
            string Group_Name = "EnglishIsAcceptable";

            // Act
            response = await Client.PostAsync($"/project/Creategroup?projectid={Project_Id}&groupname={Group_Name}", null);
            val = response.Content.ReadAsStringAsync();
            Assert.Equal("Creation Fail, duplicate group name in this project.", val.Result);
        }

        #endregion
    }
}
