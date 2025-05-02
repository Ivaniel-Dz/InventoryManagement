using SistemaInventario.Interfaces;
using OfficeOpenXml;

namespace SistemaInventario.Services
{
    public class ExcelReportService : IExcelReportService
    {
        public async Task<MemoryStream> GenerateExcelReport<T>(List<T> data, string sheetName, Dictionary<string, Func<T, object>> columnsMapping)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Compatible solo con EPPlus version 7

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            // Encabezados
            int col = 1;
            foreach (var header in columnsMapping.Keys)
            {
                worksheet.Cells[1, col].Value = header;
                col++;
            }

            // Datos
            int row = 2;
            foreach (var item in data)
            {
                col = 1;
                foreach (var column in columnsMapping.Values)
                {
                    worksheet.Cells[row, col].Value = column(item);
                    col++;
                }
                row++;
            }

            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0;

            return stream;
        }
    }

}
