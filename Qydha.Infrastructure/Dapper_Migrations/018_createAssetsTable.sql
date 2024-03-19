CREATE TABLE app_assets (
    asset_key VARCHAR(100) PRIMARY KEY, 
    asset_data JSONB 
);

INSERT INTO app_assets (asset_key , asset_data)
VALUES ('baloot_book' , '{}' );