using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewaccNet.Core.AssetDepreciation
{
    /// <summary>
    /// MANH 5 - DIEU PHOI: noi cac manh Loader -> Calculator -> VoucherBuilder.
    /// Window chi goi service nay. Khong chua nghiep vu rieng.
    /// </summary>
    public interface IAssetDepreciationService
    {
        Task<List<AssetDepreciationRow>> CalculateAsync(AssetDepreciationParams param);
        Task<int> PostAsync(AssetDepreciationParams param, IEnumerable<AssetDepreciationRow> rows);
        Task RestoreAsync(int voucherId);
    }

    public class AssetDepreciationService : IAssetDepreciationService
    {
        private readonly IAssetSnapshotLoader _loader;
        private readonly IAssetDepreciationCalculator _calculator;
        private readonly IAssetDepreciationVoucherBuilder _voucherBuilder;

        public AssetDepreciationService()
            : this(new AssetSnapshotLoader(), new AssetDepreciationCalculator(), new AssetDepreciationVoucherBuilder())
        {
        }

        public AssetDepreciationService(
            IAssetSnapshotLoader loader,
            IAssetDepreciationCalculator calculator,
            IAssetDepreciationVoucherBuilder voucherBuilder)
        {
            _loader = loader;
            _calculator = calculator;
            _voucherBuilder = voucherBuilder;
        }

        public async Task<List<AssetDepreciationRow>> CalculateAsync(AssetDepreciationParams param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            var snapshots = await _loader.LoadAsync(param);
            return _calculator.Calculate(snapshots, param);
        }

        public Task<int> PostAsync(AssetDepreciationParams param, IEnumerable<AssetDepreciationRow> rows)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            var selected = rows.Where(r => r.Selected && r.DepAmount > 0.0001).ToList();
            if (selected.Count == 0)
                throw new InvalidOperationException("Chưa chọn tài sản nào có số tiền khấu hao > 0 để khấu hao.");
            return _voucherBuilder.CreateVoucherAsync(param, selected);
        }

        public Task RestoreAsync(int voucherId)
        {
            return _voucherBuilder.RestoreVoucherAsync(voucherId);
        }
    }
}

