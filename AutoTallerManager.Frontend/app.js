const API_KEY = "autotaller_api_base";
const SESSION_KEY = "autotaller_session";

const state = {
  apiBase: localStorage.getItem(API_KEY) || `${window.location.origin}/api`,
  session: JSON.parse(localStorage.getItem(SESSION_KEY) || "null"),
  view: "dashboard",
  cache: {
    clientes: [],
    vehiculos: [],
    ordenes: [],
    repuestos: [],
    usuarios: [],
  },
};

const modules = [
  { id: "dashboard", label: "Panel", roles: ["Admin", "Recepcionista", "Mecanico", "Cliente"] },
  { id: "clientes", label: "Clientes", roles: ["Admin", "Recepcionista"] },
  { id: "vehiculos", label: "Vehiculos", roles: ["Admin", "Recepcionista", "Mecanico"] },
  { id: "ordenes", label: "Ordenes", roles: ["Admin", "Recepcionista", "Mecanico", "Cliente"] },
  { id: "inventario", label: "Repuestos", roles: ["Admin", "Recepcionista", "Mecanico"] },
  { id: "servicios", label: "Servicios", roles: ["Admin", "Recepcionista"] },
  { id: "citas", label: "Citas", roles: ["Admin", "Recepcionista", "Cliente"] },
  { id: "facturacion", label: "Facturacion", roles: ["Admin", "Recepcionista"] },
  { id: "garantias", label: "Garantias", roles: ["Admin", "Recepcionista", "Mecanico"] },
  { id: "usuarios", label: "Usuarios", roles: ["Admin"] },
];

const $ = (selector) => document.querySelector(selector);
const app = $("#app");

function canSee(module) {
  return module.roles.includes(state.session?.usuario?.rol);
}

function saveSession(session) {
  state.session = session;
  localStorage.setItem(SESSION_KEY, JSON.stringify(session));
}

function clearSession() {
  state.session = null;
  localStorage.removeItem(SESSION_KEY);
}

function toast(message, type = "ok") {
  const current = $(".toast");
  if (current) current.remove();

  const node = document.createElement("div");
  node.className = `toast ${type === "error" ? "error" : ""}`;
  node.textContent = message;
  document.body.appendChild(node);
  setTimeout(() => node.remove(), 4200);
}

function normalizeError(payload, fallback) {
  if (!payload) return fallback;
  if (typeof payload === "string") return payload;
  if (payload.mensaje) return payload.mensaje;
  if (payload.message) return payload.message;
  if (payload.errors) return Object.values(payload.errors).flat().join(" ");
  return fallback;
}

async function api(path, options = {}) {
  const headers = {
    "Content-Type": "application/json",
    ...(options.headers || {}),
  };

  if (state.session?.token) {
    headers.Authorization = `Bearer ${state.session.token}`;
  }

  const response = await fetch(`${state.apiBase}${path}`, {
    ...options,
    headers,
  });

  const text = await response.text();
  const data = text ? tryJson(text) : null;

  if (!response.ok) {
    if (response.status === 401) {
      clearSession();
      render();
    }
    throw new Error(normalizeError(data, `Error HTTP ${response.status}`));
  }

  return {
    data,
    total: Number(response.headers.get("X-Total-Count") || 0),
  };
}

function tryJson(text) {
  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}

function getFormData(form) {
  const data = Object.fromEntries(new FormData(form).entries());
  for (const key of Object.keys(data)) {
    if (data[key] === "") {
      delete data[key];
      continue;
    }
    const input = form.elements[key];
    if (input?.type === "number") data[key] = Number(data[key]);
    if (input?.type === "datetime-local") data[key] = new Date(data[key]).toISOString();
    if (input?.type === "checkbox") data[key] = input.checked;
  }
  return data;
}

function money(value) {
  const number = Number(value || 0);
  return number.toLocaleString("es-CO", { style: "currency", currency: "COP", maximumFractionDigits: 0 });
}

function date(value) {
  if (!value) return "Sin fecha";
  return new Date(value).toLocaleString("es-CO", { dateStyle: "medium", timeStyle: "short" });
}

function statusPill(value) {
  const text = String(value ?? "Sin estado");
  const lower = text.toLowerCase();
  const tone = lower.includes("cerr") || lower.includes("pag") || lower.includes("complet") || lower === "true"
    ? "ok"
    : lower.includes("pend") || lower.includes("progreso")
      ? "warn"
      : lower.includes("false") || lower.includes("anul")
        ? "bad"
        : "";
  return `<span class="pill ${tone}">${escapeHtml(text)}</span>`;
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function render() {
  if (!state.session) {
    renderLogin();
    return;
  }

  const available = modules.filter(canSee);
  if (!available.some((item) => item.id === state.view)) {
    state.view = available[0]?.id || "dashboard";
  }

  app.innerHTML = `
    <main class="shell">
      <aside class="sidebar">
        <div class="brand-row">
          <div class="brand-mark">AT</div>
          <div>
            <strong>AutoTaller</strong>
            <span class="muted">Manager</span>
          </div>
        </div>
        <nav class="nav">
          ${available.map((item) => `
            <button class="${state.view === item.id ? "active" : ""}" data-view="${item.id}">
              <span>${item.label}</span><span>${navGlyph(item.id)}</span>
            </button>
          `).join("")}
        </nav>
        <div class="user-box">
          <strong>${escapeHtml(state.session.usuario.nombre)}</strong>
          <span class="muted">${escapeHtml(state.session.usuario.correo)}</span>
          <span class="pill">${escapeHtml(state.session.usuario.rol)}</span>
        </div>
        <div class="btn-row">
          <button class="btn secondary" id="logoutBtn">Salir</button>
        </div>
      </aside>
      <section class="content" id="view"></section>
    </main>
  `;

  document.querySelectorAll("[data-view]").forEach((button) => {
    button.addEventListener("click", () => {
      state.view = button.dataset.view;
      render();
    });
  });

  $("#logoutBtn").addEventListener("click", logout);
  renderView();
}

function navGlyph(id) {
  return {
    dashboard: "01",
    clientes: "02",
    vehiculos: "03",
    ordenes: "04",
    inventario: "05",
    servicios: "06",
    citas: "07",
    facturacion: "08",
    garantias: "09",
    usuarios: "10",
  }[id] || "";
}

function renderLogin() {
  app.innerHTML = `
    <main class="auth-shell">
      <section class="auth-board">
        <div class="auth-visual">
          <div class="brand-row">
            <div class="brand-mark">AT</div>
            <strong>AutoTaller Manager</strong>
          </div>
          <h1>Gestion operativa para taller mecanico</h1>
          <p>Clientes, vehiculos, ordenes, inventario, citas, garantias y cobros conectados al backend local.</p>
        </div>
        <div class="auth-form-panel">
          <p class="eyebrow">Acceso</p>
          <h2>Iniciar sesion</h2>
          <p class="muted">Admin inicial: admin@autotaller.com / Admin123*</p>
          <form id="loginForm" class="form-grid">
            <label class="span-2">API base
              <input name="apiBase" value="${escapeHtml(state.apiBase)}" required />
            </label>
            <label class="span-2">Correo
              <input name="correo" type="email" value="admin@autotaller.com" required />
            </label>
            <label class="span-2">Contrasena
              <input name="contrasena" type="password" value="Admin123*" required />
            </label>
            <div class="btn-row span-2">
              <button class="btn" type="submit">Entrar</button>
            </div>
          </form>
        </div>
      </section>
    </main>
  `;

  $("#loginForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const data = getFormData(event.currentTarget);
    state.apiBase = data.apiBase.replace(/\/$/, "");
    localStorage.setItem(API_KEY, state.apiBase);

    try {
      const { data: login } = await api("/usuarios/login", {
        method: "POST",
        body: JSON.stringify({ correo: data.correo, contrasena: data.contrasena }),
      });

      const token = login.token || login.Token;
      const usuario = login.usuario || {
        id: login.id,
        nombre: login.nombre,
        correo: login.correo,
        rol: login.rol,
        activo: login.activo,
      };

      saveSession({ token, usuario });
      toast(`Bienvenido, ${usuario.nombre}`);
      await warmup();
      render();
    } catch (error) {
      toast(error.message, "error");
    }
  });
}

async function logout() {
  try {
    await api("/auth/logout", { method: "POST" });
  } catch {
    // The local token is cleared even if it was already expired or revoked.
  }
  clearSession();
  render();
}

async function warmup() {
  const tasks = [];
  if (["Admin", "Recepcionista"].includes(state.session.usuario.rol)) tasks.push(loadClientes());
  if (["Admin", "Recepcionista", "Mecanico"].includes(state.session.usuario.rol)) {
    tasks.push(loadVehiculos(), loadOrdenes(), loadRepuestos());
  }
  if (state.session.usuario.rol === "Cliente") tasks.push(loadOrdenes());
  if (state.session.usuario.rol === "Admin") tasks.push(loadUsuarios());
  await Promise.allSettled(tasks);
}

function renderView() {
  const views = {
    dashboard: renderDashboard,
    clientes: renderClientes,
    vehiculos: renderVehiculos,
    ordenes: renderOrdenes,
    inventario: renderInventario,
    servicios: renderServicios,
    citas: renderCitas,
    facturacion: renderFacturacion,
    garantias: renderGarantias,
    usuarios: renderUsuarios,
  };
  views[state.view]?.();
}

function setView(html) {
  $("#view").innerHTML = html;
}

function header(title, subtitle, action = "") {
  return `
    <div class="topbar">
      <div>
        <p class="eyebrow">${escapeHtml(state.session.usuario.rol)}</p>
        <h2>${title}</h2>
        <p class="muted">${subtitle}</p>
      </div>
      <div>${action}</div>
    </div>
  `;
}

function table(items, columns, empty = "Sin datos para mostrar") {
  if (!items?.length) return `<div class="empty">${empty}</div>`;
  return `
    <div class="table-wrap">
      <table>
        <thead><tr>${columns.map((col) => `<th>${col.label}</th>`).join("")}</tr></thead>
        <tbody>
          ${items.map((item) => `
            <tr>${columns.map((col) => `<td>${col.render ? col.render(item) : escapeHtml(item[col.key])}</td>`).join("")}</tr>
          `).join("")}
        </tbody>
      </table>
    </div>
  `;
}

async function loadClientes() {
  const { data } = await api("/clientes?pageNumber=1&pageSize=50");
  state.cache.clientes = Array.isArray(data) ? data : [];
}

async function loadVehiculos() {
  const { data } = await api("/vehiculos/listar-vehiculos?pageNumber=1&pageSize=50");
  state.cache.vehiculos = Array.isArray(data) ? data : [];
}

async function loadOrdenes() {
  const endpoint = state.session.usuario.rol === "Cliente" ? "/ordenes/mis-ordenes" : "/ordenes";
  const { data } = await api(`${endpoint}?pageNumber=1&pageSize=50`);
  state.cache.ordenes = Array.isArray(data) ? data : [];
}

async function loadRepuestos() {
  const { data } = await api("/repuestos?pageNumber=1&pageSize=50");
  state.cache.repuestos = Array.isArray(data) ? data : [];
}

async function loadUsuarios() {
  const { data } = await api("/usuarios");
  state.cache.usuarios = Array.isArray(data) ? data : [];
}

function renderDashboard() {
  setView(`
    ${header("Panel operativo", "Resumen rapido del taller y accesos de trabajo.")}
    <section class="cards">
      <div class="metric"><small>Clientes</small><strong>${state.cache.clientes.length}</strong></div>
      <div class="metric"><small>Vehiculos</small><strong>${state.cache.vehiculos.length}</strong></div>
      <div class="metric"><small>Ordenes</small><strong>${state.cache.ordenes.length}</strong></div>
      <div class="metric"><small>Repuestos</small><strong>${state.cache.repuestos.length}</strong></div>
    </section>
    <section class="panel">
      <div class="panel-header">
        <h3>Acciones frecuentes</h3>
        <button class="btn secondary" id="refreshAll">Actualizar</button>
      </div>
      <div class="quick-actions">
        ${modules.filter(canSee).filter((item) => item.id !== "dashboard").slice(0, 6).map((item) => `
          <button data-jump="${item.id}">${item.label}<br><span class="muted">${quickCopy(item.id)}</span></button>
        `).join("")}
      </div>
    </section>
  `);

  $("#refreshAll").addEventListener("click", async () => {
    await warmup();
    toast("Datos actualizados");
    renderDashboard();
  });

  document.querySelectorAll("[data-jump]").forEach((button) => {
    button.addEventListener("click", () => {
      state.view = button.dataset.jump;
      render();
    });
  });
}

function quickCopy(id) {
  return {
    clientes: "registro y consulta",
    vehiculos: "parque automotor",
    ordenes: "servicio y taller",
    inventario: "stock y precios",
    servicios: "catalogo interno",
    citas: "agenda",
    facturacion: "cobros",
    garantias: "reclamos",
    usuarios: "roles y acceso",
  }[id] || "";
}

function renderClientes() {
  setView(`
    ${header("Clientes", "Registro de clientes con vehiculo inicial.")}
    <section class="layout-grid">
      <div class="panel">
        <div class="panel-header">
          <h3>Clientes registrados</h3>
          <button class="btn secondary" id="reloadClientes">Actualizar</button>
        </div>
        ${table(state.cache.clientes, [
          { label: "ID", key: "id" },
          { label: "Nombre", key: "nombre" },
          { label: "Telefono", key: "telefono" },
          { label: "Correo", key: "correo" },
          { label: "Registro", render: (x) => date(x.fechaRegistro) },
          { label: "Vehiculos", render: (x) => x.vehiculos?.length ?? 0 },
        ])}
      </div>
      <div class="panel">
        <h3>Nuevo cliente</h3>
        <form id="clienteForm" class="form-grid">
          <label class="span-2">Nombre<input name="nombre" required /></label>
          <label>Telefono<input name="telefono" required /></label>
          <label>Correo<input name="correo" type="email" required /></label>
          <label>Marca<input name="marca" required /></label>
          <label>Modelo<input name="modelo" required /></label>
          <label>Anio<input name="anio" type="number" min="1900" required /></label>
          <label>VIN<input name="vin" maxlength="17" minlength="17" required /></label>
          <label class="span-2">Kilometraje<input name="kilometraje" type="number" min="0" required /></label>
          <div class="btn-row span-2"><button class="btn" type="submit">Registrar cliente</button></div>
        </form>
      </div>
    </section>
  `);

  $("#reloadClientes").addEventListener("click", refresh(loadClientes, renderClientes));
  $("#clienteForm").addEventListener("submit", submitCliente);
}

async function submitCliente(event) {
  event.preventDefault();
  const data = getFormData(event.currentTarget);
  try {
    await api("/clientes/registrar-con-vehiculo", {
      method: "POST",
      body: JSON.stringify({
        nombre: data.nombre,
        telefono: data.telefono,
        correo: data.correo,
        vehiculoInicial: {
          marca: data.marca,
          modelo: data.modelo,
          anio: data.anio,
          vin: data.vin,
          kilometraje: data.kilometraje,
        },
      }),
    });
    event.currentTarget.reset();
    await Promise.allSettled([loadClientes(), loadVehiculos()]);
    toast("Cliente registrado");
    renderClientes();
  } catch (error) {
    toast(error.message, "error");
  }
}

function renderVehiculos() {
  setView(`
    ${header("Vehiculos", "Consulta del parque automotor y kilometraje reportado.")}
    <section class="panel">
      <div class="panel-header">
        <h3>Vehiculos</h3>
        <button class="btn secondary" id="reloadVehiculos">Actualizar</button>
      </div>
      ${table(state.cache.vehiculos, [
        { label: "ID", key: "id" },
        { label: "Marca", key: "marca" },
        { label: "Modelo", key: "modelo" },
        { label: "Anio", key: "anio" },
        { label: "VIN", key: "vin" },
        { label: "Kilometraje", key: "kilometraje" },
      ])}
    </section>
  `);
  $("#reloadVehiculos").addEventListener("click", refresh(loadVehiculos, renderVehiculos));
}

function renderOrdenes() {
  const canOperate = ["Admin", "Recepcionista", "Mecanico"].includes(state.session.usuario.rol);
  const canOpen = ["Admin", "Recepcionista"].includes(state.session.usuario.rol);
  const canTech = ["Admin", "Mecanico"].includes(state.session.usuario.rol);

  setView(`
    ${header("Ordenes de servicio", "Apertura, diagnostico, servicios, repuestos y totales.")}
    <section class="layout-grid">
      <div class="panel">
        <div class="panel-header">
          <h3>${state.session.usuario.rol === "Cliente" ? "Mis ordenes" : "Ordenes"}</h3>
          <button class="btn secondary" id="reloadOrdenes">Actualizar</button>
        </div>
        ${table(state.cache.ordenes, [
          { label: "ID", key: "id" },
          { label: "Vehiculo", render: (x) => x.vehiculoId ?? x.VehiculoId ?? "" },
          { label: "Cliente", render: (x) => x.clienteId ?? x.ClienteId ?? "" },
          { label: "Estado", render: (x) => statusPill(x.estado ?? x.Estado) },
          { label: "Problema", render: (x) => escapeHtml(x.descripcionProblema ?? x.diagnosticoInicial ?? x.DescripcionProblema ?? "") },
          { label: "Creacion", render: (x) => date(x.fechaCreacion ?? x.FechaCreacion) },
        ], "No hay ordenes cargadas")}
      </div>
      <div class="panel">
        <h3>Acciones</h3>
        ${canOpen ? orderOpenForm() : ""}
        ${canOperate ? orderCommonForms(canTech) : ""}
      </div>
    </section>
  `);

  $("#reloadOrdenes").addEventListener("click", refresh(loadOrdenes, renderOrdenes));
  bindOptional("#ordenForm", submitOrden);
  bindOptional("#servicioOrdenForm", submitServicioOrden);
  bindOptional("#repuestoOrdenForm", submitRepuestoOrden);
  bindOptional("#mecanicoForm", submitMecanico);
  bindOptional("#diagnosticoForm", submitDiagnostico);
  bindOptional("#totalesForm", submitTotales);
}

function orderOpenForm() {
  return `
    <form id="ordenForm" class="form-grid">
      <label>Vehiculo ID<input name="vehiculoId" type="number" min="1" required /></label>
      <label>Cliente ID<input name="clienteId" type="number" min="1" required /></label>
      <label class="span-2">Descripcion del problema<textarea name="descripcionProblema" required></textarea></label>
      <label>Costo estimado<input name="costoEstimado" type="number" min="0" step="0.01" /></label>
      <label>Fecha cita<input name="fechaCita" type="datetime-local" /></label>
      <div class="btn-row span-2"><button class="btn" type="submit">Abrir orden</button></div>
    </form>
    <hr>
  `;
}

function orderCommonForms(canTech) {
  return `
    <form id="totalesForm" class="form-grid">
      <label class="span-2">Orden ID para totales<input name="ordenId" type="number" min="1" required /></label>
      <div class="btn-row span-2"><button class="btn secondary" type="submit">Calcular totales</button></div>
    </form>
    ${["Admin", "Recepcionista"].includes(state.session.usuario.rol) ? `
      <form id="mecanicoForm" class="form-grid">
        <label>Orden ID<input name="ordenId" type="number" min="1" required /></label>
        <label>Mecanico ID<input name="mecanicoId" type="number" min="1" required /></label>
        <div class="btn-row span-2"><button class="btn secondary" type="submit">Asignar mecanico</button></div>
      </form>
    ` : ""}
    ${canTech ? `
      <form id="diagnosticoForm" class="form-grid">
        <label class="span-2">Orden ID<input name="ordenId" type="number" min="1" required /></label>
        <label class="span-2">Diagnostico<textarea name="diagnostico" required></textarea></label>
        <div class="btn-row span-2"><button class="btn secondary" type="submit">Guardar diagnostico</button></div>
      </form>
      <form id="servicioOrdenForm" class="form-grid">
        <label>Orden ID<input name="ordenServicioId" type="number" min="1" required /></label>
        <label>Servicio ID<input name="servicioTallerId" type="number" min="1" required /></label>
        <label>Mano de obra<input name="precioManoObraHistorico" type="number" min="0.01" step="0.01" required /></label>
        <label>Horas estimadas<input name="horasEstimadas" type="number" min="1" required /></label>
        <div class="btn-row span-2"><button class="btn secondary" type="submit">Agregar servicio</button></div>
      </form>
      <form id="repuestoOrdenForm" class="form-grid">
        <label>Orden ID<input name="ordenServicioId" type="number" min="1" required /></label>
        <label>Repuesto ID<input name="repuestoId" type="number" min="1" required /></label>
        <label>Cantidad<input name="cantidad" type="number" min="1" required /></label>
        <label>Precio venta<input name="precioVentaHistorico" type="number" min="0.01" step="0.01" required /></label>
        <div class="btn-row span-2"><button class="btn secondary" type="submit">Agregar repuesto</button></div>
      </form>
    ` : ""}
  `;
}

async function submitOrden(event) {
  await postForm(event, "/ordenes/abrir", loadOrdenes, renderOrdenes, "Orden abierta");
}

async function submitServicioOrden(event) {
  await postForm(event, "/ordenes/agregar-servicio", loadOrdenes, renderOrdenes, "Servicio agregado");
}

async function submitRepuestoOrden(event) {
  await postForm(event, "/ordenes/agregar-repuesto", async () => Promise.all([loadOrdenes(), loadRepuestos()]), renderOrdenes, "Repuesto agregado");
}

async function submitMecanico(event) {
  event.preventDefault();
  const data = getFormData(event.currentTarget);
  try {
    await api(`/ordenes/${data.ordenId}/asignar-mecanico`, {
      method: "POST",
      body: JSON.stringify({ mecanicoId: data.mecanicoId }),
    });
    toast("Mecanico asignado");
    event.currentTarget.reset();
  } catch (error) {
    toast(error.message, "error");
  }
}

async function submitDiagnostico(event) {
  event.preventDefault();
  const data = getFormData(event.currentTarget);
  try {
    await api(`/ordenes/${data.ordenId}/diagnostico`, {
      method: "PATCH",
      body: JSON.stringify({ diagnostico: data.diagnostico }),
    });
    toast("Diagnostico guardado");
    event.currentTarget.reset();
  } catch (error) {
    toast(error.message, "error");
  }
}

async function submitTotales(event) {
  event.preventDefault();
  const data = getFormData(event.currentTarget);
  try {
    const { data: totals } = await api(`/ordenes/${data.ordenId}/totales`);
    toast(`Total neto: ${money(totals.totalNeto)}`);
  } catch (error) {
    toast(error.message, "error");
  }
}

function renderInventario() {
  const isAdmin = state.session.usuario.rol === "Admin";
  setView(`
    ${header("Repuestos", "Stock, precio unitario y alta de catalogo.")}
    <section class="layout-grid">
      <div class="panel">
        <div class="panel-header">
          <h3>Inventario</h3>
          <button class="btn secondary" id="reloadRepuestos">Actualizar</button>
        </div>
        ${table(state.cache.repuestos, [
          { label: "ID", key: "id" },
          { label: "Codigo", key: "codigo" },
          { label: "Descripcion", key: "descripcion" },
          { label: "Stock", key: "stock" },
          { label: "Precio", render: (x) => money(x.precioUnitario) },
          { label: "Activo", render: (x) => statusPill(x.activo) },
        ])}
      </div>
      <div class="panel">
        <h3>Nuevo repuesto</h3>
        ${isAdmin ? `
          <form id="repuestoForm" class="form-grid">
            <label class="span-2">Codigo<input name="codigo" required /></label>
            <label class="span-2">Descripcion<textarea name="descripcion" required></textarea></label>
            <label>Stock<input name="stock" type="number" min="0" required /></label>
            <label>Precio unitario<input name="precioUnitario" type="number" min="0" step="0.01" required /></label>
            <div class="btn-row span-2"><button class="btn" type="submit">Guardar repuesto</button></div>
          </form>
        ` : `<div class="empty">Solo Admin puede registrar repuestos.</div>`}
      </div>
    </section>
  `);
  $("#reloadRepuestos").addEventListener("click", refresh(loadRepuestos, renderInventario));
  bindOptional("#repuestoForm", (event) => postForm(event, "/repuestos", loadRepuestos, renderInventario, "Repuesto registrado"));
}

function renderServicios() {
  setView(`
    ${header("Servicios del taller", "Alta del catalogo de servicios facturables.")}
    <section class="panel">
      <form id="servicioForm" class="form-grid">
        <label>Nombre<input name="nombre" required /></label>
        <label>Tarifa base<input name="tarifaBaseManoObra" type="number" min="0" step="0.01" required /></label>
        <label class="span-2">Descripcion<textarea name="descripcion"></textarea></label>
        <label class="span-2">Activo<select name="activo"><option value="true">Activo</option><option value="false">Inactivo</option></select></label>
        <div class="btn-row span-2"><button class="btn" type="submit">Crear servicio</button></div>
      </form>
    </section>
  `);

  $("#servicioForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const data = getFormData(event.currentTarget);
    data.activo = String(data.activo) === "true";
    try {
      await api("/servicios-taller", { method: "POST", body: JSON.stringify(data) });
      event.currentTarget.reset();
      toast("Servicio creado");
    } catch (error) {
      toast(error.message, "error");
    }
  });
}

function renderCitas() {
  setView(`
    ${header("Citas", "Programacion, confirmacion y conversion a orden.")}
    <section class="layout-grid">
      <div class="panel">
        <h3>Programar cita</h3>
        <form id="citaForm" class="form-grid">
          <label>Cliente ID<input name="clienteId" type="number" min="1" required /></label>
          <label>Vehiculo ID<input name="vehiculoId" type="number" min="1" required /></label>
          <label>Servicio ID<input name="servicioTallerId" type="number" min="1" required /></label>
          <label>Fecha y hora<input name="fechaHoraReserva" type="datetime-local" required /></label>
          <label class="span-2">Notas y sintomas<textarea name="notasSintomas"></textarea></label>
          <div class="btn-row span-2"><button class="btn" type="submit">Programar</button></div>
        </form>
      </div>
      <div class="panel">
        <h3>Gestion de asistencia</h3>
        <form id="confirmarCitaForm" class="form-grid">
          <label class="span-2">Cita ID<input name="id" type="number" min="1" required /></label>
          <div class="btn-row span-2"><button class="btn secondary" type="submit">Confirmar asistencia</button></div>
        </form>
        <form id="convertirCitaForm" class="form-grid">
          <label class="span-2">Cita ID<input name="id" type="number" min="1" required /></label>
          <div class="btn-row span-2"><button class="btn secondary" type="submit">Convertir en orden</button></div>
        </form>
      </div>
    </section>
  `);

  $("#citaForm").addEventListener("submit", (event) => postForm(event, "/citas", null, null, "Cita programada"));
  $("#confirmarCitaForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const data = getFormData(event.currentTarget);
    await postPath(`/citas/${data.id}/confirmar`, event.currentTarget, "Asistencia confirmada");
  });
  $("#convertirCitaForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const data = getFormData(event.currentTarget);
    await postPath(`/citas/${data.id}/convertir-orden`, event.currentTarget, "Cita convertida en orden");
    await loadOrdenes().catch(() => {});
  });
}

function renderFacturacion() {
  setView(`
    ${header("Facturacion y pagos", "Cierre de orden y registro de pagos.")}
    <section class="layout-grid">
      <div class="panel">
        <h3>Facturar orden</h3>
        <form id="facturarForm" class="form-grid">
          <label class="span-2">Orden servicio ID<input name="ordenServicioId" type="number" min="1" required /></label>
          <div class="btn-row span-2"><button class="btn" type="submit">Facturar y cerrar</button></div>
        </form>
      </div>
      <div class="panel">
        <h3>Registrar pago</h3>
        <form id="pagoForm" class="form-grid">
          <label>Factura ID<input name="facturaId" type="number" min="1" required /></label>
          <label>Medio pago ID<input name="medioPagoId" type="number" min="1" required /></label>
          <label class="span-2">Monto pagado<input name="montoPagado" type="number" min="0.01" step="0.01" required /></label>
          <label class="span-2">Referencia<input name="transaccionReferencia" /></label>
          <div class="btn-row span-2"><button class="btn" type="submit">Registrar pago</button></div>
        </form>
      </div>
    </section>
  `);
  $("#facturarForm").addEventListener("submit", (event) => postForm(event, "/ordenes/facturar", loadOrdenes, renderFacturacion, "Factura generada"));
  $("#pagoForm").addEventListener("submit", (event) => postForm(event, "/pagos/registrar", null, null, "Pago registrado"));
}

function renderGarantias() {
  setView(`
    ${header("Garantias", "Reclamos asociados a ordenes cerradas o con falla.")}
    <section class="panel">
      <form id="garantiaForm" class="form-grid">
        <label class="span-2">Orden original ID<input name="ordenOriginalId" type="number" min="1" required /></label>
        <label class="span-2">Motivo de falla<textarea name="motivoFalla" required></textarea></label>
        <label class="span-2">Resolucion / dictamen<textarea name="resolucionDictamen" required></textarea></label>
        <div class="btn-row span-2"><button class="btn" type="submit">Registrar reclamo</button></div>
      </form>
    </section>
  `);
  $("#garantiaForm").addEventListener("submit", (event) => postForm(event, "/garantias/registrar", loadOrdenes, renderGarantias, "Garantia registrada"));
}

function renderUsuarios() {
  setView(`
    ${header("Usuarios", "Roles, estado de cuenta y creacion de accesos.")}
    <section class="layout-grid">
      <div class="panel">
        <div class="panel-header">
          <h3>Usuarios</h3>
          <button class="btn secondary" id="reloadUsuarios">Actualizar</button>
        </div>
        ${table(state.cache.usuarios, [
          { label: "ID", key: "id" },
          { label: "Nombre", key: "nombre" },
          { label: "Correo", key: "correo" },
          { label: "Rol", key: "rol" },
          { label: "Activo", render: (x) => statusPill(x.activo) },
          { label: "Accion", render: (x) => x.id === state.session.usuario.id ? "" : `<button class="btn danger" data-disable="${x.id}">Desactivar</button>` },
        ])}
      </div>
      <div class="panel">
        <h3>Nuevo usuario</h3>
        <form id="usuarioForm" class="form-grid">
          <label class="span-2">Nombre<input name="nombre" required /></label>
          <label class="span-2">Correo<input name="correo" type="email" required /></label>
          <label>Contrasena<input name="contrasena" type="password" required /></label>
          <label>Rol<select name="rol"><option>Recepcionista</option><option>Mecanico</option><option>Admin</option><option>Cliente</option></select></label>
          <label class="span-2">Telefono<input name="telefono" /></label>
          <div class="btn-row span-2"><button class="btn" type="submit">Crear usuario</button></div>
        </form>
      </div>
    </section>
  `);
  $("#reloadUsuarios").addEventListener("click", refresh(loadUsuarios, renderUsuarios));
  $("#usuarioForm").addEventListener("submit", (event) => postForm(event, "/usuarios/registrar", loadUsuarios, renderUsuarios, "Usuario creado"));
  document.querySelectorAll("[data-disable]").forEach((button) => {
    button.addEventListener("click", async () => {
      try {
        await api(`/usuarios/${button.dataset.disable}/desactivar`, { method: "PATCH" });
        await loadUsuarios();
        toast("Usuario desactivado");
        renderUsuarios();
      } catch (error) {
        toast(error.message, "error");
      }
    });
  });
}

function bindOptional(selector, handler) {
  const node = $(selector);
  if (node) node.addEventListener("submit", handler);
}

function refresh(loader, renderer) {
  return async () => {
    try {
      await loader();
      toast("Datos actualizados");
      renderer();
    } catch (error) {
      toast(error.message, "error");
    }
  };
}

async function postForm(event, path, after, renderer, message) {
  event.preventDefault();
  const form = event.currentTarget;
  const data = getFormData(form);
  try {
    await api(path, { method: "POST", body: JSON.stringify(data) });
    form.reset();
    if (after) await after();
    toast(message);
    if (renderer) renderer();
  } catch (error) {
    toast(error.message, "error");
  }
}

async function postPath(path, form, message) {
  try {
    await api(path, { method: "POST" });
    form.reset();
    toast(message);
  } catch (error) {
    toast(error.message, "error");
  }
}

if (state.session) {
  warmup().finally(render);
} else {
  render();
}
