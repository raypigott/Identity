Identity
========

Implementation of ASP.NET Identity using Dapper

####Integration Tests
Please note that the tests wipe the tables they have run against after a test has completed.

####Database Set Up
Create a database and use schema compare from the database project to add the tables.

####Model Changes
The boolean properties use a Is prefix
EmailConfirmed is IsEmailConfirmed. This is also reflected in the database fields.
