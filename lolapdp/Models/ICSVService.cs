namespace lolapdp.Models
{
    /// <summary>
    /// Interface định nghĩa các phương thức đọc và ghi dữ liệu CSV
    /// </summary>
    public interface ICSVService
    {
        /// <summary>
        /// Đọc dữ liệu từ file CSV
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng cần đọc</typeparam>
        /// <param name="filePath">Đường dẫn đến file CSV</param>
        /// <returns>Danh sách các đối tượng được đọc từ file CSV</returns>
        List<T> ReadCSV<T>(string filePath) where T : class, new();

        /// <summary>
        /// Ghi dữ liệu vào file CSV
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng cần ghi</typeparam>
        /// <param name="filePath">Đường dẫn đến file CSV</param>
        /// <param name="data">Danh sách các đối tượng cần ghi</param>
        void WriteCSV<T>(string filePath, List<T> data) where T : class;
    }
}
//dbl