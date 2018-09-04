use [C:\Users\max\Desktop\CPI MVVM\CPI.Server\Database\CPI_DB.mdf]
go

---------------------------------------------------------------------------------
-- Users table for login  
create table [User] (
	[ID]			uniqueidentifier	default newsequentialid()	not null,
	[Name]			nvarchar(50)	not null,
	[Surname]		nvarchar(50)	not null,
	[UserName]		nvarchar(50)	not null,
	[Password]		nvarchar(50)	not null,
	[Permission]	nvarchar(10)	not null,

	constraint	[Primary Key Users]	primary key ([ID]),
)
go

-- Phantom2015
Insert into [User] values ('f4d99435-9889-4b7e-a4f4-61abd8fb3b03', 'Phantom', 'Phantom', 'phantom','Q4L17E4WKNDFAJFidLj4AGkN4/TM5W6gag/n5vZAKAL9QZPU','SuperAdmin')
-- Phantom2015
Insert into [User] values ('3E5821A1-2166-405C-81AB-D16AE668B319', 'TT', 'TT', 'i379','Q4L17E4WKNDFAJFidLj4AGkN4/TM5W6gag/n5vZAKAL9QZPU','SuperAdmin')
-- admin
Insert into [User] values ('9cf274e2-35a6-408f-979b-3c5350a258de', 'Max', 'Parker', 'admin', 'cX/md30e1R+AyfxDX82uSnmnEkKOjW4v63TlNiRtiS+YKJkl','Admin')
go

create procedure sp_GetUserByUserName(@username nvarchar(50)) as
	begin
		select top 1 * from [User] where [UserName] = @username
	end
go

sp_GetUserByUserName admin
go


create procedure sp_GetAllUsers as
	begin
		select * from [User]
	end
go

---------------------------------------------------------------------------------
create table [License] (
	[ID]					uniqueidentifier	default newsequentialid()	not null,
	[Count]					int					not null,
	[SerialNumber]			nvarchar(100),
	[AuthorizationKey]		nvarchar(100),
	[RegistrationNumber]	nvarchar(100),
	[Period]				int,
	[Start]					datetime,
	[End]					datetime,
	[LastSession]			datetime,
	[Summary]				text,
	[Hash]					text,
	[Directory]				nvarchar(100),

	[Spare1]				nvarchar(100),
	[Spare2]				nvarchar(100),

	constraint [Primary Key License] primary key ([ID]),
)
go

create procedure sp_GetAllLicenses as
	begin
		select * from [License]
	end
go

create procedure sp_GetLastLicense as
	begin
		SELECT TOP 1 * FROM [License] ORDER BY [Count] DESC
	end
go


---------------------------------------------------------------------------------
create table [Setting] (
	[ID]				uniqueidentifier	default newsequentialid()	not null,
	[Key]				text,
	[NewValue]			text,
	[OldValue]			text,
	[DefaultValue]		text,

	constraint [Primary Key Setting] primary key ([ID]),
)
go

create procedure sp_GetAllSettings as
	begin
		select * from [Setting]
	end
go





------------------------------------------------------------------------------------------

-- Providers
create table [Provider] (
	[ID]			int				not null,
	[MCC]			int,
	[MNC]			int,
	[Network]		text,
	[Operator]		text,
	[Status]		nvarchar(200),
	[Country]		nvarchar(200),
	[Logo]			text,

	constraint	[Primary Key Provider]	primary key ([ID]),
)
go

create procedure sp_GetProvider(@mcc int, @mnc int) as
	begin
		select top 1 * from [Provider] where [MCC] = @mcc and  [MNC] = @mnc
	end
go

sp_GetProvider 404, 12
go




---------------------------------------------------------------------------------
-- Unit 
create table [Unit] (
	[ID]			uniqueidentifier	default newsequentialid()	not null,
	[Name]			nvarchar(100)	not null,
	[Active]		bit,
	[Notes]			nvarchar(1000),


	constraint	[Primary Key Unit]	primary key ([ID]),
)
go

create procedure sp_GetAllUnit as
	begin
		select * from [Unit]
	end
go

---------------------------------------------------------------------------------
-- Computer 
create table [Computer] (
	[ID]			uniqueidentifier	default newsequentialid()	not null,
	[Name]			nvarchar(50)	not null,
	[IP]			nvarchar(50)	not null,
	[Username]		nvarchar(50)	not null,
	[Password]		nvarchar(50)	not null,
	[System]		nvarchar(50)	not null,
	[Power]			int,
	[Receivers]		int,

	[Active]		bit,
	[Unit_ID]		uniqueidentifier,

	constraint	[Primary Key Computer]	primary key ([ID]),
	constraint	[Foreign Key Unit ID] foreign key ([Unit_ID]) references [Unit] ([ID]),

)
go

create procedure sp_GetAllComputers as
	begin
		select * from [Computer]
	end
go


------------------------------------------------------------------------------------------
-- ARFCN

create table [ARFCN] (
	[ID]				uniqueidentifier	default newsequentialid()	not null,
	[Chanel]			int,
	[Band]				nvarchar(50),
	[Frequency]			nvarchar(50),
	[MCC]				int,
	[MNC]				int,
	[LAC]				int,
	[CI]				int,
	[Power]				int,

	[ScramblingCode]	int,
	[System]			nvarchar(50),

	[Configuration]		nvarchar(500),
	[CellARFCNs]		nvarchar(500),
	[NeighbourCells]	nvarchar(500),
	
	[Notes]				nvarchar(1000),
	[ProviderID]		int,
	[Session_ID]		uniqueidentifier,

	constraint [Primary Key ARFCN] primary key ([ID]),
	constraint [Foreign Key Provider ID] foreign key ([ProviderID]) references [Provider] ([ID]),
	constraint [Foreign Key Session ID] foreign key ([Session_ID]) references [Session] ([ID]),
)
go

create procedure sp_GetAllARFCNs as
	begin
		select * from [ARFCN]
	end
GO

create procedure sp_GetAllARFCNsBySession (@ID uniqueidentifier)as
	begin
		select * from [ARFCN] where [Session_ID]=@ID
	end
go

------------------------------------------------------------------------------------------
-- Calls
create table [Call] (
	[ID]				uniqueidentifier	default newsequentialid()	not null,
	[UnitName]			nvarchar(100),
	
	[Kc]				nvarchar(20),
	[Codec]				nvarchar(10),
	[Rate]				nvarchar(10),
	[TS]				nvarchar(5),

	[CallerNumber]		nvarchar(50),
	[FileName]			nvarchar(1000),

	[CipherMode]		nvarchar(5),
	[TMSI]				nvarchar(20),
	[IMSI]				nvarchar(20),
	[IMEI]				nvarchar(20),

	[Created]			datetime,
	[Duration]			time,
	[Size]				nvarchar(20),
	[Notes]				nvarchar(1000),

	[ARFCNID]			uniqueidentifier,


	constraint [Primary Key Call] primary key ([ID]),
	constraint [Foreign Key Call ARFCN ID] foreign key ([ARFCNID]) references [ARFCN] ([ID]),
)
go

create procedure sp_GetAllCalls as
	begin
		select * from [Call]
	end
go

------------------------------------------------------------------------------------------
-- SMSs
create table [SMS] (
	[ID]				uniqueidentifier	default newsequentialid()	not null,
	[UnitName]			nvarchar(100),
	
	[Kc]				nvarchar(20),
	[Rate]				nvarchar(10),
	[TS]				nvarchar(5),

	[SRI]				nvarchar(5),
	[UDHI]				nvarchar(5),
	[MMS]				nvarchar(5),
	[PID]				nvarchar(5),
	[DCS]				nvarchar(5),
	[Class]				nvarchar(5),
	[Alphabet]			nvarchar(20),
	[Length]			nvarchar(10),
	[TextType]			nvarchar(50),

	[Sender]			nvarchar(50),
	[SMSC]				nvarchar(50),

	[CipherMode]		nvarchar(5),
	[TMSI]				nvarchar(20),
	[IMSI]				nvarchar(20),
	[IMEI]				nvarchar(20),

	[TimeStamp]			datetime,
	[Message]			text,
	[Notes]				nvarchar(1000),

	[ARFCNID]			uniqueidentifier,


	constraint [Primary Key SMS] primary key ([ID]),
	constraint [Foreign Key SMS ARFCN ID] foreign key ([ARFCNID]) references [ARFCN] ([ID]),
)
go

create procedure sp_GetAllSMS as
	begin
		select * from [SMS]
	end
GO

------------------------------------------------------------------------------------------
----- Receiver
create table [Receiver] (
	[ID]			uniqueidentifier	default newsequentialid()	not null,
	[Name]			nvarchar(100)	not null,
	[ARFCN_ID]		uniqueidentifier,
	[Comp_ID]		uniqueidentifier,

	constraint	[Primary Key Receiver]	primary key ([ID]),
	constraint	[Foreign Key Computer ID] foreign key ([Comp_ID]) references [Computer] ([ID]),
	constraint	[Foreign Key ARFCN ID] foreign key ([ARFCN_ID]) references [ARFCN] ([ID]),
)
GO

create procedure sp_GetAllReceivers as
	begin
		select * from [Receiver]
	end
GO



------------------------------------------------------------------------------------------
----- Session
create table [Session] (
	[ID]			uniqueidentifier	default newsequentialid()	not null,
	[Name]			nvarchar(100)	not null,
	[Date]			DATETIME,
	[Notes]				nvarchar(1000),

	constraint	[Primary Key Session]	primary key ([ID]),

)
GO

 create procedure sp_GetAllSessions as
	begin
		select * from [Session]
	end
GO


