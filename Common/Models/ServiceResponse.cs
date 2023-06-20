using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    /// <summary>
    /// Class chứa thông tin trả về cho client từ service
    /// </summary>
    public class ServiceResponse
    {
        /// <summary>
        /// Trạng thái của response
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Thông điệp trả về cho client
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Dữ liệu trả về cho client
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Hàm này trả về một response thành công
        /// </summary>
        /// <param name="data">Dữ liệu trả về cho client</param>
        /// <param name="message">Thông điệp trả về cho client</param>
        /// <returns>ServiceResponse</returns>
        public void OnSuccess(object data = null, string message = null)
        {
            Success = true;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Hàm này trả về một response thất bại
        /// </summary>
        /// <param name="message">Thông điệp trả về cho client</param>
        /// <param name="data">Dữ liệu trả về cho client</param>
        /// <returns>ServiceResponse</returns>  
        public void OnError(object data = null, string message = null)
        {
            Success = false;
            Message = message;
            Data = data;
        }
    }
}
