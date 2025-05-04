# Documentación del Proyecto: Sistema de Gestión de Inventarios

## Descripción General del Proyecto

El **Sistema de Gestión de Inventarios** es una aplicación web creada con **C# y ASP.NET CORE MVC** para ayudar a pequeñas empresas a gestionar eficientemente sus inventarios. Las principales funcionalidades incluyen:

- **Registro de productos** con detalles como nombre, categoría, precio y cantidad en stock.
- **Gestión de movimientos** de inventario (entradas y salidas).
- **Generación de reportes** de stock.
- **Alertas de inventario bajo**, permitiendo que los usuarios tomen acción antes de quedarse sin stock.
- **Autenticación de usuarios** y control de acceso.

## Objetivo del Proyecto

Desarrollar una herramienta eficiente que permita gestionar inventarios, automatizar las operaciones diarias y asegurar que las empresas tengan una visibilidad clara sobre el estado de sus productos, entradas y salidas, sin necesidad de realizar cálculos manuales o depender de sistemas de seguimiento ineficientes.

## Tecnologías Utilizadas

1. **ASP.NET Core MVC**: Marco para construir aplicaciones web utilizando el patrón MVC (Model-View-Controller).
2. **Entity Framework Core (EF Core)**: Para gestionar la base de datos relacional usando el modelo ORM (Object-Relational Mapping).
3. **SQL Server**: Base de datos para almacenar la información del inventario, productos, y usuarios.
4. **iTextSharp**: Biblioteca para generar archivos PDF, utilizada en la generación de reportes.
5. **EPPlus**: Biblioteca para generar archivos Excel, utilizada en la generación de reportes.
6. **css/JS**: Para mejorar la experiencia de usuario y proporcionar un diseño responsive.

## Paquetes Usados

1. **Microsoft.EntityFrameworkCore**: Permite usar Entity Framework Core para el acceso a bases de datos relacionales.
2. **Microsoft.EntityFrameworkCore.SqlServer**: Implementación específica para trabajar con SQL Server.
3. **Microsoft.EntityFrameworkCore.Tools**: Proporciona herramientas como migraciones para la gestión de la base de datos desde la línea de comandos.
4. **Microsoft.AspNetCore.Authentication.Cookies**: Habilita la autenticación basada en cookies en aplicaciones ASP.NET Core.

### Comandos Importantes

1. **Agregar Migración**:
   ```bash
   Add-Migration "NombreDeLaMigracion"
   ```
2. **Aplicar Cambios a la Base de Datos**:
   ```bash
   Update-Database
   ```
3. **Eliminar Migración**:
   ```bash
   Remove-Migration
   ```

## Funcionalidades Principales

### 1. Gestión de Productos

Permite a los usuarios agregar, editar y eliminar productos del sistema. Cada producto tiene propiedades como nombre, descripción, precio, cantidad en stock, y categoría.

### 2. Gestión de Movimientos de Inventario

Registra las entradas (nuevas adquisiciones) y salidas (ventas o pérdidas) de productos, actualizando automáticamente las cantidades de stock. Mantiene un historial de todos los movimientos, mostrando detalles de cada transacción.

### 3. Alertas de Inventario Bajo

Automáticamente detecta cuándo un producto tiene un nivel de stock inferior al mínimo establecido y genera alertas para que los usuarios puedan reabastecerse antes de que se agote.

### 4. Generación de Reportes

El sistema permite generar reportes de inventario en formato **PDF** y **Excel**, facilitando la revisión de productos en stock y niveles mínimos.

### 5. Seguridad y Autenticación

El sistema utiliza **autenticación basada en cookies** para gestionar la sesión de los usuarios, asegurando que solo usuarios autenticados puedan acceder a la aplicación. Los roles permiten definir niveles de acceso diferenciados.

## Buenas Prácticas y Consideraciones

1. **Uso de `var` vs declaración explícita**: Usa `var` cuando el tipo de dato es obvio o simplifica el código; utiliza una declaración explícita cuando quieras más claridad.
2. **`FirstAsync` vs `FirstOrDefaultAsync`**: Usa `FirstAsync` cuando esperas siempre un resultado (y lanzar una excepción si no hay datos). Usa `FirstOrDefaultAsync` si quieres manejar el caso en el que no haya ningún resultado (retornará `null`).

3. **Validación de cadenas**:
   - **`IsNullOrEmpty`**: Verifica si la cadena es nula o vacía.
   - **`IsNullOrWhiteSpace`**: Además de comprobar si es nula o vacía, también considera las cadenas que contienen solo espacios en blanco como vacías.

## Puntos a Mejorar
1. **Mensajes de Confirmación:** Agregar un modelo para manejar mensajes de confirmación cuando se realicen operaciones como eliminación o actualización de datos.
2. **Reporte de Movimiento:** Implementar en el controlador de Reportes, la generación de reporte para Movimiento de Inventario similar a la de Productos.
3. **Paginación:** Agregamos la lógica de paginación con su enumeración para las tablas desde el backend y en frontend el diseño.
4. **Pasar el proyecto de MVC a API REST:** Pasar el proyecto hecho en ASP.NET Core MVC a ASP.NET Core WEb API y usar Angular como la parte frontend.
5. **Restructurar el proyecto:** Separar el código del Controlador en tres partes, Interfaces, Services y controlador.
6. **Actualizar los paquetes de Excel y PDF:** Actualizar EPPlus y iTextSharp por otros paquetes que sean compatibles que la version de .NET8 o 9, gratuitas.

### 📌 **Tabla comparativa**
| Librería       | Tipo    | Licencia | .NET 8 | Ventajas                           |
|----------------|---------|----------|--------|------------------------------------|
| **QuestPDF**   | PDF     | MIT      | ✅     | Diseño fluido, moderno.            |
| **PuppeteerSharp** | PDF  | MIT      | ✅     | PDF desde HTML (flexible).         |
| **ClosedXML**  | Excel   | MIT      | ✅     | Más fácil que EPPlus.              |
| **NPOI**       | Excel   | Apache 2 | ✅     | Soporta .xls y .xlsx.              |

---

- **Para PDF:** se puede usar **QuestPDF** (si necesitas diseño programático) o **PuppeteerSharp** (si prefieres HTML → PDF).  
- **Para Excel:** se puede usar **ClosedXML** (similar a EPPlus pero con licencia MIT).  

Ambas opciones son **gratuitas, compatibles con .NET 8 y sin restricciones de licencia** para proyectos comerciales.  

## Conclusión

Este **Sistema de Gestión de Inventarios** proporciona a las pequeñas empresas una herramienta robusta y eficiente para gestionar su inventario, asegurando un control completo sobre los productos disponibles, el registro de entradas y salidas, así como la generación de reportes claros y alertas automáticas. Con un enfoque en la facilidad de uso y la seguridad, este sistema busca simplificar las operaciones diarias relacionadas con el inventario.

# Preview
![preview](/preview/preview.gif)

## Auth
### Login de usuario
![preview](/preview/login.jpeg)
### Registro de usuario
![preview](/preview/registro.jpeg)

## Dashboard Admin
### Usuarios Lista
![preview](/preview/usuario-list.jpeg)
### Usuario Nuevo
![preview](/preview/user-new.jpeg)
### Roles List
![preview](/preview/rol-list.jpeg)

## Dashboard Empleado
### Productos Lista
![preview](/preview/producto-list.jpeg)

### Producto Editar
![preview](/preview/producto-edit.jpeg)

### Producto Nuevo
![preview](/preview/producto-nuevo.jpeg)

### Categorías Lista
![preview](/preview/categoria-list.jpeg)

### Categorías Nuevo
![preview](/preview/categoria-nuevo.jpeg)

### Movimiento de Inventario
![preview](/preview/movimiento-list.jpeg)

### Movimiento de Nuevo
![preview](/preview/movimiento-nuevo.jpeg)

### Notificación
![preview](/preview/notificacion.jpeg)

### Reporte
![preview](/preview/reportes.jpeg)

### Perfil
![preview](/preview/perfil.jpeg)