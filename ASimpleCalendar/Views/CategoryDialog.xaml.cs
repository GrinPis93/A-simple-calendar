using System.Windows;
using ASimpleCalendar.Models;

namespace ASimpleCalendar.Views;

public partial class CategoryDialog : Window
{
    public CategoryItem? Result { get; private set; }

    public CategoryDialog()
    {
        InitializeComponent();
        ColorBox.ItemsSource = NoteColorPalette.Items;
        ColorBox.SelectedIndex = 0;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Введите название категории.", "ASimpleCalendar", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result = new CategoryItem
        {
            Name = NameBox.Text.Trim(),
            Color = ColorBox.SelectedItem as string ?? "#64748B"
        };

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
