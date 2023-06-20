USE [master]
GO
/****** Object:  Database [FriendsApp]    Script Date: 6/20/2023 7:28:16 AM ******/
CREATE DATABASE [FriendsApp]
 
GO

USE [FriendsApp]
GO
/****** Object:  Table [dbo].[Friends]    Script Date: 6/20/2023 7:28:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friends](
	[FriendID] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[DateOfBirth] [datetime] NULL,
 CONSTRAINT [PK_Friends] PRIMARY KEY CLUSTERED 
(
	[FriendID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Friends] ON 
GO
INSERT [dbo].[Friends] ([FriendID], [FirstName], [LastName], [DateOfBirth]) VALUES (1, N'David', N'James', CAST(N'1993-06-15T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Friends] OFF
GO
USE [master]
GO
ALTER DATABASE [FriendsApp] SET  READ_WRITE 
GO
