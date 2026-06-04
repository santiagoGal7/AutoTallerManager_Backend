# AutoTallerManager Frontend

Frontend estatico servido por la misma API local de AutoTallerManager.

## Ejecutar

1. Inicia el backend:
   ```powershell
   dotnet run --project AutoTallerManager.Api
   ```

2. Abre:
   ```text
   http://localhost:5106
   ```

## Acceso inicial

- API base: `http://localhost:5106/api`
- Correo: `admin@autotaller.com`
- Contrasena: `Admin123*`

## Modulos incluidos

- Login JWT y cierre de sesion
- Panel operativo
- Clientes con vehiculo inicial
- Vehiculos
- Ordenes de servicio
- Repuestos
- Servicios del taller
- Citas
- Facturacion y pagos
- Garantias
- Usuarios y desactivacion de cuentas

El backend sirve esta carpeta en el mismo puerto de la API. Las rutas `/api/...` siguen apuntando al backend y `/` abre la interfaz.
