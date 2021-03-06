﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using TradeUnion.Model;
using TradeUnion.Library;

namespace TradeUnion.Forms
{
    partial class EventTableForm : Form
    {
        public List<ExtendedEvent> Event;
        public Employee Employee;
        private SortableBindingList<ExtendedEvent> _findEvent;
        private readonly Storage _storage;

        public EventTableForm(Storage storage)
        {
            _storage = storage;
            Employee = null;
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _findEvent = new SortableBindingList<ExtendedEvent>(Event);
            dataGridView.DataSource = null;
            dataGridView.DataSource = _findEvent;
            dataGridView.Columns[4].DefaultCellStyle.Format = "MM.yyyy";

            if (Employee != null)
            {
                SearchTextBox.Text = Employee.ToString();
                OnSearchEvent(this, e);
            }
            else
            {
                SearchTextBox.Text = "";
            }
            Employee = null;
        }

        private void OnSearchEvent(object sender, EventArgs e)
        {
            _findEvent = new SortableBindingList<ExtendedEvent>();
            Event.ForEach(ev =>
            {
                if (ev.Like(SearchTextBox.Text))
                {
                    _findEvent.Add(ev);
                }
            });
            dataGridView.DataSource = null;
            dataGridView.DataSource = _findEvent;
        }

        private void OnDeleteEvent(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Вы уверены что хотите удалить помощь?", @"Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DataGridViewSelectedRowCollection selectedRow = dataGridView.SelectedRows;
                for (int i = 0; i < selectedRow.Count; i++)
                {
                    ExtendedEvent selectedExtendedEvent = selectedRow[i].DataBoundItem as ExtendedEvent;
                    Event.Remove(selectedExtendedEvent);
                    _storage.Delete(selectedExtendedEvent.Event);
                }
                OnSearchEvent(sender, e);
            }
        }

        private void Excel(object sender, EventArgs e)
        {
            // Создаём экземпляр нашего приложения
            Excel.Application excelApp = new Excel.Application();
            // Создаём экземпляр рабочий книги Excel
            Excel.Workbook workBook;
            // Создаём экземпляр листа Excel
            Excel.Worksheet workSheet;

            workBook = excelApp.Workbooks.Add();
            workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);

            workSheet.Columns[1].ColumnWidth = 22;
            workSheet.Columns[2].ColumnWidth = 10;

            workSheet.Cells[1, 1] = "ФИО";
            workSheet.Cells[1, 2] = "ИНН";
            workSheet.Cells[1, 3] = "Помощь";
            workSheet.Cells[1, 4] = "Сумма";
            workSheet.Cells[1, 5] = "Дата";

            for (int i = 2; i <= _findEvent.Count+1; i++)
            {
                workSheet.Cells[i, 1] = _findEvent[i - 2].EmployeeName;
                workSheet.Cells[i, 2] = _findEvent[i - 2].EmployeeInn;
                workSheet.Cells[i, 3] = _findEvent[i - 2].Title;
                workSheet.Cells[i, 4] = _findEvent[i - 2].Sum;
                workSheet.Cells[i, 5] = _findEvent[i - 2].Date.ToString("MM-yyyy");
            }
            
            // Открываем созданный excel-файл
            excelApp.Visible = true;
            excelApp.UserControl = true;
        }

        private void OnSortHeaderClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView.Sort(dataGridView.Columns[e.ColumnIndex], ListSortDirection.Ascending);
        }
    }
}

