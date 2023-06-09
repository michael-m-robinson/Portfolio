CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `AspNetRoles` (
        `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
        `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
        `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `AspNetUsers` (
        `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `FirstName` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `LastName` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `AuthorDescription` varchar(500) CHARACTER SET utf8mb4 NULL,
        `Image` longblob NULL,
        `ImageType` longtext CHARACTER SET utf8mb4 NULL,
        `FacebookUrl` varchar(200) CHARACTER SET utf8mb4 NULL,
        `InstagramUrl` varchar(200) CHARACTER SET utf8mb4 NULL,
        `PinterestUrl` varchar(200) CHARACTER SET utf8mb4 NULL,
        `YouTubeUrl` varchar(200) CHARACTER SET utf8mb4 NULL,
        `TwitterUrl` varchar(200) CHARACTER SET utf8mb4 NULL,
        `UserAcceptedTerms` tinyint(1) NOT NULL,
        `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
        `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
        `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
        `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
        `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
        `LockoutEnd` datetime(6) NULL,
        `EmailConfirmed` tinyint(1) NOT NULL,
        `TwoFactorEnabled` tinyint(1) NOT NULL,
        `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
        `PhoneNumberConfirmed` tinyint(1) NOT NULL,
        `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
        `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
        `LockoutEnabled` tinyint(1) NOT NULL,
        `AccessFailedCount` int NOT NULL,
        CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `Projects` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `Title` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Created` datetime(6) NULL,
        `ProjectUrl` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Slug` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Projects` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `AspNetRoleClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
        `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `AspNetUserClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
        `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `AspNetUserLogins` (
        `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
        CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `AspNetUserRoles` (
        `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
        CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `AspNetUserTokens` (
        `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Value` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
        CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `Blogs` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `AuthorId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
        `Created` datetime(6) NOT NULL,
        `Updated` datetime(6) NULL,
        `Image` longblob NULL,
        `ImageType` longtext CHARACTER SET utf8mb4 NULL,
        `Slug` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Blogs` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Blogs_AspNetUsers_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `ProjectCategories` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `ProjectId` char(36) COLLATE ascii_general_ci NOT NULL,
        `Text` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_ProjectCategories` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ProjectCategories_Projects_ProjectId` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `ProjectImages` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `File` longblob NOT NULL,
        `FileContentType` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ProjectId` char(36) COLLATE ascii_general_ci NOT NULL,
        CONSTRAINT `PK_ProjectImages` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ProjectImages_Projects_ProjectId` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `Categories` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `BlogId` char(36) COLLATE ascii_general_ci NOT NULL,
        `Name` varchar(35) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Categories` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Categories_Blogs_BlogId` FOREIGN KEY (`BlogId`) REFERENCES `Blogs` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `Posts` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `BlogId` char(36) COLLATE ascii_general_ci NOT NULL,
        `CategoryId` char(36) COLLATE ascii_general_ci NOT NULL,
        `AuthorId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ReadyStatus` int NOT NULL,
        `Slug` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Image` longblob NOT NULL,
        `ThumbNail` longblob NOT NULL,
        `ImageType` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Title` varchar(75) CHARACTER SET utf8mb4 NOT NULL,
        `Abstract` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Created` datetime(6) NOT NULL,
        `Updated` datetime(6) NULL,
        CONSTRAINT `PK_Posts` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Posts_AspNetUsers_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Posts_Blogs_BlogId` FOREIGN KEY (`BlogId`) REFERENCES `Blogs` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Posts_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `Comments` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `PostId` char(36) COLLATE ascii_general_ci NOT NULL,
        `AuthorId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModeratorId` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Body` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
        `Created` datetime(6) NOT NULL,
        `Updated` datetime(6) NULL,
        `Moderated` datetime(6) NULL,
        `SoftDeleted` datetime(6) NULL,
        `IsSoftDeleted` tinyint(1) NOT NULL,
        `IsModerated` tinyint(1) NOT NULL,
        `ModeratedBody` varchar(500) CHARACTER SET utf8mb4 NULL,
        `ModerationType` int NULL,
        CONSTRAINT `PK_Comments` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Comments_AspNetUsers_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Comments_AspNetUsers_ModeratorId` FOREIGN KEY (`ModeratorId`) REFERENCES `AspNetUsers` (`Id`),
        CONSTRAINT `FK_Comments_Posts_PostId` FOREIGN KEY (`PostId`) REFERENCES `Posts` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE TABLE `Tags` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `PostId` char(36) COLLATE ascii_general_ci NOT NULL,
        `BlogUserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Text` varchar(25) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Tags` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Tags_AspNetUsers_BlogUserId` FOREIGN KEY (`BlogUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Tags_Posts_PostId` FOREIGN KEY (`PostId`) REFERENCES `Posts` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Blogs_AuthorId` ON `Blogs` (`AuthorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Categories_BlogId` ON `Categories` (`BlogId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Comments_AuthorId` ON `Comments` (`AuthorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Comments_ModeratorId` ON `Comments` (`ModeratorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Comments_PostId` ON `Comments` (`PostId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Posts_AuthorId` ON `Posts` (`AuthorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Posts_BlogId` ON `Posts` (`BlogId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Posts_CategoryId` ON `Posts` (`CategoryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_ProjectCategories_ProjectId` ON `ProjectCategories` (`ProjectId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_ProjectImages_ProjectId` ON `ProjectImages` (`ProjectId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Tags_BlogUserId` ON `Tags` (`BlogUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    CREATE INDEX `IX_Tags_PostId` ON `Tags` (`PostId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407002628_001') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230407002628_001', '7.0.2');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407011916_002') THEN

    ALTER TABLE `Posts` ADD `Content` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230407011916_002') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230407011916_002', '7.0.2');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;


