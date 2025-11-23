using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GestorProductosWPF
{
    /// <summary>
    /// Lógica de interacción para AgregarProductoWindow.xaml
    /// Ventana modal para agregar nuevos productos al sistema SorTech
    /// Implementa validaciones y principios de usabilidad
    /// </summary>
    public partial class AgregarProductoWindow : Window
    {
        // ═══════════════════════════════════════════════════════════════════
        // PROPIEDADES PÚBLICAS
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Producto creado/editado en la ventana
        /// </summary>
        public Producto Producto { get; set; }

        // ═══════════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════════

        public AgregarProductoWindow()
        {
            InitializeComponent();
            ConfigurarValidacionesEnTiempoReal();
        }

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS DE CONFIGURACIÓN
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Configura las validaciones en tiempo real para los campos
        /// Principio de usabilidad: Prevención de errores
        /// </summary>
        private void ConfigurarValidacionesEnTiempoReal()
        {
            // Validación de números en campos numéricos
            txtId.PreviewTextInput += ValidarSoloNumeros;
            txtStock.PreviewTextInput += ValidarSoloNumeros;
            txtPrecio.PreviewTextInput += ValidarNumerosDecimales;

            // Prevenir pegado de texto no válido
            txtId.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(PrevenirPegadoInvalido));
            txtStock.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(PrevenirPegadoInvalido));
            txtPrecio.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(PrevenirPegadoInvalido));
        }

        // ═══════════════════════════════════════════════════════════════════
        // VALIDACIONES EN TIEMPO REAL
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Valida que solo se ingresen números enteros
        /// Principio de usabilidad: Prevención de errores
        /// </summary>
        private void ValidarSoloNumeros(object sender, TextCompositionEventArgs e)
        {
            // Expresión regular que solo permite dígitos
            e.Handled = !int.TryParse(e.Text, out _);
        }

        /// <summary>
        /// Valida que solo se ingresen números decimales
        /// Principio de usabilidad: Prevención de errores
        /// </summary>
        private void ValidarNumerosDecimales(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string nuevoTexto = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Permitir solo números y un punto decimal
            e.Handled = !decimal.TryParse(nuevoTexto, out _);
        }

        /// <summary>
        /// Previene el pegado de contenido no válido
        /// </summary>
        private void PrevenirPegadoInvalido(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string texto = (string)e.DataObject.GetData(typeof(string));
                if (!EsTextoNumericoValido(texto))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Verifica si el texto es numérico válido
        /// </summary>
        private bool EsTextoNumericoValido(string texto)
        {
            return decimal.TryParse(texto, out _);
        }

        // ═══════════════════════════════════════════════════════════════════
        // MANEJADORES DE EVENTOS
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Evento para el campo de categoría
        /// Se mantiene para compatibilidad con código existente
        /// </summary>
        private void txtCategoria_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Se puede agregar funcionalidad de autocompletado en el futuro
            // Principio de usabilidad: Flexibilidad y eficiencia de uso
        }

        /// <summary>
        /// Guarda el producto validando todos los campos
        /// Principio de usabilidad: Prevención de errores con validación completa
        /// </summary>
        private void btnGuardar_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar que todos los campos estén llenos
                if (!ValidarCamposCompletos())
                {
                    MostrarMensajeAdvertencia("Por favor, complete todos los campos obligatorios");
                    return;
                }

                // Validar y parsear los valores
                if (!int.TryParse(txtId.Text, out int id))
                {
                    MostrarMensajeError("El ID debe ser un número entero válido");
                    txtId.Focus();
                    return;
                }

                if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0)
                {
                    MostrarMensajeError("El precio debe ser un número positivo válido");
                    txtPrecio.Focus();
                    return;
                }

                if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
                {
                    MostrarMensajeError("El stock debe ser un número entero no negativo");
                    txtStock.Focus();
                    return;
                }

                // Crear el producto con los datos validados
                Producto = new Producto
                {
                    Id = id,
                    CodigoBarras = txtCodigoBarras.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Categoria = txtCategoria.Text.Trim(),
                    Precio = precio,
                    Stock = stock
                };

                // Indicar que el diálogo se completó exitosamente
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MostrarMensajeError($"Error al crear el producto: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida que todos los campos obligatorios estén completos
        /// Principio de usabilidad: Prevención de errores
        /// </summary>
        private bool ValidarCamposCompletos()
        {
            return !string.IsNullOrWhiteSpace(txtId.Text) &&
                   !string.IsNullOrWhiteSpace(txtCodigoBarras.Text) &&
                   !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                   !string.IsNullOrWhiteSpace(txtCategoria.Text) &&
                   !string.IsNullOrWhiteSpace(txtPrecio.Text) &&
                   !string.IsNullOrWhiteSpace(txtStock.Text);
        }

        /// <summary>
        /// Cancela la operación y cierra la ventana
        /// Principio de usabilidad: Control y libertad del usuario
        /// </summary>
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            // Confirmar si hay datos ingresados
            if (HayDatosIngresados())
            {
                var resultado = MessageBox.Show(
                    "¿Está seguro de cancelar? Se perderán los datos ingresados.",
                    "Confirmar Cancelación - SorTech",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    this.DialogResult = false;
                    this.Close();
                }
            }
            else
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        /// <summary>
        /// Cierra la ventana desde el botón X
        /// </summary>
        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            btnCancelar_Click(sender, e);
        }

        /// <summary>
        /// Verifica si hay datos ingresados en los campos
        /// </summary>
        private bool HayDatosIngresados()
        {
            return !string.IsNullOrWhiteSpace(txtId.Text) ||
                   !string.IsNullOrWhiteSpace(txtCodigoBarras.Text) ||
                   !string.IsNullOrWhiteSpace(txtNombre.Text) ||
                   !string.IsNullOrWhiteSpace(txtCategoria.Text) ||
                   !string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                   !string.IsNullOrWhiteSpace(txtStock.Text);
        }

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS AUXILIARES PARA MENSAJES
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Muestra un mensaje de error con estilo SorTech
        /// Principio de usabilidad: Prevención de errores y feedback claro
        /// </summary>
        private void MostrarMensajeError(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "❌ Error - SorTech",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Muestra un mensaje de advertencia con estilo SorTech
        /// </summary>
        private void MostrarMensajeAdvertencia(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "⚠️ Advertencia - SorTech",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}