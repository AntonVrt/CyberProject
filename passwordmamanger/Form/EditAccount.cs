﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;

namespace passwordmamanger
{
    public partial class EditAccount : Form
    {
        Crypto Crip = new Crypto();
        Db DataBase = new Db();
        IMongoCollection<UserInfo> collectionUser;
        UserInfo user;
        public EditAccount(UserInfo User)
        {
            InitializeComponent();
            user = User;          
            PasswordBox.Text = Crip.decrypt(user.Password);
            NameBox.Text = user.FirstName;
            EmailBox.Text = user.Email;
            collectionUser = DataBase.getcollectionUser();
        }

      
        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void EditAccount_Load(object sender, EventArgs e)
        {

        }

        private void username_textbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void PasswordBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void DeleteAccBtn_Click(object sender, EventArgs e)
        {
            var deleteFilter = Builders<UserInfo>.Filter.Eq("UserName", user.UserName);
            collectionUser.DeleteOne(deleteFilter);
            Login newForm = new Login();
            this.Hide();
            newForm.ShowDialog();
            this.Close();
        }

        private void EditAccBtn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(EmailBox.Text) || String.IsNullOrEmpty(NameBox.Text) || String.IsNullOrEmpty(PasswordBox.Text))
            {
                MessageBox.Show("אסור להשאיר שדה ריק");
                return;
            }

            var filter = Builders<UserInfo>.Filter.Eq("UserName", user.UserName);
            UpdateDefinition<UserInfo> Emailupdate = Builders<UserInfo>.Update.Set(x => x.Email, EmailBox.Text);
            UpdateDefinition<UserInfo> Nameupdate = Builders<UserInfo>.Update.Set(x => x.FirstName, NameBox.Text);
            if (CheckPassword(PasswordBox.Text))
            {
                UpdateDefinition<UserInfo> Passwordupdate = Builders<UserInfo>.Update.Set(x => x.Password, Crip.Encrypt(PasswordBox.Text));
                
                collectionUser.UpdateOneAsync(filter, Passwordupdate);
            }
            else
            {
                MessageBox.Show("הסיסמא לא תקינה\n סיסמה – בין 6 ל 10 תווים. מכיל לפחות אות אחת, סיפרה אחת ותו מיוחד אחד )!,#,$ וכו'(.", "אזהרה", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            collectionUser.UpdateOneAsync(filter, Emailupdate);
            collectionUser.UpdateOneAsync(filter, Nameupdate);
            
            MessageBox.Show("The account is successfully Update ", "Account Update", MessageBoxButtons.OK, MessageBoxIcon.Information);

            MainForm newForm = new MainForm(user);
            this.Hide();
            newForm.ShowDialog();
            this.Close();
        }
        public bool CheckPassword(string password)
        {
            if (password.Length < 6 || password.Length > 10)
                return false;
            string digits = new String(password.Where(Char.IsDigit).ToArray());
            string letters = new String(password.Where(Char.IsLetter).ToArray());
            password.Where(c => !char.IsLetterOrDigit(c));
            if (digits.Length == 0 || letters.Length == 0 || password.Length - digits.Length - letters.Length == 0)
                return false;
            return true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}