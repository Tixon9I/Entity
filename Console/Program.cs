using ContextLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static ContextLibrary.Context;

namespace ConsoleLibrary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<LibraryContext>(options =>
                    options.UseSqlServer("Data Source=DESKTOP-T9TKG8K\\SQLEXPRESS;Initial Catalog=Library;Integrated Security=True;Encrypt=False"))
                .BuildServiceProvider();

            Console.WriteLine("Вітаємо у системі бібліотеки!");
            Console.WriteLine("Оберіть дію: 1 - Вхід, 2 - Реєстрація");
            var action = Console.ReadLine();

            if (action == "1")
            {
                Login(serviceProvider);
            }
            else if (action == "2")
            {
                Register(serviceProvider);
            }
        }

        static void Register(ServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<LibraryContext>())
            {
                while (true)
                {
                    Console.WriteLine("Введіть тип користувача (1 - бібліотекар, 2 - читач, 0 - вихід):");
                    var userType = Console.ReadLine();

                    if (userType == "0")
                    {
                        Console.WriteLine("Вихід з реєстрації.");
                        return;
                    }

                    if (userType == "1")
                    {
                        while (true)
                        {
                            Console.WriteLine("Введіть секретний код для бібліотекаря (або введіть '0' для виходу):");
                            var secretCode = Console.ReadLine();

                            const string librarianSecretCode = "LIB123";

                            if (secretCode == "0")
                            {
                                Console.WriteLine("Вихід з реєстрації.");
                                return;
                            }

                            if (secretCode != librarianSecretCode)
                            {
                                Console.WriteLine("Невірний секретний код. Спробуйте ще раз.");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    Console.WriteLine("Введіть логін:");
                    var login = Console.ReadLine();

                    Console.WriteLine("Введіть пароль:");
                    var password = Console.ReadLine();

                    Console.WriteLine("Введіть email:");
                    var email = Console.ReadLine();

                    if (userType == "1")
                    {
                        var employee = new Employee
                        {
                            Login = login,
                            Password = password,
                            Email = email
                        };

                        context.Employees.Add(employee);
                        context.SaveChanges();

                        Console.WriteLine("Бібліотекаря успішно зареєстровано!");
                        break;
                    }
                    else if (userType == "2")
                    {
                        Console.WriteLine("Введіть ім'я:");
                        var firstname = Console.ReadLine();

                        Console.WriteLine("Введіть прізвище:");
                        var lastname = Console.ReadLine();

                        Console.WriteLine("Введіть тип документа (Passport, Driver License, ID Card):");
                        var typeFile = Console.ReadLine();

                        Console.WriteLine("Введіть номер документу:");
                        var identNumDoc = Console.ReadLine();

                        var visitor = new Visitor
                        {
                            Login = login,
                            Password = password,
                            Email = email,
                            Firstname = firstname,
                            Lastname = lastname,
                            TypeFile = typeFile,
                            IdentNumDoc = identNumDoc
                        };

                        context.Visitors.Add(visitor);
                        context.SaveChanges();

                        Console.WriteLine("Читача успішно зареєстровано!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Невірний тип користувача. Спробуйте ще раз.");
                    }
                }
            }
        }

        static void Login(ServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<LibraryContext>())
            {
                while (true)
                {
                    Console.WriteLine("Введіть логін (або введіть '0' для виходу):");
                    var login = Console.ReadLine();

                    if (login == "0")
                    {
                        Console.WriteLine("Вихід з авторизації.");
                        return;
                    }

                    Console.WriteLine("Введіть пароль:");
                    var password = Console.ReadLine();

                    var employee = context.Employees
                        .FirstOrDefault(e => e.Login == login && e.Password == password);

                    if (employee != null)
                    {
                        Console.WriteLine($"Вітаємо, {employee.Login}! Ви увійшли як бібліотекар.");
                        LibrarianMenu(serviceProvider);
                        return;
                    }

                    var visitor = context.Visitors
                        .FirstOrDefault(v => v.Login == login && v.Password == password);

                    if (visitor != null)
                    {
                        Console.WriteLine($"Вітаємо, {visitor.Firstname} {visitor.Lastname}! Ви увійшли як читач.");
                        ReaderMenu(serviceProvider, visitor);
                        return;
                    }

                    Console.WriteLine("Невірний логін або пароль. Спробуйте ще раз або введіть '0' для виходу.");
                }
            }
        }


        static void ReaderMenu(ServiceProvider serviceProvider, Visitor visitor)
        {
            Console.WriteLine("Функціонал для читача:");
            Console.WriteLine("1 - Пошук книг");
            Console.WriteLine("2 - Взяти книгу");
            Console.WriteLine("3 - Переглянути свої книги");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                SearchBooks(serviceProvider);
            }
            else if (choice == "2")
            {
                TakeBook(serviceProvider, visitor);
            }
            else if (choice == "3")
            {
                ViewBorrowedBooks(serviceProvider, visitor);
            }
        }

        static void SearchBooks(ServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<LibraryContext>())
            {
                Console.WriteLine("Введіть назву книги або автора для пошуку:");
                var searchTerm = Console.ReadLine();

                var books = context.Books
                    .Include(b => b.AuthorBooks)
                    .ThenInclude(ab => ab.Author)
                    .Where(b => b.Title.Contains(searchTerm) ||
                                b.AuthorBooks.Any(ab => ab.Author.Firstname.Contains(searchTerm) ||
                                                        ab.Author.Lastname.Contains(searchTerm)))
                    .ToList();

                Console.WriteLine("Результати пошуку:");
                foreach (var book in books)
                {
                    foreach (var authorBook in book.AuthorBooks)
                    {
                        var author = authorBook.Author;
                        Console.WriteLine($"- {book.Title} від {author.Firstname} {author.Lastname}");
                    }
                }
            }
        }


        static void TakeBook(ServiceProvider serviceProvider, Visitor visitor)
        {
            using (var context = serviceProvider.GetService<LibraryContext>())
            {
                Console.WriteLine("Введіть ID книги, яку хочете взяти:");
                var bookId = int.Parse(Console.ReadLine());

                var book = context.Books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                {
                    Console.WriteLine("Книга не знайдена.");
                    return;
                }

                var borrowedBook = new BorrowedBook
                {
                    VisitorId = visitor.Id,
                    BookId = bookId,
                    BorrowedDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(30)
                };

                context.BorrowedBooks.Add(borrowedBook);
                context.SaveChanges();

                Console.WriteLine($"Книгу {book.Title} успішно взято! Дата повернення: {borrowedBook.DueDate.ToShortDateString()}");
            }
        }

        static void ViewBorrowedBooks(ServiceProvider serviceProvider, Visitor visitor)
        {
            using (var context = serviceProvider.GetService<LibraryContext>())
            {
                var borrowedBooks = context.BorrowedBooks
                    .Include(bb => bb.Book)
                    .Where(bb => bb.VisitorId == visitor.Id)
                    .OrderBy(bb => bb.DueDate)
                    .ToList();

                Console.WriteLine("Список ваших взятих книг:");
                foreach (var borrowedBook in borrowedBooks)
                {
                    var color = borrowedBook.DueDate < DateTime.Now ? ConsoleColor.Red : ConsoleColor.White;
                    Console.ForegroundColor = color;
                    Console.WriteLine($"- {borrowedBook.Book.Title} (Повернення до {borrowedBook.DueDate.ToShortDateString()})");
                    Console.ResetColor();
                }
            }
        }

        static void LibrarianMenu(ServiceProvider serviceProvider)
        {
            Console.WriteLine("Функціонал для бібліотекаря:");
            Console.WriteLine("1 - Перегляд всіх книг");
            Console.WriteLine("2 - Додати/оновити книгу");
            Console.WriteLine("3 - Перегляд читачів");
            Console.WriteLine("4 - Перегляд всіх позичених книг");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                ViewAllBooks(serviceProvider);
            }
            else if (choice == "2")
            {
                AddOrUpdateBook(serviceProvider);
            }
            else if (choice == "3")
            {
                ViewAllVisitors(serviceProvider);
            }
            else if (choice == "4")
            {
                ViewAllBorrowedBooks(serviceProvider);
            }
        }

        static void ViewAllBooks(ServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<LibraryContext>())
            {
                var books = context.Books.Include(b => b.Author).ToList();
                Console.WriteLine("Усі книги в бібліотеці:");

                foreach (var book in books)
                {
                    var title = book.Title ?? "Без назви";
                    var authorFirstName = book.Author?.Firstname ?? "Невідомий";
                    var authorLastName = book.Author?.Lastname ?? "";

                    Console.WriteLine($"- {title} від {authorFirstName} {authorLastName}");
                }
            }
        }


        static void AddOrUpdateBook(ServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<LibraryContext>())
            {
                Console.WriteLine("Введіть ID книги для оновлення (або натисніть Enter для додавання нової):");
                var input = Console.ReadLine();

                Book book;

                if (int.TryParse(input, out int bookId))
                {
                    book = context.Books.Include(b => b.Author).FirstOrDefault(b => b.Id == bookId);
                    if (book == null)
                    {
                        Console.WriteLine("Книга не знайдена.");
                        return;
                    }
                    Console.WriteLine("Оновлення книги...");
                }
                else
                {
                    book = new Book();
                    Console.WriteLine("Додавання нової книги...");
                }

                Console.WriteLine("Введіть назву книги:");
                book.Title = Console.ReadLine();

                Console.WriteLine("Введіть ID автора:");
                book.AuthorId = int.Parse(Console.ReadLine());

                if (book.Id > 0)
                {
                    context.Books.Update(book);
                    Console.WriteLine("Книгу оновлено!");
                }
                else
                {
                    context.Books.Add(book);
                    Console.WriteLine("Книгу додано!");
                }

                context.SaveChanges();
            }
        }

        static void ViewAllVisitors(ServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<LibraryContext>())
            {
                var visitors = context.Visitors.ToList();
                Console.WriteLine("Усі зареєстровані читачі:");

                foreach (var visitor in visitors)
                {
                    Console.WriteLine($"- {visitor.Firstname} {visitor.Lastname}, Логін: {visitor.Login}");
                }
            }
        }


        static void ViewAllBorrowedBooks(ServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<LibraryContext>())
            {
                var borrowedBooks = context.BorrowedBooks
                    .Include(bb => bb.Book)
                    .Include(bb => bb.Visitor)
                    .ToList();

                Console.WriteLine("Всі позичені книги:");
                foreach (var borrowedBook in borrowedBooks)
                {
                    var returnDate = borrowedBook.ReturnDate.HasValue ? borrowedBook.ReturnDate.Value.ToShortDateString() : "Не повернута";
                    Console.WriteLine($"- {borrowedBook.Book.Title} взята {borrowedBook.Visitor.Firstname} {borrowedBook.Visitor.Lastname} на {borrowedBook.DueDate.ToShortDateString()}, повернута: {returnDate}");
                }
            }
        }
    }
}
