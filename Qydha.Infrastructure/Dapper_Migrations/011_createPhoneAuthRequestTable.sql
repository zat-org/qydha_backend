CREATE TABLE Phone_Authentication_Requests (
    id uuid DEFAULT uuid_generate_v4() PRIMARY KEY NOT NULL,
    phone VARCHAR(30) NOT NULL,
    otp VARCHAR(6) NOT NULL, 
    created_on TIMESTAMP NOT NULL
);