

namespace Parking.Helpers.Interface
{
    public interface IDialogService 
    {
        object ShowMessage(string message, int type = 1);   // показ сообщения
        string FilePath { get; set; }   // путь к выбранному файлу
        bool OpenFileDialog();  // открытие файла
        bool SaveFileDialog();  // сохранение файла
    }
}