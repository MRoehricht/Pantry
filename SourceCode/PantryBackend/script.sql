CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'recipes') THEN
        CREATE SCHEMA recipes;
    END IF;
END $EF$;

CREATE TABLE recipes."Recipes" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Description" text,
    "Owner" text NOT NULL,
    "CreatedOn" timestamp without time zone NOT NULL,
    "Details" jsonb NOT NULL,
    "Ingredients" jsonb,
    CONSTRAINT "PK_Recipes" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250211232856_Initial', '9.0.2');

COMMIT;

