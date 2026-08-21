using System.Reflection;
using WhatsappComercial.Enums;
using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Servicios.Contactos
{
    public class FormularioContactoService : IFormularioContactoService
    {
        private readonly IBusquedaContactos _busquedaContacto;
        private readonly IBusquedaContactoAfiliaciones _busquedaContactoAfiliaciones;

        public FormularioContactoService(
            IBusquedaContactos busquedaContacto,
            IBusquedaContactoAfiliaciones busquedaContactoAfiliaciones)
        {
            _busquedaContacto = busquedaContacto;
            _busquedaContactoAfiliaciones = busquedaContactoAfiliaciones;
        }

        public EsquemaFormularioResult GenerarEsquema(TipoContactoEnum tipo)
        {
            var resultado = new EsquemaFormularioResult();

            Type tipoModelo = tipo switch
            {
                TipoContactoEnum.Titular => typeof(ContactoTitularFormDTO),
                TipoContactoEnum.Beneficiario => typeof(ContactoBeneficiarioFormDTO),
                TipoContactoEnum.Cobranzas => typeof(ContactoCobranzasFormDTO),
                TipoContactoEnum.Pagadurias => typeof(ContactoPagaduriasFormDTO),
                TipoContactoEnum.Relacionista => typeof(ContactoRelacionstaFormDTO),
                _ => typeof(ContactoBaseFormDTO)
            };

            var propiedades = tipoModelo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in propiedades)
            {
                if (prop.Name.StartsWith("Id", StringComparison.OrdinalIgnoreCase) &&
                    !prop.Name.Contains("Afiliaciones") &&
                    !prop.Name.Equals("IdCliente") &&
                    !prop.Name.Equals("IdParentesco"))
                {
                    continue;
                }

                var attr = prop.GetCustomAttribute<FormCampoAttribute>();
                if (attr != null && attr.Ocultar) continue;

                var campo = new CampoEsquemaDTO
                {
                    NombreCampo = prop.Name,
                    Etiqueta = attr?.Label ?? prop.Name,
                    TipoDato = prop.PropertyType,
                    EsObligatorio = attr?.Requerido ?? false,
                    ValidarExistenciaOnBlur = attr?.ValidarExistenciaOnBlur ?? false,
                    MensajeError = attr?.MensajeRequerido ?? $"El campo {attr?.Label ?? prop.Name} es obligatorio.",
                    Orden = attr?.Orden ?? 99
                };

                resultado.Campos.Add(campo);

                if (prop.PropertyType.IsEnum)
                {
                    var enumValores = Enum.GetValues(prop.PropertyType);
                    resultado.ModeloInicial[prop.Name] = enumValores.GetValue(0);
                }
                else
                {
                    resultado.ModeloInicial[prop.Name] = null;
                }
            }

            resultado.Campos = resultado.Campos.OrderBy(c => c.Orden).ToList();
            return resultado;
        }

        public async Task<ResultadoBusquedaContactoDTO> ProcesarValidacionBlurAsync(
            TipoContactoEnum tipo,
            CampoEsquemaDTO campo,
            Dictionary<string, object?> modeloDinamico)
        {
            var valorBuscado = modeloDinamico.TryGetValue(campo.NombreCampo, out var val) ? val?.ToString() ?? "" : "";
            if (string.IsNullOrWhiteSpace(valorBuscado))
                return new ResultadoBusquedaContactoDTO { Exitoso = false };

            return tipo switch
            {
                TipoContactoEnum.Titular => await ProcesarTitularAsync(valorBuscado, modeloDinamico, campo),
                TipoContactoEnum.Beneficiario => await ProcesarBeneficiarioAsync(campo, valorBuscado, modeloDinamico),
                TipoContactoEnum.Pagadurias => await ProcesarPagaduriaAsync(valorBuscado, modeloDinamico, campo),
                TipoContactoEnum.Cobranzas => await ProcesarCobranzasAsync(valorBuscado, modeloDinamico, campo),
                TipoContactoEnum.Relacionista => await ProcesarRelacionistaAsync(valorBuscado),
                _ => new ResultadoBusquedaContactoDTO { Exitoso = false }
            };
        }

        private async Task<ResultadoBusquedaContactoDTO> ProcesarTitularAsync(string valor, Dictionary<string, object?> modelo, CampoEsquemaDTO campo)
        {
            var resultados = await _busquedaContactoAfiliaciones.BuscarTitularAfiliaciones(valor);
            var afiliacion = resultados.FirstOrDefault();

            if (afiliacion == null)
                return Advertencia("No Encontrado", "El titular no existe en Afiliaciones y es obligatorio.");

            var clientesLocales = await _busquedaContacto.ObtenerClienteTitularAsync(valor);
            if (clientesLocales.Any())
                return Advertencia("Ya Registrado", "El cliente ya se encuentra registrado en el sistema local.");

            var cliente = new Clientes
            {
                NumIdentificacionCliente = afiliacion.NroIdentificacion,
                NombreCompletoCliente = afiliacion.NombreContacto,
                NumCelularCliente = afiliacion.CelularContacto,
                CorreoElectronico = afiliacion.CorreoElectronico,
                IdClienteAfiliaciones = afiliacion.IdContacto,
                Genero = GeneroEnum.Seleccione
            };

            AutocompletarDesdeModelo(cliente, modelo);
            return Éxito("Datos Cargados", "Se autocompletaron los campos del Titular.");
        }

        private async Task<ResultadoBusquedaContactoDTO> ProcesarBeneficiarioAsync(CampoEsquemaDTO campo, string valor, Dictionary<string, object?> modelo)
        {
            // Si el usuario está digitando en el campo de identificación del Titular
            if (campo.NombreCampo.Contains("Titular", StringComparison.OrdinalIgnoreCase))
            {
                // 1. OBTENER ID DEL CLIENTE TITULAR (BASE DE DATOS LOCAL)
                var clientesLocales = await _busquedaContacto.ObtenerClienteTitularAsync(valor);
                var clienteTitularLocal = clientesLocales.FirstOrDefault();

                if (clienteTitularLocal == null)
                {
                    return Advertencia("Titular No Registrado",
                        "El titular no existe en la base de datos local. Debe registrarlo como Titular antes de agregar sus beneficiarios.");
                }

                // Asignamos inmediatamente el IdCliente local al modelo del formulario
                int idClienteTitularLocal = clienteTitularLocal.IdContacto; // O la propiedad PK exacta de tu entidad Cliente (ej. IdCliente)
                modelo["IdCliente"] = idClienteTitularLocal;

                // 2. OBTENER BENEFICIARIOS DESDE AFILIACIONES
                var beneficiariosAfiliaciones = await _busquedaContactoAfiliaciones.BuscarBeneficiarioAfiliaciones(valor);
                if (!beneficiariosAfiliaciones.Any())
                {
                    return Advertencia("Sin Beneficiarios",
                        "El titular está registrado localmente, pero no posee beneficiarios en la base de Afiliaciones.");
                }

                // Si solo hay 1 beneficiario en Afiliaciones, lo mapeamos automáticamente
                if (beneficiariosAfiliaciones.Count == 1)
                {
                    MapearBeneficiario(beneficiariosAfiliaciones.First(), idClienteTitularLocal, modelo);
                }

                return new ResultadoBusquedaContactoDTO
                {
                    Exitoso = true,
                    EsAdvertencia = false,
                    Titulo = "Titular Validado",
                    Mensaje = $"Titular local encontrado (ID: {idClienteTitularLocal}). Se trajeron {beneficiariosAfiliaciones.Count} beneficiarios de Afiliaciones.",
                    BeneficiariosEncontrados = beneficiariosAfiliaciones,
                    IdClienteTitularLocal = idClienteTitularLocal
                };
            }
            else
            {
                // Validación habitual si digita la identificación del propio beneficiario
                var locales = await _busquedaContacto.ObtenerBeneficiarioAsync(valor);
                if (locales.Any())
                    return Advertencia("Beneficiario Existente", "El beneficiario ya está registrado en el sistema local.");

                return new ResultadoBusquedaContactoDTO { Exitoso = true };
            }
        }

        private async Task<ResultadoBusquedaContactoDTO> ProcesarPagaduriaAsync(string valor, Dictionary<string, object?> modelo, CampoEsquemaDTO campo)
        {
            var res = await _busquedaContactoAfiliaciones.BuscarTitularAfiliaciones(valor);
            var afil = res.FirstOrDefault();
            if (afil == null) return Advertencia("No Encontrado", "La pagaduría no fue encontrada en Afiliaciones.");

            var locales = await _busquedaContacto.ObtenerClientePagaduriasAsync(valor);
            if (locales.Any()) return Advertencia("Pagaduría Existente", "La pagaduría ya está registrada localmente.");

            AutocompletarDesdeModelo(new Pagaduria
            {
                Identificacion = afil.NroIdentificacion,
                Pagadurias = afil.NombreContacto,
                Celular = afil.CelularContacto
            }, modelo);

            return Éxito("Datos Cargados", "Pagaduría cargada correctamente.");
        }

        private async Task<ResultadoBusquedaContactoDTO> ProcesarCobranzasAsync(string valor, Dictionary<string, object?> modelo, CampoEsquemaDTO campo)
        {
            var res = await _busquedaContactoAfiliaciones.BuscarTitularAfiliaciones(valor);
            var afil = res.FirstOrDefault();
            if (afil == null) return Advertencia("No Encontrado", "El registro no existe en Afiliaciones.");

            var locales = await _busquedaContacto.ObtenerClienteCobranzasAsync(valor);
            if (locales.Any()) return Advertencia("Cobranza Existente", "El registro ya existe en Cobranzas.");

            AutocompletarDesdeModelo(new BaseCobranzas
            {
                Cedula = afil.NroIdentificacion,
                Nombre = afil.NombreContacto,
                Telefono = afil.CelularContacto
            }, modelo);

            return Éxito("Datos Cargados", "Registro de cobranza cargado.");
        }

        private async Task<ResultadoBusquedaContactoDTO> ProcesarRelacionistaAsync(string valor)
        {
            var locales = await _busquedaContacto.ObtenerContactoAsync(valor);
            if (locales.Any()) return Advertencia("Contacto Existente", "El relacionista ya se encuentra registrado.");
            return new ResultadoBusquedaContactoDTO { Exitoso = true };
        }

        public void MapearBeneficiarioSeleccionado(object idContacto, List<Contacto> beneficiariosAfiliaciones, int? idClienteTitularLocal, Dictionary<string, object?> modelo)
        {
            if (idContacto == null) return;

            int id = Convert.ToInt32(idContacto);
            var beneficiarioAfil = beneficiariosAfiliaciones.FirstOrDefault(x => x.IdContacto == id);

            if (beneficiarioAfil != null)
            {
                MapearBeneficiario(beneficiarioAfil, idClienteTitularLocal, modelo);
            }
        }

        private void MapearBeneficiario(Contacto c, int? idClienteTitularLocal, Dictionary<string, object?> modelo)
        {
            // 1. Mapeamos los datos que vienen DE AFILIACIONES
            if (modelo.ContainsKey("NombreCompleto"))
                modelo["NombreCompleto"] = c.NombreContacto;

            if (modelo.ContainsKey("Telefono"))
                modelo["Telefono"] = c.CelularContacto;

            if (modelo.ContainsKey("IdBeneficiarioAfiliaciones"))
                modelo["IdBeneficiarioAfiliaciones"] = c.IdContacto;

            if (modelo.ContainsKey("IdAfiliacionesBeneficiario"))
                modelo["IdAfiliacionesBeneficiario"] = c.IdContacto;

            // 2. INYECTAMOS EL IdCliente QUE VIENE DE LA BASE DE DATOS LOCAL
            if (idClienteTitularLocal.HasValue && idClienteTitularLocal.Value > 0)
            {
                modelo["IdCliente"] = idClienteTitularLocal.Value;
            }
        }

        private void AutocompletarDesdeModelo<TModelo>(TModelo origen, Dictionary<string, object?> destino) where TModelo : class
        {
            if (origen == null) return;
            var props = typeof(TModelo).GetProperties();

            foreach (var key in destino.Keys.ToList())
            {
                if (key.Contains("Titular", StringComparison.OrdinalIgnoreCase)) continue;

                var prop = props.FirstOrDefault(p => key.Equals(p.Name, StringComparison.OrdinalIgnoreCase) || MapeoFlexible(key, p.Name));
                if (prop != null)
                {
                    var val = prop.GetValue(origen);
                    if (val != null) destino[key] = val;
                }
            }
        }

        private bool MapeoFlexible(string clave, string prop)
        {
            if ((clave.Contains("Cedula", StringComparison.OrdinalIgnoreCase) || clave.Contains("Documento", StringComparison.OrdinalIgnoreCase)) && prop.Equals("Identificacion", StringComparison.OrdinalIgnoreCase)) return true;
            if ((clave.Contains("Celular", StringComparison.OrdinalIgnoreCase) || clave.Contains("Telefono", StringComparison.OrdinalIgnoreCase)) && prop.Equals("CelularContacto", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private ResultadoBusquedaContactoDTO Advertencia(string titulo, string msg) =>
            new() { Exitoso = false, EsAdvertencia = true, Titulo = titulo, Mensaje = msg };

        private ResultadoBusquedaContactoDTO Éxito(string titulo, string msg) =>
            new() { Exitoso = true, EsAdvertencia = false, Titulo = titulo, Mensaje = msg };
    }
}

