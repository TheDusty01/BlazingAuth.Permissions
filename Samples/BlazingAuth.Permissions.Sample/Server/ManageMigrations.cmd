@echo off

:START
echo 1 = Add migration, 2 = Remove last migration, 3 = Update database
SET /p Action="Action: "

2>NUL GOTO :CASE_%Action%

:CASE_1
	SET /p MigrationName="Migration Name: "
	dotnet ef migrations add %MigrationName% -o Data\Migrations
	echo "mig add"
	GOTO CASE_END

:CASE_2
	dotnet ef migrations remove
	GOTO CASE_END

:CASE_3
	dotnet ef database update
	GOTO CASE_END

:CASE_END
	VER > NUL
	echo(
	GOTO START