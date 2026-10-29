/*
 Navicat Premium Dump SQL

 Source Server         : ACC
 Source Server Type    : SQL Server
 Source Server Version : 15002180 (15.00.2180)
 Source Host           : 192.168.1.18:50000
 Source Catalog        : DHYTCC2026
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 15002180 (15.00.2180)
 File Encoding         : 65001

 Date: 26/10/2026 22:30:40
*/


-- ----------------------------
-- Table structure for AAACCCD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[AAACCCD]') AND type IN ('U'))
	DROP TABLE [dbo].[AAACCCD]
GO

CREATE TABLE [dbo].[AAACCCD] (
  [STT] int  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CCCD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[AAACCCD] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for AAADOISOTAIKHOAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[AAADOISOTAIKHOAN]') AND type IN ('U'))
	DROP TABLE [dbo].[AAADOISOTAIKHOAN]
GO

CREATE TABLE [dbo].[AAADOISOTAIKHOAN] (
  [STT] int  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOANCU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOANMOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[AAADOISOTAIKHOAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for ABCBANGKETHUNHAPCHITIET
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ABCBANGKETHUNHAPCHITIET]') AND type IN ('U'))
	DROP TABLE [dbo].[ABCBANGKETHUNHAPCHITIET]
GO

CREATE TABLE [dbo].[ABCBANGKETHUNHAPCHITIET] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [NGAYCT] smalldatetime  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] numeric(18)  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LPHIEULUU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENTT] float(53)  NULL,
  [SOTIENCONLAI] float(53)  NULL,
  [MAMUCTIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL,
  [SOTIENCHIUTHUE] float(53)  NULL,
  [THUEKHAUTRU] float(53)  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYXOA] datetime  NULL
)
GO

ALTER TABLE [dbo].[ABCBANGKETHUNHAPCHITIET] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for AUTOUPDATE
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[AUTOUPDATE]') AND type IN ('U'))
	DROP TABLE [dbo].[AUTOUPDATE]
GO

CREATE TABLE [dbo].[AUTOUPDATE] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [OKOK] bit  NULL,
  [NGAYUPDATE] smalldatetime  NULL,
  [PATHAUTOUPDATE] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [THOIGIAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [DIENGIAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MYCOMPUTER] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [LASTUPDATE] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[AUTOUPDATE] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for AZBAOCAOTMPZZMRNAM78
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[AZBAOCAOTMPZZMRNAM78]') AND type IN ('U'))
	DROP TABLE [dbo].[AZBAOCAOTMPZZMRNAM78]
GO

CREATE TABLE [dbo].[AZBAOCAOTMPZZMRNAM78] (
  [MDONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MDONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(355) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MDONVI1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAIKHOAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTAIKHOAN] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCAP2] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUNGTUGHISO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCT] smalldatetime  NULL,
  [DIENGIAI] nvarchar(455) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKDOIUNG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENNO] float(53)  NULL,
  [SOTIENCO] float(53)  NULL,
  [SOTIENNOUSD] float(53)  NULL,
  [SOTIENCOUSD] float(53)  NULL,
  [SOTIENNO1] float(53)  NULL,
  [SOTIENCO1] float(53)  NULL,
  [SOTIENNOUSD1] float(53)  NULL,
  [SOTIENCOUSD1] float(53)  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THOIGIAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGNGAY] int  NULL,
  [CTGS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DV] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIA] float(53)  NULL,
  [LUYKENO] float(53)  NULL,
  [LUYKECO] float(53)  NULL,
  [LUYKENO1] float(53)  NULL,
  [LUYKECO1] float(53)  NULL,
  [LUYKENOUSD] float(53)  NULL,
  [LUYKECOUSD] float(53)  NULL,
  [LUYKENOUSD1] float(53)  NULL,
  [LUYKECOUSD1] float(53)  NULL,
  [LOAI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MKP] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(355) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDOITUONG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYNGAY] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGTHANG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMATTXT] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODUDAUKYNO] float(53)  NULL,
  [SODUDAUKYCO] float(53)  NULL,
  [SODUDAUKYNOUSD] float(53)  NULL,
  [SODUDAUKYCOUSD] float(53)  NULL,
  [SODUDCDAUKYNO] float(53)  NULL,
  [SODUDCDAUKYCO] float(53)  NULL,
  [SODUDCDAUKYNOUSD] float(53)  NULL,
  [SODUDCDAUKYCOUSD] float(53)  NULL,
  [SODUCUOINO] float(53)  NULL,
  [SODUCUOICO] float(53)  NULL,
  [SODUCUOINOUSD] float(53)  NULL,
  [SODUCUOICOUSD] float(53)  NULL,
  [SOLUONGNO] float(53)  NULL,
  [SOLUONGCO] float(53)  NULL,
  [SOLUONGTON] float(53)  NULL,
  [SOLUONGLKNO] float(53)  NULL,
  [SOLUONGLKCO] float(53)  NULL,
  [SOLUONGSODU] float(53)  NULL,
  [SOLUONGDCSODU] float(53)  NULL,
  [SOHOADON] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYHOADON] smalldatetime  NULL,
  [SODUDAUKYNO1] float(53)  NULL,
  [SODUDAUKYCO1] float(53)  NULL,
  [SODUDAUKYNOUSD1] float(53)  NULL,
  [SODUDAUKYCOUSD1] float(53)  NULL,
  [SODUDCDAUKYNO1] float(53)  NULL,
  [SODUDCDAUKYCO1] float(53)  NULL,
  [SODUDCDAUKYNOUSD1] float(53)  NULL,
  [SODUDCDAUKYCOUSD1] float(53)  NULL,
  [SODUCUOINO1] float(53)  NULL,
  [SODUCUOICO1] float(53)  NULL,
  [SODUCUOINOUSD1] float(53)  NULL,
  [SODUCUOICOUSD1] float(53)  NULL,
  [STT] int  NULL,
  [SODUCO] float(53)  NULL,
  [SODUNO] float(53)  NULL,
  [SODUCOUSD] float(53)  NULL,
  [SODUNOUSD] float(53)  NULL,
  [SODUCO1] float(53)  NULL,
  [SODUNO1] float(53)  NULL,
  [SODUCOUSD1] float(53)  NULL,
  [SODUNOUSD1] float(53)  NULL,
  [KYHIEU] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAUSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUDE] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[AZBAOCAOTMPZZMRNAM78] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGDOICHIEUSODUNGANHANG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGDOICHIEUSODUNGANHANG]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGDOICHIEUSODUNGANHANG]
GO

CREATE TABLE [dbo].[BANGDOICHIEUSODUNGANHANG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [THANG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODUDAUKT] float(53)  NULL,
  [SOTIENNOKT] float(53)  NULL,
  [SOTIENCOKT] float(53)  NULL,
  [SODUCUOIKT] float(53)  NULL,
  [SODUDAUSOPHU] float(53)  NULL,
  [SOTIENNOSOPHU] float(53)  NULL,
  [SODUCUOISOPHU] float(53)  NULL,
  [SOTIENCOSOPHU] float(53)  NULL,
  [CHENHLECH] float(53)  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BANGDOICHIEUSODUNGANHANG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGDOICHIEUTAMUNG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGDOICHIEUTAMUNG]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGDOICHIEUTAMUNG]
GO

CREATE TABLE [dbo].[BANGDOICHIEUTAMUNG] (
  [ID] int  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BANGDOICHIEUTAMUNG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGDULIEUCHUNGTUNGANHANG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGDULIEUCHUNGTUNGANHANG]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGDULIEUCHUNGTUNGANHANG]
GO

CREATE TABLE [dbo].[BANGDULIEUCHUNGTUNGANHANG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASOTHUNHAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHUNHAP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [NGAYCT] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTUTIENMAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTIENMAT] smalldatetime  NULL,
  [SOTIENMAT] float(53)  NULL,
  [SOTIENCKVIEN] float(53)  NULL,
  [SOTIENTHUEVIEN] float(53)  NULL,
  [SOTIENCKNGOAI] float(53)  NULL,
  [SOTIENTHUENGOAI] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGDULIEUCHUNGTUNGANHANG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGKECONGCUDUNGCU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGKECONGCUDUNGCU]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGKECONGCUDUNGCU]
GO

CREATE TABLE [dbo].[BANGKECONGCUDUNGCU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [NGAYCT] smalldatetime  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] numeric(18)  NULL,
  [SOTIEN] numeric(18)  NULL,
  [DONGIA] numeric(18,3)  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTHANGPHANBO] int  NULL,
  [SOTIENPHANBO] numeric(18)  NULL,
  [SOTHANGKYNAY] int  NULL,
  [SOTIENCONLAI] numeric(18)  NULL,
  [SOTHANGDAPHANBO] int  NULL,
  [SOTIENDAPHANBO] numeric(18)  NULL,
  [SOTHANGCONLAI] int  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASANPHAM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENSANPHAM] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BANGKECONGCUDUNGCU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGKETHUEVAT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGKETHUEVAT]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGKETHUEVAT]
GO

CREATE TABLE [dbo].[BANGKETHUEVAT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [NGAYCT] smalldatetime  NULL,
  [NGAYHOADON] smalldatetime  NULL,
  [SOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLEVAT] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [TONGCONG] float(53)  NULL,
  [THUEVAT] float(53)  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(3) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [OK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHONGCHIUTHUE] bit DEFAULT 0 NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHIUTHUE] bit DEFAULT 0 NULL,
  [LOAIVAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTAO] smalldatetime  NULL,
  [USERNAME] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITHUE] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAUSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUMAUHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [DONVITINH] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BANGKETHUEVAT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGKETHUNHAP
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGKETHUNHAP]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGKETHUNHAP]
GO

CREATE TABLE [dbo].[BANGKETHUNHAP] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTUGOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUYEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOBIENLAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DANGKYNGAY] smalldatetime  NULL,
  [NGAYCT] smalldatetime  NULL,
  [THUNHAPCHIUTHUE] float(53)  NULL,
  [GIAMTRUGIACANH] float(53)  NULL,
  [BHXHYT] float(53)  NULL,
  [THUNHAPTINHTHUE] float(53)  NULL,
  [THUETHUONGXUYEN] float(53)  NULL,
  [THUEKHONGTHUONGXUYEN] float(53)  NULL,
  [NOPTHUONGXUYEN] float(53)  NULL,
  [NOPKHONGTHUONGXUYEN] float(53)  NULL,
  [THUECONTHUONGXUYEN] float(53)  NULL,
  [THUECONKHONGTHUONGXUYEN] float(53)  NULL,
  [THUECOHOADON] float(53)  NULL,
  [THUEKHONGHOADON] float(53)  NULL,
  [THUETNCN] float(53)  NULL,
  [COHOPDONG] bit  NULL,
  [KHONGHOPDONG] bit  NULL,
  [NAMHOATDONG] int  NULL,
  [SONGUOIPHUTHUOC] float(53)  NULL,
  [LOAI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(355) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BANGKETHUNHAP] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGKETHUNHAPCHITIET
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGKETHUNHAPCHITIET]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGKETHUNHAPCHITIET]
GO

CREATE TABLE [dbo].[BANGKETHUNHAPCHITIET] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [NGAYCT] smalldatetime  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] numeric(18)  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LPHIEULUU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENTT] float(53)  NULL,
  [SOTIENCONLAI] float(53)  NULL,
  [MAMUCTIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL,
  [SOTIENCHIUTHUE] float(53)  NULL,
  [THUEKHAUTRU] float(53)  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYXOA] datetime  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN10] float(53)  NULL,
  [MALOP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENLOP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BANGKETHUNHAPCHITIET] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGKETSCD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGKETSCD]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGKETSCD]
GO

CREATE TABLE [dbo].[BANGKETSCD] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [NGAYCT] smalldatetime  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] numeric(18)  NULL,
  [SOTIEN] numeric(18)  NULL,
  [DONGIA] numeric(18,3)  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTHANGPHANBO] numeric(18)  NULL,
  [SOTIENPHANBO] numeric(18)  NULL,
  [SOTHANGKYNAY] int  NULL,
  [SOTIENCONLAI] numeric(18)  NULL,
  [SOTHANGDAPHANBO] int  NULL,
  [SOTIENDAPHANBO] numeric(18)  NULL,
  [SOTHANGCONLAI] int  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASANPHAM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENSANPHAM] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SONAMKHAUHAO] numeric(18)  NULL,
  [TYLEKHAUHAO] numeric(18,1)  NULL
)
GO

ALTER TABLE [dbo].[BANGKETSCD] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGKIEMTRATHUEVAT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGKIEMTRATHUEVAT]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGKIEMTRATHUEVAT]
GO

CREATE TABLE [dbo].[BANGKIEMTRATHUEVAT] (
  [NGAYCT] smalldatetime  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [SOTIENTHUE] float(53)  NULL,
  [LOAI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHENHLECH] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGKIEMTRATHUEVAT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONG]
GO

CREATE TABLE [dbo].[BANGLUONG] (
  [NGAYCT] smalldatetime  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMNHANVIEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGACH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BACLUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [HESOPHUCAPVUOTKHUNG] float(53)  NULL,
  [HESOPHUCAPUUDAINGHE] float(53)  NULL,
  [HESOPHUCAPCHUCVU] float(53)  NULL,
  [HESOPHUCAPPHUCVU] float(53)  NULL,
  [HESOPHUCAPTRACHNHIEM] float(53)  NULL,
  [HESOPHUCAPTHAMNIEN] float(53)  NULL,
  [HESOPHUCAPDOCHAI] float(53)  NULL,
  [HESOPHUCAPKHAC] float(53)  NULL,
  [CONGHESO1] float(53)  NULL,
  [CONGHESO2] float(53)  NULL,
  [LUONGCOBAN] float(53)  NULL,
  [LUONGPHUCAPVUOTKHUNG] float(53)  NULL,
  [LUONGPHUCAPUUDAINGHE] float(53)  NULL,
  [LUONGPHUCAPCHUCVU] float(53)  NULL,
  [LUONGPHUCAPPHUCVU] float(53)  NULL,
  [LUONGPHUCAPTRACHNHIEM] float(53)  NULL,
  [LUONGPHUCAPTHAMNIEN] float(53)  NULL,
  [LUONGPHUCAPDOCHAI] float(53)  NULL,
  [LUONGPHUCAPKHAC] float(53)  NULL,
  [TONGCONGPHUCAP] float(53)  NULL,
  [TONGCONGTIENLUONG] float(53)  NULL,
  [TONGLUONGPHUCAP] float(53)  NULL,
  [TRUBHXH] float(53)  NULL,
  [TRUBHYT] float(53)  NULL,
  [TRUBHTN] float(53)  NULL,
  [TRUKPCD] float(53)  NULL,
  [TRUTHUETNCN] float(53)  NULL,
  [TRUKHAC1] float(53)  NULL,
  [TRUKHAC2] float(53)  NULL,
  [TRUKHAC3] float(53)  NULL,
  [DIENTHOAI] float(53)  NULL,
  [TONGCONGTRU] float(53)  NULL,
  [TROCAPKHOKHAN] float(53)  NULL,
  [TRUKHAC] float(53)  NULL,
  [DIENTHOAIKHOAN] float(53)  NULL,
  [BHCHIU] float(53)  NULL,
  [BANTHANCHIU] float(53)  NULL,
  [CONLAI] float(53)  NULL,
  [CHUCDANH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHONGBHXH] float(53)  NULL,
  [KHONGPCCONGVU] float(53)  NULL,
  [HESOPCTN] float(53)  NULL,
  [LUONGBHXH] float(53)  NULL,
  [HESOLUONGBHXH] float(53)  NULL,
  [LUONGNS] float(53)  NULL,
  [LUONGLP] float(53)  NULL,
  [THUNHAPTANGTHEM] float(53)  NULL,
  [THUNHAPUUDAINGHE] float(53)  NULL,
  [DIENGIAI] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUONGTOITHIEU] float(53)  NULL,
  [GIAMTRUBANTHAN] float(53)  NULL,
  [NGUOIPHUTHUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAU] float(53)  NULL,
  [SOCUOI] float(53)  NULL,
  [NGAYOK] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGHICT] bit  NULL,
  [SODUNG] float(53)  NULL,
  [TIENDIEN] float(53)  NULL,
  [TAMUNG] float(53)  NULL,
  [HOTROTN] float(53)  NULL,
  [LOAIHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHONGDAU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PC8] float(53)  NULL,
  [LUONGPHUCAPVKTHUCLINH] float(53)  NULL,
  [LUONGPHUCAPCVTHUCLINH] float(53)  NULL,
  [LUONGPHUCAPUDNTHUCLINH] float(53)  NULL,
  [LUONGPHUCAPDHTHUCLINH] float(53)  NULL,
  [TONGLUONGTHUCLINH2] float(53)  NULL,
  [KHOAMO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONGHIENTAI] float(53)  NULL,
  [KHONGTHEOCONGTHUC] bit  NULL,
  [SELECTTED] bit  NULL,
  [NGAYCONGTHUCTE] float(53)  NULL,
  [NGAYCONGTHANG] float(53)  NULL,
  [TYLEHUONG] float(53)  NULL,
  [HESOKPCD] float(53)  NULL,
  [TYLECONLAI] float(53)  NULL,
  [TIEUDETRUKHAC] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGLUONGKHOBAC] float(53)  NULL,
  [LISBANGLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUTIENNGAYNGHI] float(53)  NULL,
  [BHXHTHAYLUONG] float(53)  NULL,
  [TRUYTHU] float(53)  NULL,
  [TRUYLINH] float(53)  NULL,
  [HINHTHUCLAODONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLEBH] float(53)  NULL,
  [TYLEBHXH] float(53)  NULL,
  [TYLEBHYT] float(53)  NULL,
  [TYLEBHTN] float(53)  NULL,
  [SOCONLAI] float(53)  NULL,
  [SONGAYNGHI] float(53)  NULL,
  [MAMOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUONGPHUCAPKHAC68] float(53)  NULL,
  [TYLEKPCD] float(53)  NULL,
  [TRUKHAC4] float(53)  NULL,
  [TRUKHAC5] float(53)  NULL,
  [PHANTRAMVUOTKHUNG] float(53)  NULL,
  [PHANTRAMGIANGDAY] float(53)  NULL,
  [PHANTRAMTHAMNIEN] float(53)  NULL,
  [HESOPHUCAPGIANGDAY] float(53)  NULL,
  [TRUTIENDINUOCNGOAI] float(53)  NULL,
  [TONGLUONGHUONG] float(53)  NULL,
  [DIEM] float(53)  NULL,
  [HESOPHUCAPDAOTAO] float(53)  NULL,
  [LUONGKHOAN] float(53)  NULL,
  [HESOHOTROTHAMNIEN] float(53)  NULL,
  [ANTRUADILAI] float(53)  NULL,
  [TONGNGAYNGHI] float(53)  NULL,
  [PHANTRAMHOTRODAOTAO] float(53)  NULL,
  [PHANTRAMHOTROTHAMNIEN] float(53)  NULL,
  [TYLENN] float(53)  NULL,
  [DINHMUCANTRUA] float(53)  NULL,
  [KHOANTTXN] float(53)  NULL,
  [KHOANPKDK] float(53)  NULL,
  [GHICHUDV] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIEUCHINHTRONSO] float(53)  NULL,
  [DINHMUCTANGTHEM] float(53)  NULL,
  [TONGCONGLUONG] float(53)  NULL,
  [SONGUOIPHUTHUOC] float(53)  NULL,
  [ANTRUATTXN] float(53)  NULL,
  [THUHUT] float(53)  NULL,
  [DIEM1] float(53)  NULL,
  [DIEM2] float(53)  NULL,
  [LUONGKHOAN1] float(53)  NULL,
  [LUONGKHOAN2] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONGKHAC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONGKHAC]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONGKHAC]
GO

CREATE TABLE [dbo].[BANGLUONGKHAC] (
  [NGAYCT] smalldatetime  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [CHUCVU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [HESOLUONGCOBAN] float(53)  NULL,
  [LUONGCOBAN] float(53)  NULL,
  [NGAYTHUONG] float(53)  NULL,
  [NGAYNGHI] float(53)  NULL,
  [NGAYLE] float(53)  NULL,
  [LUONGNGAYTHUONG] float(53)  NULL,
  [LUONGNGAYNGHI] float(53)  NULL,
  [LUONGNGAYLE] float(53)  NULL,
  [TONGLUONGLINH] float(53)  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGANHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTA] int  NULL,
  [NOIDUNGTANGTHEM] nvarchar(180) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHONGDAU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUNHAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGCONGTRUC] float(53)  NULL,
  [TIENNGAYTRUC] float(53)  NULL,
  [SOLUONG] float(53) DEFAULT 0 NULL,
  [DONGIA] float(53)  NULL,
  [TUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [HESOPHUCAPVUOTKHUNG] float(53)  NULL,
  [HESOPHUCAPCHUCVU] float(53)  NULL,
  [TONGHESOLUONG] float(53)  NULL,
  [TENTHUTHUAT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSO] float(53)  NULL,
  [MUCHUONG] float(53)  NULL,
  [LOAITHUTHUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUOTDI] float(53)  NULL,
  [NOITHANH] float(53)  NULL,
  [NGOAITHANH] float(53)  NULL,
  [TINHXA] float(53)  NULL,
  [LUOTDINGOAITHANH] float(53)  NULL,
  [LUOTDITINHXA] float(53)  NULL,
  [SONGUOI] float(53)  NULL,
  [THANGKHAC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHUNHAP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGLUONGHESO] float(53)  NULL,
  [THEOPHONG] bit  NULL,
  [KHONGTHEOCONGTHUC] bit  NULL,
  [SELECTTED] bit  NULL,
  [TYLEHUONG] float(53)  NULL,
  [HESOSOLUONGMAU] float(53)  NULL,
  [HESOSOLUONGTINHNGUYEN] float(53)  NULL,
  [HESO250ML] float(53)  NULL,
  [HESOKTC] float(53)  NULL,
  [HESOMAUHUY] float(53)  NULL,
  [TONGHESOTHAMKHAO] float(53)  NULL,
  [SOLUONGNHANLUC] float(53)  NULL,
  [MUCKHOAN] float(53)  NULL,
  [TYLEMAUTHUGOM] float(53)  NULL,
  [TYLELAYMAUTN] float(53)  NULL,
  [SODONVI250] float(53)  NULL,
  [TYLEDIEUCHE] float(53)  NULL,
  [TYLEMAUHUY] float(53)  NULL,
  [CONGHESO] float(53)  NULL,
  [HESODIEUCHINH] float(53)  NULL,
  [HESOCHUNG] float(53)  NULL,
  [HOTENCON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGTINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUTNCN] float(53)  NULL,
  [HESOHUONG] float(53)  NULL,
  [LUONGPHUCAPTRACHNHIEM] float(53)  NULL,
  [TONGLUONGSAUTHUE] float(53)  NULL,
  [PHUCAPDIENTHOAI] float(53)  NULL,
  [DISOM6H45] float(53)  NULL,
  [DISOM6H30] float(53)  NULL,
  [SOTIENDISOM6H45] float(53)  NULL,
  [SOTIENDISOM6H30] float(53)  NULL,
  [TONGHUONG] float(53)  NULL,
  [SOTIENHUONG] float(53)  NULL,
  [TRUYLINH] float(53)  NULL,
  [TRUYTHU] float(53)  NULL,
  [HESOPHUCAPTHAMNIEN] float(53)  NULL,
  [TONGLUONGPHUCAP] float(53)  NULL,
  [HESOTHAMNIEN] float(53)  NULL,
  [HESOPHUCAPTRACHNHIEM] float(53)  NULL,
  [TONGHESOPHUCAP] float(53)  NULL,
  [TONGHESO] float(53)  NULL,
  [LOAIHD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTROTHAISAN] float(53)  NULL,
  [NGAYCONGHESO] float(53)  NULL,
  [NGAYCONGTHANG] float(53)  NULL,
  [NGAYCONGTHUCTE] float(53)  NULL,
  [TIENKYTHUATGMVACD] float(53)  NULL,
  [BACLUONG] float(53)  NULL,
  [PHAYBAC] float(53)  NULL,
  [LUONGPHAY] float(53)  NULL,
  [HESONGAYLUONG] float(53)  NULL,
  [MATHUNHAP] float(53)  NULL,
  [CHUYENKHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBENHNHAN] float(53)  NULL,
  [TUOI] float(53)  NULL,
  [MABENHAN] float(53)  NULL,
  [BAOHIEM] float(53)  NULL,
  [NGAYPHAUTHUAT] float(53)  NULL,
  [CACHTHUCPHAUTHUAT] float(53)  NULL,
  [TENDICHVU] float(53)  NULL,
  [MO] float(53)  NULL,
  [GIOMOCAPCUU] float(53)  NULL,
  [PHUONGPHAPVOCAM] float(53)  NULL,
  [LOAIDICHVU] float(53)  NULL,
  [MADONVI1] float(53)  NULL,
  [TENDONVI1] float(53)  NULL,
  [MUCHUONG1] float(53)  NULL,
  [MADONVI2] float(53)  NULL,
  [TENDONVI2] float(53)  NULL,
  [MUCHUONG2] float(53)  NULL,
  [MADONVI3] float(53)  NULL,
  [TENDONVI3] float(53)  NULL,
  [MUCHUONG3] float(53)  NULL,
  [PHUME] float(53)  NULL,
  [DUNGCU] float(53)  NULL,
  [CHAYNGOAI] float(53)  NULL,
  [MUCHUONG4] float(53)  NULL,
  [TENGAYME] float(53)  NULL,
  [MUCHUONG5] float(53)  NULL,
  [MUCHUONG6] float(53)  NULL,
  [KHOA] float(53)  NULL,
  [CSDB] float(53)  NULL,
  [LAZE] float(53)  NULL,
  [NAVILIASUENS1LO] float(53)  NULL,
  [PHUCAPTTPHAUTHUAT73] float(53)  NULL,
  [PHUCAPTTCHONGDAU73] float(53)  NULL,
  [TIENMAT] float(53)  NULL,
  [HESONGACH] float(53)  NULL,
  [HESOCOBAN] float(53)  NULL,
  [SOTIENTRUOCTHUE] float(53)  NULL,
  [CHUYENKHOAN] float(53)  NULL,
  [CHONCHUYENKHOAN] bit  NULL,
  [TRUKHAC] float(53)  NULL,
  [SOTIENLAMNGOAIGIO] float(53)  NULL,
  [SOTIENTRUCNGAYLE] float(53)  NULL,
  [HESONGACHBAC] float(53)  NULL,
  [TNCNTHANGTRUOC] float(53)  NULL,
  [QUYETTOANTNCN] float(53)  NULL,
  [NGOAIGIO] float(53)  NULL,
  [NGAY] smalldatetime  NULL,
  [HESO] float(53)  NULL,
  [HESOTINH] float(53)  NULL,
  [HESOCONGVIEC] float(53)  NULL,
  [CONLAI] float(53)  NULL,
  [MAPHONGX] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONGX] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRINHDO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIENNOISOICANTHIEP] float(53)  NULL,
  [DIEUCHINHGIAM] float(53)  NULL,
  [DIEUCHINHTANG] float(53)  NULL,
  [THUNHAPTRUOCTHUE] float(53)  NULL,
  [CHIPHIXANG] float(53)  NULL,
  [LUONGYCONGBSNTHOCVIEN] float(53)  NULL,
  [LUONGKHOAN] float(53)  NULL,
  [THUTHUAT] float(53)  NULL,
  [TONGTRUOCTHUE] float(53)  NULL,
  [TONGPHUCAP73] float(53)  NULL,
  [THUKY] float(53)  NULL,
  [TIEPDON] float(53)  NULL,
  [YCONG] float(53)  NULL,
  [PHUCAP] float(53) DEFAULT 0 NULL,
  [PHUCAPBENHVIEN] float(53)  NULL,
  [MUCHUONGBACSI] float(53) DEFAULT 0 NULL,
  [BACSI] float(53)  NULL,
  [MUCHUONGKTV] float(53)  NULL,
  [TONGSO2] float(53)  NULL,
  [KYTHUATVIEN] float(53)  NULL,
  [PHUCAPKTV] float(53)  NULL,
  [SOTIENPT73] float(53)  NULL,
  [NAMCONGTAC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAPKHOAKCBTYC] float(53)  NULL,
  [NOITRU] float(53)  NULL,
  [NGOAITRU] float(53)  NULL,
  [TIENTAP] float(53)  NULL,
  [LUONGHESO] float(53)  NULL,
  [LUONGNS] float(53)  NULL,
  [LUONGTRACHNHIEM] float(53)  NULL,
  [TONGPHAY] float(53)  NULL,
  [TIENPT73] float(53)  NULL,
  [TONGSONGUOI] float(53)  NULL,
  [LOAICANBO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHANTRAMHUONG] float(53)  NULL,
  [TONGBACSY] float(53)  NULL,
  [MUCCOBAN] float(53)  NULL,
  [TONGTIENTRUOCTHUE] float(53)  NULL,
  [DOANHTHU] float(53)  NULL,
  [KHAMHAMMAT] float(53)  NULL,
  [KHAM] float(53)  NULL,
  [VTTH] float(53)  NULL,
  [VATLIEUXUONG] float(53)  NULL,
  [DICHVU] float(53)  NULL,
  [DIEUDUONG] float(53)  NULL,
  [PHUKHAM] float(53)  NULL,
  [TIENGIUONG] float(53)  NULL,
  [HESOKHOAN] float(53)  NULL,
  [HESOGIUONG] float(53)  NULL,
  [HESONGAYCONGKHOAN] float(53)  NULL,
  [HESONGAYCONGGIUONG] float(53)  NULL,
  [HESOTINH1] float(53)  NULL,
  [TIENKHOAN] float(53)  NULL,
  [LUONGYCONG] float(53)  NULL,
  [HESOTINH2] float(53)  NULL,
  [TONGSO1] float(53)  NULL,
  [TYLEHUONG1] float(53)  NULL,
  [DOANHTHUKHAM] float(53)  NULL,
  [DIEUCHINH] float(53)  NULL,
  [DONVI] float(53)  NULL,
  [TIENCANTHIEP] float(53)  NULL,
  [TIENDIEUTRI] float(53)  NULL,
  [SOLUONG1] float(53) DEFAULT 0 NULL,
  [SOLUONG2] float(53) DEFAULT 0 NULL,
  [SOLUONG3] float(53) DEFAULT 0 NULL,
  [SOLUONG4] float(53) DEFAULT 0 NULL,
  [SOLUONG5] float(53) DEFAULT 0 NULL,
  [SOLUONG6] float(53) DEFAULT 0 NULL,
  [SOLUONG7] float(53) DEFAULT 0 NULL,
  [MUCHUONG7] float(53)  NULL,
  [HESOBAC] float(53)  NULL,
  [SOTIENNGHI] float(53)  NULL,
  [SOBUOINGHI] float(53)  NULL,
  [TIENSAUTRUNGHI] float(53)  NULL,
  [MUCCOBAN1] float(53)  NULL,
  [LOAIPHUCAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOM] float(53)  NULL,
  [DIEUCHINHDICHVU] float(53)  NULL,
  [THANHTIEN] float(53)  NULL,
  [DIEUCHINHTIENGIUONG] float(53)  NULL,
  [CHITIENMAT] float(53)  NULL,
  [DICHVUKHAC] float(53)  NULL,
  [TRUKHAC1] float(53)  NULL,
  [TRUKHAC2] float(53)  NULL,
  [TRUKHAC3] float(53)  NULL,
  [GAYME] float(53)  NULL,
  [KHAMME] float(53)  NULL,
  [PHUMECHINH] float(53)  NULL,
  [IDPHAUTHUAT] int  NULL,
  [DISOM6H15] float(53)  NULL,
  [SOTIENDISOM6H15] float(53)  NULL,
  [TONGTS] float(53)  NULL,
  [SONGAYNGHI] float(53)  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUKHAC4] float(53)  NULL,
  [TRUKHAC5] float(53)  NULL,
  [SOLUONG8] float(53)  NULL,
  [SOLUONG9] float(53)  NULL,
  [SOLUONG10] float(53)  NULL,
  [TIENTHAYBANG] float(53)  NULL,
  [TIENGIUONGBAT] float(53)  NULL,
  [THUKY1] float(53)  NULL,
  [MASOTHUETNCN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSO3] float(53)  NULL,
  [PHUCAPTHUTHUAT] float(53)  NULL,
  [SOLUONG11] float(53)  NULL,
  [SOLUONG12] float(53)  NULL,
  [TONGSO4] float(53)  NULL,
  [MUCHUONG8] float(53)  NULL,
  [MUCHUONG9] float(53)  NULL,
  [HE] float(53)  NULL,
  [TYLEHUONG2] float(53)  NULL,
  [TONGHUONG1] float(53)  NULL,
  [TAMUNG] float(53)  NULL,
  [DIEUCHINHSO1] float(53)  NULL,
  [DIEUCHINHSO2] float(53)  NULL,
  [DIEUCHINHSO3] float(53)  NULL,
  [DIEUCHINHTHUE] float(53)  NULL,
  [DIEUCHINHSO4] float(53)  NULL,
  [CHONTIENMAT] bit DEFAULT 0 NOT NULL,
  [LUONGTOITHIEU] float(53)  NULL,
  [ZZTONGHOCVIEN] float(53)  NULL,
  [MALUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOTINH3] float(53)  NULL,
  [HESOTINH4] float(53)  NULL,
  [HESOTINH5] float(53)  NULL,
  [HESOTINH6] float(53)  NULL,
  [HESOTINH7] float(53)  NULL,
  [HESOHOTRO] float(53)  NULL,
  [TRUKHAC6] float(53)  NULL,
  [HESO1] float(53)  NULL,
  [HESO2] float(53)  NULL,
  [HESO3] float(53)  NULL,
  [HESO4] float(53)  NULL,
  [HESO5] float(53)  NULL,
  [HESO6] float(53)  NULL,
  [HESO7] float(53)  NULL,
  [HESO8] float(53)  NULL,
  [HESO9] float(53)  NULL,
  [HESO10] float(53)  NULL,
  [HESOTINH8] float(53)  NULL,
  [HESOTINH9] float(53)  NULL,
  [HESOTINH10] float(53)  NULL,
  [HESOHUONG1] float(53)  NULL,
  [HESOHUONG2] float(53)  NULL,
  [HESOHUONG3] float(53)  NULL,
  [HESOHUONG4] float(53)  NULL,
  [HESOHUONG5] float(53)  NULL,
  [HESOHUONG6] float(53)  NULL,
  [HESOHUONG7] float(53)  NULL,
  [HESOHUONG8] float(53)  NULL,
  [HESOHUONG9] float(53)  NULL,
  [HESOHUONG10] float(53)  NULL,
  [TYLE] float(53)  NULL,
  [TYLE1] float(53)  NULL,
  [TYLE2] float(53)  NULL,
  [TYLE3] float(53)  NULL,
  [TYLE4] float(53)  NULL,
  [TYLETINH1] float(53)  NULL,
  [TYLETINH2] float(53)  NULL,
  [TYLETINH3] float(53)  NULL,
  [TYLETINH4] float(53)  NULL,
  [TYLETINH] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONGKHAC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONGKHAC_XULY
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONGKHAC_XULY]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONGKHAC_XULY]
GO

CREATE TABLE [dbo].[BANGLUONGKHAC_XULY] (
  [NGAYCT] smalldatetime  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [CHUCVU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [HESOLUONGCOBAN] float(53)  NULL,
  [LUONGCOBAN] float(53)  NULL,
  [NGAYTHUONG] float(53)  NULL,
  [NGAYNGHI] float(53)  NULL,
  [NGAYLE] float(53)  NULL,
  [LUONGNGAYTHUONG] float(53)  NULL,
  [LUONGNGAYNGHI] float(53)  NULL,
  [LUONGNGAYLE] float(53)  NULL,
  [TONGLUONGLINH] float(53)  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGANHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTA] int  NULL,
  [NOIDUNGTANGTHEM] nvarchar(180) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHONGDAU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUNHAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGCONGTRUC] float(53)  NULL,
  [TIENNGAYTRUC] float(53)  NULL,
  [SOLUONG] float(53) DEFAULT 0 NULL,
  [DONGIA] float(53)  NULL,
  [TUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [HESOPHUCAPVUOTKHUNG] float(53)  NULL,
  [HESOPHUCAPCHUCVU] float(53)  NULL,
  [TONGHESOLUONG] float(53)  NULL,
  [TENTHUTHUAT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSO] float(53)  NULL,
  [MUCHUONG] float(53)  NULL,
  [LOAITHUTHUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUOTDI] float(53)  NULL,
  [NOITHANH] float(53)  NULL,
  [NGOAITHANH] float(53)  NULL,
  [TINHXA] float(53)  NULL,
  [LUOTDINGOAITHANH] float(53)  NULL,
  [LUOTDITINHXA] float(53)  NULL,
  [SONGUOI] float(53)  NULL,
  [THANGKHAC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHUNHAP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGLUONGHESO] float(53)  NULL,
  [THEOPHONG] bit  NULL,
  [KHONGTHEOCONGTHUC] bit  NULL,
  [SELECTTED] bit  NULL,
  [TYLEHUONG] float(53)  NULL,
  [HESOSOLUONGMAU] float(53)  NULL,
  [HESOSOLUONGTINHNGUYEN] float(53)  NULL,
  [HESO250ML] float(53)  NULL,
  [HESOKTC] float(53)  NULL,
  [HESOMAUHUY] float(53)  NULL,
  [TONGHESOTHAMKHAO] float(53)  NULL,
  [SOLUONGNHANLUC] float(53)  NULL,
  [MUCKHOAN] float(53)  NULL,
  [TYLEMAUTHUGOM] float(53)  NULL,
  [TYLELAYMAUTN] float(53)  NULL,
  [SODONVI250] float(53)  NULL,
  [TYLEDIEUCHE] float(53)  NULL,
  [TYLEMAUHUY] float(53)  NULL,
  [CONGHESO] float(53)  NULL,
  [HESODIEUCHINH] float(53)  NULL,
  [HESOCHUNG] float(53)  NULL,
  [HOTENCON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGTINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUTNCN] float(53)  NULL,
  [HESOHUONG] float(53)  NULL,
  [LUONGPHUCAPTRACHNHIEM] float(53)  NULL,
  [TONGLUONGSAUTHUE] float(53)  NULL,
  [PHUCAPDIENTHOAI] float(53)  NULL,
  [DISOM6H45] float(53)  NULL,
  [DISOM6H30] float(53)  NULL,
  [SOTIENDISOM6H45] float(53)  NULL,
  [SOTIENDISOM6H30] float(53)  NULL,
  [TONGHUONG] float(53)  NULL,
  [SOTIENHUONG] float(53)  NULL,
  [TRUYLINH] float(53)  NULL,
  [TRUYTHU] float(53)  NULL,
  [HESOPHUCAPTHAMNIEN] float(53)  NULL,
  [TONGLUONGPHUCAP] float(53)  NULL,
  [HESOTHAMNIEN] float(53)  NULL,
  [HESOPHUCAPTRACHNHIEM] float(53)  NULL,
  [TONGHESOPHUCAP] float(53)  NULL,
  [TONGHESO] float(53)  NULL,
  [LOAIHD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTROTHAISAN] float(53)  NULL,
  [NGAYCONGHESO] float(53)  NULL,
  [NGAYCONGTHANG] float(53)  NULL,
  [NGAYCONGTHUCTE] float(53)  NULL,
  [TIENKYTHUATGMVACD] float(53)  NULL,
  [BACLUONG] float(53)  NULL,
  [PHAYBAC] float(53)  NULL,
  [LUONGPHAY] float(53)  NULL,
  [HESONGAYLUONG] float(53)  NULL,
  [MATHUNHAP] float(53)  NULL,
  [CHUYENKHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBENHNHAN] float(53)  NULL,
  [TUOI] float(53)  NULL,
  [MABENHAN] float(53)  NULL,
  [BAOHIEM] float(53)  NULL,
  [NGAYPHAUTHUAT] float(53)  NULL,
  [CACHTHUCPHAUTHUAT] float(53)  NULL,
  [TENDICHVU] float(53)  NULL,
  [MO] float(53)  NULL,
  [GIOMOCAPCUU] float(53)  NULL,
  [PHUONGPHAPVOCAM] float(53)  NULL,
  [LOAIDICHVU] float(53)  NULL,
  [MADONVI1] float(53)  NULL,
  [TENDONVI1] float(53)  NULL,
  [MUCHUONG1] float(53)  NULL,
  [MADONVI2] float(53)  NULL,
  [TENDONVI2] float(53)  NULL,
  [MUCHUONG2] float(53)  NULL,
  [MADONVI3] float(53)  NULL,
  [TENDONVI3] float(53)  NULL,
  [MUCHUONG3] float(53)  NULL,
  [PHUME] float(53)  NULL,
  [DUNGCU] float(53)  NULL,
  [CHAYNGOAI] float(53)  NULL,
  [MUCHUONG4] float(53)  NULL,
  [TENGAYME] float(53)  NULL,
  [MUCHUONG5] float(53)  NULL,
  [MUCHUONG6] float(53)  NULL,
  [KHOA] float(53)  NULL,
  [CSDB] float(53)  NULL,
  [LAZE] float(53)  NULL,
  [NAVILIASUENS1LO] float(53)  NULL,
  [PHUCAPTTPHAUTHUAT73] float(53)  NULL,
  [PHUCAPTTCHONGDAU73] float(53)  NULL,
  [TIENMAT] float(53)  NULL,
  [HESONGACH] float(53)  NULL,
  [HESOCOBAN] float(53)  NULL,
  [SOTIENTRUOCTHUE] float(53)  NULL,
  [CHUYENKHOAN] float(53)  NULL,
  [CHONCHUYENKHOAN] bit  NULL,
  [TRUKHAC] float(53)  NULL,
  [SOTIENLAMNGOAIGIO] float(53)  NULL,
  [SOTIENTRUCNGAYLE] float(53)  NULL,
  [HESONGACHBAC] float(53)  NULL,
  [TNCNTHANGTRUOC] float(53)  NULL,
  [QUYETTOANTNCN] float(53)  NULL,
  [NGOAIGIO] float(53)  NULL,
  [NGAY] smalldatetime  NULL,
  [HESO] float(53)  NULL,
  [HESOTINH] float(53)  NULL,
  [HESOCONGVIEC] float(53)  NULL,
  [CONLAI] float(53)  NULL,
  [MAPHONGX] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONGX] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRINHDO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIENNOISOICANTHIEP] float(53)  NULL,
  [DIEUCHINHGIAM] float(53)  NULL,
  [DIEUCHINHTANG] float(53)  NULL,
  [THUNHAPTRUOCTHUE] float(53)  NULL,
  [CHIPHIXANG] float(53)  NULL,
  [LUONGYCONGBSNTHOCVIEN] float(53)  NULL,
  [LUONGKHOAN] float(53)  NULL,
  [THUTHUAT] float(53)  NULL,
  [TONGTRUOCTHUE] float(53)  NULL,
  [TONGPHUCAP73] float(53)  NULL,
  [THUKY] float(53)  NULL,
  [TIEPDON] float(53)  NULL,
  [YCONG] float(53)  NULL,
  [PHUCAP] float(53) DEFAULT 0 NULL,
  [PHUCAPBENHVIEN] float(53)  NULL,
  [MUCHUONGBACSI] float(53) DEFAULT 0 NULL,
  [BACSI] float(53)  NULL,
  [MUCHUONGKTV] float(53)  NULL,
  [TONGSO2] float(53)  NULL,
  [KYTHUATVIEN] float(53)  NULL,
  [PHUCAPKTV] float(53)  NULL,
  [SOTIENPT73] float(53)  NULL,
  [NAMCONGTAC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAPKHOAKCBTYC] float(53)  NULL,
  [NOITRU] float(53)  NULL,
  [NGOAITRU] float(53)  NULL,
  [TIENTAP] float(53)  NULL,
  [LUONGHESO] float(53)  NULL,
  [LUONGNS] float(53)  NULL,
  [LUONGTRACHNHIEM] float(53)  NULL,
  [TONGPHAY] float(53)  NULL,
  [TIENPT73] float(53)  NULL,
  [TONGSONGUOI] float(53)  NULL,
  [LOAICANBO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHANTRAMHUONG] float(53)  NULL,
  [TONGBACSY] float(53)  NULL,
  [MUCCOBAN] float(53)  NULL,
  [TONGTIENTRUOCTHUE] float(53)  NULL,
  [DOANHTHU] float(53)  NULL,
  [KHAMHAMMAT] float(53)  NULL,
  [KHAM] float(53)  NULL,
  [VTTH] float(53)  NULL,
  [VATLIEUXUONG] float(53)  NULL,
  [DICHVU] float(53)  NULL,
  [DIEUDUONG] float(53)  NULL,
  [PHUKHAM] float(53)  NULL,
  [TIENGIUONG] float(53)  NULL,
  [HESOKHOAN] float(53)  NULL,
  [HESOGIUONG] float(53)  NULL,
  [HESONGAYCONGKHOAN] float(53)  NULL,
  [HESONGAYCONGGIUONG] float(53)  NULL,
  [HESOTINH1] float(53)  NULL,
  [TIENKHOAN] float(53)  NULL,
  [LUONGYCONG] float(53)  NULL,
  [HESOTINH2] float(53)  NULL,
  [TONGSO1] float(53)  NULL,
  [TYLEHUONG1] float(53)  NULL,
  [DOANHTHUKHAM] float(53)  NULL,
  [DIEUCHINH] float(53)  NULL,
  [DONVI] float(53)  NULL,
  [TIENCANTHIEP] float(53)  NULL,
  [TIENDIEUTRI] float(53)  NULL,
  [SOLUONG1] float(53) DEFAULT 0 NULL,
  [SOLUONG2] float(53) DEFAULT 0 NULL,
  [SOLUONG3] float(53) DEFAULT 0 NULL,
  [SOLUONG4] float(53) DEFAULT 0 NULL,
  [SOLUONG5] float(53) DEFAULT 0 NULL,
  [SOLUONG6] float(53) DEFAULT 0 NULL,
  [SOLUONG7] float(53) DEFAULT 0 NULL,
  [MUCHUONG7] float(53)  NULL,
  [HESOBAC] float(53)  NULL,
  [SOTIENNGHI] float(53)  NULL,
  [SOBUOINGHI] float(53)  NULL,
  [TIENSAUTRUNGHI] float(53)  NULL,
  [MUCCOBAN1] float(53)  NULL,
  [LOAIPHUCAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOM] float(53)  NULL,
  [DIEUCHINHDICHVU] float(53)  NULL,
  [THANHTIEN] float(53)  NULL,
  [DIEUCHINHTIENGIUONG] float(53)  NULL,
  [CHITIENMAT] float(53)  NULL,
  [DICHVUKHAC] float(53)  NULL,
  [TRUKHAC1] float(53)  NULL,
  [TRUKHAC2] float(53)  NULL,
  [TRUKHAC3] float(53)  NULL,
  [GAYME] float(53)  NULL,
  [KHAMME] float(53)  NULL,
  [PHUMECHINH] float(53)  NULL,
  [IDPHAUTHUAT] int  NULL,
  [DISOM6H15] float(53)  NULL,
  [SOTIENDISOM6H15] float(53)  NULL,
  [TONGTS] float(53)  NULL,
  [SONGAYNGHI] float(53)  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUKHAC4] float(53)  NULL,
  [TRUKHAC5] float(53)  NULL,
  [SOLUONG8] float(53)  NULL,
  [SOLUONG9] float(53)  NULL,
  [SOLUONG10] float(53)  NULL,
  [TIENTHAYBANG] float(53)  NULL,
  [TIENGIUONGBAT] float(53)  NULL,
  [THUKY1] float(53)  NULL,
  [MASOTHUETNCN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSO3] float(53)  NULL,
  [PHUCAPTHUTHUAT] float(53)  NULL,
  [SOLUONG11] float(53)  NULL,
  [SOLUONG12] float(53)  NULL,
  [TONGSO4] float(53)  NULL,
  [MUCHUONG8] float(53)  NULL,
  [MUCHUONG9] float(53)  NULL,
  [HE] float(53)  NULL,
  [TYLEHUONG2] float(53)  NULL,
  [TONGHUONG1] float(53)  NULL,
  [TAMUNG] float(53)  NULL,
  [DIEUCHINHSO1] float(53)  NULL,
  [DIEUCHINHSO2] float(53)  NULL,
  [DIEUCHINHSO3] float(53)  NULL,
  [DIEUCHINHTHUE] float(53)  NULL,
  [DIEUCHINHSO4] float(53)  NULL,
  [CHONTIENMAT] bit DEFAULT 0 NOT NULL,
  [LUONGTOITHIEU] float(53)  NULL,
  [ZZTONGHOCVIEN] float(53)  NULL,
  [MALUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOTINH3] float(53)  NULL,
  [HESOTINH4] float(53)  NULL,
  [HESOTINH5] float(53)  NULL,
  [HESOTINH6] float(53)  NULL,
  [HESOTINH7] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONGKHAC_XULY] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONGKHACLUU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONGKHACLUU]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONGKHACLUU]
GO

CREATE TABLE [dbo].[BANGLUONGKHACLUU] (
  [NGAYCT] smalldatetime  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [CHUCVU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [HESOLUONGCOBAN] float(53)  NULL,
  [LUONGCOBAN] float(53)  NULL,
  [NGAYTHUONG] float(53)  NULL,
  [NGAYNGHI] float(53)  NULL,
  [NGAYLE] float(53)  NULL,
  [LUONGNGAYTHUONG] float(53)  NULL,
  [LUONGNGAYNGHI] float(53)  NULL,
  [LUONGNGAYLE] float(53)  NULL,
  [TONGLUONGLINH] float(53)  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGANHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTA] int  NULL,
  [NOIDUNGTANGTHEM] nvarchar(180) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHONGDAU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUNHAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGCONGTRUC] float(53)  NULL,
  [TIENNGAYTRUC] float(53)  NULL,
  [SOLUONG] float(53) DEFAULT 0 NULL,
  [DONGIA] float(53)  NULL,
  [TUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [HESOPHUCAPVUOTKHUNG] float(53)  NULL,
  [HESOPHUCAPCHUCVU] float(53)  NULL,
  [TONGHESOLUONG] float(53)  NULL,
  [TENTHUTHUAT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSO] float(53)  NULL,
  [MUCHUONG] float(53)  NULL,
  [LOAITHUTHUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUOTDI] float(53)  NULL,
  [NOITHANH] float(53)  NULL,
  [NGOAITHANH] float(53)  NULL,
  [TINHXA] float(53)  NULL,
  [LUOTDINGOAITHANH] float(53)  NULL,
  [LUOTDITINHXA] float(53)  NULL,
  [SONGUOI] float(53)  NULL,
  [THANGKHAC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHUNHAP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGLUONGHESO] float(53)  NULL,
  [THEOPHONG] bit  NULL,
  [KHONGTHEOCONGTHUC] bit  NULL,
  [SELECTTED] bit  NULL,
  [TYLEHUONG] float(53)  NULL,
  [HESOSOLUONGMAU] float(53)  NULL,
  [HESOSOLUONGTINHNGUYEN] float(53)  NULL,
  [HESO250ML] float(53)  NULL,
  [HESOKTC] float(53)  NULL,
  [HESOMAUHUY] float(53)  NULL,
  [TONGHESOTHAMKHAO] float(53)  NULL,
  [SOLUONGNHANLUC] float(53)  NULL,
  [MUCKHOAN] float(53)  NULL,
  [TYLEMAUTHUGOM] float(53)  NULL,
  [TYLELAYMAUTN] float(53)  NULL,
  [SODONVI250] float(53)  NULL,
  [TYLEDIEUCHE] float(53)  NULL,
  [TYLEMAUHUY] float(53)  NULL,
  [CONGHESO] float(53)  NULL,
  [HESODIEUCHINH] float(53)  NULL,
  [HESOCHUNG] float(53)  NULL,
  [HOTENCON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGTINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUTNCN] float(53)  NULL,
  [HESOHUONG] float(53)  NULL,
  [LUONGPHUCAPTRACHNHIEM] float(53)  NULL,
  [TONGLUONGSAUTHUE] float(53)  NULL,
  [PHUCAPDIENTHOAI] float(53)  NULL,
  [DISOM6H45] float(53)  NULL,
  [DISOM6H30] float(53)  NULL,
  [SOTIENDISOM6H45] float(53)  NULL,
  [SOTIENDISOM6H30] float(53)  NULL,
  [TONGHUONG] float(53)  NULL,
  [SOTIENHUONG] float(53)  NULL,
  [TRUYLINH] float(53)  NULL,
  [TRUYTHU] float(53)  NULL,
  [HESOPHUCAPTHAMNIEN] float(53)  NULL,
  [TONGLUONGPHUCAP] float(53)  NULL,
  [HESOTHAMNIEN] float(53)  NULL,
  [HESOPHUCAPTRACHNHIEM] float(53)  NULL,
  [TONGHESOPHUCAP] float(53)  NULL,
  [TONGHESO] float(53)  NULL,
  [LOAIHD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTROTHAISAN] float(53)  NULL,
  [NGAYCONGHESO] float(53)  NULL,
  [NGAYCONGTHANG] float(53)  NULL,
  [NGAYCONGTHUCTE] float(53)  NULL,
  [TIENKYTHUATGMVACD] float(53)  NULL,
  [BACLUONG] float(53)  NULL,
  [PHAYBAC] float(53)  NULL,
  [LUONGPHAY] float(53)  NULL,
  [HESONGAYLUONG] float(53)  NULL,
  [MATHUNHAP] float(53)  NULL,
  [CHUYENKHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBENHNHAN] float(53)  NULL,
  [TUOI] float(53)  NULL,
  [MABENHAN] float(53)  NULL,
  [BAOHIEM] float(53)  NULL,
  [NGAYPHAUTHUAT] float(53)  NULL,
  [CACHTHUCPHAUTHUAT] float(53)  NULL,
  [TENDICHVU] float(53)  NULL,
  [MO] float(53)  NULL,
  [GIOMOCAPCUU] float(53)  NULL,
  [PHUONGPHAPVOCAM] float(53)  NULL,
  [LOAIDICHVU] float(53)  NULL,
  [MADONVI1] float(53)  NULL,
  [TENDONVI1] float(53)  NULL,
  [MUCHUONG1] float(53)  NULL,
  [MADONVI2] float(53)  NULL,
  [TENDONVI2] float(53)  NULL,
  [MUCHUONG2] float(53)  NULL,
  [MADONVI3] float(53)  NULL,
  [TENDONVI3] float(53)  NULL,
  [MUCHUONG3] float(53)  NULL,
  [PHUME] float(53)  NULL,
  [DUNGCU] float(53)  NULL,
  [CHAYNGOAI] float(53)  NULL,
  [MUCHUONG4] float(53)  NULL,
  [TENGAYME] float(53)  NULL,
  [MUCHUONG5] float(53)  NULL,
  [MUCHUONG6] float(53)  NULL,
  [KHOA] float(53)  NULL,
  [CSDB] float(53)  NULL,
  [LAZE] float(53)  NULL,
  [NAVILIASUENS1LO] float(53)  NULL,
  [PHUCAPTTPHAUTHUAT73] float(53)  NULL,
  [PHUCAPTTCHONGDAU73] float(53)  NULL,
  [TIENMAT] float(53)  NULL,
  [HESONGACH] float(53)  NULL,
  [HESOCOBAN] float(53)  NULL,
  [SOTIENTRUOCTHUE] float(53)  NULL,
  [CHUYENKHOAN] float(53)  NULL,
  [CHONCHUYENKHOAN] bit  NULL,
  [TRUKHAC] float(53)  NULL,
  [SOTIENLAMNGOAIGIO] float(53)  NULL,
  [SOTIENTRUCNGAYLE] float(53)  NULL,
  [HESONGACHBAC] float(53)  NULL,
  [TNCNTHANGTRUOC] float(53)  NULL,
  [QUYETTOANTNCN] float(53)  NULL,
  [NGOAIGIO] float(53)  NULL,
  [NGAY] smalldatetime  NULL,
  [HESO] float(53)  NULL,
  [HESOTINH] float(53)  NULL,
  [HESOCONGVIEC] float(53)  NULL,
  [CONLAI] float(53)  NULL,
  [MAPHONGX] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONGX] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRINHDO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIENNOISOICANTHIEP] float(53)  NULL,
  [DIEUCHINHGIAM] float(53)  NULL,
  [DIEUCHINHTANG] float(53)  NULL,
  [THUNHAPTRUOCTHUE] float(53)  NULL,
  [CHIPHIXANG] float(53)  NULL,
  [LUONGYCONGBSNTHOCVIEN] float(53)  NULL,
  [LUONGKHOAN] float(53)  NULL,
  [THUTHUAT] float(53)  NULL,
  [TONGTRUOCTHUE] float(53)  NULL,
  [TONGPHUCAP73] float(53)  NULL,
  [THUKY] float(53)  NULL,
  [TIEPDON] float(53)  NULL,
  [YCONG] float(53)  NULL,
  [PHUCAP] float(53) DEFAULT 0 NULL,
  [PHUCAPBENHVIEN] float(53)  NULL,
  [MUCHUONGBACSI] float(53) DEFAULT 0 NULL,
  [BACSI] float(53)  NULL,
  [MUCHUONGKTV] float(53)  NULL,
  [TONGSO2] float(53)  NULL,
  [KYTHUATVIEN] float(53)  NULL,
  [PHUCAPKTV] float(53)  NULL,
  [SOTIENPT73] float(53)  NULL,
  [NAMCONGTAC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAPKHOAKCBTYC] float(53)  NULL,
  [NOITRU] float(53)  NULL,
  [NGOAITRU] float(53)  NULL,
  [TIENTAP] float(53)  NULL,
  [LUONGHESO] float(53)  NULL,
  [LUONGNS] float(53)  NULL,
  [LUONGTRACHNHIEM] float(53)  NULL,
  [TONGPHAY] float(53)  NULL,
  [TIENPT73] float(53)  NULL,
  [TONGSONGUOI] float(53)  NULL,
  [LOAICANBO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHANTRAMHUONG] float(53)  NULL,
  [TONGBACSY] float(53)  NULL,
  [MUCCOBAN] float(53)  NULL,
  [TONGTIENTRUOCTHUE] float(53)  NULL,
  [DOANHTHU] float(53)  NULL,
  [KHAMHAMMAT] float(53)  NULL,
  [KHAM] float(53)  NULL,
  [VTTH] float(53)  NULL,
  [VATLIEUXUONG] float(53)  NULL,
  [DICHVU] float(53)  NULL,
  [DIEUDUONG] float(53)  NULL,
  [PHUKHAM] float(53)  NULL,
  [TIENGIUONG] float(53)  NULL,
  [HESOKHOAN] float(53)  NULL,
  [HESOGIUONG] float(53)  NULL,
  [HESONGAYCONGKHOAN] float(53)  NULL,
  [HESONGAYCONGGIUONG] float(53)  NULL,
  [HESOTINH1] float(53)  NULL,
  [TIENKHOAN] float(53)  NULL,
  [LUONGYCONG] float(53)  NULL,
  [HESOTINH2] float(53)  NULL,
  [TONGSO1] float(53)  NULL,
  [TYLEHUONG1] float(53)  NULL,
  [DOANHTHUKHAM] float(53)  NULL,
  [DIEUCHINH] float(53)  NULL,
  [DONVI] float(53)  NULL,
  [TIENCANTHIEP] float(53)  NULL,
  [TIENDIEUTRI] float(53)  NULL,
  [SOLUONG1] float(53) DEFAULT 0 NULL,
  [SOLUONG2] float(53) DEFAULT 0 NULL,
  [SOLUONG3] float(53) DEFAULT 0 NULL,
  [SOLUONG4] float(53) DEFAULT 0 NULL,
  [SOLUONG5] float(53) DEFAULT 0 NULL,
  [SOLUONG6] float(53) DEFAULT 0 NULL,
  [SOLUONG7] float(53) DEFAULT 0 NULL,
  [MUCHUONG7] float(53)  NULL,
  [HESOBAC] float(53)  NULL,
  [SOTIENNGHI] float(53)  NULL,
  [SOBUOINGHI] float(53)  NULL,
  [TIENSAUTRUNGHI] float(53)  NULL,
  [MUCCOBAN1] float(53)  NULL,
  [LOAIPHUCAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOM] float(53)  NULL,
  [DIEUCHINHDICHVU] float(53)  NULL,
  [THANHTIEN] float(53)  NULL,
  [DIEUCHINHTIENGIUONG] float(53)  NULL,
  [CHITIENMAT] float(53)  NULL,
  [DICHVUKHAC] float(53)  NULL,
  [TRUKHAC1] float(53)  NULL,
  [TRUKHAC2] float(53)  NULL,
  [TRUKHAC3] float(53)  NULL,
  [GAYME] float(53)  NULL,
  [KHAMME] float(53)  NULL,
  [PHUMECHINH] float(53)  NULL,
  [IDPHAUTHUAT] int  NULL,
  [DISOM6H15] float(53)  NULL,
  [SOTIENDISOM6H15] float(53)  NULL,
  [TONGTS] float(53)  NULL,
  [SONGAYNGHI] float(53)  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUKHAC4] float(53)  NULL,
  [TRUKHAC5] float(53)  NULL,
  [SOLUONG8] float(53)  NULL,
  [SOLUONG9] float(53)  NULL,
  [SOLUONG10] float(53)  NULL,
  [TIENTHAYBANG] float(53)  NULL,
  [TIENGIUONGBAT] float(53)  NULL,
  [THUKY1] float(53)  NULL,
  [MASOTHUETNCN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSO3] float(53)  NULL,
  [PHUCAPTHUTHUAT] float(53)  NULL,
  [SOLUONG11] float(53)  NULL,
  [SOLUONG12] float(53)  NULL,
  [TONGSO4] float(53)  NULL,
  [MUCHUONG8] float(53)  NULL,
  [MUCHUONG9] float(53)  NULL,
  [HE] float(53)  NULL,
  [TYLEHUONG2] float(53)  NULL,
  [TONGHUONG1] float(53)  NULL,
  [TAMUNG] float(53)  NULL,
  [DIEUCHINHSO1] float(53)  NULL,
  [DIEUCHINHSO2] float(53)  NULL,
  [DIEUCHINHSO3] float(53)  NULL,
  [DIEUCHINHTHUE] float(53)  NULL,
  [DIEUCHINHSO4] float(53)  NULL,
  [CHONTIENMAT] bit DEFAULT 0 NOT NULL,
  [LUONGTOITHIEU] float(53)  NULL,
  [ZZTONGHOCVIEN] float(53)  NULL,
  [MALUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOTINH3] float(53)  NULL,
  [HESOTINH4] float(53)  NULL,
  [HESOTINH5] float(53)  NULL,
  [HESOTINH6] float(53)  NULL,
  [HESOTINH7] float(53)  NULL,
  [HESOHOTRO] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONGKHACLUU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONGKHACTHANG12NAMTRUOC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONGKHACTHANG12NAMTRUOC]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONGKHACTHANG12NAMTRUOC]
GO

CREATE TABLE [dbo].[BANGLUONGKHACTHANG12NAMTRUOC] (
  [NGAYCT] smalldatetime  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [CHUCVU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [HESOLUONGCOBAN] float(53)  NULL,
  [LUONGCOBAN] float(53)  NULL,
  [NGAYTHUONG] float(53)  NULL,
  [NGAYNGHI] float(53)  NULL,
  [NGAYLE] float(53)  NULL,
  [LUONGNGAYTHUONG] float(53)  NULL,
  [LUONGNGAYNGHI] float(53)  NULL,
  [LUONGNGAYLE] float(53)  NULL,
  [TONGLUONGLINH] float(53)  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGANHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTA] int  NULL,
  [NOIDUNGTANGTHEM] nvarchar(180) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHONGDAU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUNHAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGCONGTRUC] float(53)  NULL,
  [TIENNGAYTRUC] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [TUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [HESOPHUCAPVUOTKHUNG] float(53)  NULL,
  [HESOPHUCAPCHUCVU] float(53)  NULL,
  [TONGHESOLUONG] float(53)  NULL,
  [TENTHUTHUAT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSO] float(53)  NULL,
  [MUCHUONG] float(53)  NULL,
  [LOAITHUTHUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUOTDI] float(53)  NULL,
  [NOITHANH] float(53)  NULL,
  [NGOAITHANH] float(53)  NULL,
  [TINHXA] float(53)  NULL,
  [LUOTDINGOAITHANH] float(53)  NULL,
  [LUOTDITINHXA] float(53)  NULL,
  [SONGUOI] float(53)  NULL,
  [THANGKHAC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHUNHAP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGLUONGHESO] float(53)  NULL,
  [THEOPHONG] bit  NULL,
  [KHONGTHEOCONGTHUC] bit  NULL,
  [SELECTTED] bit  NULL,
  [TYLEHUONG] float(53)  NULL,
  [HESOSOLUONGMAU] float(53)  NULL,
  [HESOSOLUONGTINHNGUYEN] float(53)  NULL,
  [HESO250ML] float(53)  NULL,
  [HESOKTC] float(53)  NULL,
  [HESOMAUHUY] float(53)  NULL,
  [TONGHESOTHAMKHAO] float(53)  NULL,
  [SOLUONGNHANLUC] float(53)  NULL,
  [MUCKHOAN] float(53)  NULL,
  [TYLEMAUTHUGOM] float(53)  NULL,
  [TYLELAYMAUTN] float(53)  NULL,
  [SODONVI250] float(53)  NULL,
  [TYLEDIEUCHE] float(53)  NULL,
  [TYLEMAUHUY] float(53)  NULL,
  [CONGHESO] float(53)  NULL,
  [HESODIEUCHINH] float(53)  NULL,
  [HESOCHUNG] float(53)  NULL,
  [HOTENCON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGTINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUTNCN] float(53)  NULL,
  [HESOHUONG] float(53)  NULL,
  [LUONGPHUCAPTRACHNHIEM] float(53)  NULL,
  [TONGLUONGSAUTHUE] float(53)  NULL,
  [PHUCAPDIENTHOAI] float(53)  NULL,
  [DISOM6H45] float(53)  NULL,
  [DISOM6H30] float(53)  NULL,
  [SOTIENDISOM6H45] float(53)  NULL,
  [SOTIENDISOM6H30] float(53)  NULL,
  [TONGHUONG] float(53)  NULL,
  [SOTIENHUONG] float(53)  NULL,
  [TRUYLINH] float(53)  NULL,
  [TRUYTHU] float(53)  NULL,
  [HESOPHUCAPTHAMNIEN] float(53)  NULL,
  [TONGLUONGPHUCAP] float(53)  NULL,
  [HESOTHAMNIEN] float(53)  NULL,
  [HESOPHUCAPTRACHNHIEM] float(53)  NULL,
  [TONGHESOPHUCAP] float(53)  NULL,
  [TONGHESO] float(53)  NULL,
  [LOAIHD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTROTHAISAN] float(53)  NULL,
  [NGAYCONGHESO] float(53)  NULL,
  [NGAYCONGTHANG] float(53)  NULL,
  [NGAYCONGTHUCTE] float(53)  NULL,
  [TIENKYTHUATGMVACD] float(53)  NULL,
  [BACLUONG] float(53)  NULL,
  [PHAYBAC] float(53)  NULL,
  [LUONGPHAY] float(53)  NULL,
  [HESONGAYLUONG] float(53)  NULL,
  [MATHUNHAP] float(53)  NULL,
  [CHUYENKHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBENHNHAN] float(53)  NULL,
  [TUOI] float(53)  NULL,
  [MABENHAN] float(53)  NULL,
  [BAOHIEM] float(53)  NULL,
  [NGAYPHAUTHUAT] float(53)  NULL,
  [CACHTHUCPHAUTHUAT] float(53)  NULL,
  [TENDICHVU] float(53)  NULL,
  [MO] float(53)  NULL,
  [GIOMOCAPCUU] float(53)  NULL,
  [PHUONGPHAPVOCAM] float(53)  NULL,
  [LOAIDICHVU] float(53)  NULL,
  [MADONVI1] float(53)  NULL,
  [TENDONVI1] float(53)  NULL,
  [MUCHUONG1] float(53)  NULL,
  [MADONVI2] float(53)  NULL,
  [TENDONVI2] float(53)  NULL,
  [MUCHUONG2] float(53)  NULL,
  [MADONVI3] float(53)  NULL,
  [TENDONVI3] float(53)  NULL,
  [MUCHUONG3] float(53)  NULL,
  [PHUME] float(53)  NULL,
  [DUNGCU] float(53)  NULL,
  [CHAYNGOAI] float(53)  NULL,
  [MUCHUONG4] float(53)  NULL,
  [TENGAYME] float(53)  NULL,
  [MUCHUONG5] float(53)  NULL,
  [MUCHUONG6] float(53)  NULL,
  [KHOA] float(53)  NULL,
  [CSDB] float(53)  NULL,
  [LAZE] float(53)  NULL,
  [NAVILIASUENS1LO] float(53)  NULL,
  [PHUCAPTTPHAUTHUAT73] float(53)  NULL,
  [PHUCAPTTCHONGDAU73] float(53)  NULL,
  [TIENMAT] float(53)  NULL,
  [HESONGACH] float(53)  NULL,
  [HESOCOBAN] float(53)  NULL,
  [SOTIENTRUOCTHUE] float(53)  NULL,
  [CHUYENKHOAN] float(53)  NULL,
  [CHONCHUYENKHOAN] bit  NULL,
  [TRUKHAC] float(53)  NULL,
  [SOTIENLAMNGOAIGIO] float(53)  NULL,
  [SOTIENTRUCNGAYLE] float(53)  NULL,
  [HESONGACHBAC] float(53)  NULL,
  [TNCNTHANGTRUOC] float(53)  NULL,
  [QUYETTOANTNCN] float(53)  NULL,
  [NGOAIGIO] float(53)  NULL,
  [NGAY] smalldatetime  NULL,
  [HESO] float(53)  NULL,
  [HESOTINH] float(53)  NULL,
  [HESOCONGVIEC] float(53)  NULL,
  [CONLAI] float(53)  NULL,
  [MAPHONGX] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONGX] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRINHDO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIENNOISOICANTHIEP] float(53)  NULL,
  [DIEUCHINHGIAM] float(53)  NULL,
  [DIEUCHINHTANG] float(53)  NULL,
  [THUNHAPTRUOCTHUE] float(53)  NULL,
  [CHIPHIXANG] float(53)  NULL,
  [LUONGYCONGBSNTHOCVIEN] float(53)  NULL,
  [LUONGKHOAN] float(53)  NULL,
  [THUTHUAT] float(53)  NULL,
  [TONGTRUOCTHUE] float(53)  NULL,
  [TONGPHUCAP73] float(53)  NULL,
  [THUKY] float(53)  NULL,
  [TIEPDON] float(53)  NULL,
  [YCONG] float(53)  NULL,
  [PHUCAP] float(53)  NULL,
  [PHUCAPBENHVIEN] float(53)  NULL,
  [MUCHUONGBACSI] float(53)  NULL,
  [BACSI] float(53)  NULL,
  [MUCHUONGKTV] float(53)  NULL,
  [TONGSO2] float(53)  NULL,
  [KYTHUATVIEN] float(53)  NULL,
  [PHUCAPKTV] float(53)  NULL,
  [SOTIENPT73] float(53)  NULL,
  [NAMCONGTAC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAPKHOAKCBTYC] float(53)  NULL,
  [NOITRU] float(53)  NULL,
  [NGOAITRU] float(53)  NULL,
  [TIENTAP] float(53)  NULL,
  [LUONGHESO] float(53)  NULL,
  [LUONGNS] float(53)  NULL,
  [LUONGTRACHNHIEM] float(53)  NULL,
  [TONGPHAY] float(53)  NULL,
  [TIENPT73] float(53)  NULL,
  [TONGSONGUOI] float(53)  NULL,
  [LOAICANBO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHANTRAMHUONG] float(53)  NULL,
  [TONGBACSY] float(53)  NULL,
  [MUCCOBAN] float(53)  NULL,
  [TONGTIENTRUOCTHUE] float(53)  NULL,
  [DOANHTHU] float(53)  NULL,
  [KHAMHAMMAT] float(53)  NULL,
  [KHAM] float(53)  NULL,
  [VTTH] float(53)  NULL,
  [VATLIEUXUONG] float(53)  NULL,
  [DICHVU] float(53)  NULL,
  [DIEUDUONG] float(53)  NULL,
  [PHUKHAM] float(53)  NULL,
  [TIENGIUONG] float(53)  NULL,
  [HESOKHOAN] float(53)  NULL,
  [HESOGIUONG] float(53)  NULL,
  [HESONGAYCONGKHOAN] float(53)  NULL,
  [HESONGAYCONGGIUONG] float(53)  NULL,
  [HESOTINH1] float(53)  NULL,
  [TIENKHOAN] float(53)  NULL,
  [LUONGYCONG] float(53)  NULL,
  [HESOTINH2] float(53)  NULL,
  [TONGSO1] float(53)  NULL,
  [TYLEHUONG1] float(53)  NULL,
  [DOANHTHUKHAM] float(53)  NULL,
  [DIEUCHINH] float(53)  NULL,
  [DONVI] float(53)  NULL,
  [TIENCANTHIEP] float(53)  NULL,
  [TIENDIEUTRI] float(53)  NULL,
  [SOLUONG1] float(53)  NULL,
  [SOLUONG2] float(53)  NULL,
  [SOLUONG3] float(53)  NULL,
  [SOLUONG4] float(53)  NULL,
  [SOLUONG5] float(53)  NULL,
  [SOLUONG6] float(53)  NULL,
  [SOLUONG7] float(53)  NULL,
  [MUCHUONG7] float(53)  NULL,
  [HESOBAC] float(53)  NULL,
  [SOTIENNGHI] float(53)  NULL,
  [SOBUOINGHI] float(53)  NULL,
  [TIENSAUTRUNGHI] float(53)  NULL,
  [MUCCOBAN1] float(53)  NULL,
  [LOAIPHUCAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOM] float(53)  NULL,
  [DIEUCHINHDICHVU] float(53)  NULL,
  [THANHTIEN] float(53)  NULL,
  [DIEUCHINHTIENGIUONG] float(53)  NULL,
  [CHITIENMAT] float(53)  NULL,
  [DICHVUKHAC] float(53)  NULL,
  [TRUKHAC1] float(53)  NULL,
  [TRUKHAC2] float(53)  NULL,
  [TRUKHAC3] float(53)  NULL,
  [GAYME] float(53)  NULL,
  [KHAMME] float(53)  NULL,
  [PHUMECHINH] float(53)  NULL,
  [IDPHAUTHUAT] int  NULL,
  [DISOM6H15] float(53)  NULL,
  [SOTIENDISOM6H15] float(53)  NULL,
  [TONGTS] float(53)  NULL,
  [SONGAYNGHI] float(53)  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUKHAC4] float(53)  NULL,
  [TRUKHAC5] float(53)  NULL,
  [SOLUONG8] float(53)  NULL,
  [SOLUONG9] float(53)  NULL,
  [SOLUONG10] float(53)  NULL,
  [TIENTHAYBANG] float(53)  NULL,
  [TIENGIUONGBAT] float(53)  NULL,
  [THUKY1] float(53)  NULL,
  [MASOTHUETNCN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSO3] float(53)  NULL,
  [PHUCAPTHUTHUAT] float(53)  NULL,
  [SOLUONG11] float(53)  NULL,
  [SOLUONG12] float(53)  NULL,
  [TONGSO4] float(53)  NULL,
  [MUCHUONG8] float(53)  NULL,
  [MUCHUONG9] float(53)  NULL,
  [HE] float(53)  NULL,
  [TYLEHUONG2] float(53)  NULL,
  [TONGHUONG1] float(53)  NULL,
  [TAMUNG] float(53)  NULL,
  [DIEUCHINHSO1] float(53)  NULL,
  [DIEUCHINHSO2] float(53)  NULL,
  [DIEUCHINHSO3] float(53)  NULL,
  [DIEUCHINHTHUE] float(53)  NULL,
  [DIEUCHINHSO4] float(53)  NULL,
  [CHONTIENMAT] bit  NOT NULL,
  [LUONGTOITHIEU] float(53)  NULL,
  [ZZTONGHOCVIEN] float(53)  NULL,
  [MALUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOTINH3] float(53)  NULL,
  [HESOTINH4] float(53)  NULL,
  [HESOTINH5] float(53)  NULL,
  [HESOTINH6] float(53)  NULL,
  [HESOTINH7] float(53)  NULL,
  [HESOHOTRO] float(53)  NULL,
  [TRUKHAC6] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONGKHACTHANG12NAMTRUOC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONGNEW
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONGNEW]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONGNEW]
GO

CREATE TABLE [dbo].[BANGLUONGNEW] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [LUONGCOBAN] float(53)  NULL,
  [TONGCONGTIENLUONG] float(53)  NULL,
  [TONGCONGTRU] float(53)  NULL,
  [CONLAI] float(53)  NULL,
  [TRUBHXH] float(53)  NULL,
  [TRUBHYT] float(53)  NULL,
  [TRUBHTN] float(53)  NULL,
  [TRUKPCD] float(53)  NULL,
  [TRUKHAC] float(53)  NULL,
  [HESOPHUCAPCHUCVU] float(53)  NULL,
  [HESOPHUCAPPHUCVU] float(53)  NULL,
  [HESOPHUCAPTRACHNHIEM] float(53)  NULL,
  [HESOPHUCAPTHAMNIEN] float(53)  NULL,
  [HESOPHUCAPVUOTKHUNG] float(53)  NULL,
  [HESOPHUCAPDOCHAI] float(53)  NULL,
  [HESOPHUCAPKHAC] float(53)  NULL,
  [LUONGPHUCAPCHUCVU] float(53)  NULL,
  [LUONGPHUCAPPHUCVU] float(53)  NULL,
  [LUONGPHUCAPTRACHNHIEM] float(53)  NULL,
  [LUONGPHUCAPTHAMNIEN] float(53)  NULL,
  [LUONGPHUCAPVUOTKHUNG] float(53)  NULL,
  [LUONGPHUCAPDOCHAI] float(53)  NULL,
  [LUONGPHUCAPKHAC] float(53)  NULL,
  [DIENTHOAIKHOAN] float(53)  NULL,
  [MANGACH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BACLUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGHESO] float(53)  NULL,
  [TRUYLINH] float(53)  NULL,
  [LUONGTOITHIEU] float(53)  NULL,
  [GIAMTRUBANTHAN] float(53)  NULL,
  [NGUOIPHUTHUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ANTRUA] float(53)  NULL,
  [NNGOAI] float(53)  NULL,
  [TAMUNG] float(53)  NULL,
  [HESOPHANTRAMVK] float(53)  NULL,
  [HESOPHANTRAMTNN] float(53)  NULL,
  [DIENTHOAI] float(53)  NULL,
  [THUETAMTINH] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONGNEW] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONGTANGTHEM
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONGTANGTHEM]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONGTANGTHEM]
GO

CREATE TABLE [dbo].[BANGLUONGTANGTHEM] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [HOTEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCVU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPB] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHONGBAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGACH] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [HESOLUONGCOBAN] float(53)  NULL,
  [LUONGCOBAN] float(53)  NULL,
  [HESOPHUCAPCHUCVU] float(53)  NULL,
  [LUONGPHUCAPCHUCVU] float(53)  NULL,
  [HESOPHUCAPDOCHAI] float(53)  NULL,
  [LUONGPHUCAPDOCHAI] float(53)  NULL,
  [HESOPHUCAPTRACHNHIEM] float(53)  NULL,
  [LUONGPHUCAPTRACHNHIEM] float(53)  NULL,
  [HESOPHUCAPDACTHU] float(53)  NULL,
  [LUONGPHUCAPDACTHU] float(53)  NULL,
  [BHXH] float(53)  NULL,
  [BHYT] float(53)  NULL,
  [TRUKHAC] float(53)  NULL,
  [CONGGIAMTRU] float(53)  NULL,
  [THUCLINH] float(53)  NULL,
  [TONGMUCLUONG] float(53)  NULL,
  [SOTIENBHXHTRATHAYLUONG] float(53)  NULL,
  [LUONGVABHXH] float(53)  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SONGAYNGHI] float(53)  NULL,
  [TYLE] float(53)  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUBHXH] float(53)  NULL,
  [TRUBHYT] float(53)  NULL,
  [HESOLUONG] float(53)  NULL,
  [PHUCAPTRACHNHIEM] float(53)  NULL,
  [PHUCAP] float(53)  NULL,
  [DIENTHOAI] float(53)  NULL,
  [TONGSO] float(53)  NULL,
  [TROCAPKHOKHAN] float(53)  NULL,
  [HESOPHUCAPPHUCVU] float(53)  NULL,
  [HESOPHUCAPTHAMNIEN] float(53)  NULL,
  [HESOPHUCAPKHAC] float(53)  NULL,
  [LUONGPHUCAPPHUCVU] float(53)  NULL,
  [LUONGPHUCAPTHAMNIEN] float(53)  NULL,
  [LUONGPHUCAPKHAC] float(53)  NULL,
  [TRUYLINH] float(53)  NULL,
  [DIENTHOAIKHOAN] float(53)  NULL,
  [LUONGKHOAN] float(53)  NULL,
  [MANGANHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTA] int  NULL,
  [TANGTHEM] float(53)  NULL,
  [ANTRUA] float(53)  NULL,
  [LANTANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESO] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [THANHTIEN] float(53)  NULL,
  [KHAUTRUTNCN] float(53)  NULL,
  [MASOTHUE] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDUNGCONGVIEC] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYOK] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDUNGTANGTHEM] nvarchar(180) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHONGDAU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] float(53)  NULL,
  [HINHTHUCANBO] float(53)  NULL,
  [BACLUONG] float(53)  NULL,
  [HESOPHUCAPVUOTKHUNG] float(53)  NULL,
  [HESOPHUCAP] float(53)  NULL,
  [TINHDOCHAI] float(53)  NULL,
  [CONGHESO1] float(53)  NULL,
  [TRUYLINHLUONG] float(53)  NULL,
  [LUONGPHUCAPVUOTKHUNG] float(53)  NULL,
  [LUONGPHUCAP] float(53)  NULL,
  [LUONGPHUCAPCONGVU] float(53)  NULL,
  [TONGLUONGVAPHUCAP] float(53)  NULL,
  [TRUBHXHHD] float(53)  NULL,
  [TRUBHYTHD] float(53)  NULL,
  [TRUBHTN] float(53)  NULL,
  [TRUTHUETNCN] float(53)  NULL,
  [TONGCONGTRU] float(53)  NULL,
  [CONLAI] float(53)  NULL,
  [LUONGHOPDONG] float(53)  NULL,
  [LUONGTOITHIEU] float(53)  NULL,
  [HINHTHUCCANBO] float(53)  NULL,
  [NHOMNHANVIEN] float(53)  NULL,
  [PHANTRAMTANGTHEM] float(53)  NULL,
  [SOTHANG] float(53)  NULL,
  [TONGSOLUONGTANGTHEM] float(53)  NULL,
  [LOAIHINHCANBO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHONGTHEOCONGTHUC] bit  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONGTANGTHEM] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONGTHANG12NAMTRUOC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONGTHANG12NAMTRUOC]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONGTHANG12NAMTRUOC]
GO

CREATE TABLE [dbo].[BANGLUONGTHANG12NAMTRUOC] (
  [NGAYCT] smalldatetime  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMNHANVIEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGACH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BACLUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [HESOPHUCAPVUOTKHUNG] float(53)  NULL,
  [HESOPHUCAPUUDAINGHE] float(53)  NULL,
  [HESOPHUCAPCHUCVU] float(53)  NULL,
  [HESOPHUCAPPHUCVU] float(53)  NULL,
  [HESOPHUCAPTRACHNHIEM] float(53)  NULL,
  [HESOPHUCAPTHAMNIEN] float(53)  NULL,
  [HESOPHUCAPDOCHAI] float(53)  NULL,
  [HESOPHUCAPKHAC] float(53)  NULL,
  [CONGHESO1] float(53)  NULL,
  [CONGHESO2] float(53)  NULL,
  [LUONGCOBAN] float(53)  NULL,
  [LUONGPHUCAPVUOTKHUNG] float(53)  NULL,
  [LUONGPHUCAPUUDAINGHE] float(53)  NULL,
  [LUONGPHUCAPCHUCVU] float(53)  NULL,
  [LUONGPHUCAPPHUCVU] float(53)  NULL,
  [LUONGPHUCAPTRACHNHIEM] float(53)  NULL,
  [LUONGPHUCAPTHAMNIEN] float(53)  NULL,
  [LUONGPHUCAPDOCHAI] float(53)  NULL,
  [LUONGPHUCAPKHAC] float(53)  NULL,
  [TONGCONGPHUCAP] float(53)  NULL,
  [TONGCONGTIENLUONG] float(53)  NULL,
  [TONGLUONGPHUCAP] float(53)  NULL,
  [TRUBHXH] float(53)  NULL,
  [TRUBHYT] float(53)  NULL,
  [TRUBHTN] float(53)  NULL,
  [TRUKPCD] float(53)  NULL,
  [TRUTHUETNCN] float(53)  NULL,
  [TRUKHAC1] float(53)  NULL,
  [TRUKHAC2] float(53)  NULL,
  [TRUKHAC3] float(53)  NULL,
  [DIENTHOAI] float(53)  NULL,
  [TONGCONGTRU] float(53)  NULL,
  [TROCAPKHOKHAN] float(53)  NULL,
  [TRUKHAC] float(53)  NULL,
  [DIENTHOAIKHOAN] float(53)  NULL,
  [BHCHIU] float(53)  NULL,
  [BANTHANCHIU] float(53)  NULL,
  [CONLAI] float(53)  NULL,
  [CHUCDANH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHONGBHXH] float(53)  NULL,
  [KHONGPCCONGVU] float(53)  NULL,
  [HESOPCTN] float(53)  NULL,
  [LUONGBHXH] float(53)  NULL,
  [HESOLUONGBHXH] float(53)  NULL,
  [LUONGNS] float(53)  NULL,
  [LUONGLP] float(53)  NULL,
  [THUNHAPTANGTHEM] float(53)  NULL,
  [THUNHAPUUDAINGHE] float(53)  NULL,
  [DIENGIAI] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUONGTOITHIEU] float(53)  NULL,
  [GIAMTRUBANTHAN] float(53)  NULL,
  [NGUOIPHUTHUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAU] float(53)  NULL,
  [SOCUOI] float(53)  NULL,
  [NGAYOK] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGHICT] bit  NULL,
  [SODUNG] float(53)  NULL,
  [TIENDIEN] float(53)  NULL,
  [TAMUNG] float(53)  NULL,
  [HOTROTN] float(53)  NULL,
  [LOAIHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHONGDAU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PC8] float(53)  NULL,
  [LUONGPHUCAPVKTHUCLINH] float(53)  NULL,
  [LUONGPHUCAPCVTHUCLINH] float(53)  NULL,
  [LUONGPHUCAPUDNTHUCLINH] float(53)  NULL,
  [LUONGPHUCAPDHTHUCLINH] float(53)  NULL,
  [TONGLUONGTHUCLINH2] float(53)  NULL,
  [KHOAMO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONGHIENTAI] float(53)  NULL,
  [KHONGTHEOCONGTHUC] bit  NULL,
  [SELECTTED] bit  NULL,
  [NGAYCONGTHUCTE] float(53)  NULL,
  [NGAYCONGTHANG] float(53)  NULL,
  [TYLEHUONG] float(53)  NULL,
  [HESOKPCD] float(53)  NULL,
  [TYLECONLAI] float(53)  NULL,
  [TIEUDETRUKHAC] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGLUONGKHOBAC] float(53)  NULL,
  [LISBANGLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRUTIENNGAYNGHI] float(53)  NULL,
  [BHXHTHAYLUONG] float(53)  NULL,
  [TRUYTHU] float(53)  NULL,
  [TRUYLINH] float(53)  NULL,
  [HINHTHUCLAODONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLEBH] float(53)  NULL,
  [TYLEBHXH] float(53)  NULL,
  [TYLEBHYT] float(53)  NULL,
  [TYLEBHTN] float(53)  NULL,
  [SOCONLAI] float(53)  NULL,
  [SONGAYNGHI] float(53)  NULL,
  [MAMOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUONGPHUCAPKHAC68] float(53)  NULL,
  [TYLEKPCD] float(53)  NULL,
  [TRUKHAC4] float(53)  NULL,
  [TRUKHAC5] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONGTHANG12NAMTRUOC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONGTHEMGIO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONGTHEMGIO]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONGTHEMGIO]
GO

CREATE TABLE [dbo].[BANGLUONGTHEMGIO] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [HESOPHUCAP] float(53)  NULL,
  [CHENHLECHBAOLUU] float(53)  NULL,
  [CONGHESO] float(53)  NULL,
  [TIENLUONGTHANG] float(53)  NULL,
  [MUCLUONGNGAY] float(53)  NULL,
  [MUCLUONGGIO] float(53)  NULL,
  [GIOLAMTHEM] float(53)  NULL,
  [TIENLAMTHEM] float(53)  NULL,
  [SOGIOT7] float(53)  NULL,
  [SOTIENT7] float(53)  NULL,
  [TONGCONG] float(53)  NULL,
  [STT] int  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONGTHEMGIO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGLUONGTRUYLINH
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGLUONGTRUYLINH]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGLUONGTRUYLINH]
GO

CREATE TABLE [dbo].[BANGLUONGTRUYLINH] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THOIGIAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [HESOLUONGMOI] float(53)  NULL,
  [HESOTANG] float(53)  NULL,
  [CONGHESO] float(53)  NULL,
  [BHYT] float(53)  NULL,
  [SOTHANG] float(53)  NULL,
  [CONLAI] float(53)  NULL,
  [THANHTIENTANG] float(53)  NULL,
  [BHXH] float(53)  NULL,
  [THANHTIEN] float(53)  NULL,
  [TONGCONG] float(53)  NULL,
  [STT] int  NULL,
  [PHUCAPCONGVU] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGLUONGTRUYLINH] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BANGTINHTHUETHUNHAP
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BANGTINHTHUETHUNHAP]') AND type IN ('U'))
	DROP TABLE [dbo].[BANGTINHTHUETHUNHAP]
GO

CREATE TABLE [dbo].[BANGTINHTHUETHUNHAP] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [THANG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUONG] float(53)  NULL,
  [LUONGKHONGCHIUTHUE] float(53)  NULL,
  [LUONGTHEMGIO] float(53)  NULL,
  [LUONGTANGTHEM] float(53)  NULL,
  [LUONGTRUC] float(53)  NULL,
  [LUONGPHAUTHUAT] float(53)  NULL,
  [LUONGPHAUTHUATDV] float(53)  NULL,
  [LUONGTHUONG] float(53)  NULL,
  [LUONGTHUONGKHAC] float(53)  NULL,
  [LUONGKHAC] float(53)  NULL,
  [BACTINHTHUE] float(53)  NULL,
  [TONGTHUNHAP] float(53)  NULL,
  [BANTHAN] float(53)  NULL,
  [SONGUOIPHUTHUOC] float(53)  NULL,
  [GIAMTRUGIACANH] float(53)  NULL,
  [TRUBHXH] float(53)  NULL,
  [TRUBHYT] float(53)  NULL,
  [TRUBHTN] float(53)  NULL,
  [UNGHO] float(53)  NULL,
  [TONGGIAMTRU] float(53)  NULL,
  [THUNHAPTINHTHUE] float(53)  NULL,
  [THUETNCN] float(53)  NULL,
  [NGAYCT] smalldatetime  NULL,
  [LOAICANBO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUONGTRUYLINH] float(53)  NULL,
  [ANTRUADILAI] float(53)  NULL,
  [LOAIHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGCHIUTHUE] float(53)  NULL,
  [LUONGDUAN] float(53)  NULL,
  [LUONGKHACTHUE] float(53)  NULL,
  [LUONGDUANTHUE] float(53)  NULL,
  [THUEDAKHAUTRU] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BANGTINHTHUETHUNHAP] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BAOCAO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BAOCAO]') AND type IN ('U'))
	DROP TABLE [dbo].[BAOCAO]
GO

CREATE TABLE [dbo].[BAOCAO] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SELECTTED] bit  NULL,
  [STT] int  NULL,
  [THEODONVI] bit  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBAOCAO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HIENTK] bit  NULL,
  [CHONTK] bit  NULL,
  [NGUON] bit  NULL,
  [CHITIET] bit  NULL,
  [MALOAIBAOCAO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THAMSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THAMSO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [REPORTFILES] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [REPORTFILES1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EXCELFILES] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [VATTU] bit  NULL,
  [LOAIBAOCAO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOHANG] bit  NULL,
  [TIENGANH] bit  NULL,
  [HOATDONG] bit  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOANMUC] bit  NULL,
  [CHONLOAIKHOAN] float(53)  NULL,
  [CHONDUAN] bit  NULL,
  [KHO] bit  NULL,
  [MUCLUC] bit  NULL,
  [KHOANTHU] bit  NULL,
  [KHOANCHI] bit  NULL,
  [PHONGBAN] bit  NULL,
  [HOPDONG] bit  NULL,
  [SAPXEP] bit  NULL,
  [HDSXKD] bit  NULL,
  [TRACUU] bit  NULL,
  [LIST] nvarchar(1220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTTONGHOP] nvarchar(1220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAEXCEL] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTROWSUM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTCOLSUM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LASTCOLUMN] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STARTROW] int  NULL,
  [PAGESETUP] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUDE1] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STORENAME] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTGROUP1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTDETAIL] nvarchar(1500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EXCELSUM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTROWSUM1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTCOLSUM1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LASTCOLUMN1] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTROWSUM2] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTCOLSUM2] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LASTCOLUMN2] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTROWSUM3] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTCOLSUM3] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LASTCOLUMN3] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTGROUP2] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTGROUP3] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTDETAIL1] nvarchar(1500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTDETAIL2] nvarchar(1500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTDETAIL3] nvarchar(1500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EXCELNEW] bit  NULL,
  [LISTSUBGROUP1] nvarchar(1000) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTFLDSUBGROUP1] nvarchar(1000) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EXCELNEW1] int  NULL,
  [CONGTRINH] bit  NULL,
  [TAISAN] bit  NULL,
  [LOPHOC] bit  NULL,
  [CHONNHOMDV] bit  NULL,
  [BAOCAONEW] bit  NULL,
  [MAMUCTIEU] bit  NULL,
  [CTGS] bit  NULL,
  [MANHOMVATTU] bit  NULL,
  [TAIKHOANDC] bit  NULL
)
GO

ALTER TABLE [dbo].[BAOCAO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BAOCAOBANGLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BAOCAOBANGLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[BAOCAOBANGLUONG]
GO

CREATE TABLE [dbo].[BAOCAOBANGLUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SELECTTED] bit  NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBAOCAO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THAMSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [REPORTFILES] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [REPORTFILES1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EXCELFILES] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIBAOCAO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIEUKIEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LIST] nvarchar(1220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUDE] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTTONGHOP] nvarchar(1220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BAOCAOBANGLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BAOCAOCHENHLECHQUY
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BAOCAOCHENHLECHQUY]') AND type IN ('U'))
	DROP TABLE [dbo].[BAOCAOCHENHLECHQUY]
GO

CREATE TABLE [dbo].[BAOCAOCHENHLECHQUY] (
  [LOAI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACHQUY1] float(53)  NULL,
  [DATHUCHIQUY1] float(53)  NULL,
  [PHAITHUCHIQUY1] float(53)  NULL,
  [CHENHLECHQUY1] float(53)  NULL,
  [KEHOACHQUY2] float(53)  NULL,
  [DATHUCHIQUY2] float(53)  NULL,
  [PHAITHUCHIQUY2] float(53)  NULL,
  [CHENHLECHQUY2] float(53)  NULL,
  [KEHOACHQUY3] float(53)  NULL,
  [DATHUCHIQUY3] float(53)  NULL,
  [PHAITHUCHIQUY3] float(53)  NULL,
  [CHENHLECHQUY3] float(53)  NULL,
  [KEHOACHQUY4] float(53)  NULL,
  [DATHUCHIQUY4] float(53)  NULL,
  [PHAITHUCHIQUY4] float(53)  NULL,
  [CHENHLECHQUY4] float(53)  NULL,
  [KEHOACHNAM] float(53)  NULL,
  [DATHUCHINAM] float(53)  NULL,
  [PHAITHUCHINAM] float(53)  NULL,
  [CHENHLECHNAM] float(53)  NULL,
  [FORMATTXT] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NAM] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BAOCAOCHENHLECHQUY] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BAOCAOEXCEL
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BAOCAOEXCEL]') AND type IN ('U'))
	DROP TABLE [dbo].[BAOCAOEXCEL]
GO

CREATE TABLE [dbo].[BAOCAOEXCEL] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUDE1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STORENAME] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTGROUP1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTDETAIL] nvarchar(1500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LASTCOLUMN] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTCOLSUM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STARTROW] int  NULL,
  [PAGESETUP] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTROWSUM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THAMSO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUDE2] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUDE3] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUDE4] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTGROUP2] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTGROUP3] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTGROUP4] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTSUBGROUP] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTSODUDAU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTSODUCUOI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTTONGCONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTLUYKENAM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTLUYKEKHOIDAU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LITSPARAMETES] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THAMSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BAOCAOEXCEL] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BAOCAOKINHPHINAMTRUOC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BAOCAOKINHPHINAMTRUOC]') AND type IN ('U'))
	DROP TABLE [dbo].[BAOCAOKINHPHINAMTRUOC]
GO

CREATE TABLE [dbo].[BAOCAOKINHPHINAMTRUOC] (
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUCTINH] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] float(53)  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BAOCAOKINHPHINAMTRUOC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BAOCAOPHIEUTHUCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BAOCAOPHIEUTHUCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[BAOCAOPHIEUTHUCHI]
GO

CREATE TABLE [dbo].[BAOCAOPHIEUTHUCHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SELECTTED] bit  NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBAOCAO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [REPORTFILES] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EXCELFILES] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THAMSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BAOCAOPHIEUTHUCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BAOCAOTAICHINH
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BAOCAOTAICHINH]') AND type IN ('U'))
	DROP TABLE [dbo].[BAOCAOTAICHINH]
GO

CREATE TABLE [dbo].[BAOCAOTAICHINH] (
  [ABC] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAISO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTED] bit DEFAULT 0 NULL,
  [ID] float(53)  NULL,
  [REPORTFIELS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EXCELFILES] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [REPORTEXCELOK] int  NULL
)
GO

ALTER TABLE [dbo].[BAOCAOTAICHINH] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BAOCAOWEB
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BAOCAOWEB]') AND type IN ('U'))
	DROP TABLE [dbo].[BAOCAOWEB]
GO

CREATE TABLE [dbo].[BAOCAOWEB] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [TENBAOCAO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAUBAOCAO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOBAOCAO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BAOCAOWEB] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BIENLAITHUTIEN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BIENLAITHUTIEN]') AND type IN ('U'))
	DROP TABLE [dbo].[BIENLAITHUTIEN]
GO

CREATE TABLE [dbo].[BIENLAITHUTIEN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCT] smalldatetime  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOSO] nvarchar(650) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [CHONIN] bit  NULL,
  [CHUNGTUTHUCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HINHTHUCTT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL
)
GO

ALTER TABLE [dbo].[BIENLAITHUTIEN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CACCHITIEUDUTOAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CACCHITIEUDUTOAN]') AND type IN ('U'))
	DROP TABLE [dbo].[CACCHITIEUDUTOAN]
GO

CREATE TABLE [dbo].[CACCHITIEUDUTOAN] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[CACCHITIEUDUTOAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CHENHLECHTHUCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CHENHLECHTHUCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[CHENHLECHTHUCHI]
GO

CREATE TABLE [dbo].[CHENHLECHTHUCHI] (
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[CHENHLECHTHUCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CHITIETNHOMHANG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CHITIETNHOMHANG]') AND type IN ('U'))
	DROP TABLE [dbo].[CHITIETNHOMHANG]
GO

CREATE TABLE [dbo].[CHITIETNHOMHANG] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLEPHANBO] float(53)  NULL
)
GO

ALTER TABLE [dbo].[CHITIETNHOMHANG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CHITIETTAISANCODINH
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CHITIETTAISANCODINH]') AND type IN ('U'))
	DROP TABLE [dbo].[CHITIETTAISANCODINH]
GO

CREATE TABLE [dbo].[CHITIETTAISANCODINH] (
  [MAHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIQUANLY] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DANHMUC] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [THOIHAN] int  NULL,
  [SOTHANGTANG] int  NULL,
  [SOTHANGGIAM] int  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMUCDICH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUCDICHSUDUNG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITAISAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NAMSANXUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NUOCSANXUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NAMSUDUNG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DACDIEM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUYENGIADAUNAM] float(53)  NULL,
  [NGUYENGIATANG] float(53)  NULL,
  [NGUYENGIAGIAM] float(53)  NULL,
  [LYDOTANGNGUYENGIA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALYDOTANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALYDOGIAM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALYDOKHAUHAOTANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALYDOKHAUHAOGIAM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LYDOGIAMNGUYENGIA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUYENGIACUOINAM] float(53)  NULL,
  [TYLEKHAUHAO] float(53)  NULL,
  [KHAUHAOCOBANNAM] float(53)  NULL,
  [KHAUHAOCOBANTANG] float(53)  NULL,
  [MUCTRICH] float(53)  NULL,
  [KHAUHAOCOBANGIAM] float(53)  NULL,
  [LYDOTANGKHAUHAO] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LYDOGIAMKHAUHAO] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHAUHAOCOBANCUOINAM] float(53)  NULL,
  [GIATRICONLAIDAUNAM] float(53)  NULL,
  [GIATRICONLAITANG] float(53)  NULL,
  [GIATRICONLAIGIAM] float(53)  NULL,
  [LYDOGIAMCONLAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LYDOTANGCONLAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIATRICONLAICUOINAM] float(53)  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUTSCD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUY] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[CHITIETTAISANCODINH] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CHITIETTHANHTOANHOPDONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CHITIETTHANHTOANHOPDONG]') AND type IN ('U'))
	DROP TABLE [dbo].[CHITIETTHANHTOANHOPDONG]
GO

CREATE TABLE [dbo].[CHITIETTHANHTOANHOPDONG] (
  [STT] int  NULL,
  [MAHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENDATHANHTOANUSD] float(53)  NULL,
  [SOTIENDATHANHTOAN] float(53)  NULL
)
GO

ALTER TABLE [dbo].[CHITIETTHANHTOANHOPDONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CHITIEUKHOANCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CHITIEUKHOANCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[CHITIEUKHOANCHI]
GO

CREATE TABLE [dbo].[CHITIEUKHOANCHI] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[CHITIEUKHOANCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CHITIEUQUYETTOAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CHITIEUQUYETTOAN]') AND type IN ('U'))
	DROP TABLE [dbo].[CHITIEUQUYETTOAN]
GO

CREATE TABLE [dbo].[CHITIEUQUYETTOAN] (
  [ID] float(53)  NULL,
  [MASO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUCTINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[CHITIEUQUYETTOAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CHITIEUTIETKIEMKHOANCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CHITIEUTIETKIEMKHOANCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[CHITIEUTIETKIEMKHOANCHI]
GO

CREATE TABLE [dbo].[CHITIEUTIETKIEMKHOANCHI] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[CHITIEUTIETKIEMKHOANCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CHONNHOMVATTU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CHONNHOMVATTU]') AND type IN ('U'))
	DROP TABLE [dbo].[CHONNHOMVATTU]
GO

CREATE TABLE [dbo].[CHONNHOMVATTU] (
  [STT] int  NULL,
  [TENNHOM] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL
)
GO

ALTER TABLE [dbo].[CHONNHOMVATTU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CHUONGLOAIKHOAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CHUONGLOAIKHOAN]') AND type IN ('U'))
	DROP TABLE [dbo].[CHUONGLOAIKHOAN]
GO

CREATE TABLE [dbo].[CHUONGLOAIKHOAN] (
  [CHUONGLOAIKHOAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [CHUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKHOAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOBAC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEU] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [BATBUOC] bit  NULL,
  [MAQUANLY] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENQUANLY] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DUAN] bit  NULL,
  [MATINHCHATNGUON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONIN] bit  NULL
)
GO

ALTER TABLE [dbo].[CHUONGLOAIKHOAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for CONGTHUCBANGLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CONGTHUCBANGLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[CONGTHUCBANGLUONG]
GO

CREATE TABLE [dbo].[CONGTHUCBANGLUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [SELECTTED] bit  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE Finnish_Swedish_CI_AS  NULL,
  [CONGTHUC] nvarchar(350) COLLATE Finnish_Swedish_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[CONGTHUCBANGLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for COQUANNOPTHUE
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[COQUANNOPTHUE]') AND type IN ('U'))
	DROP TABLE [dbo].[COQUANNOPTHUE]
GO

CREATE TABLE [dbo].[COQUANNOPTHUE] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] smallint  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACHUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOBAC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANDKT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [COQUAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[COQUANNOPTHUE] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for COQUANSUDUNG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[COQUANSUDUNG]') AND type IN ('U'))
	DROP TABLE [dbo].[COQUANSUDUNG]
GO

CREATE TABLE [dbo].[COQUANSUDUNG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [HOTEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPTREN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANHTHUTRUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENTHUTRUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANHKETOAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANHLAPBIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTHANGLB] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENNGUOILAPBIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENKETOANTRUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOBAC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACHUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGANSACH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENTHUQUY] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHTHANHPHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUANHUYEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHUKHO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FAX] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENTHOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[COQUANSUDUNG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for COQUANSUDUNGEN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[COQUANSUDUNGEN]') AND type IN ('U'))
	DROP TABLE [dbo].[COQUANSUDUNGEN]
GO

CREATE TABLE [dbo].[COQUANSUDUNGEN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [HOTEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPTREN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANHTHUTRUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENTHUTRUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANHKETOAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANHLAPBIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTHANGLB] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENNGUOILAPBIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENKETOANTRUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOBAC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACHUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGANSACH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENTHUQUY] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHTHANHPHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUANHUYEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHUKHO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FAX] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENTHOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[COQUANSUDUNGEN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCAUTODINHKHOAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCAUTODINHKHOAN]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCAUTODINHKHOAN]
GO

CREATE TABLE [dbo].[DANHMUCAUTODINHKHOAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [TKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALYDO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENLYDO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HINHTHUCTT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHDSXKD] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCAUTODINHKHOAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCAUTOTKNGOAIBANG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCAUTOTKNGOAIBANG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCAUTOTKNGOAIBANG]
GO

CREATE TABLE [dbo].[DANHMUCAUTOTKNGOAIBANG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAINO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVICAPDUOI] int  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCAUTOTKNGOAIBANG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCCHIENLUOC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCCHIENLUOC]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCCHIENLUOC]
GO

CREATE TABLE [dbo].[DANHMUCCHIENLUOC] (
  [STT] int  NULL,
  [MACHIENLUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENCHIENLUOC] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [BATBUOC] bit  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCCHIENLUOC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCCHONIN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCCHONIN]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCCHONIN]
GO

CREATE TABLE [dbo].[DANHMUCCHONIN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [IDDONVICAPDUOI] int  NULL,
  [MASO] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTEN] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCCHONIN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCCHUCDANH
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCCHUCDANH]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCCHUCDANH]
GO

CREATE TABLE [dbo].[DANHMUCCHUCDANH] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MACHUCDANH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCHUCDANH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCCHUCDANH] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCCHUONGTRINHMUCTIEU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCCHUONGTRINHMUCTIEU]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCCHUONGTRINHMUCTIEU]
GO

CREATE TABLE [dbo].[DANHMUCCHUONGTRINHMUCTIEU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAMUCTIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGUOI] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [SOTIENTRUOC] float(53)  NULL,
  [SODUTAMUNG] float(53)  NULL,
  [SOTIENDIEUCHINH] float(53)  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHIEMVU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATDAU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETTHUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODUDUTOAN] float(53)  NULL,
  [LUYKEDUTOANTW] float(53)  NULL,
  [LUYKEDUTOANDP] float(53)  NULL,
  [LUYKEQUYETTOANDP] float(53)  NULL,
  [LUYKEQUYETTOANTW] float(53)  NULL,
  [HOTEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUNHIEM] nvarchar(130) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCCHUONGTRINHMUCTIEU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCDONVITHUCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCDONVITHUCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCDONVITHUCHI]
GO

CREATE TABLE [dbo].[DANHMUCDONVITHUCHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTAIKHOAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCDONVITHUCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCDONVITRUCTHUOC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCDONVITRUCTHUOC]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCDONVITRUCTHUOC]
GO

CREATE TABLE [dbo].[DANHMUCDONVITRUCTHUOC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHEDOKETOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCDONVITRUCTHUOC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCGIAODICH
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCGIAODICH]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCGIAODICH]
GO

CREATE TABLE [dbo].[DANHMUCGIAODICH] (
  [STT] int  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCGIAODICH] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCGIAVON
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCGIAVON]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCGIAVON]
GO

CREATE TABLE [dbo].[DANHMUCGIAVON] (
  [STT] int  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[DANHMUCGIAVON] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHACHTOANLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHACHTOANLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHACHTOANLUONG]
GO

CREATE TABLE [dbo].[DANHMUCHACHTOANLUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [NHOMHACHTOAN] int  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGANHANG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOATDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [DIEUKIENHACHTOAN] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCOT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED1] bit  NULL,
  [LOAIOK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHACHTOANLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHANGHOA
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHANGHOA]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHANGHOA]
GO

CREATE TABLE [dbo].[DANHMUCHANGHOA] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENHANGHOA] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(40) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIANHAP] float(53)  NULL,
  [GIAXUAT] float(53)  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [INSTOCK] bit DEFAULT 0 NULL,
  [TYLEVAT] float(53)  NULL,
  [SOLUONGTON] float(53)  NULL,
  [SHTK] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIATHANH] bit DEFAULT 0 NULL,
  [XUATXU] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUYCACH] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGQUYDOI] float(53)  NULL,
  [SOLUONGTONOK] float(53)  NULL,
  [HANSUDUNG] smalldatetime  NULL,
  [CHONIN] bit  NULL,
  [MANHACUNGCAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHACUNGCAP] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIAA] float(53)  NULL,
  [SOLUONGA] float(53)  NULL,
  [COSOSANXUAT] nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NUOCSANXUAT] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGSO] float(53)  NULL,
  [GIABANLE] float(53)  NULL,
  [TENHANGHOAGOC] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKDOANHTHU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [APCONGTHUC] bit  NULL,
  [DONGIAX] float(53)  NULL,
  [LOHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACACHDUNG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCACHDUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHTRANG] bit  NULL,
  [TRANGTHAI] bit  NULL,
  [DACAPNHAT] bit  NULL,
  [MANUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADVT] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SLTON1] float(53)  NULL,
  [SLTON2] float(53)  NULL,
  [SLTON3] float(53)  NULL,
  [SLTON4] float(53)  NULL,
  [SLTON5] float(53)  NULL,
  [SLTON6] float(53)  NULL,
  [SLTON7] float(53)  NULL,
  [SLTON8] float(53)  NULL,
  [SLTON9] float(53)  NULL,
  [SLTON10] float(53)  NULL,
  [AMAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ATENKHO] nvarchar(130) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SLTON37] float(53)  NULL,
  [SLTON38] float(53)  NULL,
  [SLTON11] float(53)  NULL,
  [SLTON12] float(53)  NULL,
  [SLTON13] float(53)  NULL,
  [SLTON14] float(53)  NULL,
  [SLTON15] float(53)  NULL,
  [SLTON16] float(53)  NULL,
  [SLTON17] float(53)  NULL,
  [SLTON18] float(53)  NULL,
  [SLTON19] float(53)  NULL,
  [SLTON20] float(53)  NULL,
  [SLTON21] float(53)  NULL,
  [SLTON22] float(53)  NULL,
  [SLTON23] float(53)  NULL,
  [SLTON24] float(53)  NULL,
  [SLTON25] float(53)  NULL,
  [SLTON26] float(53)  NULL,
  [SLTON27] float(53)  NULL,
  [SLTON28] float(53)  NULL,
  [SLTON29] float(53)  NULL,
  [SLTON30] float(53)  NULL,
  [SLTON31] float(53)  NULL,
  [SLTON32] float(53)  NULL,
  [SLTON33] float(53)  NULL,
  [SLTON34] float(53)  NULL,
  [SLTON35] float(53)  NULL,
  [SLTON36] float(53)  NULL,
  [SLTON39] float(53)  NULL,
  [SLTON40] float(53)  NULL,
  [SOVISA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SLTON41] float(53)  NULL,
  [SLTON42] float(53)  NULL,
  [SLTON43] float(53)  NULL,
  [SLTON44] float(53)  NULL,
  [SLTON45] float(53)  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGTHAU] float(53)  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NONGDO] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOXACNHANVT1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOXACNHANVT2] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALOAI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENLOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SLTON46] float(53)  NULL,
  [IDDONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOPDONG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIAMUAHANG] float(53)  NULL,
  [GIACHIPHI] float(53)  NULL,
  [SOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHIEUDATHANG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOQUYETDINH] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GOITHAU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SLTON47] float(53)  NULL,
  [HINHTHUCMUASAM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAQUOCGIA] nvarchar(80) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAQUANLY] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHANGHOA] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHANGHOAABC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHANGHOAABC]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHANGHOAABC]
GO

CREATE TABLE [dbo].[DANHMUCHANGHOAABC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(40) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIANHAP] float(53)  NULL,
  [GIAXUAT] float(53)  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [INSTOCK] bit DEFAULT 0 NULL,
  [TYLEVAT] float(53)  NULL,
  [SOLUONGTON] float(53)  NULL,
  [SHTK] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIATHANH] bit DEFAULT 0 NULL,
  [XUATXU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUYCACH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGQUYDOI] float(53)  NULL,
  [SOLUONGTONOK] float(53)  NULL,
  [HANSUDUNG] smalldatetime  NULL,
  [CHONIN] bit  NULL,
  [MANHACUNGCAP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHACUNGCAP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIAA] float(53)  NULL,
  [SOLUONGA] float(53)  NULL,
  [COSOSANXUAT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NUOCSANXUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGSO] float(53)  NULL,
  [GIABANLE] float(53)  NULL,
  [TENHANGHOAGOC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKDOANHTHU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [APCONGTHUC] bit  NULL,
  [DONGIAX] float(53)  NULL,
  [LOHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACACHDUNG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCACHDUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHTRANG] bit  NULL,
  [TRANGTHAI] bit  NULL,
  [DACAPNHAT] bit  NULL,
  [MANUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADVT] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SLTON1] float(53)  NULL,
  [SLTON2] float(53)  NULL,
  [SLTON3] float(53)  NULL,
  [SLTON4] float(53)  NULL,
  [SLTON5] float(53)  NULL,
  [SLTON6] float(53)  NULL,
  [SLTON7] float(53)  NULL,
  [SLTON8] float(53)  NULL,
  [SLTON9] float(53)  NULL,
  [SLTON10] float(53)  NULL,
  [AMAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ATENKHO] nvarchar(130) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SLTON37] float(53)  NULL,
  [SLTON38] float(53)  NULL,
  [SLTON11] float(53)  NULL,
  [SLTON12] float(53)  NULL,
  [SLTON13] float(53)  NULL,
  [SLTON14] float(53)  NULL,
  [SLTON15] float(53)  NULL,
  [SLTON16] float(53)  NULL,
  [SLTON17] float(53)  NULL,
  [SLTON18] float(53)  NULL,
  [SLTON19] float(53)  NULL,
  [SLTON20] float(53)  NULL,
  [SLTON21] float(53)  NULL,
  [SLTON22] float(53)  NULL,
  [SLTON23] float(53)  NULL,
  [SLTON24] float(53)  NULL,
  [SLTON25] float(53)  NULL,
  [SLTON26] float(53)  NULL,
  [SLTON27] float(53)  NULL,
  [SLTON28] float(53)  NULL,
  [SLTON29] float(53)  NULL,
  [SLTON30] float(53)  NULL,
  [SLTON31] float(53)  NULL,
  [SLTON32] float(53)  NULL,
  [SLTON33] float(53)  NULL,
  [SLTON34] float(53)  NULL,
  [SLTON35] float(53)  NULL,
  [SLTON36] float(53)  NULL,
  [SLTON39] float(53)  NULL,
  [SLTON40] float(53)  NULL,
  [SOVISA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SLTON41] float(53)  NULL,
  [SLTON42] float(53)  NULL,
  [SLTON43] float(53)  NULL,
  [SLTON44] float(53)  NULL,
  [SLTON45] float(53)  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGTHAU] float(53)  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NONGDO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHANGHOAABC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHANGHOAOLD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHANGHOAOLD]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHANGHOAOLD]
GO

CREATE TABLE [dbo].[DANHMUCHANGHOAOLD] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENHANGHOA] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIANHAP] float(53)  NULL,
  [GIAXUAT] float(53)  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [INSTOCK] bit  NULL,
  [TYLEVAT] float(53)  NULL,
  [SOLUONGTON] float(53)  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIATHANH] bit  NULL,
  [XUATXU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUYCACH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGQUYDOI] float(53)  NULL,
  [SOLUONGTONOK] float(53)  NULL,
  [HANSUDUNG] smalldatetime  NULL,
  [CHONIN] bit  NULL,
  [MANHACUNGCAP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHACUNGCAP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIAA] float(53)  NULL,
  [SOLUONGA] float(53)  NULL,
  [MASOHANGHOA] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALOAI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENLOAI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMAU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMAU] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHCHAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHUONGTHUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGHOAOK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KIEU] float(53)  NULL,
  [KHORONG] float(53)  NULL,
  [COSOSANXUAT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NUOCSANXUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGSO] float(53)  NULL,
  [AMAKHO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ATENKHO] nvarchar(130) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SLTON37] float(53)  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHANGHOAOLD] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHESOLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHESOLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHESOLUONG]
GO

CREATE TABLE [dbo].[DANHMUCHESOLUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [SELECTTED] bit  NULL,
  [HESOLUONG] float(53)  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHESOLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHINHTHUCTT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHINHTHUCTT]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHINHTHUCTT]
GO

CREATE TABLE [dbo].[DANHMUCHINHTHUCTT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHINHTHUCTT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHOATDONGSXKD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHOATDONGSXKD]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHOATDONGSXKD]
GO

CREATE TABLE [dbo].[DANHMUCHOATDONGSXKD] (
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENHOATDONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SELECTTED] bit  NULL,
  [BATBUOC] bit  NULL,
  [FORMATTXT] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHOATDONGSXKD] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHOPDONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHOPDONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHOPDONG]
GO

CREATE TABLE [dbo].[DANHMUCHOPDONG] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOPDONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDUNGHOPDONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOGOITHAU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYKY] smalldatetime  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [LOAITIEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIA] float(53)  NULL,
  [GIATRIHOPDONG] float(53)  NULL,
  [GIATRIHOPDONGDC] float(53)  NULL,
  [GIATRIHOPDONGVND] float(53)  NULL,
  [NGAYKETTHUC] smalldatetime  NULL,
  [GIATRIHOPDONGDCVND] float(53)  NULL,
  [NGAYKETTHUCDC] smalldatetime  NULL,
  [NGAYKETTHUCOK] smalldatetime  NULL,
  [KHOILUONGTHUCHIEN] float(53)  NULL,
  [KHOILUONGTHUCHIENLK] float(53)  NULL,
  [KHOILUONGTHUCHIENLKKD] float(53)  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [SOTIENLK] float(53)  NULL,
  [SOTIENLUYKE] float(53)  NULL,
  [SOTIENKHOIDAU] float(53)  NULL,
  [THOIHANTHANHTOAN] smalldatetime  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DAKETTHUC] bit DEFAULT 0 NULL,
  [HINHTHUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLETAITRO] float(53)  NULL,
  [SELECTTED] bit  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDUNGHOPDONGE] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOPPHAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIGOITHAU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENBAOLANH] float(53)  NULL,
  [KHOANMUC] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYKTHUCOK] smalldatetime  NULL,
  [NGAYKTDC] smalldatetime  NULL,
  [NGAYHETHANBAOLANH] smalldatetime  NULL,
  [NGAYKHONGPHANDOI] smalldatetime  NULL,
  [HINHTHUCTHAU] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LANDC] int  NULL,
  [SOTIENIFAD] float(53)  NULL,
  [SOTIENVND] float(53)  NULL,
  [TRANGTHAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOQUYETDINH] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYQD] smalldatetime  NULL,
  [NGAYBATDAU] smalldatetime  NULL,
  [IDDONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENGOITHAU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYKYPHULUC] smalldatetime  NULL,
  [THOIGIANGIAHAN] int  NULL,
  [THOIGIANTHUCHIEN] int  NULL,
  [NGAYTHUCHIEN] smalldatetime  NULL,
  [TENQUANLY] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIATRIHOPDONGUSD] float(53)  NULL,
  [PCSS] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIATRIHOPDONGDCUSD] float(53)  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHOPDONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHOPDONG68
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHOPDONG68]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHOPDONG68]
GO

CREATE TABLE [dbo].[DANHMUCHOPDONG68] (
  [STT] int  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYSINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TRINHDO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGACH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGACH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BACLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [NGAYHUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHOPDONG68] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHOSO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHOSO]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHOSO]
GO

CREATE TABLE [dbo].[DANHMUCHOSO] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOSO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHOSO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCHOSTMAILS
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCHOSTMAILS]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCHOSTMAILS]
GO

CREATE TABLE [dbo].[DANHMUCHOSTMAILS] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [EMAILGUI] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PASSMAILGUI] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PORTMAILGUI] int  NULL,
  [SMPTMAILGUI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SERCURITYMAIL] bit  NULL,
  [SUDUNG] bit  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCHOSTMAILS] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCKETCHUYEN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCKETCHUYEN]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCKETCHUYEN]
GO

CREATE TABLE [dbo].[DANHMUCKETCHUYEN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [SHTK] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [DIENGIAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCT] smalldatetime  NULL,
  [SOTIENUSD] float(53)  NULL,
  [CHUNGTUGHISO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [NOIDUNG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANMUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANMUC1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANMUC1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TOANDONVI] bit DEFAULT 0 NULL,
  [TOANDONVI1] bit DEFAULT 0 NULL,
  [NGAYTAO] smalldatetime  NULL,
  [USERNAME] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DTALL] bit DEFAULT 0 NULL
)
GO

ALTER TABLE [dbo].[DANHMUCKETCHUYEN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCKHOANCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCKHOANCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCKHOANCHI]
GO

CREATE TABLE [dbo].[DANHMUCKHOANCHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit  NULL,
  [SELECTTED] bit  NULL,
  [TENKHOANCHIEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCKHOANCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCKHOANMUC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCKHOANMUC]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCKHOANMUC]
GO

CREATE TABLE [dbo].[DANHMUCKHOANMUC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAKHOANMUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANMUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOPDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYBATDAU] smalldatetime  NULL,
  [NGAYKETTHUC] smalldatetime  NULL,
  [KETTHUC] bit DEFAULT 0 NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMATTXT] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCKHOANMUC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCKHOANTHU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCKHOANTHU]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCKHOANTHU]
GO

CREATE TABLE [dbo].[DANHMUCKHOANTHU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit  NULL,
  [SELECTTED] bit  NULL,
  [SOTIEN] float(53)  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCKHOANTHU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCKHOANTHUHOCPHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCKHOANTHUHOCPHI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCKHOANTHUHOCPHI]
GO

CREATE TABLE [dbo].[DANHMUCKHOANTHUHOCPHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADAOTAO] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAANA] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADOITUONG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNO] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCKHOANTHUHOCPHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCKHOVATTU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCKHOVATTU]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCKHOVATTU]
GO

CREATE TABLE [dbo].[DANHMUCKHOVATTU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENKHO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENTHOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUKHO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [MAPHONG] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVI] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVIOK] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVIOK] nvarchar(130) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTKVATTU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHUKHO] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCKHOVATTU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCKHUVUC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCKHUVUC]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCKHUVUC]
GO

CREATE TABLE [dbo].[DANHMUCKHUVUC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAKHUVUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENKHUVUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOMKHUVUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOMKHUVUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCKHUVUC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAIHOPDONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAIHOPDONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAIHOPDONG]
GO

CREATE TABLE [dbo].[DANHMUCLOAIHOPDONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [CHITIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAIHOPDONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAIKINHPHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAIKINHPHI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAIKINHPHI]
GO

CREATE TABLE [dbo].[DANHMUCLOAIKINHPHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MATINHCHATNGUON] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAIKINHPHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAILUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAILUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAILUONG]
GO

CREATE TABLE [dbo].[DANHMUCLOAILUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] smallint  NULL,
  [LOAILUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAILUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAIPHIEU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAIPHIEU]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAIPHIEU]
GO

CREATE TABLE [dbo].[DANHMUCLOAIPHIEU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [KYHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [CHITIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTEP] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THAMSO] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LAYDULIEU] bit  NULL,
  [DUNGCHOPHIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ATKNO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ATKCO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ASHTK] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ATKVAT] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAIPHIEU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAITAISAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAITAISAN]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAITAISAN]
GO

CREATE TABLE [dbo].[DANHMUCLOAITAISAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAITAISAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAITAMUNG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAITAMUNG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAITAMUNG]
GO

CREATE TABLE [dbo].[DANHMUCLOAITAMUNG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAITAMUNG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAITHUCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAITHUCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAITHUCHI]
GO

CREATE TABLE [dbo].[DANHMUCLOAITHUCHI] (
  [MA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAITHUCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAITHUCHINEW
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAITHUCHINEW]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAITHUCHINEW]
GO

CREATE TABLE [dbo].[DANHMUCLOAITHUCHINEW] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAITHUCHINEW] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAITHUNHAP
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAITHUNHAP]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAITHUNHAP]
GO

CREATE TABLE [dbo].[DANHMUCLOAITHUNHAP] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [LOAITHUNHAP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [SUDUNGPHONGBAN] bit  NULL,
  [LOAI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMTHUNHAP] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HIENOK] bit  NULL,
  [TENREPORT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LIST] nvarchar(2500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAITHUNHAP] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAITHUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAITHUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAITHUONG]
GO

CREATE TABLE [dbo].[DANHMUCLOAITHUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] smallint  NULL,
  [LOAITHUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAITHUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOAITIEN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOAITIEN]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOAITIEN]
GO

CREATE TABLE [dbo].[DANHMUCLOAITIEN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [KYHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIA] float(53)  NULL,
  [TYGIA1] float(53)  NULL,
  [TYGIA2] float(53)  NULL,
  [TYGIA3] float(53)  NULL,
  [TYGIA4] float(53)  NULL,
  [TYGIA5] float(53)  NULL,
  [TYGIA6] float(53)  NULL,
  [TYGIA7] float(53)  NULL,
  [TYGIA8] float(53)  NULL,
  [TYGIA9] float(53)  NULL,
  [TYGIA10] float(53)  NULL,
  [TYGIA11] float(53)  NULL,
  [TYGIA12] float(53)  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOAITIEN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLOPHOC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLOPHOC]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLOPHOC]
GO

CREATE TABLE [dbo].[DANHMUCLOPHOC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [TENLOP] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHE] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHE] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBAC] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIDAOTAO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUYENNGANH] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHDSXKD] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALOP] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAHOC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOPDONG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOCVIEN] float(53)  NULL,
  [SOTIENAN] float(53)  NULL,
  [SOTIENXANG] float(53)  NULL,
  [SOTIENTOTNGHIEP] float(53)  NULL,
  [GIATRIHOPDONG] float(53)  NULL,
  [SOTIENTT1] float(53)  NULL,
  [SOTIENTT2] float(53)  NULL,
  [SOTIENTT3] float(53)  NULL,
  [TENLOAIHINH] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLOPHOC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLYDONHAPXUAT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLYDONHAPXUAT]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLYDONHAPXUAT]
GO

CREATE TABLE [dbo].[DANHMUCLYDONHAPXUAT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALYDO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENLYDO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTAO] smalldatetime  NULL,
  [USERNAME] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit  NULL,
  [ATKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ATKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ASHTK] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AMAKHO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AUTOCHUNGTU] bit  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLYDONHAPXUAT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCLYDOTANGGIAM
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCLYDOTANGGIAM]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCLYDOTANGGIAM]
GO

CREATE TABLE [dbo].[DANHMUCLYDOTANGGIAM] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MALYDO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENLYDO] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTAO] smalldatetime  NULL,
  [USERNAME] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCLYDOTANGGIAM] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCMAHOADON
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCMAHOADON]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCMAHOADON]
GO

CREATE TABLE [dbo].[DANHMUCMAHOADON] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MAHOADON] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCMAHOADON] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCMALOAITAISAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCMALOAITAISAN]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCMALOAITAISAN]
GO

CREATE TABLE [dbo].[DANHMUCMALOAITAISAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCMALOAITAISAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCMUCDICHSUDUNG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCMUCDICHSUDUNG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCMUCDICHSUDUNG]
GO

CREATE TABLE [dbo].[DANHMUCMUCDICHSUDUNG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAMUCDICH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCDICH] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCMUCDICHSUDUNG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCNGUOIPHUTHUOC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCNGUOIPHUTHUOC]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCNGUOIPHUTHUOC]
GO

CREATE TABLE [dbo].[DANHMUCNGUOIPHUTHUOC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUANHE] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOVATEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCMND] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYSINH] smalldatetime  NULL,
  [SOTIEN] float(53)  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [MASOTHUEPHUTHUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGBATDAU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANGKETTHUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAQUOCTICH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENQUOCTICH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAQUANHE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCMNDPHUTHUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUYENSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAQUOCGIA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENQUOCGIA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MATINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHUYEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHUYEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAXA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENXA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCT] smalldatetime  NULL,
  [SONGUOI] float(53)  NULL,
  [MALUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCNGUOIPHUTHUOC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCNGUOISUDUNGDONVI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCNGUOISUDUNGDONVI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCNGUOISUDUNGDONVI]
GO

CREATE TABLE [dbo].[DANHMUCNGUOISUDUNGDONVI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [HOTEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCNGUOISUDUNGDONVI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCNHOMDONVI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCNHOMDONVI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCNHOMDONVI]
GO

CREATE TABLE [dbo].[DANHMUCNHOMDONVI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MANHOM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENNHOM] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCNHOMDONVI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCNHOMHANG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCNHOMHANG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCNHOMHANG]
GO

CREATE TABLE [dbo].[DANHMUCNHOMHANG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MANHOMHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENNHOMHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [TRUYENTHONG] bit DEFAULT 0 NULL,
  [KYHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCNHOMHANG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCNHOMKHUVUC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCNHOMKHUVUC]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCNHOMKHUVUC]
GO

CREATE TABLE [dbo].[DANHMUCNHOMKHUVUC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MANHOMKHUVUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENNHOMKHUVUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCNHOMKHUVUC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCNHOMQUANLY
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCNHOMQUANLY]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCNHOMQUANLY]
GO

CREATE TABLE [dbo].[DANHMUCNHOMQUANLY] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAQUANLY] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENQUANLY] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENQUANLYEN] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCNHOMQUANLY] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCNHOMTSCD1
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCNHOMTSCD1]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCNHOMTSCD1]
GO

CREATE TABLE [dbo].[DANHMUCNHOMTSCD1] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCNHOMTSCD1] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCNHOMTSCD2
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCNHOMTSCD2]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCNHOMTSCD2]
GO

CREATE TABLE [dbo].[DANHMUCNHOMTSCD2] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOMCAP1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOMCAP1] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL,
  [KYHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SONAM] int  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCNHOMTSCD2] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCPHONGBAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCPHONGBAN]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCPHONGBAN]
GO

CREATE TABLE [dbo].[DANHMUCPHONGBAN] (
  [STT] int  NULL,
  [MAPHONG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [MAQUANLY] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENQUANLY] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMAT] int  NULL,
  [NGANHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONLUONG] bit  NULL,
  [EMAIL] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVI] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTKOK] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCPHONGBAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCPHONGCUMOI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCPHONGCUMOI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCPHONGCUMOI]
GO

CREATE TABLE [dbo].[DANHMUCPHONGCUMOI] (
  [STT] int  NULL,
  [MAPHONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [MAPHONGCU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONGCU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCPHONGCUMOI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCSHORTCUT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCSHORTCUT]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCSHORTCUT]
GO

CREATE TABLE [dbo].[DANHMUCSHORTCUT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [KEYS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHIMTAT] int  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUPHIMTAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTEN] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCSHORTCUT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCSOCHUNGTU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCSOCHUNGTU]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCSOCHUNGTU]
GO

CREATE TABLE [dbo].[DANHMUCSOCHUNGTU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [LOAICHUNHTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DODAI] int  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAICHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ATKNO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ATKCO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AUTO] bit  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCSOCHUNGTU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCSOCHUNGTUHIENTAI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCSOCHUNGTUHIENTAI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCSOCHUNGTUHIENTAI]
GO

CREATE TABLE [dbo].[DANHMUCSOCHUNGTUHIENTAI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SHTK] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU2] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTUA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTUB] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCSOCHUNGTUHIENTAI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCSOTAIKHOANKHOBAC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCSOTAIKHOANKHOBAC]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCSOTAIKHOANKHOBAC]
GO

CREATE TABLE [dbo].[DANHMUCSOTAIKHOANKHOBAC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENNGANHANG] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUONGTRUC] bit  NULL,
  [MANGANHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [LOAITIEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SWIFTCODE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN1] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCSOTAIKHOANKHOBAC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTAIKHOAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTAIKHOAN]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTAIKHOAN]
GO

CREATE TABLE [dbo].[DANHMUCTAIKHOAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STTT] int  NULL,
  [SHTK] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENTAIKHOAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITAIKHOAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIET] bit DEFAULT 0 NULL,
  [CAPTK] int  NULL,
  [BATBUOC] bit DEFAULT 0 NULL,
  [NGUON] bit DEFAULT 0 NULL,
  [MUC] bit DEFAULT 0 NULL,
  [THU] bit DEFAULT 0 NULL,
  [DUAN] bit DEFAULT 0 NULL,
  [CLK] bit DEFAULT 0 NULL,
  [CHI] bit DEFAULT 0 NULL,
  [STT] int  NULL,
  [GIATHANH] bit DEFAULT 0 NULL,
  [VATTU] bit DEFAULT 0 NULL,
  [HOATDONG] bit DEFAULT 0 NULL,
  [TAITRO] bit DEFAULT 0 NULL,
  [HOPDONG] bit DEFAULT 0 NULL,
  [SANPHAM] bit DEFAULT 0 NULL,
  [CONGTRINH] bit DEFAULT 0 NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTIN] bit DEFAULT 0 NULL,
  [CHONNHOM] bit DEFAULT 0 NULL,
  [XACNHAN] bit DEFAULT 0 NULL,
  [HDSXKD] bit  NULL,
  [MUCTIEU] bit  NULL,
  [SELECTTED] bit  NULL,
  [PHONGBAN] bit  NULL,
  [ABC] int  NULL,
  [SHTKCU] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(60) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGCU] bit  NULL,
  [HANTT] bit  NULL,
  [VAYHD] bit  NULL,
  [KHOANMUC] bit  NULL,
  [HANGMUC] bit  NULL,
  [DONVIHANCHE] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUCHUNGTU] nvarchar(60) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] bit  NULL,
  [DONGIA] bit  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTAIKHOAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTENCONGTRINH
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTENCONGTRINH]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTENCONGTRINH]
GO

CREATE TABLE [dbo].[DANHMUCTENCONGTRINH] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MACONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUOCCT] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOIDAU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETTHUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSOKINHPHI] float(53)  NULL,
  [SODUOCDUYET] float(53)  NULL,
  [COQUANTHUCHIEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMDUAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTIN] bit DEFAULT 0 NULL,
  [SELECTTED] bit  NULL,
  [MASODUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINHEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit  NULL,
  [IDMADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENKEHOACH] float(53)  NULL,
  [SOTIENDAIKEHOACH] float(53)  NULL,
  [SOTIENNGUONKHAC] float(53)  NULL,
  [GIATRIHOPDONGODUPHONG] float(53)  NULL,
  [MASOCONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTENCONGTRINH] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTENDONVI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTENDONVI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTENDONVI]
GO

CREATE TABLE [dbo].[DANHMUCTENDONVI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MAMOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENTHOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [TENNGANHANG] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVISDNS] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACHUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EMAIL] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTKOK] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THU] bit  NULL,
  [PHIEUTHU] bit  NULL,
  [PHIEUCHI] bit  NULL,
  [PHIEUKHAC] bit  NULL,
  [UYNHIEMCHI] bit  NULL,
  [RUTTM] bit  NULL,
  [RUTCK] bit  NULL,
  [TRUCTHUOC] bit  NULL,
  [TENDONVIE] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONIN] bit  NULL,
  [TENNHOME] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [CHUNGTU] bit  NULL,
  [FAX] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTIN] bit  NULL,
  [MANHOM1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM1] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM2] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM2] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAQUANLY] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENQUANLY] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHANVIEN] bit  NULL,
  [NHOMNHANVIEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCVU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCMND] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYVAODONVI] smalldatetime  NULL,
  [MACAPNGANSACH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENKEHOACH] float(53)  NULL,
  [NOICAP] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCAP] smalldatetime  NULL,
  [COQUANQUANLY] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KBNNHT] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACQTHU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANDKT] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOSBHXH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDKKHAM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SONGUOI] int  NULL,
  [DTBHXH] bit  NULL,
  [CD] bit  NULL,
  [MACHUCDANH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCHUCDANH] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIOITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYSINH] smalldatetime  NULL,
  [LOAIHOPDONG] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTA] int  NULL,
  [LOAICANBO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN2] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG2] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN3] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG3] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [NGAYKETTHUCHD] smalldatetime  NULL,
  [AMACHUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [APCONGTHUC] bit  NULL,
  [DANGHIHUU] bit  NULL,
  [IDDONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAP1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPDT] int  NULL,
  [CHITIEUTT99] nvarchar(140) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONGUIMAIL] bit  NULL,
  [PATHPDF] nvarchar(320) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MKHAUWEB] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTENDONVI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTENDONVI_LUU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTENDONVI_LUU]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTENDONVI_LUU]
GO

CREATE TABLE [dbo].[DANHMUCTENDONVI_LUU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENTHOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [TENNGANHANG] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVISDNS] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACHUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EMAIL] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTKOK] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THU] bit  NULL,
  [PHIEUTHU] bit  NULL,
  [PHIEUCHI] bit  NULL,
  [PHIEUKHAC] bit  NULL,
  [UYNHIEMCHI] bit  NULL,
  [RUTTM] bit  NULL,
  [RUTCK] bit  NULL,
  [TRUCTHUOC] bit  NULL,
  [TENDONVIE] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONIN] bit  NULL,
  [TENNHOME] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [CHUNGTU] bit  NULL,
  [FAX] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTIN] bit  NULL,
  [MANHOM1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM1] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM2] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM2] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAQUANLY] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENQUANLY] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHANVIEN] bit  NULL,
  [NHOMNHANVIEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCVU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCMND] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYVAODONVI] smalldatetime  NULL,
  [MACAPNGANSACH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENKEHOACH] float(53)  NULL,
  [NOICAP] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCAP] smalldatetime  NULL,
  [COQUANQUANLY] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KBNNHT] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACQTHU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANDKT] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOSBHXH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDKKHAM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SONGUOI] int  NULL,
  [DTBHXH] bit  NULL,
  [CD] bit  NULL,
  [MACHUCDANH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCHUCDANH] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIOITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYSINH] smalldatetime  NULL,
  [LOAIHOPDONG] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTA] int  NULL,
  [LOAICANBO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN2] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG2] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN3] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG3] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [NGAYKETTHUCHD] smalldatetime  NULL,
  [AMACHUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [APCONGTHUC] bit  NULL,
  [MAMOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTENDONVI_LUU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTENDONVILUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTENDONVILUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTENDONVILUONG]
GO

CREATE TABLE [dbo].[DANHMUCTENDONVILUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MAOK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHONGDAU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENTHOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [TENNGANHANG] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVISDNS] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACHUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EMAIL] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTKOK] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THU] bit  NULL,
  [PHIEUTHU] bit  NULL,
  [PHIEUCHI] bit  NULL,
  [PHIEUKHAC] bit  NULL,
  [UYNHIEMCHI] bit  NULL,
  [RUTTM] bit  NULL,
  [RUTCK] bit  NULL,
  [TRUCTHUOC] bit  NULL,
  [TENDONVIE] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONIN] bit  NULL,
  [TENNHOME] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [CHUNGTU] bit  NULL,
  [FAX] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTIN] bit  NULL,
  [MANHOM1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM1] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM2] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM2] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAQUANLY] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENQUANLY] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHANVIEN] bit  NULL,
  [NHOMNHANVIEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCVU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCMND] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYVAODONVI] smalldatetime  NULL,
  [MACAPNGANSACH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENKEHOACH] float(53)  NULL,
  [NGAYCAP] smalldatetime  NULL,
  [NOICAP] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [COQUANQUANLY] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KBNNHT] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACQTHU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANDKT] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOSBHXH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDKKHAM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SONGUOI] int  NULL,
  [DTBHXH] bit  NULL,
  [CD] bit  NULL,
  [MACHUCDANH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCHUCDANH] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIOITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYSINH] smalldatetime  NULL,
  [LOAIHOPDONG] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTA] int  NULL,
  [MACHUCVU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCHUCVU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAICANBO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN2] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG2] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN3] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG3] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [VAOBIENCHE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAPSUDAU] smalldatetime  NULL,
  [TAPSUCUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUVIECDAU] smalldatetime  NULL,
  [THUVIECCUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHUBIENCHE] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHUTAPSU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHUTHUVIEC] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUYENKHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DATEBIENCHE_TEMPS] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DATETAPSUDAU_TEMPS] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DATETAPSUCUOI_TEMPS] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DATETHUVIECDAU_TEMPS] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DATETHUVIECCUOI_TEMPS] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HESOLUONG] float(53)  NULL,
  [NGAYKETTHUCHD] smalldatetime  NULL,
  [TRINHDO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETTHUCHOPDONG] bit  NULL,
  [NGHIKHONGLUONG] bit  NULL,
  [HD] bit  NULL,
  [NH] bit  NULL,
  [NGUNGTHEODOI] bit  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTENDONVILUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTENDUAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTENDUAN]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTENDUAN]
GO

CREATE TABLE [dbo].[DANHMUCTENDUAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENDUAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUOCCT] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOIDAU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETTHUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGSOKINHPHI] float(53)  NULL,
  [SODUOCDUYET] float(53)  NULL,
  [COQUANTHUCHIEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMDUAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUOCCHUONGTRINH] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHATAITRO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONIN] bit DEFAULT 0 NULL,
  [MANHOM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUANE] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [KYTRUOC] float(53)  NULL,
  [MASODUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCHUONGTRINH] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTENDUAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTHUCHIENHOPDONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTHUCHIENHOPDONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTHUCHIENHOPDONG]
GO

CREATE TABLE [dbo].[DANHMUCTHUCHIENHOPDONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTHUCHIENHOPDONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTHUEVAT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTHUEVAT]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTHUEVAT]
GO

CREATE TABLE [dbo].[DANHMUCTHUEVAT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [TYLEVAT] float(53)  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE Finnish_Swedish_CI_AS  NULL,
  [LOAI] nvarchar(10) COLLATE Finnish_Swedish_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTHUEVAT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTKDONGTHOI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTKDONGTHOI]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTKDONGTHOI]
GO

CREATE TABLE [dbo].[DANHMUCTKDONGTHOI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [TKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNOOK] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCOOK] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAINO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTKDONGTHOI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTOCKY
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTOCKY]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTOCKY]
GO

CREATE TABLE [dbo].[DANHMUCTOCKY] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [KYHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMMUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDUNG] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTOCKY] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTRANGTHAICHUNGTU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTRANGTHAICHUNGTU]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTRANGTHAICHUNGTU]
GO

CREATE TABLE [dbo].[DANHMUCTRANGTHAICHUNGTU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [VAOSO] bit  NULL,
  [THEKHO] bit  NULL,
  [HUY] bit  NULL,
  [KHOA] bit  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTRANGTHAICHUNGTU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DANHMUCTSCD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUCTSCD]') AND type IN ('U'))
	DROP TABLE [dbo].[DANHMUCTSCD]
GO

CREATE TABLE [dbo].[DANHMUCTSCD] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOSERI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOMCAP1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOMCAP1] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOMCAP2] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNHOMCAP2] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENLOAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THOIHAN] smallint  NULL,
  [NAMSUDUNG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NUOCSANXUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NAMSANXUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DACDIEMKYTHUAT] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLEKHAUHAO] float(53)  NULL,
  [SELECTTED] bit  NULL,
  [KYHIEUTSCD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHKHAUHAO] bit  NULL,
  [DONGIAA] float(53)  NULL,
  [SOLUONGOK] int  NULL,
  [OK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONGX] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONGX] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKPX] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHIX] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYSUDUNG] smalldatetime  NULL,
  [SHTK] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DANHMUCTSCD] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DATAHACHTOANLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DATAHACHTOANLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DATAHACHTOANLUONG]
GO

CREATE TABLE [dbo].[DATAHACHTOANLUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCT] smalldatetime  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(55) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [SOTIENUSD] float(53)  NULL,
  [TYGIA] float(53)  NULL,
  [TKNO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETCHUYEN1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHANHPHAM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MATHANHPHAM] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATVON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAMUNG] bit  NULL,
  [MAHOATDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KINHPHIGIAM] bit  NULL,
  [TKNGOAINO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL,
  [NAMHOATDONG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHANGMUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DARUTOK] bit  NULL,
  [MASOHOPDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUTOCKY] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIAEU] float(53)  NULL,
  [SOTIENEU] float(53)  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOXACNHANVIENTRO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYXACNHAN] smalldatetime  NULL,
  [MAHDSXKD] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOXACNHANVIENTRO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYXACNHAN1] smalldatetime  NULL,
  [LUONGPHUCAPDACTHU] float(53)  NULL,
  [KINHPHIKHOIPHUC] bit  NULL,
  [DIENGIAIEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MABANGKE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMUCTIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIATRUNGTHAU] float(53)  NULL,
  [LUYKECUOI] float(53)  NULL,
  [SODUTAMUNG] float(53)  NULL,
  [SOTIENBAOHANH] float(53)  NULL,
  [SOTIENTHUE] float(53)  NULL,
  [SOTIENTRUOCTHUE] float(53)  NULL,
  [MUC1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHIUTHUE] bit  NULL,
  [NGAYHOADON] smalldatetime  NULL,
  [IDOK] int  NULL,
  [MAPHONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [CAMKETCHI] bit  NULL,
  [SOPHIEUTHUCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [THANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DATAHACHTOANLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DATALIENTUC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DATALIENTUC]') AND type IN ('U'))
	DROP TABLE [dbo].[DATALIENTUC]
GO

CREATE TABLE [dbo].[DATALIENTUC] (
  [MAPHONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANMUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANMUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOCKED] bit  NULL,
  [TRANGTHAI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVITHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVITHUE] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTUTHEOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENTHUE] float(53)  NULL,
  [NGAYHOADON] smalldatetime  NULL,
  [SOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAMKETCHI] bit  NULL,
  [MABANGKE] int  NULL,
  [KINHPHIKHOIPHUC] bit  NULL,
  [LOAITAMUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGHOA] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NAMHOATDONG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENDUTOAN] float(53)  NULL,
  [SOCHUNGTUDUTOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SCTNUMBER] int  NULL,
  [KETCHUYEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [NGAYCT] smalldatetime  NULL,
  [CHUNGTUGHISO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTEDIN] bit  NULL,
  [PHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGSOTIEN] float(53)  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [NOIDUNG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDID] int  NULL,
  [DIENGIAI] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETCHUYEN1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIA] float(53)  NULL,
  [SOTIENUSD] float(53)  NULL,
  [USERNAME] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADOITUONG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDOITUONG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MATHANHPHAM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHANHPHAM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHIDOITUONG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATVON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAMUNG] int  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KINHPHIGIAM] bit  NULL,
  [KINHPHIOK] int  NULL,
  [TKNGOAINO] nvarchar(80) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(80) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTAO] smalldatetime  NULL,
  [KEMTHEO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL,
  [MADOITUONG1] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDOITUONG1] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHIDOITUONG1] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALINHTIEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENLINHTIEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOICAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCAP] smalldatetime  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DARUTOK] bit  NULL,
  [MASOHOPDONG] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUTOCKY] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AUTOTYGIA] int  NULL,
  [TYGIAEU] float(53)  NULL,
  [SOTIENEU] float(53)  NULL,
  [VAOSO] bit  NULL,
  [MAHDSXKD] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANHTIEN] float(53)  NULL,
  [SOXACNHANVIENTRO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOXACNHANVIENTRO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYXACNHAN] smalldatetime  NULL,
  [NGAYXACNHAN1] smalldatetime  NULL,
  [MAMUCTIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHATKY] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDMAVATTUPHIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGTON] float(53)  NULL,
  [LOHANG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HANSUDUNG] smalldatetime  NULL,
  [LPHIEUVATTU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGUOINHAP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHUND] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG1] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HANTHANHTOAN] smalldatetime  NULL
)
GO

ALTER TABLE [dbo].[DATALIENTUC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DATALIENTUCKHAC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DATALIENTUCKHAC]') AND type IN ('U'))
	DROP TABLE [dbo].[DATALIENTUCKHAC]
GO

CREATE TABLE [dbo].[DATALIENTUCKHAC] (
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(1350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANMUC] nvarchar(1350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOCKED] bit  NULL,
  [TRANGTHAI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVITHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVITHUE] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTUTHEOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENTHUE] float(53)  NULL,
  [NGAYHOADON] smalldatetime  NULL,
  [SOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAMKETCHI] bit  NULL,
  [MABANGKE] int  NULL,
  [KINHPHIKHOIPHUC] bit  NULL,
  [LOAITAMUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NAMHOATDONG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENDUTOAN] float(53)  NULL,
  [SOCHUNGTUDUTOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SCTNUMBER] int  NULL,
  [KETCHUYEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [NGAYCT] smalldatetime  NULL,
  [CHUNGTUGHISO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTEDIN] bit  NULL,
  [PHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGSOTIEN] float(53)  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [NOIDUNG] nvarchar(1350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDID] int  NULL,
  [DIENGIAI] nvarchar(1350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETCHUYEN1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(1350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIA] float(53)  NULL,
  [SOTIENUSD] float(53)  NULL,
  [USERNAME] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADOITUONG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDOITUONG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MATHANHPHAM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHANHPHAM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(855) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHIDOITUONG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATVON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAMUNG] int  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KINHPHIGIAM] bit  NULL,
  [KINHPHIOK] int  NULL,
  [TKNGOAINO] nvarchar(80) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(80) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTAO] smalldatetime  NULL,
  [KEMTHEO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL,
  [MADOITUONG1] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDOITUONG1] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHIDOITUONG1] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALINHTIEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENLINHTIEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOICAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCAP] smalldatetime  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DARUTOK] bit  NULL,
  [MASOHOPDONG] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUTOCKY] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AUTOTYGIA] int  NULL,
  [TYGIAEU] float(53)  NULL,
  [SOTIENEU] float(53)  NULL,
  [VAOSO] bit  NULL,
  [MAHDSXKD] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANHTIEN] float(53)  NULL,
  [SOXACNHANVIENTRO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOXACNHANVIENTRO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYXACNHAN] smalldatetime  NULL,
  [NGAYXACNHAN1] smalldatetime  NULL,
  [MAMUCTIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHATKY] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDMAVATTUPHIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGTON] float(53)  NULL,
  [LOHANG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HANSUDUNG] smalldatetime  NULL,
  [LPHIEUVATTU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGUOINHAP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHUND] nvarchar(1350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG1] nvarchar(820) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HANTHANHTOAN] smalldatetime  NULL
)
GO

ALTER TABLE [dbo].[DATALIENTUCKHAC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DATANHIEUNAMOK
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DATANHIEUNAMOK]') AND type IN ('U'))
	DROP TABLE [dbo].[DATANHIEUNAMOK]
GO

CREATE TABLE [dbo].[DATANHIEUNAMOK] (
  [KINHPHIKHOIPHUC] bit  NULL,
  [LOAITAMUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGHOA] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NAMHOATDONG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENDUTOAN] float(53)  NULL,
  [SOCHUNGTUDUTOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SCTNUMBER] int  NULL,
  [KETCHUYEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [NGAYCT] smalldatetime  NULL,
  [CHUNGTUGHISO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTEDIN] int  NULL,
  [PHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGSOTIEN] float(53)  NULL,
  [ID] int  NOT NULL,
  [NOIDUNG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDID] int  NOT NULL,
  [DIENGIAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETCHUYEN1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIA] float(53)  NULL,
  [SOTIENUSD] float(53)  NULL,
  [USERNAME] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADOITUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDOITUONG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MATHANHPHAM] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHANHPHAM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHIDOITUONG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATVON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAMUNG] int  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CLUONG] int  NULL,
  [KINHPHIGIAM] int  NULL,
  [KINHPHIOK] int  NULL,
  [TKNGOAINO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTAO] smalldatetime  NULL,
  [KEMTHEO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL,
  [MADOITUONG1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDOITUONG1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHIDOITUONG1] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALINHTIEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENLINHTIEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOICAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCAP] smalldatetime  NULL,
  [SOCMND] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DARUTOK] int  NULL,
  [MASOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUTOCKY] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AUTOTYGIA] int  NULL,
  [TYGIAEU] float(53)  NULL,
  [SOTIENEU] float(53)  NULL,
  [VAOSO] int  NULL,
  [MAHDSXKD] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANHTIEN] float(53)  NULL,
  [SOXACNHANVIENTRO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOXACNHANVIENTRO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYXACNHAN] smalldatetime  NULL,
  [NGAYXACNHAN1] smalldatetime  NULL,
  [MAMUCTIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDOK] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[DATANHIEUNAMOK] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DIEUCHUYEN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DIEUCHUYEN]') AND type IN ('U'))
	DROP TABLE [dbo].[DIEUCHUYEN]
GO

CREATE TABLE [dbo].[DIEUCHUYEN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [LOAI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVITRUCTHUOC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVITRUCTHUOC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [PHIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCT] smalldatetime  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEMTHEO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [USERNAME] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETCHUYEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHANXUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHANXUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO2] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO2] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALYDO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENLYDO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIA] float(53)  NULL,
  [SOHOADON] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUHOADON] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLEVATTMP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHUONGTIEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LENHDIEUDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DIEUCHUYEN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DINHMUCTHANHPHAM
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DINHMUCTHANHPHAM]') AND type IN ('U'))
	DROP TABLE [dbo].[DINHMUCTHANHPHAM]
GO

CREATE TABLE [dbo].[DINHMUCTHANHPHAM] (
  [STT] int  NULL,
  [MATHANHPHAM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHANHPHAM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGTHANHPHAM] float(53)  NULL,
  [SOTIENTHANHPHAM] float(53)  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [USOTIEN] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[DINHMUCTHANHPHAM] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DMTAIKHOANNHATHUOC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DMTAIKHOANNHATHUOC]') AND type IN ('U'))
	DROP TABLE [dbo].[DMTAIKHOANNHATHUOC]
GO

CREATE TABLE [dbo].[DMTAIKHOANNHATHUOC] (
  [SHTK] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTAIKHOAN] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[DMTAIKHOANNHATHUOC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DMVT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DMVT]') AND type IN ('U'))
	DROP TABLE [dbo].[DMVT]
GO

CREATE TABLE [dbo].[DMVT] (
  [MAHANGHOA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DVT] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANHOM] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIVT] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKGIAVON] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKDT] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENE] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIAODICH] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DMVT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DULIEUBANGLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DULIEUBANGLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[DULIEUBANGLUONG]
GO

CREATE TABLE [dbo].[DULIEUBANGLUONG] (
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [NGAYCT] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DULIEUBANGLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for DUTOANDUOCGIAO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[DUTOANDUOCGIAO]') AND type IN ('U'))
	DROP TABLE [dbo].[DUTOANDUOCGIAO]
GO

CREATE TABLE [dbo].[DUTOANDUOCGIAO] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [CHITIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUYI] float(53)  NULL,
  [QUYII] float(53)  NULL,
  [QUYIII] float(53)  NULL,
  [QUYIV] float(53)  NULL,
  [DUTOANNAMTRUOC] float(53)  NULL,
  [NGAYCT] smalldatetime  NULL,
  [KINHPHIOK] int  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITAMUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMUCTIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIEUCHINH] bit  NULL,
  [MAHDSXKD] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(230) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENNHAN] float(53)  NULL,
  [NAM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[DUTOANDUOCGIAO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for FIELDOFBANGLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[FIELDOFBANGLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[FIELDOFBANGLUONG]
GO

CREATE TABLE [dbo].[FIELDOFBANGLUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTOK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FLDNAME] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FLDTEXT] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHIEURONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FLDTYPE] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FLDLEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HIEN] bit  NULL,
  [BUTTONOK] bit  NULL,
  [TXTALIGN] int  NULL,
  [TENBANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FONTS] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [COLORS] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BACKCOLORS] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHOWBUTTON] bit  NULL,
  [TYPEDATE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TITLE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FLD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LACONGTHUC] bit  NULL,
  [CONGTHUC] nvarchar(1550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUNHAP] nvarchar(1550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIEUKIEN] nvarchar(1550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GROUPBY] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[FIELDOFBANGLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for FIELDOFBANGLUONGLD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[FIELDOFBANGLUONGLD]') AND type IN ('U'))
	DROP TABLE [dbo].[FIELDOFBANGLUONGLD]
GO

CREATE TABLE [dbo].[FIELDOFBANGLUONGLD] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTOK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FLDNAME] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FLDTEXT] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DANHMUCNAME] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FLDTYPE] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FLDLEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HIEN] bit  NULL,
  [BUTTONOK] bit  NULL,
  [TXTALIGN] int  NULL
)
GO

ALTER TABLE [dbo].[FIELDOFBANGLUONGLD] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for GHINO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[GHINO]') AND type IN ('U'))
	DROP TABLE [dbo].[GHINO]
GO

CREATE TABLE [dbo].[GHINO] (
  [ID] int  NOT NULL,
  [MSSV] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENSV] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKHOANTHU] int  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMONHOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMONHOC] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTINCHI] int  NULL,
  [DONGIA] numeric(18,2)  NULL,
  [THANHTIEN] numeric(18,2)  NULL,
  [SOTIENMIENGIAM] numeric(18,2)  NULL,
  [SOTIENPHAINOP] numeric(18,2)  NULL
)
GO

ALTER TABLE [dbo].[GHINO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for HANGMUCTAITRO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[HANGMUCTAITRO]') AND type IN ('U'))
	DROP TABLE [dbo].[HANGMUCTAITRO]
GO

CREATE TABLE [dbo].[HANGMUCTAITRO] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUCE] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit DEFAULT 0 NULL,
  [SELECTIN] bit DEFAULT 0 NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH] float(53)  NULL,
  [KEHOACHEU] float(53)  NULL,
  [KEHOACHUSD] float(53)  NULL
)
GO

ALTER TABLE [dbo].[HANGMUCTAITRO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for HANMUCKINHPHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[HANMUCKINHPHI]') AND type IN ('U'))
	DROP TABLE [dbo].[HANMUCKINHPHI]
GO

CREATE TABLE [dbo].[HANMUCKINHPHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [PHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCT] smalldatetime  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTEDIN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [USERNAME] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAINO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTAO] smalldatetime  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AUTOTYGIA] bit  NULL,
  [CHUNGTUGHISO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDUNG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMUCTIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICUNGCAP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICUNGCAP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONGKHUNG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTUCKC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIATRIHOPDONG] float(53)  NULL,
  [NGAYKY] smalldatetime  NULL,
  [MACONGTRINH] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN1] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC1] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[HANMUCKINHPHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for HEADEHOADONINPUT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[HEADEHOADONINPUT]') AND type IN ('U'))
	DROP TABLE [dbo].[HEADEHOADONINPUT]
GO

CREATE TABLE [dbo].[HEADEHOADONINPUT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [IDDONVICAPDUOI] int  NOT NULL,
  [STT] int  NULL,
  [STTND] int  NULL,
  [NAM] int  NOT NULL,
  [SOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [INVOICEGUI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [INVOICECODE] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EMAIL] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGUI] datetime  NULL,
  [NGAYNHAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYHOADON] smalldatetime  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENTHOAI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DAXULY] bit  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLEVAT] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [SOTIENVAT] float(53)  NULL,
  [SOTIENVND] float(53)  NULL,
  [SOTIENHANG] float(53)  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENTHUE] float(53)  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUHOADON] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MauHoaDon] nvarchar(210) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [DIACHINHAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYUPDATE] datetime  NULL,
  [KIEUNHANHOADON] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HINHTHUCTT] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(225) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [XEMSUAXOA] nchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Lock] bit  NULL,
  [DAXOA] bit  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[HEADEHOADONINPUT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for HIENANCACCOT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[HIENANCACCOT]') AND type IN ('U'))
	DROP TABLE [dbo].[HIENANCACCOT]
GO

CREATE TABLE [dbo].[HIENANCACCOT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [TENCOT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTRUONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[HIENANCACCOT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for HOATDONGCOTHU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[HOATDONGCOTHU]') AND type IN ('U'))
	DROP TABLE [dbo].[HOATDONGCOTHU]
GO

CREATE TABLE [dbo].[HOATDONGCOTHU] (
  [ID] int  NULL,
  [STT] int  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUCTINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[HOATDONGCOTHU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for HOATDONGDUAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[HOATDONGDUAN]') AND type IN ('U'))
	DROP TABLE [dbo].[HOATDONGDUAN]
GO

CREATE TABLE [dbo].[HOATDONGDUAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [TENHOATDONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONGE] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit DEFAULT 0 NULL,
  [NAMHOATDONG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTIN] bit DEFAULT 0 NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH] float(53)  NULL,
  [KEHOACHEU] float(53)  NULL,
  [KEHOACHUSD] float(53)  NULL,
  [MAQUANLY] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVIHANCHE] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[HOATDONGDUAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for HOATDONGKYTRUOC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[HOATDONGKYTRUOC]') AND type IN ('U'))
	DROP TABLE [dbo].[HOATDONGKYTRUOC]
GO

CREATE TABLE [dbo].[HOATDONGKYTRUOC] (
  [LOAIHOATDONG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [QUY] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUYKE] float(53)  NULL
)
GO

ALTER TABLE [dbo].[HOATDONGKYTRUOC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for HOATDONGSXKD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[HOATDONGSXKD]') AND type IN ('U'))
	DROP TABLE [dbo].[HOATDONGSXKD]
GO

CREATE TABLE [dbo].[HOATDONGSXKD] (
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[HOATDONGSXKD] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for INFOAPI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[INFOAPI]') AND type IN ('U'))
	DROP TABLE [dbo].[INFOAPI]
GO

CREATE TABLE [dbo].[INFOAPI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LINKAPI] nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [USERPASS] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAXCODE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EMAIL] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PASSMAIL] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[INFOAPI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KEHOACHDKDV
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KEHOACHDKDV]') AND type IN ('U'))
	DROP TABLE [dbo].[KEHOACHDKDV]
GO

CREATE TABLE [dbo].[KEHOACHDKDV] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH] float(53)  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAUNAM] float(53)  NULL,
  [MAHDSXKD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYTRONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[KEHOACHDKDV] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KEHOACHKDDVHOPDONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KEHOACHKDDVHOPDONG]') AND type IN ('U'))
	DROP TABLE [dbo].[KEHOACHKDDVHOPDONG]
GO

CREATE TABLE [dbo].[KEHOACHKDDVHOPDONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH] float(53)  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAUNAM] float(53)  NULL
)
GO

ALTER TABLE [dbo].[KEHOACHKDDVHOPDONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KEHOACHKHOANCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KEHOACHKHOANCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[KEHOACHKHOANCHI]
GO

CREATE TABLE [dbo].[KEHOACHKHOANCHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(355) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYTRONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH] float(53)  NULL,
  [SODAUNAM] float(53)  NULL,
  [FORMATTXT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH1] float(53)  NULL,
  [KEHOACH2] float(53)  NULL,
  [KEHOACH3] float(53)  NULL,
  [KEHOACH4] float(53)  NULL,
  [NAM] int  NULL,
  [KEHOACH5] float(53)  NULL,
  [KEHOACH6] float(53)  NULL,
  [KEHOACH7] float(53)  NULL,
  [KEHOACH8] float(53)  NULL,
  [KEHOACH9] float(53)  NULL,
  [KEHOACH10] float(53)  NULL,
  [KEHOACH11] float(53)  NULL,
  [KEHOACH12] float(53)  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACHIENLUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[KEHOACHKHOANCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KEHOACHKHOANCHI_CU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KEHOACHKHOANCHI_CU]') AND type IN ('U'))
	DROP TABLE [dbo].[KEHOACHKHOANCHI_CU]
GO

CREATE TABLE [dbo].[KEHOACHKHOANCHI_CU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(355) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYTRONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH] float(53)  NULL,
  [SODAUNAM] float(53)  NULL,
  [FORMATTXT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH1] float(53)  NULL,
  [KEHOACH2] float(53)  NULL,
  [KEHOACH3] float(53)  NULL,
  [KEHOACH4] float(53)  NULL,
  [NAM] int  NULL,
  [KEHOACH5] float(53)  NULL,
  [KEHOACH6] float(53)  NULL,
  [KEHOACH7] float(53)  NULL,
  [KEHOACH8] float(53)  NULL,
  [KEHOACH9] float(53)  NULL,
  [KEHOACH10] float(53)  NULL,
  [KEHOACH11] float(53)  NULL,
  [KEHOACH12] float(53)  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACHIENLUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[KEHOACHKHOANCHI_CU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KEHOACHKHOANMUC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KEHOACHKHOANMUC]') AND type IN ('U'))
	DROP TABLE [dbo].[KEHOACHKHOANMUC]
GO

CREATE TABLE [dbo].[KEHOACHKHOANMUC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASOHOATDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOATDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH1] float(53)  NULL,
  [KEHOACH2] float(53)  NULL,
  [KEHOACH3] float(53)  NULL,
  [KEHOACH4] float(53)  NULL,
  [KEHOACH5] float(53)  NULL,
  [KEHOACH6] float(53)  NULL,
  [KEHOACH7] float(53)  NULL,
  [KEHOACHUSD1] float(53)  NULL,
  [KEHOACHUSD2] float(53)  NULL,
  [KEHOACHUSD3] float(53)  NULL,
  [KEHOACHUSD4] float(53)  NULL,
  [KEHOACHUSD5] float(53)  NULL,
  [KEHOACHUSD6] float(53)  NULL,
  [KEHOACHUSD7] float(53)  NULL,
  [BATBUOC] bit  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGKEHOACH] float(53)  NULL,
  [TONGKEHOACHUSD] float(53)  NULL
)
GO

ALTER TABLE [dbo].[KEHOACHKHOANMUC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KEHOACHKHOANTHU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KEHOACHKHOANTHU]') AND type IN ('U'))
	DROP TABLE [dbo].[KEHOACHKHOANTHU]
GO

CREATE TABLE [dbo].[KEHOACHKHOANTHU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(355) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYTRONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH] float(53)  NULL,
  [SODAUNAM] float(53)  NULL,
  [FORMATTXT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH1] float(53)  NULL,
  [KEHOACH2] float(53)  NULL,
  [KEHOACH3] float(53)  NULL,
  [KEHOACH4] float(53)  NULL,
  [NAM] int  NULL,
  [KEHOACH5] float(53)  NULL,
  [KEHOACH6] float(53)  NULL,
  [KEHOACH7] float(53)  NULL,
  [KEHOACH8] float(53)  NULL,
  [KEHOACH9] float(53)  NULL,
  [KEHOACH10] float(53)  NULL,
  [KEHOACH11] float(53)  NULL,
  [KEHOACH12] float(53)  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACHIENLUOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[KEHOACHKHOANTHU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KEHOACHNGANSACHDUAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KEHOACHNGANSACHDUAN]') AND type IN ('U'))
	DROP TABLE [dbo].[KEHOACHNGANSACHDUAN]
GO

CREATE TABLE [dbo].[KEHOACHNGANSACHDUAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASOHOATDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOATDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUYI1] float(53)  NULL,
  [QUYII1] float(53)  NULL,
  [QUYIII1] float(53)  NULL,
  [QUYIV1] float(53)  NULL,
  [QUYI2] float(53)  NULL,
  [QUYII2] float(53)  NULL,
  [QUYIII2] float(53)  NULL,
  [QUYIV2] float(53)  NULL,
  [KEHOACHKHOIDAU1] float(53)  NULL,
  [KEHOACHKHOIDAU1USD] float(53)  NULL,
  [KEHOACHKHOIDAU2] float(53)  NULL,
  [KEHOACHKHOIDAU2USD] float(53)  NULL,
  [KEHOACH] float(53)  NULL,
  [KEHOACHUSD] float(53)  NULL,
  [QUYIUSD1] float(53)  NULL,
  [QUYIIUSD1] float(53)  NULL,
  [QUYIIIUSD1] float(53)  NULL,
  [QUYIVUSD1] float(53)  NULL,
  [QUYIUSD2] float(53)  NULL,
  [QUYIIUSD2] float(53)  NULL,
  [QUYIIIUSD2] float(53)  NULL,
  [QUYIVUSD2] float(53)  NULL,
  [BATBUOC] bit  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TONGNGANSACH] float(53)  NULL,
  [NGANSACH1] float(53)  NULL,
  [NGANSACH2] float(53)  NULL,
  [NGANSACH] float(53)  NULL,
  [TONGNGANSACHUSD] float(53)  NULL,
  [NGANSACHUSD1] float(53)  NULL,
  [NGANSACHUSD2] float(53)  NULL,
  [NGANSACHUSD] float(53)  NULL,
  [STT1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT2] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT3] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FINMOD] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGANSACHDC] float(53)  NULL,
  [NGANSACHDCUSD] float(53)  NULL
)
GO

ALTER TABLE [dbo].[KEHOACHNGANSACHDUAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KEHOACHNHAPVATTU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KEHOACHNHAPVATTU]') AND type IN ('U'))
	DROP TABLE [dbo].[KEHOACHNHAPVATTU]
GO

CREATE TABLE [dbo].[KEHOACHNHAPVATTU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [MAHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOQUYETDINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVICAPDUOI] int  NULL,
  [SOLUONGA] float(53)  NULL,
  [SELECTTED] bit  NULL,
  [STT] int  NULL,
  [SOLUONGTMP] float(53)  NULL,
  [SOLUONGCONLAI] float(53)  NULL,
  [TKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENTEMP] float(53)  NULL,
  [SOLUONGDANHAP] float(53)  NULL
)
GO

ALTER TABLE [dbo].[KEHOACHNHAPVATTU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KETCHUYENTAIKHOAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KETCHUYENTAIKHOAN]') AND type IN ('U'))
	DROP TABLE [dbo].[KETCHUYENTAIKHOAN]
GO

CREATE TABLE [dbo].[KETCHUYENTAIKHOAN] (
  [NGAYCT] smalldatetime  NULL,
  [STTT] int  NULL,
  [SHTK] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [DIENGIAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [SOTIENUSD] float(53)  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [CHUNGTUGHISO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [NOIDUNG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOANTHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[KETCHUYENTAIKHOAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KHOADULIEULUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KHOADULIEULUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[KHOADULIEULUONG]
GO

CREATE TABLE [dbo].[KHOADULIEULUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL
)
GO

ALTER TABLE [dbo].[KHOADULIEULUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KHOAMO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KHOAMO]') AND type IN ('U'))
	DROP TABLE [dbo].[KHOAMO]
GO

CREATE TABLE [dbo].[KHOAMO] (
  [THANG] nvarchar(7) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAMO] nvarchar(7) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MATKHAU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [NGAYKHOAMO] smalldatetime  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAMOLUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[KHOAMO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for KHOAMOBANGLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[KHOAMOBANGLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[KHOAMOBANGLUONG]
GO

CREATE TABLE [dbo].[KHOAMOBANGLUONG] (
  [THANG] nvarchar(7) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAMO] nvarchar(7) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MATKHAU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [NGAYKHOAMO] smalldatetime  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAMOLUONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[KHOAMOBANGLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LISTBANGLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTBANGLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTBANGLUONG]
GO

CREATE TABLE [dbo].[LISTBANGLUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [TENBANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LIST] nvarchar(2099) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIEUKIEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[LISTBANGLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LISTBAOCAO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTBAOCAO]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTBAOCAO]
GO

CREATE TABLE [dbo].[LISTBAOCAO] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [LOAIBAOCAO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [THAMSO] int  NULL
)
GO

ALTER TABLE [dbo].[LISTBAOCAO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LISTCHUYENKHOBAC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTCHUYENKHOBAC]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTCHUYENKHOBAC]
GO

CREATE TABLE [dbo].[LISTCHUYENKHOBAC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASO] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBAOCAO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[LISTCHUYENKHOBAC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LISTDANHMUCNGANHANGKHOBAC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTDANHMUCNGANHANGKHOBAC]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTDANHMUCNGANHANGKHOBAC]
GO

CREATE TABLE [dbo].[LISTDANHMUCNGANHANGKHOBAC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASO] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGANHANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMATTXT] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[LISTDANHMUCNGANHANGKHOBAC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LISTDATAs
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTDATAs]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTDATAs]
GO

CREATE TABLE [dbo].[LISTDATAs] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [IDDONVICAPDUOI] int  NOT NULL,
  [TENBANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [DANHSACHTRUONG] nvarchar(1200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TITLE] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ALLOWPAGING] bit  NULL,
  [PAGESIZE] int  NULL,
  [SHOWPAGESIZE] bit  NULL,
  [ALLOWFILTER] bit  NULL,
  [GRIDPKEY] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ENABLEROWNUM] bit  NULL,
  [ENABLECHECKBOX] bit  NULL,
  [ROWTEMPLATE] nvarchar(2300) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOKYTUMA] int  NULL,
  [ORDERBY] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FITLASTCOLUMN] bit  NULL,
  [TOPDATA] int  NULL,
  [ENTERSEARCH] bit  NULL,
  [LISTFLDTIM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTTXTTIM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LISTWDTIM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHIEUCAO] int  NULL,
  [CHIEURONG] int  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ALTERROWBACKCOLOR] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[LISTDATAs] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'Dùng cho Hiện list các form khác nhau',
'SCHEMA', N'dbo',
'TABLE', N'LISTDATAs',
'COLUMN', N'LOAIPHIEU'
GO


-- ----------------------------
-- Table structure for LISTFIELDGRIDS
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTFIELDGRIDS]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTFIELDGRIDS]
GO

CREATE TABLE [dbo].[LISTFIELDGRIDS] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [TENBANG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTRUONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KIEUDULIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCOT] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHIEURONG] int  NULL,
  [HIENTHI] bit  NULL,
  [FORMAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TEMPLATE] nvarchar(1500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHOWSUMEXCEL] bit  NULL,
  [IDDONVICAPDUOI] int  NULL,
  [LISTGROUP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FONTS] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [COLORS] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BACKCOLORS] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHOWBUTTON] bit  NULL,
  [LACONGTHUC] bit  NULL,
  [CONGTHUC] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DUOCPHEPSUANHANH] bit  NULL,
  [STT] int  NULL,
  [READONLY] bit  NULL,
  [LOCK] bit  NULL
)
GO

ALTER TABLE [dbo].[LISTFIELDGRIDS] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LISTFIELDS
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTFIELDS]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTFIELDS]
GO

CREATE TABLE [dbo].[LISTFIELDS] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LIST] nvarchar(2099) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TABLENAME] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[LISTFIELDS] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LISTINPHIEU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTINPHIEU]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTINPHIEU]
GO

CREATE TABLE [dbo].[LISTINPHIEU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBAOCAO] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTEP] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THAMSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [IMAGEFILE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVICAPDUOI] int  NULL,
  [LOAI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUONGTRUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONTHANHTOAN] bit  NULL,
  [EXCELNEW] int  NULL
)
GO

ALTER TABLE [dbo].[LISTINPHIEU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LISTLOAILUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTLOAILUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTLOAILUONG]
GO

CREATE TABLE [dbo].[LISTLOAILUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [CHITIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[LISTLOAILUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LISTUNC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LISTUNC]') AND type IN ('U'))
	DROP TABLE [dbo].[LISTUNC]
GO

CREATE TABLE [dbo].[LISTUNC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBAOCAO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTEP] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THAMSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [IMAGEFILE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[LISTUNC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUBAOCAOHDSXKD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUBAOCAOHDSXKD]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUBAOCAOHDSXKD]
GO

CREATE TABLE [dbo].[MAUBAOCAOHDSXKD] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUC] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHCHAT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEUEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUYETMINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SELECTTEDDAYDU] bit  NULL,
  [SELECTTEDTOMLUOC] bit  NULL,
  [GHICHU] nchar(150) COLLATE Finnish_Swedish_CI_AS  NULL,
  [FORMATTXT] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAUNAM] float(53)  NULL,
  [MDONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[MAUBAOCAOHDSXKD] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUBAOCAOLCTTGT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUBAOCAOLCTTGT]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUBAOCAOLCTTGT]
GO

CREATE TABLE [dbo].[MAUBAOCAOLCTTGT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUC] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHCHAT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEUEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUYETMINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SELECTTEDDAYDU] bit  NULL,
  [SELECTTEDTOMLUOC] bit  NULL,
  [GHICHU] nchar(150) COLLATE Finnish_Swedish_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAUNAM] float(53)  NULL
)
GO

ALTER TABLE [dbo].[MAUBAOCAOLCTTGT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUBAOCAOLCTTTT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUBAOCAOLCTTTT]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUBAOCAOLCTTTT]
GO

CREATE TABLE [dbo].[MAUBAOCAOLCTTTT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUC] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHCHAT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEUEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUYETMINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SELECTTEDDAYDU] bit  NULL,
  [SELECTTEDTOMLUOC] bit  NULL,
  [GHICHU] nchar(150) COLLATE Finnish_Swedish_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAUNAM] float(53)  NULL,
  [MDONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[MAUBAOCAOLCTTTT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUBAOCAOQTKPHD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUBAOCAOQTKPHD]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUBAOCAOQTKPHD]
GO

CREATE TABLE [dbo].[MAUBAOCAOQTKPHD] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUC] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHCHAT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEUEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUYETMINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SELECTTEDDAYDU] bit  NULL,
  [SELECTTEDTOMLUOC] bit  NULL,
  [GHICHU] nchar(150) COLLATE Finnish_Swedish_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[MAUBAOCAOQTKPHD] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUBAOCAOTAICHINH
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUBAOCAOTAICHINH]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUBAOCAOTAICHINH]
GO

CREATE TABLE [dbo].[MAUBAOCAOTAICHINH] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUC] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHCHAT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEUEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUYETMINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SELECTTEDDAYDU] bit  NULL,
  [SELECTTEDTOMLUOC] bit  NULL,
  [GHICHU] nchar(150) COLLATE Finnish_Swedish_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAUNAM] float(53)  NULL
)
GO

ALTER TABLE [dbo].[MAUBAOCAOTAICHINH] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUBAOCAOTAICHINH99
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUBAOCAOTAICHINH99]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUBAOCAOTAICHINH99]
GO

CREATE TABLE [dbo].[MAUBAOCAOTAICHINH99] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUC] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHCHAT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEUEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUYETMINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SELECTTEDDAYDU] bit  NULL,
  [SELECTTEDTOMLUOC] bit  NULL,
  [GHICHU] nchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAUNAM] float(53)  NULL
)
GO

ALTER TABLE [dbo].[MAUBAOCAOTAICHINH99] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUBAOCAOTONGHOPTHUCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUBAOCAOTONGHOPTHUCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUBAOCAOTONGHOPTHUCHI]
GO

CREATE TABLE [dbo].[MAUBAOCAOTONGHOPTHUCHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STTA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUCTINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[MAUBAOCAOTONGHOPTHUCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUBIEU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUBIEU]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUBIEU]
GO

CREATE TABLE [dbo].[MAUBIEU] (
  [STT] int  NULL,
  [TENMAUBIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTED] bit DEFAULT 0 NULL
)
GO

ALTER TABLE [dbo].[MAUBIEU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUCHIKINHPHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUCHIKINHPHI]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUCHIKINHPHI]
GO

CREATE TABLE [dbo].[MAUCHIKINHPHI] (
  [STT] int  NULL,
  [CHITIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUCTINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIHD] bit DEFAULT 0 NULL
)
GO

ALTER TABLE [dbo].[MAUCHIKINHPHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUCHUONGTRINHDUAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUCHUONGTRINHDUAN]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUCHUONGTRINHDUAN]
GO

CREATE TABLE [dbo].[MAUCHUONGTRINHDUAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUC] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHCHAT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEUEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUYETMINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SELECTTEDDAYDU] bit  NULL,
  [SELECTTEDTOMLUOC] bit  NULL,
  [GHICHU] nchar(150) COLLATE Finnish_Swedish_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[MAUCHUONGTRINHDUAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUHOATDONGCOTHU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUHOATDONGCOTHU]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUHOATDONGCOTHU]
GO

CREATE TABLE [dbo].[MAUHOATDONGCOTHU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUCTINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[MAUHOATDONGCOTHU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MAUTHUYETMINHBAOCAO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MAUTHUYETMINHBAOCAO]') AND type IN ('U'))
	DROP TABLE [dbo].[MAUTHUYETMINHBAOCAO]
GO

CREATE TABLE [dbo].[MAUTHUYETMINHBAOCAO] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MS] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] int  NULL,
  [CONGTHUC] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TINHCHAT] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODAUNAM] float(53)  NULL,
  [SOCUOIKY] float(53)  NULL,
  [CHITIEUEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [THUYETMINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [SELECTTEDDAYDU] bit  NULL,
  [SELECTTEDTOMLUOC] bit  NULL,
  [GHICHU] nchar(150) COLLATE Finnish_Swedish_CI_AS  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[MAUTHUYETMINHBAOCAO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MUCDICHSUDUNGVATTU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MUCDICHSUDUNGVATTU]') AND type IN ('U'))
	DROP TABLE [dbo].[MUCDICHSUDUNGVATTU]
GO

CREATE TABLE [dbo].[MUCDICHSUDUNGVATTU] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUCDICH] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[MUCDICHSUDUNGVATTU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MUCLUCNGANSACH
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MUCLUCNGANSACH]') AND type IN ('U'))
	DROP TABLE [dbo].[MUCLUCNGANSACH]
GO

CREATE TABLE [dbo].[MUCLUCNGANSACH] (
  [NHOM] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUNHOM] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUCCU] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUCCU] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [NHOMMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEUNHOM] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [BATBUOC] bit  NULL
)
GO

ALTER TABLE [dbo].[MUCLUCNGANSACH] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for MUCLUCNGANSACH2009
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MUCLUCNGANSACH2009]') AND type IN ('U'))
	DROP TABLE [dbo].[MUCLUCNGANSACH2009]
GO

CREATE TABLE [dbo].[MUCLUCNGANSACH2009] (
  [NHOM] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUNHOM] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUCCU] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUCCU] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [NHOMMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[MUCLUCNGANSACH2009] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NGANHANG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NGANHANG]') AND type IN ('U'))
	DROP TABLE [dbo].[NGANHANG]
GO

CREATE TABLE [dbo].[NGANHANG] (
  [TENNGANHANG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGANHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTAIKHOAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGANHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NGANHANG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NGUONKINHPHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NGUONKINHPHI]') AND type IN ('U'))
	DROP TABLE [dbo].[NGUONKINHPHI]
GO

CREATE TABLE [dbo].[NGUONKINHPHI] (
  [STT] int  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [NGUONKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUONGLOAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit DEFAULT 0 NULL,
  [SODUDAUNAM] float(53)  NULL,
  [NGUONKINHPHIE] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [SELECTIN] bit DEFAULT 0 NULL,
  [SHTKOK] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDUNG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NGUONKINHPHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NHAPSANPHAM
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NHAPSANPHAM]') AND type IN ('U'))
	DROP TABLE [dbo].[NHAPSANPHAM]
GO

CREATE TABLE [dbo].[NHAPSANPHAM] (
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYCT] smalldatetime  NULL,
  [DIENGIAI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BANGCHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[NHAPSANPHAM] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NHAPXUAT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NHAPXUAT]') AND type IN ('U'))
	DROP TABLE [dbo].[NHAPXUAT]
GO

CREATE TABLE [dbo].[NHAPXUAT] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [PHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [NGAYCT] smalldatetime  NULL,
  [NGAYGHI] smalldatetime  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUNGTUGHISO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NOIDUNG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADOITUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDOITUONG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHIDOITUONG] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEMTHEO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAINO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL,
  [MALYDO] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENLYDO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETCHUYEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOTHUE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HANTHANHTOAN] smalldatetime  NULL,
  [MAGIAONHAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHIGIAONHAN] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENGIAONHAN] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [USERNAME] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTAO] smalldatetime  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AUTOTYGIA] bit  NULL,
  [CHONIN] bit  NULL,
  [LOAITAMUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATDONGIA] bit DEFAULT 0 NULL,
  [VAOBANGKE] bit  NULL,
  [MAKHO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOPDONG] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOPDONG] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HUY] bit  NULL,
  [MAPHONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDOK] int  NULL,
  [TYLEVAT] float(53)  NULL,
  [HINHTHUCTT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITHUE] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MANGUOIMUA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENNGUOIMUA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHINGUOIMUA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIA] float(53)  NULL,
  [MASOKIEMTRAHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAUHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAIOK] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCONTAINER] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHUONGTIEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHINHANHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOVANDON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUOIGIAOHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHIGIAOHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTUTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEUMISA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOCHUNGTUXUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLECK] float(53)  NULL,
  [MASOTHUENGUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTEDIN] bit  NULL,
  [MADOITUONG1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDOITUONG1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN1] nvarchar(80) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HINHTHUCTHAU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOQUYETDINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [VAOSO] bit  NULL,
  [LOCKED] bit  NULL,
  [TRANGTHAI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EHOADON] bit  NULL,
  [EHOADONGUID] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [RESULT] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EMAIL] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CREATEDATEBL] datetime DEFAULT getdate() NOT NULL,
  [NGAYLAPBIENLAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [FILENAME_PDF] nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CODEGETFILE] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAUSOHOADON] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NHAPXUAT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NHATKY
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NHATKY]') AND type IN ('U'))
	DROP TABLE [dbo].[NHATKY]
GO

CREATE TABLE [dbo].[NHATKY] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [NGAY] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTEN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCNANG] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEUNEW] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NHATKY] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NHOMMUC
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NHOMMUC]') AND type IN ('U'))
	DROP TABLE [dbo].[NHOMMUC]
GO

CREATE TABLE [dbo].[NHOMMUC] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [NHOMMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAIKHOANMUC] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NHOMMUC] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGBIENLAI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGBIENLAI]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGBIENLAI]
GO

CREATE TABLE [dbo].[NOIDUNGBIENLAI] (
  [IDID] int  IDENTITY(1,1) NOT NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOSO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOSO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGBIENLAI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGDIEUCHUYEN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGDIEUCHUYEN]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGDIEUCHUYEN]
GO

CREATE TABLE [dbo].[NOIDUNGDIEUCHUYEN] (
  [IDID] int  IDENTITY(1,1) NOT NULL,
  [LOAIPHIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGHOA] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGTON] numeric(18,4) DEFAULT 0 NULL,
  [SOLUONG] numeric(18,4) DEFAULT 0 NULL,
  [DONGIA] numeric(18,4) DEFAULT 0 NULL,
  [DONVITINH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] numeric(18,4) DEFAULT 0 NULL,
  [SOTIENUSD] numeric(18,4) DEFAULT 0 NULL,
  [DONGIAUSD] numeric(18,4) DEFAULT 0 NULL,
  [TKNO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HANSUDUNG] datetime  NULL,
  [KETCHUYEN1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIAVON] float(53)  NULL,
  [GIAVONUSD] float(53)  NULL,
  [TKNGOAINO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHANBO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHANBO] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [COMMENT] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATGIAVON] bit DEFAULT 0 NULL,
  [DONGIAXUAT] float(53)  NULL,
  [SOTIENXUAT] float(53)  NULL,
  [DONGIAXUATUSD] float(53)  NULL,
  [SOTIENXUATUSD] float(53)  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGDIEUCHUYEN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGHMKP
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGHMKP]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGHMKP]
GO

CREATE TABLE [dbo].[NOIDUNGHMKP] (
  [IDID] int  IDENTITY(1,1) NOT NULL,
  [LOAIPHIEU] nvarchar(25) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [KHOAN] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KINHPHIOK] bit DEFAULT 0 NULL,
  [SOTIENUSD] float(53)  NULL,
  [TYGIA] float(53)  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIAEU] float(53)  NULL,
  [SOTIENEU] float(53)  NULL,
  [TENMUCTIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMUCTIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAMUNG] bit  NULL,
  [SOLUONG] float(53)  NULL,
  [TKNGOAINO] nvarchar(80) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(80) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHDSXKD] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(230) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(320) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOATDONG] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(530) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHANGMUC] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(530) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGHMKP] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGKHOANCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGKHOANCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGKHOANCHI]
GO

CREATE TABLE [dbo].[NOIDUNGKHOANCHI] (
  [STT] int  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGKHOANCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGNHAPXUAT
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGNHAPXUAT]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGNHAPXUAT]
GO

CREATE TABLE [dbo].[NOIDUNGNHAPXUAT] (
  [IDID] int  IDENTITY(1,1) NOT NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGHOA] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [SOTIENVND] float(53)  NULL,
  [SOTIENUSD] float(53)  NULL,
  [TYGIA] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(355) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(355) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HANSUDUNG] datetime  NULL,
  [MUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(455) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETCHUYEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONGTON] float(53)  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENXUAT] float(53)  NULL,
  [DONGIAXUAT] float(53)  NULL,
  [SOTIENXUATUSD] float(53)  NULL,
  [MACONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAINO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATTONKHO] int  NULL,
  [MAKHO] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENEU] float(53)  NULL,
  [TYGIAEU] float(53)  NULL,
  [KINHPHIGIAM] bit  NULL,
  [KINHPHIOK] bit  NULL,
  [MAHDSXKD] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUTOCKY] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIAUSD] float(53)  NULL,
  [MAMUCTIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIAOK] float(53)  NULL,
  [SOTIENOK] float(53)  NULL,
  [IDOK] int  NULL,
  [LOHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEUMISA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LPHIEUX] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOHANGSANPHAM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASANPHAM] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENSANPHAM] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUYCACH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYSX] smalldatetime  NULL,
  [NHATKY] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDMAVATTUPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LPHIEUVATTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYHOADON] smalldatetime  NULL,
  [SOHOADON] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENTHUE] float(53)  NULL,
  [MAKHOANMUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANMUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG1] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NUOCSANXUAT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGNHAPXUAT] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGPHANI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGPHANI]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGPHANI]
GO

CREATE TABLE [dbo].[NOIDUNGPHANI] (
  [SELECTTED] bit DEFAULT 0 NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(5) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CONGTHUCTINH] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGPHANI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGRUTVON
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGRUTVON]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGRUTVON]
GO

CREATE TABLE [dbo].[NOIDUNGRUTVON] (
  [IDID] int  IDENTITY(1,1) NOT NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAMUNG] bit DEFAULT 0 NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAINO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIA] float(53)  NULL,
  [SOTIENUSD] float(53)  NULL,
  [TYLE] float(53)  NULL,
  [NAMHOATDONG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGRUTVON] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGSANPHAM
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGSANPHAM]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGSANPHAM]
GO

CREATE TABLE [dbo].[NOIDUNGSANPHAM] (
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTT] smallint  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [SOPHIEUNHAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEUNHAP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDID] int  IDENTITY(1,1) NOT NULL,
  [NHOMHANGHOA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGSANPHAM] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGTANGGIAM
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGTANGGIAM]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGTANGGIAM]
GO

CREATE TABLE [dbo].[NOIDUNGTANGGIAM] (
  [IDID] int  IDENTITY(1,1) NOT NULL,
  [LOAIPHIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGHOA] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [DONGIA] float(53)  NULL,
  [NGAYSUDUNG] smalldatetime  NULL,
  [SOTIENVND] float(53)  NULL,
  [NGUYENGIATSCD] float(53)  NULL,
  [GIATRICONLAI] float(53)  NULL,
  [SOHAOMONDATRICH] float(53)  NULL,
  [CHIPHI] float(53)  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DAGHISO] bit  NULL,
  [VAOSO] float(53)  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASUDUNG] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENSUDUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGTANGGIAM] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOIDUNGTHUCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOIDUNGTHUCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[NOIDUNGTHUCHI]
GO

CREATE TABLE [dbo].[NOIDUNGTHUCHI] (
  [IDID] int  IDENTITY(1,1) NOT NULL,
  [LOAIPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIENGIAI] nvarchar(2000) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [SOTIENUSD] float(53)  NULL,
  [TYGIA] float(53)  NULL,
  [TKNO] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATLUONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(855) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DIACHI1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI1] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KETCHUYEN1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(1350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHOMMUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTHANHPHAM] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MATHANHPHAM] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CAPNHATVON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TAMUNG] bit DEFAULT 0 NOT NULL,
  [MAHOATDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KINHPHIGIAM] bit DEFAULT 0 NULL,
  [TKNGOAINO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKNGOAICO] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYLE] float(53)  NULL,
  [NAMHOATDONG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(1350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHANGMUC] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DARUTOK] bit DEFAULT 0 NOT NULL,
  [MASOHOPDONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHIEUTOCKY] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TYGIAEU] float(53)  NULL,
  [SOTIENEU] float(53)  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOXACNHANVIENTRO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYXACNHAN] smalldatetime  NULL,
  [MAHDSXKD] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOXACNHANVIENTRO1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYXACNHAN1] smalldatetime  NULL,
  [LUONGPHUCAPDACTHU] float(53)  NULL,
  [KINHPHIKHOIPHUC] bit  NULL,
  [DIENGIAIEN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MABANGKE] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMUCTIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(350) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GIATRUNGTHAU] float(53)  NULL,
  [LUYKECUOI] float(53)  NULL,
  [SODUTAMUNG] float(53)  NULL,
  [SOTIENBAOHANH] float(53)  NULL,
  [SOTIENTHUE] float(53)  NULL,
  [SOTIENTRUOCTHUE] float(53)  NULL,
  [MUC1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOADON] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHIUTHUE] bit DEFAULT 0 NOT NULL,
  [NGAYHOADON] smalldatetime  NULL,
  [IDOK] int  NULL,
  [MAPHONG] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOLUONG] float(53)  NULL,
  [CAMKETCHI] bit DEFAULT 0 NOT NULL,
  [SOPHIEUTHUCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [LOAIPHIEUMISA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUCTIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITAMUNG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHATKY] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DATHANHTOAN] bit  NULL,
  [FORMATTXT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [MAKHOANMUC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANMUC] nvarchar(750) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANMUC1] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANMUC1] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MABANGKEGTGC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HINHTHUCTT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGTGC] smalldatetime  NULL,
  [MASOHOPDONG1] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG1] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HACHTOANDONGTHOI] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [QUYETTOAN] bit  NULL,
  [NAMKEHOACH] int  NULL,
  [HANTHANHTOAN] smalldatetime  NULL,
  [MACONGTRINH1] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH1] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEUANA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NOIDUNGTHUCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NOPTIEN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NOPTIEN]') AND type IN ('U'))
	DROP TABLE [dbo].[NOPTIEN]
GO

CREATE TABLE [dbo].[NOPTIEN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SELECTTED] bit  NULL,
  [SOPHIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYNOP] date  NULL,
  [MSSV] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENSV] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KYHOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKHOANTHU] int  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMONHOC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMONHOC] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENNOP] numeric(18,2)  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MALOP] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIPHIEUANA] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DATAO] bit  NULL
)
GO

ALTER TABLE [dbo].[NOPTIEN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for NVU_HCSN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[NVU_HCSN]') AND type IN ('U'))
	DROP TABLE [dbo].[NVU_HCSN]
GO

CREATE TABLE [dbo].[NVU_HCSN] (
  [ID] int  NULL,
  [NAM] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MA] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DVBC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MaC] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SoCNVC] int  NULL,
  [TrongDo] int  NULL,
  [TangTN] float(53)  NULL,
  [GiamTN] float(53)  NULL,
  [TongQL] float(53)  NULL,
  [LHD] float(53)  NULL,
  [NVCB] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PHATSINH] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BIENDONG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NHANXET] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPTREN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANHTHUTRUONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENTHUTRUONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANHKETOAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCDANHLAPBIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYTHANGLB] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENNGUOILAPBIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOTENKETOANTRUONG] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[NVU_HCSN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for OPTIONANA
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[OPTIONANA]') AND type IN ('U'))
	DROP TABLE [dbo].[OPTIONANA]
GO

CREATE TABLE [dbo].[OPTIONANA] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCNANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVICAPDUOI] int  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONOK] bit  NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[OPTIONANA] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for OPTIONANAUSERS
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[OPTIONANAUSERS]') AND type IN ('U'))
	DROP TABLE [dbo].[OPTIONANAUSERS]
GO

CREATE TABLE [dbo].[OPTIONANAUSERS] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [LOAI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCNANG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [USERNAME] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IDDONVICAPDUOI] int  NULL,
  [IDUSER] int  NULL,
  [GHICHU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHONOK] bit  NULL
)
GO

ALTER TABLE [dbo].[OPTIONANAUSERS] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for PATHAUTOUPDATE
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[PATHAUTOUPDATE]') AND type IN ('U'))
	DROP TABLE [dbo].[PATHAUTOUPDATE]
GO

CREATE TABLE [dbo].[PATHAUTOUPDATE] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HOST] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [USERNAME] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PASSWORD] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PATHAUTOUPDATE] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AUTOUPDATE] bit  NULL
)
GO

ALTER TABLE [dbo].[PATHAUTOUPDATE] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for PATHWEB
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[PATHWEB]') AND type IN ('U'))
	DROP TABLE [dbo].[PATHWEB]
GO

CREATE TABLE [dbo].[PATHWEB] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] smallint  NULL,
  [PATHWEB] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL
)
GO

ALTER TABLE [dbo].[PATHWEB] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for PHANBOHOATDONGCOTHU
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[PHANBOHOATDONGCOTHU]') AND type IN ('U'))
	DROP TABLE [dbo].[PHANBOHOATDONGCOTHU]
GO

CREATE TABLE [dbo].[PHANBOHOATDONGCOTHU] (
  [LOAIHOATDONG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [QUY] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUYKE] float(53)  NULL
)
GO

ALTER TABLE [dbo].[PHANBOHOATDONGCOTHU] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for PHANQUYEN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[PHANQUYEN]') AND type IN ('U'))
	DROP TABLE [dbo].[PHANQUYEN]
GO

CREATE TABLE [dbo].[PHANQUYEN] (
  [CHUCNANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] float(53)  NULL,
  [QUYENSUDUNG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[PHANQUYEN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for PHANQUYENBANGLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[PHANQUYENBANGLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[PHANQUYENBANGLUONG]
GO

CREATE TABLE [dbo].[PHANQUYENBANGLUONG] (
  [ID] int  NULL,
  [CHUCNANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] float(53)  NULL,
  [QUYENSUDUNG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[PHANQUYENBANGLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for PHIEUTHUCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[PHIEUTHUCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[PHIEUTHUCHI]
GO

CREATE TABLE [dbo].[PHIEUTHUCHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [SOCHUNGTU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [TKNO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TKCO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[PHIEUTHUCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for QUYENBAOCAOLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[QUYENBAOCAOLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[QUYENBAOCAOLUONG]
GO

CREATE TABLE [dbo].[QUYENBAOCAOLUONG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [LOAIBAOCAO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [USERNAME] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MABAOCAO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBAOCAO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[QUYENBAOCAOLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for QUYENSUDUNG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[QUYENSUDUNG]') AND type IN ('U'))
	DROP TABLE [dbo].[QUYENSUDUNG]
GO

CREATE TABLE [dbo].[QUYENSUDUNG] (
  [HOTEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCNANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ENABLE] bit  NULL,
  [QUYENSUDUNG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SELECTTED] bit  NULL
)
GO

ALTER TABLE [dbo].[QUYENSUDUNG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for QUYENSUDUNGBANGLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[QUYENSUDUNGBANGLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[QUYENSUDUNGBANGLUONG]
GO

CREATE TABLE [dbo].[QUYENSUDUNGBANGLUONG] (
  [HOTEN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUCNANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ENABLE] bit  NULL,
  [QUYENSUDUNG] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] int  NULL,
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL
)
GO

ALTER TABLE [dbo].[QUYENSUDUNGBANGLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for QUYENSUDUNGSOSACHBAOCAO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[QUYENSUDUNGSOSACHBAOCAO]') AND type IN ('U'))
	DROP TABLE [dbo].[QUYENSUDUNGSOSACHBAOCAO]
GO

CREATE TABLE [dbo].[QUYENSUDUNGSOSACHBAOCAO] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [LOAIBAOCAO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [USERNAME] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MABAOCAO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENBAOCAO] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL
)
GO

ALTER TABLE [dbo].[QUYENSUDUNGSOSACHBAOCAO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for QUYENSUDUNGTAIKHOAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[QUYENSUDUNGTAIKHOAN]') AND type IN ('U'))
	DROP TABLE [dbo].[QUYENSUDUNGTAIKHOAN]
GO

CREATE TABLE [dbo].[QUYENSUDUNGTAIKHOAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [USERNAME] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENTAIKHOAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SELECTTED] bit DEFAULT 0 NULL
)
GO

ALTER TABLE [dbo].[QUYENSUDUNGTAIKHOAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for QUYLUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[QUYLUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[QUYLUONG]
GO

CREATE TABLE [dbo].[QUYLUONG] (
  [MASO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [STT] smallint  NULL,
  [MUCKHOAN] float(53)  NULL,
  [THUCHIEN] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[QUYLUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SEE
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SEE]') AND type IN ('U'))
	DROP TABLE [dbo].[SEE]
GO

CREATE TABLE [dbo].[SEE] (
  [STT] int  NULL,
  [GHICHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SEE] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUBANGDOICHIEUTAMUNG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUBANGDOICHIEUTAMUNG]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUBANGDOICHIEUTAMUNG]
GO

CREATE TABLE [dbo].[SODUBANGDOICHIEUTAMUNG] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIENVND] float(53)  NULL,
  [NHOMMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KINHPHIOK] bit DEFAULT 0 NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITAMUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMUCTIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHDSXKD] nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(230) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(230) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUBANGDOICHIEUTAMUNG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUCAMKETCHI
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUCAMKETCHI]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUCAMKETCHI]
GO

CREATE TABLE [dbo].[SODUCAMKETCHI] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODUCAMKETCHI] float(53)  NULL,
  [CHITIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KINHPHIOK] bit  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITAMUNG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOBAC] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAMUCTIEU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUCAMKETCHI] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUCHITIETDOITUONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUCHITIETDOITUONG]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUCHITIETDOITUONG]
GO

CREATE TABLE [dbo].[SODUCHITIETDOITUONG] (
  [SODUNO] float(53)  NULL,
  [SODUCO] float(53)  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUYKENO] float(53)  NULL,
  [LUYKECO] float(53)  NULL,
  [SODUNONT] float(53)  NULL,
  [SODUCONT] float(53)  NULL,
  [LUYKEKHOIDAUNO] float(53)  NULL,
  [LUYKEKHOIDAUCO] float(53)  NULL,
  [SOTIENEU] float(53)  NULL,
  [SODUNOEU] float(53)  NULL,
  [SODUCOEU] float(53)  NULL,
  [LUYKENOEU] float(53)  NULL,
  [LUYKECOEU] float(53)  NULL,
  [LUYKEKHOIDAUNOEU] float(53)  NULL,
  [LUYKEKHOIDAUCOEU] float(53)  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITIEN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MS] int  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(855) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUONGLOAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVIDUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVIDUAN] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MACONGTRINH] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENCONGTRINH] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUYKEKHOIDAUNOUSD] float(53)  NULL,
  [LUYKEKHOIDAUCOUSD] float(53)  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [MAMUCTIEU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENMUCTIEU] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TIEUMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit  NULL,
  [DIEUCHINH] bit  NULL,
  [MAHDSXKD] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHDSXKD] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANCHI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANCHI] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANTHU] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANTHU] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOPDONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGAYGHOADON] smalldatetime  NULL,
  [LOAIKINHPHI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOPDONG] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOHOADON] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAVAYHD] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENVAYHD] nvarchar(220) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHOANMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHOANMUC] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTKCU] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAPHONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENPHONG] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [GHICHU] nvarchar(850) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MABANGKE] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NAM] int  NULL
)
GO

ALTER TABLE [dbo].[SODUCHITIETDOITUONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA01
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA01]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA01]
GO

CREATE TABLE [dbo].[SODUHANGHOA01] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(550) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOHANG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [HANSUDUNG] smalldatetime  NULL,
  [NAM] int  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA01] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA02
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA02]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA02]
GO

CREATE TABLE [dbo].[SODUHANGHOA02] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA02] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA03
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA03]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA03]
GO

CREATE TABLE [dbo].[SODUHANGHOA03] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA03] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA04
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA04]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA04]
GO

CREATE TABLE [dbo].[SODUHANGHOA04] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA04] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA05
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA05]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA05]
GO

CREATE TABLE [dbo].[SODUHANGHOA05] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA05] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA06
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA06]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA06]
GO

CREATE TABLE [dbo].[SODUHANGHOA06] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA06] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA07
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA07]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA07]
GO

CREATE TABLE [dbo].[SODUHANGHOA07] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA07] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA08
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA08]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA08]
GO

CREATE TABLE [dbo].[SODUHANGHOA08] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA08] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA09
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA09]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA09]
GO

CREATE TABLE [dbo].[SODUHANGHOA09] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA09] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA10
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA10]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA10]
GO

CREATE TABLE [dbo].[SODUHANGHOA10] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA10] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA11
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA11]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA11]
GO

CREATE TABLE [dbo].[SODUHANGHOA11] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA11] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA12
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA12]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA12]
GO

CREATE TABLE [dbo].[SODUHANGHOA12] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA12] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGHOA13
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGHOA13]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGHOA13]
GO

CREATE TABLE [dbo].[SODUHANGHOA13] (
  [STT] int  NULL,
  [MAHANGHOA] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGHOA] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENKHO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONVITINH] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DONGIA] float(53)  NULL,
  [SOLUONG] float(53)  NULL,
  [SOTIEN] float(53)  NULL,
  [LOAIHANG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ID] int  IDENTITY(1,1) NOT NULL,
  [SOTIENUSD] float(53)  NULL,
  [DONGIAUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGHOA13] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHANGMUCTAITRO
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHANGMUCTAITRO]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHANGMUCTAITRO]
GO

CREATE TABLE [dbo].[SODUHANGMUCTAITRO] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MASOHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHANGMUC] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODUDAUNAMNO] float(53)  NULL,
  [SODUDAUNAMCO] float(53)  NULL,
  [LUYKEKHOIDAUNO] float(53)  NULL,
  [LUYKEKHOIDAUCO] float(53)  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH] float(53)  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUYKEKHOIDAUNOUSD] float(53)  NULL,
  [LUYKEKHOIDAUCOUSD] float(53)  NULL,
  [KEHOACHKHOIDAU] float(53)  NULL,
  [QUYI] float(53)  NULL,
  [QUYII] float(53)  NULL,
  [QUYIII] float(53)  NULL,
  [QUYIV] float(53)  NULL,
  [KEHOACHKHOIDAUUSD] float(53)  NULL,
  [KEHOACHUSD] float(53)  NULL,
  [QUYIUSD] float(53)  NULL,
  [QUYIIUSD] float(53)  NULL,
  [QUYIIIUSD] float(53)  NULL,
  [QUYIVUSD] float(53)  NULL,
  [BATBUOC] bit DEFAULT 0 NULL,
  [SODUDAUNAMNOUSD] float(53)  NULL,
  [SODUDAUNAMCOUSD] float(53)  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHANGMUCTAITRO] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHMKP
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHMKP]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHMKP]
GO

CREATE TABLE [dbo].[SODUHMKP] (
  [MAKP] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MUC] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODU] float(53)  NULL,
  [CHUONGLOAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHITIEU] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LOAITHONGBAO] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHMKP] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHOATDONG
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHOATDONG]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHOATDONG]
GO

CREATE TABLE [dbo].[SODUHOATDONG] (
  [LOAIHOATDONG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SOTIEN] float(53)  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHOATDONG] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHOATDONGDUAN
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHOATDONGDUAN]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHOATDONGDUAN]
GO

CREATE TABLE [dbo].[SODUHOATDONGDUAN] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVI] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAKP] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [NGUONKINHPHI] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SODUDAUNAMNO] float(53)  NULL,
  [SODUDAUNAMCO] float(53)  NULL,
  [MADUAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDUAN] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUYKEKHOIDAUNO] float(53)  NULL,
  [LUYKEKHOIDAUCO] float(53)  NULL,
  [SHTK] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CHUONGLOAIKHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACH] float(53)  NULL,
  [LOAI] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KHOAN] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MASOHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [LUYKEKHOIDAUNOUSD] float(53)  NULL,
  [LUYKEKHOIDAUCOUSD] float(53)  NULL,
  [KEHOACHKHOIDAU] float(53)  NULL,
  [QUYI] float(53)  NULL,
  [QUYII] float(53)  NULL,
  [QUYIII] float(53)  NULL,
  [QUYIV] float(53)  NULL,
  [KEHOACHKHOIDAUUSD] float(53)  NULL,
  [KEHOACHUSD] float(53)  NULL,
  [QUYIUSD] float(53)  NULL,
  [QUYIIUSD] float(53)  NULL,
  [QUYIIIUSD] float(53)  NULL,
  [QUYIVUSD] float(53)  NULL,
  [NAMHOATDONG] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BATBUOC] bit DEFAULT 0 NULL,
  [SODUDAUNAMNOUSD] float(53)  NULL,
  [SODUDAUNAMCOUSD] float(53)  NULL,
  [MASOHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MAHANGMUC] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENDONVICAPDUOI] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [KEHOACHKHOIDAUEU] float(53)  NULL,
  [KEHOACHEU] float(53)  NULL,
  [LUYKEKHOIDAUNOEU] float(53)  NULL,
  [LUYKEKHOIDAUCOEU] float(53)  NULL,
  [SODUDAUNAMNOEU] float(53)  NULL,
  [SODUDAUNAMCOEU] float(53)  NULL,
  [QUYIEU] float(53)  NULL,
  [QUYIIEU] float(53)  NULL,
  [QUYIIIEU] float(53)  NULL,
  [QUYIVEU] float(53)  NULL,
  [NGANSACH] float(53)  NULL,
  [NGANSACHUSD] float(53)  NULL,
  [NGANSACHEU] float(53)  NULL,
  [TENHANGMUC] nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[SODUHOATDONGDUAN] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for SODUHOATDONGSXKD
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[SODUHOATDONGSXKD]') AND type IN ('U'))
	DROP TABLE [dbo].[SODUHOATDONGSXKD]
GO

CREATE TABLE [dbo].[SODUHOATDONGSXKD] (
  [ID] int  IDENTITY(1,1) NOT NULL,
  [STT] int  NULL,
  [SELECTTED] bit  NULL,
  [MAHOATDONG] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TENHOATDONG] nvarchar(250) COLLATE Finnish_Swedish_CI_AS  NULL,
  [MADONVICAPDUOI] nvarchar(50) COLLATE Finnish_Swedish_CI_AS  NULL,
  [SODU] float(53)  NULL
