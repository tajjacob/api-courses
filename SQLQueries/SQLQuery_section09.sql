USE DotNetCourseDatabase
GO 


ALTER PROCEDURE TutorialAppSchema.spUsers_Get 
-- EXEC TutorialAppSchema.spUsers_Get @UserId = 3
@UserId INT = NULL,
@Active BIT = NULL 

AS 
BEGIN

    -- IF OBJECT_ID('tempdb..#TempAvgDeptSalary') IS NOT NULL
    --     BEGIN
    --         DROP TABLE #TempAvgDeptSalary
    --     END    

    DROP TABLE IF EXISTS #TempAvgDeptSalary    

    SELECT UserJobInfo.Department,
    AVG(UserSalary.Salary) AS AvgSalary 
    INTO #TempAvgDeptSalary
    FROM TutorialAppSchema.Users AS Users 
    LEFT JOIN TutorialAppSchema.UserSalary 
        AS UserSalary ON UserSalary.UserId = Users.UserId
    LEFT JOIN TutorialAppSchema.UserJobInfo 
        AS UserJobInfo ON UserJobInfo.UserId = Users.UserId
    GROUP BY UserJobInfo.Department   

    CREATE CLUSTERED INDEX cix_TempAvgDeptSalary_Department ON #TempAvgDeptSalary(Department) 

    SELECT [Users].[UserId],
            [Users].[FirstName],
            [Users].[LastName],
            [Users].[Email],
            [Users].[Gender],
            [Users].[Active],
            UserSalary.Salary,
            UserJobInfo.Department,
            UserJobInfo.JobTitle,
            UserSalary.AvgSalary,
            AvgSalary.AvgSalary
            
    FROM TutorialAppSchema.Users AS Users 
    LEFT JOIN TutorialAppSchema.UserSalary 
        AS UserSalary ON UserSalary.UserId = Users.UserId
    LEFT JOIN TutorialAppSchema.UserJobInfo 
        AS UserJobInfo ON UserJobInfo.UserId = Users.UserId 
        LEFT JOIN #TempAvgDeptSalary AS AvgSalary ON AvgSalary.Department = UserJobInfo.Department
   
    -- OUTER APPLY(
    --     SELECT UserJobInfo2.Department,
    --         AVG(UserSalary2.Salary) AS AvgSalary 
    --     FROM TutorialAppSchema.Users AS Users 
    --     LEFT JOIN TutorialAppSchema.UserSalary 
    --         AS UserSalary2 ON UserSalary2.UserId = Users.UserId
    --     LEFT JOIN TutorialAppSchema.UserJobInfo 
    --         AS UserJobInfo2 ON UserJobInfo2.UserId = Users.UserId
    --     WHERE UserJobInfo2.Department = UserJobInfo.Department
    --     GROUP BY UserJobInfo2.Department
  
    -- ) AvgSalary    
    WHERE [Users].[UserId] = ISNULL(@UserId, [Users].[UserId])
    AND [Users].[Active] = ISNULL(@Active, [Users].[Active])
END


-- SELECT CASE WHEN NULL = NULL THEN 1 ELSE 0 END,
--        CASE WHEN NULL <> NULL THEN 1 ELSE 0 END

