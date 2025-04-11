namespace InventoryManagementWebApp.Interfaces
{
    public interface IPdfReportService
    {
        Task<MemoryStream> GeneratePdfReport<T>(
            List<T> data,
            string title,
            Dictionary<string, Func<T, object>> columnsMapping
          );
    }
}
