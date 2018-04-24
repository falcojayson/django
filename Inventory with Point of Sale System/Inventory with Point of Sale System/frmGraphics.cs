using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;

namespace Inventory_with_Point_of_Sale_System
{

    public partial class frmGraphics : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Users\User\Desktop\Inventory with Point of Sale System\Inventory with Point of Sale System\bin\Debug\InventoryPosDB.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True");
        DateTime date1;
        DateTime date2;
        string d1;
        string d2;
        int totalQuantity;
        int finalQuantity;
        int minValue;
        int maxValue;
        int finalMax;
        int maxInterval;
        int sumRowCount;

        public frmGraphics()
        {
            InitializeComponent();
        }

        private void frmGraphics_Load(object sender, EventArgs e)
        {
            autocomplete();
        }

        private void autocomplete()
        {
            dtgOutQty.Visible = false;

            SqlCommand cmd = new SqlCommand("SELECT Descr FROM tblProducts", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            AutoCompleteStringCollection MyCollection = new AutoCompleteStringCollection();
            while (reader.Read())
            {
                MyCollection.Add(reader.GetString(0));
            }
            txtSearch.AutoCompleteCustomSource = MyCollection;
            con.Close();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            sumRowCount = 0;

            chart1.Series.Clear();

            if (cmbYear.Text == "" || cmbYear.Text == null)
            {
                MessageBox.Show("Please indicate the year.","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            
            else
            {
                for (int i = 1; i <= 12; i++)
                {


                    d1 = i.ToString() + "/" + "1" + "/" + cmbYear.Text.ToString();
                    d2 = i.ToString() + "/" + DateTime.DaysInMonth(2018, i).ToString() + "/" + cmbYear.Text.ToString();

                    date1 = Convert.ToDateTime(d1);
                    date2 = Convert.ToDateTime(d2);

                    //MessageBox.Show(date1.ToShortDateString() + "        " + date2.ToShortDateString());
                    if (txtSearch.Text == "" || txtSearch.Text == null)
                    {
                        SqlCommand sqlCom = new SqlCommand("SELECT ID, Descr, Qty, Date, TransType FROM tblIn_Out WHERE Date BETWEEN '" + date1.Date + "' AND '" + date2.Date + "' ", con);
                        SqlDataAdapter sqlSda = new SqlDataAdapter();
                        sqlSda.SelectCommand = sqlCom;
                        DataTable dataTbl = new DataTable();
                        sqlSda.Fill(dataTbl);
                        dtgOutQty.DataSource = dataTbl;
                    }
                    else
                    {
                        SqlCommand sqlCom = new SqlCommand("SELECT ID, Descr, Qty, Date, TransType FROM tblIn_Out WHERE Descr = '" + txtSearch.Text + "' AND Date BETWEEN '" + date1.Date + "' AND '" + date2.Date + "' ", con);
                        SqlDataAdapter sqlSda = new SqlDataAdapter();
                        sqlSda.SelectCommand = sqlCom;
                        DataTable dataTbl = new DataTable();
                        sqlSda.Fill(dataTbl);
                        dtgOutQty.DataSource = dataTbl;
                    }

                    finalQuantity = 0;

                    for (int n = 0; n < dtgOutQty.Rows.Count; n++)
                    {
                        //this is to get the total quantity of product outflow in every month
                        totalQuantity = int.Parse(dtgOutQty.Rows[n].Cells[2].Value.ToString());
                        finalQuantity += totalQuantity;

                    }

                    //this stores the total quantity of product outflow in an array
                    //every run of the parent loop gets the sum of quamtity in every month which is equal to 12 months
                    Global.qtyPerMonth[i - 1] = finalQuantity.ToString();


                    int rowCount = dtgOutQty.Rows.Count;
                    sumRowCount += rowCount;

                }

                for (int i = 0; i < 12; i++)
                {
                    Global.qtyPerMonthInt[i] = int.Parse(Global.qtyPerMonth[i]);

                }

                minValue = Global.qtyPerMonthInt.Min();
                maxValue = Global.qtyPerMonthInt.Max();

                double max = maxValue + maxValue * 0.1;


                finalMax = Convert.ToInt32(max);
                maxInterval = finalMax / 10;

                if (sumRowCount == 0)
                {
                    MessageBox.Show("No data for this Year.","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                else
                {
                    fillGraph();
                }

                //MessageBox.Show(sumRowCount.ToString());
            }
        }


        private void fillGraph()
        {
            chart1.Series.Clear();

            float x1 = float.Parse(Global.qtyPerMonth[0]);
            float x2 = float.Parse(Global.qtyPerMonth[1]);
            float x3 = float.Parse(Global.qtyPerMonth[2]);
            float x4 = float.Parse(Global.qtyPerMonth[3]);
            float x5 = float.Parse(Global.qtyPerMonth[4]);
            float x6 = float.Parse(Global.qtyPerMonth[5]);
            float x7 = float.Parse(Global.qtyPerMonth[6]);
            float x8 = float.Parse(Global.qtyPerMonth[7]);
            float x9 = float.Parse(Global.qtyPerMonth[8]);
            float x10 = float.Parse(Global.qtyPerMonth[9]);
            float x11 = float.Parse(Global.qtyPerMonth[10]);
            float x12 = float.Parse(Global.qtyPerMonth[11]);


            var chart = chart1.ChartAreas[0];
            chart.AxisX.IntervalType = DateTimeIntervalType.Number;

            chart.AxisX.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.Format = "";
            //chart.AxisX.Title = "Months";
            chart.AxisY.Title = "Sales Quantity";
            chart.AxisY.LabelStyle.IsEndLabelVisible = true;

            chart.AxisX.Minimum = 1;
            chart.AxisX.Maximum = 12;

            chart.AxisY.Minimum = 0;
            chart.AxisY.Maximum = finalMax;

            chart.AxisX.Interval = 1;
            chart.AxisY.Interval = maxInterval;

            
            chart1.Series.Add("Series");
            chart1.Series["Series"].ChartType = SeriesChartType.Line;
            chart1.Series["Series"].BorderColor = Color.Red;
            chart1.Series[0].IsVisibleInLegend = false;
            
            chart1.Series["Series"].Points.AddXY(1, x1);
            chart1.Series["Series"].Points.AddXY(2, x2);
            chart1.Series["Series"].Points.AddXY(3, x3);
            chart1.Series["Series"].Points.AddXY(4, x4);
            chart1.Series["Series"].Points.AddXY(5, x5);
            chart1.Series["Series"].Points.AddXY(6, x6);
            chart1.Series["Series"].Points.AddXY(7, x7);
            chart1.Series["Series"].Points.AddXY(8, x8);
            chart1.Series["Series"].Points.AddXY(9, x9);
            chart1.Series["Series"].Points.AddXY(10, x10);
            chart1.Series["Series"].Points.AddXY(11, x11);
            chart1.Series["Series"].Points.AddXY(12, x12);

            chart1.Series["Series"].Points[0].AxisLabel = "January";
            chart1.Series["Series"].Points[1].AxisLabel = "February";
            chart1.Series["Series"].Points[2].AxisLabel = "March";
            chart1.Series["Series"].Points[3].AxisLabel = "April";
            chart1.Series["Series"].Points[4].AxisLabel = "May";
            chart1.Series["Series"].Points[5].AxisLabel = "June";
            chart1.Series["Series"].Points[6].AxisLabel = "July";
            chart1.Series["Series"].Points[7].AxisLabel = "August";
            chart1.Series["Series"].Points[8].AxisLabel = "September";
            chart1.Series["Series"].Points[9].AxisLabel = "October";
            chart1.Series["Series"].Points[10].AxisLabel = "November";
            chart1.Series["Series"].Points[11].AxisLabel = "December";
            
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmMain main = new frmMain();
            main.Show();
        }
    }
}
