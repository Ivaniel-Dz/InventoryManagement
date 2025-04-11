using InventoryManagementWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementWebApp.Interfaces;
using InventoryManagementWebApp.Models;

namespace InventoryManagementWebApp.Controllers
{
    [Authorize(Roles = "Empleado")]
    public class ReporteController : Controller
    {
        // Inyencion de dependencias
        private readonly AppDBContext _appDbContext;
        private readonly IExcelReportService _excelService;
        private readonly IPdfReportService _pdfService;

        public ReporteController(AppDBContext appDBContext, IExcelReportService excelService, IPdfReportService pdfService)
        {
            _appDbContext = appDBContext;
            _excelService = excelService;
            _pdfService = pdfService;
        }

        // Lista de los productos en Stock
        public async Task<IActionResult> Stock()
        {
            // Obtener Todo los Productos con su cantidad en Stock
            var productos = await _appDbContext.Productos.ToListAsync();
            return View(productos);
        }

        // Exportar Tabla a Excel
        public async Task<IActionResult> ExportStockExcel()
        {
            var productos = await _appDbContext.Productos.ToListAsync();

            var columns = new Dictionary<string, Func<Producto, object>>
            {
                ["Producto"] = p => p.Nombre,
                ["Codigo"] = p => p.CodigoProducto,
                ["Cantidad en Stock"] = p => p.CantidadStock,
                ["Nivel Mínimo"] = p => 5 // Puedes cambiarlo por p.NivelMinimo si existe
            };

            var stream = await _excelService.GenerateExcelReport(productos, "Stock", columns);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StockReport.xlsx");
        }

        // Exportar Tabla a PDF
        public async Task<IActionResult> ExportStockPDF()
        {
            var productos = await _appDbContext.Productos.ToListAsync();

            var columns = new Dictionary<string, Func<Producto, object>>
            {
                ["Producto"] = p => p.Nombre,
                ["Codigo"] = p => p.CodigoProducto,
                ["Cantidad en Stock"] = p => p.CantidadStock,
                ["Nivel Mínimo"] = p => 5
            };

            var stream = await _pdfService.GeneratePdfReport(productos, "Reporte de Stock", columns);

            return File(stream, "application/pdf", "ReporteStock.pdf");
        }

    }
}
