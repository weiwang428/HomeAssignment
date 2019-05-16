# HomeAssignment
High school is opening for a new semester, we are now preparing a backend web service where the students can choose from different group-projects.

For the web service, there are certain rules.
1. Create a ASP.NET __Core__ MVC web service in order to be called.
3. One project can have multiple groups of students.
4. Each group can only have multiple students.
5. Each student in one project can only join one group.

You will at least have following typed information:
### Student:

      a. Guid studentId	  
      b. string firstName	  
      c. string lastName
	  
### Project:

      a. Guid projectId	  
      b. string projectName
	  
### Group:

      a. Guid groupId	  
      b. string groupName

This web service will provide certain APIs, return in JSON format:

## Student APIs:

/Student/Init  // Initialize the student data 

/Student/List  // List ALL students in the DB,  JSON format.

/Student/GetProjects?Id={Student_Id} // Given student ID, return the project and group info the student belongs to.  JSON format.



## Project APIs:

/project/Init // Initialize the project data

/project/List // List ALL project information, incl. all groups and students. JSON format.

/project/AddStudentToGroup?groupId={Group_Id}&studentId={Student_Id} // Given groupId and studentId, the student will be added to the group, return true or false.

/project/Creategroup?projectid={Project_Id}&groupname={Group_Name}   // Given the project Id and new group name, return the newly created group's ID.



Note:
1. You need to prepare some example data to execute:
      a. at least 3 groups in each project 
      b. at least 2 projects
      c. at least 1 student in each group
2. Consider the Extandability of the webservice.
3. You do NOT need to consider front-end page.
4. Use git repo: https://github.com/northmill/HomeAssignment for the start, fork it and commit your code in the new repo.
5. Send us your github repo url.
6. You can use any 3rd-party libs, EXCEPT GPL (inc. v2, v3) licensed ones.
7. You MUST use SQLITE as the data source, the connection string and DBContext has already prepared for you in the repo.

## Support

If you have any questions regarding the assignment, contact us at it-hire@northmill.se.

## Examples:

### Student APIs:
```
Input:
/student/Init

Output:
Done
 
Input:
/student/list

Output:

[{"id":"eba18682-0a8d-4306-8b6d-50ebb96191c7","firstName":"Demo","lastName":"Example","studentGroups":[]},{"id":"72d3e5b5-38a7-46ba-8ecb-8090b0886060","firstName":"Anders","lastName":"Andersson","studentGroups":[]}]
 
Input:
/student/GetProjects?Id=eba18682-0a8d-4306-8b6d-50ebb96191c7

Output:

[{"id":"1175a381-8c12-4a60-a222-79e78c69fccd","name":"Math","groups":[{"id":"eeb280c3-3692-43f7-bd5e-ca1b243454f1","name":"mathisgood","students":[{"id":"eba18682-0a8d-4306-8b6d-50ebb96191c7","firstName":"Demo","lastName":"Example"},{"id":"72d3e5b5-38a7-46ba-8ecb-8090b0886060","firstName":"Anders","lastName":"Andersson"}]}]}]
```


### Project APIs:

```
Input:
/project/Init

Output:
Done
 
Input:
/project/list

Output:

[{"id":"40b59faa-ed88-44c4-b41b-6359e3708a0c","name":"English","groups":[{"id":"26349fdd-04c7-4770-918b-b5dd37f7056c","name":"englishisbetter","students":[{"id":"eba18682-0a8d-4306-8b6d-50ebb96191c7","firstName":"Demo","lastName":"Example"}]}]},{"id":"1175a381-8c12-4a60-a222-79e78c69fccd","name":"Math","groups":[{"id":"eeb280c3-3692-43f7-bd5e-ca1b243454f1","name":"mathisgood","students":[{"id":"eba18682-0a8d-4306-8b6d-50ebb96191c7","firstName":"Demo","lastName":"Example"},{"id":"72d3e5b5-38a7-46ba-8ecb-8090b0886060","firstName":"Anders","lastName":"Andersson"}]}]}]
 
Input:
/project/creategroup?projectid=1175a381-8c12-4a60-a222-79e78c69fccd&groupname=WeLoveMath

Output:
"73a4b895-901c-456e-bf95-b2a0e3a34855"
 
Input:
/project/AddStudentToGroup?groupid=eeb280c3-3692-43f7-bd5e-ca1b243454f1&studentid=eba18682-0a8d-4306-8b6d-50ebb96191c7

Output:
true
```

