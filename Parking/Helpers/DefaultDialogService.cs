using System.Windows;
using Microsoft.Win32;
using Parking.Helpers.Interface;

namespace Parking.Helpers
{
    public class DefaultDialogService : IDialogService
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        public object ShowMessage(string message, int type=1)
        {
            switch (type)
            {
                case 1:
                    return MessageBox.Show(message, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                case 2:
                    return MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);

                case 3:
                    return MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                case 4:
                    return MessageBox.Show(message, "Решение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                default:
                    return MessageBox.Show(message);
              
            }           
        }
    }
}