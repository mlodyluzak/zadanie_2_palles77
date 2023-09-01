using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using Telerik.JustMock;

namespace University.Services.Tests
{
    [TestClass]
    public class DataAccessServiceTest
    {
        private UniversityContext _context;
        private IDialogService _dialogService;
        private const string TestDataFileName = "data.json";

        [TestInitialize]
        public void Initialize()
        {
            // Konfiguracja kontekstu bazy danych w pamięci
            var options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "UniversityTestDB")
                .Options;

            _context = new UniversityContext(options);
            SeedTestDatabase();
            _dialogService = new DialogService();
        }

        private void SeedTestDatabase()
        {
            // Dodawanie danych testowych do bazy danych w pamięci
            var students = new List<Student>
            {
                new Student { StudentId = "1", Name = "Wieńczysław", LastName = "Nowakowicz", PESEL="PESEL1", BirthDate = new DateTime(1987, 05, 22) },
                new Student { StudentId = "2", Name = "Stanisław", LastName = "Nowakowicz", PESEL = "PESEL2", BirthDate = new DateTime(2019, 06, 25) },
                new Student { StudentId = "3", Name = "Eugenia", LastName = "Nowakowicz", PESEL = "PESEL3", BirthDate = new DateTime(2021, 06, 08) }
            };

            var courses = new List<Course>
            {
                new Course { CourseCode = "MAT", Title = "Matematyka", Instructor = "Michalina Beldzik", Schedule = "schedule1", Description = "description1", Credits = 5, Department = "department1" },
                new Course { CourseCode = "BIOL", Title = "Biologia", Instructor = "Halina", Schedule = "schedule2", Description = "description2", Credits = 6, Department = "department3" },
                new Course { CourseCode = "CHEM", Title = "Chemia", Instructor = "Jan Nowak", Schedule = "schedule3", Description = "description3", Credits = 7, Department = "department3" }
            };

            var books = new List<Book>
            {
                new Book { BookId = "B0001", Title = "Brave New World", Author = "Aldous Huxley", ISBN = "978-0060850524", Publisher = "Harper Perennial", PublicationDate = new DateTime(2006, 11, 26), Description = "Description... ", Genre = "Novel" },
                new Book { BookId = "B0002", Title = "The Shining", Author = "Stephen King", ISBN = "978-0307743657", Publisher = "Random House US", PublicationDate = new DateTime(2012, 1, 1), Description = "Description 2... ", Genre = "Novel" }
            };

            var exams = new List<Exam>
            {
                new Exam { ExamId = "E001", CourseCode = "MAT", Date = new DateTime(2024, 10, 1), StartTime = new DateTime(2000, 1, 1, 9, 0, 0), EndTime = new DateTime(2000, 1, 1, 11, 0, 0), Description = "Final exam", Location = "location1", Professor = "Michalina Warszawa" }
            };

            _context.Students.AddRange(students);
            _context.Courses.AddRange(courses);
            _context.Books.AddRange(books);
            _context.Exams.AddRange(exams);
            _context.SaveChanges();
        }

        [TestMethod]
        public void SaveDataToFile_ShouldCreateFile()
        {
            // Arrange
            IDataAccessService dataAccessService = new DataAccessService(_context);
            var expectedFileName = "testData.json";

            // Act
            dataAccessService.SaveDataToFile(expectedFileName);

            // Assert
            Assert.IsTrue(File.Exists(expectedFileName));
        }

        [TestMethod]
        public void LoadDataFromFile_ShouldLoadDataToDatabase()
        {
            // Arrange
            var dataAccessServiceMock = Mock.Create<IDataAccessService>();
            Mock.Arrange(() => dataAccessServiceMock.SaveDataToFile(Arg.AnyString)).DoInstead(
                () =>
                {
                    const string fileContent = "[{\"StudentId\":\"1\",\"Name\":\"Wie\\u0144czys\\u0142aw\",\"LastName\":\"Nowakowicz\",\"PESEL\":\"PESEL1\",\"BirthDate\":\"1987-05-22T00:00:00\",\"BirthPlace\":\"\",\"ResidencePlace\":\"\",\"AddressLine1\":\"\",\"AddressLine2\":\"\",\"Courses\":null,\"Exams\":null},{\"StudentId\":\"2\",\"Name\":\"Stanis\\u0142aw\",\"LastName\":\"Nowakowicz\",\"PESEL\":\"PESEL2\",\"BirthDate\":\"2019-06-25T00:00:00\",\"BirthPlace\":\"\",\"ResidencePlace\":\"\",\"AddressLine1\":\"\",\"AddressLine2\":\"\",\"Courses\":null,\"Exams\":null},{\"StudentId\":\"3\",\"Name\":\"Eugenia\",\"LastName\":\"Nowakowicz\",\"PESEL\":\"PESEL3\",\"BirthDate\":\"2021-06-08T00:00:00\",\"BirthPlace\":\"\",\"ResidencePlace\":\"\",\"AddressLine1\":\"\",\"AddressLine2\":\"\",\"Courses\":null,\"Exams\":null}]\r\n[{\"CourseCode\":\"MAT\",\"Title\":\"Matematyka\",\"Instructor\":\"Michalina Beldzik\",\"Schedule\":\"schedule1\",\"Description\":\"description1\",\"Credits\":5,\"Department\":\"department1\",\"IsSelected\":false},{\"CourseCode\":\"BIOL\",\"Title\":\"Biologia\",\"Instructor\":\"Halina\",\"Schedule\":\"schedule2\",\"Description\":\"description2\",\"Credits\":6,\"Department\":\"department3\",\"IsSelected\":false},{\"CourseCode\":\"CHEM\",\"Title\":\"Chemia\",\"Instructor\":\"Jan Nowak\",\"Schedule\":\"schedule3\",\"Description\":\"description3\",\"Credits\":7,\"Department\":\"department3\",\"IsSelected\":false}]\r\n[{\"ExamId\":\"E001\",\"CourseCode\":\"MAT\",\"Date\":\"2024-10-01T";
                    File.WriteAllText(TestDataFileName, fileContent);
                }
            );

            // Act
            dataAccessServiceMock.SaveDataToFile(TestDataFileName);
            dataAccessServiceMock.ReadDataFromFile(TestDataFileName);

            // Assert
            Assert.IsTrue(_context.Students.Count() == 3 && _context.Courses.Count() == 3 && _context.Books.Count() == 2 && _context.Exams.Count() == 1);
        }
    }
}
