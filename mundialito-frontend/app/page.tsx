
"use client";

import { useEffect, useState } from "react";

const API_URL = "http://localhost:5241/api/Equipos";
const API_JUGADORES_URL = "http://localhost:5241/api/Jugadores";
const API_PARTIDOS_URL = "http://localhost:5241/api/Partidos";
const API_POSICIONES_URL = "http://localhost:5241/api/posiciones";

type Equipo = {
  id: string;
  nombre: string;
  entrenador: string;
  fechaCreacion: string;
};

type Jugador = {
  id: string;
  nombre: string;
  numeroCamiseta: number;
  posicion: string;
  equipoId: string;
  nombreEquipo?: string;
  fechaCreacion?: string;
};

type RespuestaJugadores = {
  data: Jugador[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
};

type Partido = {
  id: string;
  equipoLocalId: string;
  nombreEquipoLocal: string;
  equipoVisitanteId: string;
  nombreEquipoVisitante: string;
  fechaPartido: string;
  golesLocal: number;
  golesVisitante: number;
  estado: string;
  fechaCreacion?: string;
};

type RespuestaPartidos = {
  data: Partido[];
  pageNumber?: number;
  pageSize?: number;
  totalRecords?: number;
  totalPages?: number;
};

type RespuestaEquipos = {
  data: Equipo[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
};

const menu = [
  { id: "inicio", label: "Inicio", icon: "⌂" },
  { id: "equipos", label: "Equipos", icon: "⚑" },
  { id: "jugadores", label: "Jugadores", icon: "♙" },
  { id: "partidos", label: "Partidos", icon: "⚽" },
  { id: "posiciones", label: "Posiciones", icon: "▤" },
];

const colores = ["#eab308", "#2563eb", "#f97316", "#16a34a", "#9333ea"];

export default function Home() {
  const [active, setActive] = useState("inicio");
  const [equipos, setEquipos] = useState<Equipo[]>([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");
  const [totalEquipos, setTotalEquipos] = useState(0);
  const [filtroEquipo, setFiltroEquipo] = useState("");
  const [paginaEquipos, setPaginaEquipos] = useState(1);
  const [tamanoPaginaEquipos, setTamanoPaginaEquipos] = useState(5);
  const [ordenEquipos, setOrdenEquipos] = useState<"nombre" | "entrenador" | "fechaCreacion">("nombre");
  const [direccionOrdenEquipos, setDireccionOrdenEquipos] = useState<"asc" | "desc">("asc");
  const [modalEquipo, setModalEquipo] = useState(false);
  const [equipoEditando, setEquipoEditando] = useState<Equipo | null>(null);
  const [nombreEquipo, setNombreEquipo] = useState("");
  const [entrenadorEquipo, setEntrenadorEquipo] = useState("");
  const [guardandoEquipo, setGuardandoEquipo] = useState(false);
  const [mensajeEquipo, setMensajeEquipo] = useState("");
  const [jugadores, setJugadores] = useState<Jugador[]>([]);
  const [cargandoJugadores, setCargandoJugadores] = useState(false);
  const [errorJugadores, setErrorJugadores] = useState("");
  const [totalJugadores, setTotalJugadores] = useState(0);
  const [filtroJugador, setFiltroJugador] = useState("");
  const [filtroPosicionJugador, setFiltroPosicionJugador] = useState("Todas");
  const [filtroEquipoJugador, setFiltroEquipoJugador] = useState("Todos");
  const [paginaJugadores, setPaginaJugadores] = useState(1);
  const [tamanoJugadores, setTamanoJugadores] = useState(5);
  const [ordenJugadores, setOrdenJugadores] = useState("nombre");
  const [direccionJugadores, setDireccionJugadores] = useState("asc");
  const [modalJugador, setModalJugador] = useState(false);
  const [jugadorEditando, setJugadorEditando] = useState<Jugador | null>(null);
  const [nombreJugador, setNombreJugador] = useState("");
  const [numeroCamiseta, setNumeroCamiseta] = useState("1");
  const [posicionJugador, setPosicionJugador] = useState("Delantero");
  const [equipoJugador, setEquipoJugador] = useState("");
  const [guardandoJugador, setGuardandoJugador] = useState(false);
  const [mensajeJugador, setMensajeJugador] = useState("");
  const [partidos, setPartidos] = useState<Partido[]>([]);
  const [cargandoPartidos, setCargandoPartidos] = useState(false);
  const [errorPartidos, setErrorPartidos] = useState("");
  const [totalPartidos, setTotalPartidos] = useState(0);
  const [filtroEquipoPartido, setFiltroEquipoPartido] = useState("Todos");
  const [filtroEstadoPartido, setFiltroEstadoPartido] = useState("Todos");
  const [fechaDesde, setFechaDesde] = useState("");
  const [fechaHasta, setFechaHasta] = useState("");
  const [paginaPartidos, setPaginaPartidos] = useState(1);
  const [tamanoPartidos, setTamanoPartidos] = useState(5);
  const [direccionPartidos, setDireccionPartidos] = useState("asc");
  const [filtroPosicionEquipo, setFiltroPosicionEquipo] = useState("");
  const [paginaPosiciones, setPaginaPosiciones] = useState(1);
  const [tamanoPosiciones, setTamanoPosiciones] = useState(5);
  const [modalPartido, setModalPartido] = useState(false);
  const [equipoLocalId, setEquipoLocalId] = useState("");
  const [equipoVisitanteId, setEquipoVisitanteId] = useState("");
  const [fechaPartido, setFechaPartido] = useState("");
  const [guardandoPartido, setGuardandoPartido] = useState(false);
  const [mensajePartido, setMensajePartido] = useState("");
  const [partidoMarcador, setPartidoMarcador] = useState<Partido | null>(null);
  const [golesLocal, setGolesLocal] = useState("0");
  const [golesVisitante, setGolesVisitante] = useState("0");
  const [guardandoResultado, setGuardandoResultado] = useState(false);
  const [posiciones, setPosiciones] = useState<Record<string, unknown>[]>([]);
  const [cargandoPosiciones, setCargandoPosiciones] = useState(false);
  const [errorPosiciones, setErrorPosiciones] = useState("");

  async function cargarEquipos() {
    setCargando(true);
    setError("");

    try {
      // Consultamos el endpoint base porque la API actual puede no admitir
      // parámetros de paginación/ordenamiento en Equipos todavía.
      const respuesta = await fetch(API_URL, { cache: "no-store" });

      if (!respuesta.ok) {
        const detalle = await respuesta.text().catch(() => "");
        throw new Error(
          `La API respondió con HTTP ${respuesta.status}${detalle ? `: ${detalle}` : ""}`
        );
      }

      const resultado: unknown = await respuesta.json();
      const lista = Array.isArray(resultado)
        ? resultado as Equipo[]
        : resultado && typeof resultado === "object" && "data" in resultado && Array.isArray((resultado as { data: unknown }).data)
          ? (resultado as { data: Equipo[] }).data
          : resultado && typeof resultado === "object" && "items" in resultado && Array.isArray((resultado as { items: unknown }).items)
            ? (resultado as { items: Equipo[] }).items
            : null;

      if (!lista) {
        throw new Error("La respuesta de la API no contiene una lista de equipos reconocible.");
      }

      setEquipos(lista);
      setTotalEquipos(
        resultado && typeof resultado === "object" && "totalRecords" in resultado && typeof (resultado as { totalRecords: unknown }).totalRecords === "number"
          ? (resultado as { totalRecords: number }).totalRecords
          : lista.length
      );
    } catch (err) {
      console.error("Error al consultar los equipos:", err);

      setError(
        "No fue posible cargar los equipos. Verifica que la API esté activa y permita solicitudes desde el frontend."
      );
    } finally {
      setCargando(false);
    }
  }

  const equiposFiltrados = equipos
    .filter((equipo) => {
      const termino = filtroEquipo.trim().toLocaleLowerCase("es-SV");
      return !termino || equipo.nombre.toLocaleLowerCase("es-SV").includes(termino) ||
        (equipo.entrenador ?? "").toLocaleLowerCase("es-SV").includes(termino);
    })
    .sort((a, b) => {
      const valorA = String(a[ordenEquipos] ?? "").toLocaleLowerCase("es-SV");
      const valorB = String(b[ordenEquipos] ?? "").toLocaleLowerCase("es-SV");
      const comparacion = valorA.localeCompare(valorB, "es-SV", { numeric: true, sensitivity: "base" });
      return direccionOrdenEquipos === "asc" ? comparacion : -comparacion;
    });
  const totalPaginasEquipos = Math.max(1, Math.ceil(equiposFiltrados.length / tamanoPaginaEquipos));
  const equiposPaginados = equiposFiltrados.slice((paginaEquipos - 1) * tamanoPaginaEquipos, paginaEquipos * tamanoPaginaEquipos);

  async function cargarJugadores() {
    setCargandoJugadores(true);
    setErrorJugadores("");
    try {
      const respuesta = await fetch(API_JUGADORES_URL, { cache: "no-store" });
      if (!respuesta.ok) {
        const detalle = await respuesta.text();
        throw new Error(detalle || `La API respondió con HTTP ${respuesta.status}`);
      }
      const resultado: unknown = await respuesta.json();
      const obj = resultado && typeof resultado === "object" ? resultado as Record<string, unknown> : null;
      const lista = Array.isArray(resultado) ? resultado : Array.isArray(obj?.data) ? obj.data : Array.isArray(obj?.items) ? obj.items : null;
      if (!lista) throw new Error("La respuesta de jugadores no contiene una lista reconocible.");
      setJugadores(lista as Jugador[]);
      setTotalJugadores(typeof obj?.totalRecords === "number" ? obj.totalRecords : lista.length);
      setPaginaJugadores(1);
    } catch (err) {
      console.error("Error al consultar los jugadores:", err);
      setErrorJugadores(err instanceof Error ? err.message : "No fue posible cargar los jugadores.");
    } finally {
      setCargandoJugadores(false);
    }
  }

  async function cargarPartidos() {
    setCargandoPartidos(true);
    setErrorPartidos("");
    try {
      const respuesta = await fetch(API_PARTIDOS_URL, { cache: "no-store" });
      if (!respuesta.ok) {
        const detalle = await respuesta.text();
        throw new Error(detalle || `La API respondió con HTTP ${respuesta.status}`);
      }
      const resultado: unknown = await respuesta.json();
      const obj = resultado && typeof resultado === "object" ? resultado as Record<string, unknown> : null;
      const lista = Array.isArray(resultado) ? resultado : Array.isArray(obj?.data) ? obj.data : Array.isArray(obj?.items) ? obj.items : null;
      if (!lista) throw new Error("La respuesta de partidos no contiene una lista reconocible.");
      setPartidos(lista as Partido[]);
      setTotalPartidos(typeof obj?.totalRecords === "number" ? obj.totalRecords : lista.length);
      setPaginaPartidos(1);
    } catch (err) {
      console.error("Error al consultar partidos:", err);
      setErrorPartidos(err instanceof Error ? err.message : "No fue posible cargar los partidos.");
    } finally {
      setCargandoPartidos(false);
    }
  }

  const jugadoresFiltrados = jugadores.filter((j) => {
    const equipoNombre = j.nombreEquipo || equipos.find((e) => e.id === j.equipoId)?.nombre || "";
    const q = filtroJugador.trim().toLocaleLowerCase("es-SV");
    return (!q || j.nombre.toLocaleLowerCase("es-SV").includes(q) || String(j.numeroCamiseta).includes(q) || equipoNombre.toLocaleLowerCase("es-SV").includes(q)) &&
      (filtroPosicionJugador === "Todas" || j.posicion === filtroPosicionJugador) &&
      (filtroEquipoJugador === "Todos" || j.equipoId === filtroEquipoJugador);
  }).sort((a,b) => {
    const av = String((a as unknown as Record<string, unknown>)[ordenJugadores] ?? "");
    const bv = String((b as unknown as Record<string, unknown>)[ordenJugadores] ?? "");
    const cmp = av.localeCompare(bv, "es-SV", {numeric:true, sensitivity:"base"});
    return direccionJugadores === "asc" ? cmp : -cmp;
  });
  const paginasJugadores = Math.max(1, Math.ceil(jugadoresFiltrados.length / tamanoJugadores));
  const jugadoresPagina = jugadoresFiltrados.slice((paginaJugadores - 1) * tamanoJugadores, paginaJugadores * tamanoJugadores);

  const partidosFiltrados = partidos.filter((p) => {
    const estado = (p.estado || "Pendiente").toLocaleLowerCase("es-SV");
    const finalizado = estado === "finalizado" || estado === "finalizado";
    const fecha = p.fechaPartido ? new Date(p.fechaPartido) : null;
    return (filtroEquipoPartido === "Todos" || p.equipoLocalId === filtroEquipoPartido || p.equipoVisitanteId === filtroEquipoPartido) &&
      (filtroEstadoPartido === "Todos" || (filtroEstadoPartido === "Finalizado" ? finalizado : !finalizado)) &&
      (!fechaDesde || (!!fecha && fecha >= new Date(`${fechaDesde}T00:00:00`))) &&
      (!fechaHasta || (!!fecha && fecha <= new Date(`${fechaHasta}T23:59:59`)));
  }).sort((a,b) => (new Date(a.fechaPartido).getTime() - new Date(b.fechaPartido).getTime()) * (direccionPartidos === "asc" ? 1 : -1));
  const paginasPartidos = Math.max(1, Math.ceil(partidosFiltrados.length / tamanoPartidos));
  const partidosPagina = partidosFiltrados.slice((paginaPartidos - 1) * tamanoPartidos, paginaPartidos * tamanoPartidos);

  async function cargarPosiciones() {
    setCargandoPosiciones(true);
    setErrorPosiciones("");
    try {
      const respuesta = await fetch(API_POSICIONES_URL, { cache: "no-store" });
      if (!respuesta.ok) {
        const detalle = await respuesta.text();
        throw new Error(detalle || `La API respondió con HTTP ${respuesta.status}`);
      }
      const resultado = await respuesta.json();
      const lista = Array.isArray(resultado) ? resultado : Array.isArray(resultado.data) ? resultado.data : Array.isArray(resultado.items) ? resultado.items : [];
      setPosiciones(lista);
      if (!Array.isArray(resultado) && !Array.isArray(resultado.data) && !Array.isArray(resultado.items)) {
        throw new Error("La API respondió, pero no reconocí una lista en su formato. Revisa el Response body de GET /api/posiciones.");
      }
    } catch (err) {
      console.error("Error al consultar posiciones:", err);
      setErrorPosiciones(err instanceof Error ? err.message : "No fue posible cargar las posiciones.");
    } finally {
      setCargandoPosiciones(false);
    }
  }

  const posicionesFiltradas = posiciones.filter((fila) => {
    const nombre = String(fila.nombreEquipo ?? fila.equipo ?? fila.nombre ?? "").toLocaleLowerCase("es-SV");
    return !filtroPosicionEquipo.trim() || nombre.includes(filtroPosicionEquipo.trim().toLocaleLowerCase("es-SV"));
  });
  const paginasPosiciones = Math.max(1, Math.ceil(posicionesFiltradas.length / tamanoPosiciones));
  const posicionesPagina = posicionesFiltradas.slice((paginaPosiciones - 1) * tamanoPosiciones, paginaPosiciones * tamanoPosiciones);

  useEffect(() => {
    void cargarEquipos();
    void cargarJugadores();
    void cargarPartidos();
    void cargarPosiciones();
  }, []);

  function abrirCrearEquipo() {
    setEquipoEditando(null);
    setNombreEquipo("");
    setEntrenadorEquipo("");
    setMensajeEquipo("");
    setModalEquipo(true);
  }

  function abrirEditarEquipo(equipo: Equipo) {
    setEquipoEditando(equipo);
    setNombreEquipo(equipo.nombre);
    setEntrenadorEquipo(equipo.entrenador ?? "");
    setMensajeEquipo("");
    setModalEquipo(true);
  }

  async function guardarEquipo(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const nombre = nombreEquipo.trim();
    const entrenador = entrenadorEquipo.trim();

    if (!nombre || !entrenador) {
      setMensajeEquipo("Ingresa el nombre del equipo y su entrenador.");
      return;
    }

    setGuardandoEquipo(true);
    setMensajeEquipo("");

    try {
      const editando = equipoEditando !== null;
      const url = editando ? `${API_URL}/${equipoEditando.id}` : API_URL;
      const headers: Record<string, string> = {
        "Content-Type": "application/json",
      };

      if (!editando) headers["Idempotency-Key"] = crypto.randomUUID();

      const respuesta = await fetch(url, {
        method: editando ? "PUT" : "POST",
        headers,
        body: JSON.stringify(
          editando
            ? { id: equipoEditando.id, nombre, entrenador }
            : { nombre, entrenador }
        ),
      });

      if (!respuesta.ok) {
        const detalle = await respuesta.text();
        throw new Error(detalle || `La API respondió con HTTP ${respuesta.status}`);
      }

      setModalEquipo(false);
      setMensajeEquipo("");
      await cargarEquipos();
    } catch (err) {
      setMensajeEquipo(
        err instanceof Error ? err.message : "No fue posible guardar el equipo."
      );
    } finally {
      setGuardandoEquipo(false);
    }
  }

  async function eliminarEquipo(equipo: Equipo) {
    const confirmado = window.confirm(
      `¿Seguro que deseas eliminar el equipo "${equipo.nombre}"?`
    );
    if (!confirmado) return;

    setMensajeEquipo("");

    try {
      const respuesta = await fetch(`${API_URL}/${equipo.id}`, {
        method: "DELETE",
      });

      if (!respuesta.ok) {
        const detalle = await respuesta.text();
        throw new Error(detalle || `La API respondió con HTTP ${respuesta.status}`);
      }

      await cargarEquipos();
    } catch (err) {
      setMensajeEquipo(
        err instanceof Error ? err.message : "No fue posible eliminar el equipo."
      );
    }
  }


  function abrirCrearJugador() {
    setJugadorEditando(null);
    setNombreJugador("");
    setNumeroCamiseta("1");
    setPosicionJugador("Delantero");
    setEquipoJugador(equipos[0]?.id ?? "");
    setMensajeJugador("");
    setModalJugador(true);
  }

  function abrirEditarJugador(jugador: Jugador) {
    setJugadorEditando(jugador);
    setNombreJugador(jugador.nombre);
    setNumeroCamiseta(String(jugador.numeroCamiseta));
    setPosicionJugador(jugador.posicion);
    setEquipoJugador(jugador.equipoId);
    setMensajeJugador("");
    setModalJugador(true);
  }

  async function guardarJugador(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const nombre = nombreJugador.trim();
    const posicion = posicionJugador.trim();
    const numero = Number(numeroCamiseta);
    if (!nombre || !posicion || !Number.isInteger(numero) || numero < 0 || !equipoJugador) {
      setMensajeJugador("Completa todos los campos. El número de camiseta debe ser un entero válido y debes seleccionar un equipo.");
      return;
    }
    setGuardandoJugador(true);
    setMensajeJugador("");
    try {
      const editando = jugadorEditando !== null;
      const url = editando ? `${API_JUGADORES_URL}/${jugadorEditando.id}` : API_JUGADORES_URL;
      const headers: Record<string, string> = { "Content-Type": "application/json" };
      if (!editando) headers["Idempotency-Key"] = crypto.randomUUID();
      const cuerpo = editando
        ? { id: jugadorEditando.id, nombre, numeroCamiseta: numero, posicion }
        : { nombre, numeroCamiseta: numero, posicion, equipoId: equipoJugador };
      const respuesta = await fetch(url, {
        method: editando ? "PUT" : "POST",
        headers,
        body: JSON.stringify(cuerpo),
      });
      if (!respuesta.ok) {
        const detalle = await respuesta.text();
        throw new Error(detalle || `La API respondió con HTTP ${respuesta.status}`);
      }
      setModalJugador(false);
      await cargarJugadores();
    } catch (err) {
      setMensajeJugador(err instanceof Error ? err.message : "No fue posible guardar el jugador.");
    } finally {
      setGuardandoJugador(false);
    }
  }

  async function eliminarJugador(jugador: Jugador) {
    if (!window.confirm(`¿Seguro que deseas eliminar al jugador "${jugador.nombre}"?`)) return;
    setMensajeJugador("");
    try {
      const respuesta = await fetch(`${API_JUGADORES_URL}/${jugador.id}`, { method: "DELETE" });
      if (!respuesta.ok) {
        const detalle = await respuesta.text();
        throw new Error(detalle || `La API respondió con HTTP ${respuesta.status}`);
      }
      await cargarJugadores();
    } catch (err) {
      setMensajeJugador(err instanceof Error ? err.message : "No fue posible eliminar el jugador.");
    }
  }

  function abrirCrearPartido() {
    setEquipoLocalId(equipos[0]?.id ?? "");
    setEquipoVisitanteId(equipos[1]?.id ?? "");
    const fecha = new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 16);
    setFechaPartido(fecha);
    setMensajePartido("");
    setModalPartido(true);
  }

  async function guardarPartido(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (!equipoLocalId || !equipoVisitanteId || equipoLocalId === equipoVisitanteId || !fechaPartido) {
      setMensajePartido("Selecciona dos equipos distintos y una fecha válida.");
      return;
    }
    setGuardandoPartido(true);
    setMensajePartido("");
    try {
      const respuesta = await fetch(API_PARTIDOS_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          equipoLocalId,
          equipoVisitanteId,
          fechaPartido: new Date(fechaPartido).toISOString(),
        }),
      });
      if (!respuesta.ok) {
        const detalle = await respuesta.text();
        throw new Error(detalle || `La API respondió con HTTP ${respuesta.status}`);
      }
      setModalPartido(false);
      await cargarPartidos();
    } catch (err) {
      setMensajePartido(err instanceof Error ? err.message : "No fue posible crear el partido.");
    } finally {
      setGuardandoPartido(false);
    }
  }

  function abrirMarcador(partido: Partido) {
    setPartidoMarcador(partido);
    setGolesLocal(String(partido.golesLocal ?? 0));
    setGolesVisitante(String(partido.golesVisitante ?? 0));
    setMensajePartido("");
  }

  async function guardarResultado(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (!partidoMarcador) return;
    const local = Number(golesLocal);
    const visitante = Number(golesVisitante);
    if (!Number.isInteger(local) || local < 0 || !Number.isInteger(visitante) || visitante < 0) {
      setMensajePartido("Los goles deben ser números enteros iguales o mayores que cero.");
      return;
    }
    setGuardandoResultado(true);
    setMensajePartido("");
    try {
      const respuesta = await fetch(`${API_PARTIDOS_URL}/${partidoMarcador.id}/resultado`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ partidoId: partidoMarcador.id, golesLocal: local, golesVisitante: visitante }),
      });
      if (!respuesta.ok) {
        const detalle = await respuesta.text();
        throw new Error(detalle || `La API respondió con HTTP ${respuesta.status}`);
      }
      setPartidoMarcador(null);
      await Promise.all([cargarPartidos(), cargarPosiciones()]);
    } catch (err) {
      setMensajePartido(err instanceof Error ? err.message : "No fue posible guardar el resultado.");
    } finally {
      setGuardandoResultado(false);
    }
  }

  const titulos: Record<string, string> = {
    inicio: "Panel general",
    equipos: "Gestión de equipos",
    jugadores: "Gestión de jugadores",
    partidos: "Calendario de partidos",
    posiciones: "Tabla de posiciones",
  };

  return (
    <div className="min-h-screen bg-slate-100 text-slate-800 md:flex">
      <aside className="flex w-full flex-col bg-slate-950 p-5 text-white md:min-h-screen md:w-64">
        <div className="mb-9 flex items-center gap-3">
          <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-emerald-500 text-2xl">
            ⚽
          </div>
          <div>
            <h1 className="text-xl font-bold">Mundialito</h1>
            <p className="text-xs text-slate-400">
              Administración deportiva
            </p>
          </div>
        </div>

        <p className="mb-3 text-xs font-semibold uppercase tracking-widest text-slate-500">
          Menú principal
        </p>

        <nav className="flex flex-wrap gap-2 md:flex-col">
          {menu.map((item) => (
            <button
              key={item.id}
              onClick={() => setActive(item.id)}
              className={`flex items-center gap-3 rounded-xl px-4 py-3 text-left transition ${
                active === item.id
                  ? "bg-emerald-500 font-semibold text-white"
                  : "text-slate-300 hover:bg-slate-800"
              }`}
            >
              <span className="text-xl">{item.icon}</span>
              {item.label}
            </button>
          ))}
        </nav>

        <div className="mt-8 rounded-xl border border-slate-800 p-4 md:mt-auto">
          <p className="text-sm font-semibold">Mundialito API</p>
          <p className="mt-1 text-xs text-slate-400">
            Estado de la conexión
          </p>
          <span
            className={`mt-3 inline-flex items-center gap-2 text-xs ${
              error ? "text-red-400" : cargando ? "text-amber-400" : "text-emerald-400"
            }`}
          >
            <span
              className={`h-2 w-2 rounded-full ${
                error
                  ? "bg-red-400"
                  : cargando
                    ? "bg-amber-400"
                    : "bg-emerald-400"
              }`}
            />
            {error ? "Error de conexión" : cargando ? "Conectando..." : "API conectada"}
          </span>
        </div>
      </aside>

      <main className="min-w-0 flex-1 p-5 md:p-8">
        <header className="mb-8 flex flex-wrap items-center justify-between gap-4">
          <div>
            <p className="text-sm text-slate-500">
              Mundialito / Administración
            </p>
            <h2 className="mt-1 text-2xl font-bold md:text-3xl">
              {titulos[active]}
            </h2>
          </div>

          <div className="rounded-xl border border-slate-200 bg-white px-4 py-3 text-sm shadow-sm">
            🏆 Torneo de fútbol
          </div>
        </header>

        {active === "inicio" && (
          <>
            <section className="mb-8 rounded-2xl bg-gradient-to-r from-emerald-700 to-emerald-500 p-6 text-white shadow-lg md:p-8">
              <p className="text-sm font-medium text-emerald-100">
                BIENVENIDO AL SISTEMA
              </p>
              <h3 className="mt-2 text-3xl font-bold md:text-4xl">
                Gestiona tu Mundialito
              </h3>
              <p className="mt-3 max-w-xl text-sm leading-6 text-emerald-50 md:text-base">
                Administra equipos, jugadores y partidos desde un solo lugar.
                Consulta las posiciones y organiza tu torneo.
              </p>
              <button
                onClick={() => setActive("equipos")}
                className="mt-6 rounded-xl bg-white px-5 py-3 font-semibold text-emerald-800 hover:bg-emerald-50"
              >
                Ver equipos →
              </button>
            </section>

            <section className="mb-8 grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
              {[
                {
                  label: "Equipos registrados",
                  value: cargando ? "..." : String(totalEquipos),
                  icon: "⚑",
                },
                { label: "Jugadores", value: cargandoJugadores ? "..." : String(totalJugadores), icon: "♙" },
{
  label: "Partidos",
  value: cargandoPartidos ? "..." : String(totalPartidos),
  icon: "⚽",
},                { label: "Competición", value: "Mundialito", icon: "🏆" },
              ].map((stat) => (
                <article
                  key={stat.label}
                  className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
                >
                  <div className="flex items-center justify-between gap-2">
                    <p className="text-sm text-slate-500">{stat.label}</p>
                    <span className="text-2xl">{stat.icon}</span>
                  </div>
                  <p className="mt-4 text-xl font-bold">{stat.value}</p>
                </article>
              ))}
            </section>

            <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm md:p-6">
              <div className="mb-5 flex flex-wrap items-center justify-between gap-3">
                <div>
                  <h3 className="text-lg font-bold">Equipos del torneo</h3>
                  <p className="mt-1 text-sm text-slate-500">
                    Datos obtenidos desde la API de .NET
                  </p>
                </div>
                <button
                  onClick={() => setActive("equipos")}
                  className="text-sm font-semibold text-emerald-700 hover:underline"
                >
                  Ver todos →
                </button>
              </div>

              {cargando ? (
                <p className="py-8 text-center text-slate-500">
                  Cargando equipos...
                </p>
              ) : error ? (
                <div className="rounded-xl bg-red-50 p-4 text-sm text-red-700">
                  {error}
                  <button
                    onClick={() => void cargarEquipos()}
                    className="ml-2 font-bold underline"
                  >
                    Reintentar
                  </button>
                </div>
              ) : equipos.length === 0 ? (
                <p className="py-8 text-center text-slate-500">
                  No hay equipos registrados.
                </p>
              ) : (
                <div className="space-y-3">
                  {equipos.slice(0, 5).map((equipo, index) => (
                    <div
                      key={equipo.id}
                      className="flex flex-wrap items-center justify-between gap-3 rounded-xl border border-slate-100 p-4"
                    >
                      <div className="flex items-center gap-3">
                        <div
                          className="flex h-11 w-11 items-center justify-center rounded-xl text-lg font-bold text-white"
                          style={{
                            backgroundColor: colores[index % colores.length],
                          }}
                        >
                          {equipo.nombre.charAt(0).toUpperCase()}
                        </div>
                        <div>
                          <p className="font-semibold">{equipo.nombre}</p>
                          <p className="text-sm text-slate-500">
                            Entrenador: {equipo.entrenador || "No especificado"}
                          </p>
                        </div>
                      </div>
                      <span className="text-xs text-emerald-700">
                        Registrado en API
                      </span>
                    </div>
                  ))}
                </div>
              )}
            </section>
          </>
        )}

        {active === "equipos" && (
          <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm md:p-6">
            <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
              <div>
                <h3 className="text-lg font-bold">Equipos registrados</h3>
                <p className="mt-1 text-sm text-slate-500">
                  Total informado por la API: {totalEquipos}
                </p>
              </div>
              <div className="flex flex-wrap gap-2">
                <button
                  onClick={abrirCrearEquipo}
                  className="rounded-xl bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700"
                >
                  + Nuevo equipo
                </button>
                <button
                  onClick={() => void cargarEquipos()}
                  disabled={cargando}
                  className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold hover:bg-slate-50 disabled:opacity-50"
                >
                  {cargando ? "Cargando..." : "↻ Actualizar"}
                </button>
              </div>
            </div>

            <div className="mb-5 grid gap-3 rounded-xl bg-slate-50 p-4 md:grid-cols-3">
              <div>
                <label htmlFor="filtroEquipo" className="mb-1 block text-sm font-medium">Buscar equipo o entrenador</label>
                <input id="filtroEquipo" type="search" value={filtroEquipo}
                  onChange={(e) => { setFiltroEquipo(e.target.value); setPaginaEquipos(1); }}
                  placeholder="Ej. Águilas FC" className="w-full rounded-xl border border-slate-300 bg-white px-3 py-2 outline-none focus:border-emerald-600" />
              </div>
              <div>
                <label htmlFor="ordenEquipos" className="mb-1 block text-sm font-medium">Ordenar por</label>
                <select id="ordenEquipos" value={ordenEquipos} onChange={(e) => { setOrdenEquipos(e.target.value as "nombre" | "entrenador" | "fechaCreacion"); setPaginaEquipos(1); }} className="w-full rounded-xl border border-slate-300 bg-white px-3 py-2">
                  <option value="nombre">Nombre del equipo</option><option value="entrenador">Entrenador</option><option value="fechaCreacion">Fecha de creación</option>
                </select>
              </div>
              <div>
                <label htmlFor="direccionOrdenEquipos" className="mb-1 block text-sm font-medium">Dirección</label>
                <select id="direccionOrdenEquipos" value={direccionOrdenEquipos} onChange={(e) => { setDireccionOrdenEquipos(e.target.value as "asc" | "desc"); setPaginaEquipos(1); }} className="w-full rounded-xl border border-slate-300 bg-white px-3 py-2">
                  <option value="asc">Ascendente</option><option value="desc">Descendente</option>
                </select>
              </div>
              <div>
                <label htmlFor="tamanoPaginaEquipos" className="mb-1 block text-sm font-medium">Registros por página</label>
                <select id="tamanoPaginaEquipos" value={tamanoPaginaEquipos} onChange={(e) => { setTamanoPaginaEquipos(Number(e.target.value)); setPaginaEquipos(1); }} className="w-full rounded-xl border border-slate-300 bg-white px-3 py-2">
                  <option value={5}>5</option><option value={10}>10</option><option value={20}>20</option>
                </select>
              </div>
              <p className="flex items-end text-sm text-slate-500 md:col-span-2">Mostrando {equiposPaginados.length} de {equiposFiltrados.length} equipos filtrados.</p>
            </div>

            {cargando ? (
              <p className="py-8 text-center text-slate-500">
                Consultando la API...
              </p>
            ) : error ? (
              <div className="rounded-xl bg-red-50 p-4 text-sm text-red-700">
                {error}
                <button
                  onClick={() => void cargarEquipos()}
                  className="ml-2 font-bold underline"
                >
                  Reintentar
                </button>
              </div>
            ) : (
              <>
              <div className="overflow-x-auto">
                <table className="w-full text-left text-sm">
                  <thead>
                    <tr className="border-b text-slate-500">
                      <th className="px-3 py-3">Equipo</th>
                      <th className="px-3 py-3">Entrenador</th>
                      <th className="px-3 py-3">Fecha de creación</th>
                      <th className="px-3 py-3 text-right">Acciones</th>
                    </tr>
                  </thead>
                  <tbody>
                    {equiposPaginados.map((equipo, index) => (
                      <tr
                        key={equipo.id}
                        className="border-b border-slate-100 last:border-0"
                      >
                        <td className="px-3 py-4 font-semibold">
                          <span
                            className="mr-3 inline-flex h-8 w-8 items-center justify-center rounded-lg text-white"
                            style={{
                              backgroundColor: colores[index % colores.length],
                            }}
                          >
                            {equipo.nombre.charAt(0).toUpperCase()}
                          </span>
                          {equipo.nombre}
                        </td>
                        <td className="px-3 py-4">
                          {equipo.entrenador || "No especificado"}
                        </td>
                        <td className="px-3 py-4 text-slate-500">
                          {equipo.fechaCreacion
                            ? new Date(equipo.fechaCreacion).toLocaleDateString(
                                "es-SV"
                              )
                            : "—"}
                        </td>
                        <td className="px-3 py-4">
                          <div className="flex justify-end gap-2">
                            <button
                              onClick={() => abrirEditarEquipo(equipo)}
                              className="rounded-lg border border-slate-200 px-3 py-1.5 font-medium hover:bg-slate-50"
                            >
                              Editar
                            </button>
                            <button
                              onClick={() => void eliminarEquipo(equipo)}
                              className="rounded-lg border border-red-200 px-3 py-1.5 font-medium text-red-600 hover:bg-red-50"
                            >
                              Eliminar
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>

                {equiposPaginados.length === 0 && (
                  <p className="py-8 text-center text-slate-500">
                    No hay equipos que coincidan con la búsqueda.
                  </p>
                )}
              </div>
              <div className="mt-5 flex flex-wrap items-center justify-between gap-3 border-t border-slate-100 pt-4">
                <p className="text-sm text-slate-500">Página {Math.min(paginaEquipos, totalPaginasEquipos)} de {totalPaginasEquipos}</p>
                <div className="flex gap-2">
                  <button type="button" onClick={() => setPaginaEquipos((pagina) => Math.max(1, pagina - 1))} disabled={paginaEquipos <= 1} className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40">← Anterior</button>
                  <button type="button" onClick={() => setPaginaEquipos((pagina) => Math.min(totalPaginasEquipos, pagina + 1))} disabled={paginaEquipos >= totalPaginasEquipos} className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40">Siguiente →</button>
                </div>
              </div>
              </>
            )}
          </section>
        )}

        {mensajeEquipo && !modalEquipo && (
          <div role="alert" className="mb-4 rounded-xl bg-red-50 p-4 text-sm text-red-700">
            {mensajeEquipo}
            <button onClick={() => setMensajeEquipo("")} className="ml-3 font-bold underline">
              Cerrar
            </button>
          </div>
        )}

        {modalEquipo && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4">
            <section
              role="dialog"
              aria-modal="true"
              aria-labelledby="titulo-modal-equipo"
              className="w-full max-w-md rounded-2xl bg-white p-6 shadow-xl"
            >
              <div className="mb-5 flex items-center justify-between">
                <h3 id="titulo-modal-equipo" className="text-xl font-bold">
                  {equipoEditando ? "Editar equipo" : "Nuevo equipo"}
                </h3>
                <button
                  type="button"
                  onClick={() => setModalEquipo(false)}
                  disabled={guardandoEquipo}
                  className="rounded-lg px-3 py-1 text-slate-500 hover:bg-slate-100"
                  aria-label="Cerrar formulario"
                >
                  ✕
                </button>
              </div>

              <form onSubmit={guardarEquipo} className="space-y-4">
                <div>
                  <label htmlFor="nombreEquipo" className="mb-1 block text-sm font-medium">
                    Nombre del equipo
                  </label>
                  <input
                    id="nombreEquipo"
                    value={nombreEquipo}
                    onChange={(e) => setNombreEquipo(e.target.value)}
                    maxLength={100}
                    required
                    className="w-full rounded-xl border border-slate-300 px-3 py-2 outline-none focus:border-emerald-600"
                    placeholder="Ej. Águilas FC"
                  />
                </div>

                <div>
                  <label htmlFor="entrenadorEquipo" className="mb-1 block text-sm font-medium">
                    Entrenador
                  </label>
                  <input
                    id="entrenadorEquipo"
                    value={entrenadorEquipo}
                    onChange={(e) => setEntrenadorEquipo(e.target.value)}
                    maxLength={100}
                    required
                    className="w-full rounded-xl border border-slate-300 px-3 py-2 outline-none focus:border-emerald-600"
                    placeholder="Nombre del entrenador"
                  />
                </div>

                {mensajeEquipo && (
                  <p role="alert" className="rounded-lg bg-red-50 p-3 text-sm text-red-700">
                    {mensajeEquipo}
                  </p>
                )}

                <div className="flex justify-end gap-3 pt-2">
                  <button
                    type="button"
                    onClick={() => setModalEquipo(false)}
                    disabled={guardandoEquipo}
                    className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold hover:bg-slate-50"
                  >
                    Cancelar
                  </button>
                  <button
                    type="submit"
                    disabled={guardandoEquipo}
                    className="rounded-xl bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
                  >
                    {guardandoEquipo
                      ? "Guardando..."
                      : equipoEditando
                        ? "Guardar cambios"
                        : "Crear equipo"}
                  </button>
                </div>
              </form>
            </section>
          </div>
        )}

        {active === "jugadores" && (
          <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm md:p-6">
            <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
              <div>
                <h3 className="text-lg font-bold">Jugadores registrados</h3>
                <p className="mt-1 text-sm text-slate-500">Total informado por la API: {totalJugadores}</p>
              </div>
              <div className="flex flex-wrap gap-2">
                <button onClick={abrirCrearJugador} disabled={equipos.length === 0} className="rounded-xl bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:cursor-not-allowed disabled:opacity-50">+ Nuevo jugador</button>
                <button onClick={() => void cargarJugadores()} disabled={cargandoJugadores} className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold hover:bg-slate-50 disabled:opacity-50">{cargandoJugadores ? "Cargando..." : "↻ Actualizar"}</button>
              </div>
            </div>
            {equipos.length === 0 && <p className="mb-4 rounded-xl bg-amber-50 p-3 text-sm text-amber-800">Registra al menos un equipo antes de crear jugadores.</p>}
            <div className="mb-4 grid grid-cols-1 gap-3 md:grid-cols-2 xl:grid-cols-5">
              <input aria-label="Buscar jugador" value={filtroJugador} onChange={(e) => { setFiltroJugador(e.target.value); setPaginaJugadores(1); }} placeholder="Buscar nombre, camiseta o equipo" className="rounded-xl border border-slate-300 px-3 py-2 text-sm" />
              <select aria-label="Filtrar por posición" value={filtroPosicionJugador} onChange={(e) => { setFiltroPosicionJugador(e.target.value); setPaginaJugadores(1); }} className="rounded-xl border border-slate-300 px-3 py-2 text-sm">{["Todas", "Portero", "Defensa", "Centrocampista", "Delantero"].map((v) => <option key={v} value={v}>{v}</option>)}</select>
              <select aria-label="Filtrar por equipo" value={filtroEquipoJugador} onChange={(e) => { setFiltroEquipoJugador(e.target.value); setPaginaJugadores(1); }} className="rounded-xl border border-slate-300 px-3 py-2 text-sm"><option value="Todos">Todos los equipos</option>{equipos.map((e) => <option key={e.id} value={e.id}>{e.nombre}</option>)}</select>
              <select aria-label="Ordenar jugadores" value={ordenJugadores} onChange={(e) => setOrdenJugadores(e.target.value)} className="rounded-xl border border-slate-300 px-3 py-2 text-sm"><option value="nombre">Ordenar: nombre</option><option value="numeroCamiseta">Ordenar: camiseta</option><option value="posicion">Ordenar: posición</option></select>
              <select aria-label="Dirección del orden" value={direccionJugadores} onChange={(e) => setDireccionJugadores(e.target.value)} className="rounded-xl border border-slate-300 px-3 py-2 text-sm"><option value="asc">Ascendente</option><option value="desc">Descendente</option></select>
            </div>
            {cargandoJugadores ? <p className="py-8 text-center text-slate-500">Consultando la API...</p> : errorJugadores ? <div className="rounded-xl bg-red-50 p-4 text-sm text-red-700">{errorJugadores}<button onClick={() => void cargarJugadores()} className="ml-2 font-bold underline">Reintentar</button></div> : (
              <div className="overflow-x-auto">
                <table className="w-full text-left text-sm">
                  <thead><tr className="border-b text-slate-500"><th className="px-3 py-3">Jugador</th><th className="px-3 py-3">Camiseta</th><th className="px-3 py-3">Posición</th><th className="px-3 py-3">Equipo</th><th className="px-3 py-3 text-right">Acciones</th></tr></thead>
                  <tbody>{jugadoresPagina.map((jugador, index) => <tr key={jugador.id} className="border-b border-slate-100 last:border-0">
                    <td className="px-3 py-4 font-semibold"><span className="mr-3 inline-flex h-8 w-8 items-center justify-center rounded-lg text-white" style={{ backgroundColor: colores[index % colores.length] }}>{jugador.nombre.charAt(0).toUpperCase()}</span>{jugador.nombre}</td>
                    <td className="px-3 py-4"><span className="rounded-lg bg-slate-100 px-3 py-1 font-bold">#{jugador.numeroCamiseta}</span></td>
                    <td className="px-3 py-4">{jugador.posicion}</td><td className="px-3 py-4">{jugador.nombreEquipo || equipos.find((e) => e.id === jugador.equipoId)?.nombre || "—"}</td>
                    <td className="px-3 py-4"><div className="flex justify-end gap-2"><button onClick={() => abrirEditarJugador(jugador)} className="rounded-lg border border-slate-200 px-3 py-1.5 font-medium hover:bg-slate-50">Editar</button><button onClick={() => void eliminarJugador(jugador)} className="rounded-lg border border-red-200 px-3 py-1.5 font-medium text-red-600 hover:bg-red-50">Eliminar</button></div></td>
                  </tr>)}</tbody>
                </table>
                {jugadoresFiltrados.length === 0 && <p className="py-8 text-center text-slate-500">No hay jugadores que coincidan con los filtros.</p>}
              </div>
            )}
            {!cargandoJugadores && !errorJugadores && <div className="mt-4 flex flex-wrap items-center justify-between gap-3 text-sm"><span className="text-slate-500">{jugadoresFiltrados.length} jugador(es)</span><div className="flex items-center gap-2"><label htmlFor="tamanoJugadores">Mostrar</label><select id="tamanoJugadores" value={tamanoJugadores} onChange={(e) => { setTamanoJugadores(Number(e.target.value)); setPaginaJugadores(1); }} className="rounded-lg border border-slate-300 px-2 py-1"><option value={5}>5</option><option value={10}>10</option><option value={20}>20</option></select><button disabled={paginaJugadores <= 1} onClick={() => setPaginaJugadores(paginaJugadores - 1)} className="rounded-lg border px-3 py-1 disabled:opacity-40">Anterior</button><span>Página {Math.min(paginaJugadores, paginasJugadores)} de {paginasJugadores}</span><button disabled={paginaJugadores >= paginasJugadores} onClick={() => setPaginaJugadores(paginaJugadores + 1)} className="rounded-lg border px-3 py-1 disabled:opacity-40">Siguiente</button></div></div>}
          </section>
        )}

        {mensajeJugador && !modalJugador && <div role="alert" className="mb-4 rounded-xl bg-red-50 p-4 text-sm text-red-700">{mensajeJugador}<button onClick={() => setMensajeJugador("")} className="ml-3 font-bold underline">Cerrar</button></div>}

        {modalJugador && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4">
            <section role="dialog" aria-modal="true" aria-labelledby="titulo-modal-jugador" className="w-full max-w-md rounded-2xl bg-white p-6 shadow-xl">
              <div className="mb-5 flex items-center justify-between"><h3 id="titulo-modal-jugador" className="text-xl font-bold">{jugadorEditando ? "Editar jugador" : "Nuevo jugador"}</h3><button type="button" onClick={() => setModalJugador(false)} disabled={guardandoJugador} className="rounded-lg px-3 py-1 text-slate-500 hover:bg-slate-100" aria-label="Cerrar formulario">✕</button></div>
              <form onSubmit={guardarJugador} className="space-y-4">
                <div><label htmlFor="nombreJugador" className="mb-1 block text-sm font-medium">Nombre del jugador</label><input id="nombreJugador" value={nombreJugador} onChange={(e) => setNombreJugador(e.target.value)} maxLength={100} required className="w-full rounded-xl border border-slate-300 px-3 py-2 outline-none focus:border-emerald-600" placeholder="Nombre completo" /></div>
                <div><label htmlFor="numeroCamiseta" className="mb-1 block text-sm font-medium">Número de camiseta</label><input id="numeroCamiseta" type="number" min="0" max="999" step="1" value={numeroCamiseta} onChange={(e) => setNumeroCamiseta(e.target.value)} required className="w-full rounded-xl border border-slate-300 px-3 py-2 outline-none focus:border-emerald-600" /></div>
                <div><label htmlFor="posicionJugador" className="mb-1 block text-sm font-medium">Posición</label><select id="posicionJugador" value={posicionJugador} onChange={(e) => setPosicionJugador(e.target.value)} required className="w-full rounded-xl border border-slate-300 px-3 py-2 outline-none focus:border-emerald-600">{["Portero", "Defensa", "Centrocampista", "Delantero"].map((pos) => <option key={pos} value={pos}>{pos}</option>)}{posicionJugador && !["Portero", "Defensa", "Centrocampista", "Delantero"].includes(posicionJugador) && <option value={posicionJugador}>{posicionJugador}</option>}</select></div>
                {!jugadorEditando && <div><label htmlFor="equipoJugador" className="mb-1 block text-sm font-medium">Equipo</label><select id="equipoJugador" value={equipoJugador} onChange={(e) => setEquipoJugador(e.target.value)} required className="w-full rounded-xl border border-slate-300 px-3 py-2 outline-none focus:border-emerald-600"><option value="">Selecciona un equipo</option>{equipos.map((equipo) => <option key={equipo.id} value={equipo.id}>{equipo.nombre}</option>)}</select><p className="mt-1 text-xs text-slate-500">El endpoint de actualización no recibe equipoId, por eso no se cambia al editar.</p></div>}
                {mensajeJugador && <p role="alert" className="rounded-lg bg-red-50 p-3 text-sm text-red-700">{mensajeJugador}</p>}
                <div className="flex justify-end gap-3 pt-2"><button type="button" onClick={() => setModalJugador(false)} disabled={guardandoJugador} className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold hover:bg-slate-50">Cancelar</button><button type="submit" disabled={guardandoJugador} className="rounded-xl bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50">{guardandoJugador ? "Guardando..." : jugadorEditando ? "Guardar cambios" : "Crear jugador"}</button></div>
              </form>
            </section>
          </div>
        )}

        {active === "partidos" && (
          <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm md:p-6">
            <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
              <div><h3 className="text-lg font-bold">Calendario de partidos</h3><p className="mt-1 text-sm text-slate-500">Total informado por la API: {totalPartidos}</p></div>
              <div className="flex flex-wrap gap-2"><button onClick={abrirCrearPartido} disabled={equipos.length < 2} className="rounded-xl bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50">+ Programar partido</button><button onClick={() => void cargarPartidos()} disabled={cargandoPartidos} className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold hover:bg-slate-50">{cargandoPartidos ? "Cargando..." : "↻ Actualizar"}</button></div>
            </div>
            {equipos.length < 2 && <p className="mb-4 rounded-xl bg-amber-50 p-3 text-sm text-amber-800">Necesitas al menos dos equipos registrados para programar un partido.</p>}
            {mensajePartido && !modalPartido && !partidoMarcador && <p role="alert" className="mb-4 rounded-xl bg-red-50 p-3 text-sm text-red-700">{mensajePartido}</p>}
            <div className="mb-4 grid grid-cols-1 gap-3 md:grid-cols-2 xl:grid-cols-5">
              <select aria-label="Filtrar partidos por equipo" value={filtroEquipoPartido} onChange={(e) => { setFiltroEquipoPartido(e.target.value); setPaginaPartidos(1); }} className="rounded-xl border border-slate-300 px-3 py-2 text-sm"><option value="Todos">Todos los equipos</option>{equipos.map((e) => <option key={e.id} value={e.id}>{e.nombre}</option>)}</select>
              <select aria-label="Filtrar partidos por estado" value={filtroEstadoPartido} onChange={(e) => { setFiltroEstadoPartido(e.target.value); setPaginaPartidos(1); }} className="rounded-xl border border-slate-300 px-3 py-2 text-sm"><option value="Todos">Todos los estados</option><option value="Pendiente">Pendientes</option><option value="Finalizado">Finalizados</option></select>
              <input aria-label="Fecha desde" type="date" value={fechaDesde} onChange={(e) => { setFechaDesde(e.target.value); setPaginaPartidos(1); }} className="rounded-xl border border-slate-300 px-3 py-2 text-sm" />
              <input aria-label="Fecha hasta" type="date" value={fechaHasta} onChange={(e) => { setFechaHasta(e.target.value); setPaginaPartidos(1); }} className="rounded-xl border border-slate-300 px-3 py-2 text-sm" />
              <select aria-label="Ordenar partidos por fecha" value={direccionPartidos} onChange={(e) => setDireccionPartidos(e.target.value)} className="rounded-xl border border-slate-300 px-3 py-2 text-sm"><option value="asc">Fecha: más antiguos</option><option value="desc">Fecha: más recientes</option></select>
            </div>
            {cargandoPartidos ? <p className="py-8 text-center text-slate-500">Consultando partidos...</p> : errorPartidos ? <div className="rounded-xl bg-red-50 p-4 text-sm text-red-700">{errorPartidos}<button onClick={() => void cargarPartidos()} className="ml-2 font-bold underline">Reintentar</button></div> : (
              <div className="overflow-x-auto"><table className="w-full text-left text-sm"><thead><tr className="border-b text-slate-500"><th className="px-3 py-3">Encuentro</th><th className="px-3 py-3">Fecha</th><th className="px-3 py-3">Marcador</th><th className="px-3 py-3">Estado</th><th className="px-3 py-3 text-right">Acciones</th></tr></thead><tbody>{partidosPagina.map((partido) => <tr key={partido.id} className="border-b border-slate-100 last:border-0"><td className="px-3 py-4 font-semibold">{partido.nombreEquipoLocal || equipos.find((e) => e.id === partido.equipoLocalId)?.nombre || "Equipo local"}<span className="mx-2 text-slate-400">vs</span>{partido.nombreEquipoVisitante || equipos.find((e) => e.id === partido.equipoVisitanteId)?.nombre || "Equipo visitante"}</td><td className="px-3 py-4">{partido.fechaPartido ? new Date(partido.fechaPartido).toLocaleString("es-SV") : "—"}</td><td className="px-3 py-4"><span className="rounded-lg bg-slate-100 px-3 py-1 font-bold">{partido.golesLocal ?? 0} - {partido.golesVisitante ?? 0}</span></td><td className="px-3 py-4"><span className="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold">{partido.estado || "Programado"}</span></td><td className="px-3 py-4 text-right"><button onClick={() => abrirMarcador(partido)} className="rounded-lg border border-emerald-200 px-3 py-1.5 font-medium text-emerald-700 hover:bg-emerald-50">Registrar resultado</button></td></tr>)}</tbody></table>{partidosFiltrados.length === 0 && <p className="py-8 text-center text-slate-500">No hay partidos que coincidan con los filtros.</p>}</div>
            )}
            {!cargandoPartidos && !errorPartidos && <div className="mt-4 flex flex-wrap items-center justify-between gap-3 text-sm"><span className="text-slate-500">{partidosFiltrados.length} partido(s)</span><div className="flex items-center gap-2"><label htmlFor="tamanoPartidos">Mostrar</label><select id="tamanoPartidos" value={tamanoPartidos} onChange={(e) => { setTamanoPartidos(Number(e.target.value)); setPaginaPartidos(1); }} className="rounded-lg border border-slate-300 px-2 py-1"><option value={5}>5</option><option value={10}>10</option><option value={20}>20</option></select><button disabled={paginaPartidos <= 1} onClick={() => setPaginaPartidos(paginaPartidos - 1)} className="rounded-lg border px-3 py-1 disabled:opacity-40">Anterior</button><span>Página {Math.min(paginaPartidos, paginasPartidos)} de {paginasPartidos}</span><button disabled={paginaPartidos >= paginasPartidos} onClick={() => setPaginaPartidos(paginaPartidos + 1)} className="rounded-lg border px-3 py-1 disabled:opacity-40">Siguiente</button></div></div>}
          </section>
        )}

        {active === "posiciones" && (
          <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm md:p-6"><div className="mb-6 flex flex-wrap items-center justify-between gap-3"><div><h3 className="text-lg font-bold">Tabla de posiciones</h3><p className="mt-1 text-sm text-slate-500">Datos consultados desde la API.</p></div><button onClick={() => void cargarPosiciones()} disabled={cargandoPosiciones} className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold hover:bg-slate-50">{cargandoPosiciones ? "Cargando..." : "↻ Actualizar"}</button></div><div className="mb-4 flex flex-wrap items-center gap-3"><input aria-label="Buscar equipo en posiciones" placeholder="Buscar equipo..." value={filtroPosicionEquipo} onChange={(e) => { setFiltroPosicionEquipo(e.target.value); setPaginaPosiciones(1); }} className="min-w-56 rounded-xl border border-slate-300 px-3 py-2 text-sm" /><label className="flex items-center gap-2 text-sm">Mostrar <select value={tamanoPosiciones} onChange={(e) => { setTamanoPosiciones(Number(e.target.value)); setPaginaPosiciones(1); }} className="rounded-lg border border-slate-300 px-2 py-1"><option value={5}>5</option><option value={10}>10</option><option value={20}>20</option></select></label></div>{cargandoPosiciones ? <p className="py-8 text-center text-slate-500">Consultando posiciones...</p> : errorPosiciones ? <div className="rounded-xl bg-red-50 p-4 text-sm text-red-700">{errorPosiciones}<button onClick={() => void cargarPosiciones()} className="ml-2 font-bold underline">Reintentar</button></div> : posiciones.length === 0 ? <p className="py-8 text-center text-slate-500">La API no devolvió posiciones para mostrar.</p> : <div className="overflow-x-auto"><table className="w-full text-left text-sm"><thead><tr className="border-b text-slate-500">{Object.keys(posiciones[0]).map((key) => <th key={key} className="px-3 py-3">{key.replace(/([A-Z])/g, " $1").replace(/^./, (c) => c.toUpperCase())}</th>)}</tr></thead><tbody>{posicionesPagina.map((fila, index) => <tr key={String(fila.equipoId ?? fila.id ?? index)} className="border-b border-slate-100 last:border-0">{Object.keys(posiciones[0]).map((key) => <td key={key} className="px-3 py-4">{fila[key] === null || fila[key] === undefined ? "—" : typeof fila[key] === "object" ? JSON.stringify(fila[key]) : String(fila[key])}</td>)}</tr>)}</tbody></table><div className="mt-4 flex items-center justify-between text-sm"><span className="text-slate-500">{posicionesFiltradas.length} equipo(s)</span><div className="flex items-center gap-2"><button disabled={paginaPosiciones <= 1} onClick={() => setPaginaPosiciones(paginaPosiciones - 1)} className="rounded-lg border px-3 py-1 disabled:opacity-40">Anterior</button><span>Página {Math.min(paginaPosiciones, paginasPosiciones)} de {paginasPosiciones}</span><button disabled={paginaPosiciones >= paginasPosiciones} onClick={() => setPaginaPosiciones(paginaPosiciones + 1)} className="rounded-lg border px-3 py-1 disabled:opacity-40">Siguiente</button></div></div></div>}</section>
        )}

        {modalPartido && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4"><section role="dialog" aria-modal="true" aria-labelledby="titulo-modal-partido" className="w-full max-w-md rounded-2xl bg-white p-6 shadow-xl"><div className="mb-5 flex items-center justify-between"><h3 id="titulo-modal-partido" className="text-xl font-bold">Programar partido</h3><button type="button" onClick={() => setModalPartido(false)} disabled={guardandoPartido} className="rounded-lg px-3 py-1 text-slate-500 hover:bg-slate-100">✕</button></div><form onSubmit={guardarPartido} className="space-y-4"><div><label htmlFor="equipoLocal" className="mb-1 block text-sm font-medium">Equipo local</label><select id="equipoLocal" value={equipoLocalId} onChange={(e) => setEquipoLocalId(e.target.value)} required className="w-full rounded-xl border border-slate-300 px-3 py-2"><option value="">Selecciona equipo local</option>{equipos.map((equipo) => <option key={equipo.id} value={equipo.id}>{equipo.nombre}</option>)}</select></div><div><label htmlFor="equipoVisitante" className="mb-1 block text-sm font-medium">Equipo visitante</label><select id="equipoVisitante" value={equipoVisitanteId} onChange={(e) => setEquipoVisitanteId(e.target.value)} required className="w-full rounded-xl border border-slate-300 px-3 py-2"><option value="">Selecciona equipo visitante</option>{equipos.filter((equipo) => equipo.id !== equipoLocalId).map((equipo) => <option key={equipo.id} value={equipo.id}>{equipo.nombre}</option>)}</select></div><div><label htmlFor="fechaPartido" className="mb-1 block text-sm font-medium">Fecha y hora</label><input id="fechaPartido" type="datetime-local" value={fechaPartido} onChange={(e) => setFechaPartido(e.target.value)} required className="w-full rounded-xl border border-slate-300 px-3 py-2" /></div>{mensajePartido && <p role="alert" className="rounded-lg bg-red-50 p-3 text-sm text-red-700">{mensajePartido}</p>}<div className="flex justify-end gap-3"><button type="button" onClick={() => setModalPartido(false)} className="rounded-xl border border-slate-200 px-4 py-2">Cancelar</button><button type="submit" disabled={guardandoPartido} className="rounded-xl bg-emerald-600 px-4 py-2 font-semibold text-white disabled:opacity-50">{guardandoPartido ? "Guardando..." : "Crear partido"}</button></div></form></section></div>
        )}

        {partidoMarcador && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4"><section role="dialog" aria-modal="true" aria-labelledby="titulo-modal-resultado" className="w-full max-w-md rounded-2xl bg-white p-6 shadow-xl"><div className="mb-5 flex items-center justify-between"><h3 id="titulo-modal-resultado" className="text-xl font-bold">Registrar resultado</h3><button type="button" onClick={() => setPartidoMarcador(null)} disabled={guardandoResultado} className="rounded-lg px-3 py-1 text-slate-500 hover:bg-slate-100">✕</button></div><p className="mb-4 text-sm text-slate-600">{partidoMarcador.nombreEquipoLocal || equipos.find((e) => e.id === partidoMarcador.equipoLocalId)?.nombre} vs {partidoMarcador.nombreEquipoVisitante || equipos.find((e) => e.id === partidoMarcador.equipoVisitanteId)?.nombre}</p><form onSubmit={guardarResultado} className="space-y-4"><div className="grid grid-cols-2 gap-4"><div><label htmlFor="golesLocal" className="mb-1 block text-sm font-medium">Goles local</label><input id="golesLocal" type="number" min="0" step="1" value={golesLocal} onChange={(e) => setGolesLocal(e.target.value)} required className="w-full rounded-xl border border-slate-300 px-3 py-2" /></div><div><label htmlFor="golesVisitante" className="mb-1 block text-sm font-medium">Goles visitante</label><input id="golesVisitante" type="number" min="0" step="1" value={golesVisitante} onChange={(e) => setGolesVisitante(e.target.value)} required className="w-full rounded-xl border border-slate-300 px-3 py-2" /></div></div>{mensajePartido && <p role="alert" className="rounded-lg bg-red-50 p-3 text-sm text-red-700">{mensajePartido}</p>}<div className="flex justify-end gap-3"><button type="button" onClick={() => setPartidoMarcador(null)} className="rounded-xl border border-slate-200 px-4 py-2">Cancelar</button><button type="submit" disabled={guardandoResultado} className="rounded-xl bg-emerald-600 px-4 py-2 font-semibold text-white disabled:opacity-50">{guardandoResultado ? "Guardando..." : "Guardar resultado"}</button></div></form></section></div>
        )}

        <footer className="mt-8 text-center text-xs text-slate-400">
          Mundialito · Sistema de gestión de torneos
        </footer>
      </main>
    </div>
  );
}
