CREATE TABLE Admins (
    id uuid DEFAULT uuid_generate_v4() PRIMARY KEY NOT NULL,
    username VARCHAR(100) UNIQUE NOT NULL,
    normalized_username VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL,
    role VARCHAR(25) NOT NULL
);


INSERT INTO Admins 
(username ,normalized_username,password_hash , created_At  , role ) 
VALUES('$username$' , '$capitalUsername$','$password$', CURRENT_TIMESTAMP , 'SuperAdmin');
