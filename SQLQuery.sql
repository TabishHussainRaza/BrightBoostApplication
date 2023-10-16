USE [aspnet-BrightBoostApplication-a782c3ce-14f6-41ff-89c5-9817c261616d]
GO

INSERT INTO [dbo].[AspNetRoles]
           ([Id]
           ,[Name]
           ,[NormalizedName]
           ,[ConcurrencyStamp])
     VALUES
           ('ecbd132b-c26e-4303-9e11-b391a4de1a40'
           ,'Administrator'
           ,'ADMINISTRATOR'
           ,'2323363b-21fd-46b2-b1a1-cd7122faa978')

INSERT INTO [dbo].[AspNetUsers] 
		(Id, 
		UserName, 
		NormalizedUserName, 
		Email, NormalizedEmail, 
		EmailConfirmed, 
		PasswordHash, 
		SecurityStamp, 
		ConcurrencyStamp, 
		PhoneNumber, 
		PhoneNumberConfirmed, 
		TwoFactorEnabled, 
		LockoutEnd, 
		LockoutEnabled, 
		AccessFailedCount, 
		firstName, 
		lastName, 
		isActive)
	VALUES 
		('96558e6c-cf46-4cb1-b07e-26a7fd0bf4cb', 
		'useradmin@brightboost.com', 
		'USERADMIN@BRIGHTBOOST.COM', 
		'useradmin@brightboost.com', 
		'USERADMIN@BRIGHTBOOST.COM', 
		0, 
		'AQAAAAEAACcQAAAAEIG+wNGd/GFxRkiCOwMOegj5z9JFFTSux/glMlJatwRwGx+uoz7P3Ebku8BFKqsL8g==', 
		'VUPB635ZP5RT4OGH535DROWLNH2QO44J', 
		'09e517e8-82b4-4fac-aa6d-6dd463bb2eb8', 
		NULL,
		0, 
		0, 
		NULL, 
		1, 
		0, 
		'Admin', 
		'User',
		1);


INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
     VALUES
           ('96558e6c-cf46-4cb1-b07e-26a7fd0bf4cb'
           ,'ecbd132b-c26e-4303-9e11-b391a4de1a40')
GO

