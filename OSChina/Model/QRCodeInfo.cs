/*
 * 原作者: blu10ph
 * 
 */

namespace OSChina. Model
{
    /// <summary>
    /// 二维码信息
    /// </summary>
    public sealed class QRCodeInfo
    {
        public bool require_login;
        public string title;
        public int type;
        public string url;
    }
}
