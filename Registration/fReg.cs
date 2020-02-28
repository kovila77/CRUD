using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Registration
{
    public partial class fReg : Form
    {
        private readonly Regex loginCheckRegex = new Regex(DBUsersHandler.DBUsersHandler.regularOfCorrectLogin);
        private DBUsersHandler.DBUsersHandler _dbConrol = new DBUsersHandler.DBUsersHandler();

        private int userId = -1;
        private string userLogin = null;
        private byte[] userSalt = null;
        private byte[] userPassword = null;
        private DateTime dateReg;

        private FormType frmType;
        public enum FormType
        {
            SelfMade,
            Insert,
            Update
        }
        public FormType FrmType { get { return frmType; } }

        public int UserId
        {
            get { return userId; }
            set
            {
                if (frmType == FormType.Update)
                {
                    userId = value;
                }
            }
        }
        public string UserLogin
        {
            get { return userLogin; }
            set
            {
                if (frmType == FormType.Update)
                {
                    userLogin = value;
                }
            }
        }
        public byte[] UserSalt
        {
            get { return userSalt; }
            set
            {
                if (frmType == FormType.Update)
                {
                    userSalt = value;
                }
            }
        }
        public byte[] UserPassword
        {
            get { return userPassword; }
            set
            {
                if (frmType == FormType.Update)
                {
                    userPassword = value;
                }
            }
        }
        public DateTime DateReg
        {
            get { return dateReg; }
            set
            {
                if (frmType == FormType.Update)
                {
                    dateReg = value;
                }
            }
        }

        private void fReg_Load(object sender, EventArgs e)
        {
            if (frmType == FormType.Update)
            {
                tbLogin.Text = userLogin;
                dtpDate.Value = dateReg;
            }
        }

        public fReg(FormType frmType)
        {
            InitializeComponent();
            InitializeDBUserClass();
            this.frmType = frmType;
            switch (frmType)
            {
                case FormType.SelfMade:
                    btRegister.Text = "Зарегистрировать";
                    this.Text = "Регистрация";
                    dtpDate.Value = DateTime.Today;
                    dtpDate.Enabled = false;
                    break;
                case FormType.Insert:
                    btRegister.Text = "Добавить";
                    this.Text = "Добавление нового пользователя";
                    dtpDate.Value = DateTime.Today;
                    dtpDate.Enabled = false;
                    break;
                case FormType.Update:
                    btRegister.Text = "Изменить";
                    this.Text = "Изменение пользователя";
                    break;
                default:
                    break;
            }
        }

        private void InitializeDBUserClass()
        {
            try
            {
                _dbConrol.TryConnection();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Trouble with DB\n{e.Message}");
                throw e;
            }
        }

        private void tbLogin_TextChanged(object sender, EventArgs e)
        {
            if (!loginCheckRegex.IsMatch(tbLogin.Text))
            {
                epMain.SetError(tbLogin, "Некорректный логин: В логине могут быть использованы символы, изображённые на классической русско-английской раскладке клавиатуре. Длина логина от 6 до 50 символов");
            }
            else
            {
                if (_dbConrol.IsExistsInDBLogin(tbLogin.Text))
                {
                    epMain.SetError(tbLogin, "Логин уже занят");
                }
                else
                {
                    epMain.SetError(tbLogin, "");
                }
            }
            RefreshBtReg();
        }

        private void btRegister_Click(object sender, EventArgs e)
        {
            btRegister.Enabled = false;
            switch (frmType)
            {
                case FormType.SelfMade:

                    _dbConrol.AddNewUser(tbLogin.Text, tbPassword.Text);
                    MessageBox.Show("Регистрация прошла успешно");
                    epMain.SetError(tbLogin, "Логин уже занят");
                    RefreshBtReg();
                    break;

                case FormType.Insert:
                    _dbConrol.AddNewUser(tbLogin.Text, tbPassword.Text, out userId, out userLogin, out userSalt, out userPassword, out dateReg);
                    this.DialogResult = DialogResult.OK;
                    break;
                case FormType.Update:
                    _dbConrol.SetNewData(userId, tbLogin.Text, tbPassword.Text, dtpDate.Value
                        , tbPassword.Text.Trim() == ""
                        , userLogin == _dbConrol.RemoveExtraSpaces(tbLogin.Text)
                        , dateReg == dtpDate.Value
                        , ref userSalt
                        , ref userPassword
                        );
                    this.DialogResult = DialogResult.OK;
                    break;
                default:
                    break;
            }
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            if (frmType == FormType.Update && _dbConrol.RemoveExtraSpaces(tbPassword.Text) == "") { epMain.SetError(tbPassword, ""); return; }
            
            if (!PasswordHandler.PasswordHandler.IsStrongPassword(tbPassword.Text, new List<string> { tbLogin.Text }))
            {
                epMain.SetError(tbPassword, "Слабый пороль");
            }
            else
            {
                epMain.SetError(tbPassword, "");
            }
            RefreshBtReg();
        }

        private void RefreshBtReg()
        {
            if (epMain.GetError(tbPassword) == "" && epMain.GetError(tbLogin) == "")
            {
                btRegister.Enabled = true;
            }
            else
            {
                btRegister.Enabled = false;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
