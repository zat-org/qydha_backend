CREATE TABLE Login_with_qydha_Requests (
    id uuid DEFAULT uuid_generate_v4() PRIMARY KEY NOT NULL,
    username VARCHAR(50) NOT NULL,
    otp VARCHAR(6) NOT NULL, 
    created_at TIMESTAMP NOT NULL,
    used_at TIMESTAMP NULL
);

ALTER TABLE Users 
ALTER COLUMN username SET NOT NULL;

ALTER TABLE Users 
ALTER COLUMN Phone SET NOT NULL;