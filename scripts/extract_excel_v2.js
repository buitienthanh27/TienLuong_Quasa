const XLSX = require('xlsx');
const fs = require('fs');
const path = require('path');

const excelPath = path.join(__dirname, '..', 'Document_Quasa', '2025_Quasa', 'Tháng 11', 'Đội 1', 'LƯƠNG ĐỘI 1 THÁNG 11.2025 2  BẢN.xlsx');

console.log('Reading Excel file:', excelPath);
const workbook = XLSX.readFile(excelPath, { cellFormula: true, cellNF: true });

const result = {
  yearMonth: '2025-11',
  extractedAt: new Date().toISOString(),
  trams: [
    { code: 'T1', name: 'Trạm 1', description: 'Vùng khó khăn - Đội 1' },
    { code: 'T2', name: 'Trạm 2', description: 'Vùng bình thường - Đội 1' }
  ],
  employeeTypes: [
    { code: 'CNKT', name: 'Công nhân kỹ thuật', salaryCurrency: 'THB', paymentCurrency: 'LAK', calculationMethod: 'PRODUCTION', hasInsurance: true, sortOrder: 1 },
    { code: 'BV', name: 'Bảo vệ', salaryCurrency: 'LAK', paymentCurrency: 'LAK', calculationMethod: 'FIXED', hasInsurance: false, sortOrder: 2 },
    { code: 'TV', name: 'Tạp vụ', salaryCurrency: 'LAK', paymentCurrency: 'LAK', calculationMethod: 'FIXED', hasInsurance: false, sortOrder: 3 },
    { code: 'CS', name: 'Chăm sóc', salaryCurrency: 'LAK', paymentCurrency: 'LAK', calculationMethod: 'DAILY', hasInsurance: false, sortOrder: 4 }
  ],
  technicalGrades: [
    { grade: 'A', name: 'Hạng A - Xuất sắc', pointCoefficient: 1.00, sortOrder: 1 },
    { grade: 'B', name: 'Hạng B - Khá', pointCoefficient: 0.97, sortOrder: 2 },
    { grade: 'C', name: 'Hạng C - Trung bình', pointCoefficient: 0.93, sortOrder: 3 },
    { grade: 'D', name: 'Hạng D - Yếu', pointCoefficient: 0.90, sortOrder: 4 }
  ],
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

function parseNumber(val) {
  if (val === null || val === undefined || val === '') return 0;
  if (typeof val === 'number') return val;
  const num = parseFloat(String(val).replace(/,/g, ''));
  return isNaN(num) ? 0 : num;
}

// Extract detailed data from "sản lượng" sheet
function extractProductionSheet() {
  const sheetName = 'sản lượng';
  if (!workbook.SheetNames.includes(sheetName)) {
    console.log('Sheet "sản lượng" not found');
    return;
  }

  console.log('\n=== Extracting "sản lượng" sheet ===');
  const sheet = workbook.Sheets[sheetName];
  const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

  console.log('Total rows:', jsonData.length);

  // Print first 30 rows to understand structure
  console.log('\n--- First 30 rows ---');
  for (let i = 0; i < Math.min(30, jsonData.length); i++) {
    const row = jsonData[i];
    const nonEmpty = row.filter(c => c !== '').slice(0, 15);
    if (nonEmpty.length > 0) {
      console.log(`R${i}: ${JSON.stringify(nonEmpty).substring(0, 200)}`);
    }
  }
}

// Extract from TRẠM 1 sheet with full production data
function extractTram1Full() {
  const sheetName = 'TRẠM 1';
  if (!workbook.SheetNames.includes(sheetName)) return;

  console.log('\n=== Extracting TRẠM 1 with full data ===');
  const sheet = workbook.Sheets[sheetName];
  const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

  // Find header row
  let headerRow = -1;
  let colMap = {};

  for (let i = 0; i < Math.min(20, jsonData.length); i++) {
    const row = jsonData[i];
    for (let j = 0; j < row.length; j++) {
      const val = String(row[j]).toUpperCase().trim();
      if (val === 'MSNV' || val.includes('MÃ SỐ')) {
        headerRow = i;
        // Map columns
        for (let k = 0; k < row.length; k++) {
          const colVal = String(row[k]).toUpperCase().trim();
          if (colVal === 'MSNV' || colVal.includes('MÃ SỐ')) colMap.msnv = k;
          if (colVal.includes('TÊN') || colVal.includes('HỌ TÊN')) colMap.name = k;
          if (colVal === 'HẠNG' || colVal.includes('HẠNG KT')) colMap.grade = k;
          if (colVal.includes('MỦ TƯƠI') || colVal.includes('KG TƯƠI')) colMap.rawLatex = k;
          if (colVal === 'DRC' || colVal.includes('TỶ LỆ')) colMap.drc = k;
          if (colVal.includes('QUY KHÔ') || colVal.includes('KG KHÔ')) colMap.dryLatex = k;
          if (colVal.includes('THÀNH TIỀN') || colVal.includes('TIỀN')) colMap.salary = k;
          if (colVal.includes('CÔNG') && !colVal.includes('CHĂM')) colMap.workDays = k;
        }
        break;
      }
    }
    if (headerRow >= 0) break;
  }

  console.log('Header row:', headerRow);
  console.log('Column map:', colMap);

  // Extract employee data
  let count = 0;
  for (let i = headerRow + 1; i < jsonData.length && count < 50; i++) {
    const row = jsonData[i];
    if (!row || row.length < 3) continue;

    // Find MSNV in row
    let msnv = null, name = null, grade = null;
    let rawKg = 0, drc = 0, dryKg = 0, salary = 0, workDays = 0;

    for (let j = 0; j < row.length; j++) {
      const val = row[j];
      if (/^0\d{4}$/.test(String(val))) {
        msnv = String(val);
        // Get name from next column
        if (row[j + 1]) name = String(row[j + 1]).trim();
      }
      if (/^[ABCD]$/i.test(String(val).trim())) {
        grade = String(val).toUpperCase();
      }
    }

    if (msnv && name && name.length > 1) {
      // Get numeric values from known columns or detect them
      const numericCols = [];
      for (let j = 0; j < row.length; j++) {
        const v = row[j];
        if (typeof v === 'number' && v > 0) {
          numericCols.push({ idx: j, val: v });
        }
      }

      // Find likely production values
      for (const nc of numericCols) {
        if (nc.val > 500 && nc.val < 5000) rawKg = rawKg || nc.val;
        else if (nc.val > 0.30 && nc.val < 0.50) drc = drc || nc.val;
        else if (nc.val > 100 && nc.val < 2000) dryKg = dryKg || nc.val;
        else if (nc.val > 1000000) salary = salary || nc.val;
        else if (nc.val > 10 && nc.val < 35) workDays = workDays || nc.val;
      }

      count++;

      // Check if already exists
      if (!result.employees.find(e => e.msnv === msnv)) {
        result.employees.push({
          msnv,
          fullName: name,
          tramCode: 'T1',
          position: 'CNKT',
          technicalGrade: grade || 'A'
        });
      }

      if (!result.productions.find(p => p.employeeMsnv === msnv)) {
        result.productions.push({
          employeeMsnv: msnv,
          yearMonth: '2025-11',
          rawLatexKg: rawKg,
          dryLatexKg: dryKg || (rawKg * 0.3915),
          grade: grade || 'A',
          calculatedSalary: salary
        });
      }

      console.log(`T1 #${count}: ${msnv} | ${name.padEnd(15)} | ${grade || '?'} | Raw:${rawKg} | Dry:${Math.round(dryKg)} | Sal:${Math.round(salary/1000000)}M`);
    }
  }
  console.log(`Total T1: ${count}`);
}

// Extract from TRẠM 2 sheet
function extractTram2Full() {
  const sheetName = 'TRẠM 2';
  if (!workbook.SheetNames.includes(sheetName)) return;

  console.log('\n=== Extracting TRẠM 2 with full data ===');
  const sheet = workbook.Sheets[sheetName];
  const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

  let count = 0;
  for (let i = 0; i < jsonData.length && count < 60; i++) {
    const row = jsonData[i];
    if (!row || row.length < 3) continue;

    let msnv = null, name = null, grade = null;
    let rawKg = 0, dryKg = 0, salary = 0;

    for (let j = 0; j < row.length; j++) {
      const val = row[j];
      if (/^0\d{4}$/.test(String(val))) {
        msnv = String(val);
        if (row[j + 1]) name = String(row[j + 1]).trim();
      }
      if (/^[ABCD]$/i.test(String(val).trim())) {
        grade = String(val).toUpperCase();
      }
    }

    if (msnv && name && name.length > 1) {
      const numericCols = [];
      for (let j = 0; j < row.length; j++) {
        const v = row[j];
        if (typeof v === 'number' && v > 0) numericCols.push({ idx: j, val: v });
      }

      for (const nc of numericCols) {
        if (nc.val > 500 && nc.val < 5000) rawKg = rawKg || nc.val;
        else if (nc.val > 100 && nc.val < 2000) dryKg = dryKg || nc.val;
        else if (nc.val > 1000000) salary = salary || nc.val;
      }

      if (!result.employees.find(e => e.msnv === msnv)) {
        count++;
        result.employees.push({
          msnv,
          fullName: name,
          tramCode: 'T2',
          position: 'CNKT',
          technicalGrade: grade || 'A'
        });

        result.productions.push({
          employeeMsnv: msnv,
          yearMonth: '2025-11',
          rawLatexKg: rawKg,
          dryLatexKg: dryKg || (rawKg * 0.3851),
          grade: grade || 'A',
          calculatedSalary: salary
        });

        console.log(`T2 #${count}: ${msnv} | ${name.padEnd(15)} | ${grade || '?'} | Raw:${rawKg} | Dry:${Math.round(dryKg)}`);
      }
    }
  }
  console.log(`Total T2: ${count}`);
}

// Extract DRC from TRẠM CÁN sheet
function extractDrcFromTramCan() {
  const sheetName = 'TRẠM CÁN';
  if (!workbook.SheetNames.includes(sheetName)) return;

  console.log('\n=== Extracting DRC from TRẠM CÁN ===');
  const sheet = workbook.Sheets[sheetName];
  const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

  // Print first 20 rows
  for (let i = 0; i < Math.min(20, jsonData.length); i++) {
    const row = jsonData[i];
    const nonEmpty = row.filter(c => c !== '').slice(0, 10);
    if (nonEmpty.length > 0) {
      console.log(`R${i}: ${JSON.stringify(nonEmpty).substring(0, 150)}`);
    }
  }

  // Look for DRC values
  for (let i = 0; i < jsonData.length; i++) {
    const row = jsonData[i];
    for (let j = 0; j < row.length; j++) {
      const val = String(row[j]).toUpperCase();
      if (val.includes('TRẠM 1') || val.includes('T1')) {
        // Look for DRC value nearby
        for (let k = j; k < Math.min(j + 10, row.length); k++) {
          const numVal = parseNumber(row[k]);
          if (numVal > 0.30 && numVal < 0.50) {
            console.log(`Found T1 DRC: ${numVal}`);
            if (!result.drcRates.find(d => d.tramCode === 'T1')) {
              result.drcRates.push({ tramCode: 'T1', yearMonth: '2025-11', drcRate: numVal, source: 'TRẠM CÁN' });
            }
          }
        }
      }
      if (val.includes('TRẠM 2') || val.includes('T2')) {
        for (let k = j; k < Math.min(j + 10, row.length); k++) {
          const numVal = parseNumber(row[k]);
          if (numVal > 0.30 && numVal < 0.50) {
            console.log(`Found T2 DRC: ${numVal}`);
            if (!result.drcRates.find(d => d.tramCode === 'T2')) {
              result.drcRates.push({ tramCode: 'T2', yearMonth: '2025-11', drcRate: numVal, source: 'TRẠM CÁN' });
            }
          }
        }
      }
    }
  }
}

// Extract from CHẤM CÔNG sheet
function extractAttendance() {
  const sheetName = 'CHẤM CÔNG ';
  if (!workbook.SheetNames.includes(sheetName)) return;

  console.log('\n=== Extracting Attendance ===');
  const sheet = workbook.Sheets[sheetName];
  const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

  // Print first 20 rows
  for (let i = 0; i < Math.min(20, jsonData.length); i++) {
    const row = jsonData[i];
    const nonEmpty = row.filter(c => c !== '').slice(0, 15);
    if (nonEmpty.length > 0) {
      console.log(`R${i}: ${JSON.stringify(nonEmpty).substring(0, 180)}`);
    }
  }
}

// Extract care adjustments
function extractCareAdjustments() {
  const sheetName = 'CÔNG CHĂM SÓC';
  if (!workbook.SheetNames.includes(sheetName)) return;

  console.log('\n=== Extracting Care Adjustments ===');
  const sheet = workbook.Sheets[sheetName];
  const jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });

  // Print first 25 rows
  for (let i = 0; i < Math.min(25, jsonData.length); i++) {
    const row = jsonData[i];
    const nonEmpty = row.filter(c => c !== '').slice(0, 12);
    if (nonEmpty.length > 0) {
      console.log(`R${i}: ${JSON.stringify(nonEmpty).substring(0, 180)}`);
    }
  }

  // Extract care adjustment data
  for (let i = 0; i < jsonData.length; i++) {
    const row = jsonData[i];
    let msnv = null, name = null, days = 0, amount = 0;

    for (let j = 0; j < row.length; j++) {
      const val = row[j];
      if (/^0\d{4}$/.test(String(val))) {
        msnv = String(val);
        if (row[j + 1]) name = String(row[j + 1]).trim();
      }
      if (typeof val === 'number') {
        if (val > 0 && val < 32) days = days || val;
        else if (val > 100000) amount = amount || val;
      }
    }

    if (msnv && days > 0) {
      result.careAdjustments.push({
        employeeMsnv: msnv,
        yearMonth: '2025-11',
        careDays: days,
        calculatedAmount: amount
      });
    }
  }
  console.log(`Total care adjustments: ${result.careAdjustments.length}`);
}

// Set default values
function setDefaults() {
  // DRC rates
  if (!result.drcRates.find(d => d.tramCode === 'T1')) {
    result.drcRates.push({ tramCode: 'T1', yearMonth: '2025-11', drcRate: 0.3915, source: 'Default' });
  }
  if (!result.drcRates.find(d => d.tramCode === 'T2')) {
    result.drcRates.push({ tramCode: 'T2', yearMonth: '2025-11', drcRate: 0.3851, source: 'Default' });
  }

  // Exchange rate
  result.exchangeRates = [
    { yearMonth: '2025-11', fromCurrency: 'THB', toCurrency: 'LAK', rate: 664.71, source: 'Excel TRẠM 1' }
  ];

  // Unit prices (THB per kg, converted to KIP)
  const thbToKip = 664.71;
  result.rubberUnitPrices = [
    { tramCode: 'T1', grade: 'A', unitPriceKip: Math.round(9.2 * thbToKip), isDifficultArea: true },
    { tramCode: 'T1', grade: 'B', unitPriceKip: Math.round(8.9 * thbToKip), isDifficultArea: true },
    { tramCode: 'T1', grade: 'C', unitPriceKip: Math.round(8.6 * thbToKip), isDifficultArea: true },
    { tramCode: 'T1', grade: 'D', unitPriceKip: Math.round(8.3 * thbToKip), isDifficultArea: true },
    { tramCode: 'T2', grade: 'A', unitPriceKip: Math.round(7.7 * thbToKip), isDifficultArea: false },
    { tramCode: 'T2', grade: 'B', unitPriceKip: Math.round(7.4 * thbToKip), isDifficultArea: false },
    { tramCode: 'T2', grade: 'C', unitPriceKip: Math.round(7.1 * thbToKip), isDifficultArea: false },
    { tramCode: 'T2', grade: 'D', unitPriceKip: Math.round(6.8 * thbToKip), isDifficultArea: false }
  ];

  // Work types
  result.workTypes = [
    { code: 'CHAM_SOC', name: 'Lương chăm sóc', unitPrice: 25000, currency: 'LAK' },
    { code: 'CAO_2_LAT', name: 'Cạo 2 lát', unitPrice: 100000, currency: 'LAK' },
    { code: 'KHOP_NANG', name: 'Công khộp nặng', unitPrice: 20000, currency: 'LAK' },
    { code: 'CAY_NON', name: 'Công cây non', unitPrice: 30700, currency: 'LAK' },
    { code: 'CAO_2_CN', name: 'Cạo 2 chủ nhật', unitPrice: 150000, currency: 'LAK' }
  ];

  // System parameters
  result.systemParameters = [
    { paramCode: 'BHXH_RATE', paramValue: 0.08, description: 'Tỷ lệ BHXH 8%' },
    { paramCode: 'BHYT_RATE', paramValue: 0.015, description: 'Tỷ lệ BHYT 1.5%' },
    { paramCode: 'P7', paramValue: 27.0, description: 'Số ngày công chuẩn' },
    { paramCode: 'DIFFICULT_BONUS', paramValue: 1000, description: 'Phụ cấp vùng khó khăn (KIP/kg)' },
    { paramCode: 'CARE_RATE', paramValue: 25000, description: 'Đơn giá công chăm sóc (KIP/ngày)' }
  ];
}

// Run extraction
console.log('========================================');
console.log('EXCEL DATA EXTRACTION v2');
console.log('========================================');

extractProductionSheet();
extractTram1Full();
extractTram2Full();
extractDrcFromTramCan();
extractAttendance();
extractCareAdjustments();
setDefaults();

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
console.log('Unit Prices:', result.rubberUnitPrices.length);
console.log('Work Types:', result.workTypes.length);
console.log('System Parameters:', result.systemParameters.length);
console.log('Care Adjustments:', result.careAdjustments.length);

// Save JSON
const outputPath = path.join(__dirname, '..', 'API_Sample', 'API_Sample.Data', 'Seed', 'SeedData_Nov2025.json');
fs.writeFileSync(outputPath, JSON.stringify(result, null, 2), 'utf8');
console.log('\nSaved to:', outputPath);

console.log('\n--- Sample Data ---');
console.log('First 5 employees:', result.employees.slice(0, 5).map(e => `${e.msnv}-${e.fullName}`));
console.log('First 5 productions:', result.productions.slice(0, 5).map(p => `${p.employeeMsnv}: ${p.rawLatexKg}kg`));
console.log('DRC Rates:', result.drcRates);
console.log('Exchange Rates:', result.exchangeRates);
