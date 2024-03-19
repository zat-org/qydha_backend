CREATE TABLE InfluencerCodes (
    id uuid DEFAULT uuid_generate_v4() PRIMARY KEY NOT NULL,
    code VARCHAR(100) UNIQUE NOT NULL,
    normalized_code VARCHAR(100) UNIQUE NOT NULL,
    created_at TIMESTAMP NOT NULL, 
    expire_at TIMESTAMP , 
    number_of_days INT NOT NULL
);