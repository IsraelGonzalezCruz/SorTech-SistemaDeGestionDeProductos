using System;
using System.Collections.Generic;
using System.Linq;

// ═══════════════════════════════════════════════════════════════════════════════
// SISTEMA DE GESTIÓN DE PRODUCTOS - SORTECH
// Implementación de estructuras de datos y algoritmos de búsqueda y ordenamiento
// ═══════════════════════════════════════════════════════════════════════════════

// ═══════════════════════════════════════════════════════════════════════════════
// CLASE: PRODUCTO
// Representa un producto en el inventario de SorTech
// ═══════════════════════════════════════════════════════════════════════════════
public class Producto
{
    // ───────────────────────────────────────────────────────────────────────────
    // PROPIEDADES PÚBLICAS
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Identificador único del producto (numérico)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre descriptivo del producto
    /// </summary>
    public string Nombre { get; set; }

    /// <summary>
    /// Código de barras único del producto (string)
    /// Campo utilizado como clave primaria en el diccionario
    /// </summary>
    public string CodigoBarras { get; set; }

    /// <summary>
    /// Categoría a la que pertenece el producto
    /// </summary>
    public string Categoria { get; set; }

    /// <summary>
    /// Precio unitario del producto en pesos
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Cantidad disponible en inventario
    /// </summary>
    public int Stock { get; set; }

    // ───────────────────────────────────────────────────────────────────────────
    // MÉTODOS PÚBLICOS
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Representación en texto del producto
    /// Formato: [ID | Código | Nombre | Precio | Stock | Categoría]
    /// </summary>
    public override string ToString()
    {
        return $"[ID:{Id,3} | {CodigoBarras} | {Nombre,-15} | {Precio,6:C} | Stock: {Stock,3} | {Categoria}]";
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CLASE: GESTOR DE PRODUCTOS
// Administra la colección de productos utilizando estructuras de datos híbridas
// Combina List<T> para mantener orden de inserción y Dictionary<TKey,TValue> para búsquedas rápidas
// ═══════════════════════════════════════════════════════════════════════════════
public class GestorProductos
{
    // ───────────────────────────────────────────────────────────────────────────
    // ATRIBUTOS PRIVADOS
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Lista para mantener el orden de inserción de los productos
    /// Complejidad de acceso: O(n)
    /// Ventaja: Mantiene el orden de inserción
    /// </summary>
    private List<Producto> listaProductos = new List<Producto>();

    /// <summary>
    /// Diccionario para búsquedas rápidas por Código de Barras
    /// Complejidad de búsqueda: O(1) promedio
    /// Ventaja: Búsquedas instantáneas por clave única
    /// </summary>
    private Dictionary<string, Producto> diccionarioPorCodigo = new Dictionary<string, Producto>();

    // ───────────────────────────────────────────────────────────────────────────
    // MÉTODOS PÚBLICOS DE ACCESO
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Obtiene una copia de la lista de productos
    /// Retorna una nueva lista para evitar modificaciones externas
    /// Complejidad: O(n)
    /// </summary>
    /// <returns>Copia de la lista de productos</returns>
    public List<Producto> ObtenerListaProductos()
    {
        return new List<Producto>(listaProductos);
    }

    // ───────────────────────────────────────────────────────────────────────────
    // OPERACIONES CRUD (CREATE, READ, UPDATE, DELETE)
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Agrega un nuevo producto al sistema
    /// Valida que el código de barras sea único antes de insertar
    /// Complejidad: O(1) para verificación en diccionario + O(1) para inserción
    /// </summary>
    /// <param name="p">Producto a agregar</param>
    /// <exception cref="Exception">Si el código de barras ya existe</exception>
    public void AgregarProducto(Producto p)
    {
        try
        {
            // Validación de unicidad del código de barras
            // Complejidad: O(1) gracias al diccionario
            if (diccionarioPorCodigo.ContainsKey(p.CodigoBarras))
            {
                throw new Exception($"El código de barras '{p.CodigoBarras}' ya existe en el sistema.");
            }

            // Validaciones adicionales de datos
            if (string.IsNullOrWhiteSpace(p.Nombre))
            {
                throw new Exception("El nombre del producto no puede estar vacío.");
            }

            if (p.Precio < 0)
            {
                throw new Exception("El precio no puede ser negativo.");
            }

            if (p.Stock < 0)
            {
                throw new Exception("El stock no puede ser negativo.");
            }

            // Inserción en ambas estructuras de datos
            listaProductos.Add(p);                      // O(1) amortizado
            diccionarioPorCodigo[p.CodigoBarras] = p;   // O(1)
        }
        catch (Exception ex)
        {
            // Re-lanzar la excepción para manejo en capas superiores
            throw new Exception($"Error al agregar producto: {ex.Message}");
        }
    }

    /// <summary>
    /// Elimina un producto del sistema por su código de barras
    /// Complejidad: O(n) debido a la eliminación de la lista
    /// </summary>
    /// <param name="codigoBarras">Código de barras del producto a eliminar</param>
    /// <returns>True si se eliminó, False si no se encontró</returns>
    public bool EliminarProducto(string codigoBarras)
    {
        // Búsqueda en diccionario: O(1)
        if (diccionarioPorCodigo.TryGetValue(codigoBarras, out var producto))
        {
            listaProductos.Remove(producto);            // O(n) - requiere recorrer la lista
            diccionarioPorCodigo.Remove(codigoBarras);  // O(1)
            return true;
        }
        return false;
    }

    // ───────────────────────────────────────────────────────────────────────────
    // MÉTODOS DE CONSULTA Y VISUALIZACIÓN
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Muestra el inventario completo en consola
    /// Utilizado para pruebas y depuración
    /// Complejidad: O(n)
    /// </summary>
    public void MostrarInventario()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n═══════════════════════════════════════════════════════════");
        Console.WriteLine("                   INVENTARIO SORTECH                      ");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.ResetColor();

        if (listaProductos.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   No hay productos en el inventario.");
            Console.ResetColor();
        }
        else
        {
            foreach (var item in listaProductos)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"   {item.ToString()}");
                Console.ResetColor();
            }
        }

        Console.WriteLine("═══════════════════════════════════════════════════════════\n");
    }

    /// <summary>
    /// Busca un producto por su código de barras usando el diccionario
    /// Búsqueda óptima con complejidad O(1)
    /// </summary>
    /// <param name="codigoBarras">Código de barras a buscar</param>
    /// <returns>Producto encontrado o null</returns>
    public Producto BuscarPorCodigo(string codigoBarras)
    {
        return diccionarioPorCodigo.TryGetValue(codigoBarras, out var producto) ? producto : null;
    }

    /// <summary>
    /// Verifica si existe un producto con el código de barras dado
    /// Complejidad: O(1)
    /// </summary>
    /// <param name="codigoBarras">Código de barras a verificar</param>
    /// <returns>True si existe, False en caso contrario</returns>
    public bool ExisteProducto(string codigoBarras)
    {
        return diccionarioPorCodigo.ContainsKey(codigoBarras);
    }

    /// <summary>
    /// Muestra todos los productos de una categoría específica
    /// Complejidad: O(n) - requiere recorrer toda la lista
    /// </summary>
    /// <param name="categoria">Categoría a filtrar</param>
    public void MostrarProductosPorCategoria(string categoria)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"\n═══════════════════════════════════════════════════════════");
        Console.WriteLine($"   PRODUCTOS EN CATEGORÍA: {categoria.ToUpper()}");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.ResetColor();

        var productosFiltrados = listaProductos.Where(p =>
            p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase)).ToList();

        if (productosFiltrados.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"   No hay productos en la categoría '{categoria}'.");
            Console.ResetColor();
        }
        else
        {
            foreach (var item in productosFiltrados)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"   {item.ToString()}");
                Console.ResetColor();
            }
        }

        Console.WriteLine("═══════════════════════════════════════════════════════════\n");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CLASE: ORDENADOR SIMPLIFICADO
// Implementa algoritmos de ordenamiento clásicos: QuickSort y MergeSort
// ═══════════════════════════════════════════════════════════════════════════════
public class OrdenadorSimplificado
{
    // ───────────────────────────────────────────────────────────────────────────
    // QUICKSORT - Ordenamiento por ID
    // Algoritmo: Divide y vencerás
    // Complejidad: O(n log n) promedio, O(n²) peor caso
    // Ventaja: Ordenamiento in-place, eficiente en memoria
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Ordena una lista de productos por ID usando QuickSort
    /// </summary>
    /// <param name="productos">Lista a ordenar (se modifica directamente)</param>
    public static void QuickSortPorId(List<Producto> productos)
    {
        if (productos.Count <= 1) return;

        // 1. Seleccionar pivote (último elemento)
        Producto pivote = productos[productos.Count - 1];

        // 2. Particionar: elementos menores y mayores que el pivote
        var menores = new List<Producto>();
        var mayores = new List<Producto>();

        for (int i = 0; i < productos.Count - 1; i++)
        {
            if (productos[i].Id < pivote.Id)
                menores.Add(productos[i]);
            else
                mayores.Add(productos[i]);
        }

        // 3. Recursión: ordenar sublistas
        QuickSortPorId(menores);
        QuickSortPorId(mayores);

        // 4. Combinar resultados
        productos.Clear();
        productos.AddRange(menores);
        productos.Add(pivote);
        productos.AddRange(mayores);
    }

    // ───────────────────────────────────────────────────────────────────────────
    // QUICKSORT - Ordenamiento por Precio
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Ordena una lista de productos por Precio usando QuickSort
    /// </summary>
    /// <param name="productos">Lista a ordenar (se modifica directamente)</param>
    public static void QuickSortPorPrecio(List<Producto> productos)
    {
        if (productos.Count <= 1) return;

        Producto pivote = productos[productos.Count - 1];
        var menores = new List<Producto>();
        var mayores = new List<Producto>();

        for (int i = 0; i < productos.Count - 1; i++)
        {
            if (productos[i].Precio < pivote.Precio)
                menores.Add(productos[i]);
            else
                mayores.Add(productos[i]);
        }

        QuickSortPorPrecio(menores);
        QuickSortPorPrecio(mayores);

        productos.Clear();
        productos.AddRange(menores);
        productos.Add(pivote);
        productos.AddRange(mayores);
    }

    // ───────────────────────────────────────────────────────────────────────────
    // MERGESORT - Ordenamiento por Nombre
    // Algoritmo: Divide y vencerás con fusión
    // Complejidad: O(n log n) garantizado en todos los casos
    // Ventaja: Estable, predecible, ideal para datos parcialmente ordenados
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Ordena una lista de productos por Nombre usando MergeSort
    /// </summary>
    /// <param name="productos">Lista a ordenar</param>
    /// <returns>Nueva lista ordenada</returns>
    public static List<Producto> MergeSortPorNombre(List<Producto> productos)
    {
        // Caso base: lista de 0 o 1 elemento ya está ordenada
        if (productos.Count <= 1)
            return productos;

        // 1. Dividir la lista en dos mitades
        int mitad = productos.Count / 2;
        var izquierda = productos.GetRange(0, mitad);
        var derecha = productos.GetRange(mitad, productos.Count - mitad);

        // 2. Ordenar recursivamente ambas mitades
        izquierda = MergeSortPorNombre(izquierda);
        derecha = MergeSortPorNombre(derecha);

        // 3. Fusionar las mitades ordenadas
        return Mezclar(izquierda, derecha);
    }

    /// <summary>
    /// Fusiona dos listas ordenadas en una sola lista ordenada
    /// Método auxiliar para MergeSort
    /// </summary>
    /// <param name="izquierda">Lista ordenada 1</param>
    /// <param name="derecha">Lista ordenada 2</param>
    /// <returns>Lista fusionada y ordenada</returns>
    private static List<Producto> Mezclar(List<Producto> izquierda, List<Producto> derecha)
    {
        var resultado = new List<Producto>();
        int i = 0, j = 0;

        // Comparar elementos de ambas listas y agregar el menor
        while (i < izquierda.Count && j < derecha.Count)
        {
            // Comparación lexicográfica de nombres (alfabético)
            if (string.Compare(izquierda[i].Nombre, derecha[j].Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                resultado.Add(izquierda[i]);
                i++;
            }
            else
            {
                resultado.Add(derecha[j]);
                j++;
            }
        }

        // Agregar elementos restantes de la lista izquierda
        while (i < izquierda.Count)
        {
            resultado.Add(izquierda[i]);
            i++;
        }

        // Agregar elementos restantes de la lista derecha
        while (j < derecha.Count)
        {
            resultado.Add(derecha[j]);
            j++;
        }

        return resultado;
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CLASE: BUSCADOR SIMPLIFICADO
// Implementa algoritmos de búsqueda: Binaria y Secuencial
// ═══════════════════════════════════════════════════════════════════════════════
public class BuscadorSimplificado
{
    // ───────────────────────────────────────────────────────────────────────────
    // BÚSQUEDA BINARIA - Por ID
    // Algoritmo: Divide y vencerás
    // Complejidad: O(log n)
    // Requisito: Lista debe estar ordenada por ID
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Busca un producto por ID usando búsqueda binaria
    /// IMPORTANTE: La lista debe estar ordenada por ID antes de llamar este método
    /// </summary>
    /// <param name="productos">Lista ordenada de productos</param>
    /// <param name="idBuscado">ID a buscar</param>
    /// <returns>Tupla (Producto encontrado o null, número de iteraciones)</returns>
    public static (Producto, int) BusquedaBinaria(List<Producto> productos, int idBuscado)
    {
        int inicio = 0;
        int fin = productos.Count - 1;
        int iteraciones = 0;

        while (inicio <= fin)
        {
            iteraciones++;

            // 1. Calcular el punto medio
            int medio = (inicio + fin) / 2;

            // 2. Comparar el elemento del medio con el valor buscado
            if (productos[medio].Id == idBuscado)
            {
                // Elemento encontrado
                return (productos[medio], iteraciones);
            }
            else if (productos[medio].Id < idBuscado)
            {
                // El valor buscado está en la mitad derecha
                inicio = medio + 1;
            }
            else
            {
                // El valor buscado está en la mitad izquierda
                fin = medio - 1;
            }
        }

        // Elemento no encontrado
        return (null, iteraciones);
    }

    // ───────────────────────────────────────────────────────────────────────────
    // BÚSQUEDA SECUENCIAL - Por Nombre
    // Algoritmo: Búsqueda lineal
    // Complejidad: O(n)
    // Ventaja: No requiere lista ordenada
    // ───────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Busca un producto por nombre usando búsqueda secuencial
    /// La búsqueda es case-insensitive (no distingue mayúsculas/minúsculas)
    /// </summary>
    /// <param name="productos">Lista de productos</param>
    /// <param name="nombreBuscado">Nombre a buscar</param>
    /// <returns>Tupla (Producto encontrado o null, número de iteraciones)</returns>
    public static (Producto, int) BusquedaSecuencialNombre(List<Producto> productos, string nombreBuscado)
    {
        int iteraciones = 0;

        // Recorrer la lista elemento por elemento
        foreach (var producto in productos)
        {
            iteraciones++;

            // Comparar el nombre (sin distinguir mayúsculas/minúsculas)
            if (producto.Nombre.Equals(nombreBuscado, StringComparison.OrdinalIgnoreCase))
            {
                // Elemento encontrado
                return (producto, iteraciones);
            }
        }

        // Elemento no encontrado
        return (null, iteraciones);
    }
}