using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;
using Web_Service.Models;
using Client_WPF.Models;

namespace Client_WPF.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel()
        {
            db = new ParkingdbContext();
            dbSize = db.Parks.Count();

            Parks = new ObservableCollection<ParkingInfo>();
        }

        private readonly ParkingdbContext db;
        public int dbSize { get; private set; }
        public ObservableCollection<ParkingInfo> Parks { get; set; }

        private ParkingInfo selectedPark;
        public ParkingInfo SelectedPark
        {
            get { return selectedPark; }
            set
            {
                selectedPark = value;
                OnPropertyChanged("SelectedPark");
            }
        }

        // команда подгрузки данных в список
        private RelayCommand downCommand;
        public RelayCommand DownCommand
        {
            get
            {
                return downCommand ??
                  (downCommand = new RelayCommand(obj =>
                  {
                      int sizeList = (int)obj;

                      if (sizeList == 0)
                          sizeList = 1;

                      if (sizeList + 10 >= dbSize)
                      {
                          for (int i = sizeList; i < dbSize; i++)
                          {
                              Parks.Add(db.Parks.FirstOrDefault(p => p.Id == i));
                          }
                      }
                      else
                      {
                          for (int i = sizeList; i < sizeList + 10; i++)
                          {
                              Parks.Add(db.Parks.FirstOrDefault(p => p.Id == i));
                          }
                      }
                  },
                 (obj) => Parks.Count < dbSize));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}