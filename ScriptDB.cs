using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_restaurante
{
    internal class ScriptDB
    {
        public static string CrearBaseDatos = @"
            USE [master]
			GO
			/****** Object:  Database [GloriaRestaurant]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'GloriaRestaurant')
			BEGIN
				CREATE DATABASE GloriaRestaurant;
			END
			GO
			IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
			begin
			EXEC [GloriaRestaurant].[dbo].[sp_fulltext_database] @action = 'enable'
			end
			GO
			ALTER DATABASE [GloriaRestaurant] SET ANSI_NULL_DEFAULT OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET ANSI_NULLS OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET ANSI_PADDING OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET ANSI_WARNINGS OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET ARITHABORT OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET AUTO_CLOSE OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET AUTO_SHRINK OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET AUTO_UPDATE_STATISTICS ON 
			GO
			ALTER DATABASE [GloriaRestaurant] SET CURSOR_CLOSE_ON_COMMIT OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET CURSOR_DEFAULT  GLOBAL 
			GO
			ALTER DATABASE [GloriaRestaurant] SET CONCAT_NULL_YIELDS_NULL OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET NUMERIC_ROUNDABORT OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET QUOTED_IDENTIFIER OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET RECURSIVE_TRIGGERS OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET  DISABLE_BROKER 
			GO
			ALTER DATABASE [GloriaRestaurant] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET DATE_CORRELATION_OPTIMIZATION OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET TRUSTWORTHY OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET ALLOW_SNAPSHOT_ISOLATION OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET PARAMETERIZATION SIMPLE 
			GO
			ALTER DATABASE [GloriaRestaurant] SET READ_COMMITTED_SNAPSHOT OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET HONOR_BROKER_PRIORITY OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET RECOVERY FULL 
			GO
			ALTER DATABASE [GloriaRestaurant] SET  MULTI_USER 
			GO
			ALTER DATABASE [GloriaRestaurant] SET PAGE_VERIFY CHECKSUM  
			GO
			ALTER DATABASE [GloriaRestaurant] SET DB_CHAINING OFF 
			GO
			ALTER DATABASE [GloriaRestaurant] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
			GO
			ALTER DATABASE [GloriaRestaurant] SET TARGET_RECOVERY_TIME = 60 SECONDS 
			GO
			ALTER DATABASE [GloriaRestaurant] SET DELAYED_DURABILITY = DISABLED 
			GO
			ALTER DATABASE [GloriaRestaurant] SET ACCELERATED_DATABASE_RECOVERY = OFF  
			GO
			EXEC sys.sp_db_vardecimal_storage_format N'GloriaRestaurant', N'ON'
			GO
			ALTER DATABASE [GloriaRestaurant] SET QUERY_STORE = ON
			GO
			ALTER DATABASE [GloriaRestaurant] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
			GO
			USE [GloriaRestaurant]
			GO
			/****** Object:  User [Usuario]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			CREATE USER [Usuario] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
			GO
			/****** Object:  Table [dbo].[Caja]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Caja](
				[IdCaja] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [text] NOT NULL,
				[Activo] [bit] NOT NULL,
				[Numero] [int] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdCaja] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[CategoriaProducto]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[CategoriaProducto](
				[IdCategoria] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](80) NOT NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdCategoria] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Cliente]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Cliente](
				[IdCliente] [int] IDENTITY(1,1) NOT NULL,
				[IdPersona] [int] NOT NULL,
				[IdTipoDoc] [int] NOT NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdCliente] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Comanda]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Comanda](
				[IdComanda] [int] IDENTITY(1,1) NOT NULL,
				[IdPedido] [int] NOT NULL,
				[IdMesa] [int] NOT NULL,
				[IdProducto] [int] NOT NULL,
				[Cantidad] [decimal](12, 3) NOT NULL,
				[Estado] [varchar](10) NOT NULL,
				[Cuenta] [int] NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdComanda] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Compra]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Compra](
				[IdCompra] [int] IDENTITY(1,1) NOT NULL,
				[Fecha] [datetime2](7) NOT NULL,
				[FechaRecepcion] [datetime2](7) NULL,
				[IdProveedorPersona] [int] NOT NULL,
				[Subtotal] [decimal](12, 2) NOT NULL,
				[Impuestos] [decimal](12, 2) NOT NULL,
				[Total] [decimal](12, 2) NOT NULL,
				[Estado] [varchar](20) NOT NULL,
				[IdEmpleadoResponsable] [int] NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdCompra] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Comprobantes]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Comprobantes](
				[IdComprobante] [int] IDENTITY(1,1) NOT NULL,
				[Tipo] [int] NOT NULL,
				[Nombre] [varchar](60) NOT NULL,
				[SecuenciaActual] [int] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdComprobante] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Configuracion]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Configuracion](
				[Id] [int] IDENTITY(1,1) NOT NULL,
				[NombrePC] [varchar](100) NOT NULL,
				[ColorPanel] [varchar](25) NULL,
				[IdCaja] [int] NULL,
			PRIMARY KEY CLUSTERED 
			(
				[Id] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[NombrePC] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[ConfiguracionSistema]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[ConfiguracionSistema](
				[IdConfiguracion] [int] NOT NULL,
				[PorcentajeGanancia] [int] NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdConfiguracion] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Departamento]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Departamento](
				[IdDepartamento] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](80) NOT NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdDepartamento] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[DetalleCompra]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[DetalleCompra](
				[IdDetalle] [int] IDENTITY(1,1) NOT NULL,
				[IdCompra] [int] NOT NULL,
				[IdProducto] [int] NOT NULL,
				[Cantidad] [decimal](14, 3) NOT NULL,
				[CostoUnitario] [decimal](12, 4) NOT NULL,
				[Subtotal]  AS ([Cantidad]*[CostoUnitario]) PERSISTED,
			PRIMARY KEY CLUSTERED 
			(
				[IdDetalle] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[DetallePago]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[DetallePago](
				[IdPago] [int] IDENTITY(1,1) NOT NULL,
				[IdPedido] [int] NOT NULL,
				[TipoDetalle] [varchar](20) NULL,
				[Efectivo] [decimal](10, 2) NULL,
				[Devuelta] [decimal](10, 2) NULL,
				[Tarjeta] [decimal](10, 2) NULL,
				[TarjetaNombre] [varchar](50) NULL,
				[Transferencia] [decimal](10, 2) NULL,
				[Banco] [varchar](70) NULL,
				[Total] [decimal](10, 2) NOT NULL,
				[Estado] [bit] NOT NULL,
				[Referencia] [varchar](30) NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdPago] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[DetallePedido]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[DetallePedido](
				[IdDetalle] [int] IDENTITY(1,1) NOT NULL,
				[IdPedido] [int] NOT NULL,
				[IdProducto] [int] NOT NULL,
				[Cantidad] [decimal](12, 3) NOT NULL,
				[PrecioUnitario] [decimal](10, 2) NOT NULL,
				[Subtotal]  AS ([Cantidad]*[PrecioUnitario]) PERSISTED,
				[Cuenta] [int] NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdDetalle] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Empleado]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Empleado](
				[IdEmpleado] [int] IDENTITY(1,1) NOT NULL,
				[IdPersona] [int] NOT NULL,
				[IdPuesto] [int] NOT NULL,
				[FechaIngreso] [date] NOT NULL,
				[Activo] [bit] NOT NULL,
				[Sueldo] [decimal](10, 2) NOT NULL,
				[TipoSueldo] [int] NOT NULL,
				[IdRolEmpleado] [int] NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdEmpleado] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[IdPersona] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Entrega]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Entrega](
				[IdEntrega] [int] IDENTITY(1,1) NOT NULL,
				[IdPedido] [int] NOT NULL,
				[IdRepartidor] [int] NOT NULL,
				[Estado] [varchar](15) NOT NULL,
				[IdZona] [int] NULL,
				[HoraSalida] [datetime2](7) NULL,
				[HoraEntrega] [datetime2](7) NULL,
				[FormaPago] [varchar](20) NULL,
				[Observacion] [varchar](200) NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdEntrega] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Evento]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Evento](
				[IdEvento] [int] IDENTITY(1,1) NOT NULL,
				[Organizador] [varchar](120) NOT NULL,
				[FechaInicio] [datetime2](7) NOT NULL,
				[PersonasEstimadas] [int] NOT NULL,
				[IdSala] [int] NULL,
				[MontajeMin] [int] NOT NULL,
				[DesmontajeMin] [int] NOT NULL,
				[Estado] [varchar](15) NOT NULL,
				[CreadoEn] [datetime2](7) NOT NULL,
				[FechaFin] [datetime2](7) NOT NULL,
				[NombreEvento] [varchar](120) NOT NULL,
				[IdCliente] [int] NULL,
				[Nota] [varchar](150) NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdEvento] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[EventoMesa]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[EventoMesa](
				[IdEvento] [int] NOT NULL,
				[IdMesa] [int] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdEvento] ASC,
				[IdMesa] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Factura]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Factura](
				[IdFactura] [int] IDENTITY(1,1) NOT NULL,
				[IdPedido] [int] NOT NULL,
				[Subtotal] [decimal](12, 2) NOT NULL,
				[Impuestos] [decimal](12, 2) NOT NULL,
				[Propina] [decimal](12, 2) NOT NULL,
				[Total] [decimal](12, 2) NOT NULL,
				[Estado] [varchar](12) NOT NULL,
				[EmitidaEn] [datetime2](7) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdFactura] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[IdPedido] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Impuesto]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Impuesto](
				[IdImpuesto] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](60) NOT NULL,
				[Tipo] [varchar](15) NOT NULL,
				[Valor] [decimal](9, 4) NOT NULL,
				[FechaInicio] [date] NOT NULL,
				[FechaFin] [date] NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdImpuesto] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Ingrediente]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Ingrediente](
				[IdIngrediente] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](120) NOT NULL,
				[UnidadBase] [varchar](10) NOT NULL,
				[Activo] [bit] NOT NULL,
				[EsExento] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdIngrediente] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Inventario]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Inventario](
				[IdIngrediente] [int] NOT NULL,
				[StockActual] [decimal](14, 3) NOT NULL,
				[StockMinimo] [decimal](14, 3) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdIngrediente] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Mesa]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Mesa](
				[IdMesa] [int] IDENTITY(1,1) NOT NULL,
				[IdSala] [int] NOT NULL,
				[Numero] [int] NOT NULL,
				[Capacidad] [int] NOT NULL,
				[Estado] [varchar](20) NOT NULL,
				[Ocupado] [bit] NOT NULL,
				[Reservado] [bit] NULL,
				[IdGrupo] [int] NULL,
				[EsPrincipal] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdMesa] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[IdSala] ASC,
				[Numero] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[MetodoPago]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[MetodoPago](
				[IdMetodoPago] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](30) NOT NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdMetodoPago] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[MotivoSalidaInventario]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[MotivoSalidaInventario](
				[IdMotivo] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](40) NOT NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdMotivo] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[MovimientosInventario]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[MovimientosInventario](
				[IdMovimiento] [bigint] IDENTITY(1,1) NOT NULL,
				[Fecha] [datetime2](7) NOT NULL,
				[IdIngrediente] [int] NOT NULL,
				[TipoMovimiento] [varchar](20) NOT NULL,
				[Cantidad] [decimal](14, 3) NOT NULL,
				[CostoUnitario] [decimal](12, 4) NULL,
				[IdReferencia] [varchar](40) NULL,
				[Origen] [varchar](30) NULL,
				[IdUsuario] [int] NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdMovimiento] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Pago]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Pago](
				[IdPago] [int] IDENTITY(1,1) NOT NULL,
				[IdFactura] [int] NOT NULL,
				[IdMetodoPago] [int] NOT NULL,
				[Monto] [decimal](12, 2) NOT NULL,
				[RegistradoEn] [datetime2](7) NOT NULL,
				[Referencia] [varchar](60) NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdPago] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Parametros]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Parametros](
				[Clave] [varchar](50) NOT NULL,
				[Valor] [varchar](50) NOT NULL,
				[ModificadoEn] [datetime2](7) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[Clave] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Pedido]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Pedido](
				[IdPedido] [int] IDENTITY(1,1) NOT NULL,
				[Fecha] [datetime2](7) NOT NULL,
				[Origen] [varchar](12) NOT NULL,
				[IdMesa] [int] NULL,
				[IdClientePersona] [int] NULL,
				[Estado] [varchar](15) NOT NULL,
				[CreadoPor] [int] NULL,
				[NombreCliente] [varchar](50) NULL,
				[Total] [decimal](10, 2) NOT NULL,
				[Nota] [varchar](150) NULL,
				[Direccion] [varchar](200) NULL,
				[IdRepartidor] [int] NULL,
				[Comprobante] [varchar](30) NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdPedido] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[PermisosUsuario]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[PermisosUsuario](
				[Numero] [int] IDENTITY(1,1) NOT NULL,
				[IdUsuario] [int] NOT NULL,
				[Admin] [bit] NOT NULL,
				[CrearOrdenReservacion] [bit] NOT NULL,
				[CancelarDoc] [bit] NOT NULL,
				[CambiarPrecio] [bit] NOT NULL,
				[PrecioMinimo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[Numero] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Persona]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Persona](
				[IdPersona] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](120) NOT NULL,
				[Apellido] [varchar](120) NULL,
				[NombreCompleto]  AS (ltrim(rtrim(coalesce([Nombre],'')+case when [Apellido] IS NULL OR [Apellido]='' then '' else ' '+[Apellido] end))) PERSISTED,
				[Email] [varchar](100) NULL,
				[Activo] [bit] NOT NULL,
				[CreadoEn] [datetime2](7) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdPersona] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[PersonaDireccion]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[PersonaDireccion](
				[IdDireccion] [int] IDENTITY(1,1) NOT NULL,
				[IdPersona] [int] NOT NULL,
				[Direccion] [text] NOT NULL,
				[EsPrincipal] [bit] NOT NULL,
				[Nombre] [varchar](30) NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdDireccion] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[PersonaDocumento]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[PersonaDocumento](
				[IdPersonaDoc] [int] IDENTITY(1,1) NOT NULL,
				[IdPersona] [int] NOT NULL,
				[IdTipoDocumento] [int] NOT NULL,
				[Numero] [varchar](25) NOT NULL,
				[EsPrincipal] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdPersonaDoc] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			 CONSTRAINT [UQ_PersonaDoc] UNIQUE NONCLUSTERED 
			(
				[IdPersona] ASC,
				[IdTipoDocumento] ASC,
				[Numero] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[PersonaTelefono]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[PersonaTelefono](
				[IdTelefono] [int] IDENTITY(1,1) NOT NULL,
				[IdPersona] [int] NOT NULL,
				[Tipo] [varchar](20) NULL,
				[Numero] [varchar](20) NOT NULL,
				[EsPrincipal] [bit] NOT NULL,
				[NombreTelefono] [char](30) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdTelefono] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			 CONSTRAINT [UQ_PersonaTelefono] UNIQUE NONCLUSTERED 
			(
				[IdPersona] ASC,
				[Numero] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[ProductoImpuesto]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[ProductoImpuesto](
				[IdProductoImpuesto] [int] IDENTITY(1,1) NOT NULL,
				[IdProducto] [int] NOT NULL,
				[IdImpuesto] [int] NOT NULL,
				[FechaInicio] [date] NOT NULL,
				[FechaFin] [date] NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdProductoImpuesto] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[IdProducto] ASC,
				[IdImpuesto] ASC,
				[FechaInicio] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[ProductoPrecio]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[ProductoPrecio](
				[IdProductoPrecio] [int] IDENTITY(1,1) NOT NULL,
				[IdProducto] [int] NOT NULL,
				[Precio] [decimal](10, 2) NOT NULL,
				[FechaInicio] [date] NOT NULL,
				[FechaFin] [date] NULL,
				[Motivo] [varchar](120) NULL,
				[AprobadoPor] [int] NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdProductoPrecio] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[ProductoTipo]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[ProductoTipo](
				[IdProductoTipo] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](30) NOT NULL,
				[Activo] [bit] NOT NULL,
				[Ingrediente] [bit] NOT NULL,
				[Bebida] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdProductoTipo] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[ProductoVenta]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[ProductoVenta](
				[IdProducto] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](120) NOT NULL,
				[IdCategoria] [int] NOT NULL,
				[IdProductoTipo] [int] NOT NULL,
				[Activo] [bit] NOT NULL,
				[PrecioCompra] [decimal](10, 2) NULL,
				[PrecioVenta] [decimal](10, 2) NULL,
				[Itbis] [decimal](10, 2) NULL,
				[CodigoBarra] [char](12) NULL,
				[IdUnidadMedida] [int] NOT NULL,
				[Existencia] [decimal](10, 2) NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdProducto] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Proveedor]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Proveedor](
				[IdProveedor] [int] IDENTITY(1,1) NOT NULL,
				[IdPersona] [int] NOT NULL,
				[IdTipoDoc] [int] NOT NULL,
				[Activo] [bit] NOT NULL,
				[Informal] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdProveedor] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Puesto]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Puesto](
				[IdPuesto] [int] IDENTITY(1,1) NOT NULL,
				[IdDepartamento] [int] NOT NULL,
				[Nombre] [varchar](80) NOT NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdPuesto] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[IdDepartamento] ASC,
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Receta]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Receta](
				[IdReceta] [int] IDENTITY(1,1) NOT NULL,
				[IdProducto] [int] NOT NULL,
				[IdIngrediente] [int] NULL,
				[Activo] [bit] NULL,
				[IdUnidadMedida] [int] NULL,
				[Cantidad] [decimal](10, 2) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdReceta] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			 CONSTRAINT [UQ_Receta_ProductoIngrediente] UNIQUE NONCLUSTERED 
			(
				[IdProducto] ASC,
				[IdIngrediente] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[RecetaIngrediente]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[RecetaIngrediente](
				[IdReceta] [int] NOT NULL,
				[IdIngrediente] [int] NOT NULL,
				[Cantidad] [decimal](12, 3) NOT NULL,
				[RendimientoPct] [decimal](5, 2) NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdReceta] ASC,
				[IdIngrediente] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Repartidor]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Repartidor](
				[IdRepartidor] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](120) NOT NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdRepartidor] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Reserva]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Reserva](
				[IdReserva] [int] IDENTITY(1,1) NOT NULL,
				[IdMesa] [int] NOT NULL,
				[FechaHora] [datetime2](7) NOT NULL,
				[Personas] [int] NOT NULL,
				[Cliente] [varchar](120) NOT NULL,
				[Estado] [varchar](12) NOT NULL,
				[CreadoEn] [datetime2](7) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdReserva] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Rol]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Rol](
				[IdRol] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](40) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdRol] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[RolPersona]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[RolPersona](
				[IdPersona] [int] NOT NULL,
				[IdTipoRol] [int] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdPersona] ASC,
				[IdTipoRol] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Sala]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Sala](
				[IdSala] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](60) NOT NULL,
				[Capacidad] [int] NOT NULL,
				[Activo] [bit] NOT NULL,
				[Piso] [int] NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdSala] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[TicketCocina]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[TicketCocina](
				[IdTicket] [int] IDENTITY(1,1) NOT NULL,
				[IdPedido] [int] NOT NULL,
				[IdDetalle] [int] NOT NULL,
				[Estado] [varchar](15) NOT NULL,
				[Area] [varchar](20) NULL,
				[PreparadoPor] [int] NULL,
				[HoraEstado] [datetime2](7) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdTicket] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[TipoDocumento]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[TipoDocumento](
				[IdTipoDocumento] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](40) NOT NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdTipoDocumento] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[TipoRolPersona]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[TipoRolPersona](
				[IdTipoRol] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](30) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdTipoRol] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[UnidadMedida]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[UnidadMedida](
				[IdUnidadMedida] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](50) NOT NULL,
				[Valor] [decimal](10, 4) NOT NULL,
				[Activo] [bit] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdUnidadMedida] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[Usuario]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[Usuario](
				[IdUsuario] [int] IDENTITY(1,1) NOT NULL,
				[Login] [varchar](40) NOT NULL,
				[Contrasena] [varchar](72) NULL,
				[IdPersona] [int] NULL,
				[Activo] [bit] NOT NULL,
				[CreadoEn] [datetime2](7) NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdUsuario] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Login] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[UsuarioRol]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[UsuarioRol](
				[IdUsuario] [int] NOT NULL,
				[IdRol] [int] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdUsuario] ASC,
				[IdRol] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Table [dbo].[ZonaDelivery]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE TABLE [dbo].[ZonaDelivery](
				[IdZona] [int] IDENTITY(1,1) NOT NULL,
				[Nombre] [varchar](40) NOT NULL,
				[TiempoEstimadoMin] [int] NOT NULL,
			PRIMARY KEY CLUSTERED 
			(
				[IdZona] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
			UNIQUE NONCLUSTERED 
			(
				[Nombre] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			GO
			/****** Object:  Index [IX_DetallePedido_IdPedido]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			CREATE NONCLUSTERED INDEX [IX_DetallePedido_IdPedido] ON [dbo].[DetallePedido]
			(
				[IdPedido] ASC
			)
			INCLUDE([IdProducto],[Cantidad],[Subtotal]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			GO
			/****** Object:  Index [IX_DetallePedido_IdProducto]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			CREATE NONCLUSTERED INDEX [IX_DetallePedido_IdProducto] ON [dbo].[DetallePedido]
			(
				[IdProducto] ASC
			)
			INCLUDE([IdPedido],[Cantidad],[Subtotal]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			GO
			SET ANSI_PADDING ON
			GO
			/****** Object:  Index [IX_Pedido_Fecha_Estado]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			CREATE NONCLUSTERED INDEX [IX_Pedido_Fecha_Estado] ON [dbo].[Pedido]
			(
				[Fecha] ASC,
				[Estado] ASC
			)
			INCLUDE([IdPedido],[Total],[NombreCliente],[Origen],[IdMesa],[CreadoPor],[Comprobante]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			GO
			ALTER TABLE [dbo].[Caja] ADD  CONSTRAINT [DF_Caja_Activo]  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Caja] ADD  CONSTRAINT [DF_Caja_Numero]  DEFAULT ((1)) FOR [Numero]
			GO
			ALTER TABLE [dbo].[CategoriaProducto] ADD  CONSTRAINT [DF_CategoriaProducto_Activo]  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Cliente] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Comanda] ADD  CONSTRAINT [DF_Comanda_Comanda]  DEFAULT ((0)) FOR [Cuenta]
			GO
			ALTER TABLE [dbo].[Compra] ADD  DEFAULT (sysdatetime()) FOR [Fecha]
			GO
			ALTER TABLE [dbo].[Compra] ADD  DEFAULT ((0)) FOR [Subtotal]
			GO
			ALTER TABLE [dbo].[Compra] ADD  DEFAULT ((0)) FOR [Impuestos]
			GO
			ALTER TABLE [dbo].[Compra] ADD  DEFAULT ((0)) FOR [Total]
			GO
			ALTER TABLE [dbo].[Compra] ADD  CONSTRAINT [DF_Compra_Estado]  DEFAULT ('Pendiente') FOR [Estado]
			GO
			ALTER TABLE [dbo].[Departamento] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[DetallePedido] ADD  CONSTRAINT [DF_DetallePedido_Grupo]  DEFAULT ((0)) FOR [Cuenta]
			GO
			ALTER TABLE [dbo].[Empleado] ADD  DEFAULT (CONVERT([date],getdate())) FOR [FechaIngreso]
			GO
			ALTER TABLE [dbo].[Empleado] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Empleado] ADD  CONSTRAINT [DF_Empleado_Sueldo]  DEFAULT ((1)) FOR [Sueldo]
			GO
			ALTER TABLE [dbo].[Empleado] ADD  CONSTRAINT [DF_Empleado_TipoSueldo]  DEFAULT ((1)) FOR [TipoSueldo]
			GO
			ALTER TABLE [dbo].[Entrega] ADD  DEFAULT ('preparando') FOR [Estado]
			GO
			ALTER TABLE [dbo].[Evento] ADD  DEFAULT ((0)) FOR [MontajeMin]
			GO
			ALTER TABLE [dbo].[Evento] ADD  DEFAULT ((0)) FOR [DesmontajeMin]
			GO
			ALTER TABLE [dbo].[Evento] ADD  DEFAULT ('planeado') FOR [Estado]
			GO
			ALTER TABLE [dbo].[Evento] ADD  DEFAULT (sysdatetime()) FOR [CreadoEn]
			GO
			ALTER TABLE [dbo].[Factura] ADD  DEFAULT ((0)) FOR [Propina]
			GO
			ALTER TABLE [dbo].[Factura] ADD  DEFAULT ('emitida') FOR [Estado]
			GO
			ALTER TABLE [dbo].[Factura] ADD  DEFAULT (sysdatetime()) FOR [EmitidaEn]
			GO
			ALTER TABLE [dbo].[Impuesto] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Ingrediente] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Ingrediente] ADD  CONSTRAINT [DF_Ingrediente_EsExento]  DEFAULT ((0)) FOR [EsExento]
			GO
			ALTER TABLE [dbo].[Inventario] ADD  DEFAULT ((0)) FOR [StockActual]
			GO
			ALTER TABLE [dbo].[Inventario] ADD  DEFAULT ((0)) FOR [StockMinimo]
			GO
			ALTER TABLE [dbo].[Mesa] ADD  DEFAULT ('libre') FOR [Estado]
			GO
			ALTER TABLE [dbo].[Mesa] ADD  CONSTRAINT [DF_Mesa_Ocupado]  DEFAULT ((1)) FOR [Ocupado]
			GO
			ALTER TABLE [dbo].[Mesa] ADD  CONSTRAINT [DF_Mesa_Reservado]  DEFAULT ((1)) FOR [Reservado]
			GO
			ALTER TABLE [dbo].[Mesa] ADD  CONSTRAINT [DF_IdGrupo_Mesa]  DEFAULT ((0)) FOR [IdGrupo]
			GO
			ALTER TABLE [dbo].[Mesa] ADD  CONSTRAINT [DF_EsPrincipal_Mesa]  DEFAULT ((0)) FOR [EsPrincipal]
			GO
			ALTER TABLE [dbo].[MetodoPago] ADD  CONSTRAINT [DF_MetodoPago_Activo]  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[MotivoSalidaInventario] ADD  CONSTRAINT [DF_MotivoSalidaInventario_Activo]  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[MovimientosInventario] ADD  DEFAULT (sysdatetime()) FOR [Fecha]
			GO
			ALTER TABLE [dbo].[Pago] ADD  DEFAULT (sysdatetime()) FOR [RegistradoEn]
			GO
			ALTER TABLE [dbo].[Parametros] ADD  DEFAULT (sysdatetime()) FOR [ModificadoEn]
			GO
			ALTER TABLE [dbo].[Pedido] ADD  DEFAULT (sysdatetime()) FOR [Fecha]
			GO
			ALTER TABLE [dbo].[Pedido] ADD  DEFAULT ('abierto') FOR [Estado]
			GO
			ALTER TABLE [dbo].[Pedido] ADD  CONSTRAINT [DF_Pedido_NombreCliente]  DEFAULT ((1)) FOR [NombreCliente]
			GO
			ALTER TABLE [dbo].[Pedido] ADD  CONSTRAINT [DF_Pedido_Total]  DEFAULT ((1)) FOR [Total]
			GO
			ALTER TABLE [dbo].[Pedido] ADD  CONSTRAINT [DF_Pedido_Nota]  DEFAULT ((1)) FOR [Nota]
			GO
			ALTER TABLE [dbo].[Pedido] ADD  CONSTRAINT [DF_Pedido_Direccion]  DEFAULT ('') FOR [Direccion]
			GO
			ALTER TABLE [dbo].[Pedido] ADD  CONSTRAINT [DF_Pedido_IdRepartidor]  DEFAULT ((0)) FOR [IdRepartidor]
			GO
			ALTER TABLE [dbo].[PermisosUsuario] ADD  DEFAULT ((0)) FOR [Admin]
			GO
			ALTER TABLE [dbo].[PermisosUsuario] ADD  CONSTRAINT [DF_PermisosUsuario_CrearOrdenReservacion]  DEFAULT ((0)) FOR [CrearOrdenReservacion]
			GO
			ALTER TABLE [dbo].[PermisosUsuario] ADD  CONSTRAINT [DF_PermisosUsuario_CancelarDoc]  DEFAULT ((0)) FOR [CancelarDoc]
			GO
			ALTER TABLE [dbo].[PermisosUsuario] ADD  CONSTRAINT [DF_PermisosUsuario_CambiarPrecio]  DEFAULT ((0)) FOR [CambiarPrecio]
			GO
			ALTER TABLE [dbo].[PermisosUsuario] ADD  CONSTRAINT [DF_PermisosUsuario_PrecioMinimo]  DEFAULT ((1)) FOR [PrecioMinimo]
			GO
			ALTER TABLE [dbo].[Persona] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Persona] ADD  DEFAULT (sysdatetime()) FOR [CreadoEn]
			GO
			ALTER TABLE [dbo].[PersonaDireccion] ADD  DEFAULT ((0)) FOR [EsPrincipal]
			GO
			ALTER TABLE [dbo].[PersonaDireccion] ADD  CONSTRAINT [DF_PersonaDireccion_Nombre]  DEFAULT ((1)) FOR [Nombre]
			GO
			ALTER TABLE [dbo].[PersonaDocumento] ADD  DEFAULT ((0)) FOR [EsPrincipal]
			GO
			ALTER TABLE [dbo].[PersonaTelefono] ADD  DEFAULT ((0)) FOR [EsPrincipal]
			GO
			ALTER TABLE [dbo].[PersonaTelefono] ADD  CONSTRAINT [DF_PersonaTelefono_NombreTelefono]  DEFAULT ((1)) FOR [NombreTelefono]
			GO
			ALTER TABLE [dbo].[ProductoImpuesto] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[ProductoTipo] ADD  CONSTRAINT [DF_ProductoTipo_Activo]  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[ProductoTipo] ADD  CONSTRAINT [DF_ProductoTipo_Ingrediente]  DEFAULT ((1)) FOR [Ingrediente]
			GO
			ALTER TABLE [dbo].[ProductoTipo] ADD  CONSTRAINT [DF_ProductoTipo_Bebida]  DEFAULT ((0)) FOR [Bebida]
			GO
			ALTER TABLE [dbo].[ProductoVenta] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[ProductoVenta] ADD  CONSTRAINT [DF_ProductoVenta_PrecioCompra]  DEFAULT ((1)) FOR [PrecioCompra]
			GO
			ALTER TABLE [dbo].[ProductoVenta] ADD  CONSTRAINT [DF_ProductoVenta_PrecioVenta]  DEFAULT ((1)) FOR [PrecioVenta]
			GO
			ALTER TABLE [dbo].[ProductoVenta] ADD  CONSTRAINT [DF_ProductoVenta_Itbis]  DEFAULT ((1)) FOR [Itbis]
			GO
			ALTER TABLE [dbo].[ProductoVenta] ADD  CONSTRAINT [DF_ProductoVenta_CodigoBarra]  DEFAULT ((1)) FOR [CodigoBarra]
			GO
			ALTER TABLE [dbo].[ProductoVenta] ADD  CONSTRAINT [DF_ProductoVenta_IdUnidadMedida]  DEFAULT ((1)) FOR [IdUnidadMedida]
			GO
			ALTER TABLE [dbo].[ProductoVenta] ADD  CONSTRAINT [DF_ProductoVenta_Existencia]  DEFAULT ((1)) FOR [Existencia]
			GO
			ALTER TABLE [dbo].[Proveedor] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Proveedor] ADD  CONSTRAINT [DF_Proveedor_Informal]  DEFAULT ((1)) FOR [Informal]
			GO
			ALTER TABLE [dbo].[Puesto] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Receta] ADD  CONSTRAINT [DF_Receta_IdIngrediente]  DEFAULT ((1)) FOR [IdIngrediente]
			GO
			ALTER TABLE [dbo].[Receta] ADD  CONSTRAINT [DF_Receta_Activo]  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Receta] ADD  CONSTRAINT [DF_Receta_Cantidad]  DEFAULT ((1)) FOR [Cantidad]
			GO
			ALTER TABLE [dbo].[Repartidor] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Reserva] ADD  DEFAULT ('solicitada') FOR [Estado]
			GO
			ALTER TABLE [dbo].[Reserva] ADD  DEFAULT (sysdatetime()) FOR [CreadoEn]
			GO
			ALTER TABLE [dbo].[Sala] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Sala] ADD  CONSTRAINT [DF_Sala_Piso]  DEFAULT ((1)) FOR [Piso]
			GO
			ALTER TABLE [dbo].[TicketCocina] ADD  DEFAULT ('pendiente') FOR [Estado]
			GO
			ALTER TABLE [dbo].[TicketCocina] ADD  DEFAULT (sysdatetime()) FOR [HoraEstado]
			GO
			ALTER TABLE [dbo].[TipoDocumento] ADD  CONSTRAINT [DF_TipoDocumento_Activo]  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[UnidadMedida] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Usuario] ADD  DEFAULT ((1)) FOR [Activo]
			GO
			ALTER TABLE [dbo].[Usuario] ADD  DEFAULT (sysdatetime()) FOR [CreadoEn]
			GO
			ALTER TABLE [dbo].[Cliente]  WITH NOCHECK ADD  CONSTRAINT [FK_Cliente_Persona] FOREIGN KEY([IdPersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[Cliente] CHECK CONSTRAINT [FK_Cliente_Persona]
			GO
			ALTER TABLE [dbo].[Comanda]  WITH NOCHECK ADD  CONSTRAINT [FK_Comanda_Mesa] FOREIGN KEY([IdMesa])
			REFERENCES [dbo].[Mesa] ([IdMesa])
			GO
			ALTER TABLE [dbo].[Comanda] CHECK CONSTRAINT [FK_Comanda_Mesa]
			GO
			ALTER TABLE [dbo].[Comanda]  WITH NOCHECK ADD  CONSTRAINT [FK_Comanda_Pedido] FOREIGN KEY([IdPedido])
			REFERENCES [dbo].[Pedido] ([IdPedido])
			GO
			ALTER TABLE [dbo].[Comanda] CHECK CONSTRAINT [FK_Comanda_Pedido]
			GO
			ALTER TABLE [dbo].[Comanda]  WITH NOCHECK ADD  CONSTRAINT [FK_Comanda_Producto] FOREIGN KEY([IdProducto])
			REFERENCES [dbo].[ProductoVenta] ([IdProducto])
			GO
			ALTER TABLE [dbo].[Comanda] CHECK CONSTRAINT [FK_Comanda_Producto]
			GO
			ALTER TABLE [dbo].[Compra]  WITH NOCHECK ADD FOREIGN KEY([IdProveedorPersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[Compra]  WITH NOCHECK ADD  CONSTRAINT [FK_Compra_Empleado_Responsable] FOREIGN KEY([IdEmpleadoResponsable])
			REFERENCES [dbo].[Empleado] ([IdEmpleado])
			GO
			ALTER TABLE [dbo].[Compra] CHECK CONSTRAINT [FK_Compra_Empleado_Responsable]
			GO
			ALTER TABLE [dbo].[DetalleCompra]  WITH NOCHECK ADD  CONSTRAINT [FK_DetalleCompra_Compra] FOREIGN KEY([IdCompra])
			REFERENCES [dbo].[Compra] ([IdCompra])
			GO
			ALTER TABLE [dbo].[DetalleCompra] CHECK CONSTRAINT [FK_DetalleCompra_Compra]
			GO
			ALTER TABLE [dbo].[DetalleCompra]  WITH NOCHECK ADD  CONSTRAINT [FK_DetalleCompra_Producto] FOREIGN KEY([IdProducto])
			REFERENCES [dbo].[ProductoVenta] ([IdProducto])
			GO
			ALTER TABLE [dbo].[DetalleCompra] CHECK CONSTRAINT [FK_DetalleCompra_Producto]
			GO
			ALTER TABLE [dbo].[DetallePago]  WITH NOCHECK ADD  CONSTRAINT [FK_DetallePago_Pedido] FOREIGN KEY([IdPedido])
			REFERENCES [dbo].[Pedido] ([IdPedido])
			GO
			ALTER TABLE [dbo].[DetallePago] CHECK CONSTRAINT [FK_DetallePago_Pedido]
			GO
			ALTER TABLE [dbo].[DetallePedido]  WITH NOCHECK ADD FOREIGN KEY([IdPedido])
			REFERENCES [dbo].[Pedido] ([IdPedido])
			GO
			ALTER TABLE [dbo].[DetallePedido]  WITH NOCHECK ADD FOREIGN KEY([IdProducto])
			REFERENCES [dbo].[ProductoVenta] ([IdProducto])
			GO
			ALTER TABLE [dbo].[Empleado]  WITH NOCHECK ADD FOREIGN KEY([IdPersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[Empleado]  WITH NOCHECK ADD FOREIGN KEY([IdPuesto])
			REFERENCES [dbo].[Puesto] ([IdPuesto])
			GO
			ALTER TABLE [dbo].[Entrega]  WITH NOCHECK ADD FOREIGN KEY([IdPedido])
			REFERENCES [dbo].[Pedido] ([IdPedido])
			GO
			ALTER TABLE [dbo].[Entrega]  WITH NOCHECK ADD FOREIGN KEY([IdRepartidor])
			REFERENCES [dbo].[Repartidor] ([IdRepartidor])
			GO
			ALTER TABLE [dbo].[Entrega]  WITH NOCHECK ADD FOREIGN KEY([IdZona])
			REFERENCES [dbo].[ZonaDelivery] ([IdZona])
			GO
			ALTER TABLE [dbo].[Evento]  WITH NOCHECK ADD FOREIGN KEY([IdSala])
			REFERENCES [dbo].[Sala] ([IdSala])
			GO
			ALTER TABLE [dbo].[Evento]  WITH NOCHECK ADD  CONSTRAINT [FK_Evento_Cliente] FOREIGN KEY([IdCliente])
			REFERENCES [dbo].[Cliente] ([IdCliente])
			GO
			ALTER TABLE [dbo].[Evento] CHECK CONSTRAINT [FK_Evento_Cliente]
			GO
			ALTER TABLE [dbo].[EventoMesa]  WITH NOCHECK ADD FOREIGN KEY([IdEvento])
			REFERENCES [dbo].[Evento] ([IdEvento])
			GO
			ALTER TABLE [dbo].[EventoMesa]  WITH NOCHECK ADD FOREIGN KEY([IdMesa])
			REFERENCES [dbo].[Mesa] ([IdMesa])
			GO
			ALTER TABLE [dbo].[Factura]  WITH NOCHECK ADD FOREIGN KEY([IdPedido])
			REFERENCES [dbo].[Pedido] ([IdPedido])
			GO
			ALTER TABLE [dbo].[Inventario]  WITH NOCHECK ADD FOREIGN KEY([IdIngrediente])
			REFERENCES [dbo].[Ingrediente] ([IdIngrediente])
			GO
			ALTER TABLE [dbo].[Mesa]  WITH NOCHECK ADD FOREIGN KEY([IdSala])
			REFERENCES [dbo].[Sala] ([IdSala])
			GO
			ALTER TABLE [dbo].[MovimientosInventario]  WITH NOCHECK ADD FOREIGN KEY([IdIngrediente])
			REFERENCES [dbo].[Ingrediente] ([IdIngrediente])
			GO
			ALTER TABLE [dbo].[MovimientosInventario]  WITH NOCHECK ADD FOREIGN KEY([IdUsuario])
			REFERENCES [dbo].[Usuario] ([IdUsuario])
			GO
			ALTER TABLE [dbo].[Pago]  WITH NOCHECK ADD FOREIGN KEY([IdFactura])
			REFERENCES [dbo].[Factura] ([IdFactura])
			GO
			ALTER TABLE [dbo].[Pago]  WITH NOCHECK ADD FOREIGN KEY([IdMetodoPago])
			REFERENCES [dbo].[MetodoPago] ([IdMetodoPago])
			GO
			ALTER TABLE [dbo].[Pedido]  WITH NOCHECK ADD FOREIGN KEY([CreadoPor])
			REFERENCES [dbo].[Usuario] ([IdUsuario])
			GO
			ALTER TABLE [dbo].[Pedido]  WITH NOCHECK ADD FOREIGN KEY([IdClientePersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[Pedido]  WITH NOCHECK ADD FOREIGN KEY([IdMesa])
			REFERENCES [dbo].[Mesa] ([IdMesa])
			GO
			ALTER TABLE [dbo].[PermisosUsuario]  WITH NOCHECK ADD  CONSTRAINT [FK_PermisosUsuario_Usuario] FOREIGN KEY([IdUsuario])
			REFERENCES [dbo].[Usuario] ([IdUsuario])
			GO
			ALTER TABLE [dbo].[PermisosUsuario] CHECK CONSTRAINT [FK_PermisosUsuario_Usuario]
			GO
			ALTER TABLE [dbo].[PersonaDireccion]  WITH NOCHECK ADD  CONSTRAINT [FK_Direccion_Persona] FOREIGN KEY([IdPersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[PersonaDireccion] CHECK CONSTRAINT [FK_Direccion_Persona]
			GO
			ALTER TABLE [dbo].[PersonaDocumento]  WITH NOCHECK ADD FOREIGN KEY([IdPersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[PersonaDocumento]  WITH NOCHECK ADD FOREIGN KEY([IdTipoDocumento])
			REFERENCES [dbo].[TipoDocumento] ([IdTipoDocumento])
			GO
			ALTER TABLE [dbo].[PersonaTelefono]  WITH NOCHECK ADD FOREIGN KEY([IdPersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[ProductoImpuesto]  WITH NOCHECK ADD FOREIGN KEY([IdImpuesto])
			REFERENCES [dbo].[Impuesto] ([IdImpuesto])
			GO
			ALTER TABLE [dbo].[ProductoImpuesto]  WITH NOCHECK ADD FOREIGN KEY([IdProducto])
			REFERENCES [dbo].[ProductoVenta] ([IdProducto])
			GO
			ALTER TABLE [dbo].[ProductoPrecio]  WITH NOCHECK ADD FOREIGN KEY([AprobadoPor])
			REFERENCES [dbo].[Usuario] ([IdUsuario])
			GO
			ALTER TABLE [dbo].[ProductoPrecio]  WITH NOCHECK ADD FOREIGN KEY([IdProducto])
			REFERENCES [dbo].[ProductoVenta] ([IdProducto])
			GO
			ALTER TABLE [dbo].[ProductoVenta]  WITH NOCHECK ADD FOREIGN KEY([IdCategoria])
			REFERENCES [dbo].[CategoriaProducto] ([IdCategoria])
			GO
			ALTER TABLE [dbo].[ProductoVenta]  WITH NOCHECK ADD FOREIGN KEY([IdProductoTipo])
			REFERENCES [dbo].[ProductoTipo] ([IdProductoTipo])
			GO
			ALTER TABLE [dbo].[ProductoVenta]  WITH NOCHECK ADD  CONSTRAINT [fk_Producto_IdUnidadMedida] FOREIGN KEY([IdUnidadMedida])
			REFERENCES [dbo].[UnidadMedida] ([IdUnidadMedida])
			GO
			ALTER TABLE [dbo].[ProductoVenta] CHECK CONSTRAINT [fk_Producto_IdUnidadMedida]
			GO
			ALTER TABLE [dbo].[Proveedor]  WITH NOCHECK ADD  CONSTRAINT [FK_Proveedor_Persona] FOREIGN KEY([IdPersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[Proveedor] CHECK CONSTRAINT [FK_Proveedor_Persona]
			GO
			ALTER TABLE [dbo].[Puesto]  WITH NOCHECK ADD FOREIGN KEY([IdDepartamento])
			REFERENCES [dbo].[Departamento] ([IdDepartamento])
			GO
			ALTER TABLE [dbo].[Receta]  WITH NOCHECK ADD FOREIGN KEY([IdProducto])
			REFERENCES [dbo].[ProductoVenta] ([IdProducto])
			GO
			ALTER TABLE [dbo].[Receta]  WITH NOCHECK ADD  CONSTRAINT [FK_Receta_IdUnidadMedida] FOREIGN KEY([IdUnidadMedida])
			REFERENCES [dbo].[UnidadMedida] ([IdUnidadMedida])
			GO
			ALTER TABLE [dbo].[Receta] CHECK CONSTRAINT [FK_Receta_IdUnidadMedida]
			GO
			ALTER TABLE [dbo].[RecetaIngrediente]  WITH NOCHECK ADD FOREIGN KEY([IdIngrediente])
			REFERENCES [dbo].[Ingrediente] ([IdIngrediente])
			GO
			ALTER TABLE [dbo].[RecetaIngrediente]  WITH NOCHECK ADD FOREIGN KEY([IdReceta])
			REFERENCES [dbo].[Receta] ([IdReceta])
			GO
			ALTER TABLE [dbo].[Reserva]  WITH NOCHECK ADD FOREIGN KEY([IdMesa])
			REFERENCES [dbo].[Mesa] ([IdMesa])
			GO
			ALTER TABLE [dbo].[RolPersona]  WITH NOCHECK ADD FOREIGN KEY([IdPersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[RolPersona]  WITH NOCHECK ADD FOREIGN KEY([IdTipoRol])
			REFERENCES [dbo].[TipoRolPersona] ([IdTipoRol])
			GO
			ALTER TABLE [dbo].[TicketCocina]  WITH NOCHECK ADD FOREIGN KEY([IdDetalle])
			REFERENCES [dbo].[DetallePedido] ([IdDetalle])
			GO
			ALTER TABLE [dbo].[TicketCocina]  WITH NOCHECK ADD FOREIGN KEY([IdPedido])
			REFERENCES [dbo].[Pedido] ([IdPedido])
			GO
			ALTER TABLE [dbo].[TicketCocina]  WITH NOCHECK ADD FOREIGN KEY([PreparadoPor])
			REFERENCES [dbo].[Usuario] ([IdUsuario])
			GO
			ALTER TABLE [dbo].[Usuario]  WITH NOCHECK ADD FOREIGN KEY([IdPersona])
			REFERENCES [dbo].[Persona] ([IdPersona])
			GO
			ALTER TABLE [dbo].[UsuarioRol]  WITH NOCHECK ADD FOREIGN KEY([IdRol])
			REFERENCES [dbo].[Rol] ([IdRol])
			GO
			ALTER TABLE [dbo].[UsuarioRol]  WITH NOCHECK ADD FOREIGN KEY([IdUsuario])
			REFERENCES [dbo].[Usuario] ([IdUsuario])
			GO
			ALTER TABLE [dbo].[Comanda]  WITH NOCHECK ADD CHECK  (([Estado]='Entregado' OR [Estado]='Cocina'))
			GO
			ALTER TABLE [dbo].[Compra]  WITH NOCHECK ADD  CONSTRAINT [CK_Compra_Estado] CHECK  (([Estado]='Anulada' OR [Estado]='Recibida' OR [Estado]='Pendiente'))
			GO
			ALTER TABLE [dbo].[Compra] CHECK CONSTRAINT [CK_Compra_Estado]
			GO
			ALTER TABLE [dbo].[DetalleCompra]  WITH NOCHECK ADD  CONSTRAINT [CK_DetalleCompra_Cantidad] CHECK  (([Cantidad]>(0)))
			GO
			ALTER TABLE [dbo].[DetalleCompra] CHECK CONSTRAINT [CK_DetalleCompra_Cantidad]
			GO
			ALTER TABLE [dbo].[DetalleCompra]  WITH NOCHECK ADD  CONSTRAINT [CK_DetalleCompra_Costo] CHECK  (([CostoUnitario]>=(0)))
			GO
			ALTER TABLE [dbo].[DetalleCompra] CHECK CONSTRAINT [CK_DetalleCompra_Costo]
			GO
			ALTER TABLE [dbo].[DetallePago]  WITH NOCHECK ADD CHECK  (([TipoDetalle]='Transferencia' OR [TipoDetalle]='Tarjeta' OR [TipoDetalle]='Efectivo'))
			GO
			ALTER TABLE [dbo].[DetallePedido]  WITH NOCHECK ADD CHECK  (([Cantidad]>(0)))
			GO
			ALTER TABLE [dbo].[DetallePedido]  WITH NOCHECK ADD CHECK  (([PrecioUnitario]>=(0)))
			GO
			ALTER TABLE [dbo].[Entrega]  WITH NOCHECK ADD CHECK  (([Estado]='fallido' OR [Estado]='entregado' OR [Estado]='en ruta' OR [Estado]='preparando'))
			GO
			ALTER TABLE [dbo].[Entrega]  WITH NOCHECK ADD CHECK  (([FormaPago]='contraentrega' OR [FormaPago]='prepago'))
			GO
			ALTER TABLE [dbo].[Evento]  WITH NOCHECK ADD CHECK  (([DesmontajeMin]>=(0)))
			GO
			ALTER TABLE [dbo].[Evento]  WITH NOCHECK ADD CHECK  (([Estado]='cancelado' OR [Estado]='finalizado' OR [Estado]='en curso' OR [Estado]='confirmado' OR [Estado]='planeado'))
			GO
			ALTER TABLE [dbo].[Evento]  WITH NOCHECK ADD CHECK  (([MontajeMin]>=(0)))
			GO
			ALTER TABLE [dbo].[Evento]  WITH NOCHECK ADD CHECK  (([PersonasEstimadas]>(0)))
			GO
			ALTER TABLE [dbo].[Evento]  WITH NOCHECK ADD  CONSTRAINT [CK_Evento_Fechas] CHECK  (([FechaFin]>=[FechaInicio]))
			GO
			ALTER TABLE [dbo].[Evento] CHECK CONSTRAINT [CK_Evento_Fechas]
			GO
			ALTER TABLE [dbo].[Factura]  WITH NOCHECK ADD CHECK  (([Estado]='anulada' OR [Estado]='emitida'))
			GO
			ALTER TABLE [dbo].[Impuesto]  WITH NOCHECK ADD CHECK  (([Tipo]='monto' OR [Tipo]='porcentaje'))
			GO
			ALTER TABLE [dbo].[Impuesto]  WITH NOCHECK ADD CHECK  (([Valor]>=(0)))
			GO
			ALTER TABLE [dbo].[Ingrediente]  WITH NOCHECK ADD CHECK  (([UnidadBase]='und' OR [UnidadBase]='ml' OR [UnidadBase]='g'))
			GO
			ALTER TABLE [dbo].[Mesa]  WITH NOCHECK ADD CHECK  (([Capacidad]>(0)))
			GO
			ALTER TABLE [dbo].[MovimientosInventario]  WITH NOCHECK ADD CHECK  (([TipoMovimiento]='ajuste' OR [TipoMovimiento]='baja' OR [TipoMovimiento]='producción' OR [TipoMovimiento]='venta' OR [TipoMovimiento]='compra'))
			GO
			ALTER TABLE [dbo].[Pago]  WITH NOCHECK ADD CHECK  (([Monto]>=(0)))
			GO
			ALTER TABLE [dbo].[Pedido]  WITH NOCHECK ADD  CONSTRAINT [CK_Pedido_Estado] CHECK  (([Estado]='Cancelado' OR [Estado]='Facturado' OR [Estado]='Pendiente'))
			GO
			ALTER TABLE [dbo].[Pedido] CHECK CONSTRAINT [CK_Pedido_Estado]
			GO
			ALTER TABLE [dbo].[Pedido]  WITH NOCHECK ADD  CONSTRAINT [CK_Pedido_Origen] CHECK  (([Origen]='Delivery' OR [Origen]='Local'))
			GO
			ALTER TABLE [dbo].[Pedido] CHECK CONSTRAINT [CK_Pedido_Origen]
			GO
			ALTER TABLE [dbo].[PersonaTelefono]  WITH NOCHECK ADD CHECK  (([Tipo]='otro' OR [Tipo]='whatsapp' OR [Tipo]='fijo' OR [Tipo]='movil'))
			GO
			ALTER TABLE [dbo].[ProductoPrecio]  WITH NOCHECK ADD CHECK  (([Precio]>=(0)))
			GO
			ALTER TABLE [dbo].[RecetaIngrediente]  WITH NOCHECK ADD CHECK  (([Cantidad]>(0)))
			GO
			ALTER TABLE [dbo].[RecetaIngrediente]  WITH NOCHECK ADD CHECK  (([RendimientoPct]>=(0) AND [RendimientoPct]<=(100)))
			GO
			ALTER TABLE [dbo].[Reserva]  WITH NOCHECK ADD CHECK  (([Estado]='cancelada' OR [Estado]='confirmada' OR [Estado]='solicitada'))
			GO
			ALTER TABLE [dbo].[Reserva]  WITH NOCHECK ADD CHECK  (([Personas]>(0)))
			GO
			ALTER TABLE [dbo].[Sala]  WITH NOCHECK ADD CHECK  (([Capacidad]>=(0)))
			GO
			ALTER TABLE [dbo].[TicketCocina]  WITH NOCHECK ADD CHECK  (([Estado]='listo' OR [Estado]='preparación' OR [Estado]='pendiente'))
			GO
			ALTER TABLE [dbo].[ZonaDelivery]  WITH NOCHECK ADD CHECK  (([TiempoEstimadoMin]>=(1) AND [TiempoEstimadoMin]<=(240)))
			GO
			/****** Object:  StoredProcedure [dbo].[sp_ReporteClientes]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO

			CREATE   PROCEDURE [dbo].[sp_ReporteClientes]
				@FechaInicio DATE,
				@FechaFin    DATE
			AS
			BEGIN
				SET NOCOUNT ON;

				DECLARE @FinExclusivo DATETIME2(0)
					= DATEADD(DAY, 1, CAST(@FechaFin AS DATETIME2(0)));

				SELECT
					c.IdCliente,
					p.NombreCompleto,
					p.Email,
					pd.Numero AS Documento,
					pt.Numero AS Telefono,
					c.Activo,
					p.CreadoEn
				FROM Cliente c
				INNER JOIN Persona p
					ON p.IdPersona = c.IdPersona
				LEFT JOIN PersonaDocumento pd
					ON pd.IdPersona = p.IdPersona
				   AND pd.EsPrincipal = 1
				LEFT JOIN PersonaTelefono pt
					ON pt.IdPersona = p.IdPersona
				   AND pt.EsPrincipal = 1
				WHERE
					p.CreadoEn >= @FechaInicio
					AND p.CreadoEn <  @FinExclusivo
					AND p.IdPersona <> 2
				ORDER BY
					p.NombreCompleto;
			END;
			GO
			/****** Object:  StoredProcedure [dbo].[sp_ReporteCompras]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO


			CREATE   PROCEDURE [dbo].[sp_ReporteCompras]
				@FechaInicio DATE,
				@FechaFin    DATE
			AS
			BEGIN
				SET NOCOUNT ON;

				DECLARE @FinExclusivo DATETIME2(0)
					= DATEADD(DAY, 1, CAST(@FechaFin AS DATETIME2(0)));

				SELECT
					c.IdCompra,
					c.Fecha,
					c.IdProveedorPersona,
					c.Estado,

					-- Totales de la compra
					c.Subtotal,
					c.Impuestos,
					c.Total,

					-- Detalle
					pv.Nombre        AS Producto,
					dc.Cantidad,
					dc.CostoUnitario,
					dc.Subtotal      AS SubtotalProducto
				FROM Compra c
				INNER JOIN DetalleCompra dc
					ON dc.IdCompra = c.IdCompra
				INNER JOIN ProductoVenta pv
					ON pv.IdProducto = dc.IdProducto
				WHERE
					c.Fecha >= @FechaInicio
					AND c.Fecha <  @FinExclusivo
				ORDER BY
					c.Fecha, c.IdCompra;
			END;
			GO
			/****** Object:  StoredProcedure [dbo].[sp_ReporteEmpleados]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO

			CREATE   PROCEDURE [dbo].[sp_ReporteEmpleados]
				@FechaInicio DATE,
				@FechaFin    DATE
			AS
			BEGIN
				SET NOCOUNT ON;

				DECLARE @FinExclusivo DATETIME2(0)
					= DATEADD(DAY, 1, CAST(@FechaFin AS DATETIME2(0)));

				SELECT
					e.IdEmpleado,
					p.NombreCompleto,
					p.Email,
					pd.Numero          AS Documento,
					pt.Numero          AS Telefono,
					e.FechaIngreso,
					e.Sueldo,
					e.TipoSueldo,
					e.Activo,
					p.CreadoEn
				FROM Empleado e
				INNER JOIN Persona p
					ON p.IdPersona = e.IdPersona
				LEFT JOIN PersonaDocumento pd
					ON pd.IdPersona = p.IdPersona
				   AND pd.EsPrincipal = 1
				LEFT JOIN PersonaTelefono pt
					ON pt.IdPersona = p.IdPersona
				   AND pt.EsPrincipal = 1
				WHERE
					p.CreadoEn >= @FechaInicio
					AND p.CreadoEn <  @FinExclusivo
				ORDER BY
					p.NombreCompleto;
			END;
			GO
			/****** Object:  StoredProcedure [dbo].[sp_ReportePlatosMasVendidos]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO

			CREATE   PROCEDURE [dbo].[sp_ReportePlatosMasVendidos]
				@FechaInicio DATE,
				@FechaFin    DATE,
				@Top INT = 50  -- por defecto 50, puedes pasar 10 si quieres top 10
			AS
			BEGIN
				SET NOCOUNT ON;

				DECLARE @FinExclusivo DATETIME2(0) = DATEADD(DAY, 1, CAST(@FechaFin AS DATETIME2(0)));

				SELECT TOP (@Top)
					pv.IdProducto,
					pv.Nombre,

					CAST(SUM(dp.Cantidad) AS DECIMAL(18,2)) AS CantidadVendida,
					CAST(SUM(dp.Subtotal) AS DECIMAL(18,2)) AS MontoVendido,

					-- info opcional por si la quieres en el reporte
					pv.Activo,
					pv.Itbis,
					pv.IdCategoria,
					pv.IdProductoTipo
				FROM Pedido p
				INNER JOIN DetallePedido dp
					ON dp.IdPedido = p.IdPedido
				INNER JOIN ProductoVenta pv
					ON pv.IdProducto = dp.IdProducto
				WHERE
					p.Fecha >= @FechaInicio
					AND p.Fecha <  @FinExclusivo
					AND p.Estado = 'Facturado'
				GROUP BY
					pv.IdProducto, pv.Nombre, pv.Activo, pv.Itbis, pv.IdCategoria, pv.IdProductoTipo
				ORDER BY
					CantidadVendida DESC, MontoVendido DESC, pv.Nombre ASC;
			END;
			GO
			/****** Object:  StoredProcedure [dbo].[sp_ReporteProveedores]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO



			CREATE   PROCEDURE [dbo].[sp_ReporteProveedores]
				@FechaInicio DATE,
				@FechaFin    DATE
			AS
			BEGIN
				SET NOCOUNT ON;

				DECLARE @FinExclusivo DATETIME2(0)
					= DATEADD(DAY, 1, CAST(@FechaFin AS DATETIME2(0)));

				SELECT
					pr.IdProveedor,
					p.NombreCompleto,
					p.Email,
					pd.Numero          AS Documento,
					pt.Numero          AS Telefono,
					pr.Activo,
					pr.Informal,
					p.CreadoEn
				FROM Proveedor pr
				INNER JOIN Persona p
					ON p.IdPersona = pr.IdPersona
				LEFT JOIN PersonaDocumento pd
					ON pd.IdPersona = p.IdPersona
				   AND pd.EsPrincipal = 1
				LEFT JOIN PersonaTelefono pt
					ON pt.IdPersona = p.IdPersona
				   AND pt.EsPrincipal = 1
				WHERE
					p.CreadoEn >= @FechaInicio
					AND p.CreadoEn <  @FinExclusivo
				ORDER BY
					p.NombreCompleto;
			END;
			GO
			/****** Object:  StoredProcedure [dbo].[sp_ReporteStock]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO

			CREATE   PROCEDURE [dbo].[sp_ReporteStock]
			AS
			BEGIN
				SET NOCOUNT ON;

				SELECT
					IdProducto,
					Nombre,
					CodigoBarra,
					Existencia,
					PrecioCompra,
					PrecioVenta,
					Activo,
					IdUnidadMedida
				FROM ProductoVenta
				ORDER BY Nombre;
			END;
			GO
			/****** Object:  StoredProcedure [dbo].[sp_ReporteVentas]    Script Date: 8/4/2026 1:44:22 a. m. ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			CREATE   PROCEDURE [dbo].[sp_ReporteVentas]
				@FechaInicio DATE,
				@FechaFin    DATE
			AS
			BEGIN
				SET NOCOUNT ON;

				-- Incluye todo el día de @FechaFin
				DECLARE @FinExclusivo DATETIME2(0) = DATEADD(DAY, 1, CAST(@FechaFin AS DATETIME2(0)));

				SELECT
					p.IdPedido,
					p.Fecha,
					p.Origen,
					p.IdMesa,
					p.NombreCliente,
					p.Direccion,
					p.Estado,
					p.CreadoPor,
					p.Comprobante,

					-- Totales
					CAST(ISNULL(SUM(dp.Subtotal), 0) AS DECIMAL(18,2)) AS SubtotalDetalle,
					CAST(ISNULL(p.Total, 0) AS DECIMAL(18,2))          AS TotalPedido,

					-- Métricas
					CAST(ISNULL(SUM(dp.Cantidad), 0) AS DECIMAL(18,2)) AS TotalItems
				FROM Pedido p
				LEFT JOIN DetallePedido dp
					ON dp.IdPedido = p.IdPedido
				WHERE
					p.Fecha >= @FechaInicio
					AND p.Fecha <  @FinExclusivo
					AND p.Estado = 'Facturado'
				GROUP BY
					p.IdPedido, p.Fecha, p.Origen, p.IdMesa, p.NombreCliente,
					p.Direccion, p.Estado, p.CreadoPor, p.Comprobante, p.Total
				ORDER BY
					p.Fecha ASC;
			END;
			GO
			USE [master]
			GO
			ALTER DATABASE [GloriaRestaurant] SET  READ_WRITE 
			GO
            ";
    }
}