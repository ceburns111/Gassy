CREATE TABLE gassydb.User (
    Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(20),
    LastName VARCHAR(30),
    Email VARCHAR(100),
    PhoneNumber VARCHAR(20),
    UserName VARCHAR(25), 
    UserPassword VARCHAR(15),
    RoleId INT
    );



 