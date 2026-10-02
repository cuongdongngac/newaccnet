using DevExpress.DataAccess.ObjectBinding;
using System;

namespace NewaccNet.Reports
{
    [HighlightedClass]
    public class TrialBalanceDTO
    {
        // Constructor rỗng bắt buộc để DevExpress Report Designer có thể nhận diện qua Object DataSource
        public TrialBalanceDTO() 
        { 
        }

        /// <summary>
        /// Mã tài khoản
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Dư Nợ đầu kỳ
        /// </summary>
        public decimal BeginDebit { get; set; }

        /// <summary>
        /// Dư Có đầu kỳ
        /// </summary>
        public decimal BeginCredit { get; set; }

        /// <summary>
        /// Phát sinh Nợ trong kỳ
        /// </summary>
        public decimal IntDebit { get; set; }

        /// <summary>
        /// Phát sinh Có trong kỳ
        /// </summary>
        public decimal IntCredit { get; set; }

        /// <summary>
        /// Dư Nợ cuối kỳ
        /// </summary>
        public decimal EndDebit { get; set; }

        /// <summary>
        /// Dư Có cuối kỳ
        /// </summary>
        public decimal EndCredit { get; set; }

        /// <summary>
        /// Cờ đánh dấu đây là tài khoản cha (dữ liệu tổng hợp từ các tài khoản con).
        /// Hữu ích để áp dụng style in đậm (Highlight/Bold) hoặc ẩn/hiện trên báo cáo.
        /// Tương ứng với trường Splite trong database.
        /// </summary>
        public bool Splite { get; set; }
        
        /// <summary>
        /// Cờ dùng riêng cho giao diện báo cáo (Report) để định dạng (VD: đổi màu nền, in đậm)
        /// Giá trị thường đồng bộ với Splite, hoặc tính toán thêm tùy logic hiển thị.
        /// </summary>
        public bool Highlight { get; set; }
    }
}
