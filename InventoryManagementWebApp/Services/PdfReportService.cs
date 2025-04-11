using InventoryManagementWebApp.Interfaces;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace InventoryManagementWebApp.Services
{
    public class PdfReportService : IPdfReportService
    {

        public async Task<MemoryStream> GeneratePdfReport<T>(List<T> data, string title, Dictionary<string, Func<T, object>> columnsMapping)
        {
            return await Task.Run(() =>
            {
                var stream = new MemoryStream();
                var doc = new Document(PageSize.A4);

                using (var writer = PdfWriter.GetInstance(doc, stream))
                {
                    writer.CloseStream = false;
                    doc.Open();

                    // Título (configurable)
                    var titleParagraph = new Paragraph(title, FontFactory.GetFont("Arial", 18, Font.BOLD))
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 20f
                    };
                    doc.Add(titleParagraph);

                    // Tabla
                    var table = new PdfPTable(columnsMapping.Count) { WidthPercentage = 100 };

                    // Estilo para encabezados
                    var headerFont = FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.WHITE);

                    // Encabezados con estilo
                    foreach (var header in columnsMapping.Keys)
                    {
                        var cell = new PdfPCell(new Phrase(header, headerFont))
                        {
                            BackgroundColor = new BaseColor(51, 102, 153),
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 8f
                        };
                        table.AddCell(cell);
                    }

                    // Datos
                    var dataFont = FontFactory.GetFont("Arial", 10);
                    foreach (var item in data)
                    {
                        foreach (var column in columnsMapping.Values)
                        {
                            var value = column(item);
                            var cell = new PdfPCell(new Phrase(value?.ToString() ?? "", dataFont))
                            {
                                Padding = 5f
                            };
                            table.AddCell(cell);
                        }
                    }

                    doc.Add(table);
                    doc.Close();
                }

                stream.Position = 0;
                return stream;
            });
        }

    }

}
