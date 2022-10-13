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
    public partial class FormShow : Form {
        private List<Item> textbooks;

        public FormShow(List<Item> textbooks) {
            this.textbooks = textbooks;
            InitializeComponent();
        }

        private void FormShow_Load(object sender, EventArgs e) {
            this.addColumns();
            this.addData();
        }

        private void addData() {
            int id = 0;
            foreach (var textbook in textbooks) {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[id].Cells[0].Value = textbook.Id.ToString();
                dataGridView1.Rows[id].Cells[1].Value = textbook.Year.ToString();
                dataGridView1.Rows[id].Cells[2].Value = textbook.Faculty;
                dataGridView1.Rows[id].Cells[3].Value = textbook.Form;
                dataGridView1.Rows[id].Cells[4].Value = textbook.Specialty;
                dataGridView1.Rows[id].Cells[5].Value = textbook.Subject;
                dataGridView1.Rows[id].Cells[6].Value = textbook.Hour.ToString();
                id++;
            }
        }
        
        private void addColumns() {
            string[] colNames = {"ИД", "Год", "Факультет", "Форма", "Специальность", "Предмет", "Часы"};
            foreach(string name in colNames){
                dataGridView1.Columns.Add(name, name);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }
    }
}
