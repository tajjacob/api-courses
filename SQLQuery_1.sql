USE DotNetCourseDatabase
GO 

SELECT [UserId],
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active] 
FROM TutorialAppSchema.Users
    WHERE UserId = 1

SELECT [UserId],
    [JobTitle],
    [Department] 
FROM TutorialAppSchema.UserJobInfo

SELECT [UserId],
    [Salary],
    [AvgSalary] 
FROM TutorialAppSchema.UserSalary
