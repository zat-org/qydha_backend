CREATE TABLE User_Promo_Codes (
    Id uuid DEFAULT uuid_generate_v4() PRIMARY KEY NOT NULL,
    Code VARCHAR(50) NOT NULL,
    Number_Of_Days SMALLINT NOT NULL, 
    Created_At TIMESTAMP NOT NULL,
    Expire_At TIMESTAMP NOT NULL,
    Used_At TIMESTAMP ,
    User_Id uuid NOT NULL,
    CONSTRAINT fk_user_codes
        FOREIGN KEY(User_Id)
            REFERENCES Users(id)
            ON DELETE CASCADE
);

