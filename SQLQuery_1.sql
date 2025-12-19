USE DotNetCourseDatabase
GO 

SELECT [UserId],
[FirstName],
[LastName],
[Email],
[Gender],
[Active] FROM TutorialAppSchema.Users

SELECT [UserId],
[JobTitle],
[Department] FROM TutorialAppSchema.UserJobInfo

SELECT [UserId],
[Salary],
[AvgSalary] FROM TutorialAppSchema.UserSalary

