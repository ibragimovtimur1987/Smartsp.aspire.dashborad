using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartsp.aspire.AtlasLoad
{
    public class ExcelLoader
    {
        public List<string> Categories { get; set; }
        public List<Indicator> Indicators { get; set; }

        public List<Region> Regions { get; set; }

        public List<KeyValuePair<ValueKey, float>> Values { get; set; }
        public static string SourceValue = "AtlasLoad";
        public void Load(string filePath)
        {
            Categories = new List<string>();
            Indicators = new List<Indicator>();
            Regions = new List<Region>();
            Values = new List<KeyValuePair<ValueKey, float>>();

            Workbook book = new Workbook(filePath);
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;

            string currentCategory = string.Empty;
            int rowCategory = 1;
            int rowIndocator = 2;
            int startIndicatorColumn = 5;

            for (int i = 0; i < cells.MaxDataColumn; i++)
            {
                var cell = cells.GetCell(rowCategory, startIndicatorColumn + i);
                if (null != cell && !string.IsNullOrEmpty(cell.StringValue))
                {
                    currentCategory = cell.StringValue.Trim();
                    Categories.Add(cell.StringValue.Trim());
                }

                var cell2 = cells.GetCell(rowIndocator, startIndicatorColumn + i);
                if (null != cell2 && (cell2.StringValue)!=null)
                {
                    if (cell2.StringValue == "") Indicators.Add(new Indicator() { Category = currentCategory, Name = currentCategory, Column = startIndicatorColumn + i });
                    else Indicators.Add(new Indicator() { Category = currentCategory, Name = cell2.StringValue.Trim(), Column = startIndicatorColumn + i });
                }
            }

            int regionColumn = 1;
            int codeColumn = 2;
            int startRow = 3;
            for (int i = 0; i <= cells.MaxDataRow//85
                ; i++)
            {
                var cell = cells.GetCell(startRow + i, regionColumn);
                var cell2 = cells.GetCell(startRow + i, codeColumn);
                if (null != cell && !string.IsNullOrEmpty(cell.StringValue)
                    //&& !Regions.Any(x => x.Name == cell.StringValue)
                    )
                {
                    Regions.Add(new Region() { Name = cell.StringValue.Trim(), Code = null != cell2 ? cell2.StringValue.Trim() : string.Empty, Row = startRow + i });
                }
            }

            int dataYearColumn = 3;
            int surveyYearColumn = 4;
            foreach (var region in Regions)
            {
                var cell2 = cells.GetCell(region.Row, dataYearColumn);
                var cell3 = cells.GetCell(region.Row, surveyYearColumn);
                if (null == cell2 || null == cell3 || CellValueType.IsNumeric != cell2.Type
                    || CellValueType.IsNumeric != cell3.Type)
                {
                    continue;
                }

                foreach (var ind in Indicators)
                {
                    var cell = cells.GetCell(region.Row, ind.Column);
                    if (null != cell && cell.Type == CellValueType.IsNumeric)
                    {
                        if (ind.Category == ind.Name)
                        {
                            Values.Add(new KeyValuePair<ValueKey, float>(
                               new ValueKey() { Region = region, MCP = ind, DataYear = cell2.IntValue, SurveyYear = cell3.IntValue, Source = SourceValue },
                               cell.FloatValue ));
                        }
                        else
                        {
                            int koef = 100;
                            if (ind.Name.ToLower().Contains("джини"))
                            {
                                koef = 1;
                            }
                            Values.Add(new KeyValuePair<ValueKey, float>(
                                new ValueKey() { Region = region, MCP = ind, DataYear = cell2.IntValue, SurveyYear = cell3.IntValue, Source = SourceValue },
                                cell.FloatValue * koef));
                        }
                    }
                }
            }
        }
    }
}
