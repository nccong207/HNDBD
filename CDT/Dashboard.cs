using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraGrid;
using DataFactory;
using FormFactory;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraCharts;
using System.Data;
using CDTLib;
using System.Windows.Forms;
using CDTControl;
using Plugins;
using DevExpress.XtraEditors;

namespace CDT
{
    class Dashboard
    {
        private DataReport _data;
        private DataRow _drReport;
        public Dashboard(DataRow drReport)
        {
            _drReport = drReport;
            _data = new DataReport(drReport);
            if (_data.DsData == null)
                _data.GetData();
            if (_data.DsData.Tables[0].Rows.Count == 0)
            {
                _data.DsData.Tables[0].Rows.Add(_data.DsData.Tables[0].NewRow());
            }
            _data.GenFilterString();
        }

        void chartMain_DoubleClick(object sender, EventArgs e)
        {
            //DataReport Data = (_data as DataReport).GetDataForDetailReport("", "") as DataReport;
            //if (Data != null)
            //{
            //    if (Data.DsData.Tables[0].Rows.Count > 0)   //bổ sung trường hợp ko chuyển được dữ liệu tham số từ cha sang con
            //    {
            //        ReportPreview rptPre = new ReportPreview(Data);
            //        rptPre.ShowDialog();
            //    }
            //}
        }

        public void GetGridReport(GridControl tmp, bool first)
        {
            ReportPreview rptPre = new ReportPreview(_data, tmp, first);
            PluginManager _plugins = new PluginManager();
            DataCustomReport cData = new DataCustomReport(_data.DrTable, tmp.FindForm() as XtraForm);
            _plugins.ExecuteICR(_data.DrTable["sysReportID"].ToString(), cData);
            string[] cls = _drReport["ExtraSql"].ToString().Split(new char[] { ',' });
            if (cls.Length == 0)
                return;
        }

        public void GetPieChartReport(ChartControl chartMain, bool first)
        {
            string chartField1 = _data.DrTable["ChartField1"].ToString();//.ToUpper();
            string chartField2 = _data.DrTable["ChartField2"].ToString();//.ToUpper();
            string chartField3 = _data.DrTable["ChartField3"].ToString();//.ToUpper();
            if (chartField1 == string.Empty && chartField2 == string.Empty)
                return;
            _data.GetDataForReport();

            chartMain.DataSource = _data.DtReportData;
            Series s = new Series(chartField2, ViewType.Pie3D);
            s.DataSource = _data.DtReportData;
            s.ArgumentDataMember = chartField1;
            s.ValueDataMembers.AddRange(new string[] { chartField2 });
            s.ValueScaleType = ScaleType.Numerical;
            ((PieSeriesLabel)s.Label).Position = PieSeriesLabelPosition.TwoColumns;
            ((PiePointOptions)s.PointOptions).PercentOptions.ValueAsPercent = true;
            ((PiePointOptions)s.PointOptions).ValueNumericOptions.Format = NumericFormat.Percent;
            ((PiePointOptions)s.PointOptions).ValueNumericOptions.Precision = 0;
            ((PiePointOptions)s.PointOptions).PointView = PointView.ArgumentAndValues;

            ((PieSeriesViewBase)s.View).ExplodeMode = PieExplodeMode.All;

            chartMain.Legend.Visible = false;
            chartMain.Series.Clear();
            chartMain.Series.Add(s);
            ChartTitle chartTitle = new ChartTitle();
            chartTitle.Font = new System.Drawing.Font("Tahoma", 14);
            chartTitle.TextColor = System.Drawing.Color.RoyalBlue;
            chartTitle.Text = Config.GetValue("Language").ToString() == "0" ? _drReport["MenuName"].ToString() : _drReport["MenuName2"].ToString();
            chartMain.Titles.Clear();
            chartMain.Titles.Add(chartTitle);
            if (first && _data.DrTable["LinkField"].ToString() != "")
                chartMain.MouseDoubleClick += new MouseEventHandler(chartMain_DoubleClick);
        }

        private string GetSum(string col)
        {
            if (_data.DtReportData == null)
                return "0";
            object o = _data.DtReportData.Compute("sum([" + col + "])", "");
            string ss = o == null ? "" : o.ToString();
            decimal d = ss == "" ? 0 : Decimal.Parse(ss);
            string s = d.ToString("###,###,###,###");
            return s;
        }

        public void GetChartReport(ChartControl chartMain, bool first)
        {
            string chartField1 = _data.DrTable["ChartField1"].ToString();//.ToUpper();
            string chartField2 = _data.DrTable["ChartField2"].ToString();//.ToUpper();
            string chartField3 = _data.DrTable["ChartField3"].ToString();//.ToUpper();
            if (chartField1 == string.Empty && chartField2 == string.Empty && chartField3 == string.Empty)
                return;
            _data.GetDataForReport();

            string[] sss = chartField3.Split(',');
            ViewType vt = ViewType.Bar;
            //if (_data.DtReportData.Rows.Count > 5 && sss.Length >= 2)
            //    vt = ViewType.StackedBar;
            //if (_data.DtReportData.Rows.Count > 4 && sss.Length >= 3)
            //    vt = ViewType.StackedBar;

            chartMain.DataSource = _data.DtReportData;
            Series s1 = new Series(chartField2, vt);
            s1.DataSource = _data.DtReportData;
            s1.ArgumentDataMember = chartField1;
            s1.LegendText = chartField2 + ": " + GetSum(chartField2);
            s1.ValueDataMembers.AddRange(new string[] { chartField2});
            s1.ValueScaleType = ScaleType.Numerical;
            s1.PointOptions.PointView = PointView.Values;
            s1.PointOptions.ValueNumericOptions.Format = NumericFormat.Number;
            s1.PointOptions.ValueNumericOptions.Precision = 0;

            chartMain.Series.Clear();
            chartMain.Series.Add(s1);

            for (int i = 0; i < sss.Length; i++)
            {
                string ss = sss[i].Trim();
                Series s2 = new Series(ss, vt);
                s2.DataSource = _data.DtReportData;
                s2.ArgumentDataMember = chartField1;
                s2.LegendText = ss + ": " + GetSum(ss);
                s2.ValueDataMembers.AddRange(new string[] { ss });
                s2.ValueScaleType = ScaleType.Numerical;
                s2.PointOptions.PointView = PointView.Values;
                s2.PointOptions.ValueNumericOptions.Format = NumericFormat.Number;
                s2.PointOptions.ValueNumericOptions.Precision = 0;
                chartMain.Series.Add(s2);
            }
            ChartTitle chartTitle = new ChartTitle();
            chartTitle.Text = Config.GetValue("Language").ToString() == "0" ? _drReport["MenuName"].ToString() : _drReport["MenuName2"].ToString();
            chartTitle.Font = new System.Drawing.Font("Tahoma", 14);
            chartTitle.TextColor = System.Drawing.Color.RoyalBlue;
            chartMain.Titles.Clear();
            chartMain.Titles.Add(chartTitle);
            chartMain.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.Left;
            chartMain.Legend.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
            chartMain.Legend.Direction = LegendDirection.LeftToRight;
            XYDiagram d = (XYDiagram)chartMain.Diagram;
            d.AxisY.NumericOptions.Format = NumericFormat.Number;
            d.AxisY.NumericOptions.Precision = 0;
            if (first && _data.DrTable["LinkField"].ToString() != "")
                chartMain.DoubleClick += new EventHandler(chartMain_DoubleClick);
        }
    }
}
