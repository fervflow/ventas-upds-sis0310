﻿using System.Diagnostics;
//using upds_ventas.Utils;
using upds_ventas.Models;
using upds_ventas.Reports;
using upds_ventas.Repos;

namespace upds_ventas.Forms
{
    public partial class MenuPrincipal : Form
    {
        private readonly UsuarioRepo _usuarioRepo;
        private readonly ProveedorRepo _proveedorRepo;
        private readonly ProductoRepo _productoRepo;
        private readonly ClienteRepo _clienteRepo;
        private readonly VentaRepo _ventaRepo;

        private readonly Usuario usuarioActual;

        List<Usuario> usuarios;
        int IdUsuarioSeleccionado;
        Usuario? usuarioSeleccionado;

        List<Proveedor> proveedores;
        int IdProveedorSeleccionado;
        Proveedor? proveedorSeleccionado;
        bool buscandoProveedor;
        string nombreProvBuscar = "";

        List<Producto> productos;
        int IdProductoSeleccionado;
        Producto? productoSeleccionado;
        int IdProductoVenta;
        Producto? productoVenta;

        List<Cliente> clientes;
        int IdClienteSeleccionado;
        Cliente? clienteSeleccionado;
        Cliente? clienteVenta;

        List<DetalleVenta> detalle;
        List<Producto> productosVenta;
        int idDetalle = 0;
        decimal total;

        public MenuPrincipal(Usuario u)
        {
            _usuarioRepo = new UsuarioRepo();
            _proveedorRepo = new ProveedorRepo();
            _productoRepo = new ProductoRepo();
            _clienteRepo = new ClienteRepo();
            _ventaRepo = new VentaRepo();

            usuarioActual = u;

            usuarios = new List<Usuario>();
            proveedores = new List<Proveedor>();
            buscandoProveedor = false;
            productos = new List<Producto>();
            productosVenta = new List<Producto>();
            clientes = new List<Cliente>();
            detalle = new List<DetalleVenta>();

            InitializeComponent();
            _ = CargarUsuarios();
            _ = CargarProveedores();
            CargarProductos();
            CargarClientes();
            CargarProdVenta();

            LbUsuarioActual.Text = usuarioActual.Persona.Nombre +
                ' ' + usuarioActual.Persona.ApPaterno +
                ' ' + usuarioActual.Persona.ApMaterno +
                " (" + usuarioActual.Tipo + ')';

            txtUNombre.Text = txtUApPaterno.Text = txtUApMaterno.Text = txtUCi.Text = txtUTipo.Text = "";
            btnModificarUsuario.BaseColor = Color.DarkOliveGreen;
            btnModificarUsuario.TextColor = Color.DarkGray;
            btnEliminarUsuario.BaseColor = Color.DarkRed;
            btnEliminarUsuario.TextColor = Color.DarkGray;

            txtPvNit.Text = txtPvNombre.Text = txtPvDireccion.Text = txtPvTelefono.Text = txtPvCiudad.Text = "";
            BtnModificarProveedor.BaseColor = Color.DarkOliveGreen;
            BtnModificarProveedor.TextColor = Color.DarkGray;

            LbPrNombre.Text = LbPrStock.Text = LbPrPrecioCompra.Text = LbPrPrecioVenta.Text = LbPrProveedor.Text = "";
            BtnModificarProducto.BaseColor = Color.DarkOliveGreen;
            BtnModificarProducto.TextColor = Color.DarkGray;
            BtnModificarProducto.Enabled = false;
            BtnEliminarProducto.BaseColor = Color.DarkRed;
            BtnEliminarProducto.TextColor = Color.DarkGray;
            BtnEliminarProducto.Enabled = false;

            LbCCi.Text = LbCNombre.Text = LbCApPaterno.Text = LbCApMaterno.Text = LbCNit.Text = "";
            BtnModificarCliente.BaseColor = Color.DarkOliveGreen;
            BtnModificarCliente.TextColor = Color.DarkGray;
            BtnModificarCliente.Enabled = false;

            VNombre.Text = VApPaterno.Text = VApMaterno.Text = VCi.Text = VNit.Text = LbTotal.Text = "";
            LbTotal.Text = "0.00 Bs";
            total = 0.0m;

        }

        private async Task CargarUsuarios()
        {
            usuarios = await _usuarioRepo.ListarAsync();
            DataUsuarios.Rows.Clear();

            foreach (var u in usuarios)
            {
                object[] rowData = [
                    u.IdUsuario,
                    u.Persona.Ci!,
                    u.Persona.Nombre,
                    u.Persona.ApPaterno,
                    u.Persona.ApMaterno!,
                    u.Tipo!,
                    u.Estado ? "Habilidato" : "Deshabilitado"
                ];

                DataUsuarios.Rows.Add(rowData);
            }
            //MessageBox.Show("CargarUsuarios finalized");
        }

        private async Task CargarProveedores()
        {
            if (buscandoProveedor)
            {
                proveedores = _proveedorRepo.Buscar(nombreProvBuscar);
                Debug.WriteLine("nombreProvBuscar: " + nombreProvBuscar);
            }
            else { proveedores = await _proveedorRepo.ListarAsync(); }
            DataProveedores.Rows.Clear();
            foreach (var p in proveedores)
            {
                object[] rowData = [
                    p.IdProveedor,
                    p.Nit!,
                    p.Nombre,
                    p.Direccion,
                    p.Telefono,
                    p.Ciudad!,
                ];

                DataProveedores.Rows.Add(rowData);
            }
            //MessageBox.Show("CargarPROVEEDORES finalized");
        }

        private void CargarProductos()
        {
            productos = _productoRepo.Listar(false);

            DataProductos.Rows.Clear();

            foreach (var p in productos)
            {
                object[] rowData = [
                    p.IdProducto,
                    p.Nombre,
                    p.Stock!,
                    p.PrecioCompra!,
                    p.PrecioVenta!,
                    p.Proveedor!.Nombre,
                    p.Estado ? "Disponible" : "No disponible",
                ];

                DataProductos.Rows.Add(rowData);
            }
            //MessageBox.Show("Cargar PRODUCTOS finalized");
        }

        private void CargarClientes()
        {
            clientes = _clienteRepo.Listar();
            DataClientes.Rows.Clear();
            foreach (var c in clientes)
            {
                object[] rowData = {
                    c.IdCliente,
                    c.Persona.Ci!,
                    c.Persona.Nombre,
                    c.Persona.ApPaterno,
                    c.Persona.ApMaterno!,
                    c.Nit!
                };

                DataClientes.Rows.Add(rowData);
            }
        }

        private void CargarProdVenta()
        {
            productosVenta = _productoRepo.Listar(true);

            DataProdVenta.Rows.Clear();

            foreach (var p in productosVenta)
            {
                object[] rowData = [
                    p.IdProducto,
                    p.Nombre,
                    p.Stock!,
                    "",
                    p.PrecioVenta!,
                    p.Proveedor!.Nombre,
                ];
                DataProdVenta.Rows.Add(rowData);
            }
        }

        private void DataUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            IdUsuarioSeleccionado = (int)DataUsuarios.CurrentRow.Cells[0].Value;

            if (!btnModificarUsuario.Enabled)
            {
                btnModificarUsuario.Enabled = true;
                btnModificarUsuario.BaseColor = Color.FromArgb(35, 168, 109);
                btnModificarUsuario.TextColor = Color.WhiteSmoke;
            }
            if (!btnEliminarUsuario.Enabled)
            {
                btnEliminarUsuario.Enabled = true;
                btnEliminarUsuario.BaseColor = Color.IndianRed;
                btnEliminarUsuario.TextColor = Color.WhiteSmoke;
            }

            usuarioSeleccionado = usuarios.FirstOrDefault(u => u.IdUsuario.Equals(IdUsuarioSeleccionado))!;
            txtUNombre.Text = usuarioSeleccionado.Persona.Nombre;
            txtUApPaterno.Text = usuarioSeleccionado.Persona.ApPaterno;
            txtUApMaterno.Text = usuarioSeleccionado.Persona.ApMaterno;
            txtUCi.Text = usuarioSeleccionado.Persona.Ci;
            txtUTipo.Text = usuarioSeleccionado.Tipo;
        }

        private void DataProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            IdProveedorSeleccionado = (int)DataProveedores.CurrentRow.Cells[0].Value;

            if (!BtnModificarProveedor.Enabled)
            {
                BtnModificarProveedor.Enabled = true;
                BtnModificarProveedor.BaseColor = Color.FromArgb(35, 168, 109);
                BtnModificarProveedor.TextColor = Color.WhiteSmoke;
            }

            proveedorSeleccionado = proveedores.FirstOrDefault(p => p.IdProveedor.Equals(IdProveedorSeleccionado))!;
            txtPvNit.Text = proveedorSeleccionado.Nit;
            txtPvNombre.Text = proveedorSeleccionado.Nombre;
            txtPvDireccion.Text = proveedorSeleccionado.Direccion;
            txtPvTelefono.Text = proveedorSeleccionado.Telefono;
            txtPvCiudad.Text = proveedorSeleccionado.Ciudad;
        }

        private void DataProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            IdProductoSeleccionado = (int)DataProductos.CurrentRow.Cells[0].Value;

            if (!BtnModificarProducto.Enabled)
            {
                BtnModificarProducto.Enabled = true;
                BtnModificarProducto.BaseColor = Color.FromArgb(35, 168, 109);
                BtnModificarProducto.TextColor = Color.WhiteSmoke;
            }
            if (!BtnEliminarProducto.Enabled)
            {
                BtnEliminarProducto.Enabled = true;
                BtnEliminarProducto.BaseColor = Color.IndianRed;
                BtnEliminarProducto.TextColor = Color.WhiteSmoke;
            }

            productoSeleccionado = productos.FirstOrDefault(p => p.IdProducto.Equals(IdProductoSeleccionado))!;
            LbPrNombre.Text = productoSeleccionado.Nombre;
            LbPrStock.Text = productoSeleccionado.Stock.ToString();
            LbPrPrecioCompra.Text = productoSeleccionado.PrecioCompra.ToString();
            LbPrPrecioVenta.Text = productoSeleccionado.PrecioVenta.ToString();
            LbPrProveedor.Text = productoSeleccionado.Proveedor?.Nombre.ToString();
        }

        private void DataClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            IdClienteSeleccionado = (int)DataClientes.CurrentRow.Cells[0].Value;

            if (!BtnModificarCliente.Enabled)
            {
                BtnModificarCliente.Enabled = true;
                BtnModificarCliente.BaseColor = Color.FromArgb(35, 168, 109);
                BtnModificarCliente.TextColor = Color.WhiteSmoke;
            }

            clienteSeleccionado = clientes.FirstOrDefault(c => c.IdCliente.Equals(IdClienteSeleccionado))!;

            LbCCi.Text = clienteSeleccionado.Persona.Ci;
            LbCNombre.Text = clienteSeleccionado.Persona.Nombre;
            LbCApPaterno.Text = clienteSeleccionado.Persona.ApPaterno;
            LbCApMaterno.Text = clienteSeleccionado.Persona.ApMaterno;
            LbCNit.Text = clienteSeleccionado.Nit;
        }

        private void BtnNuevoUsuario_Click(object sender, EventArgs e)
        {
            var formSetUsuario = new SetUsuario();
            formSetUsuario.ShowDialog();
            _ = CargarUsuarios();
        }

        private void BtnModificarUsuario_Click(object sender, EventArgs e)
        {
            var formSetUsuario = new SetUsuario();
            formSetUsuario.SetForModificar(usuarioSeleccionado!);
            formSetUsuario.ShowDialog();
            _ = CargarUsuarios();
        }

        private async void BtnEliminarUsuario_ClickAsync(object sender, EventArgs e)
        {
            bool querySuccess = await _usuarioRepo.EliminarAsync(usuarioSeleccionado!);
            if (querySuccess)
            {
                MessageBox.Show("Usuario deshabilitado exitosamente.");
                _ = CargarUsuarios();
            }
            else
            {
                MessageBox.Show("Error al deshabilitar el usuario.");
            }
        }

        private void BtnNuevoProveedor_Click(object sender, EventArgs e)
        {
            var formSetProveedor = new SetProveedor();
            formSetProveedor.ShowDialog();
            _ = CargarProveedores();
        }

        private void BtnModificarProveedor_Click(object sender, EventArgs e)
        {
            var formSetProveedor = new SetProveedor();
            formSetProveedor.SetForModificar(proveedorSeleccionado!);
            formSetProveedor.ShowDialog();
            _ = CargarProveedores();
        }

        private void TbBuscarProv_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(TbBuscarProv.Text)) { buscandoProveedor = false; }
            else
            {
                buscandoProveedor = true;
                nombreProvBuscar = TbBuscarProv.Text;
            }
            _ = CargarProveedores();
        }

        private void BtnGenerarReporte_Click(object sender, EventArgs e)
        {

        }

        private void BtnNuevoProducto_Click(object sender, EventArgs e)
        {
            var formSetProducto = new SetProducto();
            formSetProducto.ShowDialog();
            CargarProductos();
        }

        private void BtnModificarProducto_Click(object sender, EventArgs e)
        {
            var formSetProducto = new SetProducto();
            formSetProducto.SetForModificar(productoSeleccionado!);
            formSetProducto.ShowDialog();
            CargarProductos();
        }

        private void BtnEliminarProducto_Click(object sender, EventArgs e)
        {
            if (productoSeleccionado!.Estado)
            {
                bool querySuccess = _productoRepo.Eliminar(productoSeleccionado!.IdProducto);
                if (querySuccess)
                {
                    MessageBox.Show("Producto deshabilitado exitosamente.");
                    CargarProductos();
                }
                else
                {
                    MessageBox.Show("Error al deshabilitar el producto.");
                }
            }
            else
            {
                MessageBox.Show("El producto ya está deshabilitado.");
            }
        }

        private void BtnNuevoCliente_Click(object sender, EventArgs e)
        {
            var formSetCliente = new SetCliente();
            formSetCliente.ShowDialog();
            CargarClientes();
        }

        private void BtnModificarCliente_Click(object sender, EventArgs e)
        {
            var formSetCliente = new SetCliente();
            formSetCliente.SetForModificar(clienteSeleccionado!);
            formSetCliente.ShowDialog();
            CargarClientes();
        }

        private void foreverLabel14_Click(object sender, EventArgs e)
        {

        }

        private void BtnBuscarCliente_Click(object sender, EventArgs e)
        {
            clienteVenta = _clienteRepo.BuscarCliente(TbNitBuscarCliente.Text);

            if (clienteVenta != null)
            {
                VNombre.Text = clienteVenta.Persona.Nombre;
                VApPaterno.Text = clienteVenta.Persona.ApPaterno;
                VApMaterno.Text = clienteVenta.Persona.ApMaterno;
                VCi.Text = clienteVenta.Persona.Ci;
                VNit.Text = clienteVenta.Nit;
            }
            else
            {
                MessageBox.Show($"Cliente con nit: '{TbNitBuscarCliente.Text}' no encontrado.");
            }
        }

        private void CargarDetalle()
        {
            DataDetalle.Rows.Clear();
            idDetalle = 0;
            total = 0.0m;
            foreach (var d in detalle)
            {
                d.IdDetalleVenta = ++idDetalle;
                object[] rowData = [
                    d.IdDetalleVenta,
                    d.IdProducto!,
                    d.Producto!.Nombre,
                    d.Cantidad!,
                    d.SubTotal!,
                    "X",
                ];
                DataDetalle.Rows.Add(rowData);
                total += d.SubTotal!.Value!;
                LbTotal.Text = total.ToString() + " Bs.";
            }
        }
        private void DataProdVenta_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            IdProductoVenta = Convert.ToInt32(DataProdVenta.CurrentRow.Cells[0].Value);

            productoVenta = productosVenta.FirstOrDefault(p => p.IdProducto.Equals(IdProductoVenta))!;

            //List<DetalleVenta> detalle = new List<DetalleVenta>();
            detalle.Add(new DetalleVenta
            {
                IdProducto = IdProductoVenta,
                Cantidad = Convert.ToInt32(TbCantidad.Text),
                SubTotal = Convert.ToDecimal(Convert.ToInt32(TbCantidad.Text)) * productoVenta.PrecioVenta,
                Producto = productoVenta,
            });
            CargarDetalle();

            LbPrNombre.Text = productoVenta.Nombre;
            LbPrStock.Text = productoVenta.Stock.ToString();
            LbPrPrecioCompra.Text = productoVenta.PrecioCompra.ToString();
            LbPrPrecioVenta.Text = productoVenta.PrecioVenta.ToString();
            LbPrProveedor.Text = productoVenta.Proveedor?.Nombre.ToString();
        }

        private void BtnRegistrarVenta_Click(object sender, EventArgs e)
        {
            Venta venta = new Venta
            {
                IdUsuario = usuarioActual.IdUsuario,
                IdCliente = clienteVenta!.IdCliente,
                Fecha = DateTime.Now,
                Total = total,
                Usuario = usuarioActual,
                Cliente = clienteVenta,
                DetalleVenta = detalle,
            };

            bool querySuccess = _ventaRepo.Insertar(venta);
            if (querySuccess)
            {
                MessageBox.Show("Venta registrada exitosamente.");
            }
            else
            {
                MessageBox.Show("Error al registrar la venta.");
            }

            //CargarDetalle();
            CargarProdVenta();
        }

        private void DataDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                var detalleEliminado = detalle.SingleOrDefault(
                    d => d.IdDetalleVenta == Convert.ToInt32(DataDetalle.CurrentRow.Cells[0].Value)
                );
                if (detalleEliminado != null)
                {
                    total -= (decimal)detalleEliminado.SubTotal!;
                    detalle.Remove(detalleEliminado);
                    DataDetalle.Rows.RemoveAt(e.RowIndex);
                }
                CargarDetalle();
            }
        }

        private void BtnCrearReporteProductos_Click(object sender, EventArgs e)
        {
            string reportPath = Utils.Utils.ReportPath("Reporte Productos");
            CrearReporteProductos reporteProductos = new CrearReporteProductos(reportPath);
            reporteProductos.GenerarReporte();
            var formVisualizarReporte = new VisualizarReporte(reportPath);
            formVisualizarReporte.ShowDialog();
        }

        private void BtnCrearReporteClientes_Click(object sender, EventArgs e)
        {
            string reportPath = Utils.Utils.ReportPath("Reporte Clientes");
            CrearReporteClientes reporteClientes = new CrearReporteClientes(reportPath);
            reporteClientes.GenerarReporte();
            var formVisualizarReporte = new VisualizarReporte(reportPath);
            formVisualizarReporte.ShowDialog();
        }

        private void BtnCrearReporteUsuarios_Click(object sender, EventArgs e)
        {
            string reportPath = Utils.Utils.ReportPath("Reporte Usuarios");
            CrearReporteUsuarios reporteUsuarios = new CrearReporteUsuarios(reportPath);
            reporteUsuarios.GenerarReporte();
            var formVisualizarReporte = new VisualizarReporte(reportPath);
            formVisualizarReporte.ShowDialog();
        }

        private void BtnCrearReporteProveedores_Click(object sender, EventArgs e)
        {
            string reportPath = Utils.Utils.ReportPath("Reporte Proveedores");
            CrearReporteProveedores reporteProveedores = new CrearReporteProveedores(reportPath);
            reporteProveedores.GenerarReporte();
            var formVisualizarReporte = new VisualizarReporte(reportPath);
            formVisualizarReporte.ShowDialog();
        }

        private void BtnCrearReporteVentas_Click(object sender, EventArgs e)
        {
            string reportPath = Utils.Utils.ReportPath("Reporte Ventas");
            CrearReporteVenta reporteVenta = new CrearReporteVenta(reportPath);
            reporteVenta.GenerarReporte();
            var formVisualizarReporte = new VisualizarReporte(reportPath);
            formVisualizarReporte.ShowDialog();
        }



        //private void BtnEliminarProveedor_Click(object sender, EventArgs e)
        //{
        //    bool querySuccess = await _proveedorRepo.EliminarProveedorAsync(proveedorSeleccionado!);
        //    if (querySuccess)
        //    {
        //        MessageBox.Show("Proveedor deshabilitado exitosamente.");
        //        CargarUsuarios();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Error al deshabilitar el proveedor.");
        //    }
        //}
    }
}
