CREATE TABLE Notifications_Data (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description VARCHAR(512) NOT NULL,
    Created_At TIMESTAMP NOT NULL,
    Action_Path VARCHAR(350),
    Action_Type SMALLINT NOT NULL,
    Payload JSONB DEFAULT '{}'::jsonb NOT NULL,
    Visibility SMALLINT NOT NULL
);
CREATE TABLE Notifications_Users_Link(
    Id BIGSERIAL PRIMARY KEY,
    Notification_Id INT NOT NULL,
    User_Id uuid NOT NULL,
    Read_At TIMESTAMP Null,
    Sent_At TIMESTAMP NOT NULL,
    CONSTRAINT fk_user_at_notification_link_table FOREIGN KEY(User_Id) REFERENCES Users(id) ON DELETE CASCADE,
    CONSTRAINT fk_notification_at_notification_link_table FOREIGN KEY(Notification_Id) REFERENCES Notifications_Data(id) ON DELETE CASCADE
);
INSERT INTO Notifications_Data (
        Title,
        Description,
        Created_At,
        Action_Path,
        Action_Type,
        Payload,
        Visibility
    )
VALUES (
        'مرحباً بك في قيدها ♥',
        'نتمنى لك تجربة جميلة، ارسلنا لك هدية بقسم المتجر😉',
        NOW() AT time zone 'utc' AS utc,
        '_',
        1,
        '{}'::jsonb,
        3
    );
INSERT INTO Notifications_Data (
        Title,
        Description,
        Created_At,
        Action_Path,
        Action_Type,
        Payload,
        Visibility
    )
VALUES (
        'شكرا لثقتك بقيدها..',
        'نتمنى لك تجربة جميلة، لا تنسى قيدها ليس مجرد حاسبة',
        NOW() AT time zone 'utc' AS utc,
        '_',
        1,
        '{}'::jsonb,
        3
    );
INSERT INTO Notifications_Data (
        Title,
        Description,
        Created_At,
        Action_Path,
        Action_Type,
        Payload,
        Visibility
    )
VALUES (
        'وصلتك هدية..🎁 ',
        'شيك على المتجر .. تتهنى♥',
        NOW() AT time zone 'utc' AS utc,
        '_',
        1,
        '{}'::jsonb,
        3
    );
INSERT INTO Notifications_Data (
        Title,
        Description,
        Created_At,
        Action_Path,
        Action_Type,
        Payload,
        Visibility
    )
VALUES (
        'تستاهل ما جاك',
        'نتمنى لك تجربة ممتعة♥',
        NOW() AT time zone 'utc' AS utc,
        '_',
        1,
        '{}'::jsonb,
        3
    );
INSERT INTO Notifications_Data (
        Title,
        Description,
        Created_At,
        Action_Path,
        Action_Type,
        Payload,
        Visibility
    )
VALUES (
        'تم تفعيل الكود',
        'إذا عجبك التطبيق لا تنسى تنشره بين أخوياك',
        NOW() AT time zone 'utc' AS utc,
        '_',
        1,
        '{}'::jsonb,
        3
    );