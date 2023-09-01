using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;

namespace University.Tests
{
    [TestClass]
    public class BooksTest
    {
        private IDataAccessService _dataAccessService;
        private DbContextOptions<UniversityContext> _options;

        [TestInitialize()]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "UniversityTestDB")
                .Options;
            SeedTestDB();
            _dataAccessService = new DataAccessService(new UniversityContext(_options));
        }

        private void SeedTestDB()
        {
            using var context = new UniversityContext(_options);
            context.Database.EnsureDeleted();

            var testBook = new Book
            {
                BookId = "B0001",
                Title = "Brave New World",
                Author = "Aldous Huxley",
                ISBN = "978-0060850524",
                Publisher = "Harper Perennial",
                PublicationDate = new DateTime(2006, 11, 26),
                Description = "Description... ",
                Genre = "Novel"
            };

            context.Books.Add(testBook);
            context.SaveChanges();
        }

        [TestMethod]
        public void BooksViewModel_ShouldHaveData()
        {
            var dialogServiceMock = new Mock<IDialogService>();
            var booksViewModel = new BooksViewModel(dialogServiceMock.Object, _dataAccessService);
            var hasData = booksViewModel.Books.Any();
            Assert.IsTrue(hasData);
        }

        [TestMethod]
        public void AddBookViewModel_ShouldAddNewBook()
        {
            var dialogServiceMock = new Mock<IDialogService>();
            var dataAccessServiceMock = new Mock<IDataAccessService>();

            var newBook = new Book
            {
                BookId = "B0002",
                Title = "The Shining",
                Author = "Stephen King",
                ISBN = "978-0307743657",
                Publisher = "Random House US",
                PublicationDate = new DateTime(2012, 1, 1),
                Description = "Description 2... ",
                Genre = "Novel"
            };

            var addBookViewModel = new AddBookViewModel(dialogServiceMock.Object, dataAccessServiceMock.Object)
            {
                Title = newBook.Title,
                BookId = newBook.BookId,
                Author = newBook.Author,
                Publisher = newBook.Publisher,
                PublicationDate = newBook.PublicationDate,
                ISBN = newBook.ISBN,
                Genre = newBook.Genre,
                Description = newBook.Description
            };

            addBookViewModel.Save.Execute(null);

            var booksViewModel = new BooksViewModel(dialogServiceMock.Object, _dataAccessService);
            var hasData = booksViewModel.Books.Any(book => book.ISBN == newBook.ISBN);
            Assert.IsTrue(hasData);
        }
    }
}
