using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinSchedule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GridToTimelineView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // UltraGrid
            ultraGrid1.DataSource = getTable();
            ultraGrid1.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            ultraGrid1.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;

            // UltraCalendarInfo
            ultraCalendarInfo1.Owners.Add("Charlie");
            ultraCalendarInfo1.Owners.Add("Jamie");
            ultraCalendarInfo1.Owners.Add("Leah");
            ultraCalendarInfo1.Owners.UnassignedOwner.Visible = false;

            // UltraTimelineView
            ultraTimelineView1.AllowDrop = true;
            ultraTimelineView1.MaximumOwnersInView = 3;
            ultraTimelineView1.CalendarInfo = this.ultraCalendarInfo1;
        }

        private void ultraGrid1_SelectionDrag(object sender, CancelEventArgs e)
        {
            // UltraGridのドラッグを開始する
            ultraGrid1.DoDragDrop(ultraGrid1.Selected.Rows, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void ultraTimelineView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(SelectedRowsCollection)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ultraTimelineView1_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップ位置の時間とオーナーを取得する
            Point point = ultraTimelineView1.PointToClient(new Point(e.X, e.Y));
            DateTime dt = (DateTime)ultraTimelineView1.DateTimeFromPoint(point);
            Owner owner = ultraTimelineView1.OwnerFromPoint(point);

            // 追加するAppointmentを作成する
            SelectedRowsCollection SelRows = (SelectedRowsCollection)e.Data.GetData(typeof(SelectedRowsCollection));
            Appointment app = new Appointment(dt, dt.AddHours((double)SelRows[0].Cells["Duration"].Value));
            app.Subject = SelRows[0].Cells["AppId"].Value.ToString();
            app.Owner = owner;

            // Appointmentを追加する
            ultraCalendarInfo1.Appointments.Add(app);
        }

        private DataTable getTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add("AppId", typeof(string));
            table.Columns.Add("Duration", typeof(double));

            for (int i = 0; i < 100; i++)
            {
                table.Rows.Add(new object[] { "Appointment" + i, 0.5 * (i % 3 + 1)});
            }

            return table;
        }
    }
}
