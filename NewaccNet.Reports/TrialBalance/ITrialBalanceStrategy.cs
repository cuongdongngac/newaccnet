using SD.LLBLGen.Pro.ORMSupportClasses;
using System;
using System.Collections.Generic;
using DataAccess.EntityClasses;

namespace NewaccNet.Reports.TrialBalance
{
    public interface ITrialBalanceStrategy
    {
        /// <summary>
        /// Tính toán số dư đầu kỳ, phát sinh, cuối kỳ cho các tài khoản lá dựa trên danh sách giao dịch đã tải lên bộ nhớ.
        /// </summary>
        List<TrialBalanceEntity> CalculateLeafBalances(List<JournalEntryEntity> rawEntries, List<string> accountIds, DateTime beginDate, DateTime endDate);
    }
}
