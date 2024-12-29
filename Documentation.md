# Documentation

## Table of Contents
- [Introduction]()
- [Getting started]()
- [Project structure]()
- [Code explanation]()
- [Useful links]()

## Introduction

The capyborrowProject repository was created to save the backend code for the project, the idea of which is _to create a universal digital platform that will combine all the key functions of the educational process in one convenient interface, simplifying the educational process for students, teachers and administration of the educational institution_.

## Getting started

To get started with capyborrowProject, follow these steps:

### 1. Clone the repository

Clone the project to your local machine using Git:

```
git clone https://github.com/Capyborrow/capyborrowProject.git
cd capyborrowProject
```
or do it using Visual Studio:
1. Copy the [link](https://github.com/Capyborrow/capyborrowProject.git).
2. Open Visual Studio and select "Clone a repository".
3. Insert the copied link and proceed.

### 2. Restore dependencies

If needed, restore the NuGet packages required for the project. In Visual Studio, this can be done by right-clicking on the solution in the Solution Explorer and selecting "Restore NuGet Packages."

## Project stucture 
For now, Solution 'capyborrowProject' consists of two projects: _cappyborrowProject_ and _TestProject_.

- **capyborrowProject**: contains the main application code.
    - **Controllers folder**: contains three API controllers for Assignment, Teacher and Student models.
    - **Data folder**: contains a database context class, the purpose of which is to act as a bridge between the application and the database, enabling the app to interact with the database in an object-oriented way.
    - **Helpers folder**: serves as a container of different files that describe some additional functionality like password hashing.
    - **Migrations folder**: contains automatically generated files that create tables in the database based on the code written for models.
    - **Models folder**: contains classes for entities: Assignment, User, Student, Teacher _(the last two are inherited from User)_. 
- **TestProject**: contains unit tests for the application.

## Code explanation
> **NOTE** The description of API Controllers will be represented on the StudentController, because the TeacherController and the AssignmentController share the same or almost the same logic.

Methods `GetAllStudents()`, `GetStudent()` and `DeleteStudent()` are self-explanatory.

`PostStudent()` takes a student object as a parameter and adds it to the database with the already hashed password, saves changes, wraps the given student object in an Ok result, which corresponds to a `200 OK HTTP` response and sends the student object back to the client in the response body.

`PutStudent()` is used to update a student in the database. The function takes and id and a student object as parameters, ensures the id from the URL matches the id of the Student. Then the `EntityState.Modified` tells Entity Framework that the student entity has been changed and should be updated in the database. After that, the fuctnion tries to save the changes and if a concurrency issue occurs (e.g., the record was modified by another user or deleted), a `DbUpdateConcurrencyException` is thrown. Later, the code checks if a student with the given ID still exists. In case of success the function returns a `204 No Content` response.

## Useful links
- [For writing documentation](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#lists)
- [A short CRUD course](https://youtu.be/b8fFRX0T38M?si=lBDJx2gsc41vuBC_)
- [An API Tutorial](https://youtu.be/sdlt3-ptt9g?si=Iqdk6i4Njr5m23cn)