using Common.Models;


namespace BusinessLogic
{
    /// <summary>
    /// Các hàm Ultility liên quan tới Authencation
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Đã đăng nhập thành công
        /// </summary>
        /// <returns></returns>
        bool IsAuthenticated();

        /// <summary>
        /// Lấy ra email của user đang đăng nhậ
        /// </summary>
        /// <returns></returns>
        string GetEmail();

        /// <summary>
        /// Lấy ra full tên của user đang đăng nhập
        /// </summary>
        /// <returns></returns>
        string GetFullName();

        /// <summary>
        /// Lấy ra user id của user đang đăng nhập
        /// </summary>
        /// <returns></returns>
        int GetUserId();


        bool IsAdmin();

        /// <summary>
        /// Lấy ra thông tin user gửi về cho client
        /// </summary>
        /// <returns></returns>
        UserInfo GetUserInfo();

    }
}
