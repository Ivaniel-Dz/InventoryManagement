﻿using InventoryManagementWebApp.Models;

namespace InventoryManagementWebApp.ViewModels
{
    public class ProductoViewModel
    {
        public string Nombre { get; set; }

        public decimal PrecioCompra { get; set; }

        public decimal PrecioVenta { get; set; }

        public int CantidadStock { get; set; }

        public string CodigoProducto { get; set; }

        public string Descripcion { get; set; }

        public int CategoriaId { get; set; } // ID de la categoría seleccionada

        // Lista de categorías para el dropdown
        public List<Categoria> Categorias { get; set; }
    }
}