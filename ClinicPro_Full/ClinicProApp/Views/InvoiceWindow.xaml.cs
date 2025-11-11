using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;
using ClinicProApp.Models;
using ClinicProApp.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicProApp.Views
{
    public partial class InvoiceWindow : Window
    {
        private readonly DatabaseService _db = new DatabaseService();
        private readonly List<InvoiceItem> items = new List<InvoiceItem>();

        public InvoiceWindow()
        {
            InitializeComponent();
            LoadPatients();
            dpDate.SelectedDate = DateTime.Today;
            dgItems.ItemsSource = items;
            UpdateSummary();
        }

        private void LoadPatients()
        {
            var list = new List<dynamic>();
            using var rdr = _db.ExecuteReader("SELECT Id, FirstName, LastName FROM Patients ORDER BY FirstName");
            while (rdr.Read())
            {
                var id = rdr.GetInt32(0);
                var full = (rdr.IsDBNull(1)?"" : rdr.GetString(1)) + " " + (rdr.IsDBNull(2)?"" : rdr.GetString(2));
                list.Add(new { Id = id, Display = full.Trim() });
            }
            cbPatients.ItemsSource = list;
            if (list.Count>0) cbPatients.SelectedIndex = 0;
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescription.Text)) { MessageBox.Show("أدخل وصفًا"); return; }
            if (!double.TryParse(txtPrice.Text, out double price)) { MessageBox.Show("السعر غير صحيح"); return; }
            if (!int.TryParse(txtQty.Text, out int qty)) { MessageBox.Show("الكمية غير صحيحة"); return; }

            items.Add(new InvoiceItem { Description = txtDescription.Text.Trim(), Price = price, Qty = qty });
            dgItems.Items.Refresh();
            txtDescription.Clear(); txtPrice.Clear(); txtQty.Clear();
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            var sub = items.Sum(i => i.Total);
            var tax = Math.Round(sub * 0.0, 2);
            var total = sub + tax;
            txtSubTotal.Text = $"المجموع الفرعي: {sub:0.00}";
            txtTax.Text = $"الضريبة: {tax:0.00}";
            txtTotal.Text = $"الإجمالي: {total:0.00}";
        }

        private void BtnSaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (cbPatients.SelectedValue == null) { MessageBox.Show("اختر مريض"); return; }
            var pid = (int)cbPatients.SelectedValue;
            var date = dpDate.SelectedDate?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
            var total = items.Sum(i => i.Total);

            _db.ExecuteNonQuery("INSERT INTO Invoices (PatientId, Date, Total, Paid, Method, Notes) VALUES (@pid,@date,@total,0,'--','')",
                new SQLiteParameter("@pid", pid),
                new SQLiteParameter("@date", date),
                new SQLiteParameter("@total", total));

            MessageBox.Show("تم حفظ الفاتورة (سجل مبسط في جدول Invoices)");
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            if (cbPatients.SelectedValue == null) { MessageBox.Show("اختر مريض"); return; }
            var pid = (int)cbPatients.SelectedValue;
            var patientName = (cbPatients.SelectedItem as dynamic)?.Display ?? "مريض";
            var invoiceDate = dpDate.SelectedDate?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
            var sub = items.Sum(i => i.Total);
            var tax = Math.Round(sub * 0.0, 2);
            var total = sub + tax;

            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Invoice_{patientName}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("ClinicPro").FontSize(20).Bold();
                                col.Item().Text("فاتورة").FontSize(14);
                            });
                            row.ConstantItem(150).AlignRight().Text($"التاريخ: {invoiceDate}");
                        });

                    page.Content()
                        .PaddingVertical(10)
                        .Column(col =>
                        {
                            col.Item().Text($"المريض: {patientName}").Bold();
                            col.Item().SizedBox(8);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(6);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("الوصف");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("السعر");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("الكمية");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("المجموع");
                                });

                                foreach (var it in items)
                                {
                                    table.Cell().Element(CellStyle).Text(it.Description);
                                    table.Cell().Element(CellStyle).AlignCenter().Text($"{it.Price:0.00}");
                                    table.Cell().Element(CellStyle).AlignCenter().Text($"{it.Qty}");
                                    table.Cell().Element(CellStyle).AlignCenter().Text($"{it.Total:0.00}");
                                }

                                static IContainer CellStyle(IContainer c) => c.PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingHorizontal(4);
                            });

                            col.Item().PaddingTop(10).AlignRight().Column(sum =>
                            {
                                sum.Item().Text($"المجموع الفرعي: {sub:0.00}");
                                sum.Item().Text($"الضريبة: {tax:0.00}");
                                sum.Item().Text($"الإجمالي: {total:0.00}").Bold();
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x => { x.Span("شكراً لزيارتكم"); });
                });
            });

            doc.GeneratePdf(fileName);

            MessageBox.Show($"تم إنشاء PDF على سطح المكتب: {fileName}");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
