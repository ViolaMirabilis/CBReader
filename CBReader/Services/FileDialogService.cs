using CBReader.Interfaces;

namespace CBReader.Services;

public class FileDialogService : IFileDialogService
{
    public string? ChooseComicFolderPath()
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog();
        dialog.Multiselect = false;
        dialog.Title = "Select comics folder";

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            return dialog.FolderName;       // returns full path to the folder
        }
        return null;
    }

    public void OpenComicBookFromFile()
    {
        throw new NotImplementedException();
    }
}
