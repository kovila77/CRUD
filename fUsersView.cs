using DBUsersHandler;
using Registration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUD
{
    public partial class fUsersView : Form
    {
        private DBUsersHandler.DBUsersHandler _dbConrol = new DBUsersHandler.DBUsersHandler();

        public fUsersView()
        {
            InitializeComponent();
            InitializeLVUsers();
        }

        private void InitializeLVUsers()
        {
            lvUsers.Clear();
            lvUsers.Columns.Add("Логин");
            lvUsers.Columns.Add("Соль");
            lvUsers.Columns.Add("Пароль");
            lvUsers.Columns.Add("Дата регистрации");
            using (var extractor = _dbConrol.CreateDataExtractor())
            {
                string login;
                byte[] salt;
                byte[] password;
                DateTime createDate;
                int id;
                while (extractor.Read())
                {
                    extractor.TakeRow(out id, out login, out salt, out password, out createDate);
                    var lvi = new ListViewItem(new[]
                    {
                        login,
                        Encoding.Default.GetString(salt),
                        Encoding.Default.GetString(password),
                        createDate.ToLongDateString()
                    })
                    {
                        Tag = Tuple.Create(id, createDate)
                    };
                    lvUsers.Items.Add(lvi);
                }
            }

            lvUsers.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvUsers.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formRegistration = new fReg();
            if (formRegistration.ShowDialog() == DialogResult.OK)
            {
                    var lvi = new ListViewItem(new[]
                    {
                        formRegistration.userLogin,
                        Encoding.Default.GetString(formRegistration.userSalt),
                        Encoding.Default.GetString(formRegistration.userPassword),
                        formRegistration.dateReg.ToLongDateString()
                    })
                    {
                        Tag = Tuple.Create(formRegistration.userId, formRegistration.dateReg)
                    };
                    lvUsers.Items.Add(lvi);                
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
