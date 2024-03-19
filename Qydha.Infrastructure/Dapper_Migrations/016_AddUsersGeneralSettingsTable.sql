CREATE TABLE user_general_settings
(
    user_id UUID PRIMARY KEY,
    enable_vibration BOOLEAN DEFAULT true,
    players_names JSONB DEFAULT '[]'::jsonb,
    teams_names JSONB DEFAULT '[]'::jsonb,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE 
);

INSERT INTO user_general_settings 
    (user_id)
SELECT
    u.id
FROM
    users u;