// Создайте Windows Forms приложение для работы с БД с использованием Entity Framework 
// по технологии Database First. В главном окне приложения должен содержаться элемент 
// TabControl с тремя вкладками: Books, Authors и Publishers. Каждая вкладка должна 
// обслуживать одну из таблиц нашей БД и выполнять добавление, удаление и редактирование 
// записей в своей таблице.
// Для добавления, редактирования и удаления записей использовать формы.

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ADOMET_DZ006_Rastvorov
{
    public partial class Form1 : Form
    {
        DataTable dtBooks; // рабочая таблица для хранения данных из таблицы базы Book 
        DataTable dtAuthors; // рабочая таблица для хранения данных из таблицы базы Authors 
        DataTable dtPublishers; // рабочая таблица для хранения данных из таблицы базы Publisher 
        Form addForm; // форма для добавления и редактирования записей

        public string TabControlPageName { set; get; }

        public Form1()
        {
            InitializeComponent();
            LoadData();
        }
        // загрузка данных из базы
        public void LoadData()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                // заполняем таблицу вкладки Books
                // формируем колонки
                dtBooks = new DataTable();
                dtBooks.Columns.Add("Id");
                dtBooks.Columns.Add("Title");
                dtBooks.Columns.Add("IdAuthor");
                dtBooks.Columns.Add("Pages");
                dtBooks.Columns.Add("Price");
                dtBooks.Columns.Add("IdPublisher");
                // загружаем данные из базы
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
                // чистим датагрид
                dataGridView1.DataSource = null;
                // закидываем рабочую таблицу на датагрид
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
        // при попытке пользователя ввести данные в новой строке датагрида, запускаем форму 
        // добавления данных в базу
        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            // создаем форму
            addForm = new Form();
            addForm.Size = new Size(400, 550);
            Label addLabel = new Label();
            addLabel.Name = "NameAddLabel";
            addLabel.Location = new Point(22, 12);
            addLabel.AutoSize = true;
            addLabel.Font = new Font("Arial", 21, FontStyle.Bold);
            // проверяем на какой вкладке пользователь и формируем заголовок формы
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
            // закидываем на форму в автоматическом режиме список полей текущей вкладки
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
                if (label.Text == "Id") textbox.Enabled = false;
                addForm.Controls.Add(textbox);
            }
            // создаем кнопку Добавить
            Button addButton = new Button();
            addButton.Text = "Add";
            addButton.AutoSize = true;
            addButton.Font = new Font("Arial", 14, FontStyle.Bold);
            addButton.Location = new Point(22, 50);
            addForm.Controls.Add(addButton);
            addButton.Click += AddButton_Click;
            addForm.Controls.Add(addLabel);
            // запускаем форму
            addForm.ShowDialog(this);
        }
        // обработка события добавления новой строки данных в базу
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
        // функция добавления в новой строки в таблицу Book базы данных
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
        // функция удаления строки из таблицу Book базы данных
        static void DelBook(int id)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                Book a = db.Book.Where((x) => x.Id == id).FirstOrDefault();
                if (a != null)
                {
                    db.Book.Remove(a);
                }
                db.SaveChanges();
            }
        }
        // функция добавления в новой строки в таблицу Author базы данных
        static void AddAuthor(Author author)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                Author au = db.Author.Where((x) => (x.FirstName + x.LastName) == (author.FirstName + author.LastName)).FirstOrDefault();
                if (au == null)
                {
                    db.Author.Add(author);
                }
                db.SaveChanges();
            }
        }
        // функция удаления строки из таблицу Book базы данных
        static void DelAuthor(int id)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                Author a = db.Author.Where((x) => x.Id == id).FirstOrDefault();
                if (a != null)
                {
                    db.Author.Remove(a);
                }
                db.SaveChanges();
            }
        }
        // функция добавленя в новой строки в таблицу Publisher базы данных
        static void AddPublisher(Publisher publisher)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                Publisher p = db.Publisher.Where((x) => x.PublisherName == publisher.PublisherName).FirstOrDefault();
                if (p == null)
                {
                    db.Publisher.Add(publisher);
                }
                db.SaveChanges();
            }
        }
        // функция удаления строки из таблицу Publisher базы данных
        static void DelPublisher(int id)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                Publisher a = db.Publisher.Where((x) => x.Id == id).FirstOrDefault();
                if (a != null)
                {
                    db.Publisher.Remove(a);
                }
                db.SaveChanges();
            }
        }
        // обработка события выделения ползователем в датагриде сроки и нажатии клавиши Del
        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            int id;
            switch ((sender as DataGridView).Parent.Name)
            {
                case "BooksPage":
                    id = Convert.ToInt32(dataGridView1.SelectedCells[0].Value);
                    DelBook(id);
                    break;
                case "AuthorsPage":
                    id = Convert.ToInt32(dataGridView2.SelectedCells[0].Value);
                    DelAuthor(id);
                    break;
                case "PublishersPage":
                    id = Convert.ToInt32(dataGridView3.SelectedCells[0].Value);
                    DelPublisher(id);
                    break;
            }
            LoadData();
        }
        // отработка события выделения польователем ячейки с данными для их редактирования
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // создаем форму для редактирования
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
                    // Проверяем то, что это не новая строка
                    if (e.RowIndex == dataGridView1.NewRowIndex) return;
                    addLabel.Text = "EDIT BOOK:";
                    TabControlPageName = "BooksPage";
                    break;
                case "AuthorsPage":
                    // Проверяем то, что это не новая строка
                    if (e.RowIndex == dataGridView2.NewRowIndex) return;
                    addLabel.Text = "EDIT AUTHOR:";
                    TabControlPageName = "AuthorsPage";
                    break;
                case "PublishersPage":
                    // Проверяем то, что это не новая строка
                    if (e.RowIndex == dataGridView3.NewRowIndex) return;
                    addLabel.Text = "EDIT PUBLISHER:";
                    TabControlPageName = "PublishersPage";
                    break;
            }
            // заполняем созданную форму текстбоксами с названиями и значением редактирумой строки
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
                // присваем свойству Text значение ячейки из DataGridView
                textbox.Text = addDGr.CurrentRow.Cells[column.HeaderText].Value.ToString();
                if (label.Text == "Id") textbox.Enabled = false;
                addForm.Controls.Add(textbox);
            }
            // создаем кнопку подтверджающую ввод отредактированной информации
            Button addButton = new Button();
            addButton.Text = "Applay";
            addButton.AutoSize = true;
            addButton.Font = new Font("Arial", 14, FontStyle.Bold);
            addButton.Location = new Point(22, 50);
            addForm.Controls.Add(addButton);
            addButton.Click += EditButton_Click;

            addForm.Controls.Add(addLabel);
            // запускаем форму
            addForm.ShowDialog(this);
        }
        // обработка события подтверждения ввода отредактированной информации
        private void EditButton_Click(object sender, EventArgs e)
        {
            switch (TabControlPageName)
            {
                case "BooksPage":
                    using (LibraryEntities db = new LibraryEntities())
                    {
                        List<TextBox> booklist = addForm.Controls.OfType<TextBox>().ToList();
                        // ищем Id редактируем книжки среди контроллов формы для редактирования
                        int id = Convert.ToInt32((booklist.Find(item => item.Name == "Id")).Text);
                        Book a = db.Book.Where((x) => x.Id == id).FirstOrDefault();
                        if (a != null)
                        {
                            foreach (var t in addForm.Controls.OfType<TextBox>())
                            {
                                if (t.Name == "Title") a.Title = t.Text;
                                if (t.Name == "IdPublisher") a.IdPublisher = Convert.ToInt32(t.Text);
                                if (t.Name == "IdAuthor") a.IdAuthor = Convert.ToInt32(t.Text);// Convert.ToInt32(tb.Name.ToString());
                                if (t.Name == "Pages") a.Pages = Convert.ToInt32(t.Text);
                                if (t.Name == "Price") a.Price = Convert.ToInt32(t.Text);
                            }
                        }
                        db.SaveChanges();
                    }
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
    }
}
