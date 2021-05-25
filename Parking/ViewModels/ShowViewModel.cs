using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Parking.Models;
using Parking.Helpers;
using Parking.Helpers.Interface;
using Parking.Cache;

namespace Parking.ViewModels
{
    public class ShowViewModel : INotifyPropertyChanged
    {
        public ShowViewModel(IDialogService dialogService)
        {
            db = new ParkingdbContext();
            
            dbSize = db.Parks.Count();

            Parks = new ObservableCollection<ParkingInfo>();

            Cache = new Caching<ParkingInfo>();
            CacheList = new ObservableCollection<ParkingInfo>();

            Favorites = JConvert<ObservableCollection<ParkingInfo>>.Deser(ReadWriteFile<ObservableCollection<ParkingInfo>>.Read("Favorites.json", true))
                ?? new ObservableCollection<ParkingInfo>();

            this.dialogService = dialogService;
        }

        private readonly ParkingdbContext db;
        
        public int dbSize { get; private set; } = 0;
        
        public ObservableCollection<ParkingInfo> Parks { get; set; }
        
        private Caching<ParkingInfo> Cache { get; set; }
        
        public ObservableCollection<ParkingInfo> CacheList { get; set; }

        public ObservableCollection<ParkingInfo> Favorites { get; set; }

        IDialogService dialogService;

        private ParkingInfo selectedPark;
        public ParkingInfo SelectedPark
        {
            get { return selectedPark; }
            set
            {
                if (value != null)
                {
                    if (value.Id != 0)
                    {
                        var item = Cache.GetOrCreate(value.Id.ToString(), value);

                        if (!CacheList.Contains(item))
                            CacheList.Insert(0, item);
                    }
                    else
                    {
                        CacheList.Remove(CacheList.FirstOrDefault(n => n.Id == 0));
                        CacheList.Insert(0, value);
                    }

                }

                selectedPark = value;
                OnPropertyChanged();
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

        // команда добавления данных в избранное
        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      ParkingInfo park = obj as ParkingInfo;

                      var copy = Favorites.FirstOrDefault(s => s.Id == park.Id);
                      
                      if (park != null && copy == null)
                      {
                          ReadWriteFile<List<ParkingInfo>>.Write(new List<ParkingInfo>() { park }, "Favorites.json", 2);
                      }
                      else if (park != null)
                      {
                          dialogService.ShowMessage("Выбранный вами элемент уже находится в избранном!", 2);
                      }

                  },
                  (obj) => CacheList.Count > 0));
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

                      if (park != null && park.Id != 0)
                      {
                          CacheList.Remove(park);
                          Cache.Remove(park.Id.ToString());
                      }
                      else if (park != null)
                          CacheList.Remove(park);

                  },
                 (obj) => CacheList.Count > 0));
            }
        }

        // команда обновления вкладки избранное
        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return updateCommand ??
                  (updateCommand = new RelayCommand(obj =>
                  {
                      List<ParkingInfo> temp_list = JConvert<List<ParkingInfo>>.Deser(ReadWriteFile<List<ParkingInfo>>.Read("Favorites.json", true));

                      if (temp_list != null)
                      {
                          if (Favorites.Count == 0)
                          {
                              foreach (var item in temp_list)
                              {
                                  Favorites.Insert(0, item);
                              }
                          }
                          else
                          {
                              bool found;
                              foreach (var itemT in temp_list)
                              {
                                  found = false;

                                  foreach (var itemD in Favorites)
                                  {
                                      if (itemD.Name == itemT.Name)
                                          found = true;
                                  }

                                  if (!found)
                                  {
                                      Favorites.Insert(0, itemT);
                                  }

                              }
                          }
                      }

                  }
                  ));
            }
        }

        // команда очистки избранного
        private RelayCommand clearCommand;
        public RelayCommand ClearCommand
        {
            get
            {
                return clearCommand ??
                  (clearCommand = new RelayCommand(obj =>
                  {
                      ReadWriteFile<object>.Write(null, "Favorites.json", 3);
                      Favorites.Clear();

                  },
                  (obj) => Favorites.Count > 0));
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