﻿CREATE TABLE [dbo].[Fdp_ImportData] (
    [FdpImportDataId]                            INT            IDENTITY (1, 1) NOT NULL,
    [FdpImportId]                                INT            NOT NULL,
    [LineNumber]                                 INT            NULL,
    [Pipeline Code]                              NVARCHAR (50)  NULL,
    [Model Year Desc]                            NVARCHAR (50)  NULL,
    [NSC or Importer Description (Vista Market)] NVARCHAR (100) NULL,
    [Country Description]                        NVARCHAR (100) NULL,
    [Derivative Code]                            NVARCHAR (10)  NULL,
    [Trim Pack Description]                      NVARCHAR (100) NULL,
    [Bff Feature Code]                           NVARCHAR (10)  NULL,
    [Feature Description]                        NVARCHAR (255) NULL,
    [Count of Specific Order No]                 NVARCHAR (10)  NULL,
    [Derivative Description]                     NVARCHAR (255) NULL,
    CONSTRAINT [PK_Fdp_ImportData] PRIMARY KEY CLUSTERED ([FdpImportDataId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_Fdp_ImportData_Fdp_Import] FOREIGN KEY ([FdpImportId]) REFERENCES [dbo].[Fdp_Import] ([FdpImportId])
);
























GO
CREATE NONCLUSTERED INDEX [IX_NC_Fdp_Import_DerivativeAndTrim]
    ON [dbo].[Fdp_ImportData]([Derivative Code] ASC, [Derivative Description] ASC, [Trim Pack Description] ASC)
    INCLUDE([Bff Feature Code], [Count of Specific Order No], [Country Description], [FdpImportId]);




GO
CREATE NONCLUSTERED INDEX [IX_NC_Fdp_Import_FeatureCode]
    ON [dbo].[Fdp_ImportData]([Bff Feature Code] ASC, [Feature Description] ASC);




GO
CREATE NONCLUSTERED INDEX [Ix_NC_Fdp_ImportData_Derivative Code]
    ON [dbo].[Fdp_ImportData]([Derivative Code] ASC, [Derivative Description] ASC)
    INCLUDE([Bff Feature Code], [Count of Specific Order No], [FdpImportId], [NSC or Importer Description (Vista Market)], [Trim Pack Description]);




GO
CREATE NONCLUSTERED INDEX [Ix_NC_Fdp_ImportData_Market]
    ON [dbo].[Fdp_ImportData]([NSC or Importer Description (Vista Market)] ASC, [Country Description] ASC);


GO
CREATE NONCLUSTERED INDEX [Ix_NC_Fdp_ImportData_FdpImportId]
    ON [dbo].[Fdp_ImportData]([FdpImportId] ASC)
    INCLUDE([LineNumber], [NSC or Importer Description (Vista Market)], [Country Description], [Derivative Code], [Trim Pack Description], [Bff Feature Code], [Feature Description], [Count of Specific Order No]);




GO
CREATE NONCLUSTERED INDEX [Ix_NC_ImportData_DerivativeCode]
    ON [dbo].[Fdp_ImportData]([FdpImportId] ASC, [Derivative Code] ASC, [Count of Specific Order No] ASC, [Derivative Description] ASC) WITH (FILLFACTOR = 90);




GO
CREATE NONCLUSTERED INDEX [Ix_NC_ImportData_Count]
    ON [dbo].[Fdp_ImportData]([FdpImportId] ASC, [Count of Specific Order No] ASC)
    INCLUDE([Derivative Code]);


GO
CREATE NONCLUSTERED INDEX [Ix_NC_Fdp_ImportData_OrderNo]
    ON [dbo].[Fdp_ImportData]([FdpImportId] ASC, [Count of Specific Order No] ASC)
    INCLUDE([Bff Feature Code], [Feature Description]);


GO
CREATE NONCLUSTERED INDEX [Ix_NC_Fdp_ImportData_ModelAndTrim]
    ON [dbo].[Fdp_ImportData]([FdpImportId] ASC, [Trim Pack Description] ASC, [Count of Specific Order No] ASC)
    INCLUDE([Derivative Code]);


GO
CREATE NONCLUSTERED INDEX [Ix_NC_Fdp_ImportData_Cover]
    ON [dbo].[Fdp_ImportData]([FdpImportId] ASC, [Count of Specific Order No] ASC)
    INCLUDE([Country Description]);

