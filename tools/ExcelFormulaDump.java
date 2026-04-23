import java.io.FileInputStream;
import java.io.InputStream;
import java.nio.file.Path;
import java.util.Iterator;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.ss.usermodel.Cell;
import org.apache.poi.ss.usermodel.CellType;
import org.apache.poi.ss.usermodel.DataFormatter;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.ss.usermodel.WorkbookFactory;

public class ExcelFormulaDump {
    public static void main(String[] args) throws Exception {
        if (args.length == 0) {
            System.err.println("Usage: ExcelFormulaDump <file.xls|file.xlsx>");
            System.exit(1);
        }
        dump(Path.of(args[0]));
    }

    private static void dump(Path path) throws Exception {
        try (InputStream in = new FileInputStream(path.toFile());
             Workbook wb = WorkbookFactory.create(in)) {
            DataFormatter fmt = new DataFormatter();
            System.out.println("FILE|" + path);
            System.out.println("SHEETS|" + wb.getNumberOfSheets());
            for (int i = 0; i < wb.getNumberOfSheets(); i++) {
                Sheet sh = wb.getSheetAt(i);
                System.out.println("SHEET|" + sh.getSheetName() + "|rows=" + (sh.getLastRowNum() + 1));
                int formulaCount = 0;
                int shown = 0;
                for (Row row : sh) {
                    for (Cell cell : row) {
                        if (cell.getCellType() == CellType.FORMULA) {
                            formulaCount++;
                            if (shown < 15) {
                                System.out.println("FORMULA|" + sh.getSheetName() + "|" + cell.getAddress() + "|" + cell.getCellFormula());
                                shown++;
                            }
                        }
                    }
                }
                System.out.println("FORMULA_COUNT|" + sh.getSheetName() + "|" + formulaCount);
                int previewRows = 0;
                for (Row row : sh) {
                    if (previewRows >= 5) {
                        break;
                    }
                    StringBuilder sb = new StringBuilder();
                    int nonEmpty = 0;
                    Iterator<Cell> cells = row.cellIterator();
                    while (cells.hasNext()) {
                        Cell c = cells.next();
                        String value = fmt.formatCellValue(c);
                        if (!value.isEmpty()) {
                            if (sb.length() > 0) {
                                sb.append(" || ");
                            }
                            sb.append(c.getAddress()).append('=').append(value);
                            nonEmpty++;
                        }
                    }
                    if (nonEmpty > 0) {
                        System.out.println("ROW|" + sh.getSheetName() + "|" + row.getRowNum() + "|" + sb);
                        previewRows++;
                    }
                }
            }
        }
    }
}
