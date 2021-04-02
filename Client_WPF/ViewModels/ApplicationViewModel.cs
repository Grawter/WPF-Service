using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;
using Web_Service.Models;
using Client_WPF.Models;
using Client_WPF.Cache;

namespace Client_WPF.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel()
        {
            db = new ParkingdbContext();
            dbSize = db.Parks.Count();

            Parks = new ObservableCollection<ParkingInfo>();

            Cache = new Caching<ParkingInfo>();
            CacheList = new ObservableCollection<ParkingInfo>();
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
                if (value != null)
                {
                    var item = Cache.GetOrCreate(value.Id.ToString(), value);

                    if (!CacheList.Contains(item))
                        CacheList.Insert(0, item);
                }

                selectedPark = value;
                OnPropertyChanged("SelectedPark");
            }
        }

        private Caching<ParkingInfo> Cache { get; set; }
        public ObservableCollection<ParkingInfo> CacheList { get; set; }

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

        // команда удаления данных из кэша
        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??
                  (removeCommand = new RelayCommand(obj =>
                  {
                      ParkingInfo park = obj as ParkingInfo;

                      if (park != null)
                      {
                          CacheList.Remove(park);
                          Cache.Remove(park.Id.ToString());
                      }

                  },
                 (obj) => CacheList.Count > 0));
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