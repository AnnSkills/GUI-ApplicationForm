using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RisLab4Client {
    public partial class FilterForm : Form {
        private Form1 parrentForm;

        public FilterForm(Form1 parrentForm) {
            InitializeComponent();
            this.parrentForm = parrentForm;
        }

        private void button1_Click(object sender, EventArgs e) {
            List<Item> textbooks = parrentForm.ReceiveTextbooks(
                comboBox1.SelectedItem.ToString(),
                comboBox2.SelectedItem.ToString(),
                textBox1.Text
            );

            FormShow formShow = new FormShow(textbooks);
            formShow.Show();
        }

        private void FilterForm_Load(object sender, EventArgs e) {
            string[] fields = new string[] { "ИД", "Год", "Факультет", "Форма", "Специальность", "Предмет", "Часы" };
            comboBox1.Items.Add("<Без Сортировки>");
            comboBox1.Items.AddRange(fields);
            comboBox2.Items.AddRange(fields);
            comboBox1.SelectedIndex = comboBox2.SelectedIndex = 0;
        }
    }
}
