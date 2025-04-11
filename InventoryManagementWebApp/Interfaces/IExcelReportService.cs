namespace InventoryManagementWebApp.Interfaces
{
    public interface IExcelReportService
    {
       Task<MemoryStream> GenerateExcelReport<T>(
            List<T> data, 
            string sheetName, 
            Dictionary<string, Func<T, object>> columnsMapping
          );
    }
}
