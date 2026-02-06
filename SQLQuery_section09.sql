USE DotNetCourseDatabase
GO 


ALTER PROCEDURE TutorialAppSchema.spUsers_Get 
-- EXEC TutorialAppSchema.spUsers_Get @UserId = 3
@UserId INT = NULL 
AS 
BEGIN
    SELECT [Users].[UserId],
            [Users].[FirstName],
            [Users].[LastName],
            [Users].[Email],
            [Users].[Gender],
            [Users].[Active] 
    FROM TutorialAppSchema.Users AS Users 
    WHERE [Users].[UserId] = ISNULL(@UserId, [Users].[UserId])
END


