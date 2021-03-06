USE [TestCrowd]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 29.04.2019 18:34:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[Id] [uniqueidentifier] NOT NULL,
	[Street] [varchar](max) NULL,
	[City] [varchar](max) NULL,
	[PostCode] [varchar](max) NULL,
	[Country_id] [uniqueidentifier] NULL,
	[UserId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationRole]    Script Date: 29.04.2019 18:34:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationRole](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](20) NOT NULL,
	[RoleName] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicatoinUser]    Script Date: 29.04.2019 18:34:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicatoinUser](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleDiscriminant] [nvarchar](255) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PasswordHash] [varchar](max) NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[CompanyName] [nvarchar](50) NULL,
	[Rating] [tinyint] NULL,
	[Credits] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 29.04.2019 18:34:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country](
	[Id] [uniqueidentifier] NOT NULL,
	[IdentificationCode] [varchar](5) NOT NULL,
	[Name] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Dispute]    Script Date: 29.04.2019 18:34:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dispute](
	[Id] [uniqueidentifier] NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
	[Resolved] [datetime] NULL,
	[AutoResolveDate] [datetime] NOT NULL,
	[CompanyId] [uniqueidentifier] NOT NULL,
	[TesterId] [uniqueidentifier] NOT NULL,
	[ResolverId] [uniqueidentifier] NOT NULL,
	[TestCaseId] [uniqueidentifier] NOT NULL,
	[Status] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DisputeMessage]    Script Date: 29.04.2019 18:34:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DisputeMessage](
	[Id] [uniqueidentifier] NOT NULL,
	[Message] [varchar](300) NOT NULL,
	[Created] [datetime] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Dispute_id] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Evidence]    Script Date: 29.04.2019 18:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Evidence](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Attached] [datetime] NOT NULL,
	[Dispute_id] [uniqueidentifier] NULL,
	[TestCase_id] [uniqueidentifier] NULL,
	[Tests_id] [uniqueidentifier] NULL,
	[Extension] [varchar](100) NULL,
	[RealName] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Review]    Script Date: 29.04.2019 18:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Review](
	[Id] [uniqueidentifier] NOT NULL,
	[Content] [varchar](max) NOT NULL,
	[Rating] [tinyint] NOT NULL,
	[Created] [datetime] NOT NULL,
	[CreatorId] [uniqueidentifier] NOT NULL,
	[Company_id] [uniqueidentifier] NULL,
	[TestCase_id] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SoftwareType]    Script Date: 29.04.2019 18:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SoftwareType](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](max) NULL,
	[Valid] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastChange] [datetime] NOT NULL,
	[ChangedById] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestCase]    Script Date: 29.04.2019 18:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestCase](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[SkillDificulty] [tinyint] NOT NULL,
	[TimeDificulty] [tinyint] NOT NULL,
	[Description] [varchar](max) NULL,
	[Reward] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[AvailableTo] [datetime] NOT NULL,
	[Rating] [tinyint] NOT NULL,
	[CreatorId] [uniqueidentifier] NOT NULL,
	[TestGroup_id] [uniqueidentifier] NULL,
	[CategoryId] [uniqueidentifier] NULL,
	[SoftwareTypeId] [uniqueidentifier] NULL,
	[IsInGroup] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestCategory]    Script Date: 29.04.2019 18:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestCategory](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](max) NULL,
	[Valid] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastChange] [datetime] NOT NULL,
	[ChangedById] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestGroup]    Script Date: 29.04.2019 18:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestGroup](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[SkillDificulty] [tinyint] NOT NULL,
	[TimeDificulty] [tinyint] NOT NULL,
	[RewardMultiplier] [decimal](3, 2) NOT NULL,
	[Created] [datetime] NOT NULL,
	[AvailableTo] [datetime] NOT NULL,
	[Rating] [int] NOT NULL,
	[CreatorId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK__TestGrou__3214EC07A137BDFF] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestGroups]    Script Date: 29.04.2019 18:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestGroups](
	[Id] [uniqueidentifier] NOT NULL,
	[Takened] [datetime] NOT NULL,
	[Finished] [datetime] NULL,
	[TestGroup_id] [uniqueidentifier] NOT NULL,
	[Tester_id] [uniqueidentifier] NULL,
	[Status] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tests]    Script Date: 29.04.2019 18:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tests](
	[Id] [uniqueidentifier] NOT NULL,
	[Takened] [datetime] NOT NULL,
	[Finished] [datetime] NULL,
	[Rejected] [datetime] NULL,
	[Test_id] [uniqueidentifier] NOT NULL,
	[Tester_id] [uniqueidentifier] NULL,
	[Status] [int] NULL,
	[TestStatus_id] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestStatus]    Script Date: 29.04.2019 18:34:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestStatus](
	[Id] [uniqueidentifier] NOT NULL,
	[Status] [varchar](40) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ApplicationRole] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ApplicatoinUser] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Country] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Dispute] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[DisputeMessage] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Evidence] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Review] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[SoftwareType] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TestCase] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TestCategory] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TestGroup] ADD  CONSTRAINT [DF__TestGroup__Id__695C9DA1]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TestGroups] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Tests] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TestStatus] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_1B079C7B] FOREIGN KEY([UserId])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_1B079C7B]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_B5765B57] FOREIGN KEY([Country_id])
REFERENCES [dbo].[Country] ([Id])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_B5765B57]
GO
ALTER TABLE [dbo].[ApplicatoinUser]  WITH CHECK ADD  CONSTRAINT [FK_B9543C5D] FOREIGN KEY([RoleId])
REFERENCES [dbo].[ApplicationRole] ([Id])
GO
ALTER TABLE [dbo].[ApplicatoinUser] CHECK CONSTRAINT [FK_B9543C5D]
GO
ALTER TABLE [dbo].[Dispute]  WITH CHECK ADD  CONSTRAINT [FK_15B0AE7F] FOREIGN KEY([TesterId])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[Dispute] CHECK CONSTRAINT [FK_15B0AE7F]
GO
ALTER TABLE [dbo].[Dispute]  WITH CHECK ADD  CONSTRAINT [FK_B2CD1600] FOREIGN KEY([ResolverId])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[Dispute] CHECK CONSTRAINT [FK_B2CD1600]
GO
ALTER TABLE [dbo].[Dispute]  WITH CHECK ADD  CONSTRAINT [FK_D0EF92F4] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[Dispute] CHECK CONSTRAINT [FK_D0EF92F4]
GO
ALTER TABLE [dbo].[Dispute]  WITH CHECK ADD  CONSTRAINT [FK_F5FDE183] FOREIGN KEY([TestCaseId])
REFERENCES [dbo].[TestCase] ([Id])
GO
ALTER TABLE [dbo].[Dispute] CHECK CONSTRAINT [FK_F5FDE183]
GO
ALTER TABLE [dbo].[DisputeMessage]  WITH CHECK ADD  CONSTRAINT [FK_7527AB88] FOREIGN KEY([UserId])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[DisputeMessage] CHECK CONSTRAINT [FK_7527AB88]
GO
ALTER TABLE [dbo].[DisputeMessage]  WITH CHECK ADD  CONSTRAINT [FK_F1A4DBC1] FOREIGN KEY([Dispute_id])
REFERENCES [dbo].[Dispute] ([Id])
GO
ALTER TABLE [dbo].[DisputeMessage] CHECK CONSTRAINT [FK_F1A4DBC1]
GO
ALTER TABLE [dbo].[Evidence]  WITH CHECK ADD  CONSTRAINT [FK_248018EA] FOREIGN KEY([TestCase_id])
REFERENCES [dbo].[TestCase] ([Id])
GO
ALTER TABLE [dbo].[Evidence] CHECK CONSTRAINT [FK_248018EA]
GO
ALTER TABLE [dbo].[Evidence]  WITH CHECK ADD  CONSTRAINT [FK_6F532ECD] FOREIGN KEY([Tests_id])
REFERENCES [dbo].[Tests] ([Id])
GO
ALTER TABLE [dbo].[Evidence] CHECK CONSTRAINT [FK_6F532ECD]
GO
ALTER TABLE [dbo].[Evidence]  WITH CHECK ADD  CONSTRAINT [FK_B04B04BD] FOREIGN KEY([Dispute_id])
REFERENCES [dbo].[Dispute] ([Id])
GO
ALTER TABLE [dbo].[Evidence] CHECK CONSTRAINT [FK_B04B04BD]
GO
ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [FK_C1E99B52] FOREIGN KEY([Company_id])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [FK_C1E99B52]
GO
ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [FK_CB3B9607] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [FK_CB3B9607]
GO
ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [FK_F5BD80E9] FOREIGN KEY([TestCase_id])
REFERENCES [dbo].[TestCase] ([Id])
GO
ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [FK_F5BD80E9]
GO
ALTER TABLE [dbo].[SoftwareType]  WITH CHECK ADD  CONSTRAINT [FK_CAD36328] FOREIGN KEY([ChangedById])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[SoftwareType] CHECK CONSTRAINT [FK_CAD36328]
GO
ALTER TABLE [dbo].[TestCase]  WITH CHECK ADD  CONSTRAINT [FK_8D2A69AF] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[TestCase] CHECK CONSTRAINT [FK_8D2A69AF]
GO
ALTER TABLE [dbo].[TestCase]  WITH CHECK ADD  CONSTRAINT [FK_A5E51068] FOREIGN KEY([SoftwareTypeId])
REFERENCES [dbo].[SoftwareType] ([Id])
GO
ALTER TABLE [dbo].[TestCase] CHECK CONSTRAINT [FK_A5E51068]
GO
ALTER TABLE [dbo].[TestCase]  WITH CHECK ADD  CONSTRAINT [FK_A88C5AC1] FOREIGN KEY([TestGroup_id])
REFERENCES [dbo].[TestGroup] ([Id])
GO
ALTER TABLE [dbo].[TestCase] CHECK CONSTRAINT [FK_A88C5AC1]
GO
ALTER TABLE [dbo].[TestCase]  WITH CHECK ADD  CONSTRAINT [FK_E32CFBE1] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[TestCategory] ([Id])
GO
ALTER TABLE [dbo].[TestCase] CHECK CONSTRAINT [FK_E32CFBE1]
GO
ALTER TABLE [dbo].[TestCategory]  WITH CHECK ADD  CONSTRAINT [FK_81557806] FOREIGN KEY([ChangedById])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[TestCategory] CHECK CONSTRAINT [FK_81557806]
GO
ALTER TABLE [dbo].[TestGroup]  WITH CHECK ADD  CONSTRAINT [FK_4EE6C2EF] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[TestGroup] CHECK CONSTRAINT [FK_4EE6C2EF]
GO
ALTER TABLE [dbo].[TestGroups]  WITH CHECK ADD  CONSTRAINT [FK_650B6F43] FOREIGN KEY([Tester_id])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[TestGroups] CHECK CONSTRAINT [FK_650B6F43]
GO
ALTER TABLE [dbo].[TestGroups]  WITH CHECK ADD  CONSTRAINT [FK_9DEA53FB] FOREIGN KEY([TestGroup_id])
REFERENCES [dbo].[TestGroup] ([Id])
GO
ALTER TABLE [dbo].[TestGroups] CHECK CONSTRAINT [FK_9DEA53FB]
GO
ALTER TABLE [dbo].[Tests]  WITH CHECK ADD  CONSTRAINT [FK_3D62CED] FOREIGN KEY([Tester_id])
REFERENCES [dbo].[ApplicatoinUser] ([Id])
GO
ALTER TABLE [dbo].[Tests] CHECK CONSTRAINT [FK_3D62CED]
GO
ALTER TABLE [dbo].[Tests]  WITH CHECK ADD  CONSTRAINT [FK_5E6EDF5B] FOREIGN KEY([TestStatus_id])
REFERENCES [dbo].[TestStatus] ([Id])
GO
ALTER TABLE [dbo].[Tests] CHECK CONSTRAINT [FK_5E6EDF5B]
GO
ALTER TABLE [dbo].[Tests]  WITH CHECK ADD  CONSTRAINT [FK_7214B190] FOREIGN KEY([Test_id])
REFERENCES [dbo].[TestCase] ([Id])
GO
ALTER TABLE [dbo].[Tests] CHECK CONSTRAINT [FK_7214B190]
GO
