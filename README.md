# Sistema de Gestión de Inventarios
> Objetivo: Crea una aplicación para pequeñas empresas que permita gestionar inventarios, registrar entradas y salidas de productos, generar reportes de stock y alertas para productos con bajos niveles de inventario. 

## Paquetes usados
1. **Microsoft.EntityFrameworkCore**: Este es el núcleo de Entity Framework Core, una herramienta ORM (Object-Relational Mapper) que permite trabajar con bases de datos usando modelos de objetos en C#. Debe instalarse, ya que no está integrado por defecto en .NET.

2. **Microsoft.EntityFrameworkCore.SqlServer**: Proporciona la implementación específica de SQL Server para Entity Framework Core, permitiendo que las aplicaciones se conecten y gestionen bases de datos de SQL Server. Este paquete también necesita ser instalado, no está incluido por defecto.

3. **Microsoft.EntityFrameworkCore.Tools**: Incluye herramientas que facilitan tareas como migraciones y generación de código desde la línea de comandos para Entity Framework Core. No viene integrado en .NET y debe instalarse manualmente si deseas utilizar estas herramientas.

4. **Microsoft.AspNetCore.Authentication.Cookies**: Este paquete habilita la autenticación basada en cookies en aplicaciones ASP.NET Core.

## Comandos Usados en el proyecto
1. Realiza las migraciones de las propiedades configuradas
```bash
add-migration "mensaje"
```

2. Actualiza/agrega la migración a la base de datos
```bash
update-database
```

# Documentación


```var``` vs ```declaración explícita```: Usa var cuando el tipo es obvio o el código es más limpio, y usa la declaración explícita cuando quieres más claridad o cuando el tipo no es evidente.
```FirstAsync``` vs ```FirstOrDefaultAsync```: Usa ```FirstAsync``` cuando esperas que siempre haya un resultado y quieres que se lance una excepción si no lo hay; usa ```FirstOrDefaultAsync``` cuando el resultado puede no existir y quieres manejar el caso de que el resultado sea null.
