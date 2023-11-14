using System.Xml.Linq;
using System;
using OfficeOpenXml;
using System.IO;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private List<Person> people = new List<Person>(); // Create a list to store person data

        public Form1()
        {
            InitializeComponent();

            // Add columns to the ListView with specific widths
            listView1.Columns.Add("Name", 150);
            listView1.Columns.Add("Family", 150);
            listView1.Columns.Add("PhoneNumber", 150);

            // Set the View property to Details to display columns
            listView1.View = View.Details;

            // Set column header text alignment
            foreach (ColumnHeader column in listView1.Columns)
            {
                column.TextAlign = HorizontalAlignment.Center;
            }

            // Load data when the form is opened
            LoadData();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string family = txtFamily.Text;
            string phoneNumber = txtPhoneNumber.Text;

            // Validate input
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(family))
            {
                MessageBox.Show("Please enter both name and family.");
                return;
            }

            // Save data to list
            people.Add(new Person { Name = name, Family = family, PhoneNumber = phoneNumber });

            // Update the ListView
            UpdateListView();

            // Save data to Excel
            SaveToExcel();

            // Clear textboxes
            txtName.Text = "";
            txtFamily.Text = "";
            txtPhoneNumber.Text = "";
        }

        private void UpdateListView()
        {
            // Clear existing items
            listView1.Items.Clear();

            // Add items to the ListView
            foreach (var person in people)
            {
                ListViewItem item = new ListViewItem(person.Name);
                item.SubItems.Add(person.Family);
                item.SubItems.Add(person.PhoneNumber);
                listView1.Items.Add(item);
            }
        }

        private void SaveToExcel()
        {
            // Create a new Excel package
            using (var package = new ExcelPackage())
            {
                // Add a worksheet to the package
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set header row
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Family";
                worksheet.Cells[1, 3].Value = "PhoneNumber";

                // Fill in data
                for (int i = 0; i < people.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = people[i].Name;
                    worksheet.Cells[i + 2, 2].Value = people[i].Family;
                    worksheet.Cells[i + 2, 3].Value = people[i].PhoneNumber;
                }

                // Save the file in the project directory
                string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string excelFilePath = Path.Combine(projectDirectory, "ExcelData.xlsx");

                package.SaveAs(new FileInfo(excelFilePath));

                MessageBox.Show($"Data saved.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Load data from Excel file
            LoadFromExcel();
        }
        private void LoadFromExcel()
        {
            // Load data from an Excel file
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string excelFilePath = Path.Combine(projectDirectory, "ExcelData.xlsx");

            if (File.Exists(excelFilePath))
            {
                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    people.Clear(); // Clear existing data

                    // Start from row 2 to skip the header
                    for (int row = 2; row <= rowCount; row++)
                    {
                        string name = worksheet.Cells[row, 1].Value?.ToString();
                        string family = worksheet.Cells[row, 2].Value?.ToString();
                        string phoneNumber = worksheet.Cells[row, 3].Value?.ToString();

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(family))
                        {
                            people.Add(new Person { Name = name, Family = family, PhoneNumber = phoneNumber });
                        }
                    }

                    // Update the ListView after loading data
                    UpdateListView();
                }

            }
            else
            {
                MessageBox.Show($"Excel file is empty.");
            }
        }
        private void LoadData()
        {
            // Load data from an XML file
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string xmlFilePath = Path.Combine(projectDirectory, "UserData.xml");

            if (File.Exists(xmlFilePath))
            {
                XElement xmlData = XElement.Load(xmlFilePath);

                foreach (var personElement in xmlData.Elements("Person"))
                {
                    string name = personElement.Element("Name").Value;
                    string family = personElement.Element("Family").Value;
                    string phoneNumber = personElement.Element("PhoneNumber").Value;

                    people.Add(new Person { Name = name, Family = family, PhoneNumber = phoneNumber });
                }

                // Update the ListView after loading data
                UpdateListView();
            }
        }

    }

    public class Person
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string PhoneNumber { get; set; }
    }
}