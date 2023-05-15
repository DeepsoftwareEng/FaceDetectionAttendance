using Emgu.CV;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ClosedXML;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.Excel;
using System.Reflection;

namespace FaceDetectionAttendance.MVVM.ViewModel
{
    internal class ExportExcel
    {
        public void ExportExcel_DataGrid(DataGrid temp)
        {
            if (temp.Items.Count > 0)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt= ".xlsx";
                saveDialog.Filter = "Excel Files|*xlsx;*xls;*xlsm";
                saveDialog.FileName = "Report.xlsx";
                if(saveDialog.ShowDialog()==true)
                {
                    try
                    {
                        var workbook = new XLWorkbook();
                        var sheet = workbook.Worksheets.Add(temp.Name);
                        int rowWrite = 1;
                        sheet.Cell(rowWrite,1).Value= temp.Name;
                        rowWrite++;
                        for(int col = 0; col< temp.Columns.Count; col++)
                        {
                            sheet.Cell(rowWrite, col+1).Value = temp.Columns[col].Header.ToString();
                        }
                        rowWrite++;
                        Type t;
                        PropertyInfo[] p;
                        for(int row = 0; row < temp.Items.Count; row++)
                        {
                            t = temp.Items[row].GetType();
                            p = t.GetProperties();
                            for(int col = 0; col < temp.Columns.Count; col++)
                            {
                                sheet.Cell(rowWrite, col + 1).Value = p[col].GetValue(temp.Items[row]).ToString();
                            }
                            rowWrite++;
                        }
                        
                        workbook.SaveAs(saveDialog.FileName);
                        MessageBox.Show("Export to excel successfull", "Message");
                        workbook.Dispose();
                        
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Data in Table is nothing","Error",MessageBoxButton.OK);
            }
        }
    }
}
