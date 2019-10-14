using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADOMET_DZ006_Rastvorov
{
    public partial class Form1 : Form
    {
        DataTable dtBooks;
        DataTable dtAuthors;
        DataTable dtPublishers;
        Form addForm;
        public string TabControlPageName { set; get; }

        public Form1()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {   
            using (LibraryEntities db = new LibraryEntities())
            {
                // заполняем таблицу вкладки Authors
                dtBooks = new DataTable();
                dtBooks.Columns.Add("Id");
                dtBooks.Columns.Add("Title");
                dtBooks.Columns.Add("IdAuthor");
                dtBooks.Columns.Add("Pages");
                dtBooks.Columns.Add("Price");
                dtBooks.Columns.Add("IdPublisher");

                var bks = db.Book.ToList();
                foreach (var b in bks)
                {
                    DataRow dr = dtBooks.NewRow();
                    dr["Id"] = b.Id;
                    dr["Title"] = b.Title;
                    dr["IdAuthor"] = b.IdAuthor;
                    dr["Pages"] = b.Pages;
                    dr["Price"] = b.Price;
                    dr["IdPublisher"] = b.IdPublisher;
                    dtBooks.Rows.Add(dr);
                }
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = dtBooks;

                // заполняем таблицу вкладки Authors
                dtAuthors = new DataTable();
                dtAuthors.Columns.Add("Id");
                dtAuthors.Columns.Add("FirstName");
                dtAuthors.Columns.Add("LastName");

                var au = db.Author.ToList();
                foreach (var a in au)
                {
                    DataRow dr = dtAuthors.NewRow();
                    dr["Id"] = a.Id;
                    dr["FirstName"] = a.FirstName;
                    dr["LastName"] = a.LastName;
                    dtAuthors.Rows.Add(dr);
                }
                dataGridView2.DataSource = null;
                dataGridView2.DataSource = dtAuthors;

                // заполняем таблицу вкладки Publisher
                dtPublishers = new DataTable();
                dtPublishers.Columns.Add("Id");
                dtPublishers.Columns.Add("PublisherName");
                dtPublishers.Columns.Add("Address");

                var pb = db.Publisher.ToList();
                foreach (var p in pb)
                {
                    DataRow dr = dtPublishers.NewRow();
                    dr["Id"] = p.Id;
                    dr["PublisherName"] = p.PublisherName;
                    dr["Address"] = p.Address;
                    dtPublishers.Rows.Add(dr);
                }
                dataGridView3.DataSource = null;
                dataGridView3.DataSource = dtPublishers;
            }

        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {         

            addForm = new Form();
            
            addForm.Size = new Size(400, 550);
            Label addLabel = new Label();
            addLabel.Name = "NameAddLabel";
            addLabel.Location = new Point(22, 12);
            addLabel.AutoSize = true;
            addLabel.Font = new Font("Arial", 21, FontStyle.Bold);
            
            switch ((sender as DataGridView).Parent.Name)
            {
                case "BooksPage":
                    addLabel.Text = "ADD NEW BOOK:";
                    TabControlPageName = "BooksPage";
                    break;
                case "AuthorsPage":
                    addLabel.Text = "ADD NEW AUTHOR:";
                    TabControlPageName = "AuthorsPage";
                    break;
                case "PublishersPage":
                    addLabel.Text = "ADD NEW PUBLISHER:";
                    TabControlPageName = "PublishersPage";
                    break;
            }

            DataTable addTable = new DataTable();
            DataGridView addDGr = new DataGridView();
            addDGr = sender as DataGridView;
            int shag = 22;
            foreach (DataGridViewColumn column in addDGr.Columns)
            {
                Label label = new Label();
                TextBox textbox = new TextBox();                
                shag += 60;
                label.Location = new Point(22, shag);
                label.AutoSize = true;
                label.Text = column.HeaderText;
                label.Font = new Font("Arial", 18, FontStyle.Bold);
                addForm.Controls.Add(label);
                textbox.Location = new Point(22, shag + 30);
                textbox.Size = new Size(300, 50);
                textbox.Font = new Font("Arial", 16, FontStyle.Bold);
                textbox.Name = column.HeaderText;
                addForm.Controls.Add(textbox);
            }

            Button addButton = new Button();
            addButton.Text = "Add";
            addButton.AutoSize = true;
            addButton.Font = new Font("Arial", 14, FontStyle.Bold);
            addButton.Location = new Point(22, 50);            
            addForm.Controls.Add(addButton);
            addButton.Click += AddButton_Click;

            //addForm.Controls.Add(addDGr);
            addForm.Controls.Add(addLabel);            
            addForm.Show(this);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            switch (TabControlPageName)
            {
                case "BooksPage":
                    Book book = new Book();
                    foreach (var tb in addForm.Controls.OfType<TextBox>())
                    {
                        if (tb.Name == "Title") book.Title = tb.Text;
                        if (tb.Name == "IdPublisher") book.IdPublisher = Convert.ToInt32(tb.Text);
                        if (tb.Name == "IdAuthor") book.IdAuthor = Convert.ToInt32(tb.Text);// Convert.ToInt32(tb.Name.ToString());
                        if (tb.Name == "Pages") book.Pages = Convert.ToInt32(tb.Text);
                        if (tb.Name == "Price") book.Price = Convert.ToInt32(tb.Text);
                    }   
                    AddBook(book);
                    addForm.Close();
                    LoadData();
                    break;
                case "AuthorsPage":
                    Author author = new Author();
                    foreach (var a in addForm.Controls.OfType<TextBox>())
                    {
                        if (a.Name == "FirstName") author.FirstName = a.Text;
                        if (a.Name == "LastName") author.LastName = a.Text;                        
                    }
                    AddAuthor(author);
                    addForm.Close();
                    LoadData();
                    break;
                case "PublishersPage":
                    Publisher publisher = new Publisher();
                    foreach (var p in addForm.Controls.OfType<TextBox>())
                    {
                        if (p.Name == "PublisherName") publisher.PublisherName = p.Text;
                        if (p.Name == "Address") publisher.Address = p.Text;
                    }
                    AddPublisher(publisher);
                    addForm.Close();
                    LoadData();
                    break;
            }
            
        }

        static void AddBook(Book book)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                Book a = db.Book.Where((x) => x.Title == book.Title).FirstOrDefault();
                if (a == null)
                {
                    db.Book.Add(book);
                }
                db.SaveChanges();                
            }
        }

        static void AddAuthor(Author author)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                Author au = db.Author.Where((x) => x.FirstName == author.FirstName).FirstOrDefault();
                if (au == null)
                {
                    db.Author.Add(author);
                }
                db.SaveChanges();
            }
        }

        static void AddPublisher(Publisher publisher)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                Author p = db.Author.Where((x) => x.FirstName == publisher.PublisherName).FirstOrDefault();
                if (p == null)
                {
                    db.Publisher.Add(publisher);
                }
                db.SaveChanges();
            }
        }
    }

}
