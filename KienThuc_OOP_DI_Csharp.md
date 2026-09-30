# TỔNG HỢP KIẾN THỨC: TỪ CODE BẢN NĂNG SANG CHUẨN OOP, REFLECTION VÀ DEPENDENCY INJECTION (C#)

## 1. Chương trình Chuyển Số Sang Chữ (Bản Pro - English Clean Code)

Dưới đây là đoạn mã hoàn chỉnh đã được tối ưu hóa theo tiêu chuẩn Clean Code quốc tế. Tên biến, tên hàm bằng tiếng Anh (PascalCase/camelCase), trong khi kết quả đầu ra hiển thị chính xác cấu trúc ngữ pháp tiếng Việt (mốt, lăm, linh/lẻ).

```csharp
using System;
using System.Text;

class Program
{
    // Arrays for basic digits and scale units
    private static readonly string[] Digits = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
    private static readonly string[] ScaleUnits = { "", "nghìn", "triệu", "tỷ" };

    static void Main(string[] args)
    {
        // Configure console to display Vietnamese characters correctly
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Test cases with various large numbers
        long[] testNumbers = { 0, 5, 15, 21, 105, 2405, 100004, 123456789, 900000000012 };

        Console.WriteLine("--- NUMBER TO WORDS CONVERTER ---");
        foreach (var num in testNumbers)
        {
            Console.WriteLine($"{num,15} -> {ConvertNumberToWords(num)}");
        }

        // Interactive user input
        Console.Write("\nEnter any integer to test: ");
        if (long.TryParse(Console.ReadLine(), out long userInput))
        {
            Console.WriteLine($"Result: {ConvertNumberToWords(userInput)}");
        }
        else
        {
            Console.WriteLine("Invalid number entered!");
        }
    }

    /// <summary>
    /// Core method to convert a large integer into Vietnamese words
    /// </summary>
    public static string ConvertNumberToWords(long number)
    {
        if (number == 0) return "Không";

        string prefix = "";
        if (number < 0)
        {
            prefix = "Âm ";
            number = Math.Abs(number);
        }

        string result = "";
        int scaleIndex = 0;

        // Process the number in groups of 3 digits from right to left
        while (number > 0)
        {
            int triplet = (int)(number % 1000);
            number /= 1000;

            if (triplet > 0)
            {
                // Only show "không trăm" if there are more significant digits ahead
                bool showZeroHundred = (number > 0);
                string tripletText = ConvertTripletToWords(triplet, showZeroHundred);
                
                result = tripletText + " " + ScaleUnits[scaleIndex] + " " + result;
            }
            // Special handling for the "Tỷ" (Billion) boundary
            else if (scaleIndex == 3 && number > 0) 
            {
                result = ScaleUnits[scaleIndex] + " " + result;
            }

            scaleIndex++;
            // Wrap around the scale index if the number exceeds billions (e.g., thousands of billions)
            if (scaleIndex > 3 && number > 0) scaleIndex = 1; 
        }

        // Capitalize the first letter and trim extra spaces
        result = prefix + result.Trim();
        return char.ToUpper(result) + result.Substring(1);
    }

    /// <summary>
    /// Processes a 3-digit group (e.g., 105 -> một trăm linh năm)
    /// </summary>
    private static string ConvertTripletToWords(int triplet, bool showZeroHundred)
    {
        int hundreds = triplet / 100;
        int tens = (triplet % 100) / 10;
        int ones = triplet % 10;

        StringBuilder sb = new StringBuilder();

        // 1. Handle Hundreds place
        if (hundreds > 0 || showZeroHundred)
        {
            sb.Append(Digits[hundreds]).Append(" trăm ");
        }

        // 2. Handle Tens place
        if (tens > 1)
        {
            sb.Append(Digits[tens]).Append(" mươi ");
        }
        else if (tens == 1)
        {
            sb.Append("mười ");
        }
        else if (tens == 0 && ones > 0)
        {
            // If hundreds place is rendered but tens is zero, insert "linh" or "lẻ"
            if (hundreds > 0 || showZeroHundred)
            {
                sb.Append("linh "); 
            }
        }

        // 3. Handle Ones place with grammar variations
        if (ones > 0)
        {
            if (ones == 1 && tens > 1)
            {
                sb.Append("mốt"); 
            }
            else if (ones == 5 && tens > 0)
            {
                sb.Append("lăm"); 
            }
            else
            {
                sb.Append(Digits[ones]);
            }
        }

        return sb.ToString().Trim();
    }
}
```

---

## 2. Tài Liệu Nghiên Cứu và Lộ Trình Phát Triển Tư Duy Bài Bản

Để bẻ gãy thói quen "code ăn xổi", bạn cần nghiên cứu có hệ thống theo lộ trình dưới đây:

### 🗺️ Chuỗi tư duy logic nền tảng
`[OOP vững chắc / SOLID] ──> [Reflection (Đọc code bằng code)] ──> [Dependency Injection (Tự động hóa khởi tạo)]`

### 📚 Tài liệu gối đầu giường
1. **Chuẩn hóa OOP & Thiết kế hệ thống:**
   * **Sách:** *Head First Design Patterns* (Eric Freeman & Elisabeth Robson) – Cực trực quan, dạy cách tư duy bằng Interface và cấu thành thay vì kế thừa.
   * **Sách:** *Clean Code* (Robert C. Martin) – Tiêu chuẩn để viết mã sạch, dễ bảo trì.
2. **Reflection & Dependency Injection chuyên sâu (C#/.NET):**
   * **Tài liệu chính thức (Microsoft docs):** [Reflection in .NET](https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection) (Quét metadata tại runtime).
   * **Tài liệu chính thức (Microsoft docs):** [Dependency injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) (Quét và quản lý vòng đời đối tượng).
   * **Sách chuyên sâu:** *Dependency Injection Principles, Practices, and Patterns* (Steven van Deursen & Mark Seemann) – Kinh thánh về DI.
3. **Kênh Video chất lượng cao (YouTube/Udemy):**
   * **Milan Jovanović:** Chuyên gia về .NET Advanced & Clean Architecture.
   * **Nick Chapsas:** Kênh chuyên sâu về các tính năng C# tối thượng và tối ưu hiệu năng.

---

## 3. Bản Chất Cross-Framework (Tính Bất Tử của Tư Duy)

Khi bạn đã vững kiến trúc bên dưới, việc đổi ngôn ngữ hay framework chỉ là thay đổi cú pháp bên ngoài. Bản chất tư duy kiến trúc là đồng nhất:

| Khái niệm | Trong C# (.NET Core) | Trong Java (Spring Boot) | Trong TypeScript (NestJS) |
| :--- | :--- | :--- | :--- |
| **Khai báo Interface** | `public interface IService` | `public interface Service` | `export interface Service` |
| **Cơ chế Reflection** | `System.Reflection` / `Type` | `java.lang.reflect` / `Class` | `Reflect-metadata` / `Reflect` |
| **Đăng ký DI (DI Container)** | `builder.Services.AddScoped<...>()` | `@Component` / `@Service` | `@Injectable()` |
| **Bơm Dependency (Inject)** | Qua Constructor | Qua `@Autowired` hoặc Constructor | Qua Constructor |

---

## 4. Minh Họa "Bẻ Lái" Sang Cấu Trúc OOP & Dependency Injection

Dưới đây là cách tái cấu trúc (Refactor) bài toán trên thành mô hình dịch vụ bài bản để áp dụng vào các dự án lớn, tuân thủ nguyên lý SOLID.

### Bước 1: Định nghĩa Interface (Hợp đồng)
```csharp
public interface INumberToWordsConverter
{
    string Convert(long number);
}
```

### Bước 2: Triển khai cụ thể (Implementation Class)
```csharp
using System;
using System.Text;

public class VietnameseNumberConverter : INumberToWordsConverter
{
    private static readonly string[] Digits = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
    private static readonly string[] ScaleUnits = { "", "nghìn", "triệu", "tỷ" };

    public string Convert(long number)
    {
        if (number == 0) return "Không";

        string prefix = "";
        if (number < 0)
        {
            prefix = "Âm ";
            number = Math.Abs(number);
        }

        string result = "";
        int scaleIndex = 0;

        while (number > 0)
        {
            int triplet = (int)(number % 1000);
            number /= 1000;

            if (triplet > 0)
            {
                bool showZeroHundred = (number > 0);
                string tripletText = ConvertTripletToWords(triplet, showZeroHundred);
                result = tripletText + " " + ScaleUnits[scaleIndex] + " " + result;
            }
            else if (scaleIndex == 3 && number > 0) 
            {
                result = ScaleUnits[scaleIndex] + " " + result;
            }

            scaleIndex++;
            if (scaleIndex > 3 && number > 0) scaleIndex = 1; 
        }

        result = prefix + result.Trim();
        return char.ToUpper(result) + result.Substring(1);
    }

    private string ConvertTripletToWords(int triplet, bool showZeroHundred)
    {
        int hundreds = triplet / 100;
        int tens = (triplet % 100) / 10;
        int ones = triplet % 10;

        StringBuilder sb = new StringBuilder();

        if (hundreds > 0 || showZeroHundred)
        {
            sb.Append(Digits[hundreds]).Append(" trăm ");
        }

        if (tens > 1)
        {
            sb.Append(Digits[tens]).Append(" mươi ");
        }
        else if (tens == 1)
        {
            sb.Append("mười ");
        }
        else if (tens == 0 && ones > 0 && (hundreds > 0 || showZeroHundred))
        {
            sb.Append("linh "); 
        }

        if (ones > 0)
        {
            if (ones == 1 && tens > 1) sb.Append("mốt"); 
            else if (ones == 5 && tens > 0) sb.Append("lăm"); 
            else sb.Append(Digits[ones]);
        }

        return sb.ToString().Trim();
    }
}
```

### Bước 3: Tạo Service nhận Dependency (Constructor Injection)
```csharp
public class InvoiceService
{
    private readonly INumberToWordsConverter _numberConverter;

    // Dependency được "bơm" vào tự động thông qua Constructor nhờ DI Container
    public InvoiceService(INumberToWordsConverter numberConverter)
    {
        _numberConverter = numberConverter;
    }

    public void PrintInvoice(long totalAmount)
    {
        Console.WriteLine($"--- HÓA ĐƠN SIÊU THỊ ---");
        Console.WriteLine($"Số tiền: {totalAmount:N0} VND");
        
        string amountInWords = _numberConverter.Convert(totalAmount);
        
        Console.WriteLine($"Bằng chữ: {amountInWords} đồng chẵn.");
    }
}
```

### Bước 4: Đăng ký cấu hình và chạy bằng DI Container (`Program.cs`)
```csharp
using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 1. Khởi tạo IoC Container và đăng ký các Service dịch vụ
        var serviceProvider = new ServiceCollection()
            // Đăng ký cặp Interface và Implementation cụ thể
            .AddTransient<INumberToWordsConverter, VietnameseNumberConverter>()
            // Đăng ký dịch vụ sử dụng
            .AddTransient<InvoiceService>()
            .BuildServiceProvider();

        // 2. Lấy Service ra sử dụng (Container tự dùng Reflection phân tích constructor và Inject đối tượng phụ thuộc vào)
        var invoiceService = serviceProvider.GetRequiredService<InvoiceService>();

        // Thực thi kiểm tra kết quả
        invoiceService.PrintInvoice(1250500); 
    }
}
```
