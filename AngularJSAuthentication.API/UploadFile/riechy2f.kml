-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
-- =============================================  
CREATE PROCEDURE [dbo].[sp_login]  
 -- Add the parameters for the stored procedure here  
 @UserName nvarchar(245),  
 @Password nvarchar(max)  
AS  
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
    -- Insert statements for procedure here  
 SELECT * from AspNetUsers where UserName = @UserName and PasswordHash = @Password  
END  