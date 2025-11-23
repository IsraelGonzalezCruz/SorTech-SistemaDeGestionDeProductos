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

        /// <summary>
        /// Inicializa los controles de la interfaz con valores predeterminados
        /// Principio de usabilidad: Valores por defecto que facilitan el uso
        /// </summary>
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

        /// <summary>
        /// Carga el conjunto de datos inicial en el sistema
        /// Ley de Jakob: Los usuarios esperan que el sistema tenga datos de ejemplo
        /// </summary>
        private void CargarDatosIniciales()
        {
            try
            {
                // Productos de electrónica para demostración
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

        /// <summary>
        /// Actualiza todos los elementos de la interfaz con los datos actuales
        /// Principio de usabilidad: Visibilidad del estado del sistema
        /// </summary>
        private void ActualizarInterfaz()
        {
            // Actualizar DataGrid de inventario
            dataGridProductos.ItemsSource = null;
            dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();

            // Actualizar información del footer
            var productos = gestor.ObtenerListaProductos();
            txtInfoInventario.Text = $"Total de productos: {productos.Count} | " +
                                    $"Valor total del inventario: {productos.Sum(p => p.Precio * p.Stock):C}";
        }

        // ═══════════════════════════════════════════════════════════════════
        // MANEJADORES DE EVENTOS - GESTIÓN DE PRODUCTOS
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Abre la ventana modal para agregar un nuevo producto
        /// Principio de usabilidad: Control y libertad del usuario
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ventanaAgregar = new AgregarProductoWindow();

                // ShowDialog bloquea la ventana principal hasta que se cierre la modal
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

        /// <summary>
        /// Elimina el producto seleccionado del inventario
        /// Principio de usabilidad: Prevención de errores con confirmación
        /// </summary>
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridProductos.SelectedItem is Producto productoSeleccionado)
            {
                // Confirmación antes de eliminar - Prevención de errores
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

        // ═══════════════════════════════════════════════════════════════════
        // MANEJADORES DE EVENTOS - BÚSQUEDA
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Realiza la búsqueda de productos según el criterio seleccionado
        /// Implementa algoritmos de búsqueda binaria y secuencial
        /// </summary>
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string criterio = comboTipoBusqueda.SelectedItem.ToString();
                string valor = txtValorBusqueda.Text.Trim();

                // Validación de entrada - Prevención de errores
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
                            // Ordenar antes de búsqueda binaria
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

        /// <summary>
        /// Muestra el resultado de la búsqueda en formato visualmente atractivo
        /// Principio de usabilidad: Visibilidad del estado del sistema y feedback
        /// </summary>
        private void MostrarResultadoBusqueda(Producto producto, int iteraciones, string tipoAlgoritmo)
        {
            if (producto != null)
            {
                // Formato mejorado del resultado
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

            // Actualizar barra de progreso con animación suave
            progressIteraciones.Value = iteraciones;
            txtIteracionesValor.Text = iteraciones.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Evento para futuras validaciones en tiempo real
        }

        // ═══════════════════════════════════════════════════════════════════
        // MANEJADORES DE EVENTOS - ORDENAMIENTO Y VISUALIZACIÓN
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Ordena los productos según el criterio seleccionado
        /// Implementa QuickSort y MergeSort según el tipo de dato
        /// </summary>
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
        /// Dibuja un gráfico de barras interactivo con los precios de los productos
        /// Principio de usabilidad: Reconocimiento antes que recordar (visualización de datos)
        /// Ley de Fitts: Elementos visuales de tamaño adecuado
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

                // Calcular escala y espaciado
                double escala = (alturaCanvas - 40) / maxPrecio;
                double anchoBarra = Math.Min(30, anchoCanvas / (productos.Count * 1.5));
                double espaciado = anchoBarra * 0.5;

                for (int i = 0; i < productos.Count && i < 15; i++) // Limitar a 15 productos
                {
                    double alturaBarra = (double)productos[i].Precio * escala;
                    double posicionX = i * (anchoBarra + espaciado) + espaciado;

                    // Crear barra con gradiente
                    Rectangle barra = new Rectangle
                    {
                        Width = anchoBarra,
                        Height = alturaBarra,
                        Fill = new LinearGradientBrush(
                            Color.FromRgb(79, 195, 247),  // #4FC3F7
                            Color.FromRgb(3, 155, 229),   // #039BE5
                            90),
                        Stroke = new SolidColorBrush(Color.FromRgb(79, 195, 247)),
                        StrokeThickness = 1,
                        RadiusX = 4,
                        RadiusY = 4
                    };

                    Canvas.SetLeft(barra, posicionX);
                    Canvas.SetTop(barra, alturaCanvas - alturaBarra);
                    canvasGrafico.Children.Add(barra);

                    // Etiqueta de precio
                    TextBlock etiquetaPrecio = new TextBlock
                    {
                        Text = $"{productos[i].Precio:C0}",
                        FontSize = 10,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(79, 195, 247)),
                        Width = anchoBarra,
                        TextAlignment = TextAlignment.Center
                    };

                    Canvas.SetLeft(etiquetaPrecio, posicionX);
                    Canvas.SetTop(etiquetaPrecio, alturaCanvas - alturaBarra - 18);
                    canvasGrafico.Children.Add(etiquetaPrecio);

                    // Etiqueta de nombre (rotada si es necesario)
                    TextBlock etiquetaNombre = new TextBlock
                    {
                        Text = productos[i].Nombre.Length > 8
                            ? productos[i].Nombre.Substring(0, 8) + "..."
                            : productos[i].Nombre,
                        FontSize = 9,
                        Foreground = new SolidColorBrush(Color.FromRgb(176, 190, 197)),
                        Width = anchoBarra,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    };

                    Canvas.SetLeft(etiquetaNombre, posicionX);
                    Canvas.SetTop(etiquetaNombre, alturaCanvas + 2);
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

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS AUXILIARES PARA MENSAJES
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Muestra un mensaje de éxito al usuario
        /// Principio de usabilidad: Feedback inmediato
        /// </summary>
        private void MostrarMensajeExito(string mensaje)
        {
            MessageBox.Show(mensaje, "✅ Éxito - SorTech", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Muestra un mensaje de error al usuario
        /// Principio de usabilidad: Prevención y manejo de errores
        /// </summary>
        private void MostrarMensajeError(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, $"❌ {titulo} - SorTech", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Muestra un mensaje de advertencia al usuario
        /// </summary>
        private void MostrarMensajeAdvertencia(string mensaje)
        {
            MessageBox.Show(mensaje, "⚠️ Advertencia - SorTech", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Muestra un mensaje informativo al usuario
        /// </summary>
        private void MostrarMensajeInformacion(string mensaje)
        {
            MessageBox.Show(mensaje, "ℹ️ Información - SorTech", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dataGridProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}