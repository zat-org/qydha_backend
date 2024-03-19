-- Step 1: Create a temporary column to hold the user_id
ALTER TABLE Login_with_qydha_Requests 
ADD COLUMN user_id UUID;

-- Step 2: Populate the user_id column based on the existing username values
UPDATE Login_with_qydha_Requests l
SET user_id = u.id
FROM users u
WHERE l.username = u.username;

-- Step 3: Drop the username column
ALTER TABLE Login_with_qydha_Requests 
DROP COLUMN username;

-- Step 4: Add a foreign key constraint to ensure referential integrity
ALTER TABLE Login_with_qydha_Requests 
ADD CONSTRAINT fk_user_id 
FOREIGN KEY (user_id) 
REFERENCES users(id);