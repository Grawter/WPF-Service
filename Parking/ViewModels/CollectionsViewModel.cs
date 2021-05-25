using Parking.Helpers;

namespace Parking.ViewModels
{
    public class CollectionsViewModel
    {
        public ShowViewModel ShowViewModel { get; set; } = new ShowViewModel(new DefaultDialogService() );
        public InformationViewModel InformationViewModel { get; set; } = new InformationViewModel(new DefaultDialogService() );
    }
}