CREATE TABLE InfluencerCodes_categories (
    id SERIAL PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL UNIQUE,
    max_codes_per_user_in_group INT NOT NULL DEFAULT 1 
);

ALTER TABLE InfluencerCodes
    ADD category_id INT NULL DEFAULT NULL,
    ADD CONSTRAINT fk_influencer_code_categories 
        FOREIGN KEY (category_id)
        REFERENCES InfluencerCodes_categories (id)
        ON DELETE SET DEFAULT
;