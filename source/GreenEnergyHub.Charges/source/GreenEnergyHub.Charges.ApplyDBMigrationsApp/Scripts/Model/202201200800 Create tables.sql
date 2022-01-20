-- Transitioning to using the shared actor register we reset all seeded test data in all environments
-- in order to avoid building on or publishing data that does not exist or is meaningless to other domains.
-- This must be deleted from seed data because it needs to apply to environments where test data scripts
-- are no longer executed.

DROP TABLE IF EXISTS Charges.ChargeLink
DROP TABLE IF EXISTS Charges.MeteringPoint
DROP TABLE IF EXISTS Charges.DefaultChargeLink
DROP TABLE IF EXISTS Charges.ChargePoint
DROP TABLE IF EXISTS Charges.Charge
DROP TABLE IF EXISTS Charges.MarketParticipant
DROP TABLE IF EXISTS MessageHub.AvailableChargeDataPoints
DROP TABLE IF EXISTS MessageHub.AvailableChargeData
DROP TABLE IF EXISTS MessageHub.AvailableChargeLinksData
DROP TABLE IF EXISTS MessageHub.AvailableChargeLinksReceiptValidationError
DROP TABLE IF EXISTS MessageHub.AvailableChargeLinksReceiptData
DROP TABLE IF EXISTS MessageHub.AvailableChargeReceiptValidationError
DROP TABLE IF EXISTS MessageHub.AvailableChargeReceiptData
DROP TABLE IF EXISTS dbo.SchemaVersions
GO

IF NOT EXISTS ( SELECT  *
    FROM    sys.schemas
    WHERE   name = N'Charges' )
    EXEC('CREATE SCHEMA [Charges]');
GO

IF NOT EXISTS ( SELECT  *
    FROM    sys.schemas
    WHERE   name = N'MessageHub' )
    EXEC('CREATE SCHEMA [MessageHub]');
GO

CREATE TABLE [Charges].[Charge](
    [Id] [uniqueidentifier] NOT NULL,
    [SenderProvidedChargeId] [nvarchar](35) NOT NULL,
    [Type] [int] NOT NULL,
    [OwnerId] [uniqueidentifier] NOT NULL,
    [TaxIndicator] [bit] NOT NULL,
    [Resolution] [int] NOT NULL,
    [TransparentInvoicing] [bit] NOT NULL,
    [Description] [nvarchar](2048) NOT NULL,
    [Name] [nvarchar](132) NOT NULL,
    [VatClassification] [int] NOT NULL,
    [StartDateTime] [datetime2](7) NOT NULL,
    [EndDateTime] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_Charge] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [Charges].[ChargeLink](
    [Id] [uniqueidentifier] NOT NULL,
    [ChargeId] [uniqueidentifier] NOT NULL,
    [MeteringPointId] [uniqueidentifier] NOT NULL,
    [StartDateTime] [datetime2](7) NOT NULL,
    [EndDateTime] [datetime2](7) NOT NULL,
    [Factor] [int] NOT NULL,
    CONSTRAINT [PK_ChargeLink] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [Charges].[ChargePoint](
    [Id] [uniqueidentifier] NOT NULL,
    [ChargeId] [uniqueidentifier] NOT NULL,
    [Time] [datetime2](7) NOT NULL,
    [Price] [decimal](14, 6) NOT NULL,
    [Position] [int] NOT NULL,
    CONSTRAINT [PK_ChargePrice] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [Charges].[DefaultChargeLink](
    [Id] [uniqueidentifier] NOT NULL,
    [MeteringPointType] [int] NOT NULL,
    [ChargeId] [uniqueidentifier] NOT NULL,
    [StartDateTime] [datetime2](7) NOT NULL,
    [EndDateTime] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_DefaultChargeLink] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [Charges].[MarketParticipant](
    [Id] [uniqueidentifier] NOT NULL,
    [MarketParticipantId] [nvarchar](35) NOT NULL,
    [BusinessProcessRole] [int] NULL,
    [IsActive] [bit] NOT NULL,
    CONSTRAINT [PK_MarketParticipant] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    UNIQUE NONCLUSTERED
(
[MarketParticipantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [Charges].[MeteringPoint](
    [Id] [uniqueidentifier] NOT NULL,
    [MeteringPointId] [nvarchar](50) NOT NULL,
    [MeteringPointType] [int] NOT NULL,
    [GridAreaId] [nvarchar](50) NOT NULL,
    [EffectiveDate] [datetime2](7) NOT NULL,
    [ConnectionState] [int] NOT NULL,
    [SettlementMethod] [int] NULL,
    CONSTRAINT [PK_MeteringPoint] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    CONSTRAINT [UC_MeteringPointId] UNIQUE NONCLUSTERED
(
[MeteringPointId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [dbo].[SchemaVersions](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ScriptName] [nvarchar](255) NOT NULL,
    [Applied] [datetime] NOT NULL,
    CONSTRAINT [PK_SchemaVersions_Id] PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [MessageHub].[AvailableChargeData](
    [Id] [uniqueidentifier] NOT NULL,
    [RecipientId] [nvarchar](35) NOT NULL,
    [RecipientRole] [int] NOT NULL,
    [BusinessReasonCode] [int] NOT NULL,
    [ChargeId] [nvarchar](35) NOT NULL,
    [ChargeOwner] [nvarchar](35) NOT NULL,
    [ChargeType] [int] NOT NULL,
    [ChargeName] [nvarchar](132) NOT NULL,
    [ChargeDescription] [nvarchar](2048) NOT NULL,
    [StartDateTime] [datetime2](7) NOT NULL,
    [EndDateTime] [datetime2](7) NOT NULL,
    [TaxIndicator] [bit] NOT NULL,
    [TransparentInvoicing] [bit] NOT NULL,
    [VatClassification] [int] NOT NULL,
    [Resolution] [int] NOT NULL,
    [RequestDateTime] [datetime2](7) NOT NULL,
    [AvailableDataReferenceId] [uniqueidentifier] NOT NULL,
    [SenderId] [nvarchar](35) NOT NULL,
    [SenderRole] [int] NOT NULL,
    CONSTRAINT [PK_AvailableChargeData] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [MessageHub].[AvailableChargeDataPoints](
    [Id] [uniqueidentifier] NOT NULL,
    [AvailableChargeDataId] [uniqueidentifier] NOT NULL,
    [Position] [int] NOT NULL,
    [Price] [decimal](14, 6) NOT NULL,
    CONSTRAINT [PK_AvailableChargeDataPoints] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [MessageHub].[AvailableChargeLinksData](
    [Id] [uniqueidentifier] NOT NULL,
    [RecipientId] [varchar](70) NOT NULL,
    [RecipientRole] [int] NOT NULL,
    [BusinessReasonCode] [int] NOT NULL,
    [ChargeId] [varchar](70) NOT NULL,
    [ChargeOwner] [varchar](70) NOT NULL,
    [ChargeType] [int] NOT NULL,
    [MeteringPointId] [nvarchar](50) NOT NULL,
    [Factor] [int] NOT NULL,
    [StartDateTime] [datetime2](7) NOT NULL,
    [EndDateTime] [datetime2](7) NOT NULL,
    [RequestDateTime] [datetime2](7) NOT NULL,
    [AvailableDataReferenceId] [uniqueidentifier] NOT NULL,
    [SenderId] [nvarchar](35) NOT NULL,
    [SenderRole] [int] NOT NULL,
    PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [MessageHub].[AvailableChargeLinksReceiptData](
    [Id] [uniqueidentifier] NOT NULL,
    [RecipientId] [nvarchar](35) NOT NULL,
    [RecipientRole] [int] NOT NULL,
    [BusinessReasonCode] [int] NOT NULL,
    [ReceiptStatus] [int] NOT NULL,
    [OriginalOperationId] [nvarchar](35) NOT NULL,
    [MeteringPointId] [nvarchar](50) NOT NULL,
    [RequestDateTime] [datetime2](7) NOT NULL,
    [AvailableDataReferenceId] [uniqueidentifier] NOT NULL,
    [SenderId] [nvarchar](35) NOT NULL,
    [SenderRole] [int] NOT NULL,
    CONSTRAINT [PK_AvailableChargeLinkReceiptData] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [MessageHub].[AvailableChargeLinksReceiptValidationError](
    [Id] [uniqueidentifier] NOT NULL,
    [AvailableChargeLinkReceiptDataId] [uniqueidentifier] NOT NULL,
    [ReasonCode] [int] NOT NULL,
    [Text] [nvarchar](max) NULL,
    CONSTRAINT [PK_AvailableChargeLinkReceiptDataReasonCodes] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    GO

CREATE TABLE [MessageHub].[AvailableChargeReceiptData](
    [Id] [uniqueidentifier] NOT NULL,
    [RecipientId] [nvarchar](35) NOT NULL,
    [RecipientRole] [int] NOT NULL,
    [BusinessReasonCode] [int] NOT NULL,
    [ReceiptStatus] [int] NOT NULL,
    [OriginalOperationId] [nvarchar](35) NOT NULL,
    [RequestDateTime] [datetime2](7) NOT NULL,
    [AvailableDataReferenceId] [uniqueidentifier] NOT NULL,
    [SenderId] [nvarchar](35) NOT NULL,
    [SenderRole] [int] NOT NULL,
    CONSTRAINT [PK_AvailableChargeReceiptData] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

CREATE TABLE [MessageHub].[AvailableChargeReceiptValidationError](
    [Id] [uniqueidentifier] NOT NULL,
    [AvailableChargeReceiptDataId] [uniqueidentifier] NOT NULL,
    [ReasonCode] [int] NOT NULL,
    [Text] [nvarchar](max) NOT NULL,
    CONSTRAINT [PK_AvailableChargeReceiptDataReasonCodes] PRIMARY KEY NONCLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    GO
    SET ANSI_PADDING ON
    GO
/****** Object:  Index [IX_SenderProvidedChargeId_ChargeType_MarketParticipantId]    Script Date: 20-01-2022 07:51:33 ******/
CREATE NONCLUSTERED INDEX [IX_SenderProvidedChargeId_ChargeType_MarketParticipantId] ON [Charges].[Charge]
(
	[SenderProvidedChargeId] ASC,
	[Type] ASC,
	[OwnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MeteringPointId_ChargeId]    Script Date: 20-01-2022 07:51:33 ******/
CREATE NONCLUSTERED INDEX [IX_MeteringPointId_ChargeId] ON [Charges].[ChargeLink]
(
	[MeteringPointId] DESC,
	[ChargeId] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MeteringPointType_StartDateTime_EndDateTime]    Script Date: 20-01-2022 07:51:33 ******/
CREATE NONCLUSTERED INDEX [IX_MeteringPointType_StartDateTime_EndDateTime] ON [Charges].[DefaultChargeLink]
(
	[MeteringPointType] ASC,
	[StartDateTime] DESC,
	[EndDateTime] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_MeteringPointId]    Script Date: 20-01-2022 07:51:33 ******/
CREATE NONCLUSTERED INDEX [IX_MeteringPointId] ON [Charges].[MeteringPoint]
(
	[MeteringPointId] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [i1]    Script Date: 20-01-2022 07:51:33 ******/
CREATE NONCLUSTERED INDEX [i1] ON [MessageHub].[AvailableChargeLinksReceiptValidationError]
(
	[AvailableChargeLinkReceiptDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [i1]    Script Date: 20-01-2022 07:51:33 ******/
CREATE NONCLUSTERED INDEX [i1] ON [MessageHub].[AvailableChargeReceiptValidationError]
(
	[AvailableChargeReceiptDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [Charges].[ChargePoint] ADD  DEFAULT ((1)) FOR [Position]
    GO
ALTER TABLE [Charges].[Charge]  WITH CHECK ADD FOREIGN KEY([OwnerId])
    REFERENCES [Charges].[MarketParticipant] ([Id])
    GO
ALTER TABLE [Charges].[ChargeLink]  WITH CHECK ADD FOREIGN KEY([ChargeId])
    REFERENCES [Charges].[Charge] ([Id])
    GO
ALTER TABLE [Charges].[ChargeLink]  WITH CHECK ADD FOREIGN KEY([MeteringPointId])
    REFERENCES [Charges].[MeteringPoint] ([Id])
    GO
ALTER TABLE [Charges].[ChargePoint]  WITH CHECK ADD  CONSTRAINT [FK_Charge] FOREIGN KEY([ChargeId])
    REFERENCES [Charges].[Charge] ([Id])
    GO
ALTER TABLE [Charges].[ChargePoint] CHECK CONSTRAINT [FK_Charge]
    GO
ALTER TABLE [Charges].[DefaultChargeLink]  WITH CHECK ADD FOREIGN KEY([ChargeId])
    REFERENCES [Charges].[Charge] ([Id])
    GO
ALTER TABLE [MessageHub].[AvailableChargeDataPoints]  WITH CHECK ADD FOREIGN KEY([AvailableChargeDataId])
    REFERENCES [MessageHub].[AvailableChargeData] ([Id])
    GO
ALTER TABLE [MessageHub].[AvailableChargeLinksReceiptValidationError]  WITH CHECK ADD FOREIGN KEY([AvailableChargeLinkReceiptDataId])
    REFERENCES [MessageHub].[AvailableChargeLinksReceiptData] ([Id])
    GO
ALTER TABLE [MessageHub].[AvailableChargeReceiptValidationError]  WITH CHECK ADD FOREIGN KEY([AvailableChargeReceiptDataId])
    REFERENCES [MessageHub].[AvailableChargeReceiptData] ([Id])
    GO
