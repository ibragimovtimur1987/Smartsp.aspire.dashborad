using Aspose.Cells;
using Smartsp.aspire.TemplateLoad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateLoad
{
    public class ExcelLoader
    {
        public List<string> Categories { get; set; }
        public List<Indicator> Indicators { get; set; }
        public List<KeyValuePair<Key, ValueKey>> Values { get; set; }
        public static string SourceValue = "TemplateLoad";
        public void Load(string filePath)
        {
            Categories = new List<string>();
            Indicators = new List<Indicator>();
            Values = new List<KeyValuePair<Key, ValueKey>>();

            Workbook book = new Workbook(filePath);
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;

            int startRow = 3;
            int yearColumn = 1;
            int regionColumn = 0;
            int areaColumn = 3;
            int categoryColumn = 5;
            int subCategoryColumn = 6;
            int indicatorColumn = 7;
            int SourceFundingIndex = 12;
            int TargetingCategoricalIndex = 18;
            int TargetingMeansTestedIndex = 19;
            int countRecipients = 20;
            int costFinancing = 24;
            for (int i = startRow; i <= cells.MaxDataRow; i++ )
            {
               
                var valueKey = new ValueKey { Region = new Region(), MCP = new Indicator(),Source  = SourceValue };
                 var indicatorCell = cells.GetCell(i, indicatorColumn);

                if (null != indicatorCell && !string.IsNullOrEmpty(indicatorCell.StringValue))
                {
                    var Key = new Key { Column = indicatorCell.Column, Row = indicatorCell.Row };
                    var item = new Indicator()  { Name = indicatorCell.StringValue.Trim(), SourceFunding = string.Empty };
                    Indicators.Add(item);

                    var yearCell = cells.GetCell(i, yearColumn);
                    if (null != yearCell && !string.IsNullOrEmpty(yearCell.StringValue))
                    {
                        valueKey.DataYear = Convert.ToInt32(yearCell.StringValue.Trim());
                    }

                    var regionCell = cells.GetCell(i, regionColumn);
                    if (null != regionCell && !string.IsNullOrEmpty(regionCell.StringValue))
                    {
                        item.Region = regionCell.StringValue.Trim();
                        valueKey.Region.Name = regionCell.StringValue.Trim();

                    }

                    var areaCell = cells.GetCell(i, areaColumn);
                    if (null != areaCell && !string.IsNullOrEmpty(areaCell.StringValue))
                    {                        
                        item.Area = areaCell.StringValue.Trim();
                    }

                    var categoryCell = cells.GetCell(i, categoryColumn);
                    if (null != categoryCell && !string.IsNullOrEmpty(categoryCell.StringValue))
                    {
                        item.Category = categoryCell.StringValue.Trim();
                    }

                    var subCategoryCell = cells.GetCell(i, subCategoryColumn);
                    if (null != subCategoryCell && !string.IsNullOrEmpty(subCategoryCell.StringValue))
                    {
                        item.SubCategory = subCategoryCell.StringValue.Trim();
                    }

                    var SourceFundingCell = cells.GetCell(i, SourceFundingIndex);
                    if (null != SourceFundingCell && !string.IsNullOrEmpty(SourceFundingCell.StringValue))
                    {
                        item.SourceFunding = SourceFundingCell.StringValue.Trim();
                    }

                    var countRecipientsCell = cells.GetCell(i, countRecipients);
                    if (null != countRecipientsCell && !string.IsNullOrEmpty(countRecipientsCell.StringValue))
                    {
                        float resRecipients = new float();
                        if (float.TryParse(countRecipientsCell.StringValue.Trim(),out resRecipients)) item.CountRecipients = resRecipients;
                        else item.CountRecipients = 0;
                        // valueKey.MCP.CountRecipients = countRecipientsCell.StringValue.Trim();
                    }
                    else
                    {
                        item.CountRecipients = 0;
                    }

                    var costFinancingCell = cells.GetCell(i, costFinancing);
                    if (null != costFinancingCell && !string.IsNullOrEmpty(costFinancingCell.StringValue))
                    {
                        float resFinancing = new float();
                        if(float.TryParse(costFinancingCell.StringValue.Trim(), out resFinancing)) item.CostFinancing = resFinancing;
                        else item.CostFinancing = 0;

                        // valueKey.MCP.CostFinancing = costFinancingCell.StringValue.Trim();
                    }
                    else
                    {
                        item.CostFinancing = 0; 
                    }
                    var TargetingCategoricalCell = cells.GetCell(i, TargetingCategoricalIndex);
                    if (null != TargetingCategoricalCell && !string.IsNullOrEmpty(TargetingCategoricalCell.StringValue))
                    {
                        // int TargetingCategoricalLocal = 0;
                        //  if (int.TryParse(TargetingCategoricalCell.StringValue.Trim(), out TargetingCategoricalLocal)) item.TargetingCategorical = TargetingCategoricalLocal;
                        //  else item.TargetingCategorical = 0;
                        item.TargetingCategorical = Convert.ToBoolean(Convert.ToInt16(
                            TargetingCategoricalCell.StringValue.Trim()));


                    }
                    else
                    {
                        item.TargetingCategorical = false;
                    }
                    var TargetingMeansTestedCell = cells.GetCell(i, TargetingMeansTestedIndex);
                    if (null != TargetingMeansTestedCell && !string.IsNullOrEmpty(TargetingMeansTestedCell.StringValue))
                    {
                        // bool TargetingMeansTestedLocal = false;
                        item.TargetingMeansTested = Convert.ToBoolean(Convert.ToInt16(
                            (TargetingMeansTestedCell.StringValue.Trim())));
                      //  if (int.TryParse(TargetingMeansTestedCell.StringValue.Trim(), out TargetingMeansTestedLocal)) item.TargetingMeansTested = TargetingMeansTestedLocal;
                     //   else item.TargetingMeansTested = 0;

                    }
                    else
                    {
                        item.TargetingMeansTested =false;
                    }
                    valueKey.MCP = item;
                    var valIndicator = new KeyValuePair<Key, ValueKey>(Key, valueKey);
                    Values.Add(valIndicator);
                   
                }
            }
            Indicators.ForEach(x => {
                if (!Categories.Contains(x.Area)) Categories.Add(x.Area);
                if (!Categories.Contains(x.Category)) Categories.Add(x.Category);
                if (!Categories.Contains(x.SubCategory)) Categories.Add(x.SubCategory);
            });



        }
    }
}
