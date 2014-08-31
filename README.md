Identity
========

Implementation of ASP.NET Identity using Dapper for SQL Server

####Integration Tests
Please note that the tests wipe the tables they have run against after a test has completed.

####Database Set Up
If using Visual Studio, use the dacpac created when the project is built.
Instructions are here:http://msdn.microsoft.com/en-us/library/ee210569.aspx
Othewise you can run the scripts in the database project. The database needs to be called Identity.SqlServer to run the integration tests out of the box.

####Model Changes
The boolean properties use an Is prefix
EmailConfirmed is IsEmailConfirmed. This is also reflected in the database fields.

####User Deletion
I decided to not delete users. Instead there is a flag that hides them.
