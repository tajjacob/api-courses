USE DotNetCourseDatabase
GO 

SELECT [UserId],
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active] 
FROM TutorialAppSchema.Users
WHERE FirstName = 'Umar'

-- explanation: Inserting a new user into the Users table
INSERT INTO TutorialAppSchema.Users(
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active]
) VALUES(
    'Taj',
    'Jacob',
    'taj.jacob@example.com',
    'Male',
    1
) 

-- explanation: Updating the user with UserId = 1 to have new details
UPDATE TutorialAppSchema.Users
SET [FirstName] = 'Taj',
    [LastName] = 'Jacob',
    [Email] = 'taj.jacob@example.com',
    [Gender] = 'Male',
    [Active] = 1
WHERE UserId = 1


SELECT [UserId],
    [JobTitle],
    [Department] 
FROM TutorialAppSchema.UserJobInfo

SELECT [UserId],
    [Salary],
    [AvgSalary] 
FROM TutorialAppSchema.UserSalary

-- explanation: Deleting the user with UserId = 1 from the Users table
DELETE FROM TutorialAppSchema.Users
WHERE UserId = 1


