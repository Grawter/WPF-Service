using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking.Helpers;
using Parking.Helpers.Interface;
using Parking.Models;

namespace Parking.ViewModels
{
    public class InformationViewModel : INotifyPropertyChanged
    {
        public InformationViewModel(IDialogService dialogService)
        {
            db = new ParkingdbContext();

            this.dialogService = dialogService;            
        }

        private readonly ParkingdbContext db;
        
        IDialogService dialogService;
   
        private string datanumber = "621",
                       api_key = System.Configuration.ConfigurationManager.AppSettings["ApiKey"];

        private string status;
        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        // команда перехода по ссылке
        private RelayCommand visitCommand;
        public RelayCommand VisitCommand
        {
            get
            {
                return visitCommand ??
                  (visitCommand = new RelayCommand(obj =>
                  {
                      if (obj != null)
                          Process.Start(new ProcessStartInfo { FileName = obj.ToString(), UseShellExecute = true });
                  }
                 ));
            }
        }

        // команда запроса к серверу и (пере)записи данных
        private RelayCommand requestCommand;
        public RelayCommand RequestCommand
        {
            get
            {
                return requestCommand ??
                  (requestCommand = new RelayCommand(async obj =>
                  {              
                      try
                      {
                          dialogService.ShowMessage("Отправка запроса к data.mos.ru...", 1);
                          
                          Status = "Получение ответа от data.mos.ru...";

                          string data = await Task.Run(() => Req.Resp($"https://apidata.mos.ru/v1/datasets/{datanumber}/rows/?api_key={api_key}"));
                          
                          var Deser_Obj = JConvert<List<Models.Parking>>.Deser(data);

                          Status = "Десериализация полученных данных...";

                          db.Parks.RemoveRange(db.Parks);

                          Status = "Удаление старых данных...";

                          foreach (var item in Deser_Obj)
                          {
                              await db.AddAsync(item.Cells);
                          }

                          await db.SaveChangesAsync();

                          Status = "Данные записаны";

                          MessageBoxResult result = (MessageBoxResult)dialogService.ShowMessage("Данные записаны. Для отображения результата требуется перезагрузка программы. Перезагрузить программу?", 4);

                          if (result == MessageBoxResult.Yes)
                          {
                              Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                              Application.Current.Shutdown();
                          }
                      }
                      catch (Exception ex)
                      {
                          Status = "Ошибка";
                          dialogService.ShowMessage(ex.Message, 3);

                          MessageBoxResult result = (MessageBoxResult)dialogService.ShowMessage("При попытке получения данных с сервера произошла ошибка. Считать данные из локального файла?", 4);

                          if (result == MessageBoxResult.Yes)                       
                              localfileCommand.Execute(null);
                                               
                      }
                  }
                 ));
            }
        }

        // команда запроса к файлу и (пере)записи данных
        private RelayCommand localfileCommand;
        public RelayCommand LocalFileCommand
        {
            get
            {
                return localfileCommand ??
                  (localfileCommand = new RelayCommand(async obj =>
                  {
                      try
                      {
                          if (dialogService.OpenFileDialog() == true)
                          {
                              string data = ReadWriteFile<ParkingInfo>.Read(dialogService.FilePath);

                              Status = "Считывание данных...";

                              List<ParkingInfo> Deser_Obj = JConvert<List<ParkingInfo>>.Deser(data);

                              Status = "Десериализация полученных данных...";

                              db.Parks.RemoveRange(db.Parks);

                              Status = "Удаление старых данных...";

                              foreach (var item in Deser_Obj)
                              {
                                  await db.AddAsync(item);
                              }

                              await db.SaveChangesAsync();

                              Status = "Данные записаны";

                              MessageBoxResult result = (MessageBoxResult)dialogService.ShowMessage("Данные записаны. Для отображения результата требуется перезагрузка программы. Перезагрузить программу?", 4);

                              if (result == MessageBoxResult.Yes)
                              {
                                  Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                                  Application.Current.Shutdown();
                              }    
                              
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message, 3);
                      }
                  }
                 ));
            }
        }

        // команда экспорта данных в .json
        private RelayCommand exportCommand;
        public RelayCommand ExportCommand
        {
            get
            {
                return exportCommand ??
                  (exportCommand = new RelayCommand(async obj =>
                  {
                      try
                      {
                          if (dialogService.SaveFileDialog() == true)
                          {
                              ReadWriteFile<List<ParkingInfo>>.Write(await db.Parks.ToListAsync(), dialogService.FilePath, 1);
                          }

                          Status = "Данные экспортированны в формат .json";

                          dialogService.ShowMessage("Данные экспортированны в формат .json", 1);
                      }
                      catch (Exception ex)
                      {
                          Status = "Ошибка";
                          dialogService.ShowMessage(ex.Message, 3);
                      }
                  }
                 ));
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