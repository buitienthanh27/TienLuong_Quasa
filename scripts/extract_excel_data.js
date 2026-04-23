const XLSX = require('xlsx');
const fs = require('fs');
const path = require('path');

const excelPath = path.join(__dirname, '..', 'Document_Quasa', '2025_Quasa', 'Tháng 11', 'Đội 1', 'LƯƠNG ĐỘI 1 THÁNG 11.2025 2  BẢN.xlsx');

console.log('Reading Excel file:', excelPath);

const workbook = XLSX.readFile(excelPath, { cellFormula: true, cellNF: true });
console.log('Sheet names:', workbook.SheetNames);

const result = {
  yearMonth: '2025-11',
  extractedAt: new Date().toISOString(),
  trams: [],
  employeeTypes: [],
  technicalGrades: [],
  employees: [],
  productions: [],
  attendances: [],
  drcRates: [],
  rubberUnitPrices: [],
  exchangeRates: [],
  workTypes: [],
  systemParameters: [],
  careAdjustments: []
};

// Seed Trams
result.trams = [
  { code: 'T1', name: 'Trạm 1', description: 'Vùng khó khăn - Đội 1' },
  { code: 'T2', name: 'Trạm 2', description: 'Vùng bình thường - Đội 1' }
];

// Seed Employee Types
result.employeeTypes = [
  { code: 'CNKT', name: 'Công nhân kỹ thuật', salaryCurrency: 'THB', paymentCurrency: 'LAK', calculationMethod: 'PRODUCTION', hasInsurance: true, sortOrder: 1 },
  { code: 'BV', name: 'Bảo vệ', salaryCurrency: 'LAK', paymentCurrency: 'LAK', calculationMethod: 'FIXED', hasInsurance: false, sortOrder: 2 },
  { code: 'TV', name: 'Tạp vụ', salaryCurrency: 'LAK', paymentCurrency: 'LAK', calculationMethod: 'FIXED', hasInsurance: false, sortOrder: 3 },
  { code: 'CS', name: 'Chăm sóc', salaryCurrency: 'LAK', paymentCurrency: 'LAK', calculationMethod: 'DAILY', hasInsurance: false, sortOrder: 4 }
];

// Seed Technical Grades
result.technicalGrades = [
  { grade: 'A', name: 'Hạng A - Xuất sắc', pointCoefficient: 1.00, sortOrder: 1 },
  { grade: 'B', name: 'Hạng B - Khá', pointCoefficient: 0.97, sortOrder: 2 },
  { grade: 'C', name: 'Hạng C - Trung bình', pointCoefficient: 0.93, sortOrder: 3 },
  { grade: 'D', name: 'Hạng D - Yếu', pointCoefficient: 0.90, sortOrder: 4 }
];

// Helper function to safely get cell value
function getCellValue(sheet, cellRef) {
  const cell = sheet[cellRef];
  if (!cell) return null;
  return cell.v !== undefined ? cell.v : null;
}

// Helper function to parse number
function parseNumber(val) {
  if (val === null || val === undefined || val === '') return 0;
  if (typeof val === 'number') return val;
  const num = parseFloat(String(val).replace(/,/g, ''));
  return isNaN(num) ? 0 : num;
}

// Extract TRẠM 1 data from "TRẠM 1" sheet
function extractTram1Data() {
  const sheetName = 'TRẠM 1';
  if (!workbook.SheetNames.includes(sheetName)) {
    console.log('Sheet not found:', sheetName);
    return;
  }

  const sheet = workbook.Sheets[sheetName];
  const range = XLSX.utils.decode_range(sheet['!ref']);

  console.log(`\n=== Extracting ${sheetName} ===`);
  console.log('Range:', sheet['!ref']);

  // Find header row and data start
  // Typically: STT | MSNV | HỌ TÊN | ...
  let dataStartRow = -1;
  let msnvCol = -1, nameCol = -1, gradeCol = -1;
  let rawLatexCol = -1, drcCol = -1, dryLatexCol = -1;

  // Scan first 20 rows to find headers
  for (let r = 0; r <= Math.min(20, range.e.r); r++) {
    for (let c = 0; c <= range.e.c; c++) {
      const cellRef = XLSX.utils.encode_cell({r, c});
      const val = getCellValue(sheet, cellRef);
      if (val === null) continue;
      const strVal = String(val).toUpperCase().trim();

      if (strVal === 'MSNV' || strVal === 'MÃ SỐ' || strVal.includes('MSNV')) {
        msnvCol = c;
        dataStartRow = r + 1;
      }
      if (strVal === 'HỌ TÊN' || strVal === 'TÊN' || strVal.includes('HỌ VÀ TÊN')) {
        nameCol = c;
      }
      if (strVal === 'HẠNG' || strVal.includes('HẠNG KT')) {
        gradeCol = c;
      }
      if (strVal.includes('MỦ TƯƠI') || strVal.includes('MỦ TẠP') || strVal === 'KG TƯƠI') {
        rawLatexCol = c;
      }
      if (strVal === 'DRC' || strVal.includes('TỈ LỆ')) {
        drcCol = c;
      }
      if (strVal.includes('QUY KHÔ') || strVal.includes('KG KHÔ')) {
        dryLatexCol = c;
      }
    }
  }

  console.log('Found columns - MSNV:', msnvCol, 'Name:', nameCol, 'Grade:', gradeCol);
  console.log('RawLatex:', rawLatexCol, 'DRC:', drcCol, 'DryLatex:', dryLatexCol);
  console.log('Data start row:', dataStartRow);

  // Read all data as JSON to inspect
  const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

  // Print first 50 rows for inspection
  console.log('\n--- First 50 rows preview ---');
  for (let i = 0; i < Math.min(50, jsonData.length); i++) {
    const row = jsonData[i];
    const nonEmpty = row.filter(c => c !== '').slice(0, 15);
    if (nonEmpty.length > 0) {
      console.log(`Row ${i}: ${JSON.stringify(nonEmpty)}`);
    }
  }

  // Find employee data rows
  let employeeCount = 0;
  for (let i = dataStartRow; i < jsonData.length && employeeCount < 100; i++) {
    const row = jsonData[i];
    if (!row || row.length < 3) continue;

    // Look for MSNV pattern (5 digits starting with 0)
    let msnv = null, name = null, grade = null;
    let rawKg = 0, drc = 0, dryKg = 0;

    for (let j = 0; j < row.length; j++) {
      const val = row[j];
      if (val === '' || val === null || val === undefined) continue;

      // Detect MSNV (5-digit number)
      if (/^0\d{4}$/.test(String(val))) {
        msnv = String(val);
        // Next non-empty cell should be name
        for (let k = j + 1; k < row.length; k++) {
          if (row[k] && String(row[k]).trim().length > 1) {
            name = String(row[k]).trim();
            break;
          }
        }
      }

      // Detect grade (single letter A/B/C/D)
      if (/^[ABCD]$/i.test(String(val).trim())) {
        grade = String(val).toUpperCase();
      }
    }

    if (msnv && name) {
      employeeCount++;
      result.employees.push({
        msnv: msnv,
        fullName: name,
        tramCode: 'T1',
        position: 'CNKT',
        technicalGrade: grade || 'A'
      });

      // Try to extract production data
      // Look for numeric values in specific columns
      const numericValues = row.filter(v => typeof v === 'number' && v > 0);
      if (numericValues.length >= 2) {
        // Assume largest number is raw latex, look for DRC around 30-45%
        const possibleRaw = numericValues.find(v => v > 100 && v < 5000);
        const possibleDrc = numericValues.find(v => v > 0.30 && v < 0.50);
        const possibleDry = numericValues.find(v => v > 50 && v < 2000 && v !== possibleRaw);

        if (possibleRaw || possibleDry) {
          result.productions.push({
            employeeMsnv: msnv,
            yearMonth: '2025-11',
            rawLatexKg: possibleRaw || 0,
            dryLatexKg: possibleDry || (possibleRaw && possibleDrc ? Math.floor(possibleRaw * possibleDrc * 100) / 100 : 0),
            grade: grade || 'A',
            calculatedSalary: 0
          });
        }
      }

      console.log(`Employee ${employeeCount}: ${msnv} - ${name} - ${grade || '?'}`);
    }
  }

  console.log(`\nTotal T1 employees found: ${employeeCount}`);
}

// Extract TRẠM 2 data
function extractTram2Data() {
  const sheetName = 'TRẠM 2';
  if (!workbook.SheetNames.includes(sheetName)) {
    console.log('Sheet not found:', sheetName);
    return;
  }

  const sheet = workbook.Sheets[sheetName];
  const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

  console.log(`\n=== Extracting ${sheetName} ===`);

  let employeeCount = 0;
  for (let i = 0; i < jsonData.length && employeeCount < 100; i++) {
    const row = jsonData[i];
    if (!row || row.length < 3) continue;

    let msnv = null, name = null, grade = null;

    for (let j = 0; j < row.length; j++) {
      const val = row[j];
      if (val === '' || val === null || val === undefined) continue;

      // Detect MSNV (5-digit number starting with 0)
      if (/^0\d{4}$/.test(String(val))) {
        msnv = String(val);
        for (let k = j + 1; k < row.length; k++) {
          if (row[k] && String(row[k]).trim().length > 1) {
            name = String(row[k]).trim();
            break;
          }
        }
      }

      if (/^[ABCD]$/i.test(String(val).trim())) {
        grade = String(val).toUpperCase();
      }
    }

    if (msnv && name) {
      // Check not already added
      if (!result.employees.find(e => e.msnv === msnv)) {
        employeeCount++;
        result.employees.push({
          msnv: msnv,
          fullName: name,
          tramCode: 'T2',
          position: 'CNKT',
          technicalGrade: grade || 'A'
        });

        const numericValues = row.filter(v => typeof v === 'number' && v > 0);
        if (numericValues.length >= 2) {
          const possibleRaw = numericValues.find(v => v > 100 && v < 5000);
          const possibleDrc = numericValues.find(v => v > 0.30 && v < 0.50);
          const possibleDry = numericValues.find(v => v > 50 && v < 2000 && v !== possibleRaw);

          if (possibleRaw || possibleDry) {
            result.productions.push({
              employeeMsnv: msnv,
              yearMonth: '2025-11',
              rawLatexKg: possibleRaw || 0,
              dryLatexKg: possibleDry || 0,
              grade: grade || 'A',
              calculatedSalary: 0
            });
          }
        }

        console.log(`Employee ${employeeCount}: ${msnv} - ${name} - ${grade || '?'}`);
      }
    }
  }

  console.log(`\nTotal T2 employees found: ${employeeCount}`);
}

// Extract DRC rates
function extractDrcRates() {
  // Look in multiple sheets for DRC values
  const sheetsToCheck = ['TRẠM 1', 'TRẠM 2', 'TỈ LỆ', 'DRC', 'TỔNG HỢP'];

  for (const sheetName of sheetsToCheck) {
    if (!workbook.SheetNames.includes(sheetName)) continue;

    const sheet = workbook.Sheets[sheetName];
    const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

    for (let i = 0; i < jsonData.length; i++) {
      const row = jsonData[i];
      for (let j = 0; j < row.length; j++) {
        const val = row[j];
        const strVal = String(val).toUpperCase();

        // Look for DRC patterns
        if (strVal.includes('DRC') || strVal.includes('TỈ LỆ')) {
          // Check next cells for numeric value
          for (let k = j + 1; k < Math.min(j + 5, row.length); k++) {
            const numVal = parseNumber(row[k]);
            if (numVal > 0.30 && numVal < 0.50) {
              console.log(`Found DRC in ${sheetName}: ${numVal}`);
              // Determine which tram based on sheet
              const tramCode = sheetName.includes('2') ? 'T2' : 'T1';
              if (!result.drcRates.find(d => d.tramCode === tramCode)) {
                result.drcRates.push({
                  tramCode: tramCode,
                  yearMonth: '2025-11',
                  drcRate: numVal,
                  source: `Excel sheet ${sheetName}`
                });
              }
            }
          }
        }
      }
    }
  }

  // Default DRC if not found
  if (!result.drcRates.find(d => d.tramCode === 'T1')) {
    result.drcRates.push({ tramCode: 'T1', yearMonth: '2025-11', drcRate: 0.3915, source: 'Default' });
  }
  if (!result.drcRates.find(d => d.tramCode === 'T2')) {
    result.drcRates.push({ tramCode: 'T2', yearMonth: '2025-11', drcRate: 0.3851, source: 'Default' });
  }
}

// Extract exchange rates
function extractExchangeRates() {
  // Look for THB rate
  for (const sheetName of workbook.SheetNames) {
    const sheet = workbook.Sheets[sheetName];
    const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

    for (let i = 0; i < Math.min(30, jsonData.length); i++) {
      const row = jsonData[i];
      for (let j = 0; j < row.length; j++) {
        const val = row[j];
        const strVal = String(val).toUpperCase();

        if (strVal.includes('TỈ GIÁ') || strVal.includes('TỶ GIÁ') || strVal.includes('THB')) {
          for (let k = j; k < Math.min(j + 5, row.length); k++) {
            const numVal = parseNumber(row[k]);
            if (numVal > 500 && numVal < 1000) {
              console.log(`Found exchange rate in ${sheetName}: ${numVal}`);
              if (!result.exchangeRates.find(e => e.fromCurrency === 'THB')) {
                result.exchangeRates.push({
                  yearMonth: '2025-11',
                  fromCurrency: 'THB',
                  toCurrency: 'LAK',
                  rate: numVal,
                  source: `Excel sheet ${sheetName}`
                });
              }
            }
          }
        }
      }
    }
  }

  // Default exchange rate if not found
  if (!result.exchangeRates.find(e => e.fromCurrency === 'THB')) {
    result.exchangeRates.push({ yearMonth: '2025-11', fromCurrency: 'THB', toCurrency: 'LAK', rate: 640, source: 'Default' });
  }
}

// Extract unit prices from sheets
function extractUnitPrices() {
  // Default unit prices based on grade
  result.rubberUnitPrices = [
    { tramCode: 'T1', grade: 'A', unitPriceKip: 6115.33, isDifficultArea: true },
    { tramCode: 'T1', grade: 'B', unitPriceKip: 5931.93, isDifficultArea: true },
    { tramCode: 'T1', grade: 'C', unitPriceKip: 5748.50, isDifficultArea: true },
    { tramCode: 'T1', grade: 'D', unitPriceKip: 5565.10, isDifficultArea: true },
    { tramCode: 'T2', grade: 'A', unitPriceKip: 5115.33, isDifficultArea: false },
    { tramCode: 'T2', grade: 'B', unitPriceKip: 4931.93, isDifficultArea: false },
    { tramCode: 'T2', grade: 'C', unitPriceKip: 4748.50, isDifficultArea: false },
    { tramCode: 'T2', grade: 'D', unitPriceKip: 4565.10, isDifficultArea: false }
  ];
}

// Extract work types (allowances)
function extractWorkTypes() {
  result.workTypes = [
    { code: 'CHAM_SOC', name: 'Lương chăm sóc', unitPrice: 25000, currency: 'LAK' },
    { code: 'CAO_2_LAT', name: 'Cạo 2 lát', unitPrice: 100000, currency: 'LAK' },
    { code: 'KHOP_NANG', name: 'Công khộp nặng', unitPrice: 20000, currency: 'LAK' },
    { code: 'CAY_NON', name: 'Công cây non', unitPrice: 30700, currency: 'LAK' },
    { code: 'CAO_2_CN', name: 'Cạo 2 chủ nhật', unitPrice: 150000, currency: 'LAK' }
  ];
}

// Extract system parameters
function extractSystemParameters() {
  result.systemParameters = [
    { paramCode: 'BHXH_RATE', paramValue: 0.08, description: 'Tỷ lệ BHXH' },
    { paramCode: 'BHYT_RATE', paramValue: 0.015, description: 'Tỷ lệ BHYT' },
    { paramCode: 'P7', paramValue: 27.0, description: 'Hệ số P7' },
    { paramCode: 'TAX_TH_1', paramValue: 5000000, description: 'Ngưỡng thuế bậc 1' },
    { paramCode: 'TAX_RATE_1', paramValue: 0.05, description: 'Thuế suất bậc 1' }
  ];
}

// Detailed sheet analysis
function analyzeSheets() {
  console.log('\n=== DETAILED SHEET ANALYSIS ===\n');

  for (const sheetName of workbook.SheetNames) {
    const sheet = workbook.Sheets[sheetName];
    if (!sheet['!ref']) continue;

    const range = XLSX.utils.decode_range(sheet['!ref']);
    const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

    console.log(`\n--- Sheet: ${sheetName} ---`);
    console.log(`Rows: ${range.e.r + 1}, Cols: ${range.e.c + 1}`);

    // Print first 10 rows
    for (let i = 0; i < Math.min(15, jsonData.length); i++) {
      const row = jsonData[i];
      const nonEmpty = row.filter(c => c !== '').slice(0, 12);
      if (nonEmpty.length > 0) {
        console.log(`  R${i}: ${JSON.stringify(nonEmpty).substring(0, 150)}`);
      }
    }
  }
}

// Run extraction
console.log('\n========================================');
console.log('STARTING DATA EXTRACTION');
console.log('========================================\n');

analyzeSheets();

extractTram1Data();
extractTram2Data();
extractDrcRates();
extractExchangeRates();
extractUnitPrices();
extractWorkTypes();
extractSystemParameters();

// Summary
console.log('\n========================================');
console.log('EXTRACTION SUMMARY');
console.log('========================================');
console.log('Trams:', result.trams.length);
console.log('Employee Types:', result.employeeTypes.length);
console.log('Technical Grades:', result.technicalGrades.length);
console.log('Employees:', result.employees.length);
console.log('Productions:', result.productions.length);
console.log('DRC Rates:', result.drcRates.length);
console.log('Exchange Rates:', result.exchangeRates.length);
console.log('Rubber Unit Prices:', result.rubberUnitPrices.length);
console.log('Work Types:', result.workTypes.length);
console.log('System Parameters:', result.systemParameters.length);

// Save to JSON
const outputPath = path.join(__dirname, '..', 'API_Sample', 'API_Sample.Data', 'Seed', 'SeedData_Nov2025_Full.json');
fs.writeFileSync(outputPath, JSON.stringify(result, null, 2), 'utf8');
console.log('\nSaved to:', outputPath);

// Also save a readable version
const readablePath = path.join(__dirname, 'extracted_data_report.txt');
let report = 'EXTRACTED DATA REPORT\n';
report += '=====================\n\n';
report += `Extracted: ${result.extractedAt}\n`;
report += `Year/Month: ${result.yearMonth}\n\n`;

report += '=== EMPLOYEES ===\n';
for (const emp of result.employees) {
  report += `${emp.msnv} | ${emp.fullName.padEnd(20)} | ${emp.tramCode} | ${emp.position} | ${emp.technicalGrade}\n`;
}

report += '\n=== PRODUCTIONS ===\n';
for (const prod of result.productions) {
  report += `${prod.employeeMsnv} | Raw: ${prod.rawLatexKg}kg | Dry: ${prod.dryLatexKg}kg | Grade: ${prod.grade}\n`;
}

report += '\n=== DRC RATES ===\n';
for (const drc of result.drcRates) {
  report += `${drc.tramCode}: ${drc.drcRate} (${drc.source})\n`;
}

report += '\n=== EXCHANGE RATES ===\n';
for (const rate of result.exchangeRates) {
  report += `${rate.fromCurrency} -> ${rate.toCurrency}: ${rate.rate}\n`;
}

fs.writeFileSync(readablePath, report, 'utf8');
console.log('Report saved to:', readablePath);
