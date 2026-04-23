using OfficeOpenXml;
using System.Text.Json;
using System.Text.Json.Serialization;

ExcelPackage.License.SetNonCommercialOrganization("Test");

var excelPath = @"D:\EcoTech2A\TLuong_Quasa_EcoTech2A\Document_Quasa\2025_Quasa\Tháng 11\Đội 1\LƯƠNG ĐỘI 1 THÁNG 11.2025 2  BẢN.xlsx";
var outputPath = @"D:\EcoTech2A\TLuong_Quasa_EcoTech2A\API_Sample\API_Sample.Data\Seed\SeedData_Nov2025.json";

using var package = new ExcelPackage(new FileInfo(excelPath));

Console.WriteLine($"=== EXTRACTING DATA FROM: {Path.GetFileName(excelPath)} ===\n");

var seedData = new SeedDataModel
{
    YearMonth = "2025-11",
    ExtractedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
};

// 1. Extract from BẢNG PHÂN TOÁN - contains prices and rates
var phanToanSheet = package.Workbook.Worksheets.FirstOrDefault(w => w.Name == "BẢNG PHÂN TOÁN ");
if (phanToanSheet != null)
{
    Console.WriteLine("=== Extracting from BẢNG PHÂN TOÁN ===");

    // TỶ GIÁ at Row 9, Col J (10)
    var tyGia = GetDecimal(phanToanSheet, 9, 10);
    Console.WriteLine($"  Tỷ giá THB/LAK: {tyGia}");
    seedData.ExchangeRates.Add(new ExchangeRateData
    {
        YearMonth = "2025-11",
        FromCurrency = "THB",
        ToCurrency = "LAK",
        Rate = tyGia,
        Source = "Vietinbank"
    });

    // DRC at Row 10, Col J
    var drc = GetDecimal(phanToanSheet, 10, 10);
    Console.WriteLine($"  DRC: {drc}");
    seedData.SystemParameters.Add(new SystemParameterData { ParamCode = "DRC_TEAM1", ParamValue = drc, Description = "DRC Đội 1 tháng 11/2025" });

    // Extract unit prices from rows 17-25
    // Trạm 1 prices (rows 17-20)
    var priceT1A = GetDecimal(phanToanSheet, 17, 6); // Col F = Đơn giá
    var priceT1B = GetDecimal(phanToanSheet, 18, 6);
    var priceT1C = GetDecimal(phanToanSheet, 19, 6);
    var priceT1D = GetDecimal(phanToanSheet, 20, 6);

    Console.WriteLine($"  Trạm 1 - Hạng A: {priceT1A} KIP");
    Console.WriteLine($"  Trạm 1 - Hạng B: {priceT1B} KIP");
    Console.WriteLine($"  Trạm 1 - Hạng C: {priceT1C} KIP");
    Console.WriteLine($"  Trạm 1 - Hạng D: {priceT1D} KIP");

    // Trạm 2 prices (rows 22-25)
    var priceT2A = GetDecimal(phanToanSheet, 22, 6);
    var priceT2B = GetDecimal(phanToanSheet, 23, 6);
    var priceT2C = GetDecimal(phanToanSheet, 24, 6);
    var priceT2D = GetDecimal(phanToanSheet, 25, 6);

    Console.WriteLine($"  Trạm 2 - Hạng A: {priceT2A} KIP");
    Console.WriteLine($"  Trạm 2 - Hạng B: {priceT2B} KIP");
    Console.WriteLine($"  Trạm 2 - Hạng C: {priceT2C} KIP");
    Console.WriteLine($"  Trạm 2 - Hạng D: {priceT2D} KIP");

    // Add to RubberUnitPrices
    seedData.RubberUnitPrices.AddRange(new[]
    {
        new RubberUnitPriceData { TramCode = "T1", Grade = "A", UnitPriceKip = priceT1A, IsDifficultArea = true },
        new RubberUnitPriceData { TramCode = "T1", Grade = "B", UnitPriceKip = priceT1B, IsDifficultArea = true },
        new RubberUnitPriceData { TramCode = "T1", Grade = "C", UnitPriceKip = priceT1C, IsDifficultArea = true },
        new RubberUnitPriceData { TramCode = "T1", Grade = "D", UnitPriceKip = priceT1D, IsDifficultArea = true },
        new RubberUnitPriceData { TramCode = "T2", Grade = "A", UnitPriceKip = priceT2A, IsDifficultArea = false },
        new RubberUnitPriceData { TramCode = "T2", Grade = "B", UnitPriceKip = priceT2B, IsDifficultArea = false },
        new RubberUnitPriceData { TramCode = "T2", Grade = "C", UnitPriceKip = priceT2C, IsDifficultArea = false },
        new RubberUnitPriceData { TramCode = "T2", Grade = "D", UnitPriceKip = priceT2D, IsDifficultArea = false },
    });

    // Work type rates from rows 27-30
    var pcNgayCong = GetDecimal(phanToanSheet, 28, 6);
    var pcChuNhat = GetDecimal(phanToanSheet, 29, 6);
    var pcCayNon = GetDecimal(phanToanSheet, 30, 6);

    Console.WriteLine($"  PC Ngày công: {pcNgayCong} KIP");
    Console.WriteLine($"  PC Chủ nhật: {pcChuNhat} KIP");
    Console.WriteLine($"  PC Cây non: {pcCayNon} KIP");

    seedData.WorkTypes.AddRange(new[]
    {
        new WorkTypeData { Code = "NGAY_CONG", Name = "Phụ cấp ngày công", UnitPrice = pcNgayCong, Currency = "LAK" },
        new WorkTypeData { Code = "CHU_NHAT", Name = "Phụ cấp ngày chủ nhật", UnitPrice = pcChuNhat, Currency = "LAK" },
        new WorkTypeData { Code = "CAY_NON", Name = "Phụ cấp vườn cây năm nhất", UnitPrice = pcCayNon, Currency = "LAK" },
    });
}

// 2. Extract Employees from TRẠM 1 and TRẠM 2
var tram1Sheet = package.Workbook.Worksheets.FirstOrDefault(w => w.Name == "TRẠM 1");
var tram2Sheet = package.Workbook.Worksheets.FirstOrDefault(w => w.Name == "TRẠM 2");

if (tram1Sheet != null)
{
    Console.WriteLine("\n=== Extracting Employees from TRẠM 1 ===");
    ExtractEmployees(tram1Sheet, "T1", seedData);
}

if (tram2Sheet != null)
{
    Console.WriteLine("\n=== Extracting Employees from TRẠM 2 ===");
    ExtractEmployees(tram2Sheet, "T2", seedData);
}

// 3. Extract BV/TV from other sheets
var luongDoiSheet = package.Workbook.Worksheets.FirstOrDefault(w => w.Name == "LƯƠNG ĐỘI");
if (luongDoiSheet != null)
{
    Console.WriteLine("\n=== Extracting from LƯƠNG ĐỘI (BV/TV) ===");
    ExtractBVTV(luongDoiSheet, seedData);
}

// 4. Add Technical Grades
seedData.TechnicalGrades.AddRange(new[]
{
    new TechnicalGradeData { Grade = "A", Name = "Hạng A - Xuất sắc", PointCoefficient = 1.00m, SortOrder = 1 },
    new TechnicalGradeData { Grade = "B", Name = "Hạng B - Khá", PointCoefficient = 0.97m, SortOrder = 2 },
    new TechnicalGradeData { Grade = "C", Name = "Hạng C - Trung bình", PointCoefficient = 0.93m, SortOrder = 3 },
    new TechnicalGradeData { Grade = "D", Name = "Hạng D - Yếu", PointCoefficient = 0.90m, SortOrder = 4 },
});

// 5. Add Employee Types
seedData.EmployeeTypes.AddRange(new[]
{
    new EmployeeTypeData { Code = "CNKT", Name = "Công nhân kỹ thuật", SalaryCurrency = "LAK", PaymentCurrency = "LAK", CalculationMethod = "PRODUCTION", HasInsurance = true, SortOrder = 1 },
    new EmployeeTypeData { Code = "BV", Name = "Bảo vệ", SalaryCurrency = "LAK", PaymentCurrency = "LAK", CalculationMethod = "FIXED", HasInsurance = false, SortOrder = 2 },
    new EmployeeTypeData { Code = "TV", Name = "Tạp vụ", SalaryCurrency = "LAK", PaymentCurrency = "LAK", CalculationMethod = "FIXED", HasInsurance = false, SortOrder = 3 },
    new EmployeeTypeData { Code = "CB", Name = "Cán bộ", SalaryCurrency = "LAK", PaymentCurrency = "LAK", CalculationMethod = "FIXED", HasInsurance = true, SortOrder = 4 },
    new EmployeeTypeData { Code = "CS", Name = "Chăm sóc", SalaryCurrency = "LAK", PaymentCurrency = "LAK", CalculationMethod = "DAILY", HasInsurance = false, SortOrder = 5 },
});

// 6. Add Tram
seedData.Trams.AddRange(new[]
{
    new TramData { Code = "T1", Name = "Trạm 1", Description = "Trạm 1 - Diện tích khộp nặng (vùng khó khăn)" },
    new TramData { Code = "T2", Name = "Trạm 2", Description = "Trạm 2" },
});

// Print summary
Console.WriteLine("\n=== SUMMARY ===");
Console.WriteLine($"  Employees: {seedData.Employees.Count}");
Console.WriteLine($"  Productions: {seedData.Productions.Count}");
Console.WriteLine($"  Rubber Unit Prices: {seedData.RubberUnitPrices.Count}");
Console.WriteLine($"  Exchange Rates: {seedData.ExchangeRates.Count}");
Console.WriteLine($"  Work Types: {seedData.WorkTypes.Count}");
Console.WriteLine($"  Technical Grades: {seedData.TechnicalGrades.Count}");
Console.WriteLine($"  Employee Types: {seedData.EmployeeTypes.Count}");
Console.WriteLine($"  System Parameters: {seedData.SystemParameters.Count}");

// Save to JSON
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};
var json = JsonSerializer.Serialize(seedData, options);
File.WriteAllText(outputPath, json, System.Text.Encoding.UTF8);
Console.WriteLine($"\n=== Saved to: {outputPath} ===");

// ======== HELPER FUNCTIONS ========

decimal GetDecimal(ExcelWorksheet ws, int row, int col)
{
    var val = ws.Cells[row, col].Value;
    if (val == null) return 0;
    if (decimal.TryParse(val.ToString(), out var d)) return d;
    return 0;
}

string GetString(ExcelWorksheet ws, int row, int col)
{
    return ws.Cells[row, col].Value?.ToString()?.Trim() ?? "";
}

void ExtractEmployees(ExcelWorksheet ws, string tramCode, SeedDataModel data)
{
    // Header at row 10, data starts at row 15
    int dataStartRow = 15;
    int maxRow = ws.Dimension?.Rows ?? 0;
    int count = 0;

    for (int row = dataStartRow; row <= maxRow; row++)
    {
        var stt = GetString(ws, row, 1);
        var msnv = GetString(ws, row, 2);
        var name = GetString(ws, row, 3);
        var position = GetString(ws, row, 4);
        var muTap = GetDecimal(ws, row, 8);     // Mủ tạp (Kg)
        var muXirum = GetDecimal(ws, row, 9);   // Mủ xirum (Kg)
        var muQuyKho = GetDecimal(ws, row, 10); // Mủ quy khô
        var grade = GetString(ws, row, 14);     // Hạng kỹ thuật
        var salary = GetDecimal(ws, row, 15);   // Thành tiền

        // Skip empty rows or summary rows
        if (string.IsNullOrEmpty(name) || name.Contains("CỘNG") || name.Contains("Cộng"))
            continue;
        if (!int.TryParse(stt, out _) && string.IsNullOrEmpty(msnv))
            continue;

        // Clean up MSNV
        if (msnv == "#N/A" || string.IsNullOrEmpty(msnv))
            msnv = $"NEW_{tramCode}_{row}";

        // Clean up grade
        grade = grade?.Trim().ToUpper() ?? "A";
        if (!new[] { "A", "B", "C", "D" }.Contains(grade))
            grade = "A";

        count++;
        data.Employees.Add(new EmployeeData
        {
            Msnv = msnv,
            FullName = name,
            TramCode = tramCode,
            Position = string.IsNullOrEmpty(position) ? "CNKT" : position,
            TechnicalGrade = grade
        });

        if (muQuyKho > 0 || muTap > 0)
        {
            data.Productions.Add(new ProductionData
            {
                EmployeeMsnv = msnv,
                YearMonth = "2025-11",
                RawLatexKg = muTap,
                DryLatexKg = muQuyKho,
                Grade = grade,
                CalculatedSalary = salary
            });
        }
    }
    Console.WriteLine($"  Extracted {count} employees from {tramCode}");
}

void ExtractBVTV(ExcelWorksheet ws, SeedDataModel data)
{
    // Find BV/TV section - look for patterns
    int maxRow = ws.Dimension?.Rows ?? 0;
    int maxCol = ws.Dimension?.Columns ?? 0;

    Console.WriteLine($"  Sheet dimension: {ws.Dimension?.Address}");

    // Print first 30 rows for analysis
    for (int row = 1; row <= Math.Min(40, maxRow); row++)
    {
        var cells = new List<string>();
        for (int col = 1; col <= Math.Min(10, maxCol); col++)
        {
            var val = GetString(ws, row, col);
            if (!string.IsNullOrEmpty(val))
                cells.Add(val);
        }
        if (cells.Any())
        {
            var line = string.Join(" | ", cells);
            if (line.Contains("BV") || line.Contains("TV") || line.Contains("Bảo vệ") || line.Contains("Tạp vụ") ||
                line.Contains("MSNV") || line.Contains("STT"))
            {
                Console.WriteLine($"    Row {row}: {line}");
            }
        }
    }
}

// ======== DATA MODELS ========

class SeedDataModel
{
    public string YearMonth { get; set; } = "";
    public string ExtractedAt { get; set; } = "";
    public List<TramData> Trams { get; set; } = new();
    public List<EmployeeTypeData> EmployeeTypes { get; set; } = new();
    public List<TechnicalGradeData> TechnicalGrades { get; set; } = new();
    public List<EmployeeData> Employees { get; set; } = new();
    public List<ProductionData> Productions { get; set; } = new();
    public List<RubberUnitPriceData> RubberUnitPrices { get; set; } = new();
    public List<ExchangeRateData> ExchangeRates { get; set; } = new();
    public List<WorkTypeData> WorkTypes { get; set; } = new();
    public List<SystemParameterData> SystemParameters { get; set; } = new();
}

class TramData
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
}

class EmployeeTypeData
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string SalaryCurrency { get; set; } = "LAK";
    public string PaymentCurrency { get; set; } = "LAK";
    public string CalculationMethod { get; set; } = "PRODUCTION";
    public bool HasInsurance { get; set; }
    public int SortOrder { get; set; }
}

class TechnicalGradeData
{
    public string Grade { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal PointCoefficient { get; set; }
    public int SortOrder { get; set; }
}

class EmployeeData
{
    public string Msnv { get; set; } = "";
    public string FullName { get; set; } = "";
    public string TramCode { get; set; } = "";
    public string Position { get; set; } = "";
    public string TechnicalGrade { get; set; } = "";
}

class ProductionData
{
    public string EmployeeMsnv { get; set; } = "";
    public string YearMonth { get; set; } = "";
    public decimal RawLatexKg { get; set; }
    public decimal DryLatexKg { get; set; }
    public string Grade { get; set; } = "";
    public decimal CalculatedSalary { get; set; }
}

class RubberUnitPriceData
{
    public string TramCode { get; set; } = "";
    public string Grade { get; set; } = "";
    public decimal UnitPriceKip { get; set; }
    public bool IsDifficultArea { get; set; }
}

class ExchangeRateData
{
    public string YearMonth { get; set; } = "";
    public string FromCurrency { get; set; } = "";
    public string ToCurrency { get; set; } = "";
    public decimal Rate { get; set; }
    public string Source { get; set; } = "";
}

class WorkTypeData
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "";
}

class SystemParameterData
{
    public string ParamCode { get; set; } = "";
    public decimal ParamValue { get; set; }
    public string Description { get; set; } = "";
}
