using System;
using System.Collections.ObjectModel;
using DataAccess.EntityClasses;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    public partial class VoucherEntryWindow
    {
        /// <summary>
        /// Khởi tạo Phiếu kế toán từ dữ liệu AI hoặc OCR đã tạo sẵn trên RAM.
        /// </summary>
        public static VoucherEntryWindow CreateForAi(JournalVoucherEntity prefilledVoucher)
        {
            var window = new VoucherEntryWindow((int?)null);
            if (prefilledVoucher != null)
            {
                window.LoadFromAiVoucher(prefilledVoucher);
            }
            return window;
        }

        /// <summary>
        /// Nạp dữ liệu chứng từ từ AI/OCR vào bộ nhớ để kế toán kiểm tra và lưu.
        /// </summary>
        public void LoadFromAiVoucher(JournalVoucherEntity aiVoucher)
        {
            if (aiVoucher == null) return;
            _voucherBatch.Clear();
            _voucherBatch.Add(aiVoucher);
            _currentVoucherIndex = 0;
            SetCurrentVoucher(aiVoucher);
            _isDirty = true;
            RefreshVoucherUI();
        }
    }
}
