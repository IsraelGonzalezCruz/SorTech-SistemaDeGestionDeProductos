using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace GestorProductosWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Ventana principal del sistema SorTech - Gestión de productos electrónicos
    /// Versión mejorada con paleta de colores pastel y diseño responsive
    /// </summary>
    public partial class MainWindow : Window
    {
        // ═══════════════════════════════════════════════════════════════════
        // ATRIBUTOS PRIVADOS
        // ═══════════════════════════════════════════════════════════════════
        private GestorProductos gestor = new GestorProductos();

        // ═══════════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════════
        public MainWindow()
        {
            InitializeComponent();
            InicializarComponentes();
            CargarDatosIniciales();
            ActualizarInterfaz();
        }

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS DE INICIALIZACIÓN
        // ═══════════════════════════════════════════════════════════════════

        private void InicializarComponentes()
        {
            // Configurar ComboBox de tipo de búsqueda
            comboTipoBusqueda.Items.Add("ID");
            comboTipoBusqueda.Items.Add("Nombre");
            comboTipoBusqueda.SelectedIndex = 0;

            // Configurar ComboBox de criterio de ordenamiento
            comboCriterioOrden.Items.Add("ID");
            comboCriterioOrden.Items.Add("Precio");
            comboCriterioOrden.Items.Add("Nombre");
            comboCriterioOrden.SelectedIndex = 0;

            // Inicializar barra de progreso
            progressIteraciones.Value = 0;
            txtIteracionesValor.Text = "0";
        }

        private void CargarDatosIniciales()
        {
            try
            {
                var productosIniciales = new[]
                {
                    new Producto { Id = 3, CodigoBarras = "3", Nombre = "Teclado", Categoria = "Electrónica", Precio = 1200, Stock = 20 },
                    new Producto { Id = 4, CodigoBarras = "4", Nombre = "Mouse", Categoria = "Electrónica", Precio = 800, Stock = 35 },
                    new Producto { Id = 5, CodigoBarras = "5", Nombre = "Monitor", Categoria = "Electrónica", Precio = 4500, Stock = 10 },
                    new Producto { Id = 6, CodigoBarras = "6", Nombre = "Impresora", Categoria = "Electrónica", Precio = 3000, Stock = 5 },
                    new Producto { Id = 7, CodigoBarras = "7", Nombre = "Auriculares", Categoria = "Electrónica", Precio = 1500, Stock = 25 },
                    new Producto { Id = 8, CodigoBarras = "8", Nombre = "Cámara Web", Categoria = "Electrónica", Precio = 2000, Stock = 15 },
                    new Producto { Id = 9, CodigoBarras = "9", Nombre = "Disco Duro Externo", Categoria = "Electrónica", Precio = 3500, Stock = 12 },
                    new Producto { Id = 10, CodigoBarras = "10", Nombre = "Memoria USB", Categoria = "Electrónica", Precio = 500, Stock = 50 },
                    new Producto { Id = 11, CodigoBarras = "11", Nombre = "Sudadera", Categoria = "Ropa", Precio = 300, Stock = 15 }
                };

                foreach (var producto in productosIniciales)
                {
                    gestor.AgregarProducto(producto);
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al cargar datos iniciales", ex.Message);
            }
        }

        private void ActualizarInterfaz()
        {
            dataGridProductos.ItemsSource = null;
            dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();

            var productos = gestor.ObtenerListaProductos();
            txtInfoInventario.Text = $"Total de productos: {productos.Count} | " +
                                    $"Valor total del inventario: {productos.Sum(p => p.Precio * p.Stock):C}";
        }

        // ═══════════════════════════════════════════════════════════════════
        // MANEJADORES DE EVENTOS
        // ═══════════════════════════════════════════════════════════════════

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ventanaAgregar = new AgregarProductoWindow();

                if (ventanaAgregar.ShowDialog() == true)
                {
                    Producto nuevoProducto = ventanaAgregar.Producto;
                    gestor.AgregarProducto(nuevoProducto);

                    ActualizarInterfaz();
                    MostrarMensajeExito("Producto agregado correctamente");
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al agregar producto", ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridProductos.SelectedItem is Producto productoSeleccionado)
            {
                var resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar el producto:\n\n{productoSeleccionado.Nombre}?",
                    "Confirmar Eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    bool eliminado = gestor.EliminarProducto(productoSeleccionado.CodigoBarras);

                    if (eliminado)
                    {
                        ActualizarInterfaz();
                        MostrarMensajeExito("Producto eliminado correctamente");
                    }
                    else
                    {
                        MostrarMensajeError("Error", "No se pudo eliminar el producto");
                    }
                }
            }
            else
            {
                MostrarMensajeAdvertencia("Seleccione un producto para eliminar");
            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string criterio = comboTipoBusqueda.SelectedItem.ToString();
                string valor = txtValorBusqueda.Text.Trim();

                if (string.IsNullOrWhiteSpace(valor))
                {
                    MostrarMensajeAdvertencia("Por favor, ingrese un valor de búsqueda");
                    return;
                }

                List<Producto> productosParaBusqueda = new List<Producto>(gestor.ObtenerListaProductos());

                switch (criterio)
                {
                    case "ID":
                        if (int.TryParse(valor, out int id))
                        {
                            OrdenadorSimplificado.QuickSortPorId(productosParaBusqueda);
                            var (producto, iteraciones) = BuscadorSimplificado.BusquedaBinaria(productosParaBusqueda, id);
                            MostrarResultadoBusqueda(producto, iteraciones, "Búsqueda Binaria");
                        }
                        else
                        {
                            MostrarMensajeAdvertencia("Por favor, ingrese un ID válido (número entero)");
                        }
                        break;

                    case "Nombre":
                        var (productoNombre, iteracionesNombre) = BuscadorSimplificado.BusquedaSecuencialNombre(productosParaBusqueda, valor);
                        MostrarResultadoBusqueda(productoNombre, iteracionesNombre, "Búsqueda Secuencial");
                        break;
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error en la búsqueda", ex.Message);
            }
        }

        private void MostrarResultadoBusqueda(Producto producto, int iteraciones, string tipoAlgoritmo)
        {
            if (producto != null)
            {
                txtResultadoBusqueda.Text =
                    $"✅ PRODUCTO ENCONTRADO\n" +
                    $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    $"🆔 ID: {producto.Id}\n" +
                    $"📦 Nombre: {producto.Nombre}\n" +
                    $"🏷️ Código de Barras: {producto.CodigoBarras}\n" +
                    $"📂 Categoría: {producto.Categoria}\n" +
                    $"💰 Precio: {producto.Precio:C}\n" +
                    $"📊 Stock: {producto.Stock} unidades\n\n" +
                    $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                    $"⚡ Algoritmo: {tipoAlgoritmo}\n" +
                    $"🔄 Iteraciones realizadas: {iteraciones}";
            }
            else
            {
                txtResultadoBusqueda.Text =
                    $"❌ PRODUCTO NO ENCONTRADO\n" +
                    $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    $"No se encontró ningún producto que coincida\n" +
                    $"con los criterios de búsqueda.\n\n" +
                    $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                    $"⚡ Algoritmo: {tipoAlgoritmo}\n" +
                    $"🔄 Iteraciones realizadas: {iteraciones}";
            }

            progressIteraciones.Value = iteraciones;
            txtIteracionesValor.Text = iteraciones.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Evento para futuras validaciones en tiempo real
        }

        private void btnOrdenar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Producto> productos = new List<Producto>(gestor.ObtenerListaProductos());
                string criterio = comboCriterioOrden.SelectedItem.ToString();

                switch (criterio)
                {
                    case "ID":
                        OrdenadorSimplificado.QuickSortPorId(productos);
                        break;
                    case "Precio":
                        OrdenadorSimplificado.QuickSortPorPrecio(productos);
                        break;
                    case "Nombre":
                        productos = OrdenadorSimplificado.MergeSortPorNombre(productos);
                        break;
                }

                listViewOrdenados.ItemsSource = productos;
                DibujarGraficoBarras(productos);
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al ordenar", ex.Message);
            }
        }

        /// <summary>
        /// Dibuja gráfico de barras con espaciado mejorado y colores pastel
        /// </summary>
        private void DibujarGraficoBarras(List<Producto> productos)
        {
            try
            {
                canvasGrafico.Children.Clear();

                if (productos.Count == 0)
                {
                    MostrarMensajeInformacion("No hay productos para visualizar");
                    return;
                }

                double maxPrecio = (double)productos.Max(p => p.Precio);
                double alturaCanvas = canvasGrafico.ActualHeight > 0 ? canvasGrafico.ActualHeight : 400;
                double anchoCanvas = canvasGrafico.ActualWidth > 0 ? canvasGrafico.ActualWidth : 600;

                // MEJORA: Espaciado aumentado entre barras
                int productosAMostrar = Math.Min(productos.Count, 12);
                double anchoTotal = anchoCanvas - 40; // Márgenes
                double espacioTotal = anchoTotal / productosAMostrar;
                double anchoBarra = espacioTotal * 0.6; // 60% del espacio para la barra
                double espaciado = espacioTotal * 0.4; // 40% para el espaciado

                double escala = (alturaCanvas - 60) / maxPrecio;

                for (int i = 0; i < productosAMostrar; i++)
                {
                    double alturaBarra = (double)productos[i].Precio * escala;
                    double posicionX = 20 + (i * espacioTotal);

                    // Crear barra con gradiente pastel
                    Rectangle barra = new Rectangle
                    {
                        Width = anchoBarra,
                        Height = alturaBarra,
                        Fill = new LinearGradientBrush(
                            Color.FromRgb(100, 181, 246),  // #64B5F6
                            Color.FromRgb(33, 150, 243),   // #2196F3
                            90),
                        Stroke = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                        StrokeThickness = 2,
                        RadiusX = 6,
                        RadiusY = 6
                    };

                    Canvas.SetLeft(barra, posicionX);
                    Canvas.SetTop(barra, alturaCanvas - alturaBarra);
                    canvasGrafico.Children.Add(barra);

                    // Etiqueta de precio
                    TextBlock etiquetaPrecio = new TextBlock
                    {
                        Text = $"{productos[i].Precio:C0}",
                        FontSize = 11,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                        Width = anchoBarra,
                        TextAlignment = TextAlignment.Center
                    };

                    Canvas.SetLeft(etiquetaPrecio, posicionX);
                    Canvas.SetTop(etiquetaPrecio, alturaCanvas - alturaBarra - 20);
                    canvasGrafico.Children.Add(etiquetaPrecio);

                    // Etiqueta de nombre
                    TextBlock etiquetaNombre = new TextBlock
                    {
                        Text = productos[i].Nombre.Length > 10
                            ? productos[i].Nombre.Substring(0, 10) + "..."
                            : productos[i].Nombre,
                        FontSize = 10,
                        Foreground = new SolidColorBrush(Color.FromRgb(84, 110, 122)),
                        Width = anchoBarra + 10,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    };

                    Canvas.SetLeft(etiquetaNombre, posicionX - 5);
                    Canvas.SetTop(etiquetaNombre, alturaCanvas + 5);
                    canvasGrafico.Children.Add(etiquetaNombre);
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al dibujar gráfico", ex.Message);
            }
        }

        private void comboCriterioOrden_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Evento para futuras actualizaciones automáticas
        }

        private void dataGridProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Evento para futuras funcionalidades
        }

        /// <summary>
        /// Botón para cerrar la aplicación
        /// </summary>
        private void btnCerrarApp_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "¿Está seguro de salir de SorTech?",
                "Confirmar Salida",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS AUXILIARES PARA MENSAJES
        // ═══════════════════════════════════════════════════════════════════

        private void MostrarMensajeExito(string mensaje)
        {
            MessageBox.Show(mensaje, "✅ Éxito - SorTech", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MostrarMensajeError(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, $"❌ {titulo} - SorTech", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void MostrarMensajeAdvertencia(string mensaje)
        {
            MessageBox.Show(mensaje, "⚠️ Advertencia - SorTech", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MostrarMensajeInformacion(string mensaje)
        {
            MessageBox.Show(mensaje, "ℹ️ Información - SorTech", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}