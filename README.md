# AutoTallerManager Backend

AutoTallerManager es una plataforma integral de gestión para talleres mecánicos de vehículos. El backend proporciona una API RESTful robusta diseñada bajo principios de arquitectura limpia y acoplamiento débil para garantizar la integridad relacional de los datos del taller, el procesamiento seguro de transacciones financieras, control de inventario y gestión operativa completa.

## Tecnologías Utilizadas
* **Plataforma:** .NET 10 (C#)
* **API Framework:** ASP.NET Core Web API
* **Base de Datos:** PostgreSQL
* **ORM:** Entity Framework Core (Code First)
* **Mapeo:** Mapster
* **Autenticación:** JWT (JSON Web Tokens)
* **Limitación de Peticiones:** AspNetCoreRateLimit

---

## Características y Funcionalidades

### 1. Gestión de Clientes y Vehículos
* **Registro de Clientes:** Registro integral de clientes con datos de contacto (nombre, teléfono, correo)
* **Vehículos Asociados:** Cada cliente puede tener múltiples vehículos con información detallada (marca, modelo, año, VIN/chasis)
* **Historial de Kilometraje:** Seguimiento automático del kilometraje registrado en cada servicio
* **Validaciones de Integridad:** Validación de VIN únicos para evitar duplicados en el sistema
* **Control IDOR:** Protección de acceso directo a información de otros clientes

### 2. Órdenes de Servicio
* **Creación de Órdenes:** Generación de órdenes de servicio vinculadas a vehículos de clientes
* **Servicios Personalizados:** Agregación de servicios del taller a la orden (mano de obra con horas presupuestadas)
* **Gestión de Repuestos:** Incorporación de repuestos/piezas a la orden con validación de inventario
* **Asignación de Mecánicos:** Asignación de técnicos responsables a cada orden
* **Diagnóstico Técnico:** Registro de diagnósticos detallados del problema encontrado
* **Horas Reales:** Registro de horas reales trabajadas (diferente a horas presupuestadas)
* **Cálculo de Totales:** Generación automática de resúmenes con subtotal de mano de obra, repuestos e impuestos
* **Estados de Orden:** Seguimiento del estado (Creada, En Progreso, Completada, Cerrada)
* **Consulta de Órdenes:** Los clientes pueden ver sus propias órdenes de servicio

### 3. Gestión de Garantías
* **Reclamos de Garantía:** Registro de reclamos por defectos en servicios previos
* **Análisis de Fallas:** Documentación del motivo de la falla y resolución del dictamen
* **Órdenes de Corrección:** Generación automática de nuevas órdenes de servicio para la rectificación
* **Trazabilidad:** Vinculación entre orden original (fallida) y orden de garantía

### 4. Facturación y Pagos
* **Generación de Facturas:** Creación de facturas con numeración automática (FACT-AÑO-NÚMERO)
* **Detalles de Factura:** Incluye subtotales de mano de obra, repuestos, impuestos y total neto
* **Estados de Pago:** Seguimiento de estados (Pendiente, Parcial, Pagada, Anulada)
* **Registro de Pagos:** Documentación de pagos recibidos por diferentes medios
* **Cierre de Órdenes:** Las órdenes se cierran automáticamente al generar factura
* **Precisión Financiera:** Uso de precisión `decimal(18,2)` para cálculos monetarios exactos

### 5. Servicios del Taller
* **Catálogo de Servicios:** Gestión del registro de servicios disponibles
* **Valores Estandarizados:** Definición de costos y tiempos de mano de obra por servicio
* **Flexibilidad:** Posibilidad de crear nuevos servicios según necesidades del taller

### 6. Gestión de Inventario y Repuestos
* **Control de Stock:** Seguimiento automático de existencias de repuestos/piezas
* **Protección de Sobreventa:** Validación transaccional que impide vender más repuestos de los disponibles
* **Validación Previa:** Consulta directa en BD antes de asignar piezas a órdenes
* **Rollback Automático:** Si no hay stock suficiente, se revierte la transacción completamente
* **Historial de Movimientos:** Registro de entrada y salida de repuestos

### 7. Gestión de Usuarios y Autenticación
* **Registro de Usuarios:** Creación de cuentas para acceso al sistema
* **Múltiples Roles:** Admin, Recepcionista, Mecánico, Cliente
* **Autenticación JWT:** Tokens seguros basados en estándares JWT
* **Credenciales Seguras:** Contraseñas hasheadas con algoritmos de seguridad
* **Usuario Admin Automático:** Creación automática de usuario administrador por defecto en primera ejecución

### 8. Control de Acceso y Seguridad
* **RBAC (Control de Acceso Basado en Roles):**
  - `Admin`: Acceso total al sistema, gestión de usuarios y configuración
  - `Recepcionista`: Gestión de clientes, órdenes, facturación y citas
  - `Mecánico`: Registro de diagnósticos, horas trabajadas y servicios
  - `Cliente`: Visualización de propias órdenes y datos personales
* **Protección IDOR:** Los endpoints personales extraen identificadores del token JWT, no de parámetros del cliente
* **Rate Limiting:** Limitación de peticiones por IP para prevenir ataques
  - `/api/usuarios/login` y `/api/usuarios/registrar`: Máximo 5 peticiones/minuto
  - `/api/ordenes/*`: Máximo 60 peticiones/minuto
  - Respuestas HTTP 429 con esquema JSON estructurado

### 9. Citas y Programación
* **Reserva de Citas:** Clientes pueden programar citas para servicios específicos
* **Información de Síntomas:** Registro de notas y síntomas reportados por el cliente
* **Vinculación Automática:** Las citas se convierten en órdenes de servicio

### 10. Auditoria y Trazabilidad
* **Middleware de Auditoría:** Registro de acciones realizadas en el sistema
* **Timestamps:** Registro automático de fechas y horas de creación y modificación
* **Integridad de Datos:** Restricciones `DeleteBehavior.Restrict` para evitar eliminación de registros referenciados

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

### Variables de Entorno

El sistema soporta configuración mediante variables de entorno con fallback automático. Las variables se cargan en este orden de prioridad:

1. **Archivo `.env`** (en la raíz del proyecto o carpeta padre)
2. **Variables de Entorno del Sistema Operativo**
3. **Archivo `appsettings.json`**
4. **Valores por defecto seguros para desarrollo**

**Variables de entorno disponibles:**

| Variable | Descripción | Fallback Desarrollo | Obligatoria |
|----------|-------------|---------------------|-------------|
| `JWT_SECRET_KEY` | Clave secreta para firmar tokens JWT | `Development_Safe_Fallback_Secret_Key_2026_Min_32_Chars!` | No |
| `DB_CONNECTION_STRING` | Cadena de conexión a PostgreSQL | `Host=localhost;Database=autotaller;Username=postgres;Password=postgres` | No |

**Ejemplo de archivo `.env`:**
```env
JWT_SECRET_KEY=tu_clave_secreta_super_robusta_minimo_32_caracteres_2026
DB_CONNECTION_STRING=Host=tuhost.com;Database=tudb;Username=usuario;Password=contraseña
```

### Pasos Técnicos para Despliegue Local

1. **Clonar el repositorio:**
   ```bash
   git clone <URL_DEL_REPOSITORIO>
   cd AutoTallerManager
   ```

2. **Configurar variables de entorno (opcional):**
   - Crear un archivo `.env` en la raíz del proyecto con tus variables
   - O establecer variables de entorno en tu sistema operativo
   - Si no configuras nada, se usan los valores por defecto para desarrollo local

3. **Ejecutar migraciones de Entity Framework Core:**
   Las configuraciones de las tablas (incluyendo restricciones `DeleteBehavior.Restrict` y precisión `decimal(18,2)`) se aplicarán automáticamente sobre PostgreSQL:
   ```bash
   dotnet ef database update --project AutoTallerManager.Infrastructure --startup-project AutoTallerManager.Api
   ```

4. **Ejecutar la aplicación:**
   ```bash
   dotnet run --project AutoTallerManager.Api
   ```
   
   La API estará disponible en `http://localhost:5106`
   
   Swagger UI estará disponible en `http://localhost:5106/swagger/index.html`

### Seeding Inicial de Datos

Al iniciar la aplicación por primera vez, el sistema ejecuta automáticamente `DatabaseInitializer` que:

1. **Aplica migraciones pendientes** de forma segura
2. **Crea usuario administrador por defecto** si no existe:
   - Email: `admin@autotaller.com`
   - Contraseña: `Admin123*`
   - Rol: `Admin`
3. **Siembra roles inmutables**: `Admin`, `Mecanico`, `Recepcionista`, `Cliente`
4. **Carga catálogo de servicios del taller** (cambios de aceite, alineaciones, revisiones, etc.)
5. **Carga medios de pago disponibles**:
   - Efectivo (no permite cuotas)
   - Tarjeta de Crédito (permite cuotas)
   - Tarjeta de Débito (no permite cuotas)
   - Transferencia Bancaria (no permite cuotas)
6. **Carga catálogo de repuestos e insumos** (aceites, filtros, bujías, pastillas de freno, etc.)

El seeding es **idempotente**: si los datos ya existen, se omite el sembrado nuevamente.

---

## Documentación de Endpoints

### Base URL
```
http://localhost:5106/api
```

### Autenticación
Todos los endpoints (excepto login y registro) requieren token JWT en el header:
```
Authorization: Bearer <tu_token_jwt>
```

### Endpoints Principales

#### Autenticación y Usuarios

**Login:**
```http
POST /api/usuarios/login
Content-Type: application/json

{
  "correo": "admin@autotaller.com",
  "contrasena": "Admin123*"
}
```

**Respuesta exitosa:**
```json
{
  "exitoso": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "usuario": {
    "id": 1,
    "nombre": "Administrador del Sistema",
    "correo": "admin@autotaller.com",
    "rol": "Admin"
  }
}
```

**Registrar Usuario:**
```http
POST /api/usuarios/registrar
Content-Type: application/json

{
  "nombre": "Juan Pérez",
  "correo": "juan@example.com",
  "contrasena": "Password123*",
  "rol": "Mecanico"
}
```

#### Gestión de Clientes

**Registrar Cliente con Vehículos:**
```http
POST /api/clientes/registrar-con-vehiculo
Authorization: Bearer <token>
Content-Type: application/json

{
  "nombre": "Carlos López",
  "telefono": "555-1234",
  "correo": "carlos@example.com",
  "vehiculoInicial": {
    "marca": "Toyota",
    "modelo": "Corolla",
    "anio": 2022,
    "vin": "12345ABCDE67890FG"
  }
}
```

**Listar Clientes:**
```http
GET /api/clientes?pageNumber=1&pageSize=10
Authorization: Bearer <token>
```

**Obtener Cliente por ID:**
```http
GET /api/clientes/{id}
Authorization: Bearer <token>
```

#### Órdenes de Servicio

**Crear Orden:**
```http
POST /api/ordenes
Authorization: Bearer <token>
Content-Type: application/json

{
  "vehiculoId": 1,
  "clienteId": 1,
  "diagnosticoInicial": "Motor con ruido"
}
```

**Agregar Servicio a Orden:**
```http
POST /api/ordenes/{ordenId}/servicios
Authorization: Bearer <token>
Content-Type: application/json

{
  "ordenId": 1,
  "servicioTallerId": 1,
  "horasPresupuestadas": 2.5
}
```

**Agregar Repuesto a Orden:**
```http
POST /api/ordenes/{ordenId}/repuestos
Authorization: Bearer <token>
Content-Type: application/json

{
  "ordenId": 1,
  "repuestoId": 5,
  "cantidadSolicitada": 2
}
```

**Obtener Mis Órdenes (Cliente):**
```http
GET /api/ordenes/mis-ordenes?pageNumber=1&pageSize=10
Authorization: Bearer <token_cliente>
```

**Calcular Totales de Orden:**
```http
GET /api/ordenes/{id}/totales
Authorization: Bearer <token>
```

**Respuesta:**
```json
{
  "ordenId": 1,
  "subtotalManoObra": 75.00,
  "subtotalRepuestos": 50.00,
  "impuestos": 20.00,
  "totalNeto": 145.00
}
```

**Facturar y Cerrar Orden:**
```http
POST /api/ordenes/facturar
Authorization: Bearer <token>
Content-Type: application/json

{
  "ordenId": 1,
  "numeroFactura": "FACT-2026-0001",
  "medioPagoId": 1
}
```

#### Repuestos

**Crear Repuesto (Admin):**
```http
POST /api/repuestos
Authorization: Bearer <token_admin>
Content-Type: application/json

{
  "codigo": "OIL-5L-10W40",
  "descripcion": "Aceite de Motor Sintético 10W40 (5 Litros)",
  "stock": 25,
  "precioUnitario": 45.00
}
```

**Listar Repuestos:**
```http
GET /api/repuestos?pageNumber=1&pageSize=10
Authorization: Bearer <token>
```

#### Garantías

**Registrar Reclamo de Garantía:**
```http
POST /api/garantias/registrar
Authorization: Bearer <token>
Content-Type: application/json

{
  "ordenOriginalId": 5,
  "motivoFalla": "El cambio de aceite causó fuga",
  "resolucionDictamen": "Aprobada (Costo Taller)"
}
```

### Códigos de Respuesta HTTP

| Código | Significado |
|--------|------------|
| `200` | OK - Solicitud exitosa |
| `201` | Created - Recurso creado exitosamente |
| `400` | Bad Request - Datos inválidos o validación fallida |
| `401` | Unauthorized - Token inválido o expirado |
| `403` | Forbidden - Usuario sin permisos suficientes |
| `404` | Not Found - Recurso no encontrado |
| `429` | Too Many Requests - Se excedió el rate limit |
| `500` | Internal Server Error - Error del servidor |

### Rate Limiting

El sistema implementa limitación de peticiones por IP:

| Endpoint | Límite |
|----------|--------|
| `/api/usuarios/login` | 5 peticiones/minuto |
| `/api/usuarios/registrar` | 5 peticiones/minuto |
| `/api/ordenes/*` | 60 peticiones/minuto |

Cuando se excede el límite, la respuesta es:
```json
{
  "status": 429,
  "mensaje": "Has superado el límite de peticiones permitido. Por favor, inténtelo de nuevo más tarde."
}
```

---

## Seguridad y Limitación de Peticiones

* **RBAC (Control de Acceso Basado en Roles):** El sistema utiliza JWT Bearer. Los endpoints de administración crítica (ej. alta y baja de repuestos) requieren rol `Admin`, mientras que la gestión operativa es asignada de manera granular a `Mecanico`, `Recepcionista` y `Cliente`.
* **Mitigación IDOR:** Los endpoints personales (como `GetMisOrdenes`) extraen el identificador de usuario directo del token autenticado (`NameIdentifier`) en lugar de aceptar parámetros arbitrarios del cliente, evitando el acceso no autorizado a información ajena.
* **Rate Limiting:** El middleware `AspNetCoreRateLimit` bloquea ataques de fuerza bruta y DDoS en base a reglas de tasa por IP.
* **Secretos Seguros:** El secreto JWT se obtiene desde variables de entorno, nunca hardcodeado en producción. Fallback seguro solo para desarrollo local.
* **Transacciones Atómicas:** Protección mediante transacciones relacionales para evitar inconsistencias de datos.

---

## Desarrolladores
* **Santiago Gallo (santiagoGal7)**
* **Kevin Pico (kvinnxz)**
