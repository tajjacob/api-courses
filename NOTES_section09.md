# Stored Procedures

- A stored procedure is a prepared SQL code that you can save, so the code can be reused over and over again.
- So if you have an SQL query that you write over and over again, save it as a stored procedure, and then just call it to execute it.
- You can also pass parameters to a stored procedure, so that the stored procedure can act based on the parameter value(s) that is passed.

## Benefits of Stored Procedures

1. **Performance**: SQL Server compiles and optimizes the execution plan for the stored procedure once and caches it. This is often faster than sending raw SQL strings from your C# code every time.
2. **Security**:
- Access Control: You can grant a user permission to execute a stored procedure without giving them SELECT or UPDATE access to the underlying tables.
- SQL Injection: Using parameters in stored procedures (like @UserId) automatically handles input sanitization, preventing SQL injection attacks.
3. **Maintainability**: If your database schema changes (e.g., a column name changes), you only need to update the Stored Procedure in the database, rather than finding and updating every SQL string scattered throughout your C# code.
4. **Network Traffic**: Instead of sending a long SQL query string over the network, you only send the name of the procedure and parameters.

# What is a Temp Table?

- a table that exists temporarily on the database server. They are stored in the system database *tempdb* and are useful for storing intermediate results during the execution of a complex query or stored procedure.

## There are two main types:

1. **Local Temp Tables (#TableName):** Visible only to the connection that created them. They are automatically deleted when the connection closes.
2. **Global Temp Tables (##TableName):** Visible to all connections. They are deleted when the last connection referencing them is closed.

## Using a temp table here is an optimization strategy

- **Performance:** Instead of calculating the average salary for the department inside a subquery for every single user (which can be computationally expensive), you calculate it once for all departments, store it, index it, and then join it.
- **Readability:** It breaks a complex logic flow into distinct steps: "First, figure out department averages. Second, get user data and attach those averages."

