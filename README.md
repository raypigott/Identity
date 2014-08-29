Identity
========

Implementation of ASP.NET Identity using Dapper

####Integration Tests
Please note that the tests wipe the tables they have run against after a test has completed.

####Database Set Up
If using Visual Studio:
Create a database and use schema compare from the database project to add the tables.

####Model Changes
The boolean properties use an Is prefix
EmailConfirmed is IsEmailConfirmed. This is also reflected in the database fields.

####User Deletion
I decided to not delete users. Instead there is a flag that hides them.
