/**
 * =========================================================================================
 * GOOGLE APPS SCRIPT: HỆ THỐNG QUẢN LÝ BẢN QUYỀN NEWACCNET ERP THEO BẢN GHI JOURNALVOUCHER
 * Tác giả: Phạm Việt Cường
 * =========================================================================================
 * 
 * Hướng dẫn cài đặt trên Google Sheet:
 * 1. Mở trang tính Google Sheet của bạn.
 * 2. Chọn menu "Tiện ích mở rộng" (Extensions) -> "Apps Script".
 * 3. Dán toàn bộ nội dung file này vào và bấm biểu tượng Lưu (Save).
 * 4. Sử dụng công thức trực tiếp trên ô tính:
 *    =TAO_MA_BAN_QUYEN("BFEBFBFF000906EA", 2000)
 *    =LUAN_SO_BAN_GHI("BFEBFBFF000906EA", "51A5-97BA-576B-BE21")
 */

const SECRET_SALT = "PVCUONG#NEWACC2026$RECORD_TIER";
const MASK_VAL = 0xA75C31;
const LIMIT_INDICES = [2, 5, 8, 11, 13, 15];
const HASH_INDICES  = [0, 1, 3, 4, 6, 7, 9, 10, 12, 14];

/**
 * Tạo mã đăng ký bản quyền theo Mã máy và Số bản ghi JournalVoucher
 * @param {string} machineCode Mã máy tính của khách hàng (vd: BFEBFBFF000906EA)
 * @param {number} limit Số lượng bản ghi cho phép (vd: 500, 1000, 2000, 5000, 10000, 999999)
 * @return {string} Mã đăng ký 16 ký tự định dạng XXXX-XXXX-XXXX-XXXX
 * @customfunction
 */
function TAO_MA_BAN_QUYEN(machineCode, limit) {
  if (!machineCode) return "";
  machineCode = machineCode.toString().replace(/[^a-zA-Z0-9]/g, "").toUpperCase();
  limit = parseInt(limit, 10) || 100;

  // 1. Biến dạng số bản ghi qua mặt nạ XOR (6 ký tự hex)
  var maskedLimit = (limit ^ MASK_VAL) & 0xFFFFFF;
  var limitHex = ("000000" + maskedLimit.toString(16).toUpperCase()).slice(-6);

  // 2. Tạo chữ ký số MD5 (10 ký tự)
  var rawPayload = machineCode + "#" + limit + "#" + SECRET_SALT;
  var digest = Utilities.computeDigest(Utilities.DigestAlgorithm.MD5, rawPayload, Utilities.Charset.UTF_8);
  var hashHex = "";
  for (var i = 0; i < digest.length; i++) {
    var byteVal = digest[i];
    if (byteVal < 0) byteVal += 256;
    var byteHex = byteVal.toString(16).toUpperCase();
    if (byteHex.length < 2) byteHex = "0" + byteHex;
    hashHex += byteHex;
  }

  // 3. Đan xen tinh vi 6 ký tự limit vào 10 ký tự hash
  var result = new Array(16);
  for (var i = 0; i < LIMIT_INDICES.length; i++) {
    result[LIMIT_INDICES[i]] = limitHex.charAt(i);
  }
  for (var i = 0; i < HASH_INDICES.length; i++) {
    result[HASH_INDICES[i]] = hashHex.charAt(i);
  }

  var s = result.join("");
  return s.slice(0, 4) + "-" + s.slice(4, 8) + "-" + s.slice(8, 12) + "-" + s.slice(12, 16);
}

/**
 * Luận ngược số lượng bản ghi từ Mã đăng ký để đối chiếu kiểm tra
 * @param {string} machineCode Mã máy tính của khách hàng
 * @param {string} registrationCode Mã đăng ký đã cấp
 * @return {number|string} Số bản ghi giải mã được (hoặc thông báo lỗi)
 * @customfunction
 */
function LUAN_SO_BAN_GHI(machineCode, registrationCode) {
  if (!registrationCode) return 0;
  machineCode = (machineCode || "").toString().replace(/[^a-zA-Z0-9]/g, "").toUpperCase();
  var clean = registrationCode.toString().replace(/[^a-zA-Z0-9]/g, "").toUpperCase();
  if (clean.length !== 16) return "MÃ KHÔNG ĐỦ 16 KÝ TỰ";

  // 1. Bóc 6 ký tự số lượng
  var limitHex = "";
  for (var i = 0; i < LIMIT_INDICES.length; i++) {
    limitHex += clean.charAt(LIMIT_INDICES[i]);
  }
  var masked = parseInt(limitHex, 16);
  var deducedLimit = (masked ^ MASK_VAL) & 0xFFFFFF;

  // 2. Tạo lại mã đối chiếu
  var expectedCode = TAO_MA_BAN_QUYEN(machineCode, deducedLimit);
  if (clean === expectedCode.replace(/-/g, "")) {
    return deducedLimit; // Trả về con số bản ghi chính xác
  } else {
    return "MÃ GIẢ MẠO / SAI MÁY";
  }
}
