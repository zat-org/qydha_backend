CREATE TABLE user_baloot_settings
(
    user_id UUID PRIMARY KEY,
    is_flipped BOOLEAN DEFAULT false,
    is_advanced_recording BOOLEAN DEFAULT false,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE 
);

INSERT INTO user_baloot_settings 
    (user_id)
SELECT
    u.id
FROM
    users u;

CREATE TABLE user_hand_settings
(
    user_id UUID PRIMARY KEY,
    rounds_count INT DEFAULT 7,
    max_limit INT DEFAULT 0,
    teams_count INT DEFAULT 2,
    players_count_in_team INT DEFAULT 2,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE 
);

INSERT INTO user_hand_settings 
    (user_id)
SELECT
    u.id
FROM
    users u;
