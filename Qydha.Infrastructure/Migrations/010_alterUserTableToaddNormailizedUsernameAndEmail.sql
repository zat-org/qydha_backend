-- Add new columns without DEFAULT expression
ALTER TABLE Users 
ADD COLUMN Normalized_Username VARCHAR(100) UNIQUE,
ADD COLUMN Normalized_Email VARCHAR(200) UNIQUE;

-- Update the new columns with normalized values
UPDATE Users SET Normalized_Username = Upper(username);
UPDATE Users SET Normalized_Email = Upper(email);

