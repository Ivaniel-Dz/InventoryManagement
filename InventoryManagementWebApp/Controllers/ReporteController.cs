using InventoryManagementWebApp.Data;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using iTextSharp.text.pdf;

namespace InventoryManagementWebApp.Controllers
{
    [Authorize(Roles = "Empleado")]
    public class ReporteController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public ReporteController(AppDBContext appDBContext)
        {
            _appDbContext = appDBContext;
        }


        public async Task<IActionResult> Stock()
        {
            // Obtener Todo los Productos con su cantidad en Stock
            var productos = await _appDbContext.Productos.ToListAsync();
            return View(productos);
        }

        public async Task<IActionResult> ExportStockExcel()
        {
            // Configurar el contexto de la licencia
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var productos = await _appDbContext.Productos.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Stock");
                worksheet.Cells[1, 1].Value = "Producto";
                worksheet.Cells[1, 2].Value = "Cantidad en Stock";
                worksheet.Cells[1, 3].Value = "Nivel Mínimo";

                int row = 2;
                foreach (var item in productos)
                {
                    worksheet.Cells[row, 1].Value = item.Nombre;
                    worksheet.Cells[row, 2].Value = item.CantidadStock;
                    worksheet.Cells[row, 3].Value = 5;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StockReport.xlsx");
            }
        }


        public async Task<IActionResult> ExportStockPDF()
        {
            var productos = await _appDbContext.Productos.ToListAsync();

            // Crear un documento PDF
            var stream = new MemoryStream();

            using (var doc = new iTextSharp.text.Document(PageSize.A4))
            {
                PdfWriter.GetInstance(doc, stream).CloseStream = false;
                doc.Open();

                // Añadir Titulo y Tabla
                doc.Add(new Paragraph("Reporte de Stock", FontFactory.GetFont("Arial", 18, Font.BOLD)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20f });

                var table = new PdfPTable(3) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 40f, 30f, 30f });
                table.AddCell("Producto"); table.AddCell("Cantidad en Stock"); table.AddCell("Cantidad Nivel Mínimo");

                productos.ForEach(p =>
                {
                    table.AddCell(p.Nombre);
                    table.AddCell(p.CantidadStock.ToString());
                    table.AddCell("5");
                });

                doc.Add(table);
                doc.Close();
            }

            stream.Position = 0;
            return File(stream, "application/pdf", "ReporteStock.pdf");
        }

    }
}
