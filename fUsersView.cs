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
            var formRegistration = new fReg(fReg.FormType.Insert);
            if (formRegistration.ShowDialog() == DialogResult.OK)
            {
                var lvi = new ListViewItem(new[]
                {
                        formRegistration.UserLogin,
                        Encoding.Default.GetString(formRegistration.UserSalt),
                        Encoding.Default.GetString(formRegistration.UserPassword),
                        formRegistration.DateReg.ToLongDateString()
                    })
                {
                    Tag = Tuple.Create(formRegistration.UserId, formRegistration.DateReg)
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
            foreach (ListViewItem selectedItem in lvUsers.SelectedItems)
            {
                var selectedItemTagAsTuple = (Tuple<int, DateTime>)selectedItem.Tag;
                var formUpdating = new fReg(fReg.FormType.Update)
                {
                    UserId = selectedItemTagAsTuple.Item1,
                    DateReg = selectedItemTagAsTuple.Item2,
                    UserLogin = selectedItem.SubItems[0].Text,
                };
                if (formUpdating.ShowDialog() == DialogResult.OK)
                {
                    selectedItem.SubItems[0].Text = formUpdating.UserLogin;
                    if (formUpdating.UserSalt != null) selectedItem.SubItems[1].Text = Encoding.Default.GetString(formUpdating.UserSalt);
                    if (formUpdating.UserPassword != null) selectedItem.SubItems[2].Text = Encoding.Default.GetString(formUpdating.UserPassword);
                    selectedItem.SubItems[3].Text = formUpdating.DateReg.ToLongDateString();
                    selectedItem.Tag = Tuple.Create(formUpdating.UserId, formUpdating.DateReg);
                }
            }
        }
    }
}
