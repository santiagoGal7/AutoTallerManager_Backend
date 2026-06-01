# AutoTallerManager Backend

AutoTallerManager es el backend para la plataforma de gestión de órdenes, clientes e inventario de un taller mecánico de vehículos. El sistema está diseñado bajo principios de arquitectura limpia y acoplamiento débil para garantizar la integridad relacional de los datos del taller y el procesamiento seguro de transacciones financieras e inventario.

## Tecnologías Utilizadas
* **Plataforma:** .NET 10 (C#)
* **API Framework:** ASP.NET Core Web API
* **Base de Datos:** PostgreSQL
* **ORM:** Entity Framework Core (Code First)
* **Mapeo:** Mapster
* **Autenticación:** JWT (JSON Web Tokens)
* **Limitación de Peticiones:** AspNetCoreRateLimit

---

## Desafíos Técnicos Resueltos

### 1. Implementación de Arquitectura Hexagonal y Limpia
Para asegurar la extensibilidad del sistema a largo plazo y evitar el acoplamiento directo con frameworks o motores de base de datos específicos, la solución está estructurada en cuatro capas desacopladas:
* **Domain:** Entidades de dominio puras libres de dependencias externas.
* **Application:** Interfaces del sistema, DTOs de comunicación y lógica de negocio pura de los servicios.
* **Infrastructure:** Repositorios genéricos, persistencia relacional con PostgreSQL vía EF Core y la implementación del patrón Unit of Work.
* **Api (Presentation):** Controladores de entrada, middlewares de excepciones globales, autenticación JWT y configuración de Rate Limiting.

### 2. Atomicidad en el Registro de Usuarios y Clientes (Unit of Work)
El registro de un cliente requiere la creación síncrona de su entidad de dominio, sus vehículos asociados, su primer registro de kilometraje y su cuenta de usuario con el rol correspondiente. Para evitar estados de datos inconsistentes (ej. crear el cliente pero fallar en la creación de sus credenciales), la lógica está encapsulada dentro del patrón *Unit of Work*.
* Toda la operación de inserción es rastreada por un único contexto y ejecutada en una sola llamada a `CompleteAsync()`.
* Para registrar usuarios generales y clientes que posean cuentas de sistema complejas, se inicia una transacción relacional física en la base de datos a través de `BeginTransactionAsync()`, asegurando un rollback automático de todos los inserts si ocurre un fallo en cualquiera de las fases del registro.

### 3. Protección de Inventario Mediante Transacciones Relacionales
El control de existencias en talleres es crítico para evitar el descalce y la sobreventa de repuestos. El método `AgregarRepuestoAOrdenAsync` fue refactorizado para operar bajo una transacción relacional explícita:
* **Validación Previa:** Antes de procesar la asignación de piezas a una orden de servicio, se consulta directamente en la base de datos el stock actual del repuesto.
* **Restricción de Negocio:** Si `Stock < CantidadSolicitada`, la aplicación interrumpe el flujo lanzando una `InvalidOperationException`.
* **Rollback Automático:** La excepción aborta la operación y ejecuta un rollback total de la transacción SQL activa. El stock se decrementa únicamente cuando la orden de servicio está activa y el stock real es suficiente, persistiendo los datos de forma atómica.

---

## Instalación y Configuración

### Requisitos Previos
* .NET 10 SDK
* Instancia de PostgreSQL activa

### Pasos Técnicos para Despliegue Local

1. **Clonar el repositorio:**
   ```bash
   git clone <URL_DEL_REPOSITORIO>
   cd AutoTallerManager
   ```

2. **Configurar la base de datos:**
   Modifique la cadena de conexión en el archivo `AutoTallerManager.Api/appsettings.json` apuntando a su servidor PostgreSQL local o en la nube:
   ```json
   "ConnectionStrings": {
     "PostgresConnection": "Host=localhost;Database=autotaller_db;Username=postgres;Password=tu_contrasena"
   }
   ```

3. **Ejecutar migraciones de Entity Framework Core:**
   Las configuraciones de las tablas (incluyendo restricciones `DeleteBehavior.Restrict` y precisión `decimal(18,2)`) se aplicarán automáticamente sobre PostgreSQL:
   ```bash
   dotnet ef database update --project AutoTallerManager.Infrastructure --startup-project AutoTallerManager.Api
   ```

4. **Ejecutar la aplicación:**
   ```bash
   dotnet run --project AutoTallerManager.Api
   ```
   *Nota:* Al iniciar por primera vez, el sistema sembrará automáticamente un usuario administrador por defecto (`admin@autotaller.com` / `Admin123*`) si la tabla de usuarios está vacía.

---

## Seguridad y Limitación de Peticiones

* **RBAC (Control de Acceso Basado en Roles):** El sistema utiliza JWT Bearer. Los endpoints de administración crítica (ej. alta y baja de repuestos) requieren rol `Admin`, mientras que la gestión operativa es asignada de manera granular a `Mecanico`, `Recepcionista` y `Cliente`.
* **Mitigación IDOR:** Los endpoints personales (como `GetMisOrdenes`) extraen el identificador de usuario directo del token autenticado (`NameIdentifier`) en lugar de aceptar parámetros arbitrarios del cliente, evitando el acceso no autorizado a información ajena.
* **Rate Limiting:** El middleware `AspNetCoreRateLimit` bloquea ataques de fuerza bruta y DDoS en base a reglas de tasa por IP:
  * `/api/usuarios/login` y `/api/usuarios/registrar`: Máximo 5 peticiones por minuto.
  * `/api/ordenes/*`: Máximo 60 peticiones por minuto.
  * Respuestas HTTP 429 con un esquema estructurado JSON.

---

## Desarrolladores
* **Santiago Gallo (santiagoGal7)**
* **Kevin Pico (kvinnxz)** 
