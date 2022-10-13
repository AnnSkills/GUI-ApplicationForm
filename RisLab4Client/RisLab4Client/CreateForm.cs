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
    public partial class CreateForm : Form {
        private Item textbook;
        private bool isCreate;
        private Form1 parrentForm;

        public CreateForm(Form1 parrentForm) {
            InitializeComponent();
            isCreate = true;
            this.textbook = new Item();
            this.parrentForm = parrentForm;
        }

        public CreateForm(Form1 parrentForm, Item textbook) {
            InitializeComponent();
            isCreate = false ;
            this.textbook = textbook;
            this.parrentForm = parrentForm;
        }

        private void CreateForm_Load(object sender, EventArgs e) {
            this.initializeFields();
        }

        private void initializeFields() {
            string[] valueArray = { "", "", "", "", "", "", "" };
            if (!this.isCreate) {
                textBox1.Enabled = false;
                valueArray[0] = this.textbook.Id.ToString();
                valueArray[1] = this.textbook.Year.ToString();
                valueArray[2] = this.textbook.Faculty;
                valueArray[3] = this.textbook.Form;
                valueArray[4] = this.textbook.Specialty;
                valueArray[5] = this.textbook.Subject;
                valueArray[6] = this.textbook.Hour.ToString();
            }
            this.appendValues(valueArray);
        }

        private void appendValues(string[] valueArray) {
            textBox1.Text = valueArray[0];
            textBox2.Text = valueArray[1];
            textBox3.Text = valueArray[2];
            textBox4.Text = valueArray[3];
            textBox5.Text = valueArray[4];
            textBox6.Text = valueArray[5];
            textBox7.Text = valueArray[6];
        }

        private void button1_Click(object sender, EventArgs e) {
            if (this.updateTextbookFields()) {
                updateTextbook();
                if (parrentForm.SendUppend(this.textbook, isCreate)) {
                    this.showErrors(this.isCreate ? "Добавление прошло успешно!" : "Изменение прошло успешно!", "Готово");
                    this.Close();
                } else {
                    this.showErrors("ИД занято!");
                }
            }
        }

        private void updateTextbook() {
            int id;
            double price, year;
            Int32.TryParse(textBox1.Text, out id);
            Double.TryParse(textBox2.Text, out price);
            Double.TryParse(textBox7.Text, out year);
            textbook.Id = id;
            textbook.Year = year;
            textbook.Faculty = textBox2.Text;
            textbook.Form = textBox3.Text;
            textbook.Specialty = textBox4.Text;
            textbook.Subject = textBox4.Text;
            textbook.Hour = price;
        }

        private bool updateTextbookFields() {
            string errorMessage = "";
            if (!Form1.isInt(textBox1.Text)) {
                errorMessage += "Ид не int-число!\n";
            }
            if (!Form1.isDouble(textBox2.Text)) {
                errorMessage += "Год не число!\n";
            }
            if (textBox3.Text.Length <= 0) {
                errorMessage += "Факультет пусто!\n";
            }
            if (textBox4.Text.Length <= 0) {
                errorMessage += "Форма пуст!\n";
            }
            if (textBox5.Text.Length <= 0) {
                errorMessage += "Специальность пусто!\n";
            }
            if (textBox6.Text.Length <= 0) {
                errorMessage += "Предмет пуст!\n";
            }
            if (!Form1.isDouble(textBox7.Text)) {
                errorMessage += "Часы не число!\n";
            }
            return this.showErrors(errorMessage);
        }

        private bool showErrors(string errorMessage, string header = "Ошибки добавления:") {
            if(errorMessage.Length > 0) {
                MessageBox.Show(
                    errorMessage,
                    header,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1
                );
                return false;
            } else {
                return true;
            }
        }

        private void label2_Click(object sender, EventArgs e) {

        }
    }
}
