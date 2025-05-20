using Org.BouncyCastle.Asn1.Crmf;
using System.Windows.Forms;

public class CategoryInputForm : Form
{
    public string CategoryName => txtCategoryName.Text.Trim();

    private TextBox txtCategoryName;
    private Button btnOk;
    private Button btnCancel;

    public CategoryInputForm()
    {
        this.Text = "Новая категория";
        this.Width = 300;
        this.Height = 130;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;

        txtCategoryName = new TextBox() { Left = 20, Top = 20, Width = 240 };
        btnOk = new Button() { Text = "ОК", Left = 50, Top = 60, Width = 80 };
        btnCancel = new Button() { Text = "Отмена", Left = 150, Top = 60, Width = 80 };

        btnOk.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Введите название категории.");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        };

        btnCancel.Click += (s, e) =>
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        };

        Controls.Add(txtCategoryName);
        Controls.Add(btnOk);
        Controls.Add(btnCancel);
    }
}
