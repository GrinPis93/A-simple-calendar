using System.Windows;
using ASimpleCalendar.Models;

namespace ASimpleCalendar.Views;

public partial class NoteDialog : Window
{
    public Note? Result { get; private set; }

    public NoteDialog(Note? existing = null)
    {
        InitializeComponent();
        ColorBox.ItemsSource = NoteColorPalette.Items;
        ColorBox.SelectedIndex = 0;

        if (existing is not null)
        {
            TitleBox.Text = existing.Title;
            ContentBox.Text = existing.Content;
            TagsBox.Text = existing.Tags ?? string.Empty;
            PinBox.IsChecked = existing.IsPinned;

            if (existing.Color is not null)
            {
                var index = NoteColorPalette.Items.ToList().IndexOf(existing.Color);
                if (index >= 0)
                {
                    ColorBox.SelectedIndex = index;
                }
            }
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            MessageBox.Show("Введите заголовок заметки.", "ASimpleCalendar", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result = new Note
        {
            Title = TitleBox.Text.Trim(),
            Content = ContentBox.Text,
            Tags = string.IsNullOrWhiteSpace(TagsBox.Text) ? null : TagsBox.Text.Trim(),
            Color = ColorBox.SelectedItem as string,
            IsPinned = PinBox.IsChecked == true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
