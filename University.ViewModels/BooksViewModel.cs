using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Interfaces;
using University.Models;
using University.Services;

namespace University.ViewModels;

public class BooksViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IDataAccessService _dataAccessService;

    public ObservableCollection<Book> Books { get; }

    public ICommand Add => new RelayCommand(AddNewBook);
    public ICommand Edit => new RelayCommand<string?>(EditBook);
    public ICommand Remove => new RelayCommand<string?>(RemoveBook);

    public bool? DialogResult { get; private set; }

    public BooksViewModel(IDialogService dialogService, IDataAccessService dataAccessService)
    {
        _dialogService = dialogService;
        _dataAccessService = dataAccessService;
        Books = new ObservableCollection<Book>(_dataAccessService.GetEntities<Book>());
    }

    private void AddNewBook()
    {
        var instance = MainWindowViewModel.Instance();
        if (instance != null)
        {
            instance.BooksSubView = new AddBookViewModel(_dialogService, _dataAccessService);
        }
    }

    private void EditBook(string? bookId)
    {
        if (bookId != null)
        {
            var editBookViewModel = new EditBookViewModel(_dialogService, _dataAccessService)
            {
                BookId = bookId
            };
            var instance = MainWindowViewModel.Instance();
            if (instance != null)
            {
                instance.BooksSubView = editBookViewModel;
            }
        }
    }

    private void RemoveBook(string? bookId)
    {
        if (bookId != null)
        {
            var book = _dataAccessService.FindEntity<Book>(bookId);
            if (book != null)
            {
                DialogResult = _dialogService.Show(book.Title);
                if (DialogResult == false)
                {
                    return;
                }
                _dataAccessService.RemoveEntity(book);
                Books.Remove(book);
            }
        }
    }
}
