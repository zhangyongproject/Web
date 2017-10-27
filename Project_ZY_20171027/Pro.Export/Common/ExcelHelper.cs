using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Drawing;
using Aspose.Cells;
using System.Xml;

namespace Pro.Export.Common {
    public class ExcelHelper {
        public Workbook workBook;
        public Worksheet workSheet;
        /// <summary>
        /// 当前Xml文档对象
        /// </summary>
        private XmlDocument xmlDocument;
        /// <summary>
        /// 当前Xml配置节点
        /// </summary>
        public XmlNode xmlNode;

        /// <summary>
        /// 打开工作文档区
        /// <param name="fileName">文件名(包含路径)</param>
        /// </summary>
        public Workbook OpenWorkbook(string fileName) {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(string.Format("没有找到模板文件:{0}.", fileName));
            workBook = new Workbook();
            workBook.Open(fileName);
            return workBook;
        }

        /// <summary>
        /// 加载Xml配置文件
        /// </summary>
        /// <param name="path"></param>
        public XmlDocument LoadExpXmlConfig(string path) {
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format("没有找到Xml配置文件:{0}.", path));
            xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            return xmlDocument;
        }

        /// <summary>
        /// 查找Xml配置文档中指定的模板名，并打开工作文档区
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="TemplateName"></param>
        /// <returns></returns>
        public Workbook OpenWorkbook(string xmlPath, string templateName) {
            LoadExpXmlConfig(xmlPath);
            xmlNode = xmlDocument.SelectSingleNode("root/Template[@name='" + templateName + "']");

            return OpenWorkbook(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlNode.Attributes["TemplatePath"].Value));
        }

        /// <summary>
        /// 设置当前工作Sheet
        /// </summary>
        public Worksheet SetWorkSheet(string sheetName) {
            workSheet = workBook.Worksheets[sheetName];
            return workSheet;
        }

        /// <summary>
        /// 移除Sheet
        /// </summary>
        public void RemoveWorkSheet(string sheetName) {
            workBook.Worksheets.RemoveAt(workBook.Worksheets[sheetName].Index);
        }

        /// <summary>
        /// 保存工作文档区
        /// </summary>
        public void SaveWorkbook(string fileName) {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            else if (File.Exists(fileName))
                File.Delete(fileName);

            workBook.Save(fileName);
        }

        /// <summary>
        /// 将 XmlElement 转换为 Point 实例
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public Point XmlNodeToPoint(XmlNode xNode) {
            int d; bool b;

            string x = xNode.Attributes["x"] == null ? null : xNode.Attributes["x"].Value;
            string y = xNode.Attributes["y"] == null ? null : xNode.Attributes["y"].Value;
            string bFmt = xNode.Attributes["bFmt"] == null ? null : xNode.Attributes["bFmt"].Value;

            return new Point {
                X = int.TryParse(x, out d) ? d : -1,
                Y = int.TryParse(y, out d) ? d : -1,
                bFmt = bool.TryParse(bFmt, out b) ? b : false
            };
        }

        /// <summary>
        /// 插入行
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="count"></param>
        public void InsertRows(int rowNum, int count) {
            workSheet.Cells.InsertRows(rowNum, count);
        }

        /// <summary>
        /// 插入列
        /// </summary>
        /// <param name="columnNum"></param>
        /// <param name="count"></param>
        public void InsertColumn(int columnNum) {
            workSheet.Cells.InsertColumn(columnNum, true);
        }

        /// <summary>
        /// 插入列
        /// </summary>
        /// <param name="columnNum"></param>
        /// <param name="count"></param>
        public void DeleteColumn(int columnNum) {
            workSheet.Cells.DeleteColumn(columnNum, true);
        }

        /// <summary>
        /// 复制行
        /// </summary>
        public void CopyColumn(int columnIndex, int destinIndex) {
            workSheet.Cells.CopyColumn(workSheet.Cells, columnIndex, destinIndex);
        }

        #region 添加图片
        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="p">需要插入图片的单元格坐标</param>
        /// <param name="imgUrl">图片的链接</param>
        /// <param name="bRowAutoFit">是否行自适应高度</param>
        /// <param name="bColumnAutoFit">是否列自适应高度</param>
        public void AddImage(Point p, string imgUrl, bool bRowAutoFit, bool bColumnAutoFit) {
            if (p.X < 0 || p.Y < 0) return;
            if (!File.Exists(imgUrl))
                return;
            Bitmap bit = new Bitmap(imgUrl);
            int height = bit.Height == 0 ? 1 : bit.Height;
            int width = bit.Width == 0 ? 1 : bit.Width;
            bit.Dispose();
            height = height > 546 ? 546 : height;
            width = width > 2046 ? 2046 : width;

            if (bRowAutoFit)
                workSheet.Cells.SetRowHeightPixel(p.X, height);
            if (bColumnAutoFit)
                workSheet.Cells.SetColumnWidthPixel(p.Y, width);

            int imgIdx = workSheet.Pictures.Add(p.X, p.Y, imgUrl);

            workSheet.Pictures[imgIdx].Height = height;
            workSheet.Pictures[imgIdx].Width = width;
            workSheet.Pictures[imgIdx].Left = 1;
            workSheet.Pictures[imgIdx].Top = 1;
        }

        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="p1">起点坐标</param>
        /// <param name="p2">终点坐标</param>
        /// <param name="imgUrl"></param>
        public void AddImage(Point p1, Point p2, string imgUrl) {
            if (File.Exists(imgUrl))
                workSheet.Pictures.Add(p1.X, p1.Y, p2.X, p2.Y, imgUrl);
        }
        #endregion

        #region 合并单元格
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="firstRow"></param>
        /// <param name="firstColumn"></param>
        /// <param name="rowNum">行的数字</param>
        /// <param name="columnNum">列的数字</param>
        public void MergeCell(int firstRow, int firstColumn, int rowNum, int columnNum) {
            workSheet.Cells.Merge(firstRow, firstColumn, rowNum, columnNum);
        }
        #endregion

        #region 创建一个高亮居中样式
        /// <summary>
        /// 创建一个高亮居中样式
        /// </summary>
        /// <returns></returns>
        public Style CreateHighlightStyle() {
            int iStyle = workBook.Styles.Add();
            Style style = workBook.Styles[iStyle];
            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            return style;
        }
        #endregion

        #region 设置单元格内容
        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="p"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Cell SetCellValue(Point p, string val) {
            if (p.X < 0 || p.Y < 0) return null;
            Cell cell = workSheet.Cells[p.X, p.Y];
            if (p.bFmt)
                cell.PutValue(string.Format(cell.StringValue, val.ToString()));
            else
                cell.PutValue(val.ToString());

            return cell;
        }

        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="val"></param>
        public Cell SetCellValue(Point p, string val, Style style) {
            if (p.X < 0 || p.Y < 0) return null;
            Cell cell = workSheet.Cells[p.X, p.Y];
            cell.PutValue(val.ToString());
            cell.Style = style;
            return cell;
        }

        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="val"></param>
        public Cell SetCellValue(int x, int y, string val) {
            if (x < 0 || y < 0) return null;
            Cell cell = workSheet.Cells[x, y];
            cell.PutValue(val.ToString());

            return cell;
        }

        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="val"></param>
        public Cell SetCellValue(int x, int y, object val) {
            Point p = new Point() { X = x, Y = x };
            return SetCellValue(p, val);
        }

        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="val"></param>
        public Cell SetCellValue(int x, int y, double val) {
            if (x < 0 || y < 0) return null;
            Cell cell = workSheet.Cells[x, y];
            cell.PutValue(val);

            return cell;
        }

        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="p"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Cell SetCellValue(Point p, double val) {
            if (p.X < 0 || p.Y < 0) return null;
            Cell cell = workSheet.Cells[p.X, p.Y];
            cell.PutValue(val);

            return cell;
        }

        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="p"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Cell SetCellValue(Point p, DateTime val) {
            if (p.X < 0 || p.Y < 0) return null;
            Cell cell = workSheet.Cells[p.X, p.Y];
            cell.PutValue(val);

            return cell;
        }

        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="p"></param>
        /// <param name="val"></param>
        public Cell SetCellValue(Point p, object val) {
            if (p.X < 0 || p.Y < 0) return null;
            Cell cell = workSheet.Cells[p.X, p.Y];

            if (val.GetType() == typeof(bool))
                cell.PutValue(Convert.ToBoolean(val));
            else
                cell.PutValue(val);

            return cell;
        }
        #endregion

        #region 获取单元格
        /// <summary>
        /// 获取单元格
        /// </summary>
        /// <param name="p"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Cell GetCell(Point p) {
            if (p.X < 0 || p.Y < 0) return null;
            return workSheet.Cells[p.X, p.Y];
        }
        #endregion

        #region 组合行
        /// <summary>
        /// 组合行
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="lastIndex"></param>
        public void GroupRows(int firstIndex, int lastIndex, bool isHidden) {
            workSheet.Cells.GroupRows(firstIndex, lastIndex, isHidden);
        }
        #endregion
    }
}
